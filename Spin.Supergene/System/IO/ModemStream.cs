using System;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;



namespace System.IO
{
  #region Enumerations
  /// <summary>
  /// Specifies the current state of the modem
  /// </summary>
  public enum ModemConnectionStage
  {
    /// <summary>The modem is in an idle, uninitialized state</summary>
    Idle,
    
    /// <summary>Sending init strings</summary>
    Initializing,
    
    /// <summary>Waiting for Carrier</summary>
    Dialing,
    
    /// <summary>The modem is connected to it's destination and is active</summary>
    Active,
    
    /// <summary>
    /// A result code was received from the modem indicating a connection error. <seealso cref="LastResultCode"/>
    /// </summary>
    ConnectionError
  }

  /// <summary>
  /// Used by the GetResultCodeType function to determine the nature of a result code.
  /// </summary>
  public enum ResultCodeType
  {
    /// <summary>An acceptable status change from the modem, such as OK or RINGING</summary>
    Status,
    
    /// <summary>A recoverable error, such as the line being busy.</summary>
    Error,
    
    /// <summary>An error that will prevent the modem from making any more connections.</summary>
    FatalError,

    /// <summary>The modem has connected to the remote host.</summary>
    Connect
  }

  /// <summary>
  /// An enumeration of valid result codes that can be returned by Modems.
  /// </summary>
  public enum ResultCode : int
  {
    OK            = 0,
    CONNECT       = 1,
    RING          = 2,
    NOCARRIER     = 3,
    ERROR         = 4,
    CONNECT1200   = 5,
    NODIALTONE    = 6,
    BUSY          = 7,
    NOANSWER      = 8,
    RESERVED      = 9,
    CONNECT2400   = 10,
    RINGING       = 11,
    VOICE         = 12,
    CONNECT9600   = 13,
    CONNECT4800   = 18,
    CONNECT7200   = 29,
    CONNECT12000  = 21,
    CONNECT14400  = 25,
    CONNECT16800  = 43,
    CONNECT19200  = 85,
    CONNECT21600  = 91,
    CONNECT24000  = 99,
    CONNECT26400  = 103,
    CONNECT28800  = 107,
    CONNECT31200  = 151,
    CONNECT33600  = 155,
    CONNECT33333  = 180,
    CONNECT37333  = 184,
    CONNECT41333  = 188,
    CONNECT42666  = 192,
    CONNECT44000  = 196,
    CONNECT45333  = 200,
    CONNECT46666  = 204,
    CONNECT48000  = 208,
    CONNECT49333  = 212,
    CONNECT50666  = 216,
    CONNECT52000  = 220,
    CONNECT53333  = 224,
    CONNECT54666  = 228,
    CONNECT56000  = 232,
    CONNECT57333        = 236,
    CONNECT28000		    = 256,
    CONNECT28000ARQ		  = 257,
    CONNECT28000V90		  = 258,
    CONNECT28000ARQV90	= 259,
    CONNECT29333		    = 260,
    CONNECT29333ARQ		  = 261,
    CONNECT29333V90		  = 262,
    CONNECT29333ARQV90	= 263,
    CONNECT30666		    = 264,
    CONNECT30666ARQ		  = 265,
    CONNECT30666V90		  = 266,
    CONNECT30666ARQV90	= 267,
    CONNECT32000		    = 268,
    CONNECT32000ARQ		  = 269,
    CONNECT32000V90		  = 270,
    CONNECT32000ARQV90	= 271,
    CONNECT34666		    = 272,
    CONNECT34666ARQ		  = 273,
    CONNECT34666V90		  = 274,
    CONNECT34666ARQV90	= 275,
    CONNECT36000		    = 276,
    CONNECT36000ARQ		  = 277,
    CONNECT36000V90		  = 278,
    CONNECT36000ARQV90	= 279,
    CONNECT38666		    = 280,
    CONNECT38666ARQ		  = 281,
    CONNECT38666V90		  = 282,
    CONNECT53333V90		  = 310,
    CONNECT53333ARQV90	= 311,
    CONNECT54666V90		  = 312,
    CONNECT54666ARQV90	= 313,
    CONNECT56000V90		  = 314,
    CONNECT56000ARQV90	= 315,
    CONNECT57333V90		  = 316,
    CONNECT57333ARQV90	= 317,
    CONNECT58666		    = 318,
    CONNECT58666ARQ		  = 319,
    CONNECT58666V90		  = 320,
    CONNECT58666ARQV90	= 321,
    CONNECT60000		    = 322,
    CONNECT38666ARQV90	= 283,
    CONNECT40000		    = 284,
    CONNECT40000ARQ		  = 285,
    CONNECT40000V90		  = 286,
    CONNECT40000ARQV90	= 287,
    CONNECT33333V90		  = 288,
    CONNECT33333ARQV90	= 289,
    CONNECT37333V90		  = 290,
    CONNECT37333ARQV90	= 291,
    CONNECT41333V90		  = 292,
    CONNECT41333ARQV90	= 293,
    CONNECT42666V90		  = 294,
    CONNECT42666ARQV90	= 295,
    CONNECT44000V90		  = 296,
    CONNECT44000ARQV90	= 297,
    CONNECT45333V90		  = 298,
    CONNECT45333ARQV90	= 299,
    CONNECT46666V90		  = 300,
    CONNECT46666ARQV90	= 301,
    CONNECT48000V90		  = 302,
    CONNECT48000ARQV90	= 303,
    CONNECT49333V90		  = 304,
    CONNECT49333ARQV90	= 305,
    CONNECT50666V90		  = 306,
    CONNECT50666ARQV90	= 307,
    CONNECT52000V90		  = 308,
    CONNECT52000ARQV90	= 309,
    CONNECT60000ARQ		  = 323,
    CONNECT60000V90		  = 324,
    CONNECT60000ARQV90	= 325,
    CONNECT61333		    = 326,
    CONNECT61333ARQ		  = 327,
    CONNECT61333V90		  = 328,
    CONNECT61333ARQV90	= 329,
    CONNECT62666		    = 330,
    CONNECT62666ARQ		  = 331,
    CONNECT62666V90		  = 332,
    CONNECT62666ARQV90	= 333,
    CONNECT64000V90		  = 334,
    CONNECT64000ARQV90	= 335,
    CONNECT25333		    = 336,
    CONNECT25333ARQ		  = 337,
    CONNECT26666		    = 338,
    CONNECT26666ARQ		  = 339,
    CONNECT31666		    = 340,
    CONNECT31666ARQ		  = 341,
    CONNECT24000V92		  = 342,
    CONNECT24000ARQV92	= 343,
    CONNECT25333V92		  = 344,
    CONNECT25333ARQV92	= 345,
    CONNECT26666V92		  = 346,
    CONNECT26666ARQV92	= 347,
    CONNECT28000V92		  = 348,
    CONNECT28000ARQV92	= 349,
    CONNECT29333V92		  = 350,
    CONNECT29333ARQV92	= 351,
    CONNECT30666V92		  = 352,
    CONNECT30666ARQV92	= 353,
    CONNECT32000V92		  = 354,
    CONNECT32000ARQV92	= 355,
    CONNECT33333V92		  = 356,
    CONNECT33333ARQV92	= 357,
    CONNECT34666V92		  = 358,
    CONNECT34666ARQV92	= 359,
    CONNECT36000V92		  = 360,
    CONNECT36000ARQV92	= 361,
    CONNECT37333V92		  = 362,
    CONNECT37333ARQV92	= 363,
    CONNECT38666V92		  = 364,
    CONNECT38666ARQV92	= 365,
    CONNECT40000V92		  = 366,
    CONNECT40000ARQV92	= 367,
    CONNECT41333V92		  = 368,
    CONNECT41333ARQV92	= 369,
    CONNECT42666V92		  = 370,
    CONNECT42666ARQV92	= 371,
    CONNECT44000V92		  = 372,
    CONNECT44000ARQV92	= 373,
    CONNECT45333V92		  = 374,
    CONNECT45333ARQV92	= 375,	
    CONNECT46666V92		  = 376,
    CONNECT46666ARQV92	= 377,
    CONNECT48000V92		  = 378,
    CONNECT48000ARQV92	= 379,
    CONNECT49333V92		  = 380,
    CONNECT49333ARQV92	= 381,
    CONNECT50666V92		  = 382,
    CONNECT50666ARQV92	= 383,
    CONNECT52000V92		  = 384,
    CONNECT52000ARQV92	= 385,
    CONNECT53333V92		  = 386,
    CONNECT53333ARQV92	= 387,
    CONNECT54666V92		  = 388,
    CONNECT54666ARQV92	= 389,
    CONNECT56000V92		  = 390,
    CONNECT56000ARQV92	= 391,
    CONNECT9600ARQV34LAPM = 134,
    Unknown = 999
}
  #endregion

	/// <summary>
	/// A connection encapsulating the functionality required to use a modem.
	/// </summary>
  public class ModemStream : Rs232Stream
  {
    public struct ModemCommandResult
    {
      public ModemCommandResult(string result)
      {
        Result = result;

        try
        {
          ResultCode = (ResultCode) Enum.Parse(typeof(ResultCode),result.Replace("/",""),true);
        } 
        catch(System.ArgumentException)
        {
          ResultCode = ResultCode.Unknown;
        }
  
        ResultCodeType = ResultCodeType.Connect;
        ResultCodeType = GetResultCodeType(ResultCode);
      }

      private ResultCodeType GetResultCodeType(ResultCode RC)
      {
        int rc = (int) RC;
        if(rc>=13||rc==5||rc==19||rc==12)
          return ResultCodeType.Connect;

        if(rc==0||rc==2||rc==11)
          return ResultCodeType.Status;

        if(rc==3||rc==7||rc==8)
          return ResultCodeType.Error;

        if(rc==4||rc==6)
          return ResultCodeType.FatalError;

        throw new Exception("Cannot get Result Code Type for unknown Result Code. Please map the Reuslt code to an appropriate type.");
      }

      public string Result;
      public ResultCode ResultCode;
      public ResultCodeType ResultCodeType;
    }
    private const int MaximumResultCodeSize = 30;
    #region Event Declarations
    public delegate void ConnectionStageChangeEvent(object sender, EventArgs e);
    public delegate void ResultCodeReceivedEvent(object sender, ResultCodeReceivedEventArgs e);
    public delegate void RingEvent(object sender, EventArgs e);

    public event ConnectionStageChangeEvent ConnectionStageChanged;
    public event ResultCodeReceivedEvent ResultCodeReceived;
    public event RingEvent Ring;
    #endregion
    #region Local Variables
    /// <summary>If the modem is possibly sending a response during an xfer, we set this to true</summary>
    //bool ModemSignal;       

    /// <summary>Stores the command the modem sent during xfer. <seealso cref="ModemSignal"/></summary>
    //string ModemCommand;  

    /// <summary>This is used to filter out characters received by the modem so that the result code can be easily cast to the ResultCodes enumeration <seealso cref="ResultCodes"/></summary>
    Regex CommandFilter = new Regex(@"[\/\s\r\n]+",RegexOptions.Compiled);

    #endregion
    #region Local Property Declarations
    /// <summary>Local Property Variable. <see cref="ConnectionStage"/></summary>
    private ModemConnectionStage p_ConnectionStage = ModemConnectionStage.Idle;
    
    /// <summary>Local Property Variable. <see cref="ModemID"/></summary>
    private int p_ModemID;
    
    /// <summary>Local Property Variable. <see cref="Prefix"/></summary>
    private string p_Prefix = "";
    
    /// <summary>Local Property Variable. <see cref="ConnectionStrings"/></summary>
    private string[] p_ConnectionStrings;

    /// <summary>Local Property Variable. <see cref="LastResultCode"/></summary>
    private ModemCommandResult p_LastResult;

    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The state of the connection for the modem. The class considers all signals from the underlying connection to be from the modem until the stage is Active. <seealso cref="ModemConnectionStage"/>
    /// </summary>
    public ModemConnectionStage ConnectionStage
    {
      get{return p_ConnectionStage;}
    }

    /// <summary>
    /// The prefix to be used when dialing.
    /// </summary>
    public string Prefix
    {
      get{return p_Prefix;}
      set{p_Prefix = value;}
    }

    /// <summary>
    /// Returns the connection strings used to initialize the modem.
    /// </summary>
    public string[] ConnectionStrings
    {
      get{return p_ConnectionStrings;}
      set{p_ConnectionStrings = value;}
    }

    /// <summary>
    /// The database ID representing the modem. Currently only being used for connection strings.
    /// </summary>
    public int ModemID
    {
      get{return p_ModemID;}
      set
      {
        p_ModemID = value;
        p_ConnectionStrings = GetConnectionStrings();
      }
    }

    /// <summary>
    /// Returns the last group of result codes received by the modem.
    /// </summary>
    public ModemCommandResult LastResult
    {
      get{return p_LastResult;}
    }
    #endregion
    #region ctors
    /// <summary>
    /// Connection Speed and the ConnectionString must both be set before the connection can be used
    /// </summary>
    /// <param name="innerStream">The underlying stream used to access the modem</param>
    /// <param name="ModemID">The database ID of the modem to be used</param>
    /// <param name="Prefix">The prefix to be used when dialing</param>
    public ModemStream(string port, int baudRate, int modemID, string prefix) : base(port, baudRate, Rs232Handshake.DsrDtr)
    {
      p_ModemID = modemID;
      p_Prefix = prefix;

      p_ConnectionStrings = GetConnectionStrings();
    }

    /// <summary>
    /// Connection Speed and the ConnectionString must both be set before the connection can be used
    /// </summary>
    /// <param name="ModemID">The database ID of the modem to be used</param>
    public ModemStream(string port, int baudRate, int modemID) : this(port, baudRate, modemID, null)
    {
    }
    #endregion
    #region overrides

    public void Dial(string phoneNumber)
    {
      #region Validation
      if(phoneNumber==null)
        throw new ArgumentNullException("phoneNumber");
      if(phoneNumber==String.Empty)
        throw new ArgumentException("Phone number must not be empty");
      #endregion
      SetStage(ModemConnectionStage.Initializing);

      Regex rex = new Regex(@"[^\d]*");          //Used to parse out any non-numeric chars
      string actualconnstr = rex.Replace(phoneNumber,"");

      try
      {
        foreach(string s in p_ConnectionStrings)  //Initialize the modem for communications
          Send(s,new TimeSpan(0,0,0,0,500),ResultCode.OK);
      }
      catch(Exception ex)
      {
        base.Close();
        throw new IOException("Unable to initialize modem.",ex);
      }

      p_ConnectionStage = ModemConnectionStage.Dialing;
      try
      {
        Send("ATDT" + p_Prefix + actualconnstr,new TimeSpan(0,0,0,30),ResultCodeType.Connect);
      }
      catch(TimeoutException ex)
      {
        throw new IOException("Remote Modem did not answer.",ex);
      }

      Thread.Sleep(1000);
      
      SetStage(ModemConnectionStage.Active);
    }

    /// <summary>
    /// Frees resources and closes the connection
    /// </summary>
    public override void Close()
    {
      base.Close ();
      Thread.Sleep(500);  //Let the modem re-initialize
    }

    #endregion
    #region Private Functions

    /// <summary>
    /// Sends an initialization string to the modem.
    /// </summary>
    /// <param name="ToSend">The string to send to the Modem</param>
    protected void Send(string toSend)
    {
      byte[] tmp = System.Text.ASCIIEncoding.ASCII.GetBytes(toSend);
      byte[] ba = new byte[tmp.Length+2];
      tmp.CopyTo(ba,0);
      ba[ba.Length-2] = (byte)ASCII.CR;
      ba[ba.Length-1] = (byte)ASCII.LF;
      base.Write(ba,0,ba.Length);
    }

    
    private ModemCommandResult GetResponse(TimeSpan timeout)
    {
      #region Validation
      if(timeout<TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("timeout",timeout,"Timeout cannot be negative");
      #endregion
      byte[] buffer = new byte[MaximumResultCodeSize];
      int numread = 0;
      DateTime started = DateTime.Now;
      bool enforcetimeout = timeout != TimeSpan.Zero;

      //----------- Read From Stream
      while(numread<MaximumResultCodeSize)
      {
        if(enforcetimeout)
          if((DateTime.Now - started) > timeout)
            throw new TimeoutException("Timed out while waiting for response code",timeout);
        
        numread+=Read(buffer,numread,MaximumResultCodeSize-numread);
        if(numread>0)
          if(buffer[numread-1]==(byte) System.Text.ASCII.LF)
            if(buffer[numread-2]==(byte) System.Text.ASCII.CR)
              break;
        Thread.Sleep(10);
      }

      if(numread>MaximumResultCodeSize)
        throw new IOException("Buffer overrun. Modem returned an extended response code (More than " + MaximumResultCodeSize.ToString() + " characters returned)");

      string code = System.Text.ASCIIEncoding.ASCII.GetString(buffer,0,numread).Replace("\r\n","").Replace(" ","");
      ResultCode rc = ResultCode.OK;

      p_LastResult = new ModemCommandResult(code);
      return p_LastResult;
    }

    /// <summary>
    /// Sends information to the modem and blocks until the Expected Response is received. If a result other than the expected response is received, a Fatal exception is thrown.
    /// </summary>
    /// <param name="ToSend">String to send to the modem</param>
    /// <param name="ResponseTimeout">The amount of time, in millisecond, to wait for the modem to respond.</param>
    /// <param name="ExpectedResponse">The response expected as a result of the string sent</param>
    public void Send(string toSend, TimeSpan timeout, ResultCode expectedResponse)
    {
      if(Send(toSend,timeout).ResultCode!=expectedResponse)
        throw new IOException("A string was sent to the modem and a '" + expectedResponse.ToString() + "' response was expected. The response '" + p_LastResult.Result + "' was received");
    }

    /// <summary>
    /// Sends information to the modem and blocks until the Expected Response is received. If a result other than the expected response is received, a Fatal exception is thrown.
    /// </summary>
    /// <param name="ToSend">String to send to the modem</param>
    /// <param name="ResponseTimeout">The amount of time, in millisecond, to wait for the modem to respond.</param>
    /// <param name="ExpectedResponse">The response expected as a result of the string sent</param>
    public void Send(string toSend, TimeSpan timeout, ResultCodeType expectedResponse)
    {
      if(Send(toSend,timeout).ResultCodeType!=expectedResponse)
        throw new IOException("A string was sent to the modem and a '" + expectedResponse.ToString() + "' response was expected. The response '" + p_LastResult.Result + "' was received");
    }

    /// <summary>
    /// Sends information to the modem and blocks until the Expected Response is received. If a result other than the expected response is received, a Fatal exception is thrown.
    /// </summary>
    /// <param name="ToSend">String to send to the modem</param>
    /// <param name="ResponseTimeout">The amount of time, in millisecond, to wait for the modem to respond.</param>
    public ModemCommandResult Send(string toSend, TimeSpan timeout)
    {
      Send(toSend);
      return GetResponse(timeout);
    }

    /// <summary>
    /// Obtains an array of connection string used to initialize the modem <seealso cref="ConnectionStrings"/>
    /// </summary>
    private string[] GetConnectionStrings()
    {
      string[] ret = new string[4];
      ret[0] = "ATE0Q0V1";
      ret[1] = "AT&F1E0Q0V1&C1&D2&A0S0=0";
      ret[2] = "ATS7=60S19=0M1&M4&K1&H1&R2&I0B0X4";
      ret[3] = "AT";

      return ret;
    }

    /// <summary>
    /// Sets the status if the connection to be read by the accessor, and notifies the event. <seealso cref="ConnectionStageChanged"/>
    /// </summary>
    /// <param name="Stage">The new stage of the modem</param>
    private void SetStage(ModemConnectionStage Stage)
    {
      p_ConnectionStage = Stage;
      OnConnectionStateChanged();
    }
    #endregion
    #region Virtual Event Methods
    protected void OnResultCodeReceived(ResultCodeReceivedEventArgs e)
    {
      p_LastResult.ResultCode = e.ResultCode;
      if(ResultCodeReceived!=null)
        ResultCodeReceived(this,e);
    }

    protected void OnRing()
    {
      if(Ring!=null)
        Ring(this,EventArgs.Empty);
    }

    protected void OnConnectionStateChanged()
    {
      if(ConnectionStageChanged!=null)
        ConnectionStageChanged(this,EventArgs.Empty);
    }
    #endregion
  }

  #region ResultCodeReceivedEventArgs Class
  public class ResultCodeReceivedEventArgs : EventArgs
  {
    #region Private Property Declarations
    /// <summary>Local Property Variable. <see cref="ResultCode"/></summary>
    private ResultCode p_ResultCode;
    
    /// <summary>
    /// Local Property Variable. <see cref="ResultCodeType"/>
    /// </summary>
    private ResultCodeType p_ResultCodeType;
    #endregion
    #region Public Property Declarations

    /// <summary>The result code returned by the modem.</summary>
    public ResultCode ResultCode
    {
      get{return p_ResultCode;}
    }

    
    /// <summary>The type of result code returned by the modem.</summary>
    public ResultCodeType ResultCodeType
    {
      get{return p_ResultCodeType;}
    }
    #endregion
    #region ctors
    public ResultCodeReceivedEventArgs(ResultCode Result, ResultCodeType ResultType)
    {
      p_ResultCode = Result;
      p_ResultCodeType = ResultType;
    }
    #endregion
  }
  #endregion
}
