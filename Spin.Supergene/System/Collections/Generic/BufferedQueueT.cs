using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
  public class BufferedQueue<T>
  {
    private T[,] _data;

    private int _buffers;
    private int _bufferSize;
    private TimeSpan _bufferTimeout;

    private int _bufferWriteNumber;  //The buffer currently in use
    private int _bufferWriteIndex;   //The position in the current buffer 
    private int _bufferReadNumber;
    private int _bufferReadIndex;

    private DateTime _lastWrite;


    public BufferedQueue(int buffers, int bufferSize)
    {
      _buffers = buffers;
      _bufferSize = bufferSize;
      _data = new T[_buffers, _bufferSize];
    }

    public void Push(T obj)
    {
      int index = ++_bufferWriteIndex;
      if (_bufferWriteIndex == _bufferSize)
      {
        _bufferWriteIndex = 0;
        _bufferWriteNumber++;
        if (_bufferWriteNumber == _buffers)
          _bufferWriteNumber = 0;
        if (_bufferWriteNumber == _bufferReadNumber)
          throw new Exception("Buffer Overrun");
      }

      _data[_bufferWriteNumber, index] = obj;
    }

    public T Pop()
    {
      //int index =
      return default(T);
    }
  }
}
