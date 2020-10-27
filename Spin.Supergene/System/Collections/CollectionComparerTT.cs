using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
  public abstract class CollectionComparer<TLeft,TRight>
  {
    #region Results
    public class Results
    {
      #region Fields
      private List<TLeft> _missingLeft = new List<TLeft>();
      private List<TRight> _missingRight = new List<TRight>();
      private Dictionary<TLeft, TRight> _different = new Dictionary<TLeft,TRight>();
      private Dictionary<TLeft, TRight> _equal = new Dictionary<TLeft,TRight>();

      #endregion
      #region Properties
      public Dictionary<TLeft, TRight> Equal
      {
        get { return _equal; }
      }

      public Dictionary<TLeft, TRight> Different
      {
        get { return _different; }
      }

      public List<TLeft> MissingLeft
      {
        get { return _missingLeft; }
      }

      public List<TRight> MissingRight
      {
        get { return _missingRight; }
      }
      #endregion
      #region Constructor
      public Results()
      {

      }
      #endregion
    }
    #endregion

    #region Fields
    //private TLeft _leftList;
    //private TRight _rightList;
    #endregion

    #region Constructors
    public CollectionComparer()
    {

    }
    #endregion

    #region Methods
    public Results Compare(IEnumerable<TLeft> leftList, IEnumerable<TRight> rightList)
    {
      Results results = new CollectionComparer<TLeft,TRight>.Results();

      results.MissingLeft.AddRange(leftList.Where(x => rightList.Count(y => IsSame(x, y)) == 0));
      results.MissingRight.AddRange(rightList.Where(x => leftList.Count(y => IsSame(y, x)) == 0));

      foreach (TLeft left in leftList)
      {
        TRight right = rightList.FirstOrDefault(x => IsSame(left, x));
        if (right == null)
          continue;
        if (!IsEqual(left, right))
          results.Different.Add(left, right);
        else
          results.Equal.Add(left, right);
      }

      return results;
    }

    protected abstract bool IsSame(TLeft left, TRight right);
    protected abstract bool IsEqual(TLeft left, TRight right);
    #endregion
  }
}
