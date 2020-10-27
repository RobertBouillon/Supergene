﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace System
{
  public struct Int128 : IFormattable, IComparable, IComparable<Int128>, IEquatable<Int128>
  {
    private UInt128 v;

    public static Int128 MinValue { get; } = (Int128)((UInt128)1 << 127);
    public static Int128 MaxValue { get; } = (Int128)(((UInt128)1 << 127) - 1);
    public static Int128 Zero { get; } = (Int128)0;
    public static Int128 One { get; } = (Int128)1;
    public static Int128 MinusOne { get; } = (Int128)(-1);

    public static Int128 Parse(string value)
    {
      Int128 c;
      if (!TryParse(value, out c))
        throw new FormatException();
      return c;
    }

    public static bool TryParse(string value, out Int128 result) => TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);

    public static bool TryParse(string value, NumberStyles style, IFormatProvider format, out Int128 result)
    {
      if (!BigInteger.TryParse(value, style, format, out var a))
      {
        result = Int128.Zero;
        return false;
      }
      UInt128.Create(out result.v, a);
      return true;
    }

    public Int128(long value) => UInt128.Create(out v, value);
    public Int128(ulong value) => UInt128.Create(out v, value);
    public Int128(double value) => UInt128.Create(out v, value);
    public Int128(decimal value) => UInt128.Create(out v, value);
    public Int128(BigInteger value) => UInt128.Create(out v, value);

    public Int128(ulong s0, ulong s1) => UInt128.Create(out v, s0, s1);

    public ulong S0 { get { return v.S0; } }
    public ulong S1 { get { return v.S1; } }

    public bool IsZero { get { return v.IsZero; } }
    public bool IsOne { get { return v.IsOne; } }
    public bool IsPowerOfTwo { get { return v.IsPowerOfTwo; } }
    public bool IsEven { get { return v.IsEven; } }
    public bool IsNegative { get { return v.S1 > long.MaxValue; } }
    public int Sign { get { return IsNegative ? -1 : v.Sign; } }

    public override string ToString() => ((BigInteger)this).ToString();
    public string ToString(string format) => ((BigInteger)this).ToString(format);
    public string ToString(IFormatProvider provider) => ToString(null, provider);
    public string ToString(string format, IFormatProvider provider) => ((BigInteger)this).ToString(format, provider);

    public static explicit operator Int128(double a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(sbyte a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(byte a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(short a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(ushort a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(int a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(uint a)
    {
      Int128 c;
      UInt128.Create(out c.v, (ulong)a);
      return c;
    }

    public static implicit operator Int128(long a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static implicit operator Int128(ulong a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static explicit operator Int128(decimal a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static explicit operator Int128(UInt128 a)
    {
      Int128 c;
      c.v = a;
      return c;
    }

    public static explicit operator UInt128(Int128 a)
    {
      return a.v;
    }

    public static explicit operator Int128(BigInteger a)
    {
      Int128 c;
      UInt128.Create(out c.v, a);
      return c;
    }

    public static explicit operator sbyte(Int128 a) => (sbyte)a.v.S0;
    public static explicit operator byte(Int128 a) => (byte)a.v.S0;
    public static explicit operator short(Int128 a) => (short)a.v.S0;
    public static explicit operator ushort(Int128 a) => (ushort)a.v.S0;
    public static explicit operator int(Int128 a) => (int)a.v.S0;
    public static explicit operator uint(Int128 a) => (uint)a.v.S0;
    public static explicit operator long(Int128 a) => (long)a.v.S0;
    public static explicit operator ulong(Int128 a) => a.v.S0;
    public static explicit operator decimal(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 c;
        UInt128.Negate(out c, ref a.v);
        return -(decimal)c;
      }
      return (decimal)a.v;
    }

    public static implicit operator BigInteger(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 c;
        UInt128.Negate(out c, ref a.v);
        return -(BigInteger)c;
      }
      return (BigInteger)a.v;
    }

    public static explicit operator float(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 c;
        UInt128.Negate(out c, ref a.v);
        return -UInt128.ConvertToFloat(ref c);
      }
      return UInt128.ConvertToFloat(ref a.v);
    }

    public static explicit operator double(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 c;
        UInt128.Negate(out c, ref a.v);
        return -UInt128.ConvertToDouble(ref c);
      }
      return UInt128.ConvertToDouble(ref a.v);
    }

    public static Int128 operator <<(Int128 a, int b)
    {
      Int128 c;
      UInt128.LeftShift(out c.v, ref a.v, b);
      return c;
    }

    public static Int128 operator >>(Int128 a, int b)
    {
      Int128 c;
      UInt128.ArithmeticRightShift(out c.v, ref a.v, b);
      return c;
    }

    public static Int128 operator &(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.And(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 operator &(Int128 a, int b) => a & (Int128)b;
    public static Int128 operator &(int a, Int128 b) => b & (Int128)a;
    public static Int128 operator &(Int128 a, long b) => a & (Int128)b;
    public static Int128 operator &(long a, Int128 b) => b & (Int128)a;

    public static Int128 operator |(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.Or(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 operator ^(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.ExclusiveOr(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 operator ~(Int128 a)
    {
      Int128 c;
      UInt128.Not(out c.v, ref a.v);
      return c;
    }

    public static Int128 operator +(Int128 a, long b)
    {
      Int128 c;
      if (b < 0)
        UInt128.Subtract(out c.v, ref a.v, (ulong)(-b));
      else
        UInt128.Add(out c.v, ref a.v, (ulong)b);
      return c;
    }

    public static Int128 operator +(long a, Int128 b)
    {
      Int128 c;
      if (a < 0)
        UInt128.Subtract(out c.v, ref b.v, (ulong)(-a));
      else
        UInt128.Add(out c.v, ref b.v, (ulong)a);
      return c;
    }

    public static Int128 operator +(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.Add(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 operator ++(Int128 a)
    {
      Int128 c;
      UInt128.Add(out c.v, ref a.v, 1);
      return c;
    }

    public static Int128 operator -(Int128 a, long b)
    {
      Int128 c;
      if (b < 0)
        UInt128.Add(out c.v, ref a.v, (ulong)(-b));
      else
        UInt128.Subtract(out c.v, ref a.v, (ulong)b);
      return c;
    }

    public static Int128 operator -(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.Subtract(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 operator +(Int128 a) => a;
    public static Int128 operator -(Int128 a)
    {
      Int128 c;
      UInt128.Negate(out c.v, ref a.v);
      return c;
    }

    public static Int128 operator --(Int128 a)
    {
      Int128 c;
      UInt128.Subtract(out c.v, ref a.v, 1);
      return c;
    }

    public static Int128 operator *(Int128 a, int b)
    {
      Int128 c;
      Multiply(out c, ref a, b);
      return c;
    }

    public static Int128 operator *(int a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref b, a);
      return c;
    }

    public static Int128 operator *(Int128 a, uint b)
    {
      Int128 c;
      Multiply(out c, ref a, b);
      return c;
    }

    public static Int128 operator *(uint a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref b, a);
      return c;
    }

    public static Int128 operator *(Int128 a, long b)
    {
      Int128 c;
      Multiply(out c, ref a, b);
      return c;
    }

    public static Int128 operator *(long a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref b, a);
      return c;
    }

    public static Int128 operator *(Int128 a, ulong b)
    {
      Int128 c;
      Multiply(out c, ref a, b);
      return c;
    }

    public static Int128 operator *(ulong a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref b, a);
      return c;
    }

    public static Int128 operator *(Int128 a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref a, ref b);
      return c;
    }

    public static Int128 operator /(Int128 a, int b)
    {
      Int128 c;
      Divide(out c, ref a, b);
      return c;
    }

    public static Int128 operator /(Int128 a, uint b)
    {
      Int128 c;
      Divide(out c, ref a, b);
      return c;
    }

    public static Int128 operator /(Int128 a, long b)
    {
      Int128 c;
      Divide(out c, ref a, b);
      return c;
    }

    public static Int128 operator /(Int128 a, ulong b)
    {
      Int128 c;
      Divide(out c, ref a, b);
      return c;
    }

    public static Int128 operator /(Int128 a, Int128 b)
    {
      Int128 c;
      Divide(out c, ref a, ref b);
      return c;
    }

    //public static int operator %(Int128 a, int b) => Remainder(ref a, b);
    //public static int operator %(Int128 a, uint b) => Remainder(ref a, b);
    //public static long operator %(Int128 a, long b) => Remainder(ref a, b);
    //public static long operator %(Int128 a, ulong b) => Remainder(ref a, b);
    public static Int128 operator %(Int128 a, Int128 b)
    {
      Int128 c;
      Remainder(out c, ref a, ref b);
      return c;
    }

    public static bool operator <(Int128 a, UInt128 b) => a.CompareTo(b) < 0;
    public static bool operator <(UInt128 a, Int128 b) => b.CompareTo(a) > 0;
    public static bool operator <(Int128 a, Int128 b) => LessThan(ref a.v, ref b.v);
    public static bool operator <(Int128 a, int b) => LessThan(ref a.v, b);
    public static bool operator <(int a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, uint b) => LessThan(ref a.v, b);
    public static bool operator <(uint a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, long b) => LessThan(ref a.v, b);
    public static bool operator <(long a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <(Int128 a, ulong b) => LessThan(ref a.v, b);
    public static bool operator <(ulong a, Int128 b) => LessThan(a, ref b.v);
    public static bool operator <=(Int128 a, UInt128 b) => a.CompareTo(b) <= 0;
    public static bool operator <=(UInt128 a, Int128 b) => b.CompareTo(a) >= 0;
    public static bool operator <=(Int128 a, Int128 b) => LessThan(ref b.v, ref a.v) || a.Equals(b);
    public static bool operator <=(Int128 a, int b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator <=(int a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator <=(Int128 a, uint b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator <=(uint a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator <=(Int128 a, long b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator <=(long a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator <=(Int128 a, ulong b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator <=(ulong a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator >(Int128 a, UInt128 b) => a.CompareTo(b) > 0;
    public static bool operator >(UInt128 a, Int128 b) => b.CompareTo(a) < 0;
    public static bool operator >(Int128 a, Int128 b) => !LessThan(ref b.v, ref a.v);
    public static bool operator >(Int128 a, int b) => LessThan(b, ref a.v);
    public static bool operator >(int a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, uint b) => LessThan(b, ref a.v);
    public static bool operator >(uint a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, long b) => LessThan(b, ref a.v);
    public static bool operator >(long a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >(Int128 a, ulong b) => LessThan(b, ref a.v);
    public static bool operator >(ulong a, Int128 b) => LessThan(ref b.v, a);
    public static bool operator >=(Int128 a, UInt128 b) => a.CompareTo(b) >= 0;
    public static bool operator >=(UInt128 a, Int128 b) => b.CompareTo(a) <= 0;
    public static bool operator >=(Int128 a, Int128 b) => LessThan(ref b.v, ref a.v) || a.Equals(b);
    public static bool operator >=(Int128 a, int b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator >=(int a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator >=(Int128 a, uint b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator >=(uint a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator >=(Int128 a, long b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator >=(long a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator >=(Int128 a, ulong b) => LessThan(b, ref a.v) || a.Equals(b);
    public static bool operator >=(ulong a, Int128 b) => LessThan(ref b.v, a) || a.Equals(b);
    public static bool operator ==(UInt128 a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, UInt128 b) => a.Equals(b);
    public static bool operator ==(Int128 a, Int128 b) => a.v.Equals(b.v);
    public static bool operator ==(Int128 a, int b) => a.Equals(b);
    public static bool operator ==(int a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, uint b) => a.Equals(b);
    public static bool operator ==(uint a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, long b) => a.Equals(b);
    public static bool operator ==(long a, Int128 b) => b.Equals(a);
    public static bool operator ==(Int128 a, ulong b) => a.Equals(b);
    public static bool operator ==(ulong a, Int128 b) => b.Equals(a);
    public static bool operator !=(UInt128 a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, UInt128 b) => !a.Equals(b);
    public static bool operator !=(Int128 a, Int128 b) => !a.v.Equals(b.v);
    public static bool operator !=(Int128 a, int b) => !a.Equals(b);
    public static bool operator !=(int a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, uint b) => !a.Equals(b);
    public static bool operator !=(uint a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, long b) => !a.Equals(b);
    public static bool operator !=(long a, Int128 b) => !b.Equals(a);
    public static bool operator !=(Int128 a, ulong b) => !a.Equals(b);
    public static bool operator !=(ulong a, Int128 b) => !b.Equals(a);
    public int CompareTo(UInt128 other)
    {
      if (IsNegative)
        return -1;
      return v.CompareTo(other);
    }

    public int CompareTo(Int128 other) => SignedCompare(ref v, other.S0, other.S1);
    public int CompareTo(int other) => SignedCompare(ref v, (ulong)other, (ulong)(other >> 31));
    public int CompareTo(uint other) => SignedCompare(ref v, (ulong)other, 0);
    public int CompareTo(long other) => SignedCompare(ref v, (ulong)other, (ulong)(other >> 63));
    public int CompareTo(ulong other) => SignedCompare(ref v, other, 0);
    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      if (!(obj is Int128))
        throw new ArgumentException();
      return CompareTo((Int128)obj);
    }

    private static bool LessThan(ref UInt128 a, ref UInt128 b)
    {
      var as1 = (long)a.S1;
      var bs1 = (long)b.S1;
      if (as1 != bs1)
        return as1 < bs1;
      return a.S0 < b.S0;
    }

    private static bool LessThan(ref UInt128 a, long b)
    {
      var as1 = (long)a.S1;
      var bs1 = b >> 63;
      if (as1 != bs1)
        return as1 < bs1;
      return a.S0 < (ulong)b;
    }

    private static bool LessThan(long a, ref UInt128 b)
    {
      var as1 = a >> 63;
      var bs1 = (long)b.S1;
      if (as1 != bs1)
        return as1 < bs1;
      return (ulong)a < b.S0;
    }

    private static bool LessThan(ref UInt128 a, ulong b)
    {
      var as1 = (long)a.S1;
      if (as1 != 0)
        return as1 < 0;
      return a.S0 < b;
    }

    private static bool LessThan(ulong a, ref UInt128 b)
    {
      var bs1 = (long)b.S1;
      if (0 != bs1)
        return 0 < bs1;
      return a < b.S0;
    }

    private static int SignedCompare(ref UInt128 a, ulong bs0, ulong bs1)
    {
      var as1 = a.S1;
      if (as1 != bs1)
        return ((long)as1).CompareTo((long)bs1);
      return a.S0.CompareTo(bs0);
    }

    public bool Equals(UInt128 other) => !IsNegative && v.Equals(other);
    public bool Equals(Int128 other) => v.Equals(other.v);
    public bool Equals(int other)
    {
      if (other < 0)
        return v.S1 == ulong.MaxValue && v.S0 == (uint)other;
      return v.S1 == 0 && v.S0 == (uint)other;
    }

    public bool Equals(uint other) => v.S1 == 0 && v.S0 == other;
    public bool Equals(long other)
    {
      if (other < 0)
        return v.S1 == ulong.MaxValue && v.S0 == (ulong)other;
      return v.S1 == 0 && v.S0 == (ulong)other;
    }

    public bool Equals(ulong other) => v.S1 == 0 && v.S0 == other;
    public override bool Equals(object obj)
    {
      if (!(obj is Int128))
        return false;
      return Equals((Int128)obj);
    }

    public override int GetHashCode() => v.GetHashCode();
    public static void Multiply(out Int128 c, ref Int128 a, int b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          UInt128.Multiply(out c.v, ref aneg, (uint)(-b));
        else
        {
          UInt128.Multiply(out c.v, ref aneg, (uint)b);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b < 0)
        {
          UInt128.Multiply(out c.v, ref a.v, (uint)(-b));
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Multiply(out c.v, ref a.v, (uint)b);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, uint b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Multiply(out c.v, ref aneg, b);
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Multiply(out c.v, ref a.v, b);
      Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, long b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          UInt128.Multiply(out c.v, ref aneg, (ulong)(-b));
        else
        {
          UInt128.Multiply(out c.v, ref aneg, (ulong)b);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b < 0)
        {
          UInt128.Multiply(out c.v, ref a.v, (ulong)(-b));
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Multiply(out c.v, ref a.v, (ulong)b);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, ulong b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Multiply(out c.v, ref aneg, b);
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Multiply(out c.v, ref a.v, b);
      Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Multiply(out Int128 c, ref Int128 a, ref Int128 b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Multiply(out c.v, ref aneg, ref bneg);
        }
        else
        {
          UInt128.Multiply(out c.v, ref aneg, ref b.v);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Multiply(out c.v, ref a.v, ref bneg);
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Multiply(out c.v, ref a.v, ref b.v);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a * (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, int b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          UInt128.Divide(out c.v, ref aneg, (uint)(-b));
        else
        {
          UInt128.Divide(out c.v, ref aneg, (uint)b);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b < 0)
        {
          UInt128.Divide(out c.v, ref a.v, (uint)(-b));
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Divide(out c.v, ref a.v, (uint)b);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, uint b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Divide(out c.v, ref aneg, b);
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Divide(out c.v, ref a.v, b);
      Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, long b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          UInt128.Divide(out c.v, ref aneg, (ulong)(-b));
        else
        {
          UInt128.Divide(out c.v, ref aneg, (ulong)b);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b < 0)
        {
          UInt128.Divide(out c.v, ref a.v, (ulong)(-b));
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Divide(out c.v, ref a.v, (ulong)b);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, ulong b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Divide(out c.v, ref aneg, b);
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Divide(out c.v, ref a.v, b);
      Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static void Divide(out Int128 c, ref Int128 a, ref Int128 b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Divide(out c.v, ref aneg, ref bneg);
        }
        else
        {
          UInt128.Divide(out c.v, ref aneg, ref b.v);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Divide(out c.v, ref a.v, ref bneg);
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Divide(out c.v, ref a.v, ref b.v);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a / (BigInteger)b);
    }

    public static int Remainder(ref Int128 a, int b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          return (int)UInt128.Remainder(ref aneg, (uint)(-b));
        else
          return -(int)UInt128.Remainder(ref aneg, (uint)b);
      }
      else
      {
        if (b < 0)
          return -(int)UInt128.Remainder(ref a.v, (uint)(-b));
        else
          return (int)UInt128.Remainder(ref a.v, (uint)b);
      }
    }

    public static int Remainder(ref Int128 a, uint b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        return -(int)UInt128.Remainder(ref aneg, b);
      }
      else
        return (int)UInt128.Remainder(ref a.v, b);
    }

    public static long Remainder(ref Int128 a, long b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b < 0)
          return (long)UInt128.Remainder(ref aneg, (ulong)(-b));
        else
          return -(long)UInt128.Remainder(ref aneg, (ulong)b);
      }
      else
      {
        if (b < 0)
          return -(long)UInt128.Remainder(ref a.v, (ulong)(-b));
        else
          return (long)UInt128.Remainder(ref a.v, (ulong)b);
      }
    }

    public static long Remainder(ref Int128 a, ulong b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        return -(long)UInt128.Remainder(ref aneg, b);
      }
      else
        return (long)UInt128.Remainder(ref a.v, b);
    }

    public static void Remainder(out Int128 c, ref Int128 a, ref Int128 b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Remainder(out c.v, ref aneg, ref bneg);
        }
        else
        {
          UInt128.Remainder(out c.v, ref aneg, ref b.v);
          UInt128.Negate(ref c.v);
        }
      }
      else
      {
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.Remainder(out c.v, ref a.v, ref bneg);
          UInt128.Negate(ref c.v);
        }
        else
          UInt128.Remainder(out c.v, ref a.v, ref b.v);
      }
      Debug.Assert((BigInteger)c == (BigInteger)a % (BigInteger)b);
    }

    public static Int128 Abs(Int128 a)
    {
      if (!a.IsNegative)
        return a;
      Int128 c;
      UInt128.Negate(out c.v, ref a.v);
      return c;
    }

    public static Int128 Square(long a)
    {
      if (a < 0)
        a = -a;
      Int128 c;
      UInt128.Square(out c.v, (ulong)a);
      return c;
    }

    public static Int128 Square(Int128 a)
    {
      Int128 c;
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Square(out c.v, ref aneg);
      }
      else
        UInt128.Square(out c.v, ref a.v);
      return c;
    }

    public static Int128 Cube(long a)
    {
      Int128 c;
      if (a < 0)
      {
        UInt128.Cube(out c.v, (ulong)(-a));
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Cube(out c.v, (ulong)a);
      return c;
    }

    public static Int128 Cube(Int128 a)
    {
      Int128 c;
      if (a < 0)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        UInt128.Cube(out c.v, ref aneg);
        UInt128.Negate(ref c.v);
      }
      else
        UInt128.Cube(out c.v, ref a.v);
      return c;
    }

    public static void Add(ref Int128 a, ref Int128 b) => UInt128.Add(ref a.v, ref b.v);
    public static void Add(ref Int128 a, Int128 b) => UInt128.Add(ref a.v, ref b.v);
    public static void Add(ref Int128 a, long b)
    {
      if (b < 0)
        UInt128.Subtract(ref a.v, (ulong)(-b));
      else
        UInt128.Add(ref a.v, (ulong)b);
    }

    public static void Subtract(ref Int128 a, long b)
    {
      if (b < 0)
        UInt128.Add(ref a.v, (ulong)(-b));
      else
        UInt128.Subtract(ref a.v, (ulong)b);
    }

    public static void Subtract(ref Int128 a, ref Int128 b) => UInt128.Subtract(ref a.v, ref b.v);
    public static void Subtract(ref Int128 a, Int128 b) => UInt128.Subtract(ref a.v, ref b.v);

    public static void AddProduct(ref Int128 a, ref UInt128 b, int c)
    {
      UInt128 product;
      if (c < 0)
      {
        UInt128.Multiply(out product, ref b, (uint)(-c));
        UInt128.Subtract(ref a.v, ref product);
      }
      else
      {
        UInt128.Multiply(out product, ref b, (uint)c);
        UInt128.Add(ref a.v, ref product);
      }
    }

    public static void AddProduct(ref Int128 a, ref UInt128 b, long c)
    {
      UInt128 product;
      if (c < 0)
      {
        UInt128.Multiply(out product, ref b, (ulong)(-c));
        UInt128.Subtract(ref a.v, ref product);
      }
      else
      {
        UInt128.Multiply(out product, ref b, (ulong)c);
        UInt128.Add(ref a.v, ref product);
      }
    }

    public static void SubtractProduct(ref Int128 a, ref UInt128 b, int c)
    {
      UInt128 d;
      if (c < 0)
      {
        UInt128.Multiply(out d, ref b, (uint)(-c));
        UInt128.Add(ref a.v, ref d);
      }
      else
      {
        UInt128.Multiply(out d, ref b, (uint)c);
        UInt128.Subtract(ref a.v, ref d);
      }
    }

    public static void SubtractProduct(ref Int128 a, ref UInt128 b, long c)
    {
      UInt128 d;
      if (c < 0)
      {
        UInt128.Multiply(out d, ref b, (ulong)(-c));
        UInt128.Add(ref a.v, ref d);
      }
      else
      {
        UInt128.Multiply(out d, ref b, (ulong)c);
        UInt128.Subtract(ref a.v, ref d);
      }
    }

    public static void AddProduct(ref Int128 a, UInt128 b, int c) => AddProduct(ref a, ref b, c);
    public static void AddProduct(ref Int128 a, UInt128 b, long c) => AddProduct(ref a, ref b, c);
    public static void SubtractProduct(ref Int128 a, UInt128 b, int c) => SubtractProduct(ref a, ref b, c);
    public static void SubtractProduct(ref Int128 a, UInt128 b, long c) => SubtractProduct(ref a, ref b, c);

    public static void Pow(out Int128 result, ref Int128 value, int exponent)
    {
      if (exponent < 0)
        throw new ArgumentException("exponent must not be negative");
      if (value.IsNegative)
      {
        UInt128 valueneg;
        UInt128.Negate(out valueneg, ref value.v);
        if ((exponent & 1) == 0)
          UInt128.Pow(out result.v, ref valueneg, (uint)exponent);
        else
        {
          UInt128.Pow(out result.v, ref valueneg, (uint)exponent);
          UInt128.Negate(ref result.v);
        }
      }
      else
        UInt128.Pow(out result.v, ref value.v, (uint)exponent);
    }

    public static Int128 Pow(Int128 value, int exponent)
    {
      Int128 result;
      Pow(out result, ref value, exponent);
      return result;
    }

    public static ulong FloorSqrt(Int128 a)
    {
      if (a.IsNegative)
        throw new ArgumentException("argument must not be negative");
      return UInt128.FloorSqrt(a.v);
    }

    public static ulong CeilingSqrt(Int128 a)
    {
      if (a.IsNegative)
        throw new ArgumentException("argument must not be negative");
      return UInt128.CeilingSqrt(a.v);
    }

    public static long FloorCbrt(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        return -(long)UInt128.FloorCbrt(aneg);
      }
      return (long)UInt128.FloorCbrt(a.v);
    }

    public static long CeilingCbrt(Int128 a)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        return -(long)UInt128.CeilingCbrt(aneg);
      }
      return (long)UInt128.CeilingCbrt(a.v);
    }

    public static Int128 Min(Int128 a, Int128 b)
    {
      if (LessThan(ref a.v, ref b.v))
        return a;
      return b;
    }

    public static Int128 Max(Int128 a, Int128 b)
    {
      if (LessThan(ref b.v, ref a.v))
        return a;
      return b;
    }

    public static double Log(Int128 a) => Log(a, Math.E);
    public static double Log10(Int128 a) => Log(a, 10);

    public static double Log(Int128 a, double b)
    {
      if (a.IsNegative || a.IsZero)
        throw new ArgumentException("argument must be positive");
      return Math.Log(UInt128.ConvertToDouble(ref a.v), b);
    }

    public static Int128 Add(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.Add(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 Subtract(Int128 a, Int128 b)
    {
      Int128 c;
      UInt128.Subtract(out c.v, ref a.v, ref b.v);
      return c;
    }

    public static Int128 Multiply(Int128 a, Int128 b)
    {
      Int128 c;
      Multiply(out c, ref a, ref b);
      return c;
    }

    public static Int128 Divide(Int128 a, Int128 b)
    {
      Int128 c;
      Divide(out c, ref a, ref b);
      return c;
    }

    public static Int128 Remainder(Int128 a, Int128 b)
    {
      Int128 c;
      Remainder(out c, ref a, ref b);
      return c;
    }

    public static Int128 DivRem(Int128 a, Int128 b, out Int128 remainder)
    {
      Int128 c;
      Divide(out c, ref a, ref b);
      Remainder(out remainder, ref a, ref b);
      return c;
    }

    public static Int128 Negate(Int128 a)
    {
      Int128 c;
      UInt128.Negate(out c.v, ref a.v);
      return c;
    }

    public static Int128 GreatestCommonDivisor(Int128 a, Int128 b)
    {
      Int128 c;
      GreatestCommonDivisor(out c, ref a, ref b);
      return c;
    }

    public static void GreatestCommonDivisor(out Int128 c, ref Int128 a, ref Int128 b)
    {
      if (a.IsNegative)
      {
        UInt128 aneg;
        UInt128.Negate(out aneg, ref a.v);
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.GreatestCommonDivisor(out c.v, ref aneg, ref bneg);
        }
        else
          UInt128.GreatestCommonDivisor(out c.v, ref aneg, ref b.v);
      }
      else
      {
        if (b.IsNegative)
        {
          UInt128 bneg;
          UInt128.Negate(out bneg, ref b.v);
          UInt128.GreatestCommonDivisor(out c.v, ref a.v, ref bneg);
        }
        else
          UInt128.GreatestCommonDivisor(out c.v, ref a.v, ref b.v);
      }
    }

    public static void LeftShift(ref Int128 c, int d) => UInt128.LeftShift(ref c.v, d);
    public static void LeftShift(ref Int128 c) => UInt128.LeftShift(ref c.v);
    public static void RightShift(ref Int128 c, int d) => UInt128.ArithmeticRightShift(ref c.v, d);
    public static void RightShift(ref Int128 c) => UInt128.ArithmeticRightShift(ref c.v);
    public static void Swap(ref Int128 a, ref Int128 b) => UInt128.Swap(ref a.v, ref b.v);
    public static int Compare(Int128 a, Int128 b) => a.CompareTo(b);
    public static void Shift(out Int128 c, ref Int128 a, int d) => UInt128.ArithmeticShift(out c.v, ref a.v, d);
    public static void Shift(ref Int128 c, int d) => UInt128.ArithmeticShift(ref c.v, d);

    public static Int128 ModAdd(Int128 a, Int128 b, Int128 modulus)
    {
      Int128 c;
      UInt128.ModAdd(out c.v, ref a.v, ref b.v, ref modulus.v);
      return c;
    }

    public static Int128 ModSub(Int128 a, Int128 b, Int128 modulus)
    {
      Int128 c;
      UInt128.ModSub(out c.v, ref a.v, ref b.v, ref modulus.v);
      return c;
    }

    public static Int128 ModMul(Int128 a, Int128 b, Int128 modulus)
    {
      Int128 c;
      UInt128.ModMul(out c.v, ref a.v, ref b.v, ref modulus.v);
      return c;
    }

    public static Int128 ModPow(Int128 value, Int128 exponent, Int128 modulus)
    {
      Int128 result;
      UInt128.ModPow(out result.v, ref value.v, ref exponent.v, ref modulus.v);
      return result;
    }

    public byte[] ToBytes()
    {
      var bytes = new byte[16];
      ToBytes(bytes);
      return bytes;
    }

    public void ToBytes(byte[] array)
    {
      var s0 = BitConverter.GetBytes(v.S0);
      var s1 = BitConverter.GetBytes(v.S1);
      Buffer.BlockCopy(s0, 0, array, 8, 8);
      Buffer.BlockCopy(s1, 0, array, 0, 8);
    }

    public static Int128 Convert(object a) => TryConvert(a, out var i) ? i : throw new FormatException();

    public static bool TryConvert(object a, out Int128 i)
    {
      if (a is Int128 i128)
        i = i128;
      else if (a is SByte i8)
        i = (Int128)i8;
      else if (a is Int16 i16)
        i = (Int128)i16;
      else if (a is Int32 i32)
        i = (Int128)i32;
      else if (a is Int64 i64)
        i = (Int128)i64;
      else if (a is Byte ui8)
        i = (Int128)ui8;
      else if (a is UInt16 ui16)
        i = (Int128)ui16;
      else if (a is UInt32 ui32)
        i = (Int128)ui32;
      else if (a is UInt64 ui64)
        i = (Int128)ui64;
      else if (a is Single f32)
        i = (Int128)f32;
      else if (a is Double f64)
        i = (Int128)f64;
      else if (a is BigInteger bi)
        i = (Int128)bi;
      else
      {
        i = 0;
        return false;
      }
      return true;
    }
  }
}
