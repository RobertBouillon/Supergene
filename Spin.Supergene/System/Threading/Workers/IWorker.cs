using System;
namespace System.Threading.Workers
{
  public interface IWorker
  {
    event EventHandler<System.Threading.Worker.ErrorEventArgs> Error;
    bool IsWorking { get; }
    bool IsStarted { get; }
    bool IsStopping { get; }
    string Name { get; }
    event System.ComponentModel.CancelEventHandler Working;
    bool Start();
    event EventHandler Started;
    event System.ComponentModel.CancelEventHandler Starting;
    bool Stop();
    bool Stop(TimeSpan timeout, bool kill);
    event EventHandler Stopped;
    event System.ComponentModel.CancelEventHandler Stopping;
    event EventHandler<System.Threading.Worker.WorkPerformedEventArgs> Worked;
  }
}
