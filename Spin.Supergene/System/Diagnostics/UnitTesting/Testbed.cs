using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace System.Diagnostics.UnitTesting
{
  public class Testbed
  {
    #region Fields
    private TypeTestProfileCollection _testProfiles = new TypeTestProfileCollection();
    private MultipassExecutionOrder _executionOrder;
    private ValidationErrorCollection _validationErrors = new ValidationErrorCollection();
    private bool _multithreadedExecution = true;
    private bool _isExecuting = false;

    private TypeTestProfileCollection _disabledDependencies;
    private Dictionary<TypeTestProfile, TypeTestResult> _testResults = new Dictionary<TypeTestProfile, TypeTestResult>();
    #endregion
    #region Properties
    public bool IsExecuting
    {
      get { return _isExecuting; }
    }

    public bool MultithreadedExecution
    {
      get { return _multithreadedExecution; }
      set { _multithreadedExecution = value; }
    }

    public TypeTestProfileCollection TestProfiles
    {
      get { return _testProfiles; }
    }

    public ValidationErrorCollection ValidationErrors
    {
      get { return _validationErrors; }
      set { _validationErrors = value; }
    }
    #endregion

    #region Constructors
    public Testbed() : this(Assembly.GetExecutingAssembly())
    {

    }

    public Testbed(Assembly target)
    {
      foreach (Type t in target.GetTypes())
        _testProfiles.Add(new TypeTestProfile(t));

      ValidateUnitTestSignatures();
      GetExecutionOrder();
    }
    #endregion

    private void TestProfile(TypeTestProfile profile)
    {
      if (!profile.Enabled)
        if (!_disabledDependencies.Contains(profile))
          return;

      if (!profile.HasTestMethods)
        return;


      TypeTestResult result = new TypeTestResult(profile);
      lock (_testResults)
        _testResults.Add(profile, result);

      if (profile.CanCreateInstance)
      {
        bool retry = false;
        do
        {
          OnTypeTestStarted(TestMethodType.CreateInstance, profile);
          profile.CreateInstance(result, new TypeTestResultCollection(_testResults.Values));
          retry = OnTypeTestCompleted(TestMethodType.CreateInstance, profile, result.CreateInstanceException).Retry;
        } while (retry);
      }

      if (!result.HasErrors)
      {
        if (profile.CanTestInstance)
        {
          bool retry = false;
          do
          {
            OnTypeTestStarted(TestMethodType.TestInstance, profile);
            profile.TestInstance(result);
            retry = OnTypeTestCompleted(TestMethodType.TestInstance, profile, result.TestInstanceException).Retry;
          } while (retry);
        }
      }
    }

    private void TestStaticProfile(TypeTestProfile profile)
    {
      if (!profile.Enabled)
        return;

      if (!profile.CanTestStatic)
        return;


      TypeTestResult result = _testResults[profile];

      bool retry = false;
      do
      {
        OnTypeTestStarted(TestMethodType.TestStatic, profile);
        profile.TestStatic(result, new TypeTestResultCollection(_testResults.Values));
        retry = OnTypeTestCompleted(TestMethodType.TestStatic, profile, result.TestStaticException).Retry;
      } while (retry);
    }

    private void DestroyProfile(TypeTestProfile profile)
    {
      if (!profile.Enabled)
        if (!_disabledDependencies.Contains(profile))
          return;

      if (!profile.HasTestMethods)
        return;

      TypeTestResult result = _testResults[profile];

      if (result.ObjectInstance == null)
        return;

      if (profile.CanDestroyInstance)
      {
        bool retry = false;
        do
        {
          OnTypeTestStarted(TestMethodType.DestroyInstance, profile);
          profile.DestroyInstance(result);
          retry = OnTypeTestCompleted(TestMethodType.DestroyInstance, profile, result.DestroyInstanceException).Retry;
        } while (retry);
      }
    }

    #region Public Methods
    public void Execute()
    {
      Execute(true);
    }

    public void Execute(bool multiThreaded)
    {
      if (_validationErrors.Count > 0)
        throw new UnitTestException("Cannot execute test while validation errors exist.");

      if (_isExecuting)
        throw new InvalidOperationException("Cannot execute test. Test is already executing");

      _isExecuting = true;
      UnitTest.IsTesting = true;

      try
      {
        _testResults = new Dictionary<TypeTestProfile, TypeTestResult>();
        _disabledDependencies = _executionOrder.GetDisabledDependencies();

        //Create objects and test

        ExecuteBatch(multiThreaded, TestProfile);
        ExecuteBatch(multiThreaded, TestStaticProfile);
        ExecuteBatch(multiThreaded, DestroyProfile, true);
      }
      finally
      {
        _isExecuting = false;
        UnitTest.IsTesting = false;
      }
    }


    private void ExecuteBatch(bool multiThreaded, Action<TypeTestProfile> action)
    {
      ExecuteBatch(multiThreaded, action, false);
    }

    private void ExecuteBatch(bool multiThreaded, Action<TypeTestProfile> action, bool reverse)
    {
      IEnumerable<TypeTestProfileCollection> passes = _executionOrder.Passes;
      if(reverse)
        passes = new ReverseEnumerator<TypeTestProfileCollection>(_executionOrder.Passes);

      foreach (TypeTestProfileCollection pass in passes)
      {
        if (multiThreaded)
          AsyncBatchOperation.BatchProcess<TypeTestProfile>(pass, action).WaitForCompletion();
        else
          pass.ForEach(action);
      }
    }
    #endregion

    #region Private Methods
    private bool ValidateUnitTestSignatures()
    {
      bool valid = true;
      foreach(TypeTestProfile profile in _testProfiles)
      {
        if (!(
        ValidateUnitTestSignature(TestMethodType.TestStatic, profile.TestStaticMethod, profile) ||
        ValidateUnitTestSignature(TestMethodType.TestInstance, profile.TestInstanceMethod, profile) ||
        ValidateUnitTestSignature(TestMethodType.DestroyInstance, profile.DestroyInstanceMethod, profile) ||
        ValidateUnitTestSignature(TestMethodType.CreateInstance, profile.CreateInstanceMethod, profile)
          ))
          valid = false;
      }
      return valid;
    }

    public void GetExecutionOrder()
    {
      _executionOrder = new MultipassExecutionOrder(_testProfiles);
      _executionOrder.ValidationErrors.CopyTo(_validationErrors);
    }

    private bool ValidateUnitTestSignature(TestMethodType type, MethodInfo method, TypeTestProfile profile)
    {
      if (method == null)
        return true;

      ParameterInfo[] pi;
      switch (type)
      {
        case TestMethodType.CreateInstance:
          if (method.ReturnType != method.ReflectedType)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test method does not return the object type as required.", method.Name)));
            return false;
          }
          break;
        case TestMethodType.TestStatic:
          if (method.ReturnType != typeof(void))
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test method must return void.", method.Name)));
            return false;
          }
          break;
        case TestMethodType.TestInstance:
          pi = method.GetParameters();
          if (pi.Length < 1)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that an instance of the type '{1}' be supplied as an argument.", method.Name, method.ReflectedType)));
            return false;
          }
          if (pi.Length > 1)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that only an instance of the type '{1}' be supplied as an argument. {2} arguments were supplied.", method.Name, method.ReflectedType, pi.Length)));
            return false;
          }
          if (pi[0].ParameterType != method.ReflectedType)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that only an instance of the type '{1}' be supplied as an argument. {2} arguments were supplied.", method.Name, method.ReflectedType, pi.Length)));
            return false;
          }
          break;
        case TestMethodType.DestroyInstance:
          pi = method.GetParameters();
          if (pi.Length < 1)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that an instance of the type '{1}' be supplied as an argument.", method.Name, method.ReflectedType)));
            return false;
          }
          if (pi.Length > 1)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that only an instance of the type '{1}' be supplied as an argument. {2} arguments were supplied.", method.Name, method.ReflectedType, pi.Length)));
            return false;
          }
          if (pi[0].ParameterType != method.ReflectedType)
          {
            _validationErrors.Add(new ValidationError(profile, String.Format("The method '{0}' marked as a Unit Test requires that only an instance of the type '{1}' be supplied as an argument. {2} arguments were supplied.", method.Name, method.ReflectedType, pi.Length)));
            return false;
          }
          break;
        default:
          throw new Exception(String.Format("Unknown TestMethodType: '{0}'", type));
      }
      return true;
    }
    #endregion

    #region Events

    #region TypeTestStartedEventArgs Subclass
    public class TypeTestStartedEventArgs : EventArgs
    {
      #region Fields
      private readonly TypeTestProfile _profile;
      private readonly TestMethodType _type;
      #endregion
      #region Properties
      public TypeTestProfile Profile
      {
        get { return _profile; }
      }

      public TestMethodType Type
      {
        get { return _type; }
      }

      #endregion
      #region Constructors
      internal TypeTestStartedEventArgs(TestMethodType type, TypeTestProfile profile)
      {
        #region Validation
        if (profile == null)
          throw new ArgumentNullException("profile");
        #endregion
        _profile = profile;
        _type = type;
      }
      #endregion
    }
    #endregion

    public event EventHandler<TypeTestStartedEventArgs> TypeTestStarted;

    protected void OnTypeTestStarted(TestMethodType type, TypeTestProfile profile)
    {
      OnTypeTestStarted(new TypeTestStartedEventArgs(type, profile));
    }

    protected virtual void OnTypeTestStarted(TypeTestStartedEventArgs e)
    {
      if (TypeTestStarted != null)
        TypeTestStarted(this, e);
    }



    public class TypeTestCompletedEventArgs : EventArgs
    {
      #region Fields
      private readonly TestMethodType _type;
      private readonly TypeTestProfile _profile;
      private readonly bool _success;
      private readonly Exception _error;
      private bool _retry = false;
      #endregion
      #region Properties

      public bool Retry
      {
        get { return _retry; }
        set { _retry = value; }
      }

      public Exception Error
      {
        get { return _error; }
      } 

      public bool Success
      {
        get { return _success; }
      } 

      public TestMethodType Type
      {
        get { return _type; }
      } 

      public TypeTestProfile Profile
      {
        get { return _profile; }
      } 

      #endregion
      #region Constructors
      public TypeTestCompletedEventArgs(TestMethodType type, TypeTestProfile profile, Exception errorMessage)
      {
        #region Validation
        if (type == null)
          throw new ArgumentNullException("type");
        if (profile == null)
          throw new ArgumentNullException("profile");
        #endregion
        _type = type;
        _profile = profile;
        _success = errorMessage==null;
        _error = errorMessage;
      }
      #endregion
    }

    public event EventHandler<TypeTestCompletedEventArgs> TypeTestCompleted;

    protected TypeTestCompletedEventArgs OnTypeTestCompleted(TestMethodType type, TypeTestProfile profile, Exception error)
    {
      TypeTestCompletedEventArgs args = new TypeTestCompletedEventArgs(type, profile, error);
      OnTypeTestCompleted(args);
      return args;
    }

    protected virtual void OnTypeTestCompleted(TypeTestCompletedEventArgs e)
    {
      if (TypeTestCompleted != null)
        TypeTestCompleted(this, e);
    }

    #endregion

    public int GetTypeTestCount()
    {
      int count = 0;
      foreach (TypeTestProfile profile in from p in _testProfiles where p.Enabled select p)
      {
        if (profile.CanCreateInstance)
          count++;
        if (profile.CanDestroyInstance)
          count++;
        if (profile.CanTestInstance)
          count++;
        if (profile.CanTestStatic)
          count++;
      }

      return count;
    }
  }
}
