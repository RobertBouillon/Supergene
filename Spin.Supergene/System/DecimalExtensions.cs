using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

public static class DecimalExtensions
{
  public static decimal SafeDivide(this decimal a, decimal b) => (b == 0) ? 0 : a / b;
}
