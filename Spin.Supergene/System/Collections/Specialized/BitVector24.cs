using System;

namespace System.Collections.Specialized
{
	/// <summary>
	/// Summary description for BitVector24.
	/// </summary>
	public class BitVector24
	{
    #region Private Property Declarations
    private byte[] p_Source;
    #endregion
    #region Public Property Declarations
    public byte[] Source
    {
      get{return p_Source;}
      set{p_Source = value;}
    }
    #endregion
    #region Ctors
		public BitVector24(byte[] b)
		{
      #region Validation
      if(b.Length!=3)
        throw new ArgumentOutOfRangeException("Byte array must be of length 3");
      #endregion
      p_Source = b;
		}

    public BitVector24()
    {
      p_Source = new Byte[3];
    }
    #endregion

    #region Indexers
    public bool this[int index]
    {
      get
      {
        int pos = ((index<8)?0:(index<16)?1:2);
        return p_Source[pos]>>((index%8)&0x01)==1;
      }
      set
      {
        int pos = ((index<8)?0:(index<16)?1:2);
        if(!value)
          p_Source[pos]&=(byte)(0xFF^(0x01<<(index%8)));
        else
          p_Source[pos]|=(byte)(0x01<<(index%8));
      }
    }
    #endregion
	}
}
