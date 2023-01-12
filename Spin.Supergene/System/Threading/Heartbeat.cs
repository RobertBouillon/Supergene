using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

namespace System.Threading;

public class Heartbeat
{
  public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
  public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
  public static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
  public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

  #region Fields
  private DateTime _lastSecond;
  private DateTime _lastMinute;
  private DateTime _lastHour;
  private DateTime _lastDay;

  private bool _secondActive;
  private bool _minuteActive;
  private bool _hourActive;
  private bool _dayActive;
  private bool _isActive;

  private bool _isEnabled;

  private Stopwatch _stopwatch;
  private DateTime _offset;
  #endregion

  #region Properties

  public bool IsEnabled
  {
    get { return _isEnabled; }
    set { _isEnabled = value; }
  }

  #endregion


  #region Constructors
  public Heartbeat() : this(DateTime.Now)
  {

  }

  public Heartbeat(DateTime lastBeat)
  {
    _stopwatch = new Stopwatch();
    _offset = lastBeat;
    _stopwatch.Start();

    _lastDay =
      _lastHour =
      _lastMinute =
      _lastSecond =
      lastBeat;
  }
  #endregion

  #region Methods

  public void Assert(Action<TimeSpan> client)
  {
    if (!(_isEnabled || _isActive))
      return;

    if (_secondActive)
      client(OneSecond);
    if (_minuteActive)
      client(OneMinute);
    if (_hourActive)
      client(OneHour);
    if (_dayActive)
      client(OneDay);
  }

  public void Check()
  {
    Check(_offset + _stopwatch.Elapsed);
  }

  private void Check(DateTime newBeat)
  {
    DateTime now = newBeat;

    if (_secondActive = (now - _lastSecond > OneSecond)) _lastSecond = now;
    if (_minuteActive = (now - _lastMinute > OneMinute)) _lastMinute = now;
    if (_hourActive = (now - _lastHour > OneHour)) _lastHour = now;
    if (_dayActive = (now - _lastDay > OneDay)) _lastDay = now;

    _isActive =
      _secondActive ||
      _minuteActive ||
      _hourActive ||
      _dayActive;
  }
  #endregion
}
