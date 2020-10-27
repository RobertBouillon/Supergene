using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.UnitTesting
{
  public static class UnitTest
  {
    #region Public Fields
    public static readonly string NewTestString = "UNIT TEST GENERATED";
    public static readonly string UpdatedTestString = "UNIT TEST UPDATED";
    public static readonly DateTime NewTestDate = new DateTime(2009, 1, 2, 3, 4, 5);
    public static readonly DateTime UpdatedTestDate = new DateTime(2010, 5, 4, 3, 2, 1);
    public static bool IsTesting = false;
    public static Dictionary<string, string> Parameters = new Dictionary<string, string>();
    #endregion
  }
}
