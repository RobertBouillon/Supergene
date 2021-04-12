using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Linq
{
  public class FluentPropertyParser<T>
  {
    private T _defaultValue;
    private Dictionary<JTokenType, Func<T>> _casters = new Dictionary<JTokenType, Func<T>>();
    public JToken Source { get; }
    public string PropertyName { get; }

    public bool HasValue => !(Source is null);
    public bool HasDefaultValue => EqualityComparer<T>.Default.Equals(_defaultValue, default(T));
    public bool IsRequired { get; private set; }

    private JTokenType GetTokenType(Type type) =>
      type.IsArray ? JTokenType.Array :
      (type == typeof(string)) ? JTokenType.String :
      (type == typeof(int)) ? JTokenType.Integer :
      (type == typeof(float)) ? JTokenType.Float :
      (type == typeof(double)) ? JTokenType.Float :
      (type == typeof(decimal)) ? JTokenType.Float :
      (type.IsClass) ? JTokenType.Object :
      throw new NotSupportedException($"Unknown JSON type: {type}");

    public FluentPropertyParser<T> From<TSource>()
    {
      if (typeof(T).IsEnum)
        _casters.Add(GetTokenType(typeof(TSource)), new Func<T>(ParseEnum));
      else
        _casters.Add(GetTokenType(typeof(TSource)), new Func<T>(Source.Value<T>));
      return this;
    }

    public FluentPropertyParser<T> From<TSource>(Func<TSource, T> parser)
    {
      _casters.Add(GetTokenType(typeof(TSource)), () => parser(Source.Value<TSource>()));
      return this;
    }

    public FluentPropertyParser<T> OrDefault(T defaultValue)
    {
      IsRequired = false;
      _defaultValue = defaultValue;
      return this;
    }

    private T ParseEnum() =>
      Source.Type == JTokenType.String ? (T)Enum.Parse(typeof(T), Source.Value<String>()) :
      Source.Type == JTokenType.Integer ? (T)Enum.ToObject(typeof(T), Source.Value<Int32>()) :
      throw new Exception($"{Source.Type} is not a valid {typeof(T).Name} for {PropertyName}");

    public T Value =>
      (HasValue) ?
        _casters.Count == 0 ?
          typeof(T).IsEnum ? ParseEnum() : Source.ToObject<T>() :
          _casters.TryGetValue(Source.Type, out var func) ? func() : throw new Exception($"{Source.Type} is not a valid type for {PropertyName}") :
        !IsRequired ? _defaultValue : throw new Exception($"{PropertyName} is a required property");

    public FluentPropertyParser(JToken source, string name, bool required = true)
    {
      Source = source[name];
      PropertyName = name;
      IsRequired = required;
    }
  }
}
