using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System
{
  public static class ICustomAttributeProviderExtensions
  {
    public static T GetCustomAttribute<T>(this ICustomAttributeProvider type) where T : Attribute
    {
      return GetCustomAttribute<T>(type, false);
    }

    public static T GetCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit) where T : Attribute
    {
      return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    }

    public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type) where T : Attribute
    {
      return HasCustomAttribute<T>(type, false);
    }

    public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit) where T : Attribute
    {
      return GetCustomAttribute<T>(type,inherit) != null;
    }
  }
}
