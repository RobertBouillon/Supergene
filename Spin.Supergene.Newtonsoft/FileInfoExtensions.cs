using Newtonsoft.Json;

namespace System.IO;

public static class JsonFileInfoExtensions
{
  public static void WriteAllJson(this FileInfo file, Action<JsonWriter> json)
  {
    using (var fs = file.OpenWrite())
    using (var buffer = new BufferedStream(fs))
    using (var sw = new StreamWriter(buffer))
    using (var writer = new JsonTextWriter(sw))
    {
      json(writer);
      writer.Flush();
      sw.Flush();
      buffer.Flush();
      fs.Flush();
    }
  }
}
