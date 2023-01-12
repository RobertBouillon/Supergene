using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading;


[global::System.Serializable]
public class AsyncOperationException : Exception
{
  //
  // For guidelines regarding the creation of new exception types, see
  //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
  // and
  //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
  //

  #region Constructors
  public AsyncOperationException() { }
  public AsyncOperationException(string message) : base(message) { }
  public AsyncOperationException(string message, Exception innerException) : base(message, innerException) { }
  protected AsyncOperationException(
  System.Runtime.Serialization.SerializationInfo info,
  System.Runtime.Serialization.StreamingContext context)
    : base(info, context) { }
  #endregion
}
