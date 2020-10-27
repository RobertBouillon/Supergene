using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading
{
  public class AsyncBatchException : Exception
  {
    #region Fields
    private ExceptionCollection _exceptions;

    public ExceptionCollection Exceptions
    {
      get { return _exceptions; }
    }
    #endregion

    #region Constructors
    public AsyncBatchException(ExceptionCollection exceptions)
    {
      #region Validation
      if (exceptions == null)
        throw new ArgumentNullException("exceptions");
      #endregion
      _exceptions = exceptions;
    }
    public AsyncBatchException(ExceptionCollection exceptions, string message)
      : base(message)
    {
      #region Validation
      if (exceptions == null)
        throw new ArgumentNullException("exceptions");
      #endregion
      _exceptions = exceptions;
    }
    public AsyncBatchException(ExceptionCollection exceptions, string message, Exception innerException)
      : base(message, innerException)
    {
      #region Validation
      if (exceptions == null)
        throw new ArgumentNullException("exceptions");
      #endregion
      _exceptions = exceptions;
    }
    #endregion
  }
}
