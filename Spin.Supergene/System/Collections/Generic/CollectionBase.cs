using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Collections.Generic;

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
public class CollectionBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, ICollection, IList, IEnumerable
{
  #region Private Members
  private List<T> _innerList;
  #endregion
  #region Constructors
  public CollectionBase()
  {
    _innerList = new List<T>();
  }

  public CollectionBase(int capacity)
  {
    _innerList = new List<T>(capacity);
  }

  public CollectionBase(IEnumerable<T> source)
  {
    _innerList = new List<T>(source);
  }
  #endregion
  
  #region Public Properties
  public IList<T> InnerList
  {
    get { return _innerList; }
    //set { throw new NotSupportedException(); }
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
  public virtual void ForEach(Action<T> action)
  {
    _innerList.ForEach(action);
  }

  public virtual void CopyTo(ICollection<T> destination)
  {
    #region Validation
    if (destination == null)
      throw new ArgumentNullException("destination");
    #endregion
    foreach (T t in this)
      destination.Add(t);
  }

  public virtual T[] ToArray()
  {
    T[] ret = new T[Count];
    int index = 0;
    foreach(T t in this)
      ret[index++] = t;

    return ret;
  }

  public virtual void AddRange(IEnumerable<T> array)
  {
    //TODO: Change OnInsert to take array as parameters
    Exception e;
    int count = Count;
    foreach (var item in array)
      if ((e = OnInserting(count++, item)) != null)
        throw e;

    _innerList.AddRange(array);

    count = Count;
    foreach (T item in array)
      OnInserted(count++, item);
  }

  public void Clear()
  {
    OnClear();
    //_innerList.Clear();
    while (Count > 0)
      RemoveAt(0);
    _innerList.Clear();
    OnClearComplete();
  }

  public void RemoveAt(int index)
  {
    T value = _innerList[index];
    OnRemoving(index, value);
    _innerList.RemoveAt(index);
    OnRemoved(index, value);
  }
  #endregion

  #region Overrides
  protected virtual void OnClear() { }
  protected virtual void OnClearComplete() { if (Cleared != null)Cleared(this, EventArgs.Empty); }
  protected virtual void OnInserted(int index, T value) { if (Inserted != null)Inserted(this, new CollectionBase<T>.InsertedEventArgs(index, value)); }
  protected virtual void OnRemoved(int index, T value) { if (Removed != null)Removed(this, new CollectionBase<T>.RemovedEventArgs(value)); }
  protected virtual void OnSet(int index, T oldValue, T newValue) { }
  protected virtual void OnSetComplete(int index, T oldValue, T newValue) { if (ItemChanged != null)ItemChanged(this, new CollectionBase<T>.ItemChangedEventArgs(oldValue, newValue, index)); }
  protected virtual void OnValidate(T value) { }
  protected void OnRemoving(int index, T item){OnRemoving(new RemovingEventArgs(index, item));}
  protected virtual void OnRemoving(RemovingEventArgs e){if (Removing != null)Removing(this, e);}
  protected Exception OnInserting(int index, T item){return OnInserting(new InsertingEventArgs(index, item));}
  protected virtual Exception OnInserting(InsertingEventArgs e) { if (Inserting != null)Inserting(this, e); return e.CancelException; }
  #endregion

  #region IList<T> Members

  public int IndexOf(T item)
  {
    return _innerList.IndexOf(item);
  }

  public void Insert(int index, T item)
  {
    Exception e;
    if ((e = OnInserting(index, item))!=null)
      throw e;
    _innerList.Insert(index, item);
    OnInserted(index, item);
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

  public virtual void Add(T item)
  {
    int index = _innerList.Count;
    Exception e;
    if ((e = OnInserting(index, item)) !=null)
      throw e;
    _innerList.Add(item);
    OnInserted(index, item);
  }

  public virtual bool Contains(T item)
  {
    return _innerList.Contains(item);
  }

  public virtual void CopyTo(T[] array, int arrayIndex)
  {
    _innerList.CopyTo(array, arrayIndex);
  }

  public bool IsReadOnly
  {
    get { return false; }
  }

  public virtual bool Remove(T item)
  {
    if (!_innerList.Contains(item))
      return false;

    int index = _innerList.IndexOf(item);

    OnRemoving(index, item);
    _innerList.RemoveAt(index);
    OnRemoved(index, item);

    return true;
  }

  public virtual bool Remove(IEnumerable<T> list)
  {
    #region Validation
    if (list == null)
      throw new ArgumentNullException("list");
    #endregion
    foreach (var item in list)
      Remove(item);

    return true;
  }

  #endregion

  #region IEnumerable<T> Members

  public IEnumerator<T> GetEnumerator()
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


  #region ICollection Members

  public void CopyTo(Array array, int index)
  {
    for (int i = index; i < Count; i++)
      array.SetValue(InnerList[i - index], i);
  }

  public bool IsSynchronized
  {
    get { return false; }
  }

  public object SyncRoot
  {
    get { return this; }
  }

  #endregion

  #region IList Members


  #endregion

  #region IList Members

  int IList.Add(object value)
  {
    InnerList.Add((T)value);
    return Count - 1;
  }

  void IList.Clear()
  {
    InnerList.Clear();
  }

  bool IList.Contains(object value)
  {
    return InnerList.Contains((T)value);
  }

  int IList.IndexOf(object value)
  {
    return InnerList.IndexOf((T)value);
  }

  void IList.Insert(int index, object value)
  {
    InnerList.Insert(index, (T)value);
  }

  bool IList.IsFixedSize => false;

  bool IList.IsReadOnly
  {
    get { return false; }
  }

  void IList.Remove(object value) => InnerList.Remove((T)value);
  void IList.RemoveAt(int index) => InnerList.RemoveAt(index);

  object IList.this[int index]
  {
    get
    {
      return InnerList[index];
    }
    set
    {
      InnerList[index] = (T)value;
    }
  }

  #endregion

  #region ICollection Members

  void ICollection.CopyTo(Array array, int index)
  {
    for (int i = index; i < Count; i++)
      array.SetValue(InnerList[i - index], i);
  }

  int ICollection.Count
  {
    get { return InnerList.Count; }
  }

  bool ICollection.IsSynchronized
  {
    get { return false; }
  }

  object ICollection.SyncRoot
  {
    get { return this; }
  }

  #endregion

  #region Events
  #region InsertedEventArgs Subclass
  public class InsertedEventArgs : EventArgs
  {
    #region Fields
    private readonly T _item;
    private readonly int _index;
    #endregion
    #region Properties
    public int Index
    {
      get { return _index; }
    }

    public T Item
    {
      get { return _item; }
    }
    #endregion
    #region Constructors
    internal InsertedEventArgs(int index, T item)
    {
      #region Validation
      if (item == null)
        throw new ArgumentNullException(nameof(item));
      #endregion
      _index = index;
      _item = item;
    }
    #endregion
  }
  #endregion
  #region RemovedEventArgs Subclass
  public class RemovedEventArgs : EventArgs
  {
    #region Fields
    private readonly T _item;
    #endregion
    #region Properties
    public T Item
    {
      get { return _item; }
    }
    #endregion
    #region Constructors
    internal RemovedEventArgs(T item)
    {
      #region Validation
      if (item == null)
        throw new ArgumentNullException("item");
      #endregion
      _item = item;
    }
    #endregion
  }
  #endregion
  #region ItemChangedEventArgs Subclass
  public class ItemChangedEventArgs : EventArgs
  {
    #region Fields
    private readonly T _oldItem;
    private readonly T _newItem;
    private readonly int _index;
    #endregion
    #region Properties
    public T OldItem
    {
      get { return _oldItem; }
    }

    public T NewItem
    {
      get { return _newItem; }
    }

    public int Index
    {
      get { return _index; }
    }
    #endregion
    #region Constructors
    internal ItemChangedEventArgs(T oldItem, T newItem, int index)
    {
      _oldItem = oldItem;
      _newItem = newItem;
      _index = index;
    }
    #endregion
  }
  #endregion
  #region InsertingEventArgs Subclass
  public class InsertingEventArgs : EventArgs
  {
    #region Fields
    private readonly int _index;
    private readonly T _item;
    private Exception _cancelException;
    #endregion
    #region Properties

    public Exception CancelException
    {
      get { return _cancelException; }
      set { _cancelException = value; }
    }

    public int Index
    {
      get { return _index; }
    }

    public T Item
    {
      get { return _item; }
    }
    #endregion
    #region Constructors
    internal InsertingEventArgs(int index, T item)
    {
      #region Validation
      if (item == null)
        throw new ArgumentNullException("item");
      #endregion
      _index = index;
      _item = item;
    }
    #endregion
  }
  #endregion
  #region RemovingEventArgs Subclass
  public class RemovingEventArgs : EventArgs
  {
    #region Fields
    private readonly T _item;
    private readonly int _index;
    #endregion
    #region Properties

    public int Index
    {
      get { return _index; }
    }
    
    public T Item
    {
      get { return _item; }
    }
    #endregion
    #region Constructors
    internal RemovingEventArgs(int index, T item)
    {
      #region Validation
      if (item == null)
        throw new ArgumentNullException("item");
      #endregion
      _item = item;
      _index = index;
    }
    #endregion
  }
  #endregion




  public event EventHandler<RemovingEventArgs> Removing;
  public event EventHandler<InsertingEventArgs> Inserting;
  public event EventHandler<ItemChangedEventArgs> ItemChanged;
  public event EventHandler Cleared;
  public event EventHandler<RemovedEventArgs> Removed;
  public event EventHandler<InsertedEventArgs> Inserted;
  #endregion
}
