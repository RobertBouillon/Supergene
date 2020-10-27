using System;

namespace System
{
	/// <summary>
	/// Occurs when a an object has timed out
	/// </summary>
	public class TimeoutException : IO.IOException
	{
    #region Private Property Declarations
    private TimeSpan p_Threshold;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The amount of time that was allotted to the device that timed out.
    /// </summary>
    public TimeSpan Threshold
    {
      get{return p_Threshold;}
      set{p_Threshold = value;}
    }
    #endregion

    #region ctors
		public TimeoutException(TimeSpan threshold)
		{
      p_Threshold = threshold;
    }

    public TimeoutException(string message, TimeSpan threshold) : base(message)
    {
      p_Threshold = threshold;
    }

    public TimeoutException(string message, Exception innerException, TimeSpan threshold) : base(message, innerException)
    {
      p_Threshold = threshold;
    }
    #endregion
	}
}
