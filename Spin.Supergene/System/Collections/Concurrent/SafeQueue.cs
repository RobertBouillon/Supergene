using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Concurrent
{
  public class SafeQueue<T>  //: IQueue<T>
  {
    #region Fields
    private Object[] _buffer;
    private volatile SafeQueue<T> _overflowBuffer;
    private readonly int _bufferSize;

    private int _availableWrites;
    private int _availableReads;

    private int _readPosition = 0;
    private int _writePosition = 0;

    private long _readScans = 0;
    private long _writeScans = 0;
    #endregion

    #region Properties
    public int Count
    {
      get { return _availableReads; }
    }

    #endregion

    #region Constructors
    public SafeQueue(int bufferSize)
    {
      //if (Int32.MaxValue % bufferSize != bufferSize - 1)
      //  throw new ArgumentException("Buffer size must divide evenly into Int32.Max");

      _availableWrites = bufferSize - 1;
      _buffer = new Object[_bufferSize = bufferSize];
    }
    #endregion

    #region Methods


    public bool TryEnqueue(T item)
    {
      //Ensure we have a slot available
      int count;
      do
        if((count = _availableWrites) <= 0)
          return false;
      while(Interlocked.CompareExchange(ref _availableWrites, count - 1, count) != count);

      //Scan the buffer until we find an empty slot
      while (Interlocked.CompareExchange(ref _buffer[((uint)Interlocked.Increment(ref _writePosition)) % _bufferSize], item, null) != null)
        Interlocked.Increment(ref _writeScans);
      
      Interlocked.Increment(ref _availableReads);
      return true;
    }
    
    public bool TryDequeue(out T item)
    {
      int count;
      object o;

      //Ensure we have a slot available
      do
        if ((count = _availableReads) <= 0)
        {
          item = default(T);
          return false;
        }
      while (Interlocked.CompareExchange(ref _availableReads, count - 1, count) != count);


      //Scan the buffer until we find a full slot
      while ((o = Interlocked.Exchange(ref _buffer[((uint)(Interlocked.Increment(ref _readPosition))) % _bufferSize], null)) == null)
        Interlocked.Increment(ref _readScans);
      item = (T)o;

      Interlocked.Increment(ref _availableWrites);
      return true;
    }

    #endregion
  }
}
