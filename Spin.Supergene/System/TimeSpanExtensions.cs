using System;
using System.Collections.Generic;
using System.Text;

namespace stdcorlib.System
{
  public static class TimeSpanExtensions
  {
    public static TimeSpan RoundToHours(this TimeSpan timeSpan) => new TimeSpan((long)timeSpan.TotalHours * TimeSpan.TicksPerSecond);
    public static TimeSpan RoundToSeconds(this TimeSpan timeSpan) => new TimeSpan((long)timeSpan.TotalSeconds * TimeSpan.TicksPerSecond);
    public static TimeSpan RoundToMinutes(this TimeSpan timeSpan) => new TimeSpan((long)timeSpan.TotalMinutes * TimeSpan.TicksPerMinute);
    public static TimeSpan Round(this TimeSpan time, TimeSpan nearest) => new TimeSpan((time.Ticks / nearest.Ticks) * nearest.Ticks);

    public static string ToVerboseString(this TimeSpan elapsed)
    {
      StringBuilder text = new StringBuilder();

      if (elapsed.Days >= 1 && elapsed.Days < 2)
        text.AppendFormat(" {0} day", elapsed.Days);
      else if (elapsed.Days > 0)
        text.AppendFormat(" {0} days", elapsed.Days);

      if (elapsed.Hours >= 1 && elapsed.Hours < 2)
        text.AppendFormat(" {0} hour", elapsed.Hours);
      else if (elapsed.Hours > 0)
        text.AppendFormat(" {0} hours", elapsed.Hours);

      if (elapsed.Minutes >= 1 && elapsed.Minutes < 2)
        text.AppendFormat(" {0} minute", elapsed.Minutes);
      else if (elapsed.Minutes >= 2)
        text.AppendFormat(" {0} minutes", elapsed.Minutes);

      if (elapsed.Seconds >= 1 && elapsed.Seconds < 2)
        text.AppendFormat(" {0} second", elapsed.Seconds);
      else if (elapsed.Seconds >= 2)
        text.AppendFormat(" {0} seconds", elapsed.Seconds);

      return text.ToString().Trim();
    }
  }
}
