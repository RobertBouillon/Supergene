using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public class FormattedString
  {
    #region Fields
    private string _formatText;
    private object[] _arguments;
    #endregion

    #region Properties

    public string FormatText
    {
      get { return _formatText; }
      set { _formatText = value; }
    }

    public object[] Arguments
    {
      get { return _arguments; }
      set { _arguments = value; }
    }

    #endregion

    #region Constructors
    public FormattedString(string formatText, params object[] arguments)
    {
      #region Validation
      if (formatText == null)
        throw new ArgumentNullException("formatText");
      if (arguments == null)
        throw new ArgumentNullException("arguments");
      #endregion
      _formatText = formatText;
      _arguments = arguments;
    }
    #endregion

  }
}
