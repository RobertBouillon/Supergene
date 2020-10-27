using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.UnitTesting
{
  public class TypeTestProfileCollection : CollectionBase<TypeTestProfile>
  {
    #region Constructors
    public TypeTestProfileCollection()
    {

    }

    public TypeTestProfileCollection(TypeTestProfile[] profiles)
      : base(profiles)
    {

    }
    #endregion

    #region Indexers
    public TypeTestProfile this[Type type]
    {
      get
      {
        foreach (TypeTestProfile i in this)
          if (i.Type == type)
            return i;
        return null;
      }
    }
    #endregion
    #region Methods
    public bool Contains(Type type)
    {
      foreach (TypeTestProfile profile in this)
        if (profile.Type == type)
          return true;
      return false;
    }
    #endregion
  }
}
