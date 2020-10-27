using System;
using System.IO;
using System.Net;

namespace System.Net.Sockets
{
	/// <summary>
	/// Provides a stream interface to an open and active socket
	/// </summary>
	public class SocketStream : Stream
	{
    #region Private Property Declarations
    private Socket p_Source;
    #endregion
    #region Public Property Declarations
    public Socket Source
    {
      get{return p_Source;}
    }
    #endregion
    #region Ctors
		public SocketStream(Socket source)
		{
      p_Source = source;
      p_Source.Blocking = false;
		}
    #endregion
    #region Overrides
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return p_Source.BeginReceive(buffer,offset,count,SocketFlags.None,callback,state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return p_Source.BeginSend(buffer,offset,count,SocketFlags.None,callback,state);
    }

    public override bool CanRead
    {
      get{return true;}
    }

    public override bool CanSeek
    {
      get{return false;}
    }

    public override bool CanWrite
    {
      get{return true;}
    }

    public override void Close()
    {
      p_Source.Close();
    }

    public override void Flush()
    {
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      return p_Source.EndReceive(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      p_Source.EndSend(asyncResult);
    }


    public override int Read(byte[] buffer, int offset, int count)
    {
      int ret = 0;
      try
      {
        ret = p_Source.Receive(buffer,offset,count,SocketFlags.None);
        if(ret==0)
          throw new Exception("Remote connection Closed");
      }
      catch(SocketException ex)
      {
        if(ex.Message!="A non-blocking socket operation could not be completed immediately")
          throw ex;
        ret = 0;
      }
      return ret;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      p_Source.Send(buffer,offset,count,SocketFlags.None);
    }

    public override int ReadByte()
    {
      byte[] buffer = new byte[1];
      int read = Read(buffer,0,1);
      if(read==0)
        return -1;
      else
        return buffer[0];
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void WriteByte(byte value)
    {
      byte[] buffer = new byte[]{value};
      p_Source.Send(buffer,0,1,SocketFlags.None);
    }

    public override long Length
    {
      get{throw new NotSupportedException();}
    }

    public override long Position
    {
      get{throw new NotSupportedException();}
      set{throw new NotSupportedException();}
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }
    #endregion
	}
}
