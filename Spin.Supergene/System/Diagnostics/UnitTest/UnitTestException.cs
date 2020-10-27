using System;

namespace System.Diagnostics.UnitTesting
{
	/// <summary>
	/// Summary description for UnitTestException.
	/// </summary>
	public class UnitTestException : Exception
	{
		public UnitTestException()
		{}

    public UnitTestException(string message):base(message)
    {}

    public UnitTestException(string message, Exception innerException) : base(message,innerException)
    {}
	}
}
