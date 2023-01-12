using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System;

public static class RandomExtensions
{
  public static int Next(this Random rnd, Range<int> range) => rnd.Next(range.Start, range.End);
  public static TimeSpan Next(this Random rnd, Range<TimeSpan> time) => time.Start + TimeSpan.FromTicks((long)((time.End - time.Start).Ticks * rnd.NextDouble()));
}
