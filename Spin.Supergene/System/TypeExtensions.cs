using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
  public static class TypeExtensions
  {
    public static PropertyInfo GetIndexer(this Type type, params Type[] arguments) => type.GetProperties().First(x => x.GetIndexParameters().Select(y => y.ParameterType).SequenceEqual(arguments));
  }
}
