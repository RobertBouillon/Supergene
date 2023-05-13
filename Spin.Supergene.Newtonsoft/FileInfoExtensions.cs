using Newtonsoft.Json;

namespace System.IO;

public static class JsonFileInfoExtensions
{
  public static void WriteAllJson(this FileInfo file, Action<JsonWriter> json)
  {
    using (var fs = file.OpenWrite())
    using (var sw = new StreamWriter(fs))
    using (var writer = new JsonTextWriter(sw))
      json(writer);
  }
}
