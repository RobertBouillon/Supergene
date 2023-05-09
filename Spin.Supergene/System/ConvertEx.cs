using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class ConvertEx
  {
    public static T ChangeType<T>(object value) where T : struct => (T)ChangeType(value, typeof(T));
  }
}
