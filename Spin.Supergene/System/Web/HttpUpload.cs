using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace System.Web
{
	/// <summary>
	/// Parses the Mime Header from the http request and sends notification when data fields are encountered
	/// </summary>
	/// <remarks>
	/// The raw data is read into the buffer and the buffer is searched for mime delimiters. 
	/// The header information is parsed from the buffer and the actual data is then sent to
	/// the DataStream, and an event is fired sending notification that the stream contains data.
	/// </remarks>
	public class HttpUpload
	{
    ManualResetEvent m_Event = new ManualResetEvent(false);
    public enum HeaderPosition
    {
      Delimiter,
      Header,
      Data
    }
    #region Private Property Declarations
    private int p_TotalBytes;
    private int p_BytesRead;
    private int p_Cursor;
    private int p_Read;
    private byte[] p_Boundary;
    private int p_BoundaryFound = 0;
    private byte[] p_HeaderData = new byte[1024];
    private HeaderPosition p_Position = HeaderPosition.Delimiter;
    private Stream p_DataStream = new MemoryStream(8192);
    private StringCollection p_BinaryFields;
    private byte[] p_Buffer = new byte[1024];
    private bool p_IsFinished;
    private Stream p_Source;

    #endregion
    #region Public Property Declarations
    public int TotalBytes
    {
      get{return p_TotalBytes;}
      set{p_TotalBytes=value;}
    }

    public int BytesRead
    {
      get{return p_BytesRead;}
      set{p_BytesRead=value;}
    }

    public int Cursor
    {
      get{return p_Cursor;}
      set{p_Cursor=value;}
    }

    public int Read
    {
      get{return p_Read;}
      set{p_Read=value;}
    }

      public HeaderPosition Position
    {
      get{return p_Position;}
      set{p_Position=value;}
    }

    public Stream DataStream
    {
      get{return p_DataStream;}
      set{p_DataStream=value;}
    }

    public StringCollection BinaryFields
    {
      get{return p_BinaryFields;}
      set{p_BinaryFields=value;}
    }

    public byte[] Buffer
    {
      get{return p_Buffer;}
      set{p_Buffer=value;}
    }

    public bool IsFinished
    {
      get{return p_IsFinished;}
      set{p_IsFinished=value;}
    }

    public Stream Source
    {
      get{return p_Source;}
      set{p_Source=value;}
    }
    #endregion
    #region Ctors
		public HttpUpload(Stream source, string boundary, int totalBytes)
		{
      p_Source = source;
      p_Boundary = System.Text.ASCIIEncoding.ASCII.GetBytes("--" + boundary);
      p_TotalBytes = totalBytes;
		}
    #endregion
    #region Private Methods
    private void BeginAsyncRead()
    {
      //p_Source.BeginRead(p_Buffer,0,p_Buffer.Length,new AsyncCallback(AsyncRead),this);
    }

//    public void StartRead()
//    {
//      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(InternalRead));
//      m_Event.WaitOne();
//    }

    public void StartRead()
    {
      StringBuilder rawmime = new StringBuilder(2048);
      byte lastb=0;       //Used in searching for the Mime-Part header end-delimiter
      byte thisb=0;       //Used in searching for the Mime-Part header end-delimiter

      //Ensure we load enough data into our buffer to validate
      while(p_Read<p_Boundary.Length)
        p_Read+=p_Source.Read(p_Buffer,p_Read,p_Buffer.Length-p_Read);
      p_BytesRead+=p_Read;

      //Skip the beginning 2 dashes "--"

      //Validate the first boundary
      while(p_Cursor<p_Boundary.Length)
        if(p_Buffer[p_Cursor++]!=p_Boundary[p_BoundaryFound++])
          throw new Exception("Expected starting boundary while parsing MIME multi-part");
      p_BoundaryFound=0;

      //Left-align our header data. Don't preserve the 2 beginning dashes or the CrLf
      SlideBuffer(p_Read-p_Boundary.Length-2);
      p_Cursor = 0;

      while(p_BytesRead<p_TotalBytes)
      {
        //Safety net in-case code breaks, we don't kill the machine. (Also simulate slow uploading)
        System.Threading.Thread.Sleep(500);       

        //Read more data into the buffer
        int read = p_Source.Read(p_Buffer,p_Read,p_Buffer.Length-p_Read); 
        p_Read += read;
        p_BytesRead+=read;

        while(p_Cursor<p_Read)
        {
          switch(p_Position)
          {
            case HeaderPosition.Delimiter:
              //Read to the end of the header. The buffer should be adequate to handle the entire header
              //read to the end of the delimiter
              //0x0D=CR 0x0A=LF
              
              //True when the end delimiter is found.
              bool end = false;       

              //Read the mime-field header, add to the stringbuilder, and search for the delimiter.
              while(p_Cursor<p_Read)
              {
                if(lastb==0x0A&&thisb==0x0D)
                {
                  end=true;
                  break;
                }
                lastb=thisb;
                thisb=p_Buffer[p_Cursor++];
                rawmime.Append((char)thisb);
              }
              if(end)
              {
                thisb=0;
                lastb=0;

                string temp = rawmime.ToString();
                rawmime.Remove(0,rawmime.Length);

                //Reposition the cursor to the start of the MIME field data.
                p_Cursor++;

                //Left-align the data in the buffer to make it easier to use
                SlideBuffer(p_Read-p_Cursor);
                p_Cursor=0;

                //Update class state
                p_Position = HeaderPosition.Data;
                OnBinaryDataStarting(new BinaryDataStartingEventArgs(temp.ToString().Split(new char[]{(char)ASCII.CR,(char)ASCII.LF})));
              }
              break;
            case HeaderPosition.Data:
              //search for the end delimiter
              if(p_Buffer[p_Cursor]==p_Boundary[p_BoundaryFound])
              {
                if(p_BoundaryFound==p_Boundary.Length-1)
                {
                  //We've encountered a boundary. Reset the class state and sned notification
                  if(p_DataStream!=null)
                    p_DataStream.Write(p_Buffer,0,p_Cursor-p_Boundary.Length);
                  OnBinaryDataArrived(new BinaryDataArrivedEventArgs());
                  OnBinaryDataEnding(new BinaryDataEndingEventArgs());
                  SlideBuffer(p_Read-p_Cursor-1);
                  p_Cursor=0;
                  p_Position=HeaderPosition.Delimiter;
                  p_BoundaryFound = 0;
                  break;
                } 
                else 
                {
                  p_BoundaryFound++;
                }
              }
              else
              {
                p_BoundaryFound=0;
              }

              if(p_Cursor==p_Read-1)
              {
                if(p_DataStream!=null)
                  p_DataStream.Write(p_Buffer,0,p_Read-p_BoundaryFound);
                OnBinaryDataArrived(new BinaryDataArrivedEventArgs());
                p_Cursor=SlideBuffer(p_BoundaryFound);
                break;
              }
              p_Cursor++;
              break;
          }
        }
      }
      //m_Event.Set();
    }

    private int SlideBuffer(int preserve)
    {
      int readcursor = p_Read-preserve;
      for(int i=readcursor;i<p_Read;i++)
        p_Buffer[i-readcursor]=p_Buffer[i];
      p_Read = preserve;
      return preserve;
    }
    #endregion

    #region Events / Delegates
    public event BinaryDataStartingEvent BinaryDataStarting;
    public event BinaryDataEndingEvent BinaryDataEnding;
    public event BinaryDataArrivedEvent BinaryDataArrived;

    public delegate void BinaryDataEndingEvent(object sender, BinaryDataEndingEventArgs e);
    public delegate void BinaryDataStartingEvent(object sender, BinaryDataStartingEventArgs e);
    public delegate void BinaryDataArrivedEvent(object sender, BinaryDataArrivedEventArgs e);
    #endregion
    #region Overrides (OnXXXX)
    protected virtual void OnBinaryDataArrived(BinaryDataArrivedEventArgs e)
    {
      if(BinaryDataArrived!=null)
        BinaryDataArrived(this,e);
    }

    protected virtual void OnBinaryDataEnding(BinaryDataEndingEventArgs e)
    {
      if(BinaryDataEnding!=null)
        BinaryDataEnding(this,e);
    }
    protected virtual void OnBinaryDataStarting(BinaryDataStartingEventArgs e)
    {
      if(BinaryDataStarting!=null)
        BinaryDataStarting(this,e);
    }
    #endregion

    #region BinaryDataStartingEventArgs Subclass
    public class BinaryDataStartingEventArgs : EventArgs
    {
      #region Private Property Declarations
      private string[] p_MimeHeaders;

      #endregion
      #region Public Property Declarations
      public string[] MimeHeaders
      {
        get{return p_MimeHeaders;}
        set{p_MimeHeaders=value;}
      }


      #endregion
      #region Ctors
      public BinaryDataStartingEventArgs(string[] mimeHeaders)
      {
        p_MimeHeaders = mimeHeaders;
      }
      #endregion
    }
    #endregion
    #region BinaryDataEndingEventArgs Subclass
    public class BinaryDataEndingEventArgs : EventArgs
    {
      #region Private Property Declarations

      #endregion
      #region Public Property Declarations

      #endregion
      #region Ctors
      public BinaryDataEndingEventArgs()
      {

      }
      #endregion
    }
    #endregion
    #region BinaryDataArrivedEventArgs Subclass
    public class BinaryDataArrivedEventArgs : EventArgs
    {
      #region Private Property Declarations

      #endregion
      #region Public Property Declarations

      #endregion
      #region Ctors
      public BinaryDataArrivedEventArgs()
      {

      }
      #endregion
    }
    #endregion
  }
}
