using System;
using System.IO;

using java.util;
using java.util.zip;

namespace System.IO.Compression
{
	/// <summary>
	/// Summary description for ZipFileEntry.
	/// </summary>
	public class ZipFileEntry
	{
    #region Private Property Declarations
    private string p_FullName;
    private string p_Name;
    private bool p_IsDirectory;
    private bool p_IsStored;
    private long p_CompressedSize = long.MinValue;
    private long p_Size = long.MinValue;
    private string p_Comment;

    internal ZipEntry p_Entry;
    private bool p_HasMethod = false;
    private bool p_HasDirectory = false;
    private bool p_HasName = false;
    #endregion
    #region Public Property Declarations
    public string FullName
    {
      get
      {
        if(!p_HasName)
        {
          p_FullName = p_Entry.getName();
          p_Name = Path.GetFileName(p_FullName);
          p_HasName = true;
        }
        return p_FullName;
      }
    }

    public string Name
    {
      get
      {
        if(!p_HasName)
        {
          p_FullName = p_Entry.getName();
          p_Name = Path.GetFileName(p_FullName);
          p_HasName = true;
        }
        return p_Name;
      }
    }

    public bool IsDirectory
    {
      get
      {
        if(!p_HasDirectory)
        {
          p_IsDirectory = p_Entry.isDirectory();
          p_HasDirectory = true;
        }
        return p_IsDirectory;
      }
    }

    public bool IsStored
    {
      get
      {
        if(!p_HasMethod)
        {
          p_IsStored = (p_Entry.getMethod() == ZipEntry.STORED);
          p_HasMethod = true;
        }
        return p_IsStored;
      }
    }

    public long CompressedSize
    {
      get
      {
        if(p_CompressedSize == long.MinValue)
          p_CompressedSize = p_Entry.getCompressedSize();
        return p_CompressedSize;
      }
    }

    public long Size
    {
      get
      {
        if(p_Size == long.MinValue)
          p_Size = p_Entry.getSize();
        return p_Size;
      }
    }

    public string Comment
    {
      get
      {
        if(p_Comment == null)
          p_Comment = p_Entry.getComment();
        return p_Comment;
      }
    }
    #endregion

    #region Ctors
		internal ZipFileEntry(ZipEntry entry)
		{
      p_Entry = entry;
		}
    #endregion

    #region Public Methods
    public void ExtractTo(DirectoryInfo destination)
    {

    }

    public void ExtractTo(FileInfo file)
    {
      ExtractTo(file,false);
    }

    public void ExtractTo(FileInfo file, bool overwrite)
    {

    }

    public void ExtractTo(Stream stream)
    {

    }
    #endregion
	}
}
