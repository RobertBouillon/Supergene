using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
  public class CacheItems<T> : CollectionBase<CacheItem<T>>
  {
    #region Constructors
    public CacheItems()
    {

    }
    #endregion

    #region Methods
    public void Add(int id, T data)
    {
      Add(new CacheItem<T>(id, data));
    }

    public void Add(int id, T data, TimeSpan slidingExpiration)
    {
      Add(new CacheItem<T>(id, data, slidingExpiration));
    }

    public void Add(int id, T data, DateTime absoluteExpiration)
    {
      Add(new CacheItem<T>(id, data, absoluteExpiration));
    }

    public T GetDataByID(int id)
    {
      foreach (CacheItem<T> item in this)
        if (item.Key == id)
          return item.Data;
      return default(T);
    }


    public bool Contains(int id)
    {
      foreach (CacheItem<T> item in this)
        if (item.Key == id)
          return true;
      return false;
    }
    #endregion

    
  }
}
