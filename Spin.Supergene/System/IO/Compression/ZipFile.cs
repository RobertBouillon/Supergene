using System;
using System.Collections.Specialized;
using System.IO;

using java.util;
using java.util.zip;


namespace System.IO.Compression
{
	/// <summary>
	/// Summary description for ZipFile.
	/// </summary>
  public class ZipFile
  {
    #region Private Property Declarations
    private ZipFileEntries p_Entries;
    private StringCollection p_AutoStore = new StringCollection();
    private int p_Level;
    private FileInfo p_File;

    #endregion
    #region Public Property Declarations
    public ZipFileEntries Entries
    {
      get{return p_Entries;}
      set{p_Entries=value;}
    }

      /// <summary>
      /// Adding a file extension to this collection forces the file to be stored, rather than compressed, regardless of request
      /// </summary>
      public StringCollection AutoStore
    {
      get{return p_AutoStore;}
      set{p_AutoStore=value;}
    }

    public int Level
    {
      get{return p_Level;}
      set{p_Level=value;}
    }

    public FileInfo File
    {
      get{return p_File;}
      set{p_File=value;}
    }
    #endregion

    #region Ctors
    public ZipFile(string path)
    {
      #region Validation
      if(path==null)
        throw new ArgumentNullException("path");
      if(path==String.Empty)
        throw new ArgumentException("Path cannot be an empty string");
      #endregion

      p_File = new FileInfo(path);
      p_AutoStore.Add("rar");
      p_AutoStore.Add("zip");
    } 

    public ZipFile(FileInfo file)
    {
      p_File = file;
    }
    #endregion

    #region Public Methods

    #endregion

    #region Events / Delegates
    public event CompressionErrorEvent CompressionError;

    public delegate void CompressionErrorEvent(object sender, CompressionErrorEventArgs e);
    #endregion
    #region Protected Methods (OnXXXXX)
    protected virtual bool OnCompressionError(CompressionErrorEventArgs e)
    {
      if(CompressionError!=null)
        CompressionError(this,e);

      return true;
    }
    #endregion

    #region CompressionErrorEventArgs Subclass
    public class CompressionErrorEventArgs : EventArgs
    {
      #region Private Property Declarations
      private Exception p_Exception;
      #endregion
      #region Public Property Declarations
      public Exception Exception
      {
        get{return p_Exception;}
        set{p_Exception=value;}
      }
      #endregion
      #region Ctors
      public CompressionErrorEventArgs(Exception exception)
      {
        p_Exception = exception;
      }
      #endregion
    }
    #endregion
  }
}
