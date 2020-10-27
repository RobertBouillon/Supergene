using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Text
{
  public static class StringBuilderExtensions
  {
    public static void Clear(this StringBuilder sb) => sb.Remove(0, sb.Length);
    public static void AppendLines(this StringBuilder sb, IEnumerable<String> lines)
    {
      foreach (var line in lines)
        sb.AppendLine(line);
    }
  }
}
