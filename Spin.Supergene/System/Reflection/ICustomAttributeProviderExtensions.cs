using System.Linq;
using System.Reflection;

namespace System;

public static class ICustomAttributeProviderExtensions
{
  public static T GetCustomAttribute<T>(this ICustomAttributeProvider type) where T : Attribute => 
    GetCustomAttribute<T>(type, false);

  public static T GetCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit) where T : Attribute => 
    type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;

  public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type) where T : Attribute => 
    HasCustomAttribute<T>(type, false);

  public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type, bool inherit) where T : Attribute => 
    GetCustomAttribute<T>(type, inherit) != null;
}
