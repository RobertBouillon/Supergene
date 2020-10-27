using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.UnitTesting
{
  public class TypeTestResult
  {
    #region Fields
    private Exception _createInstanceException;
    private Exception _testInstanceException;
    private Exception _destroyInstanceException;
    private Exception _testStaticException;
    private TypeTestProfile _profile;
    private object _objectInstance;
    #endregion
    #region Properties

    public object ObjectInstance
    {
      get { return _objectInstance; }
      internal set { _objectInstance = value; }
    }

    public TypeTestProfile Profile
    {
      get { return _profile; }
    }

    public Exception CreateInstanceException
    {
      get { return _createInstanceException; }
      internal set { _createInstanceException = value; }
    }

    public Exception TestInstanceException
    {
      get { return _testInstanceException; }
      internal set { _testInstanceException = value; }
    }

    public Exception DestroyInstanceException
    {
      get { return _destroyInstanceException; }
      internal set { _destroyInstanceException = value; }
    }

    public Exception TestStaticException
    {
      get { return _testStaticException; }
      internal set { _testStaticException = value; }
    }

    public bool HasErrors
    {
      get
      {
        return (
          _createInstanceException != null ||
          _testInstanceException != null ||
          _destroyInstanceException != null ||
          _testStaticException != null
        );
      }
    }

    #endregion
    #region Constructors
    public TypeTestResult(TypeTestProfile profile)
    {
      #region Validation
      if (profile == null)
        throw new ArgumentNullException("profile");
      #endregion
      _profile = profile;
    }
    #endregion


  }
}
