using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

public class TimeRange
{
  #region Fields
  private TimeSpan _startTime;
  private TimeSpan _endTime;
  #endregion
  #region Public Properties
  public TimeSpan StartTime
  {
    get { return _startTime; }
    set { _startTime = value; }
  }

  public TimeSpan EndTime
  {
    get { return _endTime; }
    set { _endTime = value; }
  }

  public TimeSpan Span
  {
    get { return _endTime - _startTime; }
  }
  #endregion
  #region Constructors
  public TimeRange()
  {

  }

  public TimeRange(TimeSpan startTime, TimeSpan endTime)
  {
    if (endTime < startTime)
      throw new ArithmeticException("endTime cannot be a time before startTime");

    _endTime = endTime;
    _startTime = startTime;
  }
  #endregion
}
