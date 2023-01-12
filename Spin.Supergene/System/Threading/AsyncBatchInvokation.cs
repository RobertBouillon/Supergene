using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading;

public class AsyncBatchInvokation : AsyncOperation
{
  #region Fields
  private AsyncInvokation[] _invokations;
  private ExceptionCollection _exceptions = new ExceptionCollection();
  private int _completedOperations = 0;
  private AsyncOperationCallback _callback;
  private List<AsyncOperation> _operations = new List<AsyncOperation>();
  private SynchronizationContext _callbackContext;
  #endregion

  #region Constructors
  public AsyncBatchInvokation(AsyncOperationCallback callback, params AsyncInvokation[] invokations)
  {
    _invokations = invokations;
    _callback = callback;
    _callbackContext = SynchronizationContext.Current;
    AsyncOperationCallback internalcallback = new AsyncOperationCallback(InternalCallback);


    foreach (AsyncInvokation method in invokations)
      _operations.Add(method(internalcallback));
  }
  #endregion

  #region Private Methods
  private void InternalCallback(AsyncOperation op)
  {
    lock (this)
    {
      if (op.Result == AsyncOperationResult.Error)
        _exceptions.Add(op.Error);


      if (++_completedOperations == _invokations.Length)
      {
        if (_exceptions.Count > 0)
        {
          CompleteOperation(AsyncOperationResult.Error);
          return;
        }

        foreach (AsyncOperation cop in _operations)
        {
          if (cop.Result == AsyncOperationResult.Cancelled)
          {
            CompleteOperation(AsyncOperationResult.Cancelled);
            return;
          }
        }

        CompleteOperation(AsyncOperationResult.Completed);
      }
    }
  }

  private void SetCascadingResult(AsyncOperationResult result)
  {
  }
  #endregion

  #region Overrides
  public override void Cancel(bool waitForCompletion)
  {
    foreach (AsyncOperation op in _operations)
      op.Cancel();

    if (waitForCompletion)
      WaitForCompletion();
  }

  public override AsyncOperationResult WaitForCompletion(TimeSpan timeout)
  {
    WaitHandle[] handles = new WaitHandle[_operations.Count];

    for (int i = 0; i < _operations.Count; i++)
      handles[i] = _operations[i].WaitHandle;

    CompoundWaitHandle wait = new CompoundWaitHandle(handles);
    wait.WaitOne();

    return Result;
  }

  public override Exception Error
  {
    get
    {
      if (_exceptions.Count > 0)
        return new AsyncBatchException(new ExceptionCollection(_exceptions));
      else
        return null;
    }
    protected set
    {
      throw new NotSupportedException();
    }
  }

  protected override void CompleteOperation(AsyncOperationResult result)
  {
    base.CompleteOperation(result);
    if (_callbackContext != SynchronizationContext.Current)
      _callbackContext.Post(new SendOrPostCallback(delegate (object state) { _callback(this); }), null);
    else
      _callback(this);
  }
  #endregion
}
