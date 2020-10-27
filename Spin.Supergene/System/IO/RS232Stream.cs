using System;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace System.IO
{
  #region Enumerations

  /// <summary>
  /// Parity settings
  /// </summary>
  public enum Rs232Parity 
  {
    /// <summary>
    /// Characters do not have a parity bit.
    /// </summary>
    None = 0,
    /// <summary>
    /// If there are an odd number of 1s in the data bits, the parity bit is 1.
    /// </summary>
    Odd = 1,
    /// <summary>
    /// If there are an even number of 1s in the data bits, the parity bit is 1.
    /// </summary>
    Even = 2,
    /// <summary>
    /// The parity bit is always 1.
    /// </summary>
    Mark = 3,
    /// <summary>
    /// The parity bit is always 0.
    /// </summary>
    Space = 4
  };

  /// <summary>
  /// Stop bit settings
  /// </summary>
  public enum Rs232StopBits
  {
    /// <summary>
    /// Line is asserted for 1 bit duration at end of each character
    /// </summary>
    One = 0,
    /// <summary>
    /// Line is asserted for 1.5 bit duration at end of each character
    /// </summary>
    OnePointFive = 1,
    /// <summary>
    /// Line is asserted for 2 bit duration at end of each character
    /// </summary>
    Two = 2
  };

  /// <summary>
  /// Uses for RTS or DTR pins
  /// </summary>
  public enum Rs232HandshakeOutput : uint
  {
    /// <summary>
    /// Pin is asserted when this station is able to receive data.
    /// </summary>
    Handshake = 2,
    /// <summary>
    /// Pin is asserted when this station is transmitting data (RTS on NT, 2000 or XP only).
    /// </summary>
    Gate = 3,
    /// <summary>
    /// Pin is asserted when this station is online (port is open).
    /// </summary>
    Online = 1,
    /// <summary>
    /// Pin is never asserted.
    /// </summary>
    None = 0
  };

  /// <summary>
  /// Standard handshake methods
  /// </summary>
  public enum Rs232Handshake
  {
    /// <summary>
    /// No handshaking
    /// </summary>
    None,
    /// <summary>
    /// Software handshaking using Xon / Xoff
    /// </summary>
    XOnXOff,
    /// <summary>
    /// Hardware handshaking using CTS / RTS
    /// </summary>
    CtsRts,
    /// <summary>
    /// Hardware handshaking using DSR / DTR
    /// </summary>
    DsrDtr
  }

  #endregion
	/// <summary>
	/// Summary description for RS232Stream.
	/// </summary>
  public class Rs232Stream : Stream, IDisposable
  {
    #region Private Variables
    private IntPtr hPort;
    #endregion
    #region Private Property Declarations
    private string p_Port;
    private int p_BaudRate;
    private Rs232Parity p_Parity;
    private int p_DataBits = 8;
    private Rs232StopBits p_StopBits = Rs232StopBits.One;
    private bool p_TxFlowCTS = false;
    private bool p_TxFlowDSR = false;
    private bool p_TxFlowX = false;
    private bool p_TxWhenRxXOff = true;
    private bool p_RxGateDSR = false;
    private bool p_RxFlowX = false;
    private Rs232HandshakeOutput p_UseRTS = Rs232HandshakeOutput.None;
    private Rs232HandshakeOutput p_UseDTR = Rs232HandshakeOutput.None;
    private ASCII p_XOnChar = ASCII.DC1;
    private ASCII p_XOffChar = ASCII.DC3;
    private int p_RxHighWater = 0;
    private int p_RxLowWater = 0;
    private uint p_SendTimeoutMultiplier = 0;
    private uint p_SendTimeoutConstant = 0;
    private int p_RxQueue = 0;
    private int p_TxQueue = 0;
    private bool p_AutoReopen = false;
    private bool p_CheckAllSends = true;
    private bool p_IsOpen;
    private Rs232Stream.Rs232Status p_Status;
    #endregion
    #region Public Property Declarations
    #region Inherited
    public override bool CanRead
    {
      get{return true;}
    }

    public override bool CanSeek
    {
      get{return false;}
    }

    public override bool CanWrite
    {
      get{return true;}
    }

    public override long Length
    {
      get{throw new NotSupportedException();}
    }

    public override long Position
    {
      get{throw new NotSupportedException();}
      set{throw new NotSupportedException();}
    }
    #endregion
    public Rs232Stream.Rs232Status Status
    {
      get{return p_Status;}
    }

    /// <summary>
    /// Port Name
    /// </summary>
    public string Port
    {
      get{return p_Port;}
      set{p_Port = value;}
    }

    /// <summary>
    /// Baud Rate
    /// </summary>
    public int BaudRate
    {
      get{return p_BaudRate;}
      set{p_BaudRate = value;}
    }

    /// <summary>
    /// The parity checking scheme (default: None)
    /// </summary>
    public Rs232Parity Parity
    {
      get{return p_Parity;}
      set{p_Parity = value;}
    }

    /// <summary>
    /// Number of databits 1..8 (default: 8)
    /// </summary>
    public int DataBits 
    {
      get{return p_DataBits;}
      set
      {
        if(value < 1 || value > 8)
          throw new ArgumentException("Data bits must be between 1 and 8");
        p_DataBits = value;
      }
    }

    /// <summary>
    /// Number of Stop Bits (default: One)
    /// </summary>
    public Rs232StopBits StopBits
    {
      get{return p_StopBits;}
      set{p_StopBits = value;}
    }

    /// <summary>
    /// If true, transmission is halted unless CTS is asserted by the remote station (default: false)
    /// </summary>
    public bool TxFlowCTS
    {
      get{return p_TxFlowCTS;}
      set{TxFlowCTS = value;}
    }

    /// <summary>
    /// If true, transmission is halted unless DSR is asserted by the remote station (default: false)
    /// </summary>
    public bool TxFlowDSR
    {
      get{return p_TxFlowDSR;}
      set{p_TxFlowDSR = value;}
    }

    /// <summary>
    /// If true, transmission is halted when Xoff is received and restarted when Xon is received (default: false)
    /// </summary>
    public bool TxFlowX
    {
      get{return p_TxFlowX;}
      set{p_TxFlowX = value;}
    }

    /// <summary>
    /// If false, transmission is suspended when this station has sent Xoff to the remote station (default: true)
    /// Set false if the remote station treats any character as an Xon.
    /// </summary>
    public bool TxWhenRxXOff
    {
      get{return p_TxWhenRxXOff;}
      set{p_TxWhenRxXOff = value;}
    }

    /// <summary>
    /// If true, received characters are ignored unless DSR is asserted by the remote station (default: false)
    /// </summary>
    public bool RxGateDSR
    {
      get{return p_RxGateDSR;}
      set{p_RxGateDSR = value;}
    }

    /// <summary>
    /// If true, Xon and Xoff characters are sent to control the data flow from the remote station (default: false)
    /// </summary>
    public bool RxFlowX
    {
      get{return p_RxFlowX;}
      set{p_RxFlowX = value;}
    }

    /// <summary>
    /// Specifies the use to which the RTS output is put (default: none)
    /// </summary>
    public Rs232HandshakeOutput UseRTS
    {
      get{return p_UseRTS;}
      set{p_UseRTS = value;}
    }

    /// <summary>
    /// Specidies the use to which the DTR output is put (default: none)
    /// </summary>
    public Rs232HandshakeOutput UseDTR
    {
      get{return p_UseDTR;}
      set{p_UseDTR = value;}
    }

    /// <summary>
    /// The character used to signal Xon for X flow control (default: DC1)
    /// </summary>
    public ASCII XOnChar
    {
      get{return p_XOnChar;}
      set{p_XOnChar = value;}
    }

    /// <summary>
    /// The character used to signal Xoff for X flow control (default: DC3)
    /// </summary>
    public ASCII XOffChar
    {
      get{return p_XOffChar;}
      set{p_XOffChar = value;}
    }

    /// <summary>
    /// The number of free bytes in the reception queue at which flow is disabled
    /// (Default: 0 = Set to 1/10th of actual rxQueue size)
    /// </summary>
    public int RxHighWater
    {
      get{return p_RxHighWater;}
      set{p_RxHighWater = value;}
    }

    /// <summary>
    /// The number of bytes in the reception queue at which flow is re-enabled
    /// (Default: 0 = Set to 1/10th of actual rxQueue size)
    /// </summary>
    public int RxLowWater
    {
      get{return p_RxLowWater;}
      set{p_RxLowWater = value;}
    }

    /// <summary>
    /// Multiplier. Max time for Send in ms = (Multiplier * Characters) + Constant
    /// (default: 0 = No timeout)
    /// </summary>
    public uint SendTimeoutMultiplier
    {
      get{return p_SendTimeoutMultiplier;}
      set{p_SendTimeoutMultiplier = value;}
    }

    /// <summary>
    /// Constant.  Max time for Send in ms = (Multiplier * Characters) + Constant (default: 0)
    /// </summary>
    public uint SendTimeoutConstant
    {
      get{return p_SendTimeoutConstant;}
      set{p_SendTimeoutConstant = value;}
    }

    /// <summary>
    /// Requested size for receive queue (default: 0 = use operating system default)
    /// </summary>
    public int RxQueue
    {
      get{return p_RxQueue;}
      set{p_RxQueue = value;}
    }

    /// <summary>
    /// Requested size for transmit queue (default: 0 = use operating system default)
    /// </summary>
    public int TxQueue
    {
      get{return p_TxQueue;}
      set{p_TxQueue = value;}
    }

    /// <summary>
    /// If true, the port will automatically re-open on next send if it was previously closed due
    /// to an error (default: false)
    /// </summary>
    public bool AutoReopen
    {
      get{return p_AutoReopen;}
      set{p_AutoReopen = value;}
    }

    /// <summary>
    /// If true, subsequent Send commands wait for completion of earlier ones enabling the results
    /// to be checked. If false, errors, including timeouts, may not be detected, but performance
    /// may be better.
    /// </summary>
    public bool CheckAllSends
    {
      get{return p_CheckAllSends;}
      set{p_CheckAllSends = value;}
    }

    /// <summary>
    /// True if the port is Open
    /// </summary>
    public bool IsOpen
    {
      get{return p_IsOpen;}
    }
    #endregion
    #region ctors
    public Rs232Stream(string port, int BaudRate)
    {
      p_Port = port;
      p_BaudRate = BaudRate;
      p_Status = new Rs232Status(this);

    }

    public Rs232Stream(string Port, int BaudRate, Rs232Handshake hs) : this(Port,BaudRate)
    {
      SetDefaults(hs);
    }
    #endregion
    #region Private Methods
    private IntPtr AllocateOverlappedMemory()
    {
      Overlapped o = new Overlapped();
      return Marshal.AllocHGlobal(Marshal.SizeOf(o));
    }

    private IntPtr AllocateOverlappedMemory(Overlapped o)
    {
      IntPtr mem = AllocateOverlappedMemory();
      Marshal.StructureToPtr(o,mem,true);
      return mem;
    }
    #endregion
    #region Public Methods
    #region Inherited
    public override void SetLength(long value)
    {
      throw new NotSupportedException("Seeking not supported: Stream length not absolute");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException("Seeking not supported: Stream length not absolute");
    }
    #endregion
    public void SetDefaults(Rs232Handshake Hs)
    {
      p_DataBits = 8; p_StopBits = Rs232StopBits.One; p_Parity = Rs232Parity.None;
      switch (Hs)
      {
        case Rs232Handshake.None:
          p_TxFlowCTS = false; p_TxFlowDSR = false; p_TxFlowX = false;
          p_RxFlowX = false; p_UseRTS = Rs232HandshakeOutput.None; //Online
          p_UseDTR = Rs232HandshakeOutput.None; p_TxWhenRxXOff = true; p_RxGateDSR = false;
          break;
        case Rs232Handshake.XOnXOff:
          p_TxFlowCTS = false; p_TxFlowDSR = false; p_TxFlowX = true;
          p_RxFlowX = true; p_UseRTS = Rs232HandshakeOutput.Online; 
          p_UseDTR = Rs232HandshakeOutput.Online; p_TxWhenRxXOff = true; p_RxGateDSR = false;
          p_XOnChar = ASCII.DC1; p_XOffChar = ASCII.DC3;
          break;
        case Rs232Handshake.CtsRts:
          p_TxFlowCTS = true; p_TxFlowDSR = false; p_TxFlowX = false;
          p_RxFlowX = false; p_UseRTS = Rs232HandshakeOutput.Handshake; 
          p_UseDTR = Rs232HandshakeOutput.Online; p_TxWhenRxXOff = true; p_RxGateDSR = false;
          break;
        case Rs232Handshake.DsrDtr:
          p_TxFlowCTS = false; p_TxFlowDSR = true; p_TxFlowX = false;
          p_RxFlowX = false; p_UseRTS = Rs232HandshakeOutput.Online;
          p_UseDTR = Rs232HandshakeOutput.Handshake; p_TxWhenRxXOff = true; p_RxGateDSR = false;
          break;
      }
    }
    public void SetDefaults(string Port, int BaudRate, Rs232Handshake Hs)
    {
      p_Port = Port; p_BaudRate = BaudRate;
      SetDefaults(Hs);
    }

    /// <summary>
    /// Opens the com port and configures it with the required settings
    /// </summary>
    public void Open() 
    {
      #region Validation
      if (p_IsOpen)
        throw new InvalidOperationException("Port Already Open");
      #endregion

      //-----------------> Open Port
      hPort = CreateFile(p_Port, DesiredAccess.GENERIC_READ | DesiredAccess.GENERIC_WRITE, 
        FileShare.None, IntPtr.Zero, FileCreationDispostion.OPEN_EXISTING, 
        FILE_FLAG_OVERLAPPED, IntPtr.Zero);

      if (hPort == (IntPtr)INVALID_HANDLE_VALUE)
        throw new IOException("Error Opening Port",new Win32Exception());

      p_IsOpen = true;

      #region Port Property Validation
      CommunicationProperties cp = new CommunicationProperties();
      if(!GetCommProperties(hPort, out cp))
        throw new IOException("Unable to retrieve COM Port properties. Error: 0x" + Marshal.GetLastWin32Error().ToString("X2"));

      if(cp.MaximumBaud != SettableBaud.BAUD_USER)
        if(SettableBaudToLong(cp.MaximumBaud) < p_BaudRate)
          throw new IOException("Baud rate of " + p_BaudRate.ToString() + " exceeds the maximum baud rate specified by the system of " + cp.MaximumBaud.ToString());

      if(cp.MaxRxQueue < p_RxQueue)
        if(cp.MaxRxQueue != 0)
          throw new IOException("Receive Queue size of of " + p_RxQueue.ToString() + " exceeds the maximum baud rate specified by the system of " + cp.MaxRxQueue.ToString());

      if(cp.MaxTxQueue < p_TxQueue)
        if(cp.MaxTxQueue !=0)
          throw new IOException("Transmit Queue size of of " + p_TxQueue.ToString() + " exceeds the maximum baud rate specified by the system of " + cp.MaxTxQueue.ToString());

      if((cp.SettableBaud&SettableBaud.BAUD_USER) != SettableBaud.BAUD_USER)
        if(LongToSettableBaud(p_BaudRate) != (cp.SettableBaud&LongToSettableBaud(p_BaudRate)))
          throw new IOException("Baud rate of " + p_BaudRate.ToString() + " is not allowed by the device. Acceptable values are: " + cp.SettableBaud.ToString());

      SettableData sd = new SettableData();
      sd = ((SettableData) Enum.Parse(sd.GetType(),"DATABITS_" + p_DataBits.ToString()));
      if((cp.SettableData & sd) != sd)
        throw new IOException("Data Bits setting of " + p_DataBits.ToString() + " Invalid for device. Acceptable values are: " + cp.SettableData.ToString());

      SettableStopParity ssp = new SettableStopParity();
      ssp = ((SettableStopParity) Enum.Parse(ssp.GetType(),"PARITY_" + p_Parity.ToString().ToUpper()));
      if((cp.SettableStopParity&ssp) != ssp)
        throw new IOException("Parity setting of " + p_Parity.ToString() + " not supported by device. Acceptable values: " + cp.SettableStopParity.ToString());

      switch(p_StopBits)
      {
        case Rs232StopBits.One:
          ssp = SettableStopParity.STOPBITS_10;
          break;
        case Rs232StopBits.OnePointFive:
          ssp = SettableStopParity.STOPBITS_15;
          break;
        case Rs232StopBits.Two:
          ssp = SettableStopParity.STOPBITS_20;
          break;
      }

      if((cp.SettableStopParity&ssp) != ssp)
        throw new IOException("Stop bits setting of " + p_Parity.ToString() + " not supported by device. Acceptable values: " + cp.SettableStopParity.ToString());

      if((p_UseDTR==Rs232HandshakeOutput.Online)&&(ProviderCapabilities.PCF_DTRDSR!=(cp.ProviderCapabilities&ProviderCapabilities.PCF_DTRDSR)))
        throw new IOException("DTR/DSR Not supported by hardware");

      if((p_UseRTS==Rs232HandshakeOutput.Online)&&(ProviderCapabilities.PCF_RTSCTS!=(cp.ProviderCapabilities&ProviderCapabilities.PCF_RTSCTS)))
        throw new IOException("RTS/CTS Not supported by hardware");

      if((p_Parity!=Rs232Parity.None)&&(ProviderCapabilities.PCF_PARITY_CHECK!=(cp.ProviderCapabilities&ProviderCapabilities.PCF_PARITY_CHECK)))
        throw new IOException("Parity not supported by hardware");

      if((p_RxFlowX||p_TxFlowX)&&(ProviderCapabilities.PCF_XONXOFF!=(cp.ProviderCapabilities&ProviderCapabilities.PCF_XONXOFF)))
        throw new IOException("XON/XOFF not supported by hardware");
      #endregion

      //-----------------> Set Queue Size
      if ((p_RxQueue != 0) || (p_TxQueue != 0))
        if (!SetupComm(hPort, (uint)p_RxQueue, (uint)p_TxQueue))
          throw new IOException("Bad Queue settings. Windows returned error: 0x" + Marshal.GetLastWin32Error().ToString("X2"));

      
      //-----------------> Set Comm Timeouts
      CommTimeouts CommTimeouts = 
        new CommTimeouts(MAXDWORD,0,0,
        (p_SendTimeoutMultiplier==0&&System.Environment.OSVersion.Platform!=System.PlatformID.Win32NT)?5000:p_SendTimeoutMultiplier,
        p_SendTimeoutMultiplier);

      if (!SetCommTimeouts(hPort, ref CommTimeouts))
        throw new IOException("Bad Timeout settings. Windows returned error: 0x" + Marshal.GetLastWin32Error().ToString("X2"));

      //-----------------> Set Port Settings
      Dcb PortDCB = new Dcb(p_TxFlowCTS, p_TxFlowDSR, p_UseDTR, 
        p_RxGateDSR, !p_TxWhenRxXOff, p_TxFlowX, p_RxFlowX, p_UseRTS,
        p_BaudRate,p_DataBits,p_Parity,p_StopBits,(char)p_XOnChar,(char)p_XOffChar,
        p_RxHighWater, p_RxLowWater, cp.CurrentRxQueue);

      if (!SetCommState(hPort, ref PortDCB)) 
        throw new IOException("Bad Port settings. Windows returned error: 0x" + Marshal.GetLastWin32Error().ToString("X2"));

      //-----------------> Set Class State
      /*
      stateBRK = 0;
      if (p_UseDTR == Rs232HandshakeOutput.None) stateDTR = 0;
      if (p_UseDTR == Rs232HandshakeOutput.Online) stateDTR = 1;
      if (p_UseRTS == Rs232HandshakeOutput.None) stateRTS = 0;
      if (p_UseRTS == Rs232HandshakeOutput.Online) stateRTS = 1;
      */
      //TODO: Clean this up.

      //p_Status.PortHandle = hPort;      
      
      p_Status.Enabled = true;

      //p_Status.Monitor(new TimeSpan(0,0,0,1,0));
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      #region Validation
      if(!p_IsOpen)
        throw new InvalidOperationException("Port is Closed.");
      #endregion
      uint read;
      int ovread;
      byte[] offsetbuf = new byte[buffer.Length];

      NativeOverlapped ov = new NativeOverlapped();
      while(!ReadFile(hPort, offsetbuf, (uint)count, out read, ref ov)) 
        if(Win32Exception.Check(false)!=NativeError.ERROR_IO_PENDING)
          throw new IOException("Unable to read from Stream",new Win32Exception("Error during ReadFile."));

      Array.Copy(offsetbuf,0,buffer,offset,read);

      while(!GetOverlappedResult(hPort, ref ov, out ovread, false))
        if(Win32Exception.Check(false)!=NativeError.ERROR_IO_PENDING)
          throw new Win32Exception("Error during GetOverlappedResult");
      
      return ovread;
    }

    public override void Flush()
    {
      #region Validation
      if(!p_IsOpen)
        throw new InvalidOperationException("Port is Closed.");
      #endregion
      NativeOverlapped ov = new NativeOverlapped();
      int foo;
      while(!GetOverlappedResult(hPort, ref ov, out foo, true))
        if(Marshal.GetLastWin32Error()!=(int)Errors.ERROR_IO_PENDING)
          throw new Win32Exception("Error during GetOverlappedResult");

      //TODO: If an error occurs, then the user may catch it. If so, the state should be reset for the class.
      //TODO: All exceptions that give win32lasterror should throw a win32exception
      //TODO: Find a better location for the overlapped struct
      //TODO: Consider replacing all IntPtrs with actual objects, and let P/Invoke marshal managed/unmanagaed code.
    }

    public override void Close()
    {
      #region Validation
      if(!p_IsOpen)
        throw new InvalidOperationException("Port is already Closed.");
      #endregion
      try
      {
        if(!CancelIo(hPort))
          throw new Win32Exception("Unable to cancel IO operations on port.");

        if(p_Status.Enabled)
          p_Status.Enabled = false;
      }
      finally
      {
        if(!CloseHandle(hPort))
          throw new Win32Exception("Unable to free port resources.");

        p_IsOpen = false;
      }
      base.Close ();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      uint foo;
      int foo2;
      NativeOverlapped ov = new NativeOverlapped();

      if(!WriteFile(hPort, buffer, (uint) count, out foo, ref ov))
        if(Marshal.GetLastWin32Error()!=(int)Errors.ERROR_IO_PENDING)
          throw new IOException("Error sending data.",new Win32Exception());

      while(!GetOverlappedResult(hPort, ref ov, out foo2, false))
        if(Win32Exception.Check(false)!=NativeError.ERROR_IO_PENDING&&Win32Exception.Check(false)!=NativeError.ERROR_IO_INCOMPLETE)
          throw new IOException("Error sending Data.",new Win32Exception("Error during GetOverlappedResult"));
    }

    #endregion
    
    #region P/Invoke
    #region Enumerations
    
    [Flags]
    public enum PurgeCommFlags
    {
      AbortTransmit = 1,
      AbortReceive = 2,
      PurgeTransmitBuffer = 4,
      PurgeReceiveBuffer = 8
    }

    /// <summary>
    /// Parity scheme to be used.
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    public enum DcbParity : byte
    {
      /// <summary>
      /// No Parity
      /// </summary>
      NOPARITY            = 0,

      /// <summary>
      /// Odd Parity
      /// </summary>
      ODDPARITY           = 1,
      
      /// <summary>
      /// Even Parity
      /// </summary>
      EVENPARITY          = 2,
      
      /// <summary>
      /// Mark Parity
      /// </summary>
      MARKPARITY          = 3,
      
      /// <summary>
      /// Space Parity
      /// </summary>
      SPACEPARITY         = 4
    }

    /// <summary>
    /// Number of stop bits to be used. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    public enum DcbStopBits : byte
    {
      /// <summary>
      /// 1 Stop Bit
      /// </summary>
      ONESTOPBIT          =0,
      
      /// <summary>
      /// 1.5 Stop Bits
      /// </summary>
      ONE5STOPBITS        =1,
      
      /// <summary>
      /// 2 Stop Bits
      /// </summary>
      TWOSTOPBITS         =2
    }

    /// <summary>
    /// RTS (request-to-send) flow control. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    public enum RtsControl : uint
    {
      /// <summary>
      /// Disables the RTS line when the device is opened and leaves it disabled.
      /// </summary>
      RTS_CONTROL_DISABLE    = 0x00,

      /// <summary>
      /// Enables the RTS line when the device is opened and leaves it on.
      /// </summary>
      RTS_CONTROL_ENABLE     = 0x01,
      
      /// <summary>
      /// Enables RTS handshaking. 
      /// </summary>
      /// <remarks>The driver raises the RTS line when the "type-ahead" (input) buffer is less than one-half full and lowers the RTS line when the buffer is more than three-quarters full. If handshaking is enabled, it is an error for the application to adjust the line by using the EscapeCommFunction function.</remarks>
      RTS_CONTROL_HANDSHAKE  = 0x02,

      /// <summary>
      /// Specifies that the RTS line will be high if bytes are available for transmission. After all buffered bytes have been sent, the RTS line will be low.
      /// </summary>
      /// <remarks>Windows Me/98/95:  This value is not supported.</remarks>
      RTS_CONTROL_TOGGLE     = 0x03
    }
    
    /// <summary>
    /// DTR (data-terminal-ready) flow control. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    public enum DtrControl : uint
    {
      /// <summary>
      /// Disables the DTR line when the device is opened and leaves it disabled.
      /// </summary>
      DTR_CONTROL_DISABLE    = 0x00,
      
      /// <summary>
      /// Enables the DTR line when the device is opened and leaves it on.
      /// </summary>
      DTR_CONTROL_ENABLE     = 0x01,

      /// <summary>
      /// Enables DTR handshaking. If handshaking is enabled, it is an error for the application to adjust the line by using the EscapeCommFunction function.
      /// <seealso cref="EscapeCommFunction"/>
      /// </summary>
      DTR_CONTROL_HANDSHAKE  = 0x02
    }

    /// <summary>
    /// Baud rate at which the communications device operates. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    private enum DcbBaudRate
    {

      /// <summary>
      /// 110 Baud
      /// </summary>
      CBR_110             = 110,
      
      /// <summary>
      /// 300 Baud
      /// </summary>
      CBR_300             = 300,
      
      /// <summary>
      /// 600 Baud
      /// </summary>
      CBR_600             = 600,
      
      /// <summary>
      /// 1200 Baud
      /// </summary>
      CBR_1200            = 1200,
      
      /// <summary>
      /// 2400 Baud
      /// </summary>
      CBR_2400            = 2400,
      
      /// <summary>
      /// 4800 Baud
      /// </summary>
      CBR_4800            = 4800,
      
      /// <summary>
      /// 9600 Baud
      /// </summary>
      CBR_9600            = 9600,

      /// <summary>
      /// 14400 Baud
      /// </summary>
      CBR_14400           = 14400,
      
      /// <summary>
      /// 19200 Baud
      /// </summary>
      CBR_19200           = 19200,
      
      /// <summary>
      /// 34800 Baud
      /// </summary>
      CBR_38400           = 38400,
      
      /// <summary>
      /// 56000 Baud
      /// </summary>
      CBR_56000           = 56000,
      
      /// <summary>
      /// 57600 Baud
      /// </summary>
      CBR_57600           = 57600,
      
      /// <summary>
      /// 115200 Baud
      /// </summary>
      CBR_115200          = 115200,
      
      /// <summary>
      /// 128000 Baud
      /// </summary>
      CBR_128000          = 128000,
      
      /// <summary>
      /// 256000 Baud
      /// </summary>
      CBR_256000          = 256000,
    }

    /// <summary>
    /// Constants for dwFunc
    /// </summary>
    private enum CommFunction
    {
      /// <summary>
      /// Causes transmission to act as if an XOFF character has been received.
      /// </summary>
      SETXOFF = 1,

      /// <summary>
      /// Causes transmission to act as if an XON character has been received.
      /// </summary>
      SETXON = 2,

      /// <summary>
      /// Sends the RTS (request-to-send) signal.
      /// </summary>
      SETRTS = 3,

      /// <summary>
      /// Clears the RTS (request-to-send) signal.
      /// </summary>
      CLRRTS = 4,

      /// <summary>
      /// Sends the DTR (data-terminal-ready) signal.
      /// </summary>
      SETDTR = 5,

      /// <summary>
      /// Clears the DTR (data-terminal-ready) signal.
      /// </summary>
      CLRDTR = 6,

      //TODO: Find the def for this code
      RESETDEV = 7,

      /// <summary>
      /// Suspends character transmission and places the transmission line in a break state until the ClearCommBreak function is called (or EscapeCommFunction is called with the CLRBREAK extended function code). The SETBREAK extended function code is identical to the SetCommBreak function. Note that this extended function does not flush data that has not been transmitted.
      /// </summary>
      SETBREAK = 8,

      /// <summary>
      /// Restores character transmission and places the transmission line in a nonbreak state. The CLRBREAK extended function code is identical to the ClearCommBreak function.
      /// </summary>
      CLRBREAK = 9
    }

    [Flags]
    private enum EventMask : uint
    {
      /// <summary>
      /// A character was received and placed in the input buffer.
      /// </summary>
      EV_RXCHAR = 0x0001,

      /// <summary>
      /// The event character was received and placed in the input buffer. The event character is specified in the device's DCB structure, which is applied to a serial port by using the SetCommState function.
      /// </summary>
      EV_RXFLAG = 0x0002,

      /// <summary>
      /// The last character in the output buffer was sent.
      /// </summary>
      EV_TXEMPTY = 0x0004,

      /// <summary>
      /// The CTS (clear-to-send) signal changed state.
      /// </summary>
      EV_CTS = 0x0008,

      /// <summary>
      /// The DSR (data-set-ready) signal changed state.
      /// </summary>
      EV_DSR = 0x0010,

      /// <summary>
      /// The RLSD (receive-line-signal-detect) signal changed state.
      /// </summary>
      EV_RLSD = 0x0020,

      /// <summary>
      /// A break was detected on input.
      /// </summary>
      EV_BREAK = 0x0040,

      /// <summary>
      /// A line-status error occurred. Line-status errors are CE_FRAME, CE_OVERRUN, and CE_RXPARITY.
      /// </summary>
      EV_ERR = 0x0080,

      /// <summary>
      /// A ring indicator was detected.
      /// </summary>
      EV_RING = 0x0100,

      /// <summary>
      /// A printer error occurred.
      /// </summary>
      EV_PERR = 0x0200,

      /// <summary>
      /// The receive buffer is 80 percent full.
      /// </summary>
      EV_RX80FULL = 0x0400,

      /// <summary>
      /// An event of the first provider-specific type occurred.
      /// </summary>
      EV_EVENT1 = 0x0800,

      /// <summary>
      /// An event of the second provider-specific type occurred.
      /// </summary>
      EV_EVENT2 = 0x1000
    }

    /// <summary>
    /// Constants for lpModemStat
    /// </summary>
    [Flags]
    private enum ModemStatus : uint
    {
      /// <summary>
      /// The CTS (clear-to-send) signal is on.
      /// </summary>
      MS_CTS_ON = 0x0010,

      /// <summary>
      /// The DSR (data-set-ready) signal is on.
      /// </summary>
      MS_DSR_ON = 0x0020,

      /// <summary>
      /// The ring indicator signal is on.
      /// </summary>
      MS_RING_ON = 0x0040,

      /// <summary>
      /// The RLSD (receive-line-signal-detect) signal is on.
      /// </summary>
      MS_RLSD_ON = 0x0080
    }

    /// <summary>
    /// Constants for lpErrors:
    /// </summary>
    [Flags]
    private enum CommErrors : uint
    {
      /// <summary>
      /// An input buffer overflow has occurred. There is either no room in the input buffer, or a character was received after the end-of-file (EOF) character.
      /// </summary>
      CE_RXOVER = 0x0001,

      /// <summary>
      /// A character-buffer overrun has occurred. The next character is lost.
      /// </summary>
      CE_OVERRUN = 0x0002,

      /// <summary>
      /// The hardware detected a parity error.
      /// </summary>
      CE_RXPARITY = 0x0004,

      /// <summary>
      /// The hardware detected a framing error.
      /// </summary>
      CE_FRAME = 0x0008,

      /// <summary>
      /// The hardware detected a break condition.
      /// </summary>
      CE_BREAK = 0x0010,

      /// <summary>
      /// The application tried to transmit a character, but the output buffer was full.
      /// </summary>
      CE_TXFULL = 0x0100,

      /// <summary>
      /// A time-out occurred on a parallel device. (Windows Me/98/95)
      /// </summary>
      CE_PTO = 0x0200,

      /// <summary>
      /// An I/O error occurred during communications with the device.
      /// </summary>
      CE_IOE = 0x0400,

      /// <summary>
      /// A parallel device is not selected. (Windows Me/98/95)
      /// </summary>
      CE_DNS = 0x0800,

      /// <summary>
      /// A parallel device signaled that it is out of paper. (Windows Me/98/95)
      /// </summary>
      CE_OOP = 0x1000,

      /// <summary>
      /// The requested mode is not supported, or the hFile parameter is invalid. If this value is specified, it is the only valid error.
      /// </summary>
      CE_MODE = 0x8000
    }

    /// <summary>
    /// Maximum allowable baud rate, in bits per second (bps).
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    [Flags]
    private enum SettableBaud : uint
    {
      
      /// <summary>
      /// 75 bps
      /// </summary>
      BAUD_075          = 0x00000001,

      /// <summary>
      /// 110 bps
      /// </summary>
      BAUD_110          = 0x00000002,

      /// <summary>
      /// 134.5 bps
      /// </summary>
      BAUD_134_5        = 0x00000004,

      /// <summary>
      /// 150 bps
      /// </summary>
      BAUD_150          = 0x00000008,

      /// <summary>
      /// 300 bps
      /// </summary>
      BAUD_300          = 0x00000010,

      /// <summary>
      /// 600 bps
      /// </summary>
      BAUD_600          = 0x00000020,

      /// <summary>
      /// 1200 bps
      /// </summary>
      BAUD_1200         = 0x00000040,

      /// <summary>
      /// 1800 bps
      /// </summary>
      BAUD_1800         = 0x00000080,

      /// <summary>
      /// 2400 bps
      /// </summary>
      BAUD_2400         = 0x00000100,

      /// <summary>
      /// 4800 bps
      /// </summary>
      BAUD_4800         = 0x00000200,

      /// <summary>
      /// 7200 bps
      /// </summary>
      BAUD_7200         = 0x00000400,

      /// <summary>
      /// 9600 bps
      /// </summary>
      BAUD_9600         = 0x00000800,

      /// <summary>
      /// 14400 bps
      /// </summary>
      BAUD_14400        = 0x00001000,

      /// <summary>
      /// 19200 bps
      /// </summary>
      BAUD_19200        = 0x00002000,

      /// <summary>
      /// 38400 bps
      /// </summary>
      BAUD_38400        = 0x00004000,

      /// <summary>
      /// 56K bps
      /// </summary>
      BAUD_56K          = 0x00008000,

      /// <summary>
      /// 57600 bps
      /// </summary>
      BAUD_128K         = 0x00010000,

      /// <summary>
      /// 115200 bps
      /// </summary>
      BAUD_115200       = 0x00020000,

      /// <summary>
      /// 128K bps
      /// </summary>
      BAUD_57600        = 0x00040000,

      /// <summary>
      /// Programmable baud rates available
      /// </summary>
      BAUD_USER         = 0x10000000
    }

    /// <summary>
    /// Communications-provider type. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    public enum ProviderSubType : uint
    {
      /// <summary>
      /// Unspecified
      /// </summary>
      PST_UNSPECIFIED      = 0x00000000,

      /// <summary>
      /// RS-232 serial port
      /// </summary>
      PST_RS232            = 0x00000001,

      /// <summary>
      /// Parallel port
      /// </summary>
      PST_PARALLELPORT     = 0x00000002,

      /// <summary>
      /// RS-422 port
      /// </summary>
      PST_RS422            = 0x00000003,

      /// <summary>
      /// RS-423 port
      /// </summary>
      PST_RS423            = 0x00000004,

      /// <summary>
      /// RS-449 port
      /// </summary>
      PST_RS449            = 0x00000005,

      /// <summary>
      /// Modem device
      /// </summary>
      PST_MODEM            = 0x00000006,

      /// <summary>
      /// FAX device
      /// </summary>
      PST_FAX              = 0x00000021,

      /// <summary>
      /// Scanner device
      /// </summary>
      PST_SCANNER          = 0x00000022,

      /// <summary>
      /// Unspecified Network Bridge
      /// </summary>
      PST_NETWORK_BRIDGE   = 0x00000100,

      /// <summary>
      /// LAT protocol
      /// </summary>
      PST_LAT              = 0x00000101,

      /// <summary>
      /// TCP/IP Telnet® protocol
      /// </summary>
      PST_TCPIP_TELNET     = 0x00000102,

      /// <summary>
      /// X.25 standards
      /// </summary>
      PST_X25              = 0x00000103
    }

    /// <summary>
    /// Capabilities offered by the provider. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    [Flags]
    private enum ProviderCapabilities : uint
    {
      /// <summary>
      /// DTR (data-terminal-ready)/DSR (data-set-ready) supported
      /// </summary>
      PCF_DTRDSR        = 0x0001,

      /// <summary>
      /// RTS (request-to-send)/CTS (clear-to-send) supported
      /// </summary>
      PCF_RTSCTS        = 0x0002,

      /// <summary>
      /// RLSD (receive-line-signal-detect) supported
      /// </summary>
      PCF_RLSD          = 0x0004,

      /// <summary>
      /// Parity checking supported
      /// </summary>
      PCF_PARITY_CHECK  = 0x0008,

      /// <summary>
      /// XON/XOFF flow control supported
      /// </summary>
      PCF_XONXOFF       = 0x0010,

      /// <summary>
      /// Settable XON/XOFF supported
      /// </summary>
      PCF_SETXCHAR      = 0x0020,

      /// <summary>
      /// Total (elapsed) time-outs supported
      /// </summary>
      PCF_TOTALTIMEOUTS = 0x0040,

      /// <summary>
      /// Interval time-outs supported
      /// </summary>
      PCF_INTTIMEOUTS   = 0x0080,

      /// <summary>
      /// Special character support provided
      /// </summary>
      PCF_SPECIALCHARS  = 0x0100,

      /// <summary>
      /// Special 16-bit mode supported
      /// </summary>
      PCF_16BITMODE     = 0x0200
    }

    /// <summary>
    /// Communications parameter that can be changed. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    [Flags]
    private enum SettableParameters : uint
    {
      /// <summary>
      /// Parity
      /// </summary>
      SP_PARITY         = 0x0001,
      
      /// <summary>
      /// Baud rate
      /// </summary>
      SP_BAUD           = 0x0002,
      
      /// <summary>
      /// Data bits
      /// </summary>
      SP_DATABITS       = 0x0004,
      
      /// <summary>
      /// Stop bits
      /// </summary>
      SP_STOPBITS       = 0x0008,
      
      /// <summary>
      /// Handshaking (flow control)
      /// </summary>
      SP_HANDSHAKING    = 0x0010,
      
      /// <summary>
      /// Parity checking
      /// </summary>
      SP_PARITY_CHECK   = 0x0020,
      
      /// <summary>
      /// RLSD (receive-line-signal-detect)
      /// </summary>
      SP_RLSD           = 0x0040
    }

    /// <summary>
    /// Number of data bits that can be set. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    [Flags]
    private enum SettableData : ushort
    {
      /// <summary>
      /// 5 data bits
      /// </summary>
      DATABITS_5        = 0x0001,
      
      /// <summary>
      /// 6 data bits
      /// </summary>
      DATABITS_6        = 0x0002,
      
      /// <summary>
      /// 7 data bits
      /// </summary>
      DATABITS_7        = 0x0004,
      
      /// <summary>
      /// 8 data bits
      /// </summary>
      DATABITS_8        = 0x0008,
      
      /// <summary>
      /// 16 data bits
      /// </summary>
      DATABITS_16       = 0x0010,
      
      /// <summary>
      /// Special wide path through serial hardware lines
      /// </summary>
      DATABITS_16X      = 0x0020
    }

    /// <summary>
    /// Stop bit and parity settings that can be selected. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp</remarks>
    [Flags]
    private enum SettableStopParity : ushort
    {
      /// <summary>
      /// 1 stop bit
      /// </summary>
      STOPBITS_10       = 0x0001,

      /// <summary>
      /// 1.5 stop bits
      /// </summary>
      STOPBITS_15       = 0x0002,
      
      /// <summary>
      /// 2 stop bits
      /// </summary>
      STOPBITS_20       = 0x0004,
      
      /// <summary>
      /// No parity
      /// </summary>
      PARITY_NONE       = 0x0100,
      
      /// <summary>
      /// Odd parity
      /// </summary>
      PARITY_ODD        = 0x0200,
      
      /// <summary>
      /// Even parity
      /// </summary>
      PARITY_EVEN       = 0x0400,
      
      /// <summary>
      /// Mark parity
      /// </summary>
      PARITY_MARK       = 0x0800,
      
      /// <summary>
      /// Space parity
      /// </summary>
      PARITY_SPACE      = 0x1000
      
    }
    #endregion
    #region Structs
    /// <summary>
    /// The CommunicationProperties structure is used by the GetCommProperties function to return information about a given communications driver.
    /// </summary>
    /// <remarks>
    /// Original Win32 Name: COMMPROP
    /// http://msdn.microsoft.com/library/en-us/devio/base/commprop_str.asp
    /// </remarks>
    [StructLayout( LayoutKind.Sequential )]
      private struct CommunicationProperties
    {
      /// <summary>
      /// Size of the entire data packet, regardless of the amount of data requested, in bytes. 
      /// </summary>
      /// <remarks>Original Name: wPacketLength </remarks>
      public ushort PacketLength; 

      /// <summary>
      /// Version of the structure. 
      /// </summary>
      /// <remarks>Original Name: wPacketVersion </remarks>
      public ushort PacketVersion; 
      
      /// <summary>
      /// Bitmask indicating which services are implemented by this provider. The SP_SERIALCOMM value is always specified for communications providers, including modem providers. 
      /// </summary>
      /// <remarks>Original Name: dwServiceMask </remarks>
      public uint ServiceMask;

      /// <summary>
      /// Reserved; do not use. 
      /// </summary>
      /// <remarks>Original Name: dwReserved1 </remarks>
      public uint Reserved1; 

      /// <summary>
      /// Maximum size of the driver's internal output buffer, in bytes. A value of zero indicates that no maximum value is imposed by the serial provider. 
      /// </summary>
      /// <remarks>Original Name: dwMaxTxQueue </remarks>
      public uint MaxTxQueue; 

      /// <summary>
      /// Maximum size of the driver's internal input buffer, in bytes. A value of zero indicates that no maximum value is imposed by the serial provider. 
      /// </summary>
      /// <remarks>Original Name: dwMaxRxQueue </remarks>
      public uint MaxRxQueue; 

      /// <summary>
      /// Maximum allowable baud rate, in bits per second (bps).
      /// </summary>
      /// <remarks>Original Name: dwMaxBaud </remarks>
      public SettableBaud MaximumBaud; 

      /// <summary>
      /// Communications-provider type.
      /// </summary>
      /// <remarks>Original Name: dwProvSubType </remarks>
      public ProviderSubType ProviderSubType; 

      /// <summary>
      /// Capabilities offered by the provider.
      /// </summary>
      /// <remarks>Original Name: dwProvCapabilities </remarks>
      public ProviderCapabilities ProviderCapabilities; 
      
      /// <summary>
      /// Communications parameter that can be changed. 
      /// </summary>
      /// <remarks>Original Name: dwSettableParams </remarks>
      public SettableParameters SettableParameters; 

      /// <summary>
      /// Baud rates that can be used. 
      /// </summary>
      /// <remarks>Original Name: dwSettableBaud </remarks>
      public SettableBaud SettableBaud;

      /// <summary>
      /// Number of data bits that can be set. 
      /// </summary>
      /// <remarks>Original Name: wSettableData </remarks>
      public SettableData SettableData; 

      /// <summary>
      /// Stop bit and parity settings that can be selected.
      /// </summary>
      /// <remarks>Original Name: wSettableStopParity </remarks>
      public SettableStopParity SettableStopParity; 

      /// <summary>
      /// Size of the driver's internal output buffer, in bytes. A value of zero indicates that the value is unavailable.
      /// </summary>
      /// <remarks>Original Name: dwCurrentTxQueue </remarks>
      public uint CurrentTxQueue; 

      /// <summary>
      /// Size of the driver's internal input buffer, in bytes. A value of zero indicates that the value is unavailable. 
      /// </summary>
      /// <remarks>Original Name: dwCurrentRxQueue </remarks>
      public uint CurrentRxQueue; 

      /// <summary>
      /// Provider-specific data. Applications should ignore this member unless they have detailed information about the format of the data required by the provider. 
      /// </summary>
      /// <remarks>Set this member to COMMPROP_INITIALIZED before calling the GetCommProperties function to indicate that the wPacketLength member is already valid.</remarks>
      /// <remarks>Original Name: dwProvSpec1 </remarks>
      public uint ProviderSpecific1; // = 0xE73CF52E 

      /// <summary>
      /// Provider-specific data. Applications should ignore this member unless they have detailed information about the format of the data required by the provider. 
      /// </summary>
      /// <remarks>Original Name: dwProvSpec1 </remarks>
      public uint ProviderSpecific2; 
      
      /// <summary>
      /// Provider-specific data. Applications should ignore this member unless they have detailed information about the format of the data required by the provider.
      /// </summary>
      /// <remarks>
      /// If the provider subtype is PST_MODEM, use the MODEMDEVCAPS Win32 Structure<br/>
      /// Original Name: wcProvChar 
      /// </remarks>
      public byte ProviderChar; 
    }

    /// <summary>
    /// The COMSTAT structure contains information about a communications device. This structure is filled by the ClearCommError function.
    /// </summary>
    /// <remarks>
    /// Original Win32 Name: COMSTAT<br/>
    /// http://msdn.microsoft.com/library/en-us/devio/base/comstat_str.asp
    /// </remarks>
    [StructLayout( LayoutKind.Sequential )]
      private struct CommunicationStatus
    {
      /// <summary>
      /// If this member is TRUE, transmission is waiting for the CTS (clear-to-send) signal to be sent. 
      /// </summary>
      /// <remarks>Original Name: fCtsHold</remarks>
      const uint CtsHold = 0x1;

      /// <summary>
      /// If this member is TRUE, transmission is waiting for the DSR (data-set-ready) signal to be sent. 
      /// </summary>
      /// <remarks>Original Name: fDsrHold </remarks>
      const uint DsrHold = 0x2;
      
      /// <summary>
      /// If this member is TRUE, transmission is waiting for the RLSD (receive-line-signal-detect) signal to be sent. 
      /// </summary>
      /// <remarks>Original Name: fRlsdHold</remarks>
      const uint RlsdHold = 0x4;

      /// <summary>
      /// If this member is TRUE, transmission is waiting because the XOFF character was received. 
      /// </summary>
      /// <remarks>Original Name: fXoffHold</remarks>
      const uint XOffHold = 0x8;
      
      /// <summary>
      /// If this member is TRUE, transmission is waiting because the XOFF character was transmitted. (Transmission halts when the XOFF character is transmitted to a system that takes the next character as XON, regardless of the actual character.) 
      /// </summary>
      /// <remarks>Original Name: fXoffSent</remarks>
      const uint XOffSent = 0x10;

      /// <summary>
      /// If this member is TRUE, the end-of-file (EOF) character has been received. 
      /// </summary>
      /// <remarks>Original Name: fEof</remarks>
      const uint Eof = 0x20;

      /// <summary>
      /// If this member is TRUE, there is a character queued for transmission that has come to the communications device by way of the TransmitCommChar function. The communications device transmits such a character ahead of other characters in the device's output buffer. 
      /// </summary>
      /// <remarks>Original Name: fTxim</remarks>
      const uint Txim = 0x40;

      /// <summary>
      /// Reserved; do not use.
      /// </summary>
      /// <remarks>Original Name: fReserved</remarks>
      public uint Flags;

      /// <summary>
      /// Number of bytes received by the serial provider but not yet read by a ReadFile operation. 
      /// </summary>
      /// <remarks>Original Name: cbInQue</remarks>
      public uint InQue;
      
      /// <summary>
      /// Number of bytes of user data remaining to be transmitted for all write operations. This value will be zero for a nonoverlapped write. 
      /// </summary>
      /// <remarks>Original Name: cbOutQue</remarks>
      public uint OutQue;
    }

    /// <summary>
    /// The DCB structure defines the control setting for a serial communications device.
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/dcb_str.asp</remarks>
    [StructLayout( LayoutKind.Sequential )]
      public struct Dcb
    {
      #region ctors
      public Dcb(bool outCTS, bool outDSR, Rs232HandshakeOutput dtr, 
        bool inDSR, bool txc, bool xOut, bool xIn, Rs232HandshakeOutput rts, 
        int baudRate, int byteSize, Rs232Parity parity, Rs232StopBits stopBits, 
        char xOnChar, char xOffChar, int rxLowWater, int rxHighWater,
        uint currentRxQueue)
      {
        DCBlength = 0;
        //Bitmasked Values
        this.m_Flags = 0;
        this.BaudRate = baudRate;
        this.Reserved = 0;
        this.ByteSize = (byte) byteSize;
        this.Parity = (DcbParity) parity;              
        this.StopBits = (DcbStopBits) stopBits;
        this.XOnChar = xOnChar;
        this.XOffChar = xOffChar;
        this.ErrorChar = (char) 0x00;
        this.EofChar = (char) 0x00;
        this.EventChar = (char) 0x00;
        this.Reserved1 = 0;

        //JH 1.2: Defaulting mechanism for handshake thresholds - prevents problems of setting specific
        //defaults which may violate the size of the actually granted queue. If the user specifically sets
        //these values, it's their problem!
        if ((rxLowWater == 0) || (rxHighWater == 0)) 
          if (currentRxQueue > 0)
            this.XOffLim = this.XOnLim = (short)((int)currentRxQueue / 10);
          else
            this.XOffLim = this.XOnLim = 8;
        else
        {
          this.XOffLim = (short)rxHighWater;
          this.XOnLim = (short)rxLowWater;
        }

        this.fParity = (parity!=Rs232Parity.None);
        this.OutxCtsFlow = outCTS;
        this.OutxDsrFlow = outDSR;
        this.DsrSensitivity = inDSR;
        this.TxContinueOnXOff= txc;
        this.OutX = xOut;
        this.InX = xIn;
        this.Binary = true;
        this.AbortOnError = true;
        this.fErrorChar = false;
        this.DtrControl = (Rs232Stream.DtrControl) dtr;
        this.RtsControl = (Rs232Stream.RtsControl) rts;

        DCBlength = Marshal.SizeOf(this);
      }
      #endregion

      /// <summary>
      /// Length of the structure, in bytes. 
      /// </summary>
      public int DCBlength;      

      /// <summary>
      /// Baud rate at which the communications device operates. See DCBBaudRates. <see cref="DCBBaudRate"/>
      /// </summary>
      public int BaudRate;       

      #region Private Bitmasked value
      private uint m_Flags;
      #endregion
      #region Public bitmasked accessors
      /// <summary>
      /// If this member is TRUE, binary mode is enabled. Windows does not support nonbinary mode transfers, so this member must be TRUE. 
      /// </summary>
      /// <remarks>Original Name: fBinary</remarks>
      public bool Binary
      {
        get{return (m_Flags & 0x01)==0x01;}
        set
        {
          if(value)
            m_Flags |= 0x01;
          else
            m_Flags &= 0xFFFFFFFE;
        }
      }

      /// <summary>
      /// If this member is TRUE, parity checking is performed and errors are reported. 
      /// </summary>
      /// <remarks>Original Name: fParity</remarks>
      public bool fParity
      {
        get{return (m_Flags & 0x02)==0x02;}
        set
        {
          if(value)
            m_Flags |= 0x02;
          else
            m_Flags &= 0xFFFFFFFD;
        }
      }
    
      /// <summary>
      /// If this member is TRUE, the CTS (clear-to-send) signal is monitored for output flow control. If this member is TRUE and CTS is turned off, output is suspended until CTS is sent again. 
      /// </summary>
      /// <remarks>Original Name: fOutxCtsFlow</remarks>
      public bool OutxCtsFlow
      {
        get{return (m_Flags & 0x04)==0x04;}
        set
        {
          if(value)
            m_Flags |= 0x04;
          else
            m_Flags &= 0xFFFFFFFB;
        }
      }

      /// <summary>
      /// If this member is TRUE, the DSR (data-set-ready) signal is monitored for output flow control. If this member is TRUE and DSR is turned off, output is suspended until DSR is sent again. 
      /// </summary>
      /// <remarks>Original Name: fOutxDsrFlow</remarks>
      public bool OutxDsrFlow
      {
        get{return (m_Flags & 0x08)==0x08;}
        set
        {
          if(value)
            m_Flags |= 0x08;
          else
            m_Flags &= 0xFFFFFFF7;
        }
      }

      /// <summary>
      /// DTR (data-terminal-ready) flow control. 
      /// </summary>
      /// <remarks>Original Name: fDtrControl</remarks>
      public DtrControl DtrControl
      {
        get{return (DtrControl) (m_Flags >> 4);}
        set{m_Flags |= ((0xFFFFFFCF & m_Flags) | ((uint) value << 4));}
      }

      /// <summary>
      /// If this member is TRUE, the communications driver is sensitive to the state of the DSR signal. The driver ignores any bytes received, unless the DSR modem input line is high. 
      /// </summary>
      /// <remarks>Original Name: fDsrSensitivity</remarks>
      public bool DsrSensitivity
      {
        get{return (m_Flags & 0x40)==0x40;}
        set
        {
          if(value)
            m_Flags |= 0x40;
          else
            m_Flags &= 0xFFFFFFBF;
        }
      }
 

      /// <summary>
      /// If this member is TRUE, transmission continues after the input buffer has come within XoffLim bytes of being full and the driver has transmitted the XoffChar character to stop receiving bytes. If this member is FALSE, transmission does not continue until the input buffer is within XonLim bytes of being empty and the driver has transmitted the XonChar character to resume reception. 
      /// </summary>
      /// <remarks>Original Name: fTXContinueOnXoff</remarks>
      public bool TxContinueOnXOff
      {
        get{return (m_Flags & 0x80)==0x80;}
        set
        {
          if(value)
            m_Flags |= 0x80;
          else
            m_Flags &= 0xFFFFFF7F;
        }
      }
 
      /// <summary>
      /// Indicates whether XON/XOFF flow control is used during transmission. If this member is TRUE, transmission stops when the XoffChar character is received and starts again when the XonChar character is received. 
      /// </summary>
      /// <remarks>Original Name: fOutX</remarks>
      public bool OutX
      {
        get{return (m_Flags & 0x0100)==0x0100;}
        set
        {
          if(value)
            m_Flags |= 0x0100;
          else
            m_Flags &= 0xFFFFFEFF;
        }
      }
      
      /// <summary>
      /// Indicates whether XON/XOFF flow control is used during reception. If this member is TRUE, the XoffChar character is sent when the input buffer comes within XoffLim bytes of being full, and the XonChar character is sent when the input buffer comes within XonLim bytes of being empty. 
      /// </summary>
      /// <remarks>Original Name: fInX</remarks>
      public bool InX
      {
        get{return (m_Flags & 0x0200)==0x0200;}
        set
        {
          if(value)
            m_Flags |= 0x0200;
          else
            m_Flags &= 0xFFFFFDFF;
        }
      }
      
      /// <summary>
      /// Indicates whether bytes received with parity errors are replaced with the character specified by the ErrorChar member. If this member is TRUE and the fParity member is TRUE, replacement occurs. 
      /// </summary>
      /// <remarks>Original Name: fErrorChar</remarks>
      public bool fErrorChar
      {
        get{return (m_Flags & 0x0400)==0x0400;}
        set
        {
          if(value)
            m_Flags |= 0x0400;
          else
            m_Flags &= 0xFFFFFBFF;
        }
      }
      
      /// <summary>
      /// RTS (request-to-send) flow control. 
      /// </summary>
      /// <remarks>Original Name: fRtsControl</remarks>
      public RtsControl RtsControl
      {
        get{return (RtsControl) (m_Flags >> 12);}
        set{m_Flags |= ((0xFFFFCFFF & m_Flags) | ((uint)value << 12));}
      }  

      /// <summary>
      /// If this member is TRUE, the driver terminates all read and write operations with an error status if an error occurs. The driver will not accept any further communications operations until the application has acknowledged the error by calling the ClearCommError function. <seealso cref="ClearCommError"/>
      /// </summary>
      /// <remarks>Original Name: fAbortOnError</remarks>
      public bool AbortOnError
      {
        get{return (m_Flags & 0x4000)==0x4000;}
        set
        {
          if(value)
            m_Flags |= 0x4000;
          else
            m_Flags &= 0xFFFFBFFF;
        }
      }

      #endregion
 
      /// <summary>
      /// Reserved; must be zero. 
      /// </summary>
      /// <remarks>Original Name: wReserved</remarks>
      public short Reserved;

      /// <summary>
      /// Minimum number of bytes allowed in the input buffer before flow control is activated to inhibit the sender. 
      /// </summary>
      /// <remarks>Note that the sender may transmit characters after the flow control signal has been activated, so this value should never be zero. This assumes that either XON/XOFF, RTS, or DTR input flow control is specified in fInX, fRtsControl, or fDtrControl.</remarks>
      /// <remarks>Original Name: XonLim</remarks>
      public short XOnLim;   

      /// <summary>
      /// Maximum number of bytes allowed in the input buffer before flow control is activated to allow transmission by the sender.
      /// </summary>
      /// <remarks>Original Name: XoffLim</remarks>
      public short XOffLim;  

      /// <summary>
      /// This assumes that either XON/XOFF, RTS, or DTR input flow control is specified in fInX, fRtsControl, or fDtrControl. The maximum number of bytes allowed is calculated by subtracting this value from the size, in bytes, of the input buffer. 
      /// </summary>
      /// <remarks>Original Name: ByteSize</remarks>
      public byte ByteSize;  

      /// <summary>
      /// Parity scheme to be used.
      /// </summary>
      /// <remarks>Original Name: Parity</remarks>
      public DcbParity Parity;    


      /// <summary>
      /// Number of stop bits to be used. 
      /// </summary>
      /// <remarks>Original Name: StopBits</remarks>
      public DcbStopBits StopBits;  

      /// <summary>
      /// Value of the XON character for both transmission and reception. 
      /// </summary>
      /// <remarks>Original Name: XonChar</remarks>
      public char XOnChar;   

      /// <summary>
      /// Value of the XOFF character for both transmission and reception. 
      /// </summary>
      /// <remarks>Original Name: XoffChar</remarks>
      public char XOffChar;  

      /// <summary>
      /// Value of the character used to replace bytes received with a parity error. 
      /// </summary>
      /// <remarks>Original Name: ErrorChar</remarks>
      public char ErrorChar; 

      /// <summary>
      /// Value of the character used to signal the end of data. 
      /// </summary>
      /// <remarks>Original Name: EofChar</remarks>
      public char EofChar;   
      
      /// <summary>
      /// Value of the character used to signal an event. 
      /// </summary>
      /// <remarks>Original Name: EvtChar</remarks>
      public char EventChar;

      /// <summary>
      /// Reserved; do not use. 
      /// </summary>
      /// <remarks>Original Name: wReserved1</remarks>
      public short Reserved1;
    }

    /// <summary>
    /// The OVERLAPPED structure contains information used in asynchronous input and output (I/O).
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/library/en-us/dllproc/base/overlapped_str.asp
    /// You can use the HasOverlappedIoCompleted macro to determine whether an asynchronous I/O operation has completed. You can use the CancelIo function to cancel an asynchronous I/O operation.
    /// </remarks>
//    [StructLayout( LayoutKind.Sequential )] 
//    private struct Overlapped
//    {
//      #region ctors
//      public Overlapped(UIntPtr Internal, UIntPtr InternalHigh, UInt32 Offset, UInt32 OffsetHigh, IntPtr hEvent)
//      {
//        this.Internal = Internal;
//        this.InternalHigh = InternalHigh;
//        this.Offset = Offset;
//        this.OffsetHigh = OffsetHigh;
//        this.hEvent = hEvent;
//      }
//
//      public Overlapped(UInt32 OffsetHigh, UInt32 Offset, IntPtr hEvent)
//      {
//        this.Internal = new UIntPtr(0);
//        this.InternalHigh = new UIntPtr(0);
//        this.Offset = Offset;
//        this.OffsetHigh = OffsetHigh;
//        this.hEvent = hEvent;
//      }
//      #endregion
//      /// <summary>
//      /// Reserved for operating system use. This member, which specifies a system-dependent status, is valid when the GetOverlappedResult function returns without setting the extended error information to ERROR_IO_PENDING. <see cref="GetOverlappedResult"/>
//      /// </summary>
//      public UIntPtr Internal;
//      
//      /// <summary>
//      /// Reserved for operating system use. This member, which specifies the length of the data transferred, is valid when the GetOverlappedResult function returns TRUE.
//      /// </summary>
//      public UIntPtr InternalHigh;
//      
//      /// <summary>
//      /// File position at which to start the transfer. The file position is a byte offset from the start of the file. The calling process sets this member before calling the ReadFile or WriteFile function.  
//      /// </summary>
//      /// <remarks>This member is ignored when reading from or writing to named pipes and communications devices and should be zero.</remarks>
//      public UInt32 Offset;
//
//      /// <summary>
//      /// High-order word of the byte offset at which to start the transfer.
//      /// </summary>
//      /// <remarks>This member is ignored when reading from or writing to named pipes and communications devices and should be zero.</remarks>
//      public UInt32 OffsetHigh;
//
//      /// <summary>
//      /// Handle to an event set to the signaled state when the operation has been completed. The calling process must set this member either to zero or a valid event handle before calling any overlapped functions. To create an event object, use the CreateEvent function. <seealso cref="CreateEvent"/>
//      /// </summary>
//      /// <remarks>Functions such as WriteFile set the event to the nonsignaled state before they begin an I/O operation.</remarks>
//      public IntPtr hEvent;
//    }


    /// <summary>
    /// The COMMTIMEOUTS structure is used in the SetCommTimeouts and GetCommTimeouts functions to set and query the time-out parameters for a communications device. 
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/commtimeouts_str.asp</remarks>
    [StructLayout( LayoutKind.Sequential )]
      private struct CommTimeouts 
    {
      #region ctors
      public CommTimeouts(uint ReadIntervalTimeout,uint ReadTotalTimeoutMultiplier,
        uint ReadTotalTimeoutConstant,uint WriteTotalTimeoutMultiplier,
        uint WriteTotalTimeoutConstant)
      {
        this.ReadIntervalTimeout = ReadIntervalTimeout;
        this.ReadTotalTimeoutMultiplier = ReadTotalTimeoutMultiplier;
        this.ReadTotalTimeoutConstant = ReadTotalTimeoutConstant;
        this.WriteTotalTimeoutMultiplier = WriteTotalTimeoutMultiplier;
        this.WriteTotalTimeoutConstant = WriteTotalTimeoutConstant;
      }

      #endregion
      /// <summary>
      /// Maximum time allowed to elapse between the arrival of two characters on the communications line, in milliseconds. 
      /// </summary>
      /// <remarks>During a ReadFile operation, the time period begins when the first character is received. If the interval between the arrival of any two characters exceeds this amount, the ReadFile operation is completed and any buffered data is returned. A value of zero indicates that interval time-outs are not used. A value of MAXDWORD, combined with zero values for both the ReadTotalTimeoutConstant and ReadTotalTimeoutMultiplier members, specifies that the read operation is to return immediately with the characters that have already been received, even if no characters have been received.</remarks>
      public uint ReadIntervalTimeout;

      /// <summary>
      /// Multiplier used to calculate the total time-out period for read operations, in milliseconds.each read operation, this value is multiplied by the requested number of bytes to be read. 
      /// </summary>
      /// <remarks>A value of zero for both the ReadTotalTimeoutMultiplier and ReadTotalTimeoutConstant members indicates that total time-outs are not used for read operations.</remarks>
      public uint ReadTotalTimeoutMultiplier;

      /// <summary>
      /// Constant used to calculate the total time-out period for read operations, in milliseconds. For each read operation, this value is added to the product of the ReadTotalTimeoutMultiplier member and the requested number of bytes. 
      /// </summary>
      public uint ReadTotalTimeoutConstant;
      
      /// <summary>
      /// Multiplier used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is multiplied by the number of bytes to be written. 
      /// </summary>
      public uint WriteTotalTimeoutMultiplier;
      
      /// <summary>
      /// Constant used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is added to the product of the WriteTotalTimeoutMultiplier member and the number of bytes to be written. 
      /// </summary>
      /// <remarks>A value of zero for both the ReadTotalTimeoutMultiplier and ReadTotalTimeoutConstant members indicates that total time-outs are not used for read operations.</remarks>
      public uint WriteTotalTimeoutConstant;
    }

    #endregion
    #region Helper Functions
    private long SettableBaudToLong(SettableBaud baudRate)
    {
      return Int64.Parse(baudRate.ToString().Substring(4).Replace("k","000"));
    }

    private SettableBaud LongToSettableBaud(long baudRate)
    {
      SettableBaud sb = SettableBaud.BAUD_USER;
      try
      {
        return (SettableBaud) Enum.Parse(sb.GetType(),
          "BAUD_" + baudRate.ToString().Replace("000","k").Replace(".","_").PadLeft(3,'0'));
      }
      catch(InvalidCastException)
      {
        return SettableBaud.BAUD_USER;
      }
 
    }
    #endregion

    //Constants for return value:
    private const Int32 INVALID_HANDLE_VALUE = -1;

    //Constants for dwFlagsAndAttributes:
    private const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

    private const UInt32 MAXDWORD = 0xffffffff;

    /// <summary>
    /// The CreateFile function creates or opens a file, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, or named pipe. 
    /// </summary>
    /// <param name="lpFileName">[in] Pointer to a null-terminated string that specifies the name of the object to create or open.</param>
    /// <param name="dwDesiredAccess">[in] Access to the object (reading, writing, or both). <seealso cref="DesiredAccess"/></param>
    /// <param name="dwShareMode">[in] Sharing mode of the object (reading, writing, both, or neither).</param>
    /// <param name="lpSecurityAttributes">[in] Pointer to a SECURITY_ATTRIBUTES structure that determines whether the returned handle can be inherited by child processes.</param>
    /// <param name="dwCreationDisposition">[in] Action to take on files that exist, and which action to take when files do not exist. For more information about this parameter, see the Remarks section.</param>
    /// <param name="dwFlagsAndAttributes">[in] File attributes and flags.</param>
    /// <param name="hTemplateFile">[in] Handle to a template file, with the GENERIC_READ access right. (Windows Me/98/95:  The hTemplateFile parameter must be NULL.)</param>
    /// <returns>If the function succeeds, the return value is an open handle to the specified file. If the function fails, the return value is INVALID_HANDLE_VALUE.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/fileio/base/createfile.asp</remarks>
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern IntPtr CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, 
      FileShare dwShareMode, IntPtr lpSecurityAttributes, FileCreationDispostion dwCreationDisposition, 
      UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

    /// <summary>
    /// The CloseHandle function closes an open object handle.
    /// </summary>
    /// <param name="hObject">[in, out] Handle to an open object.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/sysinfo/base/closehandle.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);

    /// <summary>
    /// The GetCommState function retrieves the current control settings for a specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="lpDCB">[out] Pointer to a DCB structure that receives the control settings information.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/getcommstate.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool GetCommState(IntPtr hFile, ref Dcb lpDCB);

    /// <summary>
    /// The GetCommTimeouts function retrieves the time-out parameters for all read and write operations on a specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="lpCommTimeouts">[out] Pointer to a COMMTIMEOUTS structure in which the time-out information is returned. <see cref="CommTimeouts"/></param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/getcommtimeouts.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool GetCommTimeouts(IntPtr hFile, out CommTimeouts lpCommTimeouts);

    /// <summary>
    /// The BuildCommDCBAndTimeouts function translates a device-definition string into appropriate device-control block codes and places them into a device control block.
    /// </summary>
    /// <param name="lpDef">[in] Pointer to a null-terminated string that specifies device-control information.</param>
    /// <param name="lpDCB">Pointer to a DCB structure that receives information from the device-control information string pointed to by lpDef.</param>
    /// <param name="lpCommTimeouts">Pointer to a COMMTIMEOUTS structure that the function can use to set device time-out values. <see cref="CommTimeouts"/></param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/buildcommdcbandtimeouts.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool BuildCommDCBAndTimeouts(String lpDef, ref Dcb lpDCB, ref CommTimeouts lpCommTimeouts);

    /// <summary>
    /// The SetCommState function configures a communications device according to the specifications in a device-control block (a DCB structure). 
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="lpDCB">[in] Pointer to a DCB structure that contains the configuration information for the specified communications device.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/setcommstate.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool SetCommState(IntPtr hFile, [In] ref Dcb lpDCB);

    /// <summary>
    /// The SetCommTimeouts function sets the time-out parameters for all read and write operations on a specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="lpCommTimeouts">[in] Pointer to a COMMTIMEOUTS structure that contains the new time-out values.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/setcommtimeouts.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool SetCommTimeouts(IntPtr hFile, [In] ref CommTimeouts lpCommTimeouts);

    /// <summary>
    /// The SetupComm function initializes the communications parameters for a specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle. <see cref="CreateFile"/></param>
    /// <param name="dwInQueue">[in] Recommended size of the device's internal input buffer, in bytes.</param>
    /// <param name="dwOutQueue">[in] Recommended size of the device's internal output buffer, in bytes.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/setupcomm.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool SetupComm(IntPtr hFile, UInt32 dwInQueue, UInt32 dwOutQueue);

    /// <summary>
    /// The WriteFile function writes data to a file at the position specified by the file pointer. This function is designed for both synchronous and asynchronous operation.
    /// </summary>
    /// <param name="fFile">[in] Handle to the file. The file handle must have been created with the GENERIC_WRITE access right.</param>
    /// <param name="lpBuffer">[in] Pointer to the buffer containing the data to be written to the file.</param>
    /// <param name="nNumberOfBytesToWrite">[in] Number of bytes to be written to the file.</param>
    /// <param name="lpNumberOfBytesWritten">[out] Pointer to the variable that receives the number of bytes written. WriteFile sets this value to zero before doing any work or error checking.</param>
    /// <param name="lpOverlapped">[in] Pointer to an OVERLAPPED structure.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/fileio/base/writefile.asp</remarks>
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool WriteFile(IntPtr hFile, byte [] lpBuffer,uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
      [In] ref System.Threading.NativeOverlapped lpOverlapped);


    /// <summary>
    /// The SetCommMask function specifies a set of events to be monitored for a communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="dwEvtMask">[in] Events to be enabled. A value of zero disables all events. <seealso cref="EventMask"/></param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/setcommmask.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool SetCommMask(IntPtr hFile, EventMask dwEvtMask);

    /// <summary>
    /// The WaitCommEvent function waits for an event to occur for a specified communications device. 
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="lpEvtMask">[out] Pointer to a variable that receives a mask indicating the type of event that occurred. If an error occurs, the value is zero. <seealso cref="EventMask"/></param>
    /// <param name="lpOverlapped">[in] Pointer to an OVERLAPPED structure. This structure is required if hFile was opened with FILE_FLAG_OVERLAPPED.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/waitcommevent.asp</remarks>
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool WaitCommEvent(IntPtr hFile, IntPtr lpEvtMask, [In] ref System.Threading.NativeOverlapped lpOverlapped);

    /// <summary>
    /// The PurgeComm function discards all characters from the output or input buffer of a specified communications resource. It can also terminate pending read or write operations on the resource.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications resource. The CreateFile function returns this handle.</param>
    /// <param name="flags">[in] This parameter can be one or more of the following values.</param>
    /// <returns></returns>
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool PurgeComm(IntPtr hFile, PurgeCommFlags dwFlags);

    /// <summary>
    /// The CancelIo function cancels all pending input and output (I/O) operations that were issued by the calling thread for the specified file handle. 
    /// </summary>
    /// <param name="hFile">[in] Handle to a file.</param>
    /// <returns>If the function succeeds, the return value is nonzero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/fileio/base/cancelio.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool CancelIo(IntPtr hFile);
		
    /// <summary>
    /// The ReadFile function reads data from a file, starting at the position indicated by the file pointer. This function is designed for both synchronous and asynchronous operation. 
    /// </summary>
    /// <param name="hFile">[in] Handle to the file to be read. The file handle must have been created with the GENERIC_READ access right.</param>
    /// <param name="lpBuffer">[out] Pointer to the buffer that receives the data read from the file.</param>
    /// <param name="nNumberOfBytesToRead">[in] Number of bytes to be read from the file.</param>
    /// <param name="nNumberOfBytesRead">[out] Pointer to the variable that receives the number of bytes read.</param>
    /// <param name="lpOverlapped">[in] Pointer to an OVERLAPPED structure.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.If the return value is nonzero and the number of bytes read is zero, the file pointer was beyond the current end of the file at the time of the read operation. However, if the file was opened with FILE_FLAG_OVERLAPPED and lpOverlapped is not NULL, the return value is zero and GetLastError returns ERROR_HANDLE_EOF when the file pointer goes beyond the current end of file.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/fileio/base/readfile.asp</remarks>
    [DllImport("kernel32.dll", SetLastError=true)]
    public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, 
      out uint lpNumberOfBytesRead, [In] ref System.Threading.NativeOverlapped lpOverlapped);


    /// <summary>
    /// The TransmitCommChar function transmits a specified character ahead of any pending data in the output buffer of the specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="cChar">[in] Character to be transmitted.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/transmitcommchar.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool TransmitCommChar(IntPtr hFile, Byte cChar);

    /// <summary>
    /// The EscapeCommFunction function directs a specified communications device to perform an extended function.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device. The CreateFile function returns this handle.</param>
    /// <param name="dwFunc">[in] Extended function to be performed. This parameter can be one of the following values. <seealso cref="CommFunction"/></param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/escapecommfunction.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool EscapeCommFunction(IntPtr hFile, CommFunction dwFunc);

    /// <summary>
    /// The GetCommModemStatus function retrieves modem control-register values.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device.</param>
    /// <param name="lpModemStat">[out] Pointer to a variable that specifies the current state of the modem control-register values.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/getcommmodemstatus.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool GetCommModemStatus(IntPtr hFile, out ModemStatus lpModemStat);

    /// <summary>
    /// The GetOverlappedResult function retrieves the results of an overlapped operation on the specified file, named pipe, or communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the file, named pipe, or communications device.</param>
    /// <param name="lpOverlapped">[in] Pointer to an OVERLAPPED structure that was specified when the overlapped operation was started.</param>
    /// <param name="nNumberOfBytesTransferred">[out] Pointer to a variable that receives the number of bytes that were actually transferred by a read or write operation.</param>
    /// <param name="bWait">[in] If this parameter is TRUE, the function does not return until the operation has been completed. </param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>
    /// Windows Me/98/95:  This function works only on communications devices or on files opened using the DeviceIoControl function.<br/>
    /// http://msdn.microsoft.com/library/en-us/dllproc/base/getoverlappedresult.asp
    /// </remarks>
    [DllImport("kernel32.dll", SetLastError=true)]
    private static extern bool GetOverlappedResult(IntPtr hFile, [In] ref NativeOverlapped lpOverlapped, 
      out int nNumberOfBytesTransferred, bool bWait);

    /// <summary>
    /// The ClearCommError function retrieves information about a communications error and reports the current status of a communications device. 
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device.</param>
    /// <param name="lpErrors">[out] Pointer to a variable to be filled with a mask indicating the type of error.</param>
    /// <param name="CommunicationStatus">[out] Pointer to a COMSTAT structure in which the device's status information is returned. If this parameter is NULL, no status information is returned.</param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.<seealso cref="CommStatus"/></returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/clearcommerror.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool ClearCommError(IntPtr hFile, out CommErrors lpErrors, out CommunicationStatus cs);

    /// <summary>
    /// The GetCommProperties function retrieves information about the communications properties for a specified communications device.
    /// </summary>
    /// <param name="hFile">[in] Handle to the communications device.</param>
    /// <param name="cp">[out] Pointer to a COMMPROP structure in which the communications properties information is returned.<seealso cref="CommunicationProperties"/></param>
    /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. </returns>
    /// <remarks>http://msdn.microsoft.com/library/en-us/devio/base/getcommproperties.asp</remarks>
    [DllImport("kernel32.dll")]
    private static extern bool GetCommProperties(IntPtr hFile, out CommunicationProperties cp);
    #endregion
    #region Rs232Status Subclass
    
    /// <summary>
    /// Hosts the Rs232 Status thread and any pertinent information provided by it
    /// </summary>
    public class Rs232Status : IDisposable
    {
      #region Private Variables
      /// <summary>
      /// Monitors the underlying Win32 event handle that fires when the status of the port changes
      /// </summary>
      public AutoResetEvent m_StatusChangeEvent = new AutoResetEvent(false);

      /// <summary>
      /// Provides a global handle to the Overlapped object created upon instanciation
      /// </summary>
      private NativeOverlapped m_Overlapped;

      /// <summary>
      /// Contains the last event recevied from the Win32 event as a Pointer to an unmanaged uint. It's stored globally so the variable doesn't have to be recreated every time the event fires.
      /// </summary>
      private IntPtr m_UnmanagedCurrentEvent;

      /// <summary>
      /// Contains the last event recevied from the Win32 event. It's stored globally so the variable doesn't have to be recreated every time the event fires.
      /// </summary>
      private EventMask m_CurrentEvent;

      /// <summary>
      /// Synchronizes the 
      /// </summary>
      private ManualResetEvent m_InitSync = new ManualResetEvent(false);

      /// <summary>
      /// Contains a reference to the thread monitoring the status of the Line.
      /// </summary>
      private Thread m_StatusThread;

      #endregion
      #region Event Declarations
      /*
      public delegate void CharacterReceivedEvent(object sender, EventArgs e);
      public delegate void BreakEncounteredEvent(object sender, EventArgs e);
      public delegate void LineErrorEvent(object sender, EventArgs e);
      public delegate void CtsStateChangedEvent(object sender, EventArgs e);
      public delegate void DsrStateChangedEvent(object sender, EventArgs e);
      public delegate void RingEvent(object sender, EventArgs e);
      public delegate void RlsdStateChangedEvent(object sender, EventArgs e);
      public delegate void ReceiveBufferWarningEvent(object sender, EventArgs e);
      public delegate void EventCharacterReceviedEvent(object sender, EventArgs e);
      public delegate void TransmitCompleteEvent(object sender, EventArgs e);
      */

      public event EventHandler CharacterReceived;
      public event EventHandler BreakEncountered;
      public event EventHandler LineError;
      public event EventHandler CtsStateChanged;
      public event EventHandler DsrStateChanged;
      public event EventHandler Ring;
      public event EventHandler RlsdStateChanged;
      public event EventHandler ReceiveBufferWarning;
      public event EventHandler EventCharacterRecevied;
      public event EventHandler TransmitComplete;
      #endregion
      #region Private Property Declarations
      private bool p_DTR;
      private bool p_RTS;
      private long p_BytesReceived = 0;
      private long p_BytesSent = 0;
      private bool p_DSR;
      private bool p_CTS;
      private bool p_Ring;
      private bool p_RLSD;
      private bool p_Enabled = false;
      private Rs232Stream p_Parent;
      #endregion
      #region Public Property Declarations
      public bool DTR
      {
        get{return p_DTR;}
        set
        {
          if(p_DTR==value)
            return;

          if(value)
            EscapeCommFunction(p_Parent.hPort,CommFunction.SETDTR);
          else
            EscapeCommFunction(p_Parent.hPort,CommFunction.CLRDTR);

          p_DTR=value;
        }
      }

      public bool RTS
      {
        get{return p_RTS;}
        set
        {
          if(p_RTS==value)
            return;

          if(value)
            EscapeCommFunction(p_Parent.hPort,CommFunction.SETRTS);
          else
            EscapeCommFunction(p_Parent.hPort,CommFunction.CLRRTS);

          p_RTS=value;
        }
      }

      /// <summary>
      /// Current state of the Ring flag
      /// </summary>
      public bool RingState
      {
        get
        {
          throw new NotSupportedException("Win32 calls do not support checking the state of the RING flag");
        }
      }

      /// <summary>
      /// Current state of RLSD
      /// </summary>
      public bool RLSD
      {
        get{return p_RLSD;}
      }
      
      /// <summary>
      /// Gets or sets the object to Actively Monitor the port. The port must be open and initialized to enable active monitoring.
      /// </summary>
      public bool Enabled
      {
        /*
        get{return m_StatusThread!=null;}
        set
        {
          if(value)
          {
            if(m_StatusThread==null)
            {
              m_StatusThread = new Thread(new ThreadStart(this.RecursiveRequery));
              m_StatusThread.Name = "Rs232 Status Thread";
              m_StatusThread.Priority = ThreadPriority.AboveNormal;
              m_StatusThread.Start();
            }
            else
              throw new InvalidOperationException("Port is already being Monitored.");
          }
          else
          {
            if(m_StatusThread!=null)
            {
              m_StatusThread.Abort();
              if(!m_StatusThread.Join(5000))
                throw new Exception("Unable to stop Port Monitoring. Thread did not abort in the given time (5 Seconds).");
              m_StatusThread = null;
            }
            else
              throw new InvalidOperationException("Port Monitor is already disabled");
          }
        }
        */
        get{return p_Enabled;}
        set
        {
          if(value!=p_Enabled)
          {
            if(value)
            {
              UpdateLines();

              if(m_StatusThread!=null)
                throw new Exception("Cannot start status thread: old status thread already exists");

              m_StatusThread = new Thread(new ThreadStart(Monitor));
              m_StatusThread.IsBackground = true;
              m_StatusThread.Name = "RS232Stream Status Monitor";
              m_StatusThread.Priority = ThreadPriority.Normal;
              m_StatusThread.Start();
            }
            else
            {
              lock(this)
              {
                p_Enabled = false;
                Console.Out.WriteLine("ABORT!");
                m_StatusThread.Abort();
                if(!PurgeComm(p_Parent.hPort,PurgeCommFlags.AbortReceive|PurgeCommFlags.AbortTransmit|PurgeCommFlags.PurgeReceiveBuffer|PurgeCommFlags.PurgeTransmitBuffer))
                  throw new Win32Exception("Unable to purge comm");
                if(!CancelIo(p_Parent.hPort))
                  throw new Win32Exception("Unable to cancel comm");
                if(m_StatusThread.IsAlive)
                  if(!m_StatusThread.Join(10000))
                    if(m_StatusThread.IsAlive)
                      throw new Exception("Unable to stop Port Monitoring. Thread did not abort in the given time (10 Seconds).");
                m_StatusThread=null;
              }
            }
            p_Enabled = value;
          }
        }
      }

      /// <summary>
      /// The number of total bytes received
      /// </summary>
      public long BytesReceived
      {
        get{return p_BytesReceived;}
      }

      /// <summary>
      /// The number of total bytes sent
      /// </summary>
      public long BytesSent
      {
        get{return p_BytesSent;}
      }

      /// <summary>
      /// Handle to the original port
      /// </summary>
//      public IntPtr PortHandle
//      {
//        get{return p_PortHandle;}
//        set{p_PortHandle = value;}
//      }

      /// <summary>
      /// State of the Clear To Send Pin
      /// </summary>
      public bool CTS
      {
        get{return p_CTS;}
      }

      /// <summary>
      /// State of the Data Transmit Ready Pin
      /// </summary>
      public bool DSR
      {
        get{return p_DSR;}
      }
      #endregion
      #region Event Override Methods
      public virtual void OnRing(EventArgs e)
      {
        if(Ring!=null)
          Ring(this,e);

        Console.Out.WriteLine(">Ring");
      }

      public virtual void OnLineError(EventArgs e)
      {
        if(LineError!=null)
          LineError(this,e);

        Console.Out.WriteLine(">Line Error");
      }

      public virtual void OnReceiveBufferWarning(EventArgs e)
      {
        if(ReceiveBufferWarning!=null)
          ReceiveBufferWarning(this,e);

        Console.Out.WriteLine(">Rx Buffer Warning");
      }

      public virtual void OnBreakEncountered(EventArgs e)
      {
        if(BreakEncountered!=null)
          BreakEncountered(this,e);
      }

      public virtual void OnCharacterReceived(EventArgs e)
      {
        if(CharacterReceived!=null)
          CharacterReceived(this,e);
      }

      public virtual void OnCtsStateChanged(EventArgs e)
      {
        if(CtsStateChanged!=null)
          CtsStateChanged(this,e);
      }

      public virtual void OnDsrStateChanged(EventArgs e)
      {
        if(DsrStateChanged!=null)
          DsrStateChanged(this,e);
      }

      public virtual void OnEventCharacterReceived(EventArgs e)
      {
        if(EventCharacterRecevied!=null)
          EventCharacterRecevied(this,e);
      }

      public virtual void OnTransmitComplete(EventArgs e)
      {
        if(TransmitComplete!=null)
          TransmitComplete(this,e);
      }

      public virtual void OnRlsdStateChanged(EventArgs e)
      {
        if(RlsdStateChanged!=null)
          RlsdStateChanged(this,e);
      }
      #endregion

      #region ctors
      public Rs232Status(Rs232Stream parent)
      {
        p_Parent = parent;

        m_Overlapped = new NativeOverlapped();
        m_Overlapped.EventHandle = m_StatusChangeEvent.Handle;

        m_UnmanagedCurrentEvent = Marshal.AllocHGlobal(Marshal.SizeOf(Type.GetType("System.UInt32")));
        Marshal.WriteInt32(m_UnmanagedCurrentEvent,0);
        
        //-----------> Open another handle to the port so we can access it at the same time as the main class.
//        p_PortHandle = CreateFile(p_Parent.Port, DesiredAccess.GENERIC_READ, 
//          FileShare.None, IntPtr.Zero, FileCreationDispostion.OPEN_EXISTING, 
//          FILE_FLAG_OVERLAPPED, IntPtr.Zero);
//
//        if (p_PortHandle == (IntPtr)INVALID_HANDLE_VALUE)
//          throw new IOException("Error Opening Port",new Win32Exception());

        //m_MonitorInterval = interval;
      }
      #endregion
      #region Public Methods
      public void Requery(IntPtr portHandle)
      {
        int ovread = 0;
        TimeSpan timeout = new TimeSpan(0,0,3);
        //The WaitCommEvent, called in the status loop, blocks until an event occurs or an error. This
        //Will determine if it was a Win32 error or an IO event that caused the event to unblock. 
        //This is a non-blocked check. If the check blocks and the thread is aborted, GetOverlappedResult 
        //may never return
        DateTime start = DateTime.Now;
        while(!GetOverlappedResult(p_Parent.hPort,ref m_Overlapped,out ovread,false))
          if(Win32Exception.Check(false)!=NativeError.ERROR_IO_PENDING&&Win32Exception.Check(false)!=NativeError.ERROR_IO_INCOMPLETE)
            throw new IOException("Error occurred getting event mask",new Win32Exception());
          else if((DateTime.Now - start)>timeout)
            return;
          else
            Thread.Sleep(0);  //Forfeit the rest of this quantum to give the I/O a chance to complete.

        EventMask currentevent = 0;

        //-------------> Handle Errors
        m_CurrentEvent = (EventMask) Marshal.ReadInt32(m_UnmanagedCurrentEvent);
        if((m_CurrentEvent&EventMask.EV_ERR)!=0)
        {
          CommErrors errors;
          CommunicationStatus cs;
          while(!ClearCommError(p_Parent.hPort, out errors, out cs)) //Was Using PortHandle
          {
            if(Win32Exception.Check(false)==NativeError.ERROR_IO_PENDING)
              continue;

            if(errors==CommErrors.CE_BREAK)
            {
              if((int) errors == 0)
                currentevent |= EventMask.EV_BREAK;
            }
            else
              throw new IOException("Unable to clear COM Error.", new Win32Exception());
          }
        }


        //-----> Check the state of the lines
        bool ctschanged = false;
        bool dsrchanged = false;
        bool ringchanged = false;
        bool rlsdchanged = false;
        //----------------> Fire events
        if((m_CurrentEvent&EventMask.EV_CTS)!=0)
          ctschanged=true;
          
        if((m_CurrentEvent&EventMask.EV_DSR)!=0)
          dsrchanged=true;
          
        if((m_CurrentEvent&EventMask.EV_RING)!=0)
          ringchanged=true;

        if((m_CurrentEvent&EventMask.EV_RLSD)!=0)
          rlsdchanged=true;

        if(ctschanged||dsrchanged||ringchanged||rlsdchanged)
          UpdateLines();

        if(ctschanged)
          OnCtsStateChanged(EventArgs.Empty);

        if(rlsdchanged)
          OnRlsdStateChanged(EventArgs.Empty);

        if(ringchanged)
          OnRing(EventArgs.Empty);

        if(dsrchanged)
          OnDsrStateChanged(EventArgs.Empty);

        
        //-----------> Check for the other events
        if((m_CurrentEvent&EventMask.EV_BREAK)!=0)
          OnBreakEncountered(EventArgs.Empty);

        if((m_CurrentEvent&EventMask.EV_RX80FULL)!=0)
          OnReceiveBufferWarning(EventArgs.Empty);

        if((m_CurrentEvent&EventMask.EV_RXCHAR)!=0)
          OnCharacterReceived(EventArgs.Empty);

        if((m_CurrentEvent&EventMask.EV_RXFLAG)!=0)
          OnEventCharacterReceived(EventArgs.Empty);

        if((m_CurrentEvent&EventMask.EV_TXEMPTY)!=0)
          OnTransmitComplete(EventArgs.Empty);


        //HACK: For whatever reason we have to make another call to ensure our overlapped struct is free to use again.
        while(!GetOverlappedResult(p_Parent.hPort,ref m_Overlapped,out ovread,false))
          if(Win32Exception.Check(false)!=NativeError.ERROR_IO_PENDING)
            throw new IOException("Error occurred getting event mask",new Win32Exception());
      }

      /// <summary>
      /// Monitors the Serial Port opened by the parent stream for events
      /// </summary>
      /// <remarks>
      /// Sits directly on the m_StatusThread
      /// <see cref="Parent"/>
      /// <see cref="m_StatusThread"/>
      /// </remarks>
      private void Monitor()
      {
        InitEventMask();
        int test = 0;
        TimeSpan timeout = new TimeSpan(0,0,2);

        while(true)
        {
          try
          {
            if(!WaitCommEvent(p_Parent.hPort, m_UnmanagedCurrentEvent, ref m_Overlapped))
            {
              NativeError err = Win32Exception.Check(false);
              if (err!= NativeError.ERROR_IO_PENDING) 
                throw new IOException("Error setting comm event handle", new Win32Exception());
            }

            test++;
          
            while(!m_StatusChangeEvent.WaitOne(timeout,false))
              Thread.Sleep(0);

            if(p_Enabled)
              Requery(p_Parent.hPort);
          }
          catch(ThreadAbortException)
          {
            return;
          }
        }
      }

      private void InitEventMask()
      {
        const EventMask ev = EventMask.EV_RXCHAR | EventMask.EV_TXEMPTY | 
                EventMask.EV_CTS | EventMask.EV_DSR | EventMask.EV_BREAK | EventMask.EV_RLSD | 
                EventMask.EV_RING | EventMask.EV_ERR;

        InitEventMask(ev);
      }

      private void InitEventMask(EventMask ev)
      {
        //------------------> Init the Event Mask
        while(!SetCommMask(p_Parent.hPort, ev))
        {
          NativeError err = Win32Exception.Check(false);
          if(err!=NativeError.ERROR_IO_PENDING&&err!=NativeError.ERROR_IO_INCOMPLETE&&err!=NativeError.ERROR_SUCCESS)
            throw new IOException("Error resetting event mask for port",new Win32Exception());
        }
      }

      /// <summary>
      /// Checks the status of the Flow Control Lines
      /// </summary>
      /// <param name="portHandle">Handle to the opened port</param>
      /// <remarks>Called by Requery</remarks>
      public void UpdateLines()
      {
        ModemStatus ms;
        while(!GetCommModemStatus(p_Parent.hPort,out ms))
          if(Win32Exception.Check(false)==NativeError.ERROR_IO_PENDING)
            continue;
          else
            throw new Win32Exception("Error during GetCommModemStatus");

        if(((ms&ModemStatus.MS_CTS_ON)!=0)!=p_CTS)
          p_CTS = !p_CTS;

        if(((ms&ModemStatus.MS_DSR_ON)!=0)!=p_DSR)
          p_DSR = !p_DSR;

        if(((ms&ModemStatus.MS_RING_ON)!=0)!=p_Ring)
          p_Ring = !p_Ring;

        if(((ms&ModemStatus.MS_RLSD_ON)!=0)!=p_RLSD)
          p_RLSD = !p_RLSD;

        Console.Out.WriteLine("======== LINE STATUS =======");
        Console.Out.WriteLine("  CTS     DSR    RING    CD");
        Console.Out.WriteLine(String.Format("  {0}     {1}    {2}     {3}\r\n",new object[]
        {
          p_CTS?"ON ":"OFF",
          p_DSR?"ON ":"OFF",
          p_Ring?"ON ":"OFF",
          p_RLSD?"ON ":"OFF"
        }));
        Console.Out.WriteLine(ms.ToString());
      }
      #endregion
      #region IDisposable Members

      public void Dispose()
      {
        if(m_UnmanagedCurrentEvent!=IntPtr.Zero)
          Marshal.FreeHGlobal(m_UnmanagedCurrentEvent);

        m_UnmanagedCurrentEvent = IntPtr.Zero;
      }

      #endregion
    }
    #endregion
    #region IDisposable Members
    public void Dispose()
    {
      if(hPort!=IntPtr.Zero)
      {
        if(!CancelIo(hPort))
          throw new IOException("Unable to cancel IO operations on port.", new Win32Exception());

        if(!CloseHandle(hPort))
          throw new IOException("Unable to free port resources.", new Win32Exception());
      }
    }
    #endregion
  }
}

