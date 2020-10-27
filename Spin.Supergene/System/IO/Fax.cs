using System;
using FAXCOMLib;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace System.IO
{
	/// <summary>
	/// Summary description for Fax.
	/// </summary>
	public class Fax
	{
		public Fax()
		{
			//
			// TODO: Add constructor logic here
			//
      // TODO: Add comments to the store
      // TODO: Add a converstaion log to the store
      // TODO: Add a t
      FaxServer fs = new FaxServerClass();
      FaxDoc fd = (FaxDoc) fs.CreateDocument(@"C:\snd.xls");

      fs.Connect("CISDev");
      fs.Retries = 0;
      fs.RetryDelay = 0;
      
      fd.DisplayName = "CISPoll Test";
      fd.FaxNumber = "9,5663481";
      fd.CoverpageName = "Cover Page";
      //fd.ServerCoverpage = 0;
      fd.Send();
		}
    public static void Send(string filePath, string number)
    {
      FaxServer fs = new FaxServerClass();
      FaxDoc fd = (FaxDoc) fs.CreateDocument(filePath);
      //FaxJobClass fj;
      //fj.DeviceStatus = "Completed";

      fs.Connect("CISDev");
      fs.Retries = 0;
      fs.RetryDelay = 0;
      
      fd.DisplayName = "Report";
      //fd.FaxNumber = "9," + number;
      fd.FaxNumber = number;
      fd.CoverpageName = "Cover Page";
      //fd.ServerCoverpage = 0;
      int item = fd.Send();
      //if(fs.GetJobs
    }

    public static void Send(ReportClass report, string number)
    {
      DiskFileDestinationOptions dfko = new DiskFileDestinationOptions();
      System.IO.FileInfo fi = new System.IO.FileInfo("C:\\cistmp.pdf");
      if(fi.Exists)
        fi.Delete();
        
      dfko.DiskFileName = "C:\\cistmp.pdf";

      ExportOptions eo = report.ExportOptions;
      eo.ExportDestinationType = ExportDestinationType.DiskFile;
      eo.ExportFormatType = ExportFormatType.PortableDocFormat;
      //eo.ExportFormatType = ExportFormatType.WordForWindows;

      eo.DestinationOptions = dfko;

      report.Export();

      Send("C:\\cistmp.pdf",number);

    }
	}
}
