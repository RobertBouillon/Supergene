
using System;

namespace System.IO
{
	/// <summary>
	/// Summary description for RapiException.
	/// </summary>
	public class RapiException : IOException
	{
    #region Private Variable Declarations
    private int p_ErrorNumber;
    #endregion
    #region Public Variable Declarations
    /// <summary>
    /// The error number received from the CeGetLastError Function
    /// </summary>
    public int ErrorNumber
    {
      get{return p_ErrorNumber;}
      set{p_ErrorNumber = value;}
    }
    #endregion
    public RapiException() : this("")
    {}

    public RapiException(string message) : this(RAPI.CeGetLastError(), message)
    {}

    public RapiException(string message, Exception innerException) : this(RAPI.CeGetLastError(),"",innerException)
    {}

    public RapiException(int errorNum) : this(errorNum,"")
    {}

    public RapiException(int errorNum, string message) : base(message + "\n\nRAPI Error #" + errorNum.ToString() + ": " + ParseError(errorNum))
    {
      p_ErrorNumber = errorNum;
    }

    public RapiException(int errorNum, string message, Exception innerExcpetion) : base(message + "\n\nRAPI Error #" + errorNum.ToString() + ": " + ParseError(errorNum) + "\n", innerExcpetion)
    {
      p_ErrorNumber = errorNum;
    }

    public static string ParseError(int errorNum)
    {
      string err = ((Microsoft.WinCE.CeBase.CeError) errorNum).ToString();
      return err;
    }
  }
}
