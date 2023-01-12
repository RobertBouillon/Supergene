using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO;

public static class FileInfoExtensions
{
  private static Regex _dedupParser = new Regex(@"(.+?)((_\d+$)|$)", RegexOptions.Compiled);

  public static bool IsLocked(this FileInfo file, TimeSpan timeout, TimeSpan pollInterval)
  {
    int iterations = (int)(timeout.Ticks / pollInterval.Ticks);
    for (int i = 0; i < iterations; i++)
      if (IsLocked(file))
        Thread.Sleep(pollInterval);
      else
        return false;
    return true;
  }

  public static bool IsLocked(this FileInfo file)
  {
    FileStream stream = null;
    try
    {
      stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }
    catch (IOException)
    {
      return true;
    }
    finally
    {
      if (stream != null)
        stream.Close();
    }
    return false;
  }

  public static FileInfo MoveWithRename(this FileInfo file, DirectoryInfo destination)
  {
    var dedup = DeduplicateFileName(new FileInfo(Path.Combine(destination.FullName, file.Name)));
    file.MoveTo(dedup.FullName);
    return dedup;
  }

  public static FileInfo MoveWithRename(this FileInfo file, string newName)
  {
    var dedup = DeduplicateFileName(new FileInfo(Path.Combine(file.DirectoryName, newName)));
    file.MoveTo(dedup.FullName);
    return dedup;
  }

  public static FileInfo CopyWithRename(this FileInfo file, DirectoryInfo destination)
  {
    var dedup = DeduplicateFileName(new FileInfo(Path.Combine(destination.FullName, file.Name)));
    file.CopyTo(dedup.FullName);
    return dedup;
  }


  public static FileInfo DeduplicateFileName(FileInfo file)
  {
    string shortname = _dedupParser.Match(Path.GetFileNameWithoutExtension(file.Name)).Groups[1].Value;
    int index = 1;
    while (file.Exists)
      file = new FileInfo(String.Format("{0}_{1}{2}",
        Path.Combine(file.DirectoryName, shortname),
        (index++).ToString(),
        file.Extension
      ));
    return file;
  }

  public static void OpenWriter(this FileInfo file, Action<BinaryWriter> action, CompressionLevel compress = CompressionLevel.Optimal, bool buffered = true)
  {
    if (!buffered)
      if (compress != CompressionLevel.NoCompression)
        using (var stream = file.OpenWrite())
        using (var zip = new GZipStream(stream, compress))
        using (var writer = new BinaryWriter(zip))
          action(writer);
      else
        using (var stream = file.OpenWrite())
        using (var writer = new BinaryWriter(stream))
          action(writer);
    else
      if (compress != CompressionLevel.NoCompression)
      using (var stream = file.OpenWrite())
      using (var buffer = new BufferedStream(stream, 4 * 1024 * 1024))  //4MB
      using (var zip = new GZipStream(buffer, compress))
      using (var writer = new BinaryWriter(zip))
        action(writer);
    else
      using (var stream = file.OpenWrite())
      using (var buffer = new BufferedStream(stream, 4 * 1024 * 1024))  //4MB
      using (var writer = new BinaryWriter(buffer))
        action(writer);
  }

  public static void OpenReader(this FileInfo file, Action<BinaryReader> action, bool compressed = false)
  {
    if (compressed)
      using (var stream = file.OpenRead())
      using (var zip = new GZipStream(stream, CompressionMode.Decompress))
      using (var reader = new BinaryReader(zip))
        action(reader);
    else
      using (var stream = file.OpenRead())
      using (var reader = new BinaryReader(stream))
        action(reader);
  }

  public static void CopyTo(this FileInfo file, FileInfo destination, Action<int> progressCallback)
  {
    const int bufferSize = 1024 * 1024;  //1MB
    byte[] buffer = new byte[bufferSize], buffer2 = new byte[bufferSize];
    bool swap = false;
    int progress = 0, progress2 = 0, read = 0;
    long len = file.Length;
    float flen = len;
    Task writer = null;

    using (var source = file.OpenRead())
    using (var dest = destination.OpenWrite())
    {
      dest.SetLength(len);
      for (long size = 0; size < len; size += read)
      {
        if ((progress = ((int)((size / flen) * 100))) != progress2)
          progressCallback(progress2 = progress);
        read = source.Read(swap ? buffer : buffer2, 0, bufferSize);
        if (writer != null) writer.Wait();
        writer = dest.WriteAsync(swap ? buffer : buffer2, 0, read);
        swap = !swap;
      }
      dest.Write(swap ? buffer2 : buffer, 0, read);
    }
  }
}
