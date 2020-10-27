using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public class GenericObject<T1>
  {
    public T1 O1 { get; set; }
    public GenericObject() { }
  }

  public class GenericObject<T1, T2>
  {
    public T1 O1 { get; set; }
    public T2 O2 { get; set; }
    public GenericObject() { }
  }

  public class GenericObject<T1, T2, T3>
  {
    public T1 O1 { get; set; }
    public T2 O2 { get; set; }
    public T3 O3 { get; set; }
    public GenericObject() { }
  }

  public class GenericObject<T1, T2, T3, T4>
  {
    public T1 O1 { get; set; }
    public T2 O2 { get; set; }
    public T3 O3 { get; set; }
    public T4 O4 { get; set; }
    public GenericObject() { }
  }

  public class GenericObject<T1, T2, T3, T4, T5>
  {
    public T1 O1 { get; set; }
    public T2 O2 { get; set; }
    public T3 O3 { get; set; }
    public T4 O4 { get; set; }
    public T5 O5 { get; set; }
    public GenericObject() { }
  }

}
