using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public struct Money
  {
    private int _value;

    public Money(int value)
    {
      _value = value;
    }

    public Money(float value)
    {
      _value = unchecked((int)(value * 1000));
    }

    public Money(double value)
    {
      _value = unchecked((int)(value * 1000));
    }

    public Money(decimal value)
    {
      _value = unchecked((int)(value * 1000));
    }
  }
}
