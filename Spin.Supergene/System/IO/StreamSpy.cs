using System;

namespace System.IO;

/// <summary>
/// Summary description for StreamSpy.
/// </summary>
public class StreamSpy : Stream
{
  #region Protected Proprety Declarations
  private Stream p_InnerStream;
  #endregion
  #region Public Property Declarations
  protected Stream InnerStream
  {
    get { return p_InnerStream; }
    set { p_InnerStream = value; }
  }
  #endregion


  #region Ctors
  public StreamSpy(Stream stream)
  {
    p_InnerStream = stream;
  }
  #endregion

  #region Overrides
  public override void Write(byte[] buffer, int offset, int count)
  {
    p_InnerStream.Write(buffer, offset, count);
    OnDataWritten(new DataTransferEventArgs(buffer));
  }

  public override void WriteByte(byte value)
  {
    p_InnerStream.WriteByte(value);
    OnDataWritten(new DataTransferEventArgs(new byte[] { value }));
  }

  public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
  {
    OnDataRead(new DataTransferEventArgs(buffer));
    return p_InnerStream.BeginRead(buffer, offset, count, callback, state);
  }

  public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
  {
    OnDataWritten(new DataTransferEventArgs(buffer));
    return p_InnerStream.BeginWrite(buffer, offset, count, callback, state);
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    int read = p_InnerStream.Read(buffer, offset, count);
    byte[] dr = new byte[read];
    if (read + offset > buffer.Length)
      Console.Out.WriteLine("Stop here");
    //HACK: Read returns 1273 'read' when count was 2? changed iterator from read to count
    for (int i = 0; i < read; i++)
      dr[i] = buffer[offset + i];

    OnDataRead(new DataTransferEventArgs(dr));
    return read;
  }

  public override int ReadByte()
  {
    int ret = p_InnerStream.ReadByte();
    OnDataRead(new DataTransferEventArgs(new byte[] { (byte)ret }));
    return ret;
  }

  public override bool CanRead
  {
    get { return p_InnerStream.CanRead; }
  }

  public override bool CanSeek
  {
    get { return p_InnerStream.CanSeek; }
  }

  public override bool CanWrite
  {
    get { return p_InnerStream.CanWrite; }
  }

  public override void Close()
  {
    p_InnerStream.Close();
  }

  public override int EndRead(IAsyncResult asyncResult)
  {
    return p_InnerStream.EndRead(asyncResult);
  }

  public override void EndWrite(IAsyncResult asyncResult)
  {
    p_InnerStream.EndWrite(asyncResult);
  }

  public override void Flush()
  {
    p_InnerStream.Flush();
  }

  public override long Length
  {
    get { return p_InnerStream.Length; }
  }

  public override long Position
  {
    get { return p_InnerStream.Position; }
    set { p_InnerStream.Position = value; }
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    return p_InnerStream.Seek(offset, origin);
  }

  public override void SetLength(long value)
  {
    p_InnerStream.SetLength(value);
  }
  #endregion

  #region Events and Delegates
  public delegate void DataWrittenEvent(object source, DataTransferEventArgs e);
  public delegate void DataReadEvent(object source, DataTransferEventArgs e);

  public event DataWrittenEvent DataWritten;
  public event DataReadEvent DataRead;
  #endregion
  #region Protected Event Declarations (OnXXXXX)
  public void OnDataWritten(DataTransferEventArgs e)
  {
    if (DataWritten != null)
      DataWritten(this, e);
  }

  public void OnDataRead(DataTransferEventArgs e)
  {
    if (DataRead != null)
      DataRead(this, e);
  }
  #endregion
  #region DataTransferEventArgs subclass
  public class DataTransferEventArgs : EventArgs
  {
    #region Private Property Declarations
    private byte[] p_Data;
    #endregion
    #region Public Property Declaration
    public byte[] Data
    {
      get { return p_Data; }
    }
    #endregion
    #region Ctors
    public DataTransferEventArgs(byte[] data)
    {
      p_Data = data;
    }
    #endregion
  }

  #endregion
}
