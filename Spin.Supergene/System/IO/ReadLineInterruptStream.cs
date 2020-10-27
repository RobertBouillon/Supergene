using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class ReadLineInterruptStream : Stream
  {
    #region Fields
    private Stream _innerStream;
    private bool _interrupt = false;
    private bool _isClosed = false;
    #endregion

    #region Constructors
    public ReadLineInterruptStream(Stream inner)
    {
      #region Validation
      if (inner == null)
        throw new ArgumentNullException("inner");
      #endregion
      _innerStream = inner;
    }
    #endregion

    #region Overrides

    public override bool CanRead
    {
      get { return _innerStream.CanRead; }
    }

    public override bool CanSeek
    {
      get { return _innerStream.CanSeek; }
    }

    public override bool CanWrite
    {
      get { return _innerStream.CanWrite; }
    }

    public override void Flush()
    {
      _innerStream.Flush();
    }

    public override long Length
    {
      get { return _innerStream.Length; }
    }

    public override long Position
    {
      get
      {
        return _innerStream.Position;
      }
      set
      {
        _innerStream.Position = value;
      }
    }

    public override void Close()
    {
      _isClosed = true;
      base.Close();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if ((_interrupt | _isClosed) && count > 0)
      {
        buffer[offset] = (byte)'\n';
        _interrupt = false;
        return 1;
      }
      return _innerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return _innerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Write(buffer, offset, count);
    }
    #endregion

    #region Methods
    public void Interrupt()
    {
      _interrupt = true;
    }
    #endregion
  }
}
