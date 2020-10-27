using System;
using System.Threading;

using Microsoft.Win32;

namespace System.IO
{
	/// <summary>
	/// Encapsulates a RAPI file stream
	/// </summary>
	public class CeFileStream : Stream, IDisposable
	{
    #region Private Property Declarations
    private string p_RemotePath;
    private FileMode p_Mode;
    private long p_Position;
    #endregion
    #region Private Variables
    private IntPtr hSrc;                            //Pointer to the source file on the remote device
    private int connectresult;                      //Result of the connection
    private bool CloseSessionOnDestroy = false;     //If the 
    private int length;                             //File Length
    private bool m_HasChanged = false;              //True if the file has been written to.
    #endregion
    #region ctors

    //public CeFileStream(string RemotePath, System.IO.FileMode Mode, int hRes)
    //{
    //  InitializeStream(RemotePath, Mode, hRes);
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="RemotePath"></param>
    /// <param name="Mode">Mode to access the </param>
    /// <param name="hRes">The handle to the current session</param>
    public CeFileStream(string RemotePath, FileMode Mode, FileAccess access)
    {
      RAPI.RAPIINIT ri = new RAPI.RAPIINIT();
      ri.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(ri);
      int hRes = RAPI.CeRapiInitEx(ref ri);

      ManualResetEvent me = new ManualResetEvent(false);
      me.Handle = ri.heRapiInit;

      if (!me.WaitOne(new TimeSpan(0,0,10), true))
        throw new IOException("Connection Timed out");

      //TODO: Replace this connection timeout exception. Maybe even make a special connection object for Remote Devices?
      CloseSessionOnDestroy = true;

      InitializeStream(RemotePath, Mode, access, hRes);
    }
    #endregion

    #region Private Methods
    private void CheckError()
    {
      int err = RAPI.CeGetLastError();
      if(err!=0)
        throw new RapiException(err);
    }

    private void InitializeStream(string remotePath, System.IO.FileMode mode, System.IO.FileAccess desiredAccess, int hRes)
    {
      #region Validation
      #endregion
      
      
      p_RemotePath = remotePath;
      p_Mode = mode;
      connectresult = hRes;

      //Establish a connection with the Remote Device
      
      //Create a handle to the remote file we're trying to access
      hSrc = RAPI.CeCreateFile(
        p_RemotePath,
        Microsoft.Win32.Convert.ToDesiredAccess(desiredAccess),
        ShareMode.FILE_SHARE_READ,
        0,
        Microsoft.Win32.Convert.ToCreationDisposition(mode),
        (int) FileAttribute.FILE_ATTRIBUTE_NORMAL,
        0);

      try
      {
        CheckError();
      }
      catch(RapiException ex)
      {
        if(ex.ErrorNumber!=183||mode!=FileMode.Open)
          throw ex;
      }

      length = RAPI.CeGetFileSize(hSrc,0);
      try
      {
        CheckError();
      }
      catch(RapiException ex)
      {
        if(ex.ErrorNumber!=183||mode!=FileMode.Open)
          throw ex;
      }
    }
    #endregion
    #region Public Property Declarations
    public override bool CanRead
    {
      get{return true;}
    }

    public override bool CanWrite
    {
      get{return true;}
    }

    public override bool CanSeek
    {
      get{return true;}
    }
    public override long Length
    {
      get{return length;}
    }

    public override long Position
    {
      get{return p_Position;}
      set{p_Position = value;}
    }
    #endregion
    #region Public Methods
    //public IAsyncResult BeginRead(byte[] buffer,int offset,int count,AsyncCallback callback,object state)
    //{
    //}

    public override void Write(byte[] buffer, int offset, int count)
    {
      m_HasChanged = true;
      int written = 0;
      //byte[] buf = new byte[count];
      //buffer.CopyTo(buf,offset);

      if(RAPI.CeWriteFile(hSrc,buffer,count,out written,0)==0)
        throw new RapiException("Error writing to file.");
    }

    public override void Flush()
    {
      //throw new NotSupportedException("Flush Not supported in synchronous operations.");
      //TODO: Is this ok?
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      #region Validation
      if(offset!=0)
        throw new ArgumentException("Non-Zero offset not supported");
      #endregion
      int numread;
      RAPI.CeReadFile(hSrc,buffer, count,out numread,0);
      return numread;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      int hooffset = 0;
      int looffset = 0;
      if(offset>Int32.MaxValue)
      {
        hooffset = (int)offset - Int32.MaxValue;
        looffset = Int32.MaxValue;
      }
      else
        looffset=0;


      RAPI.CeSetFilePointer(hSrc,looffset,hooffset,(int) origin);
      //TODO: Check for errors throughout class
      
      switch(origin)
      {
        case SeekOrigin.Begin:
          p_Position = offset;
          break;
        case SeekOrigin.Current:
          p_Position+=offset;
          break;
        case SeekOrigin.End:
          p_Position = Length - offset;
          break;
      }
      return p_Position;
    }

    public override void SetLength(long value)
    {
      //throw new NotSupportedException("Writing Not Supported");
    }

    public override void Close()
    {
      if((connectresult==0)&&CloseSessionOnDestroy)
        RAPI.CeRapiUninit();
      if(hSrc!=IntPtr.Zero)
        RAPI.CeCloseHandle(hSrc);
      if(m_HasChanged)
        RAPI.CeSetEndOfFile(hSrc);
      base.Close ();
    }
    #endregion
    #region IDisposable Members

    public void Dispose()
    {
      RAPI.CeRapiUninit();
    }

    #endregion
  }
}
