using System;
using System.Collections.Generic;

namespace System.Text.Json
{
  public class FluentPropertyParser<T>
  {
    private T _defaultValue;
    private Dictionary<JsonValueKind, Func<T>> _casters = new Dictionary<JsonValueKind, Func<T>>();
    public JsonElement Source { get; }
    public string PropertyName { get; }

    public bool HasValue => Source.ValueKind != JsonValueKind.Null;
    public bool HasDefaultValue => EqualityComparer<T>.Default.Equals(_defaultValue, default(T));
    public bool IsRequired { get; private set; }

    private JsonValueKind GetTokenType(Type type) =>
      type.IsArray ? JsonValueKind.Array :
      (type == typeof(string)) ? JsonValueKind.String :
      (type == typeof(int)) ? JsonValueKind.Number :
      (type == typeof(float)) ? JsonValueKind.Number :
      (type == typeof(double)) ? JsonValueKind.Number :
      (type == typeof(decimal)) ? JsonValueKind.Number :
      (type.IsClass) ? JsonValueKind.Object :
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
      Source.ValueKind == JsonValueKind.String ? (T)Enum.Parse(typeof(T), Source.GetString()) :
      Source.ValueKind == JsonValueKind.Number ? (T)Enum.ToObject(typeof(T), Source.GetInt32()) :
      throw new Exception($"{Source.ValueKind} is not a valid {typeof(T).Name} for {PropertyName}");

    public T Value =>
      (HasValue) ?
        _casters.Count == 0 ?
          typeof(T).IsEnum ? ParseEnum() : Source.<T>() :
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
