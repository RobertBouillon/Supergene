using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class BinaryDeserializer : IDisposable
  {
    private Stream _baseStream;
    private BinaryReader _reader;

    public Dictionary<int, int> Versions { get; set; } = new Dictionary<int, int>();
    private const short MAGIC_NUMBER = 0x28E8;
    public Stream Stream => _baseStream;
    private Dictionary<int, string> Marks { get; set; } = new Dictionary<int, string>();
    private int _lastMarkID = -1;

    public BinaryDeserializer(Stream stream)
    {
      #region Validation
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      #endregion
      _baseStream = stream;
      _reader = new BinaryReader(stream);
    }

    public byte ReadByte() => _reader.ReadByte();
    public sbyte ReadSByte() => _reader.ReadSByte();
    public bool ReadBoolean() => _reader.ReadBoolean();
    public ushort ReadUInt16() => _reader.ReadUInt16();
    public short ReadInt16() => _reader.ReadInt16();
    public int ReadInt32() => _reader.ReadInt32();
    public long ReadInt64() => _reader.ReadInt64();
    public ulong ReadUInt64() => _reader.ReadUInt64();
    public string ReadString() => _reader.ReadString();
    public float ReadSingle() => _reader.ReadSingle();
    public double ReadDouble() => _reader.ReadDouble();
    public decimal ReadDecimal() => _reader.ReadDecimal();
    public DateTime ReadDateTime() => new DateTime(ReadInt64());
    public TimeSpan ReadTimeSpan() => new TimeSpan(ReadInt64());

    public void Read(out bool target) => target = _reader.ReadBoolean();
    public void Read(out byte target) => target = _reader.ReadByte();
    public void Read(out short target) => target = _reader.ReadInt16();
    public void Read(out int target) => target = _reader.ReadInt32();
    public void Read(out long target) => target = _reader.ReadInt64();
    public void Read(out ushort target) => target = _reader.ReadUInt16();
    public void Read(out uint target) => target = _reader.ReadUInt32();
    public void Read(out ulong target) => target = _reader.ReadUInt64();
    public void Read(out float target) => target = _reader.ReadSingle();
    public void Read(out double target) => target = _reader.ReadDouble();
    public void Read(out decimal target) => target = _reader.ReadDecimal();
    public void Read(out string target) => target = _reader.ReadString();

    public int Mark(object source) => Mark(source.GetType().Name);
    public int Mark<T>() => Mark(typeof(T).FullName);
    public void ReadToNextMark()
    {
      byte[] mark = new byte[2];
      mark[1] = ReadByte();
      mark[0] = ReadByte();
      while (BitConverter.ToInt16(mark, 0) != MAGIC_NUMBER)
      {
        mark[0] = mark[1];
        mark[1] = ReadByte();
      }
      _reader.BaseStream.Seek(-2, SeekOrigin.Current);
    }

    public int Mark(String mark)
    {
      int actual_id, version;

      var mnum = ReadInt16();
      if (mnum != MAGIC_NUMBER)
        if (_lastMarkID < 0)
          throw new Exception($"File format not recognized"); //Very first mark was not read.
        else
          throw new Exception($"Serialization symmetry error. '{Marks[_lastMarkID]}' did not deserialize properly.");

      Read(out actual_id);
      if (!Versions.ContainsKey(actual_id))
      {
        var actual_mark = ReadString();
        if (mark != actual_mark)
          throw new Exception($"Expected mark '{mark}' but read {actual_mark}");

        _lastMarkID = actual_id;
        Marks.Add(actual_id, actual_mark);
        Versions.Add(actual_id, version = ReadInt32());
        return version;
      }
      else
      {
        _lastMarkID = actual_id;
        return Versions[actual_id];
      }
    }

    public IEnumerable<T> Read<T>(Func<BinaryDeserializer, T> factory)
    {
      int count = ReadInt32();
      for (int i = 0; i < count; i++)
        yield return factory(this);
    }

    public void Dispose()
    {
      _reader?.Dispose();
    }
  }
}
