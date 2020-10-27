using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
  public class CollectionComparer<T> : CollectionComparer<T, T>
  {
    #region Constructors
    public CollectionComparer()
    {

    }
    #endregion

    #region Overrides
    protected override bool IsSame(T left, T right)
    {
      return left.Equals(right);
    }

    protected override bool IsEqual(T left, T right)
    {
      return left.Equals(right);
    }
    #endregion

  }
}
