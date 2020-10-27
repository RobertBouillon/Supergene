using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class BinarySerializer : IDisposable
  {
    private Stream _baseStream;
    private BinaryWriter _writer;

    public Dictionary<string, int> Versions { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> Marks { get; set; } = new Dictionary<string, int>();
    private int _markID = Int32.MaxValue - 1;
    private const short MAGIC_NUMBER = 0x28E8;
    public Stream Stream => _baseStream;


    public BinarySerializer(Stream stream)
    {
      #region Validation
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      #endregion
      _baseStream = stream;
      _writer = new BinaryWriter(stream);
    }

    public void Write(bool value) => _writer.Write(value);
    public void Write(short value) => _writer.Write(value);
    public void Write(int value) => _writer.Write(value);
    public void Write(long value) => _writer.Write(value);
    public void Write(float value) => _writer.Write(value);
    public void Write(double value) => _writer.Write(value);
    public void Write(decimal value) => _writer.Write(value);
    public void Write(DateTime value) => _writer.Write(value.Ticks);
    public void Write(TimeSpan value) => _writer.Write(value.Ticks);
    public void Write(byte value) => _writer.Write(value);
    public void Write(sbyte value) => _writer.Write(value);
    public void Write(ushort value) => _writer.Write(value);
    public void Write(uint value) => _writer.Write(value);
    public void Write(ulong value) => _writer.Write(value);
    public void Write(string value) => _writer.Write(value);

    public void Write(IEnumerable<bool> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<short> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<int> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<long> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<float> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<double> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<decimal> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<DateTime> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<TimeSpan> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<byte> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<sbyte> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<ushort> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<uint> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<ulong> values) => Write(values, x => y => y.Write(x));
    public void Write(IEnumerable<string> values) => Write(values, x => y => y.Write(x));


    public void Mark(object source, int version = 0) => Mark(source.GetType().Name, version);
    public void Mark<T>(int version = 0) => Mark(typeof(T).FullName, version);
    public void Mark(string mark, int version = 0)
    {
      int id;
      if (!Marks.TryGetValue(mark, out id))
      {
        Marks.Add(mark, id = --_markID);
        Versions.Add(mark, id);
        _writer.Write(MAGIC_NUMBER);
        _writer.Write(id);
        _writer.Write(mark);
        _writer.Write(version);
      }
      else
      {
        _writer.Write(MAGIC_NUMBER);
        _writer.Write(id);
      }
    }


    public void Write<T>(IEnumerable<T> source, Func<T, Action<BinarySerializer>> serializer)
    {
      var count = source.Count();
      Write(count);
      foreach (var item in source)
        serializer(item)(this);
    }

    public void Flush()
    {
      _writer.Flush();
    }

    public void Dispose()
    {
      _writer?.Flush();
      _writer?.Dispose();
    }
  }
}
