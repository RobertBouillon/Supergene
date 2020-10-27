using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.UnitTesting
{
  class TypeTestResultCollection : CollectionBase<TypeTestResult>
  {
    #region Constructors

    public TypeTestResultCollection(IEnumerable<TypeTestResult> source) : base(source)
    {
    }

    public TypeTestResultCollection()
    {

    }


    #endregion

    #region Indexers

    public TypeTestResult this[Type index]
    {
      get
      {
        foreach (TypeTestResult i in this)
          if (i.Profile.Type == index)
            return i;
        return null;
      }
    }
	
    #endregion

    #region Methods
    public bool Contains(Type type)
    {
      return this[type] != null;
    }
    #endregion
  }
}
