namespace System.IO;

public class FauxStream : Stream
{
  public override bool CanRead => true;
  public override bool CanSeek => true;
  public override bool CanWrite => true;
  public override long Length => Position;
  public override long Position { get; set; }

  public override void Flush() { }
  public override int Read(byte[] buffer, int offset, int count)
  {
    Position += count;
    return count;
  }

  public override long Seek(long offset, SeekOrigin origin) =>
    origin == SeekOrigin.Begin ? Position = offset :
    origin == SeekOrigin.Current ? Position += offset :
    throw new NotSupportedException();

  public override void SetLength(long value) { }

  public override void Write(byte[] buffer, int offset, int count) => Position += count;
}
