using System;
using System.Collections;

namespace System.Collections.Specialized
{
	/// <summary>
	/// Provides a strongly-typed Integer-based index.
	/// </summary>
	/// <remarks>Indexes are used to store integer data pointing to Array locations, referencable by the host type, in this case an Int32.</remarks>
	public class Int32Index : DictionaryBase
	{
    #region ctors
		public Int32Index()
		{
		}
    #endregion
    #region Indexers
    public int this[int key]
    {
      get
      {
        object ret = Dictionary[key];
        if(ret==null)
          return -1;
        return (int)ret;
      }
      set{Dictionary[key]=value;}
    }
    #endregion
    #region Public Methods

    public void Add(int key, int arrayIndex)
    {
      Dictionary.Add(key,arrayIndex);
    }
    public void Remove(int key)
    {
      //Recalculate the index.
      int index = (int)Dictionary[key];
      int keycount = Dictionary.Keys.Count;
      int[] keys = new int[keycount];
      Dictionary.Keys.CopyTo(keys,0);
      for(int i=0;i<keycount;i++)
      {
        int ckey = keys[i];
        if((int)Dictionary[ckey]>index)
          Dictionary[ckey]=(int)Dictionary[ckey]-1;
      }

      Dictionary.Remove(key);

    }
    public bool Contains(int key)
    {
      return Dictionary.Contains(key);
    }

    /// <summary>
    /// Looks up the Key by the value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReverseLookup(int value)
    {
      foreach(int key in Dictionary.Keys)
        if((int)Dictionary[key]==value)
          return key;
      return -1;
    }
    #endregion
    #region Overrides
    protected override void OnInsert(object key, object value)
    {
      #region Validation
      if ( key.GetType() != Type.GetType("System.Int32") )
        throw new ArgumentException( "key must be of type Int32.", "key" );
      #endregion
      if ( Dictionary.Contains(key) )
        throw new DuplicateIndexException(((int) key).ToString() + " already exists in the index.");

      base.OnInsert(key,value);
    }
    protected override void OnRemove(object key, object value)
    {
      if ( key.GetType() != Type.GetType("System.Int32") )
        throw new ArgumentException( "key must be of type Int32.", "key" );

      base.OnRemove (key, value);
    }
    protected override void OnSet(object key, object oldValue, object newValue)
    {
      if ( key.GetType() != Type.GetType("System.Int32") )
        throw new ArgumentException( "key must be of type Int32.", "key" );
      
      base.OnSet (key, oldValue, newValue);
    }
    protected override void OnValidate(object key, object value)
    {
      if ( key.GetType() != Type.GetType("System.Int32") )
        throw new ArgumentException( "key must be of type Int32.", "key" );
      base.OnValidate (key, value);
    }
    #endregion
	}
}
