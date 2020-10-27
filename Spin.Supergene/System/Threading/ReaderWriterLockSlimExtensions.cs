using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading
{
  public static class ReaderWriterLockSlimExtensions
  {
    public static void Read(this ReaderWriterLockSlim handle, Action action)
    {
      try
      {
        handle.EnterReadLock();
        action();
      }
      finally
      {
        handle.ExitReadLock();
      }
    }

    public static void Write(this ReaderWriterLockSlim handle, Action action)
    {
      try
      {
        handle.EnterWriteLock();
        action();
      }
      finally
      {
        handle.ExitWriteLock();
      }
    }
  }
}
