using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Threading.Workers
{
  // Obsolete by QueueWorker. Checked the code for circular and it's garbage anyway. I suck(ed).

  public abstract class CircularQueueWorker<T> : Worker
  {
    #region Fields
    private bool _isWaiting = false;
    private AutoResetEvent _cwaitHandle = new AutoResetEvent(false);
    private AutoResetEvent _fwaitHandle = new AutoResetEvent(false);
    //private Queue<T> _innerQueue = new Queue<T>();
    private int _bufferSize = 2048;
    private int _maxSize = 2047;
    private int _writePosition = 0;
    private int _readPosition = 0;
    private int _count = 0;
    private bool _isFull = false;
    private T[] _buffer;
    #endregion
    #region Properties
    public int Count
    {
      get
      {
        return Thread.VolatileRead(ref _count);
      }
    }
    #endregion


    #region Constructors
    public CircularQueueWorker(string name, int bufferSize)
      : base(name)
    {
      #region Validation
      if (Int32.MaxValue % bufferSize != bufferSize - 1)
        throw new ArgumentException("bufferSize must be able to evenly rollover Int32.Max");
      #endregion
      _bufferSize = bufferSize;
      _maxSize = _bufferSize - 1;
      _buffer = new T[bufferSize];
    }
    #endregion

    #region Methods
    public void WaitForCompletion(bool stopWhenComplete)
    {
      if (Count > 0)
      {
        _isWaiting = true;
        _cwaitHandle.WaitOne();
      }

      if (stopWhenComplete)
        Stop();
    }

    protected int Enqueue(T item)
    {
      //int c = Thread.VolatileRead(ref _count);
      if (Thread.VolatileRead(ref _count) == _maxSize)
      {
        _isFull = true;
        _fwaitHandle.WaitOne();
      }

      if (Thread.VolatileRead(ref _count) > _maxSize)
      {
        throw new Exception();
      }

      int pos = Interlocked.Increment(ref _writePosition);
      if (pos < 0)
        pos += Int32.MaxValue;

      _buffer[pos % _bufferSize] = item;
      Interlocked.Increment(ref _count);

      WaitHandle.Set();
      return _count;
    }
    #endregion

    #region Abstract Declarations
    
    public abstract void PerformWork(T item);
    #endregion

    #region Overrides
    protected override bool CanWork
    {
      get
      {
        return Count > 0;
      }
    }

    protected override void OnStopped(EventArgs e)
    {
      _readPosition = _writePosition;
      base.OnStopped(e);
    }

    protected override void Work()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      while (_count > 0)
      {
        //DateTime started = DateTime.Now;  //DateTime.Now is expensive.
        sw.Restart();

        int pos = Interlocked.Increment(ref _readPosition);
        if (pos < 0)
          pos += Int32.MaxValue;
        
        pos = pos % _bufferSize;
        T item = _buffer[pos % _bufferSize];

        try
        {
          PerformWork(item);
          Interlocked.Decrement(ref _count);
        }
        catch (Exception ex)
        {
          OnError(ex, item);
        }

        if (_isFull)
          _fwaitHandle.Set();

        OnWorkPerformed(sw.Elapsed, item);
      }

      if (_isWaiting)
        if (_count == 0)
          _cwaitHandle.Set();

      if (_count == _bufferSize)
        _fwaitHandle.Set();


    }
    #endregion

    #region Events

    #region WorkPerformedEventArgs Subclass
    public class WorkPerformedEventArgs : EventArgs
    {
      #region Fields
      private readonly TimeSpan _duration;
      private readonly T _item;
      #endregion
      #region Properties
      public T Item
      {
        get { return _item; }
      }

      public TimeSpan Duration
      {
        get { return _duration; }
      }
      #endregion
      #region Constructors
      internal WorkPerformedEventArgs(TimeSpan duration, T item)
      {
        #region Validation
        if (duration == null)
          throw new ArgumentNullException("duration");
        if (item == null)
          throw new ArgumentNullException("item");
        #endregion
        _duration = duration;
        _item = item;
      }
      #endregion
    }
    #endregion

    public new event EventHandler<WorkPerformedEventArgs> WorkPerformed;

    protected void OnWorkPerformed(TimeSpan duration, T item)
    {
      OnWorkPerformed(new WorkPerformedEventArgs(duration, item));
    }

    protected virtual void OnWorkPerformed(WorkPerformedEventArgs e)
    {
      if (WorkPerformed != null)
        WorkPerformed(this, e);
    }


    #region ErrorEventArgs Subclass
    public class ErrorEventArgs : EventArgs
    {
      #region Fields
      private readonly Exception _exception;
      private readonly T _item;
      #endregion
      #region Properties
      public T Item
      {
        get { return _item; }
      }
      
      public Exception Exception
      {
        get { return _exception; }
      }
      #endregion
      #region Constructors
      internal ErrorEventArgs(Exception exception, T item)
      {
        #region Validation
        if (exception == null)
          throw new ArgumentNullException("exception");
        #endregion
        _exception = exception;
        _item = item;
      }
      #endregion
    }
    #endregion

    public event EventHandler<ErrorEventArgs> Error;

    protected void OnError(Exception exception, T item)
    {
      OnError(new ErrorEventArgs(exception, item));
    }

    protected virtual void OnError(ErrorEventArgs e)
    {
      if (Error != null)
        Error(this, e);
    }
    #endregion
  }
}
