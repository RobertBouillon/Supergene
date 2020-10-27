using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Workers
{
  public abstract class QueueWorker<T> : EventWorker, IEnumerable<T>
  {
    #region Fields
    private IProducerConsumerCollection<T> _queue;
    private AutoResetEvent _handle = new AutoResetEvent(false);
    private T _item;
    #endregion

    #region Constructors
    public QueueWorker() => _queue = new ConcurrentQueue<T>();
    public QueueWorker(IProducerConsumerCollection<T> queue)
    {
      #region Validation
      if (queue == null)
        throw new ArgumentNullException(nameof(queue));
      #endregion
      _queue = queue;
    }
    public QueueWorker(string name) : this(name, new ConcurrentQueue<T>()){}
    public QueueWorker(string name, IProducerConsumerCollection<T> queue) : base(name)
    {
      #region Validation
      if (queue == null)
        throw new ArgumentNullException(nameof(queue));
      #endregion
      _queue = queue;
    }

    #endregion

    #region Methods
    public virtual void Enqueue(T item)
    {
      // Just keep retrying
      while (!_queue.TryAdd(item)) ;
      _handle.Set();
    }
    #endregion

    #region Abstract Declarations
    public abstract void Work(T item);
    #endregion

    #region Overrides
    protected override bool HasWork => _queue.Count > 0;
    protected override WaitHandle Handle => _handle;

    public void Flush()
    {
      while (_queue.Count > 0)
        Thread.Sleep(WaitDelay);
    }

    public override bool Stop(TimeSpan timeout, bool kill)
    {
      return base.Stop(timeout, kill);
    }

    protected override void Work()
    {
      if (_queue.TryTake(out T item))
        Work(_item = item);
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

    public new event EventHandler<WorkPerformedEventArgs> Worked;
    protected override void OnWorked(Worker.WorkPerformedEventArgs e)
    {
      Worked?.Invoke(this, new WorkPerformedEventArgs(e.Duration, _item));
      base.OnWorked(e);
    }

    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    #endregion
  }
}
