using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class EnumExtension
  {
    //Now part of the BCL
    //public static bool HasFlag<T>(this Enum e, params T[] a) where T : struct
    //{
    //  foreach (T b in a)
    //  {
    //    int value = (int)Convert.ChangeType(e, typeof(int));
    //    int check = (int)Convert.ChangeType(b, typeof(int));

    //    if (check == 0)
    //      return value == 0;
        
    //    if ((value & check) != check)
    //      return false;
    //  }
    //  return true;
    //}

    public static bool IsFlag<T>(this Enum e, params T[] a) where T : struct
    {
      foreach (T b in a)
      {
        int value = (int)Convert.ChangeType(e, typeof(int));
        int check = (int)Convert.ChangeType(b, typeof(int));

        if ((value & check) != check)
          return false;
      }
      return true;
    }

    public static T AddFlag<T>(this Enum e, T a) where T : struct
    {
      int value = (int)Enum.ToObject(typeof(T), e);
      int check = (int)Enum.ToObject(typeof(T), a);
      return (T)Enum.ToObject(typeof(T), value | check); ;
    }

    public static T RemoveFlag<T>(this Enum e, T a) where T : struct
    {
      int value = (int)Convert.ChangeType(e, typeof(int));
      int check = (int)Convert.ChangeType(a, typeof(int));
      return (T)Enum.ToObject(typeof(T), (value | check) ^ check); ;
    }

    public static T ChangeFlag<T>(this Enum e, T a, bool add) where T : struct
    {
      if (add)
        return AddFlag(e, a);
      else
        return RemoveFlag(e, a);
    }


  }
}
