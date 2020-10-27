using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace System.IO
{
	/// <summary>
	/// Summary description for CisFtp.
	/// </summary>
	public class CisFtp : Protocol
	{
    #region Structs and Enumerations
    public enum Command : int
    {
      FileHeader = 0,
      FileData = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    protected struct Preamble
    {
      public int DataLength
      {
        get{return p_DataLength;}
        set{p_DataLength = value;}
      }

      public int TotalPackets
      {
        get{return p_TotalPackets;}
        set{p_TotalPackets = value;}
      }

      public Command Command
      {
        get{return (Command)p_Command;}
        set{p_Command = (byte)value;}
      }

      public long FileSize
      {
        get{return p_FileSize;}
        set{p_FileSize = value;}
      }

      int p_DataLength;
      int p_TotalPackets;
      long p_FileSize;
      byte p_Command;
    }

    [StructLayout(LayoutKind.Sequential)]
      protected struct Postamble
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst=16,ArraySubType=UnmanagedType.U1)]
      public byte[] Crc;
    }
    #endregion

    #region Private Property Declarations
    #endregion
    #region Public Property Declarations
    #endregion
    #region Ctors
		public CisFtp(Stream source) : base(source)
		{
		}
    #endregion
    #region Public Methods
    public FileInfo ReadFile(string path)
    {
      FileInfo fi = new FileInfo(path);
      ReadFile(fi);
      return fi;
    }

    public void ReadFile(FileInfo file)
    {
      FileStream fs = file.OpenWrite();
      try
      {
        int currentpacket = 0;
        int totalpackets = 0;

        do
        {
          Packet packet = ReadPacket();

          Preamble pream = (Preamble)packet.Preamble;
          totalpackets = pream.TotalPackets;

          if(currentpacket==0)
            fs.SetLength(pream.FileSize);

          int length = pream.DataLength;

          byte[] payload = packet.RawPayload;

          fs.Write(payload,0,length);
        
        } while(++currentpacket<totalpackets);
      }
      finally
      {
        fs.Close();
      }
    }

    public void WriteFile(string path)
    {
      WriteFile(new FileInfo(path));
    }

    public void WriteFile(FileInfo file)
    {
      const int BUFFER_SIZE = 2048;
      FileStream fs = file.OpenRead();
      try
      {
        int totalpackets = (int)(Math.Ceiling((double)fs.Length/(double)BUFFER_SIZE));
        long filesize = file.Length;
      
        while(fs.Position<fs.Length)
        {
          byte[] buffer = new byte[BUFFER_SIZE];
          int read = 0;
          int packetsize = (int)(((fs.Length-fs.Position)>BUFFER_SIZE)?BUFFER_SIZE:((int)fs.Length-fs.Position));

          //--> Read our file to a buffer
          while(read<packetsize)
            read+=fs.Read(buffer,read,packetsize);

          if(packetsize!=BUFFER_SIZE)
          {
            byte[] tempbuffer = new byte[packetsize];
            for(int i=0;i<packetsize;i++)
              tempbuffer[i] = buffer[i];
            buffer = tempbuffer;
          }

          //--> Create and Send our packet
          CisFtpPacket packet = new CisFtpPacket(buffer,Command.FileData,totalpackets,filesize);
          WritePacket(packet);
        }
      }
      finally
      {
        fs.Close();
      }
    }
    #endregion

    #region Overrides
    protected override void OnReceivingPacket(ReceivingPacketEventArgs e)
    {
      if(e.PacketSegment==PacketSegment.Payload)
        e.Packet.Reconstruct(PacketSegment.Preamble);
      base.OnReceivingPacket (e);
    }

    public override PacketSegment EscapedSegments
    {
      get{return PacketSegment.Payload;}
    }

    protected override Packet CreatePacket()
    {
      return new CisFtpPacket();
    }

    protected override bool OnPacketReceived(PacketReceivedEventArgs e)
    {
      if(!e.Packet.Validate())
      {
        WriteEscapedCharacter(ASCII.NAK);
        base.OnPacketReceived (e);
        return false;
      }
      else
      {
        WriteEscapedCharacter(ASCII.ACK);
        base.OnPacketReceived (e);
        return true;
      }
    }

    protected override bool OnPacketSent(PacketSentEventArgs e)
    {
      ASCII read = (ASCII)ReadEscapedCharacter();
      base.OnPacketSent (e);

      switch(read)
      {
        case ASCII.ACK:
          return true;
        case ASCII.NAK:
          return false;
        default:
          throw new ProtocolException("Encountered unknown escape character " + read.ToString());
      }
    }
    #endregion

    #region CisFtpPacket Class
    public class CisFtpPacket : Packet
    {
      #region Private Methods
      CisFtp.Preamble p_Preamble;
      CisFtp.Postamble p_Postamble;
      #endregion

      #region Public Methods

      #endregion
      #region Ctors
      public CisFtpPacket()
      {
        PreambleType = typeof(CisFtp.Preamble);
        PostambleType = typeof(CisFtp.Postamble);
        PayloadType = typeof(Byte);
      }

      public CisFtpPacket(byte[] payload, Command cmd, int totalPackets, long filesize) : this()
      {
        p_Preamble = new Preamble();
        p_Preamble.Command = cmd;
        p_Preamble.DataLength = payload.Length;
        p_Preamble.TotalPackets = totalPackets;
        p_Preamble.FileSize = filesize;

        RawPayload = payload;
      }
      #endregion
      #region Overrides
      public override int GetPayloadSize()
      {
        return p_Preamble.DataLength;
      }

      public override void Prepare()
      {
        p_Postamble = new Postamble();
        MD5 md5 = MD5.Create();
        byte[] payload = RawPayload;
        p_Postamble.Crc = md5.ComputeHash(payload);

        Deconstruct(PacketSegment.Preamble|PacketSegment.Postamble);
      }

      public override bool Validate()
      {
        MD5 md5 = MD5.Create();
        Reconstruct(PacketSegment.Postamble);
        byte[] payload = RawPayload;
        byte[] hash = md5.ComputeHash(payload);
        byte[] crc = p_Postamble.Crc;

        for(int i = 0;i<16;i++)
          if(crc[i]!=hash[i])
            return false;

        return true;
      }

      public override Array Payload
      {
        get{return RawPayload;}
        set{RawPayload = (byte[])value;}
      }

      public override object Preamble
      {
        get{return p_Preamble;}
        set{p_Preamble = (CisFtp.Preamble)value;}
      }

      public override object Postamble
      {
        get{return p_Postamble;}
        set{p_Postamble = (CisFtp.Postamble)value;}
      }
      #endregion
    }
    #endregion
  }
}
