using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices;

/// <summary>
/// A Custom Marshaller for BCD Data Types
/// </summary>
public class BcdMarshal : ICustomMarshaler
{
  #region Constructors
  public BcdMarshal()
  {
    //
    // TODO: Add constructor logic here
    //
  }
  #endregion
  #region ICustomMarshaler Members

  public object MarshalNativeToManaged(System.IntPtr pNativeData)
  {

    return null;
  }

  public System.IntPtr MarshalManagedToNative(object ManagedObj)
  {
    // TODO:  Add BcdMarshal.MarshalManagedToNative implementation
    return new System.IntPtr();
  }

  public void CleanUpManagedData(object ManagedObj)
  {
    // TODO:  Add BcdMarshal.CleanUpManagedData implementation
  }

  public int GetNativeDataSize()
  {
    // TODO:  Add BcdMarshal.GetNativeDataSize implementation
    return 0;
  }

  public void CleanUpNativeData(System.IntPtr pNativeData)
  {
    // TODO:  Add BcdMarshal.CleanUpNativeData implementation
  }

  #endregion
}
