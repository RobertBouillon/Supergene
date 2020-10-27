using System;

namespace System.Diagnostics
{
	/// <summary>
	/// The result of a Unit Test
	/// </summary>
	public class UnitTestResult
	{
    #region Private Property Declarations
    private bool p_Success;
    private UnitTestResultItems p_Items = new UnitTestResultItems();
    private Exception p_Exception;
    #endregion

    #region Public Property Declarations
    /// <summary>
    /// True the the unit test executed successfully.
    /// </summary>
    public bool Success
    {
      get{return p_Success;}
      set{p_Success = value;}
    }

    /// <summary>
    /// The messages returned from the class regarding the Unit Test Execution.
    /// </summary>
    public UnitTestResultItems Items
    {
      get{return p_Items;}
      set{p_Items = value;}
    }

    public Exception Exception
    {
      get{return p_Exception;}
      set{p_Exception = value;}
    }

    #endregion
		public UnitTestResult()
		{
		}
	}
}
