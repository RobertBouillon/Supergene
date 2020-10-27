using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text.RegularExpressions
{
  public static class RegexExtensions
  {
    public static Match TryMatch(this Regex regex, string source)
    {
      var match = regex.Match(source);
      return match.Success ? match : null;
    }

    public static bool TryMatch(this Regex regex, string source, out Match match)
    {
      match = regex.Match(source);
      return match.Success;
    }
  }
}
