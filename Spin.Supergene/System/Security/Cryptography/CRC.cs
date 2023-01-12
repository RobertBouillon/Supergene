using System;

namespace System.Security.Cryptography;

/// <summary>
/// Summary description for CRC.
/// </summary>
#region Enumerations
public enum CrcStandard
{
  CRC_CCITT,
  CRC16,
  CRC32
}
#endregion
public class Crc
{
  #region Private Property Declarations
  private int p_Order = 16;
  private ulong p_Polynomial = 0x1021;
  private bool p_Direct = true;
  private ulong p_CrcInitializer = 0xFFFF;
  private ulong p_CrcFinalizer = 0x0;
  private bool p_ReflectIn = false;
  private bool p_ReflectOut = false;
  private bool p_IsStandard = false;
  private byte[] p_Target;
  private byte[] p_ReverseLookup;
  private int p_LeftOffset;
  private ulong p_InitializedCrc;

  private ulong crcmask;
  private ulong crchighbit;
  private ulong crcinit_direct = 0;
  private ulong crcinit_nondirect = 0;
  private ulong[] crctab = new ulong[256];

  //private bool m_IsPrepared = false;
  #endregion
  #region Public Property Declarations
  public byte[] Target
  {
    get { return p_Target; }
    set { p_Target = value; }
  }

  public int Order
  {
    get { return p_Order; }
    set
    {
      p_IsStandard = (value == 8 || value == 16 || value == 32 || value == 64);
      p_LeftOffset = value - 8;
      p_Order = value;
    }
  }

  public ulong Polynomial
  {
    get { return p_Polynomial; }
    set { p_Polynomial = value; }
  }

  /// <summary>
  /// True if the Order is 8,16,32, or 64
  /// </summary>
  public bool IsStandard
  {
    get { return p_IsStandard; }
  }

  /// <summary>
  /// If true, Bytes of value Zero (0x00) will be augmented
  /// </summary>
  /// <remarks>http://www.repairfaq.org/filipg/LINK/F_crc_v33.html</remarks>
  public bool Direct
  {
    get { return p_Direct; }
    set { p_Direct = value; }
  }

  public ulong CrcInitializer
  {
    get { return p_CrcInitializer; }
    set { p_CrcInitializer = value; }
  }

  public ulong CrcFinalizer
  {
    get { return p_CrcFinalizer; }
    set { p_CrcFinalizer = value; }
  }

  public bool ReflectIn
  {
    get { return p_ReflectIn; }
    set { p_ReflectIn = value; }
  }

  public bool ReflectOut
  {
    get { return p_ReflectOut; }
    set { p_ReflectOut = value; }
  }
  #endregion

  #region Ctors
  public Crc()
  {
    PopulateReverseLookup();
    //TODO: Disabling reflect breaks the class
  }

  public Crc(CrcStandard defaults) : this()
  {
    p_IsStandard = true;
    switch (defaults)
    {
      case CrcStandard.CRC_CCITT:
        p_Order = 16;
        p_Direct = true;
        p_Polynomial = 0x1021;
        p_CrcInitializer = 0xFFFF;
        p_CrcFinalizer = 0;
        p_ReflectIn = false;
        p_ReflectOut = false;
        break;
      case CrcStandard.CRC16:
        p_Order = 16;
        p_Direct = true;
        p_Polynomial = 0x8005;
        p_CrcInitializer = 0x0;
        p_CrcFinalizer = 0x0;
        p_ReflectIn = true;
        p_ReflectOut = true;
        break;
      case CrcStandard.CRC32:
        p_Order = 32;
        p_Direct = true;
        p_Polynomial = 0x4c11db7;
        p_CrcInitializer = 0xFFFFFFFF;
        p_CrcFinalizer = 0xFFFFFFFF;
        p_ReflectIn = true;
        p_ReflectOut = true;
        break;
    }
    Prepare();
  }
  #endregion

  #region Public Methods
  public ulong Calculate()
  {
    if (p_IsStandard)
      return CalculateByTable();
    else
      return CalculateByBit();
  }
  #endregion

  /// <summary>
  /// Caches expensive functions to allow for fast repeat calculations
  /// </summary>
  public void Optimize()
  {
    //This creates a mask that prevents the return value from exceeding the byte order mark
    crcmask = ((((ulong)1 << (p_Order - 1)) - 1) << 1) | 1;

    //This is a decimal representation of the Most Significant Byte
    crchighbit = (ulong)1 << (p_Order - 1);

    //Generates a lookup table for fast computation
    generate_crc_table();
  }

  public void Prepare()
  {
    Optimize();

    bool check;
    int i;

    //If we are using a standard order, we can use tabular calculations for direct and 
    //non-direct. Otherwise, we use bit-by-bit.

    //Convert our Initial CRC value from Direct<->Non Direct if we're using a standard order
    //So that we can use fast table access either way.
    if (!p_Direct)
    {
      //crcinit_nondirect = p_CrcInitializer;
      p_InitializedCrc = p_CrcInitializer;
      for (i = 0; i < p_Order; i++)
      {
        check = (p_InitializedCrc & crchighbit) != 0;
        p_InitializedCrc <<= 1;
        if (check)
          p_InitializedCrc ^= p_Polynomial;

      }
      p_InitializedCrc &= crcmask;
    }
    else if (!p_IsStandard)
    {
      p_InitializedCrc = p_CrcInitializer;
      for (i = 0; i < p_Order; i++)
      {
        check = (p_InitializedCrc & 1) != 0;
        if (check)
          p_InitializedCrc ^= p_Polynomial;

        p_InitializedCrc >>= 1;
        if (check)
          p_InitializedCrc |= crchighbit;
      }
    }

    p_Direct = p_IsStandard || p_Direct;
  }

  unsafe private ulong CalculateByBit()
  {
    ulong j, c, bit;
    ulong crc = p_InitializedCrc;

    for (int i = 0; i < p_Target.Length; i++)
    {
      c = (ulong)p_Target[i];
      if (p_ReflectIn)
        c = Reverse(c, 8);

      if (!p_Direct)
      {
        for (j = 0x80; j > 0; j >>= 1)
        {
          bit = crc & crchighbit;
          crc <<= 1;
          if ((c & j) > 0)
            bit ^= crchighbit;
          if (bit > 0)
            crc ^= p_Polynomial;
        }

        for (i = 0; (int)i < p_Order; i++)
        {
          bit = crc & crchighbit;
          crc <<= 1;
          if (bit != 0) crc ^= p_Polynomial;
        }
      }
      else
      {
        for (j = 0x80; j != 0; j >>= 1)
        {
          bit = crc & crchighbit;
          crc <<= 1;
          if ((c & j) != 0)
            crc |= 1;
          if (bit != 0)
            crc ^= p_Polynomial;
        }

      }
    }

    if (p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= p_CrcFinalizer;
    crc &= crcmask;

    return (crc);
  }

  unsafe private ulong CalculateByTable()
  {
    ulong crc = p_InitializedCrc;
    fixed (byte* fastarray = &p_Target[0])
    {
      if (p_Direct)
      {
        if (p_ReflectIn)
        {
          crc = Reverse(crc, p_Order);

          for (int i = 0; i < p_Target.Length; i++)
            crc = (crc >> 8) ^ crctab[((byte)crc) ^ fastarray[i]];
        }
        else
          for (int i = 0; i < p_Target.Length; i++)
            crc = (crc << 8) ^ crctab[((byte)(crc >> p_LeftOffset)) ^ fastarray[i]];
      }
      else
      {
        if (!p_ReflectIn)
        {
          for (int i = 0; i < p_Target.Length; i++)
            crc = ((crc << 8) | fastarray[i]) ^ crctab[(byte)(crc >> p_LeftOffset)];

          for (int i = 0; i < p_Order / 8; i++)
            crc = (crc << 8) ^ crctab[(byte)(crc >> p_LeftOffset)];
        }
        else
        {
          for (int i = 0; i < p_Target.Length; i++)
            crc = (ulong)(((int)(crc >> 8) | (fastarray[i] << p_LeftOffset)) ^ (int)crctab[(byte)crc]);

          for (int i = 0; i < p_Order / 8; i++)
            crc = (crc >> 8) ^ crctab[(byte)crc];
        }
      }
    }

    if (p_ReflectIn ^ p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= CrcFinalizer;
    crc &= crcmask;
    return (crc);
  }


  /// <summary>
  /// 4 ways to calculate the crc checksum. If you have to do a lot of encoding
  /// you should use the table functions. Since they use precalculated values, which 
  /// saves some calculating.
  /// </summary>.
  public ulong crctablefast(byte[] p)
  {
    // fast lookup table algorithm without augmented zero bytes, e.g. used in pkzip.
    // only usable with polynom orders of 8, 16, 24 or 32.
    ulong crc = crcinit_direct;
    if (p_ReflectIn)
      crc = Reverse(crc, p_Order);

    if (!p_ReflectIn)
      for (int i = 0; i < p.Length; i++)
        crc = (crc << 8) ^ crctab[((crc >> (p_Order - 8)) & 0xff) ^ p[i]];
    else
      for (int i = 0; i < p.Length; i++)
        crc = (crc >> 8) ^ crctab[(crc & 0xff) ^ p[i]];

    if (p_ReflectIn ^ p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= CrcFinalizer;
    crc &= crcmask;
    return (crc);
  }

  public ulong crctable(byte[] p)
  {
    // normal lookup table algorithm with augmented zero bytes.
    // only usable with polynom orders of 8, 16, 24 or 32.
    ulong crc = crcinit_nondirect;
    if (p_ReflectIn)
      crc = Reverse(crc, p_Order);

    if (!p_ReflectIn)
      for (int i = 0; i < p.Length; i++)
        crc = ((crc << 8) | p[i]) ^ crctab[(crc >> (p_Order - 8)) & 0xff];
    else
      for (int i = 0; i < p.Length; i++)
        crc = (ulong)(((int)(crc >> 8) | (p[i] << (p_Order - 8))) ^ (int)crctab[crc & 0xff]);

    if (!p_ReflectIn)
      for (int i = 0; i < p_Order / 8; i++)
        crc = (crc << 8) ^ crctab[(crc >> (p_Order - 8)) & 0xff];
    else
      for (int i = 0; i < p_Order / 8; i++)
        crc = (crc >> 8) ^ crctab[crc & 0xff];

    if (p_ReflectIn ^ p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= p_CrcFinalizer;
    crc &= crcmask;

    return (crc);
  }

  public ulong crcbitbybit(byte[] p)
  {
    // bit by bit algorithm with augmented zero bytes.
    // does not use lookup table, suited for polynom orders between 1...32.
    int i;
    ulong j, c, bit;
    ulong crc = crcinit_nondirect;

    for (i = 0; i < p.Length; i++)
    {
      c = (ulong)p[i];
      if (p_ReflectIn)
        c = Reverse(c, 8);

      for (j = 0x80; j != 0; j >>= 1)
      {
        bit = crc & crchighbit;
        crc <<= 1;
        if ((c & j) != 0)
        {
          crc |= 1;
        }
        if (bit != 0)
        {
          crc ^= p_Polynomial;
        }
      }
    }

    for (i = 0; (int)i < p_Order; i++)
    {

      bit = crc & crchighbit;
      crc <<= 1;
      if (bit != 0) crc ^= p_Polynomial;
    }

    if (p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= p_CrcFinalizer;
    crc &= crcmask;

    return (crc);
  }

  public ulong crcbitbybitfast(byte[] p)
  {
    // fast bit by bit algorithm without augmented zero bytes.
    // does not use lookup table, suited for polynom orders between 1...32.
    int i;
    ulong j, c, bit;
    ulong crc = crcinit_direct;

    for (i = 0; i < p.Length; i++)
    {
      c = (ulong)p[i];
      if (p_ReflectIn)
        c = Reverse(c, 8);

      for (j = 0x80; j > 0; j >>= 1)
      {
        bit = crc & crchighbit;
        crc <<= 1;
        if ((c & j) > 0) bit ^= crchighbit;
        if (bit > 0) crc ^= p_Polynomial;
      }
    }

    if (p_ReflectOut)
      crc = Reverse(crc, p_Order);

    crc ^= p_CrcFinalizer;
    crc &= crcmask;

    return (crc);
  }


  /// <summary>
  /// CalcCRCITT is an algorithm found on the web for calculating the CRCITT checksum
  /// It is included to demonstrate that although it looks different it is the same 
  /// routine as the crcbitbybit* functions. But it is optimized and preconfigured for CRCITT.
  /// </summary>
  public ushort CalcCRCITT(byte[] p)
  {
    uint uiCRCITTSum = 0xFFFF;
    uint uiByteValue;

    for (int iBufferIndex = 0; iBufferIndex < p.Length; iBufferIndex++)
    {
      uiByteValue = ((uint)p[iBufferIndex] << 8);
      for (int iBitIndex = 0; iBitIndex < 8; iBitIndex++)
      {
        if (((uiCRCITTSum ^ uiByteValue) & 0x8000) != 0)
        {
          uiCRCITTSum = (uiCRCITTSum << 1) ^ 0x1021;
        }
        else
        {
          uiCRCITTSum <<= 1;
        }
        uiByteValue <<= 1;
      }
    }
    return (ushort)uiCRCITTSum;
  }


  #region Private Methods
  /// <summary>
  /// This method performs a binary reverse of the provided ulong
  /// </summary>
  /// <param name="toReverse">The number to reverse</param>
  /// <param name="width">The number of Least-significant bytes to reverse.</param>
  /// <returns>A reversed version of toReverse</returns>
  private ulong Reverse(ulong toReverse, int width)
  {
    ulong reversed = 0;
    for (int i = 0; i < width; i += 8)
      reversed += (((ulong)p_ReverseLookup[(byte)(toReverse >> i)]) << (width - (i + 8)));

    return reversed;
  }


  /// <summary>
  /// Applies the polynomial to a table of 256 sequential numbers, starting at 0, and saves 
  /// the information to the CrcTable Property.
  /// </summary>
  private void generate_crc_table()
  {
    bool applypolynomial;
    ulong crc;
    //TODO: Test performance by reflecting the entire table before and after rather than during.

    for (int i = 0; i < 256; i++)
    {
      crc = (ulong)i;
      if (p_ReflectIn)
        crc = Reverse(crc, 8);

      crc <<= p_Order - 8;

      for (int j = 0; j < 8; j++)
      {
        applypolynomial = (crc & crchighbit) != 0;
        crc <<= 1;
        if (applypolynomial)
          crc ^= p_Polynomial;
      }

      if (p_ReflectIn)
        crc = Reverse(crc, p_Order);

      crc &= crcmask;
      crctab[i] = crc;
    }

  }

  private void PopulateReverseLookup()
  {
    byte j = 1;
    byte reversed = 0;
    p_ReverseLookup = new byte[256];

    for (byte toReverse = 1; toReverse != 0; toReverse++)
    {
      j = 1;
      reversed = 0;
      for (byte i = 1 << 7; i != 0; i >>= 1)
      {
        if ((toReverse & i) != 0)
          reversed |= j;

        j <<= 1;
      }
      p_ReverseLookup[toReverse] = reversed;
    }
  }


  #endregion
}

