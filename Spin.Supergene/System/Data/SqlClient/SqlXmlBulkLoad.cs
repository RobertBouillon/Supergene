using System;
using SQLXMLBULKLOADLib;
using System.Xml;
using ADODB;
using System.Runtime.InteropServices;
using System.Reflection;



namespace System.Data.SqlClient
{
	/// <summary>
	/// Summary description for SqlXmlBulkLoad.
	/// </summary>
	public class SqlXmlBulkLoad
	{


    public static void Load(string xsdPath, string xmlToLoad, bool IgnoreDuplicates)
    {
      Load(xsdPath,GetStream(xmlToLoad),IgnoreDuplicates);
    }

    public static void Load(string xsdPath, XmlDocument docToLoad, bool ignoreDuplicates)
    {
      Load(xsdPath,GetStream(docToLoad),ignoreDuplicates);
    }

    public static void Load(string xsdPath, string xmlToLoad)
    {
      Load(xsdPath,GetStream(xmlToLoad),false);
    }

    public static void Load(string xsdPath, XmlDocument docToLoad)
    {
      Load(xsdPath,GetStream(docToLoad),false);
    }

    public static void Load(string xsdPath, System.IO.Stream stream)
    {
      Load(xsdPath,stream,false);
    }

    public static void Load(string xsdPath, System.IO.Stream stream, bool ignoreDuplicates)
    {
      Load(xsdPath,GetStream(stream),ignoreDuplicates);
    }

    public static void Load(string XSDPath, Stream XmlData, bool IgnoreDuplicates)
    {
      SQLXMLBulkLoad3Class blkload = new SQLXMLBulkLoad3Class();
      // NOTE: The XML Bulk Load COM object does not appear to be Thread Safe
      blkload.GetType().InvokeMember("ConnectionString",BindingFlags.SetProperty,null,blkload,
        new Object[] {"Provider=SQLOLEDB;Initial Catalog=CIS;Data Source=(local);uid=sa;pwd=saserver"});
      //blkload.ConnectionString = "Provider=SQLOLEDB;Initial Catalog=CSIG;Data Source=(local);uid=sa;pwd=saserver";
      
      blkload.GetType().InvokeMember("TempFilePath",BindingFlags.SetProperty,null,blkload,
        new Object[] {@"C:\Temp\"});
      //blkload.TempFilePath = @"C:\Temp\";

      blkload.GetType().InvokeMember("ErrorLogFile",BindingFlags.SetProperty,null,blkload,
        new Object[] {@"C:\Error.xml"});
      //blkload.ErrorLogFile = @"C:\Error.xml";

      blkload.GetType().InvokeMember("KeepIdentity",BindingFlags.SetProperty,null,blkload,
        new Object[] {false});
      //blkload.KeepIdentity = false;

      blkload.GetType().InvokeMember("IgnoreDuplicateKeys",BindingFlags.SetProperty,null,blkload,
        new Object[] {IgnoreDuplicates});
      //blkload.IgnoreDuplicateKeys = IgnoreDuplicates;

      Console.Out.WriteLine("Loading XMl Bulk Loader");
      blkload.GetType().InvokeMember("Execute",BindingFlags.InvokeMethod,null,blkload,
        new Object[] {XSDPath,XmlData});
      Console.Out.WriteLine("Bulk Operation Completed Successfully");
    }

    private static Stream GetStream(XmlDocument DocToLoad)
    {
      Stream str = new ADODB.Stream();
      
      str.Charset = "UTF-8";
      str.Open(Type.Missing,ADODB.ConnectModeEnum.adModeUnknown,ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,"","");
      str.WriteText(DocToLoad.InnerXml,ADODB.StreamWriteEnum.stWriteLine);
      str.Position = 0;
      
      return str;
    }

    private static Stream GetStream(System.IO.Stream s)
    {
      Stream str = new ADODB.Stream();

      System.IO.StreamReader sr = new System.IO.StreamReader(s);
      str.Charset = "UTF-8";
      str.Open(Type.Missing,ADODB.ConnectModeEnum.adModeUnknown,ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,"","");
      str.WriteText(sr.ReadToEnd(),ADODB.StreamWriteEnum.stWriteLine);
      str.Position = 0;

      return str;
    }

    private static Stream GetStream(string XmlToLoad)
    {
      Stream str = new ADODB.Stream();
      
      str.Charset = "UTF-8";
      str.Open(Type.Missing,ADODB.ConnectModeEnum.adModeUnknown,ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,"","");
      str.WriteText(XmlToLoad,ADODB.StreamWriteEnum.stWriteLine);
      str.Position = 0;
      
      return str;
    }
  }
}
