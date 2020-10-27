using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public struct SmallMoney
  {
    private short _value;

    public SmallMoney(int value)
    {
      _value = unchecked((short)value);
    }

    public SmallMoney(float value)
    {
      _value = unchecked((short)(value * 1000));
    }

    public SmallMoney(double value)
    {
      _value = unchecked((short)(value * 1000));
    }

    public SmallMoney(decimal value)
    {
      _value = unchecked((short)(value * 1000));
    }
  }
}
