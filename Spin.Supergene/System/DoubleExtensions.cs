using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System;

public static class DoubleExtensions
{
  public static double PercentOf(this double a, double b) => (a / b) * 100;
  public static double PercentOf(this double a, double b, int round) => Math.Round((a / b) * 100, round);
  public static double SafeDivide(this double a, double b) => (b == 0) ? 0 : a / b;
}
