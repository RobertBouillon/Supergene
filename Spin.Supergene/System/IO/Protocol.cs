using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.IO
{
  #region Public enums
  public enum PacketSegment : int
  {
    /// <summary>
    /// The Packet reader is composing the preamble
    /// </summary>
    Preamble = 1,

    /// <summary>
    /// The packet reader is composing the payload
    /// </summary>
    Payload = 2,

    /// <summary>
    /// The packet reader is composing the Postamble
    /// </summary>
    Postamble = 4,

    /// <summary>
    /// The preamble, payload, and postamble. This is only data that is stored by the packet, not control data mitigated by the protocol.
    /// </summary>
    All = 7
  }
  #endregion
  #region PacketReaderStatus Enum
  /// <summary>
  /// The status of the packet reader.
  /// </summary>
  public enum PacketReaderStatus
  {
    /// <summary>
    /// Memory is being allocated and initialized.
    /// </summary>
    Initializing,

    /// <summary>
    /// The underlying connection is connecting to the data stream.
    /// </summary>
    Connecting,

    /// <summary>
    /// The connection is being authenticated with the stream provider.
    /// </summary>
    Authenticating,

    /// <summary>
    /// The packet reader is connected to the stream and idle.
    /// </summary>
    Connected,

    /// <summary>
    /// The packet readers is sending a request to the stream provider.
    /// </summary>
    SendingRequest,

    /// <summary>
    /// The packet reader is validating a request from the stream.
    /// </summary>
    ConfirmingRequest,

    /// <summary>
    /// The packet reader is sending packets to the stream.
    /// </summary>
    UploadingData,

    /// <summary>
    /// The packet reader is receiving packets from the stream.
    /// </summary>
    DownloadingData,

    /// <summary>
    /// The packet reader is parsing the data received.
    /// </summary>
    SavingData,

    /// <summary>
    /// The underlying stream is being disconnected
    /// </summary>
    Disconnecting,

    /// <summary>
    /// The underlying stream is disconnected and the packet reader is idle.
    /// </summary>
    Disconnected
  }
  #endregion
  #region ValidationErrorType Enum
  /// <summary>
  /// Specifies the type of validation error when an error occurs.
  /// </summary>
  public enum ValidationErrorType
  {
    /// <summary>
    /// The error is obscure.
    /// </summary>
    General,

    /// <summary>
    /// The actual size of the packet did not match the expected size.
    /// </summary>
    Size,

    /// <summary>
    /// The packet checksum was incorrect.
    /// </summary>
    Checksum,

    /// <summary>
    /// There was a parity error over the connection.
    /// </summary>
    Parity
  }
  #endregion
  #region PacketReaderStatusChangeEventArgs Class
  public class PacketReaderStatusChangeEventArgs
  {
    private PacketReaderStatus p_Status;

    public PacketReaderStatusChangeEventArgs(PacketReaderStatus Status)
    {
      p_Status = Status;
    }
  }
  #endregion

  /// <summary>
  /// A class designed to parse a binary stream for Packetized information. Override to abstract the protocol details from packet retrieval.
  /// </summary>
  public abstract class Protocol
  {
    //TODO: Change p_Source to InnerStream when changing to an inherited class
    //TODO: Add support for an unescaped checksum.
    #region Event Declarations
    /// <summary>Custom Delegate for the PacketReceived Event</summary>
    public delegate void PacketReceivedEvent(object sender, PacketReceivedEventArgs e);
    public delegate void PacketSentEvent(object sender, PacketSentEventArgs e);
    public delegate void SendingPacketEvent(object sender, SendingPacketEventArgs e);
    public delegate void ReceivingPacketEvent(object sender, ReceivingPacketEventArgs e);
    public delegate void ValidationErrorEvent(object sender, ValidationErrorEventArgs e);
    public delegate void EscapeCharacterSentEvent(object sender, EscapeCharacterSentEventArgs e);
    public delegate void EscapeCharacterReceivedEvent(object sender, EscapeCharacterReceivedEventArgs e);
    public delegate void ProtocolErrorEvent(object sender, ProtocolErrorEventArgs e);
    public delegate void ErrorReadingPacketEvent(object sender, ErrorReadingPacketEventArgs e);

    /// <summary>
    /// Occurs when a packet validation error is thrown, such as a bad checksum. If this number breaks a certain
    /// threshhold as defined in the parameters in the database, the register will be set to use additional
    /// fault tolerance at the expense of performance, such as parity over the lines, and smaller packet sizes, etc.
    /// </summary>
    public event ValidationErrorEvent ValidationError;
    public event SendingPacketEvent SendingPacket;
    public event ReceivingPacketEvent ReceivingPacket;
    public event PacketSentEvent PacketSent;
    public event PacketReceivedEvent PacketReceived;
    public event EscapeCharacterSentEvent EscapeCharacterSent;
    public event EscapeCharacterReceivedEvent EscapeCharacterReceived;
    public event ProtocolErrorEvent ProtocolError;
    public event ErrorReadingPacketEvent ErrorReadingPacket;
    #endregion
    #region Private Property Declarations
    private Stream    p_Source;
    private char      p_EscapeChar;
    private int       p_BufferSize = 1024;
    private int       p_TransmitRetries = 3;
    private int       p_ReceiveRetries = 3;
    private TimeSpan  p_TransmitRetryDelay = new TimeSpan(0,0,1);
    private TimeSpan  p_ReceiveRetryDelay = new TimeSpan(0,0,1);
    private TimeSpan  p_ReadTimeout = new TimeSpan(0,0,10);
    private int       p_PacketsReceived = 0;
    private int       p_PacketsSent = 0;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The total number of Packets received by the Packet Reader 
    /// </summary>
    public int PacketsReceived
    {
      get{return p_PacketsReceived;}
      set{p_PacketsReceived = value;}
    }

    public int PacketsSent
    {
      get{return p_PacketsSent;}
      set{p_PacketsSent = value;}
    }

    /// <summary>
    /// Amount if time before reading times out. (default: 10 Seconds)
    /// </summary>
    public TimeSpan ReadTimeout 
    {
      get{return p_ReadTimeout;}
      set{p_ReadTimeout = value;}
    }

    /// <summary>
    /// Amount of time to wait before retrying a failed packet transmission (default: 1 Second)
    /// </summary>
    public TimeSpan TransmitRetryDelay
    {
      get{return p_TransmitRetryDelay;}
      set{p_TransmitRetryDelay = value;}
    }

    /// <summary>
    /// Number of times to retry sending a packet if the packet fails to transmit (default: 3)
    /// </summary>
    public int TransmitRetries
    {
      get{return p_TransmitRetries;}
      set{p_TransmitRetries = value;}
    }

    /// <summary>
    /// Amount of time to wait before retrying a failed packet receipt (default: 1 Second)
    /// </summary>
    public TimeSpan ReceiveRetryDelay
    {
      get{return p_ReceiveRetryDelay;}
      set{p_ReceiveRetryDelay = value;}
    }

    /// <summary>
    /// Number of times to retry receiving a packet if the packet fails to transmit (default: 3)
    /// </summary>
    public int ReceiveRetries
    {
      get{return p_ReceiveRetries;}
      set{p_ReceiveRetries = value;}
    }

    /// <summary>
    /// The underlying stream
    /// </summary>
    protected Stream Source
    {
      get{return p_Source;}
    }

    public char EscapeChar
    {
      get{return p_EscapeChar;}
      set{p_EscapeChar = value;}
    }

    /// <summary>
    /// Actively changes the size of the buffer if the Buffer setting is True
    /// </summary>
    public int BufferSize
    {
      get{return p_BufferSize;}
      set{p_BufferSize = value;}
    }
    #endregion

    #region ctors
    /// <summary>
    /// Creates a Packet Reader that encapsulates a stream
    /// </summary>
    /// <param name="stream">The stream on which this protocol will communicate</param>
    protected Protocol(Stream stream)
    {
      p_Source = stream;
      EscapeChar = (char)ASCII.DLE;
    }
    #endregion

    #region Public Methods

    public Packet ReadPacket()
    {
      return ReadPacket(null);
    }

    /// <summary>
    /// Performs a Synchronous Read of the stream and requests the next packet from the source.
    /// </summary>
    /// <param name="bufferSize">The size of buffer to use (default: 1024)</param>
    /// <returns></returns>
    public Packet ReadPacket(Type preambleType)
    {
      int tries = 0;
      Packet currentpacket = CreatePacket();

      do
      {
        if(tries>0)
        {
          if(tries>p_ReceiveRetries)
            throw new PacketException("Could not receive packet due to " + p_ReceiveRetries.ToString() + " validation errors.",currentpacket);
          Thread.Sleep(p_ReceiveRetryDelay);
        }

        try
        {
          //--------> Reset Packet State
          if(preambleType!=null)                    //Override the packet preamble, if necessary
            currentpacket.PreambleType = preambleType;
      
          //--------> Read Packet From Stream
          OnReceivingPacket(new ReceivingPacketEventArgs(currentpacket, PacketSegment.Preamble));
          currentpacket.RawPreamble = Read(Marshal.SizeOf(currentpacket.PreambleType), (EscapedSegments&PacketSegment.Preamble)!=0);
        
          OnReceivingPacket(new ReceivingPacketEventArgs(currentpacket, PacketSegment.Payload));
          if(currentpacket.PayloadType!=null)   //Allow payload-less packets
            currentpacket.RawPayload = Read(currentpacket.GetPayloadSize(),(EscapedSegments&PacketSegment.Payload)!=0);

          OnReceivingPacket(new ReceivingPacketEventArgs(currentpacket, PacketSegment.Postamble));
          currentpacket.RawPostamble = Read(Marshal.SizeOf(currentpacket.PostambleType),(EscapedSegments&PacketSegment.Postamble)!=0);

          int fsz = currentpacket.RawPreamble.Length + currentpacket.RawPostamble.Length;
          if(currentpacket.RawPayload!=null)
            fsz+=currentpacket.RawPayload.Length;

          byte[] fullpacket = new byte[fsz];
          currentpacket.RawPreamble.CopyTo(fullpacket,0);
          if(currentpacket.RawPayload!=null)
          {
            currentpacket.RawPayload.CopyTo(fullpacket,currentpacket.RawPreamble.Length);
            currentpacket.RawPostamble.CopyTo(fullpacket,currentpacket.RawPreamble.Length + currentpacket.RawPayload.Length);
          }
          else
            currentpacket.RawPostamble.CopyTo(fullpacket,currentpacket.RawPreamble.Length);

          currentpacket.RawPacket = fullpacket;
          currentpacket.IsDeconstructed = true;

        }
        catch(ProtocolException ex)
        {
          if(!OnErrorReadingPacket(new ErrorReadingPacketEventArgs(ex,tries)))
            throw ex;
          else
          {
            tries++;
            continue;
          }
        }
        tries++;
      } while(!OnPacketReceived(new PacketReceivedEventArgs(currentpacket)));

      p_PacketsReceived++;
      return currentpacket;
    }

    public virtual void WritePacket(Packet packetToSend)
    {
      int tries = 0;
      byte[] escaped = null;
        
      do
      {
        if(tries!=0)
        {
          if(tries>p_TransmitRetries)
            throw new PacketException("Remote device rejected the packet more than " + p_TransmitRetries.ToString() + " times.", packetToSend);
          Thread.Sleep(p_TransmitRetryDelay);
        }
        tries++;

        packetToSend.Prepare();

        OnSendingPacket(new SendingPacketEventArgs(packetToSend, PacketSegment.Preamble));
        if((EscapedSegments&PacketSegment.Preamble)!=0)
          escaped = Escape(packetToSend.RawPreamble);
        else
          escaped = packetToSend.RawPreamble;

        p_Source.Write(escaped,0,escaped.Length);
      
        if(packetToSend.Payload!=null)
        {
          OnSendingPacket(new SendingPacketEventArgs(packetToSend, PacketSegment.Payload));
          if((EscapedSegments&PacketSegment.Payload)!=0)
            escaped = Escape(packetToSend.RawPayload);
          else
            escaped = packetToSend.RawPayload;
          p_Source.Write(escaped,0,escaped.Length);
        }

        OnSendingPacket(new SendingPacketEventArgs(packetToSend, PacketSegment.Postamble));
        if((EscapedSegments&PacketSegment.Postamble)!=0)
          escaped = Escape(packetToSend.RawPostamble);
        else
          escaped = packetToSend.RawPostamble;

        p_Source.Write(escaped,0,escaped.Length);

      } while(!OnPacketSent(new PacketSentEventArgs(packetToSend)));
    }

    /// <summary>
    /// Closes the Packet reader and the underlying stream
    /// </summary>
    public virtual void Close()
    {
      p_Source.Close();
    }

    #endregion
    #region Protected Methods
    /// <summary>
    /// Reads from the underlying stream.
    /// </summary>
    /// <param name="bytesToRead"></param>
    /// <returns></returns>
    protected byte[] Read(int bytesToRead, bool unescape)
    {
      #region Validation
      if(bytesToRead<=0)
        throw new ArgumentOutOfRangeException("bytesToRead",bytesToRead,"At least one byte must be specified to be read");
      if(bytesToRead>16384)
        throw new ArgumentOutOfRangeException("bytesToRead",bytesToRead,"Request too large. Cannot read " + bytesToRead.ToString() + " bytes");
      #endregion

      byte[] buf = new byte[bytesToRead];
      int bytesread = 0;
      int readposition = 0;
      int writeposition = 0;
      bool escape = false;
      DateTime readstarted = DateTime.Now;

      while(bytesread<bytesToRead)
      {
        while(bytesread<bytesToRead)
        {
          if((DateTime.Now - readstarted) > p_ReadTimeout)
            throw new TimeoutException("Stream read timed out (" + p_ReadTimeout.TotalSeconds.ToString() + " seconds)",p_ReadTimeout);
          bytesread+=p_Source.Read(buf,bytesread,bytesToRead-bytesread);
          if(bytesread>=bytesToRead)
            break;
          Thread.Sleep(10);   //We're buffered. No need to kill the proc. We'll leave this at 10ms
        }

        //--------------   Manually unescape rather than using function.
        //Continue if no Escape character was specified
        if(p_EscapeChar==0x00||(!unescape)) continue;

        writeposition = readposition;

        byte escar = (byte) p_EscapeChar;
        for(int i = readposition;i<bytesread;i++)
        {
          if(escape)
          {
            if(buf[readposition]==escar)
              writeposition--;
            //throw new ProtocolException("Unexpected escape character received: " + ((ASCII)(char)buf[readposition]).ToString());
            escape = false;
          }
          else
          {
            if(buf[readposition]==escar)
              escape = true;
          }

          if(writeposition!=readposition)
            buf[writeposition]=buf[readposition];
          readposition++;
          writeposition++;
        }
        
        bytesread -= (int)(readposition - writeposition);
        readposition = bytesread;
        
      }

      if(escape)
      {
        int i = Int32.MaxValue;
        while(i==Int32.MaxValue)
          i = p_Source.ReadByte();
        if(((byte)i)!=((byte) p_EscapeChar))
          throw new Exception("Encountered unknown escaped character: " + i.ToString());
      }
      return buf;
    }

    protected void WriteEscapedCharacter(char protocolChar)
    {
      byte[] pb = new byte[2];
      pb[0] = (byte) p_EscapeChar;
      pb[1] = (byte) protocolChar;

      p_Source.Write(pb,0,2);

      OnEscapeCharacterSent(new EscapeCharacterSentEventArgs((byte)protocolChar));
    }

    public void WriteEscapedCharacter(ASCII protocolChar)
    {
      #region Validation
      if(p_EscapeChar==0x00)
        throw new InvalidOperationException("Cannot send escaped character: Escape character not set");
      #endregion
      WriteEscapedCharacter((char) protocolChar);
    }

    /// <summary>
    /// Blocks waiting for an escaped character
    /// </summary>
    /// <returns>The escaped character</returns>
    /// <remarks>This method will throw a protocol exception if the first character received is not an escape character</remarks>
    public char ReadEscapedCharacter()
    {
      #region Validation
      if(p_EscapeChar==0x00)
        throw new InvalidOperationException("Cannot get escaped character: Escape character not set");
      #endregion
      byte[] esc = new byte[2];
      esc = Read(2,false);

      if((char)esc[0]!=p_EscapeChar)
        throw new ProtocolException("Expected escape character (0x" + ((byte)p_EscapeChar).ToString("X2") + ") and received 0x" + esc[0].ToString("X2"));

      OnEscapeCharacterReceived(new EscapeCharacterReceivedEventArgs(esc[1]));

      return (char)esc[1];
    }

    /// <summary>
    /// Gets an escaped character, (0x10, 0xXX), and throws an exception if the escaped character does not match the expectedChar
    /// </summary>
    /// <param name="expectedChar">The character we're expecting</param>
    public void ReadEscapedCharacter(ASCII expectedChar)
    {
      byte received = (byte)ReadEscapedCharacter();
      if(received!=(byte)expectedChar)
      {
        string receivedstring = ((ASCII)received).ToString();
        if(receivedstring==null||receivedstring==String.Empty)
          receivedstring="0x" + received.ToString("X2");

        throw new ProtocolException(String.Format("Expected DLE '{0}' and received '{1}'",expectedChar.ToString(),receivedstring));
      }
      
    }
    #endregion
    #region Event Methods (OnXXXXX)
    protected virtual void OnValidationError(ValidationErrorType ValidationErrorType)
    {
      if(ValidationError!=null)
        ValidationError(this, new ValidationErrorEventArgs(ValidationErrorType));
    }

    protected virtual void OnSendingPacket(SendingPacketEventArgs e)
    {
      if(SendingPacket!=null)
        SendingPacket(this,e);
    }

    protected virtual void OnReceivingPacket(ReceivingPacketEventArgs e)
    {
      if(ReceivingPacket!=null)
        ReceivingPacket(this,e);

      p_PacketsReceived++;
    }

    /// <summary>
    /// Override to validate packet, otherwise it's always considered a valid send
    /// </summary>
    /// <returns>True if the packet iswas accepted by the remote host. False will cause the packet to be resent.</returns>
    protected virtual bool OnPacketSent(PacketSentEventArgs e)
    {
      if(PacketSent!=null)
        PacketSent(this,e);

      p_PacketsSent++;
      return true;
    }

    /// <summary>
    /// Fires when a packet has been received. Override to specify whether the packet is good or not.
    /// </summary>
    /// <returns>True of the packet is valid, otherwise the Packet Reader assumes the packet was re-requested</returns>
    protected virtual bool OnPacketReceived(PacketReceivedEventArgs e)
    {
      if(PacketReceived!=null)
        PacketReceived(this,e);

      return true;
    }

    protected virtual void OnEscapeCharacterReceived(EscapeCharacterReceivedEventArgs e)
    {
      if(EscapeCharacterReceived!=null)
        EscapeCharacterReceived(this,e);
    }

    protected virtual void OnEscapeCharacterSent(EscapeCharacterSentEventArgs e)
    {
      if(EscapeCharacterSent!=null)
        EscapeCharacterSent(this,e);
    }

    protected virtual bool OnProtocolError(ProtocolErrorEventArgs e)
    {
      if(ProtocolError!=null)
        ProtocolError(this,e);

      return true;
    }

    
    /// <summary>
    /// Override to handle protocol errors during ReadPacket
    /// </summary>
    /// <param name="e">Event Arguments</param>
    /// <returns>True if the protocol should attempt to retry</returns>
    protected virtual bool OnErrorReadingPacket(ErrorReadingPacketEventArgs e)
    {
      if(ErrorReadingPacket!=null)
        ErrorReadingPacket(this,e);

      return false;
    }
    #endregion
    #region Private Methods

    /// <summary>
    /// Encodes escape characters into the packet
    /// </summary>
    /// <param name="rawData">Data to Escape</param>
    /// <returns>Escaped Packet</returns>
    private byte[] Escape(byte[] rawData)
    {
      #region Validation
      if(rawData==null)
        throw new ArgumentNullException("rawData");

      if(rawData.Length==0)
        throw new ArgumentException("rawData cannot be empty.", "rawData");
      #endregion

      if(p_EscapeChar == (char) 0x00) return rawData;

      int escapefound = 0;

      //--------> Count the number of escape chars to insert
      byte escar = (byte) p_EscapeChar;
      foreach(byte c in rawData)
        if(c==escar)
          escapefound++;

      //---------> Exit function if we don't need to change anything
      if(escapefound==0) return rawData;

      byte[] ret = new byte[rawData.Length+escapefound];
      int retposition = 0;

      //---------> Create return data
      foreach(byte b in rawData)
      {
        ret[retposition++]=(byte) b;
        if(b==escar)
          ret[retposition++]=(byte) b;
      }

      return ret;
    }

    private byte[] Unescape(byte[] rawData, int position)
    {
      #region Validation
      if(rawData==null)
        throw new ArgumentNullException("rawData");
      #endregion

      //TODO: Revise this whole Unescape procedure, from reading on. The buffer should have enough space for 2 packets, and only the new data should be copied to a new byte buffer with only enough space for a real, unescaped packet.
      //Put in  START position so it doesn't escape twice (10 10 10 10 is reduced to 10)
      
      //Exit if no Escape character was specified
      if(p_EscapeChar==0x00) return rawData;

      //If there is no escape character, exit to increase efficiency
      int found = 0;
      byte escar = (byte) p_EscapeChar;
      foreach(byte b in rawData)
        if(b==escar)
          found++;
      if(found==0)
        return rawData;
      //if(Array.BinarySearch(rawData , p_EscapeChar )<0) return rawData;
      
      byte[] newarray = new byte[rawData.Length];

      //TODO: Bench enumerator iteration versus a binary search
      bool escape = false;
      int escapecount = 0;
      int insertposition = 0;
      for(int i = 0;i<rawData.Length;i++)
      {
        if(escape)
        {
          escape=false;
          if(rawData[i]==p_EscapeChar)
          {
            escapecount++;
            continue;
          }
        }
        else
          if(rawData[i]==p_EscapeChar)
          if(i>=position)
            escape=true;


        newarray[insertposition++]=rawData[i];
      }

      byte[] ret = new byte[rawData.Length-escapecount];
      for(int j = 0;j<ret.Length;j++)
        ret[j] = newarray[j];

      return ret;
    }
    #endregion
    #region Abstract Declarations
    /// <summary>
    /// Override to provide the packet reader with a new Packet when reading the next packet <seealso cref="GetNextPacket"/>
    /// </summary>
    /// <returns>A newly created packet with default Type Definitions for the Preamble, Payload, and Postamble.</returns>
    protected abstract Packet CreatePacket();

    public abstract PacketSegment EscapedSegments{get;}
    #endregion
  }

  #region ErrorReadingPacketEventArgs Subclass
  public class ErrorReadingPacketEventArgs : EventArgs
  {
    #region Private Property Declarations
    private ProtocolException p_Exception;
    private int p_Attempt;
    #endregion
    #region Public Property Declarations
    public ProtocolException Exception
    {
      get{return p_Exception;}
      set{p_Exception=value;}
    }

    public int Attempt
    {
      get{return p_Attempt;}
      set{p_Attempt=value;}
    }
    #endregion
    #region Ctors
    public ErrorReadingPacketEventArgs(ProtocolException exception, int attempt)
    {
      p_Exception = exception;
      p_Attempt = attempt;
    }
    #endregion
  }
  #endregion

  #region ReceivingPacketEventArgs Subclass
  public class ReceivingPacketEventArgs
  {
    #region Private Property Declarations
    private PacketSegment p_PacketSegment;
    private Packet p_Packet;
    #endregion
    #region Public Property Declarations
    public PacketSegment PacketSegment
    {
      get{return p_PacketSegment;}
    }

    public Packet Packet
    {
      get{return p_Packet;}
    }
    #endregion
    #region Ctors
    public ReceivingPacketEventArgs(Packet packet, PacketSegment segment)
    {
      p_PacketSegment = segment;
      p_Packet = packet;
    }
    #endregion
  }
  #endregion
  #region SendingPacketEventArgs Subclass
  public class SendingPacketEventArgs
  {
    #region Private Property Declarations
    private PacketSegment p_PacketSegment;
    private Packet p_Packet;
    #endregion
    #region Public Property Declarations
    public PacketSegment PacketSegment
    {
      get{return p_PacketSegment;}
    }

    public Packet Packet
    {
      get{return p_Packet;}
    }
    #endregion
    #region Ctors
    public SendingPacketEventArgs(Packet packet, PacketSegment segment)
    {
      p_PacketSegment = segment;
      p_Packet = packet;
    }
    #endregion
  }
  #endregion
  #region EscapeCharacterSentEventArgs Subclass
  public class EscapeCharacterSentEventArgs : EventArgs
  {
    #region Private Property Declarations
    private byte p_Character;
    #endregion
    #region Public Property Declarations
    public byte Character
    {
      get{return p_Character;}
      set{p_Character = value;}
    }
    #endregion
    #region Ctors
    public EscapeCharacterSentEventArgs(byte character)
    {
      p_Character = character;
    }
    #endregion
  }
  #endregion
  #region EscapeCharacterReceivedEventArgs Subclass
  public class EscapeCharacterReceivedEventArgs  : EventArgs
  {
    #region Private Property Declarations
    private byte p_Character;
    #endregion
    #region Public Property Declarations
    public byte Character
    {
      get{return p_Character;}
      set{p_Character = value;}
    }
    #endregion
    #region Ctors
    public EscapeCharacterReceivedEventArgs (byte character)
    {
      p_Character = character;
    }
    #endregion
  }
  #endregion

  #region PacketReceivedEventArgs Class
  public class PacketReceivedEventArgs : EventArgs
  {
    #region Private Property Declarations
    private Packet p_Packet;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The packet that was received
    /// </summary>
    public Packet Packet
    {
      get{return p_Packet;}
      set{p_Packet = value;}
    }
    #endregion
    #region ctors
    public PacketReceivedEventArgs(Packet packet)
    {
      p_Packet = packet;
    }
    #endregion
  }
  #endregion
  #region PacketSentEventArgs Class
  public class PacketSentEventArgs : EventArgs
  {
    #region Private Property Declarations
    private Packet p_Packet;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The packet that was received
    /// </summary>
    public Packet Packet
    {
      get{return p_Packet;}
      set{p_Packet = value;}
    }
    #endregion
    #region ctors
    public PacketSentEventArgs(Packet packet)
    {
      p_Packet = packet;
    }
    #endregion
  }
  #endregion
  #region ProtocolErrorEventArgs Class
  public class ProtocolErrorEventArgs
  {
    #region Private Property Declarations
    private ProtocolException p_Exception;
    private int p_Attempt;
    #endregion
    #region Public Property Declarations
    public ProtocolException Exception
    {
      get{return p_Exception;}
    }

    public int Attempt
    {
      get{return p_Attempt;}
    }
    #endregion
    #region Ctors
    public ProtocolErrorEventArgs(ProtocolException ex, int attempt)
    {
      p_Exception = ex;
      p_Attempt = attempt;
    }
    #endregion
  }
  #endregion
  #region ValidationErrorEventArgs
  public class ValidationErrorEventArgs
  {
    ValidationErrorType p_ErrorType;

    public ValidationErrorEventArgs(){}
    public ValidationErrorEventArgs(ValidationErrorType ErrorType)
    {
      p_ErrorType = ErrorType;
    }

    /// <summary>
    /// Returns the type of validation error that occurred.
    /// </summary>
    public ValidationErrorType ErrorType
    {
      get{return p_ErrorType;}
      set{p_ErrorType = value;}
    }
  }
  #endregion
}