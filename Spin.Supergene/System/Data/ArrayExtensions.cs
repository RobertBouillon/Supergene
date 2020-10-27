using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Data
{
  public static class ArrayExtensions
  {
    public static string ToSqlXmlParam(this Array array)
    {
      return ToSqlXmlParam(array, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="encode">True to encode each element for XML</param>
    /// <returns></returns>
    /// <remarks>
    /// Set encode to false to improve performance
    /// </remarks>
    public static string ToSqlXmlParam(this Array array, bool encode)
    {
      StringBuilder sb = new StringBuilder("<d>");

      if(encode)
        foreach (object o in array)
          sb.AppendFormat("<i v=\"{0}\"/>", o.ToString().EncodeMarkup());
      else
        foreach (object o in array)
          sb.AppendFormat("<i v=\"{0}\"/>", o.ToString());


      sb.Append("</d>");
      return sb.ToString();
    }
  }
}
