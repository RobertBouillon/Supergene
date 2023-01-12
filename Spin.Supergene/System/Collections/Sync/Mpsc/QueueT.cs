using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Collections.Sync.Mpsc;

public class Queue<T> : IEnumerable<T> where T : class
{
  //Buffer overflow!!

  //private struct MarkedEntry { public int HasValue; public T Value; }
  //private MarkedEntry[] _queue;
  private T[] _buffer;
  private volatile int _readPosition;
  private volatile int _writePosition;
  private readonly int _size;
  //private bool _corrupt = false;

  public int Count
  {
    get { return _writePosition - _readPosition; }
  }

  public Queue()
    : this(32)
  {

  }

  public Queue(int size)
  {
    //if ((size & (size - 1)) != 0)
    //  throw new ArgumentOutOfRangeException("Size must be a power of two.");
    //_queue = new MarkedEntry[size];
    _size = size;
    _buffer = new T[size];
  }

  public Queue(T[] source)
  {
    int len = source.Length;
    _buffer = new T[len];
    Array.Copy(source, _buffer, len);

    //if ((size & (size - 1)) != 0)
    //  throw new ArgumentOutOfRangeException("Size must be a power of two.");
    //_queue = new MarkedEntry[size];
  }

  public void Push(T value)
  {
    //if (_corrupt)//Safety first.
    //  throw new InvalidOperationException("Buffer has been corrupted by a buffer overflow. Must be reset");

    int index = Interlocked.Increment(ref _writePosition);
    //Handle Overflow
    //if (index < 0)
    //  index += Int32.MaxValue;

    value = Interlocked.Exchange(ref _buffer[index % _size], value);
    if (value != null)
      throw new Exception("Buffer Overflow");
  }


  //Should be trypop... if multiple threads read, it's possible that only one will have an item to return.
  public T Pop()
  {
    if (Count == 0)
      return null;

    int index = Interlocked.Increment(ref _readPosition);
    T value = null;
    value = Interlocked.Exchange(ref _buffer[index % _size], value);
    if (value == null)
      throw new Exception("Buffer underrun");

    return value;
  }

  public T Peek()
  {
    T value = null;
    while (value == null)
      if (Count == 0)
        return default(T);
      else
        value = _buffer[(_readPosition + 1) % _size];

    return value;
  }

  public IEnumerable<T> Flush()
  {
    while (Count > 0)
      yield return Pop();
  }

  public IEnumerator<T> GetEnumerator()
  {
    int index = _readPosition;
    do
    {
      T ret = _buffer[_readPosition % _size];
      if (ret != null)
        yield return ret;
    } while (++_readPosition < _writePosition);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
  {
    while (Count > 0)
      yield return Pop();
  }
}
