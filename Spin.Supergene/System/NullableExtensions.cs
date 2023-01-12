using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System;

public static class NullableExtensions
{
  public static object ToObject<T>(this Nullable<T> o) where T : struct => (o.HasValue) ? (object)o.Value : null;
}
