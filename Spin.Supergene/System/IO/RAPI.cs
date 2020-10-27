using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;


namespace System.IO
{
	/// <summary>
	/// RAPI Wrapper
	/// </summary>
  public class RAPI
  {
    #region Structs
    /// <summary>
    /// This structure contains operating system version information. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefceosversioninfo.asp</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct OSVERSIONINFO
    {
      /// <summary>
      /// Specifies the size, in bytes, of this data structure. 
      /// </summary>
      public int dwOSVersionInfoSize;

      /// <summary>
      /// Identifies the major version number of the operating system.
      /// </summary>
      public int dwMajorVersion;

      /// <summary>
      /// Identifies the minor version number of the operating system.
      /// </summary>
      
      /// <summary>
      /// Identifies the build number of the operating system or is set to 0.
      /// </summary>
      public int dwMinorVersion;

      /// <summary>
      /// UNDOCUMENTED
      /// </summary>
      public int dwBuildNumber;

      /// <summary>
      /// Identifies the operating system platform.
      /// </summary>
      public Microsoft.WinCE.CeBase.PlatformId dwPlatformId;

      /// <summary>
      /// Null-terminated string that provides arbitrary additional information about the operating system. 
      /// </summary>
      [MarshalAs(UnmanagedType.ByValTStr,SizeConst= 128)]
      public string szCSDVersion;
    }

    /// <summary>
    /// This structure contains information used to initialize a RAPI connection.
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefrapiinit.asp</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAPIINIT
    {
      /// <summary>
      /// Specifies the size of the RAPIINIT structure being passed in. 
      /// </summary>
      public int cbsize;

      /// <summary>
      /// Specifies a handle to an event. The event is set when the RAPI connection is made or an error occurs. 
      /// </summary>
      public IntPtr heRapiInit;

      /// <summary>
      /// Specifies the results, success or failure, of the RAPI connection. 
      /// </summary>
      public int hrRapiInit;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
      public int nLength;
      public int lpSecurityDescriptor;
      public IntPtr bInheritHandle;
    }
    #endregion
    #region Imports
    /// <summary>
    /// This function closes an open object handle. 
    /// </summary>
    /// <param name="hObject">Handle to an open object.</param>
    /// <returns>Nonzero indicates success. Zero indicates failure. To get extended error information, call CeGetLastError.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefceclosehandlerapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeCloseHandle(IntPtr hObject);

    /// <summary>
    /// This function creates, opens, or truncates a file, pipe, communications resource, disk device, or console. It returns a handle that can be used to access the object. It can also open and return a handle to a directory. 
    /// </summary>
    /// <param name="lpFileName">Pointer to a null-terminated string that specifies the name of the object, such as file, COM port, disk device, or console, to create or open.</param>
    /// <param name="dwDesiredAccess">Specifies the type of access to the object.</param>
    /// <param name="dwShareMode">Specifies how the object can be shared. If dwShareMode is 0, the object cannot be shared. Subsequent open operations on the object will fail, until the handle is closed.</param>
    /// <param name="lpSecurityAttributes">Ignored; set to NULL</param>
    /// <param name="dwCreationDisposition">Specifies which action to take on files that exist, and which action to take when files do not exist.</param>
    /// <param name="dwFlagsAndAttributes">Specifies the file attributes and flags for the file.</param>
    /// <param name="hTemplateFile">Ignored; as a result, CeCreateFile does not copy the extended attributes to the new file.</param>
    /// <returns>An open handle to the specified file indicates success.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/wceactsy/html/cerefCeCreateFileRAPI.asp?frame=true</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern IntPtr CeCreateFile(string lpFileName, DesiredAccess
      dwDesiredAccess,ShareMode dwShareMode,int lpSecurityAttributes,CreationDispostion
      dwCreationDisposition,int dwFlagsAndAttributes,int hTemplateFile);

    /// <summary>
    /// This function attempts to initialize the Windows CE remote application programming interface (RAPI) and initially returns an event handle.
    /// </summary>
    /// <param name="pRapiInit">Pointer to a RAPIINIT structure. The cbSize member of this structure contains the size of the structure being passed in.</param>
    /// <returns>_OK indicates success. E_FAIL indicates failure. E_INVALIDARG indicates that an invalid value was encountered.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcerapiinitex.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeRapiInitEx ([MarshalAs(UnmanagedType.Struct)] ref
      RAPIINIT pRapiInit);

    /// <summary>
    /// This function attempts to initialize the Windows CE remote application programming interface (RAPI). 
    /// </summary>
    /// <returns>S_OK indicates success. E_FAIL indicates failure.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcerapiinit.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeRapiInit ();

    /// <summary>
    /// This function uninitializes the Windows CE remote application programming interface (RAPI). 
    /// </summary>
    /// <returns>_FAIL indicates that RAPI was not originally initialized. </returns>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeRapiUninit ();

    /// <summary>
    /// This function reads data from a file, starting at the position indicated by the file pointer. After the read operation has been completed, the file pointer is adjusted by the number of bytes actually read. 
    /// </summary>
    /// <param name="hFile">Handle to the file to be read.</param>
    /// <param name="lpBuffer">Pointer to the buffer that receives the data read from the file.</param>
    /// <param name="nNumberOfbytesToRead">Number of bytes to be read from the file.</param>
    /// <param name="lpNumberOfbytesRead">Pointer to the number of bytes read. CeReadFile sets this value to 0 before doing any work or error checking.</param>
    /// <param name="lpOverlapped">Unsupported; set to NULL.</param>
    /// <returns>Nonzero indicates success. The CeReadFile function returns when one of the following is true: the number of bytes requested has been read or an error occurs.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcereadfilerapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeReadFile(IntPtr hFile,byte[] lpBuffer,
      int nNumberOfbytesToRead, out int lpNumberOfbytesRead, int lpOverlapped);

    /// <summary>
    /// This function writes data to a file.
    /// </summary>
    /// <param name="hFile">Handle to the file to be written to.</param>
    /// <param name="lpBuffer">Pointer to the buffer containing the data to be written to the file.</param>
    /// <param name="nNumberOfbytesToWrite">Number of bytes to write to the file.</param>
    /// <param name="lpNumberOfbytesWritten">Pointer to the number of bytes written by this function call.</param>
    /// <param name="lpOverlapped">Unsupported; set to NULL. </param>
    /// <returns>Nonzero indicates success. Zero indicates failure.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcewritefilerapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeWriteFile(IntPtr hFile,byte[] lpBuffer, int
      nNumberOfbytesToWrite, out int lpNumberOfbytesWritten, int lpOverlapped);

    /// <summary>
    /// This function moves the end-of-file position for the specified file to the current position of the file pointer.
    /// </summary>
    /// <param name="hFile">Handle to the file to have its EOF position moved.</param>
    /// <returns>Nonzero indicates success. Zero indicates failure.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcesetendoffilerapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeSetEndOfFile(IntPtr hFile);

    /// <summary>
    /// This function retrieves the size, in bytes, of the specified file. 
    /// </summary>
    /// <param name="hFile">Open handle of the file whose size is being returned.</param>
    /// <param name="lpFileSizeHigh">Pointer to the variable where the high-order word of the file size is returned.</param>
    /// <returns>If lpFileSizeHigh is non-NULL, 0xFFFFFFFF indicates failure and CeGetLastError returns a value other than NO_ERROR.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/wceactsy/html/cerefCeGetFileSizeRAPI.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeGetFileSize(IntPtr hFile,int lpFileSizeHigh);


    /// <summary>
    /// This function moves the file pointer of an open file. 
    /// </summary>
    /// <param name="hFile">Handle to the file whose file pointer is to be moved.</param>
    /// <param name="lDistanceToMove">Low-order 32 bits of a signed value that specifies the number of bytes to move the file pointer.</param>
    /// <param name="lpDistanceToMoveHigh">Pointer to the high-order 32 bits of the signed 64-bit distance to move.</param>
    /// <param name="dwMoveMethod">Specifies the starting point for the file pointer move.</param>
    /// <returns>The low-order DWORD of the new file pointer indicates success and that lpDistanceToMoveHigh is NULL.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcesetfilepointerrapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern uint CeSetFilePointer(IntPtr hFile, int lDistanceToMove, int lpDistanceToMoveHigh, int dwMoveMethod);

    /// <summary>
    /// This function returns the calling thread's last-error code value. 
    /// </summary>
    /// <returns>The calling thread's last-error code value indicates success.</returns>
    /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wceactsy/html/cerefcegetlasterrorrapi.asp</remarks>
    [DllImport("rapi.dll", CharSet=CharSet.Unicode)]
    public static extern int CeGetLastError();
    #endregion
  }
}
