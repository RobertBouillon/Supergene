using System;
using System.IO;

namespace System.Diagnostics
{
	/// <summary>
	/// Desingates a class as a Unit for Unit Testing Purposes.
	/// </summary>
	public interface IUnit
	{
    /// <summary>
    /// Override to allow an external program to test the class
    /// </summary>
    /// <param name="output">The stream to write raw trace information to.</param>
    /// <param name="verbose">True if the output should be verbose</param>
    /// <returns></returns>
    UnitTestResult UnitTest(ref Stream output, bool verbose);
	}
}
