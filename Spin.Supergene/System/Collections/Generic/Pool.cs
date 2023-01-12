using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace System.Collections.Generic;

public class Pool<T> : IEnumerable
{
  #region Fields
  private int _maximumSize = -1;
  private int _count;
  private List<T> _allocated;
  private Stack<T> _available;
  private AutoResetEvent _queue;
  private int _queueSize = 0;

  Mutex _localMutex = new Mutex();
  #endregion

  #region Properties
  /// <summary>
  /// Gets the number of allocations waiting in the queue
  /// </summary>
  public int Queued
  {
    get { return _queueSize; }
  }

  public List<T> Allocated
  {
    get { return _allocated; }
  }

  public int Count
  {
    get { return _count; }
  }

  public int MaxCount
  {
    get { return _maximumSize; }
  }

  public int NumberAvailable
  {
    get { return _available.Count; }
  }

  public int AmountAllocated
  {
    get { return _allocated.Count; }
  }
  #endregion

  #region Constructors
  public Pool()
  {
    _available = new Stack<T>();
    _allocated = new List<T>();
    _queue = new AutoResetEvent(false);
  }

  public Pool(int maximumSize)
  {
    #region Validation
    if (maximumSize < 0)
      throw new ArgumentOutOfRangeException("maximumSize", "maximumSize cannot be less than Zero.");
    #endregion
    _maximumSize = maximumSize;
    _available = new Stack<T>(_maximumSize);
    _allocated = new List<T>(_maximumSize);
    _queue = new AutoResetEvent(false);
  }
  #endregion


  /// <summary>
  /// Override to add an object to the pool when a new object is required.
  /// </summary>
  /// <returns></returns>
  protected virtual T Grow()
  {
    return Activator.CreateInstance<T>(); ;
  }

  /// <summary>
  /// Allocates an object in the pool to the caller
  /// </summary>
  /// <returns></returns>
  public virtual T Allocate()
  {
    _localMutex.WaitOne();
    while (_available.Count == 0)
    {
      if (_count < _maximumSize || _maximumSize <= 0)
        _available.Push(Grow());
      else
      {
        _queueSize++;
        _localMutex.ReleaseMutex();
        _queue.WaitOne();
        _localMutex.WaitOne();
      }
    }

    T o;
    //This should NEVER happen, but if it does, at least we'll get a descriptive error message.
    if (_available.Count == 0)
      throw new Exception("Unable to acquire object from the pool.");

    o = _available.Pop();
    _allocated.Add(o);
    _count++;
    _localMutex.ReleaseMutex();
    return o;
  }

  /// <summary>
  /// Deallocated an object from the pool and makes it available.
  /// </summary>
  /// <param name="o"></param>
  public virtual void Deallocate(T o)
  {
    #region Validation
    if (o == null)
      throw new ArgumentNullException("o");
    #endregion
    _localMutex.WaitOne();
    _available.Push(o);
    _allocated.Remove(o);

    _count--;

    //FIFO Queue
    if (_queueSize > 0)
    {
      _queue.Set();
      _queueSize--;
    }

    Trace.WriteLine(String.Format("Connection Deallocated: {0} connections left allocated", _count));
    _localMutex.ReleaseMutex();
  }

  /// <summary>
  /// Removes all unallocated objects
  /// </summary>
  public virtual void Compact()
  {
    lock (this)
    {
      while (_available.Count > 0)
      {
        _available.Pop();
        _count--;
      }
    }
  }

  #region IEnumerable Members

  public IEnumerator GetEnumerator()
  {
    foreach (T o in _allocated)
      yield return o;

    foreach (T o in _available)
      yield return o;
  }

  #endregion
}
