using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

[Serializable]
public class FormattedException : Exception
{
  #region Fields
  private FormattedString _formattedString;
  #endregion
  #region Properties
  public FormattedString FormattedString
  {
    get { return _formattedString; }
    set { _formattedString = value; }
  }
  #endregion

  #region Constructors
  public FormattedException()
  {
  }
  public FormattedException(string message)
    : base(message)
  {
    _formattedString = new FormattedString(message);
  }
  public FormattedException(string message, params object[] args)
    : base(String.Format(message, args))
  {
    _formattedString = new FormattedString(message, args);
  }
  public FormattedException(string message, Exception inner) : base(message, inner) { }
  public FormattedException(string message, Exception inner, params object[] args)
    : base(String.Format(message, args), inner)
  {
    _formattedString = new FormattedString(message, args);
  }
  protected FormattedException(
    Runtime.Serialization.SerializationInfo info,
    Runtime.Serialization.StreamingContext context) : base(info, context) { }
  #endregion
}
