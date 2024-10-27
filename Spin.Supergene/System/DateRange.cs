using System;
using System.Collections.Generic;
using System.Text;

namespace System;

public class DateRange : IEnumerable<DateTime>
{
  #region Static Methods
  public static DateRange Parse(string date) => Parse(date, date);
  public static DateRange Parse(string startDate, string endDate) => new DateRange(DateTime.Parse(startDate), DateTime.Parse(endDate));
  #endregion
  #region Fields
  private DateTime _startDate;
  private DateTime _endDate;
  #endregion
  #region Properties

  public DateTime StartDate
  {
    get { return _startDate; }
    set { _startDate = value; }
  }

  public DateTime EndDate
  {
    get { return _endDate; }
    set { _endDate = value; }
  }

  public TimeSpan Span => _endDate - _startDate;
  #endregion

  #region Constructors
  public DateRange(string startDate, string endDate)
    : this(DateTime.Parse(startDate), DateTime.Parse(endDate)) { }

  public DateRange(DateTime startDate, DateTime endDate)
  {
    if (_startDate > _endDate)
      throw new ArgumentOutOfRangeException("StartDate cannot occur after the EndDate");

    _startDate = startDate;
    _endDate = endDate;
  }

  public DateRange(DateTime date) : this(date, date) { }

  /// <summary>
  /// Exlusively within the dates. 
  /// </summary>
  /// <param name="date"></param>
  /// <param name="inclusive">Include the Start Date and End Date (%gt;= and &lt;=)</param>
  /// <returns></returns>
  public bool IsWithin(DateTime date, bool inclusive = true) => inclusive ? date <= _endDate && date >= _startDate : date < _endDate && date > _startDate;

  /// <summary>
  /// Inclusively within the dates. 
  /// </summary>
  public bool IsInclusive(DateTime date) => date <= _endDate && date >= _startDate;

  /// <summary>
  /// Less than the End Date, but greater than or equal to the start date.
  /// </summary>
  /// <param name="date"></param>
  /// <returns></returns>
  public bool IsBetween(DateTime date) => date < _endDate && date >= _startDate;
  #endregion
  #region Public Methods
  public bool IsBetween(DateTime date, bool startInclusive = false, bool endInclusive = false) => startInclusive ? _startDate <= date : _startDate < date && endInclusive ? _endDate >= date : _endDate > date;
  public TimeRange ToTimeRange() => new TimeRange(_startDate.TimeOfDay, _endDate.TimeOfDay);
  #endregion
  #region IEnumerable<DateTime> Members
  public IEnumerator<DateTime> GetEnumerator()
  {
    for (DateTime day = _startDate; day <= _endDate; day = day.AddDays(1))
      yield return day;
  }
  #endregion
  #region IEnumerable Members
  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
  {
    for (DateTime day = _startDate; day <= _endDate; day = day.AddDays(1))
      yield return day;
  }
  #endregion
  public IEnumerable<DateTime> Iterate(TimeSpan interval)
  {
    var start = StartDate.Round(interval);
    var end = EndDate.Round(interval);

    for (var current = start; current < end; current.Add(interval))
      yield return current;
  }
}
