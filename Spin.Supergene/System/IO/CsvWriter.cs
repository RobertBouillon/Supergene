using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public class CsvWriter //: TextWriter
  {
    private readonly TextWriter _baseWriter;
    private string _fieldDelimiter;
    private string _rowDelimiter = Environment.NewLine;
    private bool _writeDelim = false;

    public CsvWriter(TextWriter baseWriter) : this(baseWriter, ",")
    {

    }

    public CsvWriter(TextWriter baseWriter, string delimiter)
    {
      #region Validation
      if (baseWriter == null)
        throw new ArgumentNullException("baseWriter");
      #endregion
      _fieldDelimiter = delimiter;
      _baseWriter = baseWriter;
    }

    public void Flush()
    {
      _baseWriter.Flush();
    }

    public void WriteEndOfRecord()
    {
      _baseWriter.Write(_rowDelimiter);
      _writeDelim = false;
    }

    public void Write(bool value)
    {
      Write(value ? 1 : 0);
    }

    public void Write(double? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(float? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(byte? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;

      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(short? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;

      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(int? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(bool? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value ? 1 : 0);
    }

    public void Write(DateTime? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(TimeSpan? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }

    public void Write(decimal? value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      if (value.HasValue)
        _baseWriter.Write(value.Value);
    }



    public void Write(double value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(float value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(long value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(byte value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(short value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(int value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(decimal value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }

    public void Write(string value)
    {
      if (_writeDelim)
        _baseWriter.Write(_fieldDelimiter);
      else
        _writeDelim = true;
      _baseWriter.Write(value);
    }
  }
}
