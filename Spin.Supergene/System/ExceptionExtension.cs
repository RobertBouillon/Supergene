using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

public static class ExceptionExtension
{
  public static string ToInnerExceptionString(this Exception e)
  {
    return ToInnerExceptionString(e, " -> ");
  }

  public static string ToInnerExceptionString(this Exception e, string delimiter)
  {
    StringBuilder sb = new StringBuilder();
    Exception ex;
    for (ex = e; ex.InnerException != null; ex = ex.InnerException)
      if (ex != null)
        sb.AppendFormat("{0}{1}", ex.Message, delimiter);

    sb.Append(ex.Message);
    return sb.ToString();
  }
}
