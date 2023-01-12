using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Text.RegularExpressions;

public static class MatchCollectionExtension
{
  public static string[] ToResultArray(this MatchCollection mc, int group)
  {
    string[] ret = new string[mc.Count];
    int index = 0;
    foreach (Match m in mc)
      ret[index++] = m.Groups[group].Value;

    return ret;
  }

  public static string Format(this Regex regex, string match, string format)
  {
    var m = regex.Match(match);
    string[] args = new string[m.Groups.Count];
    for (int i = 0; i < m.Groups.Count; i++)
      args[i] = m.Groups[i].Value;

    return String.Format(format, args);
  }

  public static string Format(this Regex regex, string match, string format, IFormatProvider provider)
  {
    var m = regex.Match(match);
    string[] args = new string[m.Groups.Count];
    for (int i = 0; i < m.Groups.Count; i++)
      args[i] = m.Groups[i].Value;

    return String.Format(provider, format, args);
  }

  public static string ReplaceFormat(this Regex regex, string match, string replace)
  {
    return regex.Replace(match, (x) => String.Format(replace, x.Groups.OfType<Group>().Select(y => y.Value).ToArray()));
  }
}
