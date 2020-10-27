using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.IO
{
	/// <summary>
	/// An array of information encapsulated by a preamble and postamble for transport.
	/// </summary>
	/// <remarks>
	/// Used with the Protocol Class. <br />
	/// <br />
	/// The packet will handle construction and deconstruction, or the process of transforming the 
	/// bytes into usable, structured information, and vise-versa.
	/// 
	/// Most of the code in this class assumes that the Preamble will always be a single, blittable struct.
	/// The payload can be fixed or variable length. The deconstruction will deconstruct a payload based on 
	/// the payload type defined by the PayloadType property.
	/// The postamble is assumed to be a fixed-length blittle struct.
	/// 
	/// The pre-amble and postamble can be defined in the implementing class' code, based on context. For
	/// example, two packets with different headers can be defined in one class by defining the Preamble type
	/// in the constructor of the implementing class.
	/// 
	/// Construction and deconstruction of specific packet segments can be overriden by the implementing class
	/// by overriding the OnDeconstructing and OnReconstructing methods and returning false. The method then 
	/// assumes the responsibility of completing the packet information for the segment. The segment being 
	/// deconstructed is supplied in the event arguments.
	/// </remarks>
	public abstract class Packet
	{
    #region Private Property Declarations
    private byte[]  p_RawPreamble;
    private byte[]  p_RawPayload;
    private byte[]  p_RawPostamble;
    private byte[]  p_RawPacket;
    private Type    p_PreambleType;
    private Type    p_PayloadType;
    private Type    p_PostambleType;
    private bool    p_IsConstructed = false;
    private bool    p_IsDeconstructed = false;
    #endregion
    #region Public Property Declarations
    
    /// <summary>
    /// Returns the Entire packet in Raw Bytes.
    /// </summary>
    public virtual Byte[] RawPacket
    {
      get
      {
        if(!IsDeconstructed)
          Deconstruct();
        return p_RawPacket;
      }
      set{p_RawPacket = value;}
    }

    /// <summary>
    /// The Structure type of the preamble
    /// </summary>
    public virtual Type PreambleType
    {
      get{return p_PreambleType;}
      set{p_PreambleType = value;}
    }

    /// <summary>
    /// Gets or sets the expected Payload Type. Only required for Packet Reconstruction.
    /// </summary>
    public virtual Type PayloadType
    {
      get{return p_PayloadType;}
      set{p_PayloadType = value;}
    }

    /// <summary>
    /// Gets or sets the expected postamble Type. Only required for Packet Reconstruction.
    /// </summary>
    public virtual Type PostambleType
    {
      get{return p_PostambleType;}
      set{p_PostambleType = value;}
    }

    /// <summary>
    /// Returns only the Raw Preamble in bytes.
    /// </summary>
    public virtual byte[] RawPreamble
    {
      get{return p_RawPreamble;}
      set{p_RawPreamble = value;}
    }

    /// <summary>
    /// Returns only the Raw Payload in bytes.
    /// </summary>
    public virtual byte[] RawPayload
    {
      get{return p_RawPayload;}
      set{p_RawPayload = value;}
    }

    /// <summary>
    /// Returns only the Raw Postamble in Bytes
    /// </summary>
    public virtual byte[] RawPostamble
    {
      get{return p_RawPostamble;}
      set{p_RawPostamble = value;}
    }

    /// <summary>
    /// Returns true of the packet contains valid Structured data.
    /// </summary>
    public virtual bool IsConstructed
    {
      get{return p_IsConstructed;}
      set{p_IsConstructed = value;}
    }

    /// <summary>
    /// Returns true of the packet contains valid Raw data.
    /// </summary>
    public virtual bool IsDeconstructed
    {
      get{return p_IsDeconstructed;}
      set{p_IsDeconstructed = value;}
    }
    #endregion
    #region ctors
		protected Packet()
		{
		}
    #endregion

    #region Event Declarations
    public delegate void DeconstructingEvent(object sender, DeconstructingEventArgs e);
    public delegate void ReconstructingEvent(object sender, ReconstructingEventArgs e);
    
    public event DeconstructingEvent Deconstructing;
    public event ReconstructingEvent Reconstructing;
    public event EventHandler Deconstructed;
    public event EventHandler Reconstructed;
 
    #endregion

    #region Abstract Declarations
    /// <summary>
    /// When overriden, prepares the packet to be sent to the destination. This usually calculates the packet checksum.
    /// </summary>
    public abstract void Prepare();

    /// <summary>
    /// Validates the packet's raw data before reconstruction.
    /// </summary>
    /// <returns></returns>
    public abstract bool Validate();

    /// <summary>
    /// Override to let the PacketReader know what to the payload size to be.
    /// </summary>
    public abstract int GetPayloadSize();

    /// <summary>
    /// When overriden, provides the Packet with a structured Preamble
    /// </summary>
    public abstract object Preamble{get;set;}

    /// <summary>
    /// When overriden, provides the Packet with a structured Payload Array
    /// </summary>
    public abstract Array Payload{get;set;}

    /// <summary>
    /// When overriden, provides the Packet with a structured Preamble
    /// </summary>
    public abstract object Postamble{get;set;}
    #endregion
    #region Virtual Declarations
    /// <summary>
    /// Deconstructs all Data Structures in the Packet to raw data.
    /// </summary>
    protected virtual void Deconstruct()
    {
      Deconstruct(PacketSegment.All);
    }

    public void Deconstruct(PacketSegment segment)
    {
      #region Validation
      if(Preamble==null)
        throw new InvalidOperationException("Cannot deconstruct Packet. The packet Preamble has not been specified");
      if(Postamble==null)
        throw new InvalidOperationException("Cannot deconstruct Packet. The packet Postamble has not been specified");
      if(!Preamble.GetType().IsLayoutSequential)
        throw new InvalidOperationException("The preamble must be of Value Type and must have the SequentialLayout attribute set");
      if(!Postamble.GetType().IsLayoutSequential)
        throw new InvalidOperationException("The preamble must be of Value Type and must have the SequentialLayout attribute set");
      #endregion
      long position;
      int presize = Marshal.SizeOf(p_PreambleType);
      int plsize = 0;
      if(Payload!=null&&(segment&PacketSegment.Payload)!=0)
        plsize=GetPayloadSize();
      int pstsize = Marshal.SizeOf(p_PostambleType);

      //----> Deconstruct the bytes to a Managed Stream
      MemoryStream ms = new MemoryStream(presize+plsize+pstsize);
      BinaryParser bp = new BinaryParser(ms);

      if((segment&PacketSegment.Preamble)!=0)
      {

        if(OnDeconstructing(new DeconstructingEventArgs(ms,PacketSegment.Preamble)))
        {
          bp.Write(Preamble);
          p_RawPreamble = new byte[presize];

          ms.Seek(0,SeekOrigin.Begin);
          ms.Read(p_RawPreamble,0,p_RawPreamble.Length);
        }
      }
      else if (segment==PacketSegment.All&&p_RawPreamble!=null)
        ms.Write(p_RawPayload,0,p_RawPayload.Length);


      if(Payload!=null&&(segment&PacketSegment.Payload)!=0)
      {
        if(OnDeconstructing(new DeconstructingEventArgs(ms,PacketSegment.Payload)))
        {
          position = ms.Position;

          bp.Write(Payload);
          p_RawPayload = new byte[GetPayloadSize()];

          ms.Seek(position,SeekOrigin.Begin);
          ms.Read(p_RawPayload,0,p_RawPayload.Length);
        }
      } 
      else if (segment==PacketSegment.All&&p_RawPayload!=null)
        ms.Write(p_RawPayload,0,p_RawPayload.Length);
    

      if((segment&PacketSegment.Postamble)!=0)
      {
        if(OnDeconstructing(new DeconstructingEventArgs(ms,PacketSegment.Postamble)))
        {
          position = ms.Position;

          bp.Write(Postamble);
          p_RawPostamble = new byte[pstsize];

          ms.Seek(position,SeekOrigin.Begin);
          ms.Read(p_RawPostamble,0,p_RawPostamble.Length);
        }
      }
      else if (segment==PacketSegment.All&&p_RawPostamble!=null)
        ms.Write(p_RawPostamble,0,p_RawPostamble.Length);

      //----> Copy the Stream to the Raw Packet Member
      if(segment==PacketSegment.All)
        p_RawPacket = ms.ToArray();

      OnDeconstructed(EventArgs.Empty);
      p_IsDeconstructed = true;
    }

    public virtual void Reconstruct()
    {
      Reconstruct(PacketSegment.All);
    }

    /// <summary>
    /// Constructs the Structures of the Packet based on the information in the RawData
    /// </summary>
    public virtual void Reconstruct(PacketSegment segment)
    {
      #region Validation
      if(p_RawPreamble==null&&(segment&PacketSegment.Preamble)!=0)
        throw new InvalidOperationException("Cannot Reconstruct Packet. Missing Preamble information");
      if(p_RawPostamble==null&&(segment&PacketSegment.Postamble)!=0)
        throw new InvalidOperationException("Cannot Reconstruct Packet. Missing Postamble information");
      if(p_PreambleType==null&&(segment&PacketSegment.Preamble)!=0)
        throw new InvalidOperationException("Cannot reconstruct Preamble without Preamble Type Information. Set the PreambleType property.");
      if(p_PostambleType==null&&(segment&PacketSegment.Postamble)!=0)
        throw new InvalidOperationException("Cannot reconstruct Preamble without Postamble Type Information. Set the PostmbleType property.");
      if(p_RawPayload!=null&&(segment&PacketSegment.Payload)!=0)
        if(p_PayloadType==null)
          throw new InvalidOperationException("Cannot reconstruct Payload without Payload Type Information. Set the PayloadType property.");
      #endregion

      MemoryStream ms = ToStream(segment);
      BinaryParser bp = new BinaryParser(ms);

      if((segment&PacketSegment.Preamble)!=0)
        if(OnReconstructing(new ReconstructingEventArgs(ms,PacketSegment.Preamble)))
          Preamble = bp.Read(p_PreambleType);

      if(p_RawPayload!=null&&(segment&PacketSegment.Payload)!=0)
        if(OnReconstructing(new ReconstructingEventArgs(ms,PacketSegment.Payload)))
          Payload = bp.Read(p_PayloadType, (GetPayloadSize()/Marshal.SizeOf(p_PayloadType)));

      if((segment&PacketSegment.Postamble)!=0)
        if(OnReconstructing(new ReconstructingEventArgs(ms,PacketSegment.Postamble)))
          Postamble = bp.Read(PostambleType);

      if(segment==PacketSegment.All)
      {
        OnReconstructed(EventArgs.Empty);
        p_IsConstructed = true;
      }
    }
    #endregion

    #region Event Methods (OnXXXXX)
    protected virtual bool OnReconstructing(ReconstructingEventArgs e)
    {
      if(Reconstructing!=null)
        Reconstructing(this,e);

      return true;
    }

    protected virtual bool OnDeconstructing(DeconstructingEventArgs e)
    {
      if(Deconstructing!=null)
        Deconstructing(this,e);

      return true;
    }

    protected virtual void OnReconstructed(EventArgs e)
    {
      if(Reconstructed!=null)
        Reconstructed(this,e);
    }

    protected virtual void OnDeconstructed(EventArgs e)
    {
      if(Deconstructed!=null)
        Deconstructed(this,e);
    }
    #endregion
    #region Private Methods
    /// <summary>
    /// Checks the status of the Structured information when Structured data is set.
    /// </summary>
    /// <returns></returns>
    private bool IsStructuredDataComplete()
    {
      return Preamble!=null&&Postamble!=null;
    }

    private bool IsRawDataComplete()
    {
      return p_RawPreamble!=null&&p_RawPostamble!=null;
    }

    #endregion
    #region Public Methods
    public MemoryStream ToStream(PacketSegment segment)
    {
      int totalsize = 0;
      if(p_RawPreamble!=null&&(segment&PacketSegment.Preamble)!=0)
        totalsize+=RawPreamble.Length;

      if(p_RawPayload!=null&&(segment&PacketSegment.Payload)!=0)
        totalsize+=RawPayload.Length;

      if(p_RawPostamble!=null&&(segment&PacketSegment.Postamble)!=0)
        totalsize+=RawPostamble.Length;

      MemoryStream ret = new MemoryStream(totalsize);

      if(p_RawPreamble!=null&&(segment&PacketSegment.Preamble)!=0)
        ret.Write(p_RawPreamble,0,p_RawPreamble.Length);

      if(p_RawPayload!=null&&(segment&PacketSegment.Payload)!=0)
        ret.Write(p_RawPayload,0,p_RawPayload.Length);

      if(p_RawPostamble!=null&&(segment&PacketSegment.Postamble)!=0)
        ret.Write(p_RawPostamble,0,p_RawPostamble.Length);

      ret.Seek(0,SeekOrigin.Begin);
      return ret;
    }

    public void SaveToDisk(string path)
    {
      BinaryFormatter bf = new BinaryFormatter();
      Packet.SerializationInfo si = new Packet.SerializationInfo(this);

      FileInfo fi = new FileInfo(path);
      if(fi.Exists)
        fi.Delete();

      FileStream fs = fi.Create();
      

      bf.Serialize(fs,si);
      fs.Flush();
      fs.Close();
    }

    public static Packet LoadFromDisk(string path)
    {
      BinaryFormatter bf = new BinaryFormatter();

      FileInfo fi = new FileInfo(path);
      FileStream fs = fi.Open(FileMode.Open);
      
      object dso = bf.Deserialize(fs);

      Packet ret = ((Packet.SerializationInfo) dso).ConvertToPacket();
      fs.Close();

      return ret;
    }
    #endregion

    #region Serialization Info Subclass
    /// <summary>
    /// Creates a Serializable instance of the Packet Class
    /// </summary>
    [Serializable]
      public class SerializationInfo
    {
      #region Private Property Declarations
      public byte[] p_RawPacket;
      public byte[] p_RawPayload;
      public byte[] p_RawPreamble;
      public byte[] p_RawPostamble;
      public string p_PreambleType;
      public string p_PayloadType;
      public string p_PostambleType;
      public object p_Preamble;
      public object p_Postamble;

      public Array p_Payload;
      public string p_ThisType;
      #endregion

      #region Public Property Declarations
      /*
      public byte[] RawPacket
      {
        get{return p_RawPacket;}
        set{p_RawPacket = value;}
      }
      */
      #endregion
      public SerializationInfo()
      {

      }

      public SerializationInfo(Packet packetToSerialize)
      {
        p_RawPacket = packetToSerialize.RawPacket;
        p_RawPayload = packetToSerialize.RawPayload;
        p_RawPreamble = packetToSerialize.RawPreamble;
        p_RawPostamble = packetToSerialize.RawPostamble;

      
        if(packetToSerialize.PreambleType!=null)
          p_PreambleType = packetToSerialize.PreambleType.ToString();
        if(packetToSerialize.PayloadType!=null)
          p_PayloadType = packetToSerialize.PayloadType.ToString();
        if(packetToSerialize.PostambleType!=null)
          p_PostambleType = packetToSerialize.PostambleType.ToString();
      
        p_Preamble = packetToSerialize.Preamble;
        p_Postamble = packetToSerialize.Postamble;
        p_Payload = packetToSerialize.Payload;
        p_ThisType = packetToSerialize.ToString();
      }

      public Packet ConvertToPacket()
      {
        Packet packetToSerialize = (Packet) Activator.CreateInstance(Type.GetType(p_ThisType,true,true),new object[]{});

        packetToSerialize.RawPacket = p_RawPacket;
        packetToSerialize.RawPayload = p_RawPayload;
        packetToSerialize.RawPreamble = p_RawPreamble;
        packetToSerialize.RawPostamble = p_RawPostamble ;
        if(p_PreambleType!=null)
          packetToSerialize.PreambleType = Type.GetType(p_PreambleType);
        if(p_PayloadType!=null)
          packetToSerialize.PayloadType = Type.GetType(p_PayloadType);
        if(p_PostambleType!=null)
          packetToSerialize.PostambleType = Type.GetType(p_PostambleType);
        packetToSerialize.Preamble = p_Preamble;
        packetToSerialize.Postamble = p_Postamble;
        packetToSerialize.Payload = p_Payload;

        return packetToSerialize;
      }
    }
    #endregion
    #region DeconstructingEventArgs Subclass
    public class DeconstructingEventArgs 
    {
      #region Private Property Declarations
      private Stream p_Stream;
      private PacketSegment p_Segment;
      #endregion
      #region Public Property Declarations
      public Stream Stream
      {
        get{return p_Stream;}
        set{p_Stream = value;}
      }

      public PacketSegment Segment
      {
        get{return p_Segment;}
        set{p_Segment = value;}
      }
      #endregion
      #region Ctors
      public DeconstructingEventArgs(Stream stream, PacketSegment segment)
      {
        p_Segment = segment;
        p_Stream = stream;
      }
      #endregion
    }
    #endregion
    #region ReconstructingEventArgs Subclass
    public class ReconstructingEventArgs 
    {
      #region Private Property Declarations
      private Stream p_Stream;
      private PacketSegment p_Segment;
      #endregion
      #region Public Property Declarations
      public Stream Stream
      {
        get{return p_Stream;}
        set{p_Stream = value;}
      }

      public PacketSegment Segment
      {
        get{return p_Segment;}
        set{p_Segment = value;}
      }
      #endregion
      #region Ctors
      public ReconstructingEventArgs(Stream stream, PacketSegment segment)
      {
        p_Segment = segment;
        p_Stream = stream;
      }
      #endregion
    }
    #endregion
  }
}
