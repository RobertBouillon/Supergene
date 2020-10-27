using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace excorlib.System
{
  public class StringComparer : IEqualityComparer<String>
  {
    #region Fields
    private readonly StringComparison _comparison;
    #endregion
    #region Properties

    public StringComparison Comparison
    {
      get { return _comparison; }
    }

    #endregion
    #region Constructors

    public StringComparer(StringComparison comparison)
    {
      _comparison = comparison;
    }
    #endregion
    #region IEqualityComparer<string> Members

    public bool Equals(string x, string y)
    {
      return x.Equals(y, _comparison);
    }

    public int GetHashCode(string obj)
    {
      return obj.GetHashCode();
    }

    #endregion
  }
}
