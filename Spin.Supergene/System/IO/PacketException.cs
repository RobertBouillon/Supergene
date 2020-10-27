using System;

namespace System.IO
{
	/// <summary>
	/// Used when an error with a Packet's data is encountered. For general purposes, this allows debugging information about the packet to be stored
	/// </summary>
	public class PacketException : Exception
	{
    #region Private Property Declarations
    private Packet p_Packet;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The packet that is the subject caused the exceptionS.
    /// </summary>
    public Packet Packet
    {
      get{return p_Packet;}
      set{p_Packet = value;}
    }
    #endregion  
		public PacketException(Packet badPacket) : base()
		{
      this.p_Packet = badPacket;
		}

    public PacketException(string message, Packet badPacket) : base(message)
    {
      this.p_Packet = badPacket;
    }

    public PacketException(string message, Exception innerException, Packet badPacket) : base(message,innerException)
    {
      this.p_Packet = badPacket;
    }
	}
}
