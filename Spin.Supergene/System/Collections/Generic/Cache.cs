using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic;

public class Cache<TKey, TValue> : Dictionary<TKey, CacheItem<TValue>>
{
  #region Fields
  #endregion
  #region Properties
  #endregion
  #region Constructors
  public Cache()
  {
    
  }
  #endregion
  #region Methods
  public CacheItem<TValue> Add(TKey key, TValue value, TimeSpan expiration)
  {
    CacheItem<TValue> item = new CacheItem<TValue>(value, expiration);
    if (ContainsKey(key))
      Remove(key);
    Add(key, item);
    return item;
  }

  public CacheItem<TValue> Add(TKey key, TValue value, DateTime expiration)
  {
    CacheItem<TValue> item = new CacheItem<TValue>(value, expiration);
    if (ContainsKey(key))
      Remove(key);
    Add(key, item);
    return item;
  }
  #endregion



}
