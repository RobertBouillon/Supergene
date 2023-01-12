using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel;

public static class CancelEventArgsExtensions
{
  public static bool Invoke(this CancelEventArgs args, Action<CancelEventArgs> method)
  {
    method(args);
    return !args.Cancel;
  }
}
