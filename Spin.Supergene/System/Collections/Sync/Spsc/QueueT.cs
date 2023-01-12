using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace excorlib.System.Collections.Sync.Spsc;

public class QueueT<T> : IEnumerable<T>
{
  private T[] _queue;
  private volatile int _readPosition;
  private volatile int _writePosition;
  private int _size;
  private bool _corrupt = false;

  public QueueT()
    : this(32)
  {

  }

  public QueueT(int size)
  {
    _queue = new T[size];
  }

  public void Push(T value)
  {
    if (_writePosition + 1 == _readPosition)
      throw new Exception("Buffer overflow");

    int index = _writePosition;
    _queue[index] = value;
    _writePosition++;

    //circular buffer
    if (_writePosition == _size)
      _writePosition = 0;
  }

  public bool Pop(out T value)
  {
    if (_writePosition == _readPosition)
    {
      value = default(T);
      return false;
    }
    value = _queue[_readPosition];
    _readPosition++;
    return true;
  }

  public IEnumerator<T> GetEnumerator()
  {
    var ret = default(T);
    while (Pop(out ret))
      yield return ret;
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
  {
    var ret = default(T);
    while (Pop(out ret))
      yield return ret;
  }
}
