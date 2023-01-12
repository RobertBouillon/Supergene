using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Specialized;

[Serializable]
public class MruList<T> : IList<T>, ICollection<T>, IEnumerable<T>
{
  #region Fields
  private int _capacity = 5;
  private List<T> _innerList = new List<T>();
  #endregion
  #region Properties
  public int Capacity
  {
    get { return _capacity; }
    set { _capacity = value; }
  }

  protected List<T> InnerList
  {
    get { return _innerList; }
  }
  #endregion
  #region Constructors
  public MruList()
  {
  }

  public MruList(IEnumerable<T> source)
  {
    _innerList = new List<T>(source);
  }

  public MruList(int capacity)
  {
    _capacity = capacity;
  }
  #endregion
  #region ICollection<T> Members
  public void Add(T item)
  {
    if (_innerList.Contains(item))
      _innerList.Remove(item);

    _innerList.Add(item);
    if (_innerList.Count > _capacity)
      _innerList.RemoveAt(_innerList.Count - 1);

    OnAdded(item);
  }

  public void Clear()
  {
    while (_innerList.Count > 0)
      RemoveAt(0);
  }

  public bool Contains(T item)
  {
    return _innerList.Contains(item);
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    _innerList.CopyTo(array, arrayIndex);
  }

  public int Count
  {
    get { return _innerList.Count; }
  }

  public bool IsReadOnly
  {
    get { return false; }
  }

  public bool Remove(T item)
  {
    bool ret = _innerList.Remove(item);
    OnRemoved(item);
    return ret;
  }

  #endregion

  #region IEnumerable<T> Members

  public IEnumerator<T> GetEnumerator()
  {
    foreach (T i in _innerList)
      yield return i;
  }

  #endregion

  #region IEnumerable Members

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
  {
    return _innerList.GetEnumerator();
  }

  #endregion

  #region IList<T> Members

  public int IndexOf(T item)
  {
    return _innerList.IndexOf(item);
  }

  public void Insert(int index, T item)
  {
    _innerList.Insert(index, item);
    OnAdded(item);
  }

  public void RemoveAt(int index)
  {
    T item = _innerList[index];
    _innerList.RemoveAt(index);
    OnRemoved(item);
  }

  public T this[int index]
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

  #region Events

  public event EventHandler<MruListItemEventArgs> Added;

  protected void OnAdded(T item)
  {
    OnAdded(new MruListItemEventArgs(item));
  }

  protected virtual void OnAdded(MruListItemEventArgs e)
  {
    if (Added != null)
      Added(this, e);
  }


  #region RemovedEventArgs Subclass
  public class MruListItemEventArgs : EventArgs
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
    internal MruListItemEventArgs(T item)
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

  public event EventHandler<MruListItemEventArgs> Removed;

  protected void OnRemoved(T item)
  {
    OnRemoved(new MruListItemEventArgs(item));
  }

  protected virtual void OnRemoved(MruListItemEventArgs e)
  {
    if (Removed != null)
      Removed(this, e);
  }



  #endregion
}
