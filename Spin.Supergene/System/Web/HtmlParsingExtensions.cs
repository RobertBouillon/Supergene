using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Web
{
  public static class HtmlParsingExtensions
  {
    #region Fields
    private static readonly Dictionary<string, string> _htmlConversionStrings = new Dictionary<string, string>();
    private static readonly Regex _reHtmlAscii = new Regex(@"&#\s{2,4};", RegexOptions.Compiled);
    private static readonly Regex _reUrlAscii = new Regex(@"%([\dA-Fa-f]{2})", RegexOptions.Compiled);
    private static readonly Regex _removeHtml = new Regex(@"(<(.|\n)+?>)", RegexOptions.Compiled);
    #endregion
    #region Extensions

    public static string DecodeHtml(this String source)
    {
      string html = _reHtmlAscii.Replace(source, (m) => { return new String((char)Int32.Parse(m.Groups[1].Value), 1); });

      StringBuilder sb = new StringBuilder(html);
      foreach (string s in _htmlConversionStrings.Keys)
        if (html.Contains(s))
          sb.Replace(s, _htmlConversionStrings[s]);

      return _removeHtml.Replace(sb.ToString(), String.Empty);
    }

    public static string DecodeUrl(this String source)
    {
      return _reUrlAscii.Replace(source, (m) => { return new String((char)Int32.Parse(m.Groups[1].Value,System.Globalization.NumberStyles.AllowHexSpecifier), 1); });
    }

    public static string EncodeMarkup(this String source)
    {
      string ret = source;
      foreach (string key in _htmlConversionStrings.Keys)
        ret = ret.Replace(_htmlConversionStrings[key], key);

      return ret;
    }
    #endregion
    #region Constructors
    static HtmlParsingExtensions()
    {
      _htmlConversionStrings.Add("&amp;", "&");
      _htmlConversionStrings.Add("&quot;", "\"");
      _htmlConversionStrings.Add("&lt;", "<");
      _htmlConversionStrings.Add("&gt;", ">");
      _htmlConversionStrings.Add("&apos;", "'");
      _htmlConversionStrings.Add("<br>", "\n");
      _htmlConversionStrings.Add("</p>", "\n");
    }
    #endregion
  }
}
