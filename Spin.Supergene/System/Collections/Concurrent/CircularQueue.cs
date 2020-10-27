using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Collections.Concurrent
{
  public class CircularQueue<T> : IProducerConsumerCollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
  {
    private int _count;
    private int _size;
    private int _writePosition = 0;
    private int _readPosition = 0;
    private T[] _buffer;

    public int Count => _count;
    public bool IsSynchronized => true;
    public object SyncRoot => this;

    public void CopyTo(T[] array, int index)
    {
      throw new NotImplementedException();
    }

    public void CopyTo(Array array, int index)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public T[] ToArray()
    {
      throw new NotImplementedException();
    }

    public bool TryAdd(T item)
    {
      int write = Interlocked.Increment(ref _writePosition);
      

      if (write < 0)
        write += Int32.MaxValue;

      _buffer[write % _bufferSize] = item;
      Interlocked.Increment(ref _count);

      WaitHandle.Set();
      return _count;
    }

    public bool TryTake(out T item)
    {
      int pos = Interlocked.Increment(ref _readPosition);
      if (pos < 0)
        pos += Int32.MaxValue;

      pos = pos % _bufferSize;
      T item = _buffer[pos % _bufferSize];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}
