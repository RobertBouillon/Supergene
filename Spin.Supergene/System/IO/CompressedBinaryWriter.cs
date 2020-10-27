using System;
using System.Collections.Generic;
//using System.Diagnostics.UnitTesting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class CompressedBinaryWriter : BinaryWriter
  {
    #region Fields
    #endregion

    #region Constructors
    public CompressedBinaryWriter() { }
    public CompressedBinaryWriter(Stream output) : base(output) { }
    public CompressedBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }
    public CompressedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen) { }
    #endregion

    #region Methods
    public override void Write(ulong value)
    {
      if (value > 0 && value <= 0xFF >> 2)
      {
        base.Write((byte)value);
        return;
      }

      BaseStream.WriteByte((byte)((3 << 6) | (int)value & 0x3F));

      value >>= 6;
      bool terminate = false;
      do
      {
        var data = ((int)value & 0x7F);
        terminate = value <= 0x7F;
        value >>= 7;
        BaseStream.WriteByte((byte)((terminate ? 0x80 : 0x00) | data));
      } while (!terminate);
    }

    public override void Write(long value)
    {
      if (value > 0 && value <= 0xFF >> 2)
      {
        base.Write((byte)value);
        return;
      }

      bool invert = value < 0;
      if (invert)
      {
        value = -value;
        BaseStream.WriteByte((byte)((3 << 6) | (int)value & 0x3F));
      }
      else
        BaseStream.WriteByte((byte)((2 << 6) | (int)value & 0x3F));

      value >>= 6;
      bool terminate = false;
      do
      {
        var data = ((int)value & 0x7F);
        terminate = value <= 0x7F;
        value >>= 7;
        BaseStream.WriteByte((byte)((terminate ? 0x80 : 0x00) | data));
      } while (!terminate);
    }

    public override void Write(uint value)
    {
      Write((ulong)value);
    }

    //public override void Write(byte value)
    //{
    //  Write((long)value);
    //}

    public override void Write(int value)
    {
      Write((long)value);
    }

    public override void Write(sbyte value)
    {
      Write((long)value);
    }

    public override void Write(short value)
    {
      Write((long)value);
    }

    public override void Write(ushort value)
    {
      Write((ulong)value);
    }

    public void Write(TimeSpan time)
    {
      Write((ulong)time.TotalMilliseconds);
    }
    #endregion

    #region Unit Test
    //[UnitTest(TestMethodType.TestStatic)]
    //public static void TestStatic()
    //{
    //  MemoryStream stream = new MemoryStream();
    //  CompressedBinaryWriter writer = new CompressedBinaryWriter(stream);
    //  writer.Write(32);
    //  writer.Write(321U);
    //  writer.Write(321321U);
    //  writer.Write(321321321U);
    //  writer.Write(321321321321UL);
    //  writer.Write(321321321321321UL);
    //  writer.Write(3213213213213213213UL);
    //  writer.Write(7777777777777777777UL);
      

    //  writer.Write(-32);
    //  writer.Write(-321);
    //  writer.Write(-321321);
    //  writer.Write(-321321321);
    //  writer.Write(-321321321321);
    //  writer.Write(-321321321321321);
    //  writer.Write(-3213213213213213213);

    //  CompressedBinaryReader reader = new CompressedBinaryReader(stream);
    //  stream.Position = 0;
    //  reader = new CompressedBinaryReader(stream);
    //  long l;
    //  ulong ul;
      

    //  if ((ul = reader.ReadUInt16()) != 32) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt16()) != 321) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt32()) != 321321) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt64()) != 321321321) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt64()) != 321321321321) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt64()) != 321321321321321) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt64()) != 3213213213213213213) throw new UnitTestException();
    //  if ((ul = reader.ReadUInt64()) != 7777777777777777777UL) throw new UnitTestException();

    //  if ((l = reader.ReadSByte()) != -32) throw new UnitTestException();
    //  if ((l = reader.ReadInt16()) != -321) throw new UnitTestException();
    //  if ((l = reader.ReadInt32()) != -321321) throw new UnitTestException();
    //  if ((l = reader.ReadInt64()) != -321321321) throw new UnitTestException();
    //  if ((l = reader.ReadInt64()) != -321321321321) throw new UnitTestException();
    //  if ((l = reader.ReadInt64()) != -321321321321321) throw new UnitTestException();
    //  if ((l = reader.ReadInt64()) != -3213213213213213213) throw new UnitTestException();
    //}
    #endregion
  }
}
