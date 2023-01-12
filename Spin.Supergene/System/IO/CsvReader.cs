using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO;

public class CsvReader
{
  #region Fields
  private char _delimiter;
  private char _textQualifier = '"';
  private bool _hasTextQualifier;
  #endregion
  #region Properties
  public bool HasTextQualifier
  {
    get { return _hasTextQualifier; }
    set { _hasTextQualifier = value; }
  }

  public char TextQualifier
  {
    get { return _textQualifier; }
    set { _textQualifier = value; }
  }

  public char Delimiter
  {
    get { return _delimiter; }
    set { _delimiter = value; }
  }
  #endregion

  #region Constructors
  public CsvReader(char delimiter)
  {
    _delimiter = delimiter;
    _hasTextQualifier = false;
  }
  #endregion

  #region Methods
  public string[] ParseLine(string line)
  {
    List<string> ret = new List<string>(64);
    ParseLine(line, ret);
    return ret.ToArray();
  }

  public void ParseLine(string line, List<string> output)
  {
    List<string> ret = output;
    int start = 0;
    bool delim = false;
    Action<int, int, int> add = (x, y, z) =>
    {
      ret.Add(line.Substring(start, z));
      start = x + y;
      if (delim)
        delim = false;
    };

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];

      if (i == start && !delim && (c == _textQualifier && _hasTextQualifier))
      {
        delim = true;
        ++start;
        continue;
      }

      if (delim)
      {

        if ((c == _textQualifier && _hasTextQualifier) && ((i + 1) == line.Length || line[i + 1] == _delimiter))
          add(i++, 2, i - start - 1);
      }
      else
      {
        if (c == _delimiter)
          add(i, 1, i - start);
      }
    }

    //EOL
    if (start <= line.Length)
      if (line.Length == start)
        ret.Add(String.Empty);
      else
        add(start, 1, line.Length - start);
  }

  public IEnumerable<List<string>> ParseCSV2(Stream source)
  {
    List<string> cols = new List<string>(64);
    using (var reader = new StreamReader(source))
    {
      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        if (String.IsNullOrWhiteSpace(line))
          continue;
        else
        {
          ParseLine(line, cols);
          yield return cols;
          cols.Clear();
        }
    }
  }

  public IEnumerable<T> ParseCSV<T>(Stream source, Func<List<string>, T> factory)
  {
    List<string> cols = new List<string>(64);
    using (var reader = new StreamReader(source))
    {
      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        if (String.IsNullOrWhiteSpace(line))
          continue;
        else
        {
          ParseLine(line, cols);
          yield return factory(cols);
          cols.Clear();
        }
    }
  }

  public IEnumerable<string[]> ParseCSV(Stream source)
  {
    using (var reader = new StreamReader(source))
    {
      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        if (String.IsNullOrWhiteSpace(line))
          continue;
        else
          yield return ParseLine(line);
    }
  }
  #endregion
}
