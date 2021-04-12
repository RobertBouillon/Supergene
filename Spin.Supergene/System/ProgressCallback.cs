using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public delegate void ProgressCallback<T>(string status, T current, T total);
}
