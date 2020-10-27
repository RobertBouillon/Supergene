using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.UnitTesting;

namespace System.Diagnostics.UnitTesting
{
  public class ValidationError
  {
    #region Fields
    private TypeTestProfile _profile;
    private string _error;
    #endregion
    #region Properties

    public string Error
    {
      get { return _error; }
      set { _error = value; }
    }

    public TypeTestProfile Profile
    {
      get { return _profile; }
      set { _profile = value; }
    }

    #endregion
    #region Constructors
    public ValidationError()
    {

    }

    public ValidationError(TypeTestProfile profile, string error)
    {
      #region Validation
      if (profile == null)
        throw new ArgumentNullException("profile");
      if (error == null)
        throw new ArgumentNullException("error");
      #endregion
      _profile = profile;
      _error = error;
    }
    #endregion
  }
}
