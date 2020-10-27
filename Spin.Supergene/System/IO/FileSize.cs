using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
  public struct FileSize
  {
    #region Static Declarations
    #endregion

    #region Fields
    private long _size;
    private static string[] _abbreviations = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
    #endregion

    #region Properties
    public long Value
    {
      get { return _size; }
    }
    #endregion

    #region Constructors
    public FileSize(long size)
    {
      _size = size;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
      return ToString(0);
    }

    public string ToString(int roundToPlaces)
    {
      if (_size == 0)
        return "0 B";

      //int places = (int)Math.Log10((double)_size) / 3 * 3;
      int places = (int)Math.Log((double)_size, 8) / 3 * 3;
      var abbreviationKey = places / 3;
      if (abbreviationKey == 0)
        roundToPlaces = 0; //Bytes should never be displayed with a decimal
      var format = roundToPlaces == 0 ? "{0:0} {1}" : "{0:0." + new string('0',roundToPlaces) + "} {1}";
      return String.Format(format, _size / Math.Pow(1024, abbreviationKey), _abbreviations[abbreviationKey]);
    }
    #endregion

    #region Overrides
    public static implicit operator long(FileSize size)
    {
      return size._size;
    }

    public static implicit operator FileSize(long size)
    {
      return new FileSize(size);
    }
    #endregion
  }
}
