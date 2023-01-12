using System;

namespace System;

/// <summary>
/// An attempt to make a duplicate index failed.
/// </summary>
public class DuplicateIndexException : Exception
{
  public DuplicateIndexException()
  {
  }

  public DuplicateIndexException(string message) : base(message)
  { }

  public DuplicateIndexException(string message, Exception innerException) : base(message, innerException)
  { }
}
