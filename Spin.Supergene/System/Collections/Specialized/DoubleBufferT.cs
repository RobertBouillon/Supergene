using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Specialized;

public class DoubleBuffer<T>
{
  #region Fields
  private T[] _buffers;
  private T _readBuffer;
  private T _writeBuffer;
  private bool _hasData = false;
  private bool _which;
  private System.Threading.ReaderWriterLockSlim _lock = new Threading.ReaderWriterLockSlim();
  Action<T> _clearAction;
  #endregion

  #region Properties

  public bool HasData
  {
    get { return _hasData; }
  }

  public T ReadBuffer
  {
    get { lock (this) return _readBuffer; }
  }

  public T WriteBuffer
  {
    get { lock (this) return _writeBuffer; }
  }

  #endregion

  #region Constructors
  public DoubleBuffer(Action<T> clearAction)
  {
    _clearAction = clearAction;
    _buffers = new T[2];
    _readBuffer = _buffers[0] = Activator.CreateInstance<T>();
    _writeBuffer = _buffers[1] = Activator.CreateInstance<T>();
  }

  public DoubleBuffer(Action<T> clearAction, T readBuffer, T writeBuffer)
  {
    _clearAction = clearAction;
    _buffers = new T[2];
    _readBuffer = _buffers[0] = readBuffer;
    _writeBuffer = _buffers[1] = writeBuffer;
  }

  public DoubleBuffer(Action<T> clearAction, Func<T> initializer)
  {
    _clearAction = clearAction;
    _buffers = new T[2];
    _readBuffer = _buffers[0] = initializer();
    _writeBuffer = _buffers[1] = initializer();
  }
  #endregion

  #region Methods
  public void Write(Action<T> action)
  {
    lock (this)
    {
      _hasData = true;
      action(_writeBuffer);
    }
  }

  public void Read(Action<T> action)
  {
    lock (this)
    {
      //Swap Buffers
      _readBuffer = _which ? _buffers[1] : _buffers[0];
      _writeBuffer = _which ? _buffers[0] : _buffers[1];
      _which = !_which;
      _hasData = false;
    }
    action(_readBuffer);
    _clearAction(_readBuffer);
  }
  #endregion
}
