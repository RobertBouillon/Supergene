using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel;

public class ProgressTextChangedEventArgs : EventArgs
{
  #region Private Members
  private object _userState;
  private string _progressText;
  #endregion

  #region Public Property Declarations
  public object UserState
  {
    get { return _userState; }
  }

  public string ProgressText
  {
    get { return _progressText; }
  }
  #endregion

  #region Constructors
  public ProgressTextChangedEventArgs(string progressText, object userState)
  {
    #region Validation
    if (progressText == null)
      throw new ArgumentNullException("progressText");
    #endregion

    _progressText = progressText;
    _userState = userState;
  }
  #endregion
}
