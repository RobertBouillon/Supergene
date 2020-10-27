using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace System.Diagnostics.UnitTesting
{
  public class TypeTestProfile
  {
    #region Fields
    private Type _type;
    private bool _enabled = true;
    private MethodInfo _createInstanceMethod;
    private MethodInfo _testInstanceMethod;
    private MethodInfo _destroyInstanceMethod;
    private MethodInfo _testStaticMethod;
    private List<Type> _dependencies = new List<Type>();
    #endregion

    #region Constructors

    public bool Enabled
    {
      get
      {
        return (HasTestMethods && _enabled);
      }
      set { _enabled = value; }
    }

    public MethodInfo CreateInstanceMethod
    {
      get { return _createInstanceMethod; }
      set { _createInstanceMethod = value; }
    }

    public MethodInfo TestInstanceMethod
    {
      get { return _testInstanceMethod; }
      set { _testInstanceMethod = value; }
    }

    public MethodInfo DestroyInstanceMethod
    {
      get { return _destroyInstanceMethod; }
      set { _destroyInstanceMethod = value; }
    }

    public MethodInfo TestStaticMethod
    {
      get { return _testStaticMethod; }
      set { _testStaticMethod = value; }
    }

    public List<Type> Dependencies
    {
      get { return _dependencies; }
    }

    public bool CanTestStatic
    {
      get { return _testStaticMethod != null; }
    }

    public bool CanDestroyInstance
    {
      get { return _destroyInstanceMethod != null; }
    }

    public bool CanTestInstance
    {
      get { return _testInstanceMethod != null; }
    }

    public bool CanCreateInstance
    {
      get { return _createInstanceMethod != null; }
    }

    public Type Type
    {
      get { return _type; }
      set { _type = value; }
    }
    #endregion

    #region Constructors
    public TypeTestProfile(Type type)
    {
      #region Validation
      if (type == null)
        throw new ArgumentNullException("type");
      #endregion
      _type = type;

      GetMethods();
      CalculateDependencies();
    }

    private void GetMethods()
    {
        List<MethodInfo> _taggedMethods = new List<MethodInfo>(4);

        //Find all methods in the class marked with the UnitTest attribute
        foreach (MemberInfo mi in _type.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, null, null))
          if (Attribute.IsDefined(mi, typeof(UnitTestAttribute)))
            _taggedMethods.Add((MethodInfo)mi);

        _testInstanceMethod = GetUnitTestMethod(TestMethodType.TestInstance, _taggedMethods);
        _createInstanceMethod = GetUnitTestMethod(TestMethodType.CreateInstance, _taggedMethods);
        _destroyInstanceMethod = GetUnitTestMethod(TestMethodType.DestroyInstance, _taggedMethods);
        _testStaticMethod = GetUnitTestMethod(TestMethodType.TestStatic, _taggedMethods);
    }

    private MethodInfo GetUnitTestMethod(TestMethodType type, List<MethodInfo> _taggedMethods)
    {
      foreach (MethodInfo mi in _taggedMethods)
        if ((mi.GetCustomAttributes(typeof(UnitTestAttribute), false)[0] as UnitTestAttribute).Type == type)
          return mi;

      return null;
    }

    private void CalculateDependencies()
    {
      List<ParameterInfo> parms = new List<ParameterInfo>();

      if (_createInstanceMethod != null)
        parms.AddRange(_createInstanceMethod.GetParameters());
      if (_testStaticMethod!= null)
        parms.AddRange(_testStaticMethod.GetParameters());

      foreach (ParameterInfo pi in parms)
      {
        if (pi.ParameterType == _type)
          throw new Exception(String.Format("Recursive dependancy found. Please create a static member called TestInstance in class '{0}' of type '{0}' to use for testing", _type.ToString()));
        _dependencies.Add(pi.ParameterType);
      }
    }
    #endregion

    #region Public Methods
    public bool HasDependencies(TypeTestProfileCollection profiles)
    {
      foreach (Type dependencytype in _dependencies)
        if (!profiles.Contains(dependencytype))
          return false;
        else
          if (!profiles[dependencytype].CanCreateInstance)
            return false;

      return true;
    }

    //public bool HasDependencies(Type[] search)
    //{
    //  foreach (Type t in _dependencies)
    //  {
    //    bool found = false;
    //    foreach (Type st in search)
    //    {
    //      if (st == null)
    //        continue;
    //      if (st == t)
    //      {
    //        found = true;
    //        break;
    //      }
    //    }
    //    if (!found)
    //      return false;
    //  }
    //  return true;
    //}

    public bool HasTestMethods
    {
      get
      {
        return CanCreateInstance || CanDestroyInstance || CanTestInstance || CanTestStatic;
      }
    }
    #endregion

    internal void TestStatic(TypeTestResult result, TypeTestResultCollection results)
    {
      try
      {
        TestStaticMethod.Invoke(null, GetDependencies(_testStaticMethod,results));
        result.TestStaticException = null;
      }
      catch (Exception ex)
      {
        result.TestStaticException = ex;
      }
    }

    internal void TestInstance(TypeTestResult result)
    {
      try
      {
        TestInstanceMethod.Invoke(null, new object[] { result.ObjectInstance });
        result.TestInstanceException = null;
      }
      catch (Exception ex)
      {
        result.TestInstanceException = ex;
      }
    }

    internal void CreateInstance(TypeTestResult result, TypeTestResultCollection results)
    {
      try
      {
        result.ObjectInstance = CreateInstanceMethod.Invoke(null, GetDependencies(_createInstanceMethod, results));
        result.CreateInstanceException = null;
      }
      catch (Exception ex)
      {
        result.CreateInstanceException = ex;
      }
    }

    private object[] GetDependencies(MethodInfo source, TypeTestResultCollection results)
    {
      List<Type> dependencies = new List<Type>();

      foreach (ParameterInfo p in source.GetParameters())
        dependencies.Add(p.ParameterType);

      //Get the objects that we need
      object[] ret = new object[dependencies.Count];

      for (int i = 0; i < dependencies.Count; i++)
      {
        if(!results.Contains(dependencies[i]))
          throw new UnitTestException(String.Format("Execution Error: {0} requires type {1}, which is not available for tests (No unit test methods).", _type.ToString(), dependencies[i].ToString()));

        TypeTestResult result = results[dependencies[i]];

        if (result.ObjectInstance == null)
          throw new UnitTestException(String.Format("Execution Order Error: {0} requires type {1}, which has not been created", _type.ToString(), result.Profile.Type.ToString()));

        ret[i] = result.ObjectInstance;
      }

      return ret;
    }

    internal void DestroyInstance(TypeTestResult result)
    {
      try
      {
        DestroyInstanceMethod.Invoke(null, new object[] { result.ObjectInstance });
        result.DestroyInstanceException = null;
      }
      catch (Exception ex)
      {
        result.DestroyInstanceException = ex;
      }
    }
  }
}
