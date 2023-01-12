using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data;

public static class IDataParameterCollectionExtensions
{
  public static void AddRange(this IDataParameterCollection collection, IList<IDataParameter> parms)
  {
    foreach (IDataParameter p in parms)
      collection.Add(p);
  }

}
