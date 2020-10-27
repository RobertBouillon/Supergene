using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace System.Threading.Workers
{
  // Replaced by the System.Modules.Heartbeat class, which is more robust and stable.

  /// <summary>
  /// A public timer used by the application to check time-sensitive events
  /// </summary>
  public class Heartbeat : TimedWorker
  {
    #region Fields
    //private DateTime _lastActivity;
    private TimeSpan _idleTimeout = TimeSpan.Zero;
    private bool _invokeIdle = false;
    private Stopwatch _monitor = new Stopwatch();
    private Stopwatch _idleMonitor = new Stopwatch();
    private DateTime _offset;
    #endregion
    #region Properties
    public TimeSpan IdleTimeout
    {
      get { return _idleTimeout; }
      set
      {
        lock (this)
        {
          ResetIdle();
          _idleTimeout = value;
        }
      }
    }
    #endregion
    #region Constructors
    public Heartbeat() : base("Heartbeat",TimeSpan.FromSeconds(1))
    {
      _monitor.Start();
      _idleMonitor.Start();
      _offset = DateTime.Now;
    }

    #endregion

    #region Events
    /// <summary>
    /// Occurs when a new second has elapsed
    /// </summary>
    public event EventHandler SecondElapsed;
    /// <summary>
    /// Occurs when a new Minute has elapsed
    /// </summary>
    public event EventHandler MinuteElapsed;
    /// <summary>
    /// Occurs when a new Hour has elapsed
    /// </summary>
    public event EventHandler HourElapsed;
    /// <summary>
    /// Occurs when a new Day has elapsed
    /// </summary>
    public event EventHandler DayElapsed;
    /// <summary>
    /// Occurs when a new Week has elapsed
    /// </summary>
    public event EventHandler WeekElapsed;
    /// <summary>
    /// Occurs when a new Month has elapsed
    /// </summary>
    public event EventHandler MonthElapsed;
    /// <summary>
    /// Occurs when a new Year has elapsed
    /// </summary>
    public event EventHandler YearElapsed;
    /// <summary>
    /// Occurs when the user has gone idle
    /// </summary>
    public event EventHandler IdleElapsed;

    /// <summary>
    /// Occurs when an exception occurs on a thread.
    /// </summary>
    public event EventHandler<ErrorEventArgs> Error;

    #region ErrorEventArgs Subclass
    public class ErrorEventArgs : EventArgs
    {
      #region Fields
      private readonly Exception _error;
      #endregion
      #region Properties
      public Exception Error
      {
        get { return _error; }
      }
      #endregion
      #region Constructors
      internal ErrorEventArgs(Exception error)
      {
        #region Validation
        if (error == null)
          throw new ArgumentNullException("error");
        #endregion
        _error = error;
      }
      #endregion
    }
    #endregion
    #region Virtual Methods

    protected void OnError(Exception error)
    {
      OnError(new ErrorEventArgs(error));
    }

    protected virtual void OnError(ErrorEventArgs e)
    {
      if (Error != null)
        Error(this, e);
    }
    #endregion
    #endregion
    #region Methods
    /// <summary>
    /// This will force the IdleElapsed event to fire on the next beat.
    /// </summary>
    /// <remarks>We don't fire this directly because this should be a simulated event. The simulation won't be accurate if we raise the event on the calling thread; it needs to originate from the timer thread.</remarks>
    public void ForceIdle()
    {
      _invokeIdle = true;
    }

    public void ResetIdle()
    {
      //lock(this)
        //_lastActivity = DateTime.Now;
      _idleMonitor.Restart();
    }

    private void Beat()
    {
      //NOTE: We don't lock this because we can easily deadlock
      
      //DateTime now = DateTime.Now;
      DateTime now = _offset + _monitor.Elapsed;
      EventArgs ea = EventArgs.Empty;

      try
      {
        if (SecondElapsed != null)
          SecondElapsed(this, ea);

        if (now.Second == 0)
        {
          if (MinuteElapsed != null)
            MinuteElapsed(this, ea);

          if (now.Minute == 0)
          {
            if (HourElapsed != null)
              HourElapsed(this, ea);

            if (now.Hour == 0)
            {
              if (DayElapsed != null)
                DayElapsed(this, ea);

              if (now.DayOfWeek == DayOfWeek.Sunday)
                if (WeekElapsed != null)
                  WeekElapsed(this, ea);

              if (now.Day == 1)
              {
                if (MonthElapsed != null)
                  MonthElapsed(this, ea);

                if (now.Month == 1)
                  if (YearElapsed != null)
                    YearElapsed(this, ea);
              }
            }
          }
        }

        if (_idleTimeout != TimeSpan.Zero)
        {
          if (_idleMonitor.Elapsed > _idleTimeout || _invokeIdle)
          {
            if (IdleElapsed != null)
            {
              _idleMonitor.Restart();
              IdleElapsed(this, EventArgs.Empty);
            }
          }
        }
      }
      catch (Exception ex)
      {
        OnError(ex);
      }
    }
    #endregion

    #region TimedWorker Members
    protected override void CancelWork()
    {
    }

    protected override void Work()
    {
      ThreadPool.QueueUserWorkItem((x) => Beat());
    }
    #endregion
  }
}
