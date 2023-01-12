using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Sync.Mpsc;

public class MpscCollection<T> : ICollection<T>
{
  //2 Gotchas - 1: Enumerator omits default values (0 for blittable and null for ref type) 2: If doing more than Int32.Max Adds, Compact must be called if the size is not a power of 2.
  //Buffer overflow keeps happening!


  #region Fields

  private T[] _data;
  private int[] _slots;
  private int _capcacity;

  private volatile int _openSlots;
  private volatile int _reservedSlots;
  private ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();
  #endregion

  #region Constructors

  public MpscCollection(int initialCapacity = 16)
  {
    Initialize(initialCapacity, new T[] { });
  }

  public MpscCollection(IEnumerable<T> source, int intialCapacity = 16) : this(intialCapacity)
  {
    Initialize(intialCapacity, source);
  }
  #endregion

  #region Methods
  private void Grow()
  {
    Grow(_capcacity * 2);
  }

  private void Grow(int newCapacity)
  {
    _sync.EnterWriteLock();

    if (_capcacity < newCapacity)
      Initialize(newCapacity, ToArray());

    _sync.ExitWriteLock();
  }

  private void Initialize(int capacity, IEnumerable<T> source)
  {
    _data = new T[capacity];
    _slots = new int[capacity];

    for (int i = 0; i < capacity; i++)
      _slots[i] = i;
    _capcacity = capacity;

    int index = 0;
    foreach (var item in source)
      _data[index++] = item;

    _openSlots = _reservedSlots += index;
  }


  public void Compact()
  {
    _sync.EnterWriteLock();

    _openSlots = _openSlots % _capcacity;
    _reservedSlots = _reservedSlots % _capcacity;

    _sync.ExitWriteLock();
  }

  public int AddWithIndex(T item)
  {
    if (Count == _capcacity)
      throw new Exception("Buffer overflow");

    _sync.EnterReadLock();

    int i = -1;
    int position = Interlocked.Increment(ref _openSlots);
    while (i == -1)
      Interlocked.Exchange(ref _slots[position], -1);  //Mark the slot as reserved / invalid. If it's currently -1, then another thread is about to write the value to it, so spin.
    _data[position % _capcacity] = item;

    _sync.ExitReadLock();

    return position;
  }

  public bool Remove(T item)
  {
    _sync.EnterReadLock();

    for (int i = 0; i < _capcacity; i++)
      if (_data[i].Equals(item))
      {
        RemoveByIndexUnsafe(i);
        _sync.ExitReadLock();
        return true;
      }

    _sync.ExitReadLock();
    return false;
  }

  public bool RemoveByIndex(int index)
  {
    _sync.EnterReadLock();
    var ret = RemoveByIndexUnsafe(index);
    _sync.ExitReadLock();
    return ret;
  }

  private bool RemoveByIndexUnsafe(int index)
  {
    _data[index] = default(T); //Remove the reference.
    var slot = Interlocked.Increment(ref _reservedSlots);
    var result = Interlocked.Exchange(ref _slots[slot % _capcacity], index);
    if (result != -1)
      throw new Exception("Something bad happened. It shouldn't have.");

    return true;
  }
  #endregion

  #region Overrides

  public IEnumerator<T> GetEnumerator()
  {
    var def = default(T);
    for (int i = 0; i < _capcacity; i++)
    {
      var item = _data[i];
      if (!item.Equals(def))
        yield return item;
    }
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
  {
    var def = default(T);
    for (int i = 0; i < _capcacity; i++)
    {
      var item = _data[i];
      if (!item.Equals(def))
        yield return item;
    }
  }

  public void CopyTo(Array array, int index)
  {
    foreach (var item in this)
      array.SetValue(item, index++);
  }

  public void CopyTo(T[] array, int index)
  {
    foreach (var item in this)
      array[index++] = item;
  }

  public T[] ToArray()
  {
    return new List<T>(this).ToArray();
  }


  public int Count
  {
    get { return _openSlots - _reservedSlots; }
  }

  public bool IsSynchronized
  {
    get { return false; }
  }

  public object SyncRoot
  {
    get { throw new NotSupportedException(); }
  }

  public void Add(T item)
  {
    AddWithIndex(item);
  }

  public virtual void Clear()
  {
    foreach (var item in this)
      Remove(item);
  }

  public virtual bool Contains(T item)
  {
    foreach (var item2 in this)
      if (item.Equals(item2))
        return true;

    return false;
  }

  public bool IsReadOnly
  {
    get { return false; }
  }
  #endregion

  #region Unit Tests
  [Conditional("DEBUG")]
  public static void Test()
  {
    int readers = 8;
    int writers = 8;
    int duplex = 16;

    MpscCollection<int> col = new MpscCollection<int>(4096);

    Action writer = () =>
    {
      for (int i = 0; i < 256; i++)
      {
        col.Add(i);
        col.Remove(i);
      }

      for (int i = 0; i < 512; i++)
      {
        var index = col.AddWithIndex(i);
        col.RemoveByIndex(index);
      }
    };

    Thread[] threads = new Thread[duplex];
    for (int i = 0; i < duplex; i++)
      threads[i] = new Thread(() => writer()) { IsBackground = true, Priority = ThreadPriority.BelowNormal };

    for (int i = 0; i < duplex; i++)
      threads[i].Start();

  }
  #endregion
}
