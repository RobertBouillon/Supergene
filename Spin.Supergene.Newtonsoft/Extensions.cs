using System.Collections.Generic;

namespace Newtonsoft.Json.Linq;

public static class Extensions
{
  public static FluentPropertyParser<T> ParseProperty<T>(this JObject source, string property, bool required = true) => new FluentPropertyParser<T>(source, property, required);

  public static void WriteArray(this JsonWriter writer, IEnumerable<string> array)
  {
    writer.WriteStartArray();

    foreach (var item in array)
      writer.WriteValue(item);

    writer.WriteEndArray();
  }
}
