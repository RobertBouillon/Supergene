using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class StringExtensions
  {
    public static string PadRight(this String source, int totalWidth, int minimumPadWidth)
    {
      return PadRight(source, totalWidth, minimumPadWidth, ' ');
    }

    public static string PadRight(this String source, int totalWidth, int minimumPadWidth, char paddingChar)
    {
      #region Validation
      if (minimumPadWidth >= totalWidth)
        throw new ArgumentOutOfRangeException("minimumPadWidth cannot be greater than totalWidth");
      #endregion
      if (totalWidth - source.Length < minimumPadWidth)
        totalWidth += source.Length + minimumPadWidth;

      return source.PadRight(totalWidth, paddingChar);
    }

    public static string Substring(this String source, int startIndex, string start, string end)
    {
      int s = source.IndexOf(start, startIndex);

      if (s == -1)
        throw new FormatException("Cannot find start text");

      int e = source.IndexOf(end, s);

      if (e == -1)
        throw new FormatException("Cannot find end text");

      int l = e - s;
      if (s < 0)
        return null;
      return source.Substring(s, l);
    }


    public static string[] Split(this String source, int chunkSize)
    {
      char[] cs = source.ToCharArray();
      List<String> ret = new List<string>();
      for (int i = 0; i < source.Length; i += chunkSize)
        ret.Add(new String(cs, i, Math.Min(chunkSize, cs.Length - i)));

      return ret.ToArray();
    }

    public static string StripControlCharacters(this string source)
    {
      return new string(source.Where(x => !Char.IsControl(x)).ToArray());
    }

    public static string Remove(this string source, string remove)
    {
      int index = source.IndexOf(remove);
      if (index < 0)
        return source;
      return source.Remove(index, remove.Length);
    }

    public static string Format(this string source, params object[] args) => String.Format(source, args);
  }
}
