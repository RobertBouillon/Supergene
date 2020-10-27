using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.CodeDom
{
  public class CsCodeBuilder
  {
    #region Static Declarations
    private static Regex _parseGenericName = new Regex(@"(\w+)\`", RegexOptions.Compiled);
    #endregion
    #region Fields
    private StringBuilder _buffer;
    private int _explicitScopeCount;
    private string _indent;
    private int _regions;
    #endregion

    #region Properties
    public StringBuilder Buffer { get; set; }
    #endregion

    #region Cosntructors
    public CsCodeBuilder()
    {
      _buffer = new StringBuilder();
      _indent = String.Empty;
    }
    #endregion

    #region Methods
    public static string GetTypeName(Type type, bool qualified = false)
    {
      if (type.GenericTypeArguments.Length == 0)
        return qualified ? type.FullName : type.Name;
      else
        return String.Format(qualified ? "{0}.{1}<{2}>" : "{1}<{2}>", type.Namespace, _parseGenericName.Match(type.Name).Groups[1].Value, String.Join(", ", type.GenericTypeArguments.Select(x => GetTypeName(x, qualified)).ToArray()));
    }

    public void CreateFields<T>(IEnumerable<T> source, Func<T, string> type, Func<T, string> name)
    {
      foreach (T item in source)
        AddField(type(item), name(item));
    }

    public void AddField(string type, string name, string comment = null)
    {
      if(!String.IsNullOrWhiteSpace(comment))
        AddCode("///<summary>{0}</summary>", comment.Replace("\r\n", "<br/>"));
      AddCode("private {0} {1};", type, name);
    }

    public void EnterRegion(string format, params string[] args) => EnterRegion(String.Format(format, args));
    public void EnterRegion(string name)
    {
      _regions++;
      AddCode("#region {0}", name);
    }

    public void ExitRegion()
    {
      _regions--;
      AddCode("#endregion");
    }

    public void AddUsing(string usingNamespace) => AddCode("using {0};", usingNamespace);
    public void AddUsing(string usingNamespace, params object[] parms) => AddCode("using {0};", String.Format(usingNamespace, parms));
    public void EnterNamespace(string namespaceName) => EnterScope("namespace {0}", namespaceName);
    public void EnterNamespace(string namespaceName, params object[] args) => EnterScope("namespace {0}", String.Format(namespaceName, args));
    public void ExitNamespace() => ExitScope();
    public void EnterType(string accessModifier, string typename) => EnterScope("{0} class {1}", accessModifier, typename);
    public void EnterType(string accessModifier, string typename, params string[] inherit) => EnterScope("{0} class {1} : {2}", accessModifier, typename, String.Join(", ", inherit));
    public void ExitType() => ExitScope();
    public void EnterScope(string format, params object[] args) => EnterScope(String.Format(EscapeCurlyBraces(format), args));

    public void EnterScope(string scope)
    {
      AddCode(scope);
      AddCode("{");
      Indent();
    }

    public void ExitScope()
    {
      Unindent();
      AddCode("}");
    }

    public void Indent()
    {
      _explicitScopeCount++;
      SetIndent();
    }

    public void Unindent()
    {
      if (_explicitScopeCount <= 0)
        throw new InvalidOperationException("Cannot unindent. Currently not indented.");

      _explicitScopeCount--;
      SetIndent();
    }

    public void AddCode(string format, params object[] args) => AddCode(String.Format(EscapeCurlyBraces(format), args));
    public void AddCode(string code) => _buffer.AppendFormat("{0}{1}{2}", _indent, code, Environment.NewLine);
    public void LineFeed() => _buffer.AppendLine();
    public void LineFeed(int lines) => _buffer.Append(String.Concat(Enumerable.Repeat("\r\n", lines)));
    #endregion

    #region Private Methods
    private static Regex _reformat = new Regex(@"\{(\{\d\})\}", RegexOptions.Compiled);
    private string EscapeCurlyBraces(string format) => _reformat.ReplaceFormat(format.Replace("}", "}}").Replace("{", "{{"), "{1}");
    private void SetIndent() => _indent = new String(' ', _explicitScopeCount * 2);
    public override string ToString() => _buffer.ToString();
    #endregion
  }
}
