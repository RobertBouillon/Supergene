using System;

namespace System.Collections.Specialized
{
  public struct Int32IndexEntry
  {
    public int NextFragment;
    public int NextIndex;
    public int Key;
    public object Value;
  }
	/// <summary>
	/// Summary description for Int32IndexA.
	/// </summary>
	public class Int32IndexA
	{
    #region Private Property Declarations
    private float p_Padding = 0.2f;
    private int p_MaximumFragmentation = 5;
    private int p_MaximumSize = -1;
    private int p_Count;
    private Int32IndexEntry[] p_Data;
    #endregion

    #region Public Property Declarations

    #endregion
    #region Ctors
		public Int32IndexA() : this(256)
		{
		}

    public Int32IndexA(int initialSize)
    {
      p_Data = new Int32IndexEntry[initialSize];
    }

    public Int32IndexA(int intialSize, int maximumSize) : this(intialSize)
    {
      p_MaximumSize = maximumSize;
    }
    #endregion
    #region Public Methods
    public void Add(int key, object value)
    {
      if(p_Count==p_Data.Length)
        Defragment();

      Int32IndexEntry entry;
      bool bulkadd = false;
      if(p_Count==0)
        bulkadd=true;
      else if(key>p_Data[p_Count-1].Key)
        bulkadd=true;

      if(bulkadd)
      {
        entry.Key = key;
        entry.Value = value;
        entry.NextFragment = 0;
        entry.NextIndex = 0;

        p_Data[p_Count++] = entry;
        return;
      }

      int nearest = NearestIndex(key);
      if(p_Data[nearest].Key==key)
        throw new Exception(String.Format("Key {0} already exists in the index",key));

      entry.Key = key;
      entry.Value = value;
      entry.NextFragment = 0;
      entry.NextIndex = 0;

      p_Data[p_Count++] = entry;
      p_Data[nearest].NextFragment = p_Count;
    }

    /// <summary>
    /// Defragments the index for optimal search efficiency and repads it.
    /// </summary>
    public void Defragment()
    {
      Int32IndexEntry[] p_NewData = new Int32IndexEntry[(int)(p_Data.Length * (1+p_Padding))];
      
      //TODO: FInd a way to add to the end of the index with no checks (index.append?)
      int newindex = 0;
      Int32IndexEntry entry;
      for(int i = 0;i<p_Count;i++)
      {
        entry = p_Data[i];
        p_NewData[newindex++] = entry;

        while(entry.NextFragment!=0)
        {
          entry = p_Data[entry.NextFragment];
          p_NewData[newindex++] = entry;
        }

        //----> Clear the links to the new fragments
        entry = p_Data[i];
        while(entry.NextFragment!=0)
        {
          entry.NextFragment = 0;
          entry = p_Data[entry.NextFragment];
        }
      }
      p_Data = p_NewData;
    }
    #endregion
    #region PrivateMethods
    /// <summary>
    /// Finds the array index of the IndexEntry that closest matches the provided key
    /// </summary>
    /// <param name="key">The key in which to search</param>
    /// <returns>The p_Data index </returns>
    private int NearestIndex(int key)
    {
      int bottom = 0;
      int top = p_Count;
      int scan = 0;
      while(top-bottom>1)
      {
        scan = ((top-bottom)/2)+bottom;

        if(p_Data[scan].Key==key)
          return scan;
        else if(p_Data[scan].Key<key)
          bottom=scan;
        else
          top=scan;
      }

      //Search the fragments
      Int32IndexEntry entry = p_Data[scan];
      int fragments = 0;
      while(entry.NextFragment!=0)
      {
        fragments++;
        if(p_Data[entry.NextFragment].Key==key)
          return entry.NextFragment;
        else
          entry = p_Data[entry.NextFragment];
      }

      if(fragments>p_MaximumFragmentation)
        Defragment();

      return scan;
    }
    #endregion
    #region Indexers
    public object this[int key]
    {
      get
      {
        int nearest = NearestIndex(key);
        if(p_Data[nearest].Key==key)
          return p_Data[nearest].Value;
        else
          throw new Exception(String.Format("Key {0} was not found in the index",key));
      }
    }
    #endregion
	}
}
