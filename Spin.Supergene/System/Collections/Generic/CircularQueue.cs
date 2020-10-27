using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace excorlib.System.Collections.Generic
{
  /// <summary>
  /// Provides a "thread-safer" queue
  /// </summary>
  public class CircularQueue<T> //: IEnumerable<T>, ICollection<T>, ICollection, IEnumerable
  {
    private int _capacity;
    private int _writeIndex;
    private int _readIndex;
    private int _readableIndex;
    private T[] _data;


    public CircularQueue(int capacity)
    {
      _capacity = capacity;
      _data = new T[_capacity];
    }

    public bool HasData()
    {
      return _readableIndex > _readIndex;
    }

    public T Pop()
    {
      throw new NotImplementedException();
      int loc = -1;
      if (_readableIndex > _readIndex)
        loc = Interlocked.Increment(ref _readIndex);
      if (loc > _readableIndex)
      {
        return default(T);
      }
      return default(T);
    }

    public void Push(T obj)
    {
      throw new NotImplementedException();
      int loc = Interlocked.Increment(ref _writeIndex);
      _data[loc] = obj;
      Interlocked.Increment(ref _readableIndex);
    }


  }
}
