using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Threading
{
  /// <summary>
  /// Provides access to manipulate an asynchronous operation
  /// </summary>
  /// <remarks>
  /// This is a handle to a single async operation, and cannot be reused.
  /// </remarks>
  public class AsyncOperation
  {
    #region Fields
    private ManualResetEvent _waitHandle = new ManualResetEvent(false);
    private AsyncOperationResult _result = AsyncOperationResult.Pending;
    private Exception _error;
    private SynchronizationContext _context;
    private Delegate _worker;
    private Delegate[] _callbacks;
    private object _state;
    #endregion

    #region Properties
    protected Delegate Worker
    {
      get { return _worker; }
    }

    protected Delegate[] Callbacks
    {
      get { return _callbacks; }
    }

    protected SynchronizationContext Context
    {
      get { return _context; }
    }

    protected internal virtual WaitHandle WaitHandle
    {
      get { return _waitHandle; }
    }

    public virtual Exception Error
    {
      get { return _error; }
      protected set { _error = value; }
    }

    public virtual AsyncOperationResult Result
    {
      get { return _result; }
    }
    #endregion
    #region Constructors
    protected AsyncOperation()
    {

    }

    protected AsyncOperation(Delegate worker)
    {
      #region Validation
      if (worker == null)
        throw new ArgumentNullException("worker");
      #endregion
      _worker = worker;
      _context = SynchronizationContext.Current;
    }

    protected AsyncOperation(Delegate worker, params Delegate[] callbacks)
    {
      #region Validation
      if (worker == null)
        throw new ArgumentNullException("worker");
      if (callbacks == null)
        throw new ArgumentNullException("callback");
      #endregion
      _worker = worker;
      _callbacks = callbacks;
      _context = SynchronizationContext.Current;
    }

    protected AsyncOperation(SynchronizationContext context, Delegate worker, params Delegate[] callbacks)
    {
      #region Validation
      if (worker == null)
        throw new ArgumentNullException("worker");
      if (callbacks == null)
        throw new ArgumentNullException("callback");
      if (context == null)
        throw new ArgumentNullException("context");
      #endregion
      _worker = worker;
      _callbacks = callbacks;
      _context = context;
    }
    #endregion

    #region Methods
    #region Private Methods
    protected virtual void Start()
    {
      ThreadPool.QueueUserWorkItem(new WaitCallback(InternalWorker));
    }

    protected virtual void InternalWorker(object state)
    {
      object[] empty = new object[] { };
      try
      {
        if (_worker is EmptyMethod)
          _worker.DynamicInvoke(empty);
        else
          _state = _worker.DynamicInvoke(empty);

        if (Result == AsyncOperationResult.Cancelled)
          return;

        if (Result != AsyncOperationResult.Cancelled)
          CompleteOperation(AsyncOperationResult.Completed);

      }
      catch (Exception ex)
      {
        if (Result != AsyncOperationResult.Cancelled)
          CompleteOperation(ex);
        return;
      }
      finally
      {
        if (_callbacks != null)
        {
          ExceptionCollection exceptions = new ExceptionCollection();
          foreach (Delegate callback in _callbacks)
          {
            try
            {
              if (callback is AsyncOperationCallback)
              {
                AsyncOperationCallback cb = (AsyncOperationCallback)callback;
                if (SynchronizationContext.Current != _context)
                  _context.Send(new SendOrPostCallback(delegate(object os) { cb(this); }), null);
                else
                  cb(this);
              }
              else if (callback is EmptyMethod)
              {
                EmptyMethod cb = (EmptyMethod)callback;
                if (SynchronizationContext.Current != _context)
                  _context.Send(new SendOrPostCallback(delegate(object os) { cb(); }), null);
                else
                  cb();
              }
              else
              {
                if (SynchronizationContext.Current != _context)
                  _context.Send(new SendOrPostCallback(delegate(object os) { callback.DynamicInvoke(this, _state); }), null);
                else
                  callback.DynamicInvoke(this, _state);
              }
            }
            catch (Exception ex)
            {
              if (callback.Target == null)
                exceptions.Add(new AsyncOperationException(String.Format("An exception occurred during an anonymous callback"),ex));
              else
                exceptions.Add(new AsyncOperationException(String.Format("An exception occurred during a callback for object '{0}'",callback.Target),ex));
            }
          }
          if (exceptions.Count == 1)
            CompleteOperation(exceptions[0]);
          else if (exceptions.Count > 1)
            CompleteOperation(new AsyncBatchException(exceptions));
        }
      }
    }
    #endregion

    ///// <summary>
    ///// Forcefully cancels the operation
    ///// </summary>
    //public void Kill()
    //{
    //  Cancel(false);
      
    //}

    public void Cancel()
    {
      Cancel(false);
    }

    /// <summary>
    /// Cancels the async operation.
    /// </summary>
    /// <param name="waitForCompletion">True to wait for the cancel to commit.</param>
    public virtual void Cancel(bool waitForCompletion)
    {
      CompleteOperation(AsyncOperationResult.Cancelled);

      if (waitForCompletion)
        WaitForCompletion();
    }

    protected virtual void CompleteOperation(AsyncOperationResult result)
    {
      _result = result;
      _waitHandle.Set();
    }

    /// <summary>
    /// Completes the operation, sets the Error property and sets the Result to Error.
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    protected virtual void CompleteOperation(Exception ex)
    {
      _error = ex;
      CompleteOperation(AsyncOperationResult.Error);
    }

    public AsyncOperationResult WaitForCompletion()
    {
      return WaitForCompletion(TimeSpan.Zero);
    }

    public virtual AsyncOperationResult WaitForCompletion(TimeSpan timeout)
    {
      if (timeout == TimeSpan.Zero)
      {
        if (!_waitHandle.WaitOne(-1, false))
          return AsyncOperationResult.Timeout;
      }
      else
      {
        if (!_waitHandle.WaitOne(timeout, false))
          return AsyncOperationResult.Timeout;
      }
      return Result;
    }
    #endregion
    #region Static Methods
    public static AsyncOperation Start(SynchronizationContext context, EmptyMethod worker, params EmptyMethod[] callbacks)
    {
      AsyncOperation op = new AsyncOperation(context, worker, callbacks);
      op.Start();
      return op;
    }

    public static AsyncOperation Start(EmptyMethod worker, params EmptyMethod[] callbacks)
    {
      AsyncOperation op = new AsyncOperation(worker, callbacks);
      op.Start();
      return op;
    }

    public static AsyncOperation Start(EmptyMethod worker, params AsyncOperationCallback[] callbacks)
    {
      AsyncOperation op = new AsyncOperation(worker, callbacks);
      op.Start();
      return op;
    }

    public static AsyncOperation Start(EmptyMethod worker)
    {
      AsyncOperation op = new AsyncOperation(worker);
      op.Start();
      return op;
    }


    public static AsyncOperation Start<T>(StatefulWorker<T> worker, params StatefulCallback<T>[] callbacks)
    {
      AsyncOperation op = new AsyncOperation(worker, callbacks);
      op.Start();
      return op;
    }
    #endregion
  }
}
