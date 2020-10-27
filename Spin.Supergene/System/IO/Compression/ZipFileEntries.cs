using System;
using System.Collections;

using java.util;
using java.util.zip;

namespace System.IO.Compression
{
	/// <summary>
	/// Summary description for ZipFileEntries.
	/// </summary>
  public class ZipFileEntries : CollectionBase
  {
    #region Private Property Declarations
    private ZipFile p_Parent;
    private ZipOutputStream p_OutputStream;
    #endregion
    #region Public Property Declarations
    public ZipFile Parent
    {
      get{return p_Parent;}
      set{p_Parent=value;}
    }

    #endregion
    #region Constructors
    public ZipFileEntries(ZipFile parent)
    {
      p_Parent = parent;
    }
    #endregion
    #region Public Methods
    public int Add(ZipFileEntry z)
    {
      return List.Add(z);
    }
    public void Remove(ZipFileEntry z)
    {
      throw new NotSupportedException();
      //List.Remove(z);
    }
    #endregion
    #region Indexers
    public ZipFileEntry this[int index]
    {
      get{return (ZipFileEntry) List[index];}
      set{List[index] = value;}
    }
    #endregion
    #region Overrides
    protected override void OnInsert(int index, object value)
    {
      if(!(value is ZipFileEntry))
        throw new Exception("Cannot add type of '" + value.GetType().Name + "' into a collection of type '" + GetType().Name + "'");
      base.OnInsert (index, value);
    }
    #endregion

    #region Public Methods
    #region Compression
    public void Add(FileInfo file)
    {
      Add(file,String.Empty);
    }

    public void Add(FileInfo file, string virtualPath)
    {
      Add(file,virtualPath,true);
    }

    public void Add(FileInfo file, string virtualPath, bool compress)
    {
      AddRange(new FileInfo[]{file},virtualPath,compress);
    }

    public void AddRange(FileInfo[] files)
    {
      AddRange(files, String.Empty);
    }

    public void AddRange(FileInfo[] files, string virtualPath)
    {
      AddRange(files,virtualPath,true);
    }

    public void AddRange(FileInfo[] files, string virtualPath, bool compress)
    {
      #region Validation
      if(virtualPath==null)
        throw new ArgumentNullException("virtualPath");
      if(files==null)
        throw new ArgumentNullException("files");
      #endregion
      if(files.Length==0)
        return;

      bool opened = false;
      //----> Create our Output Stream
      if(p_OutputStream==null)
      {
        Open();
        opened = true;
      }

      p_OutputStream.setLevel(p_Parent.Level);
      //----> Compress our files
      foreach(FileInfo fi in files)
      {
        try
        {
          //----> Create our Zip Entry
          ZipEntry zipentry=new ZipEntry(Path.Combine(virtualPath,fi.Name));
          if(p_Parent.AutoStore.Contains(Path.GetExtension(fi.Name))||(!compress))
            zipentry.setMethod(ZipEntry.STORED);
          else
            zipentry.setMethod(ZipEntry.DEFLATED);
          p_OutputStream.putNextEntry(zipentry);


          //-----> Add the entry data from file
          java.io.FileInputStream sourcestream=new java.io.FileInputStream(fi.FullName);
          int got = 0;
          sbyte[] buffer = new sbyte[8192];
          while ((got = sourcestream.read(buffer, 0, buffer.Length)) > 0)
            p_OutputStream.write(buffer, 0, got);

          sourcestream.close();
          p_OutputStream.closeEntry();
        }
        catch(Exception ex)
        {
          //if(!OnCompressionError(new CompressionErrorEventArgs(ex)))
            //break;
          throw ex;
        }
      }

      //HACK: Calling finish throws an incorrect exception
      //zipfile.finish();
      p_OutputStream.flush();

      if(opened)
        Close();
    }
    #endregion
    #region Decompression
    public void ExtractTo(DirectoryInfo destination)
    {
      //----> Unzip the files to the temp directory
      java.util.zip.ZipFile zipfile = new java.util.zip.ZipFile(p_Parent.File.FullName);
      //java.util.Enumeration entries = zipfile.entries();

      try
      {
        foreach(ZipFileEntry zentry in List)
        {
          //ZipEntry entry = (ZipEntry) entries.nextElement();

          java.io.InputStream stream = null;
          java.io.FileOutputStream fs = null;

          try
          {
            ZipEntry entry = zentry.p_Entry;
            stream = zipfile.getInputStream(entry);
            //fs = File.Create(Path.Combine(tempdirectory.FullName,entry.getName()));
            //fs.SetLength(entry.getSize());
            string entryname = entry.getName();
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Path.Combine(destination.FullName,entryname)));

            java.io.File f = new java.io.File(di.FullName,entryname);
            fs = new java.io.FileOutputStream(f);
          
            int written = 0;
            sbyte[] buffer = new sbyte[4096];

            while ((written = stream.read(buffer, 0, buffer.Length)) > 0)
              fs.write(buffer, 0, written);
          }
          finally
          {
            if(fs!=null)
            {
              fs.flush();
              fs.close();
            }
            if(stream!=null)
              stream.close();
          }
        }
      }
      finally
      {
        zipfile.close();
      }    
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>This method is NOT multi-thread safe. It's possible that between the deletion of the 
    /// tmp file and the creation of the directory, that another file would be created and an error 
    /// will occur. Override to make multi-thread safe.
    /// </remarks>
    public virtual DirectoryInfo ExtractToTempDirectory()
    {
      FileInfo tmp = new FileInfo(Path.GetTempFileName());
      DirectoryInfo di = new DirectoryInfo(tmp.FullName);
      tmp.Delete();
      di.Create();

      ExtractTo(di);
      return di;
    }
    #endregion
    #region State
    /// <summary>
    /// When compressing multiple files, use this to start compressing 
    /// </summary>
    public void Open()
    {
      try
      {
        if(p_OutputStream==null)
        {
          //We need to create an output stream where we'll put the files
          if(!p_Parent.File.Exists)
            p_Parent.File.Create().Close();

          p_OutputStream = new ZipOutputStream(new java.io.FileOutputStream(p_Parent.File.FullName));
        }
      }
      catch(Exception ex)
      {
        throw new IOException("Unable to open zip file for writing.",ex);
      }
    }

    /// <summary>
    /// When compressing multiple files, use this to start compressing 
    /// </summary>
    public void Close()
    {
      p_OutputStream.close();
      p_OutputStream = null;
    }
    #endregion
    #endregion
  }
}
