using System;

namespace System.Data
{
  public enum ConflictResolution : int
  {
    CannotResolve = 0,
    Add = 1,
    Change = 2,
    Remove = 3
  }
}
