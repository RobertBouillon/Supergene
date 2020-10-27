using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class CompressedBinaryReader : BinaryReader
  {
    #region Constructors
    public CompressedBinaryReader(Stream input) : base(input) { }
    public CompressedBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }
    public CompressedBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }
    #endregion

    #region Overrides
    public override long ReadInt64()
    {
      byte data = ReadByte();
      if ((data & 0x80) == 0)
        return (long)(data & ~0x80);

      bool invert = ((data & 0x40) == 0x40);
      long ret = (long)(data & ~0xC0);  //Trim first two

      int index = 1;
      do
      {
        data = ReadByte();
        ret |= (long)(data & ~0x80) << ((7 * index++) - 1);
      } while ((data & 0x80) == 0);

      return invert ? -ret : ret;
    }

    public override ulong ReadUInt64()
    {
      //Same as ReadInt64, except the sign processing has been removed (optimized for performance)
      byte data = ReadByte();
      if ((data & 0x80) == 0)
        return (ulong)(data & ~0x80);

      ulong ret = (ulong)(data & ~0xC0);  //Trim first two

      int index = 1;
      do
      {
        data = ReadByte();
        ret |= (ulong)(data & ~0x80) << ((7 * index++) - 1);
      } while ((data & 0x80) == 0);

      return ret;
    }

    #region Stubs
    public override sbyte ReadSByte()
    {
      return checked((sbyte)ReadInt64());
    }

    //public override byte ReadByte()
    //{
    //  return checked((byte)ReadUInt64());
    //}

    public override int ReadInt32()
    {
      return checked((int)ReadInt64());
    }

    public override short ReadInt16()
    {
      return checked((short)ReadInt64());
    }

    public override ushort ReadUInt16()
    {
      return checked((ushort)ReadUInt64());
    }

    public override uint ReadUInt32()
    {
      return checked((uint)ReadUInt64());
    }

    public TimeSpan ReadTimeSpan()
    {
      return TimeSpan.FromMilliseconds(ReadUInt64());
    }
    #endregion

    #endregion
  }
}
