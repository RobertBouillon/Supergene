using System;
using System.Collections;

namespace System.Diagnostics
{
	/// <summary>
	/// A Collection of Unit Test Result Items
	/// </summary>
	public class UnitTestResultItems : CollectionBase
	{
    #region ctors
    /// <summary>
    /// A Collection of Unit Test Result Items
    /// </summary>
    public UnitTestResultItems()
		{
		}
    #endregion
    #region Public Methods
    public int Add(UnitTestResultItem item)
    {
      return List.Add(item);
    }

    public int Add(string message, Exception innerException)
    {
      return Add(new UnitTestResultItem(message, innerException));
    }

    public int Add(string message)
    {
      return Add(message,null);
    }

    public void Remove(UnitTestResultItem item)
    {
      List.Remove(item);
    }
    #endregion
	}
}
