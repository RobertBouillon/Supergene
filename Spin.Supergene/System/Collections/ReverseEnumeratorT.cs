using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic;

class ReverseEnumerator<T> : IEnumerable<T>
{
  #region Fields
  private readonly IList<T> _innerCollection;
  #endregion
  #region Properties
  public IList<T> InnerCollection
  {
    get { return _innerCollection; }
  }
  #endregion
  #region Constructors
  public ReverseEnumerator(IList<T> innerCollection)
  {
    #region Validation
    if (innerCollection == null)
      throw new ArgumentNullException("innerCollection");
    #endregion
    _innerCollection = innerCollection;
  }
  #endregion

  #region IEnumerable<T> Members

  public IEnumerator<T> GetEnumerator()
  {
    int count = _innerCollection.Count;
    for (int i = 0; i < count; i++)
      yield return _innerCollection[count - i - 1];
  }

  #endregion
  #region IEnumerable Members

  IEnumerator IEnumerable.GetEnumerator()
  {
    int count = _innerCollection.Count;
    for (int i = 0; i < count; i++)
      yield return _innerCollection[count - i - 1];
  }

  #endregion
}
