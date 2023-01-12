using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System;

public static class EnumEx
{
  public static void ForEach<T>(Action<T> action) where T : struct
  {
    #region Validation
    if (!typeof(T).IsEnum)
      throw new ArgumentException("T must be an enumeration");
    #endregion
    foreach (string name in Enum.GetNames(typeof(T)))
      action((T)Enum.Parse(typeof(T), name, true));
  }

  public static IEnumerable<T> GetEnumerator<T>()
  {
    #region Validation
    if (!typeof(T).IsEnum)
      throw new ArgumentException("T must be an enumeration");
    #endregion
    foreach (string name in Enum.GetNames(typeof(T)))
      yield return (T)Enum.Parse(typeof(T), name, true);
  }

  public static T Parse<T>(string memberName)
  {
    return (T)Enum.Parse(typeof(T), memberName, true);
  }

  public static void ForEach<T>(this Enum e, Action<T> action) where T : struct
  {
    foreach (T val in Enum.GetValues(typeof(T)))
      action(val);
  }

  public static IEnumerable<TReturn> Select<TEnum, TReturn>(this Enum e, Func<TEnum, TReturn> action) where TEnum : struct
  {
    foreach (TEnum val in Enum.GetValues(typeof(TEnum)))
      yield return action(val);
  }

  public static void ForEachFlag<T>(this Enum e, Action<T> action) where T : Enum
  {
    foreach (T val in Enum.GetValues(typeof(T)))
      if (e.HasFlag(val))
        action(val);
  }
}
