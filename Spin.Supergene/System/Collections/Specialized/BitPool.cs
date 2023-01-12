using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Specialized;

public class BitPool
{
  #region Fields
  private uint[] _source;

  private readonly int _size;
  private int _allocated;
  #endregion


  #region Properties

  public int Size
  {
    get { return _size; }
  }

  public int Allocated
  {
    get { return _allocated; }
  }

  public int Available
  {
    get { return _size - _allocated; }
  }
  #endregion

  #region Constructors
  public BitPool(int size)
  {
    #region Validation
    if (size < 0)
      throw new ArgumentOutOfRangeException("bits must be greater than 0");
    #endregion
    _source = new uint[(size / 32) + 1];
  }
  #endregion



  #region Methods
  public int ReserveNextAvailable()
  {
    return ReserveNextAvailable(0);
  }

  public int ReserveNextAvailable(int startIndex)
  {
    if (_size == _allocated)
      throw new InvalidOperationException("No more bits available in the pool");

    int i = startIndex / 32;
    while (_source[i] == UInt32.MaxValue)
      if (++i > _source.Length)
        return -1;

    ++_allocated;
    var val = _source[i];
    uint x = 1;
    int y = 0;
    while ((x & val) > 0)
    {
      x <<= 1;
      y++;
    }

    _source[i] |= x;
    return (i * 32) + y;
  }

  public bool Reserve(int index)
  {
    if (_size == _allocated)
      throw new InvalidOperationException("No more bits available in the pool");

    int i = index / 32;
    uint mask = (uint)(1 << (index % 32));
    if ((_source[i] & mask) != 0)
      return false;

    _source[i] |= mask;
    ++_allocated;
    return true;
  }

  public bool Release(int index)
  {
    int i = index / 32;
    uint mask = (uint)(1 << (index % 32));
    if ((_source[i] & mask) == 0)
      return false;

    _source[i] &= ~mask;
    --_allocated;
    return true;
  }
  #endregion
}
