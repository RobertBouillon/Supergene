using System;
using System.Management;

namespace System.IO
{
	/// <summary>
	/// Summary description for DriveSpace.
	/// </summary>
	public struct DriveSpace
	{
    #region Private Property Declarations
    private ulong p_Bytes;

    #endregion
    #region Public Property Declarations
    public decimal GB
    {
      get{return Math.Round(MB/1024,2);}
    }

    public decimal MB
    {
      get{return Math.Round(KB/1024,2);}
    }

    public decimal KB
    {
      get{return Math.Round((decimal)p_Bytes / 1024,2);}
    }

    public ulong Bytes
    {
      get{return p_Bytes;}
      set{p_Bytes=value;}
    }
    #endregion

    #region Ctors
    public DriveSpace(ulong bytes)
    {
      p_Bytes = bytes;
		}
    #endregion

    #region Overrides
    public override string ToString()
    {
      const string format = "{0} {1}";
      if(p_Bytes > 1073741824)
        return String.Format(format,GB.ToString("n"),"GB");

      if(p_Bytes > 1048576)
        return String.Format(format,MB.ToString("n"),"MB");

      if(p_Bytes > 1024)
        return String.Format(format,KB.ToString("n"),"KB");

      return String.Format(format,p_Bytes.ToString("n"),"B");
    }
    #endregion
    #region Static Declarations
    public static DriveSpace GetFreeSpace(char driveLetter)
    {
      ManagementObjectCollection myMOC = (new ManagementObjectSearcher(new SelectQuery("SELECT FreeSpace FROM Win32_LogicalDisk WHERE deviceID = '" + driveLetter + ":'"))).Get();
      foreach (ManagementObject myMO in myMOC)
        return new DriveSpace((ulong)myMO.Properties["FreeSpace"].Value);

      throw new Exception("Unable to get drive information for drive letter '" + driveLetter + "'");
    }

    #endregion
	}
}
