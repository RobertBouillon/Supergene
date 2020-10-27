using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace System.Collections.Specialized
{
  public static class StringCollectionExtension
  {
    public static string Join(this StringCollection sc, string format, params object[] args)
    {
      return Join(sc, String.Format(format, args));
    }

    public static string Join(this StringCollection sc, string separator)
    {
      return String.Join(separator, ToList(sc).ToArray());
    }

    public static List<string> ToList(this StringCollection sc)
    {
      return new List<string>(sc.Cast<string>());
    }

    public static int Add(this StringCollection sc, string format, params object[] args)
    {
      return sc.Add(String.Format(format, args));
    }
  }
}
