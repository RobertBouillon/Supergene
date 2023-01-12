using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO;

public static class MemoryStreamExtensions
{
  public static MemoryStream Clone(this MemoryStream ms)
  {
    var ret = new MemoryStream(ms.Capacity);
    ms.CopyTo(ret);
    ret.Position = ms.Position;
    return ret;
  }
}
