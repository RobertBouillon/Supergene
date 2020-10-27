using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace System.IO
{
  unsafe public class UnmanagedMemoryStream : Stream, IDisposable
  {
    #region Fields
    private readonly IntPtr _buffer;
    private byte* _ptr;
    private readonly long _bufferAddress;
    private readonly long _bufferEndAddress;
    private int _capacity;
    private bool _created = false;
    #endregion

    #region Constructors
    public UnmanagedMemoryStream(int capacity, IntPtr source)
    {
      _buffer = source;
      _ptr = (byte*)_buffer;
      _bufferAddress = _buffer.ToInt64();
      _bufferEndAddress = _bufferAddress + _capacity;
    }

    public UnmanagedMemoryStream(int capacity)
    {
      _created = true;
      _buffer = Marshal.AllocHGlobal(capacity);
      _ptr = (byte*)_buffer;
      _bufferAddress = _buffer.ToInt64();
      _bufferEndAddress = _bufferAddress + _capacity;
    }
    #endregion

    #region Stream Members
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
        if(_created)
          Marshal.FreeHGlobal(_buffer);
    }

    public override bool CanRead
    {
      get { return true; }  
    }

    public override bool CanSeek
    {
      get { return true; }  
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Flush()
    {
      _ptr = (byte *) _buffer;
    }

    public override long Length
    {
      get { return _capacity; }
    }

    public override long Position
    {
      get
      {
        return ((IntPtr)_ptr).ToInt64() - _buffer.ToInt64();
      }
      set
      {
        Seek(value, SeekOrigin.Begin);
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      count = (int)Math.Min(count, _capacity - Position);
      for (int i = 0; i < count; i++)
      {
        buffer[offset + i] = *_ptr;
        _ptr++;
      }
      return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long np;
      switch (origin)
      {
        case SeekOrigin.Begin:
          np = _bufferAddress + offset;
          if (np > _bufferEndAddress)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          if (offset < 0)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          _ptr = (byte*)new IntPtr(np);
          break;
        case SeekOrigin.Current:
          np = _bufferAddress + offset;
          if (np > _bufferEndAddress)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          if (np < _bufferAddress)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          _ptr += offset;
          break;
        case SeekOrigin.End:
          np = _buffer.ToInt64() + _capacity - offset;
          if (np > _bufferEndAddress)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          if (np < _bufferAddress)
            throw new ArgumentOutOfRangeException("Seek position exceeds memory capacity");
          _ptr = (byte*)new IntPtr(np);
          break;
      }
      return Position;
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if(((long)_ptr) + offset > _bufferEndAddress)
        throw new ArgumentOutOfRangeException("Count exceeds memory capacity");

      count = (int)Math.Min(count, _capacity - Position);
      for (int i = 0; i < count; i++)
      {
        buffer[offset + i] = *_ptr;
        _ptr++;
      }
    }
    #endregion

    #region Methods
    unsafe public void WriteStructure(object structure)
    {
      Marshal.StructureToPtr(structure, (IntPtr)_ptr, false);
      _ptr += Marshal.SizeOf(structure);
    }

    public void Write(string str)
    {
      Marshal.Copy(str.ToCharArray(), 0, (IntPtr)_ptr, str.Length);
      _ptr += str.Length;
    }

    public void Write(int value)
    {
      Marshal.WriteInt32((IntPtr)_ptr, value);
      _ptr+=4;
    }

    public void Write(short value)
    {
      Marshal.WriteInt32((IntPtr)_ptr, value);
      _ptr+=2;
    }

    public void Write(byte value)
    {
      *_ptr = value;
      _ptr++;
    }

    public void Write(long value)
    {
      Marshal.WriteInt64((IntPtr)_ptr, value);
      _ptr+=8;
    }
    #endregion
  }
}
