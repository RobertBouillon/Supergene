using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace System.Threading.Workers
{
  /// <summary>
  /// Performs work on a background thread
  /// </summary>
  public abstract class TimedWorker : Worker
  {
    #region Fields
    private TimeSpan _interval;
    private Stopwatch _stopwatch;
    private bool _isHighResolution;
    private bool _isSynchronous;
    private TimeSpan _spinResolution = TimeSpan.FromMilliseconds(15);
    private TimeSpan _realInterval;
    private TimeSpan _next;
    private volatile bool _hasWork;
    #endregion

    #region Properties
    public bool IsSynchronous
    {
      get { return _isSynchronous; }
      set
      {
        if (IsStarted)
          throw new InvalidOperationException("Cannot change this property while the worker is running");
        _isSynchronous = value;
      }
    }

    public bool IsHighResolution
    {
      get { return _isHighResolution; }
      set
      {
        if (!Stopwatch.IsHighResolution)
          throw new InvalidOperationException("This platform does not support high-resolution timers");

        if (IsStarted)
          throw new InvalidOperationException("Cannot change this property while the worker is running");
        _isHighResolution = value;
      }
    }

    public TimeSpan Interval
    {
      get { return _interval; }
      set
      {
        if (IsStarted)
          throw new InvalidOperationException("Cannot change this property while the worker is running");
        _interval = value;
      }
    }
    #endregion

    #region Constructors
    public TimedWorker(TimeSpan interval)
    {
      _interval = interval;
    }

    public TimedWorker(string name, TimeSpan interval)
      : base(name)
    {
      #region Validation
      if (interval.TotalMilliseconds <= 0)
        throw new ArgumentOutOfRangeException("interval", "interval must be breater than zero");
      #endregion
      _interval = interval;
      _stopwatch = new Stopwatch();
    }

    #endregion

    #region Overrides
    protected override bool HasWork => _hasWork;

    protected override void WaitForWork()
    {
      var wait = _isHighResolution ? _next - _stopwatch.Elapsed - _spinResolution : _next - _stopwatch.Elapsed;
      var lwait = Math.Min(WaitDelay.Ticks, wait.Ticks);
      if (lwait > 0)
      {
        Thread.Sleep(TimeSpan.FromTicks(lwait));
        return;
      }

      if (_isHighResolution)
        while (_stopwatch.Elapsed < _next) ;

      _next += _interval;

      _hasWork = true;
    }

    protected override void OnStarting(CancelEventArgs e)
    {
      _stopwatch = new Stopwatch();
      _stopwatch.Start();
      _realInterval = _isHighResolution ? _interval - _spinResolution : _interval;
      base.OnStarting(e);
    }

    protected override void OnWorked(WorkPerformedEventArgs e)
    {
      _hasWork = false;
      base.OnWorked(e);
    }
    #endregion
  }
}
