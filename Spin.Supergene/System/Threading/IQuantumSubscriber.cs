using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading;

public interface IQuantumSubscriber
{
  void Monitor();
  void MonitorHeartbeat(TimeSpan interval);
}
