using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics
{
  public class ConsoleStreamReader
  {
    private StreamReader _stream;
    private TextWriter _writer;
    private StringBuilder _builder;
    private char[] _buffer = new char[4096];
    private Task<int> _task;

    public ConsoleStreamReader(StreamReader source, TextWriter writer = null, StringBuilder builder = null)
    {
      _stream = source;
      _writer = writer;
      _builder = builder;

      Read();
    }

    private void Read() => _task = _stream.ReadAsync(_buffer, 0, _buffer.Length);
    public void Check()
    {
      if (!_task.IsCompleted)
        return;

      var read = _task.Result;
      _writer?.Write(_buffer, 0, read);
      _builder.Append(_buffer, 0, read);
      Read();
    }
  }
}
