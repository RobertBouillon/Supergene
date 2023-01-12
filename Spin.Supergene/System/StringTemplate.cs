using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System;

public class StringTemplate
{
  private string _template;
  private string[] _parts;
  private int[] _tokens;
  private string[] _parameters;
  private Dictionary<string, int> _parameterIndices;
  private string[] _replaceGraph;
  private Action<StreamWriter>[] _callbackGraph;
  private bool[] _graphTypeIsReplace;

  public string[] Parameters
  {
    get { return _parameters; }
  }

  public StringTemplate(string template)
  {
    _template = template;
    Prepare();
  }

  public void SetParameter(string name, string replacement)
  {
    name = name.Trim('$');
    int index;
    if (!_parameterIndices.TryGetValue(name, out index))
      throw new ArgumentException($"{name} is not a named argument in the template", name);
    index += _parts.Length;
    _replaceGraph[index] = replacement;
    _graphTypeIsReplace[index] = true;
  }

  public void SetParameter(string name, Action<StreamWriter> callback)
  {
    name = name.Trim('$');
    int index;
    if (!_parameterIndices.TryGetValue(name, out index))
      throw new ArgumentException($"{name} is not a named argument in the template", name);
    index += _parts.Length;
    _callbackGraph[index] = callback;
    _graphTypeIsReplace[index] = false;
  }

  private static Regex _parser = new Regex(@"\$(\w{1,64})\$", RegexOptions.Compiled);
  private void Prepare()
  {
    _parameterIndices = new Dictionary<string, int>(64);
    List<string> parts = new List<string>(64);
    List<object> tokens = new List<object>(64);
    int param_index = 0;
    int part_start = 0;

    foreach (Match match in _parser.Matches(_template))
    {
      if (match.Index > part_start)
      {
        //Add Part
        tokens.Add(parts.Count);
        parts.Add(_template.Substring(part_start, match.Index - part_start));
      }


      //Add Parameter
      var param = _template.Substring(match.Index + 1, match.Length - 2);
      if (!_parameterIndices.TryGetValue(param, out param_index))
        _parameterIndices[param] = param_index = _parameterIndices.Count;
      tokens.Add(param);

      part_start = match.Index + match.Length;
    }

    //Add last part, if it exists.
    if (part_start < _template.Length)
    {
      tokens.Add(parts.Count);
      parts.Add(_template.Substring(part_start, _template.Length - part_start));
    }

    //Set State
    _parts = parts.ToArray();
    _tokens = new int[tokens.Count];

    int index = 0;
    foreach (var token in tokens)
    {
      var parameter = token as string;
      if (parameter == null)
        _tokens[index++] = (int)token;
      else
        _tokens[index++] = _parameterIndices[parameter] + _parts.Length;
    }

    _parameters = _parameterIndices.Keys.ToArray();
    int graphlen = _parts.Length + _parameters.Length;
    _callbackGraph = new Action<StreamWriter>[graphlen];
    _replaceGraph = new string[graphlen];
    _graphTypeIsReplace = new bool[graphlen];

    for (int i = 0; i < _parts.Length; i++)
    {
      _graphTypeIsReplace[i] = true;
      _replaceGraph[i] = _parts[i];
    }
  }

  public void GenerateString(Stream output)
  {
    var writer = new StreamWriter(output);
    GenerateString(writer);
    writer.Flush();
  }

  public void GenerateString(StreamWriter writer)
  {
    //Resolve graph
    foreach (var token in _tokens)
    {
      if (_graphTypeIsReplace[token])
        writer.Write(_replaceGraph[token]);
      else
        _callbackGraph[token](writer);
    }
  }

  public void SetReplacements(params string[] parameters)
  {
    int index;
    for (int i = 0; i < _parameters.Length; i++)
    {
      if (!Int32.TryParse(_parameters[i], out index))
        continue;
      else
        SetParameter(index.ToString(), parameters[i]);
    }
  }

  public string GenerateString(int bufferSize = 1024)
  {
    using (var ms = new MemoryStream(bufferSize))
    {
      GenerateString(ms);
      ms.Position = 0;
      using (var reader = new StreamReader(ms))
        return reader.ReadToEnd();
    }
  }

  public string GenerateString(params string[] parameters)
  {
    SetReplacements(parameters);
    return GenerateString();
  }

  public void GenerateString(StreamWriter writer, params string[] parameters)
  {
    SetReplacements(parameters);
    GenerateString(writer);
  }

  private static void Assert(bool test)
  {
    if (!test) throw new Exception("Stahp");
  }

  public static void Test()
  {
    //Unit Test
    var t1 = "$V1$ is the $V2$";
    var t2 = "I am $V1$ awesome";
    var t3 = "$Person$ gave me $25! Yay!";
    var t4 = "I think $0$ is a $1$";
    var t5 = "He asked me to marry him and I said $0$ $0$ $0$";

    StringTemplate rep = new StringTemplate(t1);
    Assert(rep.Parameters.Length == 2);
    rep.SetParameter("V1", "Robear");
    rep.SetParameter("V2", "best");
    Assert(rep.GenerateString() == "Robear is the best");

    rep = new StringTemplate(t2);
    Assert(rep.Parameters.Length == 1);
    rep.SetParameter("V1", "probably not that");
    Assert(rep.GenerateString() == "I am probably not that awesome");

    rep = new StringTemplate(t3);
    rep.Prepare();
    Assert(rep.Parameters.Length == 1);
    rep.SetParameter("Person", "Robear");
    Assert(rep.GenerateString() == "Robear gave me $25! Yay!");

    rep = new StringTemplate(t3);
    rep.Prepare();
    Assert(rep.Parameters.Length == 1);
    rep.SetParameter("Person", x => x.Write("Bob"));
    Assert(rep.GenerateString() == "Bob gave me $25! Yay!");

    rep = new StringTemplate(t4);
    rep.Prepare();
    Assert(rep.Parameters.Length == 2);
    rep.SetReplacements("Tom", "Martian");
    Assert(rep.GenerateString() == "I think Tom is a Martian");

    rep = new StringTemplate(t5);
    rep.Prepare();
    Assert(rep.Parameters.Length == 1);
    rep.SetReplacements("YES");
    Assert(rep.GenerateString() == "He asked me to marry him and I said YES YES YES");
  }
}
