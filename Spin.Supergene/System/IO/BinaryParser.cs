using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace System.IO;

/// <summary>
/// Summary description for BinaryParser.
/// </summary>
public class BinaryParser
{
  #region Private Property Declarations
  private Stream p_Source;
  #endregion
  #region Public Property Declarations
  public Stream Source
  {
    get { return p_Source; }
    set { p_Source = value; }
  }
  #endregion
  #region Ctors
  public BinaryParser(Stream source)
  {
    p_Source = source;
  }
  #endregion
  #region Public Methods
  public T Read<T>() where T : struct
  {
    return Read<T>(1)[0];
  }

  unsafe public List<T> Read<T>(int count) where T : struct
  {
    #region Validation
    if (count < 1)
      throw new ArgumentOutOfRangeException("count");
    if (typeof(T).IsAutoLayout)
      throw new ArgumentException(String.Format("Struct '{0}' must be squential or explicit layout", typeof(T)));
    #endregion
    const int STREAM_BUFFER_SIZE = 2048;
    int typelen = Marshal.SizeOf(typeof(T));
    int totallen = typelen * count;
    byte* origbuffer = stackalloc byte[totallen];
    byte* buffer = origbuffer;
    byte[] streambuffer = new byte[STREAM_BUFFER_SIZE]; //allocate a 2k buffer

    List<T> ret = new List<T>(count);
    //Array ret = Array.CreateInstance(typeof(T),count);

    //Copy the bytes from our stream to our buffer
    int sccount = (int)Math.Ceiling(((double)totallen / (double)STREAM_BUFFER_SIZE));
    for (int i = 0; i < sccount; i++)
    {
      int readlen = (i == (sccount - 1)) ? totallen % STREAM_BUFFER_SIZE : STREAM_BUFFER_SIZE;
      int bytesread = 0;
      while (bytesread < readlen)
        bytesread += p_Source.Read(streambuffer, bytesread, readlen - bytesread);

      for (int z = 0; z < readlen; z++)
        buffer[z] = streambuffer[z];
    }

    buffer = origbuffer;

    //Parse the structs and place in the return array
    for (int i = 0; i < count; i++)
    {
      T val = (T)Marshal.PtrToStructure(new IntPtr(buffer), typeof(T));
      ret.Add(val);
      buffer += typelen;
    }

    return ret;
    //TODO: Change the buffer so we're only allocating a few k at a time on the stack.
  }

  unsafe public void Write(object toWrite)
  {
    #region Validation
    if (toWrite == null)
      throw new ArgumentNullException("toWrite");
    #endregion

    int objlen = Marshal.SizeOf(toWrite);
    byte* buffer = stackalloc byte[objlen];

    Marshal.StructureToPtr(toWrite, (IntPtr)buffer, false);

    byte[] streambuffer = new byte[objlen];
    for (int i = 0; i < objlen; i++)
      streambuffer[i] = buffer[i];

    p_Source.Write(streambuffer, 0, objlen);
  }

  public void Write(Array toWrite)
  {
    #region Validation
    if (toWrite == null)
      throw new ArgumentNullException("toWrite");
    if (toWrite.Length == 0)
      throw new ArgumentException("toWrite array cannot be empty");
    #endregion
    foreach (object o in toWrite)
      Write(o);

    //TODO: Allocate a buffer for the array (Similar to read) to increase efficiency
  }

  #endregion
}
