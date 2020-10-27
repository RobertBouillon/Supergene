using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System.IO
{
  unsafe public class BinaryWriter
  {
    #region Fields
    private readonly UnmanagedMemoryStream _stream;
    #endregion

    #region Constructors
    public BinaryWriter(UnmanagedMemoryStream stream)
    {
      #region Validation
      if (stream == null)
        throw new ArgumentNullException("stream");
      #endregion
      _stream = stream;
    }
    #endregion

    #region Methods
    public void WriteStructure(object structure)
    {
      Marshal.StructureToPtr(structure, (IntPtr)_stream.PositionPointer, false);
      _stream.PositionPointer += Marshal.SizeOf(structure);
    }

    public void Write(string str)
    {
      Marshal.Copy(str.ToCharArray(), 0, (IntPtr)_stream.PositionPointer, str.Length);
      _stream.PositionPointer += str.Length;
    }

    public void Write(int value)
    {
      Marshal.WriteInt32((IntPtr)_stream.PositionPointer, value);
      _stream.PositionPointer += 4;
    }

    public void Write(short value)
    {
      Marshal.WriteInt32((IntPtr)_stream.PositionPointer, value);
      _stream.PositionPointer += 2;
    }

    public void Write(byte value)
    {
      *_stream.PositionPointer = value;
      _stream.PositionPointer++;
    }

    public void Write(long value)
    {
      Marshal.WriteInt64((IntPtr)_stream.PositionPointer, value);
      _stream.PositionPointer += 8;
    }
    #endregion    

  }
}
