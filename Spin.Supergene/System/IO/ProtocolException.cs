using System;

namespace System.IO
{
	/// <summary>
	/// Thrown when a binary stream sends information that violates a stated protocol.
	/// </summary>
	public class ProtocolException : IOException
	{
		public ProtocolException()
		{}

    public ProtocolException(string message) : base(message)
    {}

    public ProtocolException(string message, Exception innerException) : base(message,innerException)
    {}
	}
}
