using System;
using System.Collections.Generic;
using System.Text;

namespace System;

public class ExceptionCollection : CollectionBase<Exception>
{
  #region Constructors
  public ExceptionCollection()
  {

  }

  public ExceptionCollection(IEnumerable<Exception> exceptions)
    : base(exceptions)
  {

  }
  #endregion
}
