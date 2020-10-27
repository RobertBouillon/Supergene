using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Threading
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>
  /// The result does not change until none of the operations are pending.
  /// </remarks>
  public sealed class AsyncBatchOperation :
    AsyncOperation, 
    IList<AsyncOperation>, 
    ICollection<AsyncOperation>,
    IEnumerable<AsyncOperation>,
    IList,
    ICollection,
    IEnumerable
  {
    #region Static Declarations
    public static AsyncBatchOperation BatchProcess<T>(IEnumerable<T> target, Action<T> action)
    {
      List<AsyncOperation> items = new List<AsyncOperation>();
      foreach (T t in target)
      {
        T anchor = t; 
        items.Add(AsyncOperation.Start(new EmptyMethod(delegate() { action(anchor); })));
      }

      return new AsyncBatchOperation(items);
    }

    public static AsyncBatchOperation Start(params EmptyMethod[] operations)
    {
      List<AsyncOperation> items = new List<AsyncOperation>();
      foreach (EmptyMethod m in operations)
        items.Add(AsyncOperation.Start(m));

      return new AsyncBatchOperation(items);
    }

    public static AsyncBatchOperation Start(AsyncOperationCallback callback, params EmptyMethod[] operations)
    {
      List<AsyncOperation> items = new List<AsyncOperation>();
      foreach (EmptyMethod m in operations)
        items.Add(AsyncOperation.Start(m));

      return new AsyncBatchOperation(callback, items);
    }
    #endregion

    #region Fields
    private ExceptionCollection _exceptions = new ExceptionCollection();
    private readonly CollectionBase<AsyncOperation> _innerList;
    //private Thread _waitHandleThread;
    private ManualResetEvent _waitHandle = new ManualResetEvent(false);
    private AsyncOperationCallback _callback = null;
    #endregion
    #region Constructors
    [Obsolete("Use the static methods instead")]
    public AsyncBatchOperation()
    {
      _innerList = new CollectionBase<AsyncOperation>();
    }

    [Obsolete("Use the static methods instead")]
    public AsyncBatchOperation(IEnumerable<AsyncOperation> operations)
    {
      _innerList = new CollectionBase<AsyncOperation>(operations);
    }

    [Obsolete("Use the static methods instead")]
    public AsyncBatchOperation(params AsyncOperation[] operations)
      : this((IEnumerable<AsyncOperation>)operations)
    {
    }

    [Obsolete("Use the static methods instead")]
    public AsyncBatchOperation(AsyncOperationCallback callback, IEnumerable<AsyncOperation> operations)
      : this(operations)
    {
      #region Validation
      if (callback == null)
        throw new ArgumentNullException("callback");
      #endregion
      _callback = callback;
    }

    [Obsolete("Use the static methods instead")]
    public AsyncBatchOperation(AsyncOperationCallback callback, params AsyncOperation[] operations)
      : this(callback, (IEnumerable<AsyncOperation>)operations)
    {
    }

    #endregion

    #region Overrides
    

    public override void Cancel(bool waitForCompletion)
    {
      lock (this)
        foreach (AsyncOperation op in this)
          op.Cancel(false);

      if (waitForCompletion)
        WaitForCompletion();
    }

    protected internal override WaitHandle WaitHandle
    {
      get
      {
        //Only create the thread if someone actually needs the wait handle.
        Threading.WaitHandle[] handles = new WaitHandle[Count];
        int i = 0;
        foreach (AsyncOperation op in this)
          handles[i++] = op.WaitHandle;
        return new CompoundWaitHandle(handles);
      }
    }

    //private void CreateWaitHandleThread()
    //{
    //  _waitHandleThread = new Thread(new ThreadStart(WaitHandleWorker));
    //  _waitHandleThread.IsBackground = true;
    //  _waitHandleThread.Name = "AsyncBatch Watcher";
    //  _waitHandleThread.Priority = ThreadPriority.Normal;
    //  _waitHandleThread.Start();
    //}

    //private void WaitHandleWorker()
    //{
    //  if (_waitHandleThread == null)
    //  {
    //    Threading.WaitHandle[] handles;
    //    lock (this)
    //    {
    //      handles = new System.Threading.WaitHandle[this.Count];
    //      for (int i = 0; i < Count; i++)
    //        handles[i] = this[i].WaitHandle;
    //    }
    //    Threading.WaitHandle.WaitAll(handles, timeout, true);
    //    _waitHandle.Set();
    //  }
    //}

    public override Exception Error
    {
      get
      {
        return new AsyncBatchException(_exceptions);
      }
      protected set
      {
        //Sealed. We'll never hit this (Part of the reason why we're sealing. 
        //You can't inherit this and safely override any operations.)
        throw new NotSupportedException("Sealed");
      }
    }

    public override AsyncOperationResult Result
    {
      get
      {
        lock (this)
        {
          foreach (AsyncOperation op in this)
          {
            if (op.Result == AsyncOperationResult.Pending)
              return AsyncOperationResult.Pending;
          }
          //If we're here, then nothing is pending. Let's check for errors.
          //This is a seperate loop because we don't want to execute this unless we're complete above.
          foreach (AsyncOperation op in this)
          {
            AsyncOperationResult result = op.Result;  //unbox

            if (result == AsyncOperationResult.Cancelled)
              return AsyncOperationResult.Cancelled;

            if (result == AsyncOperationResult.Timeout)
              return AsyncOperationResult.Timeout;
          }

          _exceptions = new ExceptionCollection();
          foreach (AsyncOperation op in this)
            if (op.Result == AsyncOperationResult.Error)
              _exceptions.Add(op.Error);

          if (_exceptions.Count > 0)
            return AsyncOperationResult.Error;

          return AsyncOperationResult.Completed;
        }
      }
    }

    public override AsyncOperationResult WaitForCompletion(TimeSpan timeout)
    {
      if (!this.WaitHandle.WaitOne(timeout, false))
        return AsyncOperationResult.Timeout;

      return Result;
    }
    #endregion

    #region IList<StatelessAsyncOperation> Members

    public int IndexOf(AsyncOperation item)
    {
      return _innerList.IndexOf(item);
    }

    public void Insert(int index, AsyncOperation item)
    {
      _innerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      _innerList.RemoveAt(index);
    }

    public AsyncOperation this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        _innerList[index] = value;
      }
    }

    #endregion
    #region ICollection<StatelessAsyncOperation> Members

    public void Add(AsyncOperation item)
    {
      _innerList.Add(item);
    }

    public void Clear()
    {
      _innerList.Clear();
    }

    public bool Contains(AsyncOperation item)
    {
      return _innerList.Contains(item);
    }

    public void CopyTo(AsyncOperation[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _innerList.Count; }
    }

    public bool IsReadOnly
    {
      get { return _innerList.IsReadOnly; }
    }

    public bool Remove(AsyncOperation item)
    {
      return _innerList.Remove(item);
    }

    #endregion
    #region IEnumerable<StatelessAsyncOperation> Members

    public IEnumerator<AsyncOperation> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion
    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    #endregion
    #region IList Members

    public int Add(object value)
    {
      _innerList.Add((AsyncOperation)value);
      return Count - 1;
    }

    public bool Contains(object value)
    {
      return _innerList.Contains((AsyncOperation)value);
    }

    public int IndexOf(object value)
    {
      return _innerList.IndexOf((AsyncOperation)value);
    }

    public void Insert(int index, object value)
    {
      _innerList.Insert(index, (AsyncOperation)value);
    }

    public bool IsFixedSize
    {
      get { return false; }
    }

    public void Remove(object value)
    {
      _innerList.Remove((AsyncOperation)value);
    }

    object IList.this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        _innerList[index] = (AsyncOperation)value;
      }
    }

    #endregion
    #region ICollection Members

    public void CopyTo(Array array, int index)
    {
      _innerList.CopyTo(array, index);
    }

    public bool IsSynchronized
    {
      get { return _innerList.IsSynchronized; }
    }

    public object SyncRoot
    {
      get { return _innerList.SyncRoot; }
    }

    #endregion
  }
}
