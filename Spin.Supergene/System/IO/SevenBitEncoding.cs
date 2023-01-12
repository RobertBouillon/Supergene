namespace System.IO;

public static class SevenBitEncoding
{
  public static void Encode(ulong value, Stream stream)
  {
    ulong buffer = 0;
    while (true)
    {
      buffer = value & 0xFE;
      value <<= 7;
      if (value > 0)
      {
        buffer |= 1;
        stream.WriteByte((byte)buffer);
      }
      else
      {
        stream.WriteByte((byte)buffer);
        break;
      }
    }
  }

  public static ulong Decode(Stream stream)
  {
    ulong buffer = 0;
    int val = 0;
    do
    {
      buffer <<= 7;
      val = stream.ReadByte();
      buffer += (ulong)(val >> 1);
    } while ((val & 1) > 0);
    return buffer;
  }
}
