using System;
using System.Runtime.InteropServices;

namespace System.IO;

/// <summary>
/// Summary description for FileVersion.
/// </summary>
[Serializable, StructLayout(LayoutKind.Sequential)]
public struct FileVersion
{
  [MarshalAs(UnmanagedType.LPStr)]
  public string Description;
  public int MinorVersion;
  public int MajorVersion;
}
