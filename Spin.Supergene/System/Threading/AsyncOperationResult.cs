using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading
{
  public enum AsyncOperationResult
  {
    /// <summary>
    /// The operation has not been started 
    /// </summary>
    /// <remarks>NOT SUPPORTED. The class does not support events, and this is used to enforce that constraint.</remarks>
    //Idle,
    
    /// <summary>
    /// The operation has been started and is pending
    /// </summary>
    Pending,

    /// <summary>
    /// The operation completed successfully
    /// </summary>
    Completed,

    /// <summary>
    /// The operation was cancelled
    /// </summary>
    Cancelled,

    /// <summary>
    /// The operation did not complete successfully
    /// </summary>
    Error,

    /// <summary>
    /// The operation timed out
    /// </summary>
    /// <remarks>This will only be returned by the WaitForCompletion method</remarks>
    Timeout
  }
}
