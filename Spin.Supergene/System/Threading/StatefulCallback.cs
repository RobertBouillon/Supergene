using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading
{
  public delegate void StatefulCallback<T>(AsyncOperation op, T state);

}
