using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace System
{
  public struct UInt256
  {
    public ulong s0;
    public ulong s1;
    public ulong s2;
    public ulong s3;

    public uint R0 => (uint)s0;
    public uint R1 => (uint)(s0 >> 32);
    public uint R2 => (uint)s1;
    public uint R3 => (uint)(s1 >> 32);
    public uint R4 => (uint)s2;
    public uint R5 => (uint)(s2 >> 32);
    public uint R6 => (uint)s3;
    public uint R7 => (uint)(s3 >> 32);

    public UInt128 t0 { get { UInt128 result; UInt128.Create(out result, s0, s1); return result; } }
    public UInt128 t1 { get { UInt128 result; UInt128.Create(out result, s2, s3); return result; } }

    public static implicit operator BigInteger(UInt256 a) => (BigInteger)a.s3 << 192 | (BigInteger)a.s2 << 128 | (BigInteger)a.s1 << 64 | a.s0;
    public override string ToString() => ((BigInteger)this).ToString();
  }
}
