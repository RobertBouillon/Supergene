using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Workers
{
  public class DoubleBufferWorker<T> : Worker
  {
    #region Fields
    private readonly T _buffer1;
    private readonly T _buffer2;
    private readonly Action<T> _write;

    private volatile bool _hasData = false;
    private volatile bool _useBuffer1 = true;
    private AutoResetEvent _publicWait;
    private AutoResetEvent _privateWait;
    #endregion

    #region Properties
    private T WriteBuffer => _useBuffer1 ? _buffer1 : _buffer2;
    private T ReadBuffer => _useBuffer1 ? _buffer2 : _buffer1;
    #endregion

    #region Constructors
    public DoubleBufferWorker(string threadName, T buffer1, T buffer2, Action<T> write) : base(threadName)
    {
      #region Validation
      if (threadName == null)
        throw new ArgumentNullException("threadName");
      if (buffer1 == null)
        throw new ArgumentNullException("buffer1");
      if (buffer2 == null)
        throw new ArgumentNullException("buffer2");
      #endregion
      _buffer1 = buffer1;
      _buffer2 = buffer2;
      _write = write;

      _privateWait = new AutoResetEvent(false);
      _publicWait = new AutoResetEvent(false);
    }
    #endregion

    #region Methods
    protected override void Work()
    {
      _privateWait.WaitOne();
      _write(WriteBuffer);
      _publicWait.Set();
    }

    public T ReadNextBuffer()
    {
      if (!_hasData)
      {
        //Trigger the write and wait for it to finish to get the initial data
        _privateWait.Set();
        _publicWait.WaitOne();
        _hasData = true;
      }
      else
        _publicWait.WaitOne();

      _useBuffer1 = !_useBuffer1;
      _privateWait.Set();
      return ReadBuffer;
    }
    #endregion

  }
}
