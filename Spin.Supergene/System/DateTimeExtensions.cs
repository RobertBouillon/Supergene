using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class DateTimeExtensions
  {
    public static DateTime RoundToMinute(this DateTime dt)
    {
      return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
    }

    public static DateTime RoundToSeconds(this DateTime dt)
    {
      return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
    }

    public static DateTime Round(this DateTime dt, TimeSpan roundToNext)
    {
      return new DateTime((dt.Ticks / roundToNext.Ticks) * roundToNext.Ticks);
    }

    public static double ToJulianDate(this DateTime date)
    {
      return date.ToOADate() + 2415018.5;
    }

    public static DateTime GetWeekEnd(this DateTime date, DayOfWeek weekEndDate)
    {
      var diff = weekEndDate - date.DayOfWeek;
      return date.AddDays((diff + 7) % 7);
    }
  }
}
