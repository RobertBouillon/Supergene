using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System;

public struct BigMoney
{
  private long _value;

  public BigMoney(int value)
  {
    _value = value;
  }

  public BigMoney(float value)
  {
    _value = unchecked((int)(value * 1000));
  }

  public BigMoney(double value)
  {
    _value = unchecked((int)(value * 1000));
  }

  public BigMoney(decimal value)
  {
    _value = unchecked((int)(value * 1000));
  }
}
