using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
  public class Pool<T> : IPool<T>
  {
    #region Fields
    private Queue<T> _available = new Queue<T>();
    private List<T> _allocated = new List<T>();
    private int _maxSize;
    private int _size;
    private int _maxUnallocated;
    #endregion


    #region Constructors
    public Pool()
    {

    }
    #endregion

    #region Methods
    public bool TryAllocate(out T item)
    {
      T ret = default(T);

      if (_available.Count > 0)
        _allocated.Add(ret = _available.Dequeue());
      else
        if (_size < _maxSize)
          _allocated.Add(ret = Grow());

      item = ret;
      return item != null;
    }

    protected virtual T Grow()
    {
      return Activator.CreateInstance<T>();
    }

    public T Allocate()
    {
      T ret;

      if (!TryAllocate(out ret))
        throw new Exception("Unable to grow the pool. Max Size reached");

      return ret;
    }

    public int AmountAllocated
    {
      get { return _allocated.Count; }
    }

    public void Compact()
    {
      _available.Clear();
    }

    public int Count
    {
      get { return _available.Count + _allocated.Count; }
    }

    public virtual void Deallocate(T o)
    {
      _allocated.Remove(o);
      _available.Enqueue(o);
    }

    public int MaxCount
    {
      get { return _maxSize; }
    }

    public int NumberAvailable
    {
      get { return _available.Count; }
    }
    #endregion

    #region Explicit Definitions
    IList<T> IPool<T>.Allocated
    {
      get { return _allocated; }
    }
    #endregion
  }
}
