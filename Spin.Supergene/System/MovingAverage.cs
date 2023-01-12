using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

public class MovingAverage
{
  #region Private Members
  private decimal _sum;
  private int _total;
  private double _value;
  private TimeSpan _span;
  private Queue<KeyValuePair<DateTime, Double>> _buffer = new Queue<KeyValuePair<DateTime, double>>(1024);
  private DateTime _lowerBound = DateTime.MinValue;
  private DateTime _last;
  #endregion
  #region Public Property Declarations
  public double Value
  {
    get
    {
      lock (this)
      {
        if ((_last - _lowerBound) > _span)
        {
          _lowerBound = _last - _span;
          while (_buffer.Peek().Key < _lowerBound)
          {
            _sum -= (decimal)_buffer.Dequeue().Value;
            _total--;
          }
          _value = (double)(_sum / _total);
        }
      }

      return _value;
    }
  }
  #endregion
  #region Constructors
  public MovingAverage(TimeSpan span)
  {
    #region Validation
    if (span < TimeSpan.Zero)
      throw new ArgumentException("span must be greater than zero", "span");
    #endregion
    _span = span;
  }
  #endregion

  #region Public Methods
  public void AppendValue(DateTime time, double value)
  {
    lock (this)
    {
      if (time < _last)
        throw new InvalidOperationException("Time cannot be less than than the last value appended.");

      _last = time;
      _sum += (decimal)value;
      _total++;

      _buffer.Enqueue(new KeyValuePair<DateTime, Double>(time, value));
    }
  }
  #endregion

}
