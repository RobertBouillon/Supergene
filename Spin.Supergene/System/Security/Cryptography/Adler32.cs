using System;

namespace System.Security.Cryptography;

/// <summary>
/// Summary description for Adler32.
/// </summary>
/// <remarks>
/// High order 
/// 
/// 
/// </remarks>
public class Adler32
{
  //Try abd repplace p_Value with two variables representing the HO and LO bytes. See how it fits in the
  //algorithm
  private const uint BASE = 65521;
  #region Private Property Declarations
  private uint p_HighOrder;
  private uint p_LowOrder = 1;
  private uint p_Value;
  #endregion
  #region Public Property Declarations
  public uint Value
  {
    get { return (p_HighOrder << 16) | p_LowOrder; }
  }
  #endregion

  #region Ctors
  public Adler32()
  {
    //
    // TODO: Add constructor logic here
    //
  }
  #endregion

  #region Public Methods
  /// <summary>
  /// Resets the Adler32 checksum to the initial value.
  /// </summary>
  public void Reset()
  {
    p_LowOrder = 1;
    p_HighOrder = 0;
  }

  /// <summary>
  /// Updates the checksum with the byte b.
  /// </summary>
  /// <param name="bval">
  /// the data value to add. The high byte of the int is ignored.
  /// </param>
  public void Update(uint bval)
  {
    //We could make a length 1 byte array and call update again, but I
    //would rather not have that overhead
    uint s1 = p_Value & 0xFFFF;
    uint s2 = p_Value >> 16;

    s1 = (s1 + (bval & 0xFF)) % BASE;
    s2 = (s1 + s2) % BASE;

    p_Value = (s2 << 16) + s1;
  }

  /// <summary>
  /// Updates the checksum with the bytes taken from the array.
  /// </summary>
  /// <param name="buffer">
  /// buffer an array of bytes
  /// </param>
  public void Update(byte[] buffer)
  {
    Update(buffer, 0, buffer.Length);
  }

  /// <summary>
  /// Updates the checksum with the bytes taken from the array.
  /// </summary>
  /// <param name="buf">
  /// an array of bytes
  /// </param>
  /// <param name="off">
  /// the start of the data used for this update
  /// </param>
  /// <param name="len">
  /// the number of bytes to use for this update
  /// </param>
  public void Update(byte[] buffer, int offset, int length)
  {
    #region Validation
    if (buffer == null)
      throw new ArgumentNullException("buffer");

    if (offset < 0 || length < 0 || offset + length > buffer.Length)
    {
      if (offset < 0)
        throw new ArgumentOutOfRangeException("Offset", offset, "Offset cannot be less than 0");
      if (length < 0)
        throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than 0");
      if (offset + length > buffer.Length)
        throw new ArgumentException("Offset and Length cannot be greater than the length of the array");
    }
    #endregion
    //(By Per Bothner)
    uint s1 = p_Value & 0xFFFF;
    uint s2 = p_Value >> 16;

    while (length > 0)
    {
      // We can defer the modulo operation:
      // s1 maximally grows from 65521 to 65521 + 255 * 3800
      // s2 maximally grows by 3800 * median(s1) = 2090079800 < 2^31
      int n = (length < 3800) ? length : 3800;

      length -= n;
      while (--n >= 0)
      {
        s1 = s1 + (uint)(buffer[offset++] & 0xFF);
        s2 = s2 + s1;
      }

      s1 %= BASE;
      s2 %= BASE;
    }

    p_Value = (s2 << 16) | s1;
  }
  #endregion
}
