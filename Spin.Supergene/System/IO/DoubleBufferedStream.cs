using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace System.IO;
public class DoubleBufferedStream : Stream
{
  private BufferedStream[] _streams;
  private bool _writeFirst;
  private bool _readFirst;
  private volatile bool _shuttingDown;
  private AutoResetEvent _flushed;
  private AutoResetEvent _flushing;
  private Thread _flushThread;
  private BufferedStream _writeBuffer;
  private long _length;
  private Stream _stream;

  public DoubleBufferedStream(Stream destination, FileAccess access, int bufferSize = 8 * 1024 * 1024)
  {
    #region Validation
    if (destination is null)
      throw new ArgumentNullException(nameof(destination));
    if (access == FileAccess.Read)
      throw new NotImplementedException();
    if (access == FileAccess.ReadWrite)
      throw new NotSupportedException();
    #endregion

    _stream = destination;
    _streams = new BufferedStream[]
    {
      new BufferedStream(_stream, bufferSize),
      new BufferedStream(_stream, bufferSize)
    };

    _flushThread = new Thread(x => FlushAsync())
    {
      IsBackground = true,
      Name = "Double Buffer Flush",
      Priority = ThreadPriority.Normal
    };

    _flushed = new AutoResetEvent(true);
    _flushing = new AutoResetEvent(false);
    _writeBuffer = _streams[_readFirst ? 0 : 1];

    _flushThread.Start();
  }

  private new void FlushAsync()
  {
    var sw = Stopwatch.StartNew();
    while (true)
    {
      _flushing.WaitOne();
      if (_shuttingDown)
        return;
      sw.Restart();
      _streams[_writeFirst ? 0 : 1].Flush();
      Console.WriteLine($"Flushed in {sw.Elapsed}");
      _writeFirst = !_writeFirst;
      _flushed.Set();
    }
  }

  public override bool CanRead => false;
  public override bool CanSeek => false;
  public override bool CanWrite => _writeBuffer.CanWrite;
  public override long Length => _length;
  public override long Position
  {
    get => _length;
    set => throw new NotSupportedException();
  }

  public override void Flush()
  {
    _flushing.Set();
    _flushed.WaitOne();
    Console.WriteLine($"Flushing {_writeBuffer.Position}");
    _readFirst = !_readFirst;
    _writeBuffer = _streams[_readFirst ? 0 : 1];
  }

  public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
  public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
  public override void SetLength(long value) => _stream.SetLength(value);
  public override void Write(byte[] buffer, int offset, int count)
  {
    _writeBuffer.Write(buffer, offset, count);
    _length += count;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && _flushThread is not null)
    {
      _shuttingDown = true;
      _flushing.Set();
      _flushThread.Join();
      _flushThread = null;
    }
    base.Dispose(disposing);
  }
}
