using System;

namespace System.IO;

/// <summary>
/// Summary description for BitStream.
/// </summary>
public class BitStream : Stream
{
  public static int BitsLost = 0;
  #region Private Property Declarations
  private byte p_CurrentWrite;
  private ushort p_CurrentRead;
  private int p_BitOffset;
  private Stream p_InnerStream;
  #endregion
  #region Protected Property Declarations
  protected int BitOffset
  {
    get { return p_BitOffset; }
  }

  protected Stream InnerStream
  {
    get { return p_InnerStream; }
  }
  #endregion

  #region Ctors
  public BitStream(Stream innerStream)
  {
    p_InnerStream = innerStream;
  }
  #endregion

  #region Stream Overrides
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

  public override long Length
  {
    get { return p_InnerStream.Length; }
  }

  public override long Position
  {
    get { return p_InnerStream.Position; }
    set { p_InnerStream.Position = value; }
  }

  public override void SetLength(long value)
  {
    p_InnerStream.SetLength(value);
  }


  #endregion
  #region Public Methods
  //Pushes left-aligned bits onto the stream
  public void WriteByte(byte byteToPush, int bitsToPush)
  {
    int oppositepush = (8 - bitsToPush);
    //Clear the bits we're not using
    byteToPush >>= oppositepush;
    byteToPush <<= oppositepush;

    int newPosition = p_BitOffset + bitsToPush;
    if (newPosition >= 8)
    {
      p_CurrentWrite |= (byte)(byteToPush >> p_BitOffset);
      p_InnerStream.WriteByte(p_CurrentWrite);
      p_CurrentWrite = (byte)(0x00 | (byte)(byteToPush << (8 - p_BitOffset)));
      p_BitOffset = newPosition % 8;
    }
    else
    {
      p_CurrentWrite |= (byte)(byteToPush >> p_BitOffset);
      p_BitOffset = newPosition;
    }
  }

  /// <summary>
  /// Flush should ALWAYS be called for bit streams, prior to closing.
  /// </summary>
  public override void Flush()
  {
    if (p_BitOffset != 0)
      WriteByte(p_CurrentWrite);
    BitsLost += (8 - p_BitOffset);
    p_InnerStream.Flush();
  }

  public override void Write(byte[] buffer, int offset, int count)
  {
    int stop = offset + count;
    for (int i = offset; i < stop; i++)
      WriteByte(buffer[i]);
  }

  public override void WriteByte(byte value)
  {
    WriteByte(value, 8);
  }

  public override int Read(byte[] buffer, int offset, int count)
  {
    for (int i = 0; i < count; i++)
      buffer[offset + i] = (byte)ReadByte();

    return count;
  }

  public override long Seek(long offset, SeekOrigin origin)
  {
    p_BitOffset = 0;
    return p_InnerStream.Seek(offset, origin);
  }


  public override int ReadByte()
  {
    return ReadByte(8);
  }

  public byte ReadByte(int bitsToRead)
  {
    if (p_BitOffset == 0 && bitsToRead == 8)
      return (byte)p_InnerStream.ReadByte();

    int oppositepush = (8 - bitsToRead);
    ushort returnmask = 0xFF;
    byte ret = 0x00;
    returnmask >>= oppositepush;
    returnmask <<= oppositepush + 8;

    //Our p_CurrentRead should always be left-aligned
    if (p_BitOffset < bitsToRead)
    {
      //We don't have enough bits in our buffer to satisfy the request
      //Read the next byte
      byte buffer = (byte)p_InnerStream.ReadByte();

      //Move the bits from the buffer to the current read buffer (p_CurrentRead)
      p_CurrentRead |= (ushort)((buffer << 8) >> p_BitOffset);

      //Set the return value
      ret = (byte)((returnmask & p_CurrentRead) >> 8);

      //Left-Align the current bits
      p_CurrentRead <<= bitsToRead;

      //Set the BitOffset
      p_BitOffset = (p_BitOffset + 8 - bitsToRead);
    }
    else
    {
      //Set the return value
      ret = (byte)((returnmask & p_CurrentRead) >> 8);

      //Left-Align the current bits
      p_CurrentRead <<= bitsToRead;

      //Set the BitOffset
      p_BitOffset -= bitsToRead;
    }

    return ret;
  }
  #endregion
}
