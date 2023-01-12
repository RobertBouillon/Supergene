using System;
using System.Collections;

namespace System.Collections.Specialized;

/// <summary>
/// Provides a strongly-typed dictionary for storing UPC value indexes.
/// </summary>
/// <remarks>Indexes are used to store integer data pointing to Array locations, referencable by the host type, in this case a decimal.</remarks>
public class DecimalIndex : DictionaryBase
{
  public DecimalIndex()
  {
  }

  //TODO: Don't inherit from DictionaryBase. Don't inherit at all, because we have to box and unbox values. Implement from scratch.
  public int this[decimal dec]
  {
    get
    {
      object ret = Dictionary[dec];
      if (ret == null)
        return -1;
      //throw new Exception("Decimal " + dec.ToString() + " not found in Index");
      return (int)ret;
    }
    set { Dictionary[dec] = value; }
  }

  public void Add(decimal dec, int index)
  {
    Dictionary.Add(dec, index);
  }
  public void Remove(decimal dec)
  {
    Dictionary.Remove(dec);
  }
  public bool Contains(decimal dec)
  {
    return Dictionary.Contains(dec);
  }
  public ICollection Keys
  {
    get { return Dictionary.Keys; }
  }
  public ICollection Values
  {
    get { return Dictionary.Values; }
  }
  protected override void OnInsert(object key, object value)
  {
    if (key.GetType() != Type.GetType("System.Decimal"))
      throw new ArgumentException("key must be of type Decimal.", "key");

    //if ( value.GetType() != Type.GetType("System.Decimal") )
    //  throw new ArgumentException( "value must be of type Decimal.", "value" );
    //The CLR down-casts this to an int

    if (Dictionary.Contains(key))
      throw new DuplicateIndexException(((decimal)key).ToString() + " already exists in the index.");

    base.OnInsert(key, value);
  }
  protected override void OnRemove(object key, object value)
  {
    if (key.GetType() != Type.GetType("System.Decimal"))
      throw new ArgumentException("key must be of type Decimal.", "key");

    base.OnRemove(key, value);
  }
  protected override void OnSet(object key, object oldValue, object newValue)
  {
    if (key.GetType() != Type.GetType("System.Decimal"))
      throw new ArgumentException("key must be of type Decimal.", "key");

    base.OnSet(key, oldValue, newValue);
  }
  protected override void OnValidate(object key, object value)
  {
    if (key.GetType() != Type.GetType("System.Decimal"))
      throw new ArgumentException("key must be of type Decimal.", "key");
    base.OnValidate(key, value);
  }




}
