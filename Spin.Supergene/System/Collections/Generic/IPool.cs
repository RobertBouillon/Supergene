using System;
namespace System.Collections.Generic;

public interface IPool<T>
{
  T Allocate();
  IList<T> Allocated { get; }
  int AmountAllocated { get; }
  void Compact();
  int Count { get; }
  void Deallocate(T o);
  int MaxCount { get; }
  int NumberAvailable { get; }
}
