using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
  public static class StreamExtension
  {
    public static void CopyTo(this Stream source, Stream destination) => CopyTo(source, destination, 4096);
    public static void CopyTo(this Stream source, Stream destination, int bufferSize)
    {
      byte[] buffer = new byte[bufferSize];

      for (int read = source.Read(buffer, 0, bufferSize); read > 0; read = source.Read(buffer, 0, bufferSize))
        destination.Write(buffer, 0, bufferSize);
    }

    public static void Write(this Stream dis, byte[] source) => dis.Write(source, 0, source.Length);
    public static void Write(this Stream dis, byte[] source, int offset) => dis.Write(source, offset, source.Length);
  }
}
