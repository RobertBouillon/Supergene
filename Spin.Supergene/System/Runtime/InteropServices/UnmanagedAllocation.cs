using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.InteropServices
{
  public class UnmanagedAllocation : IDisposable
  {
    #region Fields
    private IntPtr _pointer;
    private readonly int _size;
    private bool _isAllocated;
    #endregion

    #region Properties
    public IntPtr Pointer
    {
      get { return _pointer; }
      protected set { _pointer = value; }
    }

    public int Size
    {
      get { return _size; }
    }

    public bool IsAllocated
    {
      get { return _isAllocated; }
      protected set { _isAllocated = value; }
    }
    #endregion

    #region Constructors
    public UnmanagedAllocation(int size)
    {
      _size = size;
    }

    public UnmanagedAllocation(int size, IntPtr data)
    {
      _size = size;
      _pointer = data;
      _isAllocated = true;
    }
    #endregion

    #region Methods
    public virtual void Allocate()
    {
      if (_isAllocated)
        throw new InvalidOperationException("Memory is already allocated");

      _pointer = Marshal.AllocHGlobal(_size);

      _isAllocated = true;
    }

    public virtual void Free()
    {
      if (!_isAllocated)
        throw new InvalidOperationException("Memory is not allocated");

      Marshal.FreeHGlobal(_pointer);

      _pointer = IntPtr.Zero;
      _isAllocated = false;
    }

    public virtual void Clear()
    {
      throw new NotImplementedException();
    }

    unsafe public void Read(Action<BinaryReader> read)
    {
      var stream = new UnmanagedMemoryStream((byte*)_pointer.ToPointer(), _size);
      var reader = new BinaryReader(stream);
      read(reader);
    }

    unsafe public void Write(Action<BinaryWriter> write)
    {
      var stream = new UnmanagedMemoryStream((byte*)_pointer.ToPointer(), _size, _size, FileAccess.Write);
      var writer = new BinaryWriter(stream);
      write(writer);
    }
    #endregion

    #region Overrides
    public void Dispose()
    {
      if (_isAllocated)
        Free();
    }
    #endregion
  }
}
