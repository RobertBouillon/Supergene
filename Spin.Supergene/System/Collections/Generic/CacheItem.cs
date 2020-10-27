using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
  public class CacheItem<T>
  {
    #region Fields
    private DateTime? _expiration;
    private T _data;
	  #endregion

    #region Properties

    public T Data
    {
      get { return _data; }
      set { _data = value; }
    }

    public DateTime? AbsoluteExpiration
    {
      get { return _expiration; }
      set { _expiration = value; }
    }

    public bool IsExpired
    {
      get
      {
        if (!_expiration.HasValue)
          return false;
        else
          return DateTime.Now > _expiration;
      }
    }
    #endregion
    #region Constructors
    public CacheItem(T data, DateTime expiration) 
      : this(data)
    {
      _expiration = expiration;
    }

    public CacheItem(T data, TimeSpan expiration)
      : this(data)
    {
      _expiration = DateTime.Now + expiration;
    }

    /// <summary>
    /// Constructs a Cached Entity collection that never automatically expires.
    /// </summary>
    /// <param name="data"></param>
    internal CacheItem(T data)
    {
      #region Validation
      if (data == null)
        throw new ArgumentNullException("data");
      #endregion
      _data = data;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Extends the absolute expiration time by the time span
    /// </summary>
    /// <param name="span"></param>
    public void Extend(TimeSpan span)
    {
      _expiration = _expiration.Value + span;
    }
    #endregion

  }
}
