using System;

namespace System.Diagnostics
{
  #region Enums
  #endregion
	/// <summary>
	/// A single item returned by a Unit Test
	/// </summary>
	public class UnitTestResultItem
	{
    #region Private Property Declarations
    private string p_Message;
    //private DateTime p_MessageTime;
    private Exception p_Exception;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The message sent to the user regaring the class
    /// </summary>
    public string Message
    {
      get{return p_Message;}
      set{p_Message = value;}
    }

    /// <summary>
    /// The the item is an error, this contains the exception that was thrown.
    /// </summary>
    public Exception Exception
    {
      get{return p_Exception;}
      set{p_Exception = value;}
    }

    #endregion
    #region ctors
    /// <summary>
    /// A single item returned by a Unit Test
    /// </summary>
    public UnitTestResultItem(string message, Exception innerException)
		{
      p_Exception = innerException;
      p_Message = message;
		}
    #endregion
	}
}
