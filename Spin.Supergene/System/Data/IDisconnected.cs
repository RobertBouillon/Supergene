using System;

namespace System.Data
{
	/// <summary>
	/// Represents a class where the actual data is located in a foreign or remote data store.
	/// </summary>
	public interface IDisconnected
	{
    /// <summary>
    /// Synchronizes the data class
    /// </summary>
    void Synchronize();

    void Publish();

    void Subscribe();

    /// <summary>
    /// True if the class is synchronized with the remote data.
    /// </summary>
    bool IsSynchronized{get;set;}
	}
}
