using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
  /// <summary>
  /// Summary description for ActiveSync.
  /// </summary>
  public class ActiveSync : IDisposable 
  {
    private bool p_IsConnected;  //Stores whether a connection has been initialized with the device

    public bool IsConnected
    {
      get{return p_IsConnected;}
    }

    public static CeFileStream GetFile( string RemotePath , FileAccess access)
    {
      return new CeFileStream(RemotePath,FileMode.Open, access);
    }

    public bool Connect()
    {
      RAPIINIT ri = new RAPIINIT();
      ri.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(ri);
      uint hRes = CeRapiInitEx(ref ri);

      ManualResetEvent me = new ManualResetEvent(false);
      me.Handle = ri.heRapiInit;

      if (!me.WaitOne(15, true))
      {
        CeRapiUninit();
        p_IsConnected = false;
        return false;
      }
      else
      {
        p_IsConnected = true;
        return true;
      }
    }

    public void UninitDevice()
    {
      CeRapiUninit();
    }
    
    #region IDisposable Members

    public void Dispose()
    {
      if(p_IsConnected)
        UninitDevice();
    }

    #endregion
    #region P/Invoke Declarations
    public enum RegistryHive : uint
    {
      HKEY_CLASSES_ROOT = 0x80000000,
      HKEY_CURRENT_USER = 0x80000001,
      HKEY_LOCAL_MACHINE = 0x80000002
    }

    public enum RegistryValueType : int
    {
      REG_DWORD = 4, // 32-bit number
      REG_SZ = 1, // Unicode null terminated string
      REG_MULTI_SZ = 7 // Multiple Unicode strings
    }

    [StructLayout(LayoutKind.Explicit)]
      private struct RAPIINIT
    {
      [FieldOffset(0)] public int cbsize;
      [FieldOffset(4)] public System.IntPtr heRapiInit;
      [FieldOffset(8)] public System.IntPtr hrRapiInit;
    };

    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    private static extern int CeCreateFile( string lpFileName, uint
      dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint
      dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    private static extern uint CeWriteFile( int hFile, byte[] lpBuffer, uint
      nNumberOfBytesToWrite, ref uint nNumberOfBytesWritten, uint lpOverlapped);

    [DllImport("rapi.dll")]
    private static extern bool CeCloseHandle( int hObject );

    [DllImport("rapi.dll")]
    private static extern uint CeRapiUninit();

    [DllImport("rapi.dll")]
    private static extern uint CeRapiInit();

    [DllImport("rapi.dll")]
    private static extern uint CeRapiInitEx(ref RAPIINIT pRapiInit);

    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    private static extern uint CeRegOpenKeyEx(uint HKEY, string lpSubKey, int
      ulOptions, int samDesired, out uint phkResult);

    /* CeRegQueryValueEx variants */
    [DllImport("rapi.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
    private static extern uint CeRegQueryValueEx(uint hKey, string lpValueName,
      int lpReserved, ref int lpType, int pValue, ref int lpchData);

    [DllImport("rapi.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
    private static extern uint CeRegQueryValueEx(uint hKey, string lpValueName,
      int lpReserved, ref int lpType, StringBuilder pValue, ref int lpchData);

    [DllImport("rapi.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
    private static extern uint CeRegQueryValueEx(uint hKey, string lpValueName,
      int lpReserved, ref int lpType, ref uint pValue, ref int lpchData);

    /* CeRegSetValueEx variants */
    [DllImport("rapi.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
    private static extern uint CeRegSetValueEx(uint hKey, string lpValueName,
      int lpReserved, int dwType, ref uint lpData, int cbData);

    [DllImport("rapi.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
    private static extern uint CeRegSetValueEx(uint hKey, string lpValueName,
      int lpReserved, int dwType, StringBuilder lpData, int cbData);

    [DllImport("rapi.dll")]
    private static extern uint CeRegCloseKey(uint hkey);

    [DllImport("kernel32.dll")]
    private static extern uint WaitForSingleObject(int hHandle, int timeout);
    #endregion
  }
}


