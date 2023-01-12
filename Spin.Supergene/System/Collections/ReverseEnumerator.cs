using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Collections;

public class ReverseEnumerator : IEnumerable
{
  #region Fields
  private readonly IList _innerCollection;
  #endregion
  #region Properties
  public ICollection InnerCollection
  {
    get { return _innerCollection; }
  }
  #endregion
  #region Constructors
  public ReverseEnumerator(IList innerCollection)
  {
    #region Validation
    if (innerCollection == null)
      throw new ArgumentNullException("innerCollection");
    #endregion
    _innerCollection = innerCollection;
  }
  #endregion
  #region IEnumerable Members

  public IEnumerator GetEnumerator()
  {
    int count = _innerCollection.Count;
    for (int i = 0; i < count; i++)
      yield return _innerCollection[count - i - 1];
  }

  #endregion
}
