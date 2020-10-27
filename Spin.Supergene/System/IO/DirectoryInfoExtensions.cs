using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.IO
{
  public static class DirectoryInfoExtensions
  {
    public static void Traverse(this DirectoryInfo dir, Action<DirectoryInfo, FileInfo> action)
    {
      action(dir, null);
      foreach (FileInfo file in dir.GetFiles())
        action(dir, file);
      foreach (DirectoryInfo sdir in dir.GetDirectories())
        Traverse(sdir, action);
    }

    public static IEnumerable<FileInfo> GetFiles(this DirectoryInfo dir, Func<FileInfo, bool> filter, SearchOption options)
    {
      List<FileInfo> files = new List<FileInfo>();
      dir.Traverse((x, y) => { if (y != null) if (filter(y)) files.Add(y); });
      return files;
    }

    public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
    {
      if (!target.Exists)
        target.Create();

      foreach (var file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name), true);

      foreach (var subdir in source.GetDirectories())
        subdir.CopyTo(target.CreateSubdirectory(subdir.Name));
    }
  }
}
