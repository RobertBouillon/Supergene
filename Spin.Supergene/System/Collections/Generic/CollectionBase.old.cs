using System;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <remarks>
  /// Implementations of the non-generic collections were omitted to hold true to the new .NET 2.0 standards (The old interface are only 
  /// really for supporting the older interfaces now replaced by their generic counterparts).
  /// 
  /// Classes that require the backwards-compatability should implement the interfaces explicitly.
  /// 
  /// Since this is a base class, rather than forcing all new classes to use this backwards compatability even if it's not needed,
  /// the old interfaces were removed.
  /// </remarks>
  public class CollectionBase<T> : IList<T>, ICollection<T>, IEnumerable<T>
  {
    #region Private Members
    private List<T> _innerList;
    #endregion
    #region Constructors
    protected CollectionBase()
    {
      _innerList = new List<T>();
    }

    protected CollectionBase(int capacity)
    {
      _innerList = new List<T>(capacity);
    }

    public CollectionBase(IEnumerable<T> collection)
    {
      _innerList = new List<T>(collection);
    }
    #endregion

    #region Public Properties
    public IList<T> InnerList
    {
      get { return _innerList; }
      set { throw new NotSupportedException(); }
    }

    [ComVisible(false)]
    public int Capacity
    {
      get { return _innerList.Capacity; }
      set { _innerList.Capacity = value; }
    }

    public int Count
    {
      get { return _innerList.Count; }
    }
    #endregion

    #region Public Methods
    public void Clear()
    {
      OnClear();
      _innerList.Clear();
      OnClearComplete();
    }

    public void RemoveAt(int index)
    {
      T value = _innerList[index];
      OnRemove(index, value);
      _innerList.RemoveAt(index);
      OnRemoveComplete(index, value);
    }
    #endregion

    #region Overrides
    protected virtual void OnClear() { }
    protected virtual void OnClearComplete() { }
    protected virtual void OnInsert(int index, T value) { }
    protected virtual void OnInsertComplete(int index, T value) { }
    protected virtual void OnRemove(int index, T value) { }
    protected virtual void OnRemoveComplete(int index, T value) { }
    protected virtual void OnSet(int index, T oldValue, T newValue) { }
    protected virtual void OnSetComplete(int index, T oldValue, T newValue) { }
    protected virtual void OnValidate(T value) { }
    #endregion


    #region IList<T> Members

    public int IndexOf(T item)
    {
      return _innerList.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      OnInsert(index, item);
      _innerList.Insert(index, item);
      OnInsertComplete(index, item);
    }

    public virtual T this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        T ovalue = _innerList[index];
        OnSet(index, ovalue, value);
        _innerList[index] = value;
        OnSetComplete(index, ovalue, value);
      }
    }

    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
      int index = _innerList.Count;
      OnInsert(index, item);
      _innerList.Add(item);
      OnInsertComplete(index, item);
    }

    public bool Contains(T item)
    {
      return _innerList.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(T item)
    {
      if (!_innerList.Contains(item))
        return false;

      int index = _innerList.IndexOf(item);
      
      OnRemove(index, item);
      _innerList.RemoveAt(index);
      OnRemoveComplete(index, item);

      return true;
    }

    #endregion

    #region IEnumerable<T> Members

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
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
    /*
    public int Add(object value)
    {
      if (!(value is T))
        throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));

      Add((T)value);
      return _innerList.IndexOf((T)value);
    }

    public bool Contains(object value)
    {
      if (!(value is T))
        throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));

      return Contains((T)value);
    }

    public int IndexOf(object value)
    {
      if (!(value is T))
        throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));

      return _innerList.IndexOf((T)value);
    }

    public void Insert(int index, object value)
    {
      if (!(value is T))
        throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));

      Insert(index, (T)value);
    }

    public bool IsFixedSize
    {
      get { return false; }
    }

    public void Remove(object value)
    {
      if (!(value is T))
        throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));


      Remove((T)value);
    }

    object IList.this[int index]
    {
      get
      {
        return _innerList[index];
      }
      set
      {
        if (!(value is T))
          throw new ArgumentException(String.Format("value must be of type '{0}'", value.GetType().ToString()));

        _innerList[index] = (T)value;
      }
    }
    */
    #endregion

    #region ICollection Members
/*
    public void CopyTo(Array array, int index)
    {
      _innerList.CopyTo((T[])array, index);
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    public object SyncRoot
    {
      get { return null; }
    }
*/
    #endregion

  }
}
