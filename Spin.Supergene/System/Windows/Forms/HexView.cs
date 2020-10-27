using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace System.Windows.Forms
{
	/// <summary>
	/// Summary description for HexView.
	/// </summary>
	public class HexView : System.Windows.Forms.ScrollableControl
	{
    #region Designer Variables
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
    #endregion
    #region Private Property Declarations
    private int p_DataWidth = 8;
    private int p_CellWidth = 25;
    private int p_CellHeight = 15;
    private HexViewPaneCollection p_Panes;
    private ScrollBars p_ScrollBars = ScrollBars.None;
    #endregion

    #region Public Property Declarations
    public HexViewPaneCollection Panes
    {
      get{return p_Panes;}
    }

    [DefaultValue(8),Category("Data"),Browsable(true)]
    public int DataWidth
    {
      get{return p_DataWidth;}
      set{p_DataWidth = value;}
    }

    [DefaultValue(27),Category("Data"),Browsable(true)]
    public int CellWidth
    {
      get{return p_CellWidth;}
      set{p_CellWidth = value;}
    }

    [DefaultValue(17),Category("Data"),Browsable(true)]
    public int CellHeight
    {
      get{return p_CellHeight;}
      set{p_CellHeight = value;}
    }

    public ScrollBars ScrollBars
    {
      get{return p_ScrollBars;}
      set{p_ScrollBars = value;}
    }
    #endregion
    #region Ctors
    public HexView()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      p_DataWidth = 8;

      p_Panes = new HexViewPaneCollection(this);

#if DESIGN
      byte[] test = new byte[]{0x00,0x04,0x10,0x00,0x50,0x00,0x04,0x10,0x00,0x50,0x00,0x04,0x10,0x00,0x50,0x00,0x04,0x10,0x00,0x50};
      byte[] test2 = new byte[]{0x10,0x04};
      //byte[] test = new byte[]{0x00,0x04,0x10,0x50};
      HexViewPane pane = new HexViewPane("READ",test);
      Controls.Add(pane);

      HexViewPane pane2 = new HexViewPane("WRITE",test);
      pane2.SetColorScheme(Color.Red);
      Controls.Add(pane2);
#endif
    }
    #endregion

    #region Overrides
    protected override void OnControlAdded(ControlEventArgs e)
    {
      if(e.Control is HexViewPane)
      {
        HexViewPane added = (HexViewPane)e.Control;
        int y = 0;
        foreach(Control c in Controls)
          if(c is HexViewPane)
            if(c!=added)
            {
              Console.Out.WriteLine(y);
              y+=c.Height;
            }

        int x = 0;
        added.Location = new Point(x,AutoScrollPosition.Y+y);
      }
      base.OnControlAdded (e);
    }
    #endregion

    #region Protected Methods
    


    #endregion
    #region IDisposable Members
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
    #endregion
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      // 
      // HexView
      // 
      this.AutoScroll = true;
      this.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.Size = new System.Drawing.Size(280, 200);

    }
		#endregion

    #region HexViewPane Subcontrol
    public class HexViewPane : Control
    {
      #region Private Property Declarations
      //private HexView p_StrongParent;
      private ArrayList p_Data;
      private Color p_TitleForeColor = Color.AliceBlue;
      private Color p_TitleBackColor = Color.DarkBlue;
      private Color p_HighlightColor = Color.White;
      private Color p_BorderColor = Color.AliceBlue;
      private int p_TitleWidth = 16;
      private bool p_ShowTitle = true;
      private bool p_MouseOver = false;
      #endregion

      #region Public Property Declarations
      public Color BorderColor
      {
        get{return p_BorderColor;}
        set{p_BorderColor = value;}
      }
      public Color TitleForeColor
      {
        get{return p_TitleForeColor;}
        set{p_TitleForeColor = value;}
      }

      public Color TitleBackColor
      {
        get{return p_TitleBackColor;}
        set{p_TitleBackColor = value;}
      }

      public Color HighlightColor
      {
        get{return p_HighlightColor;}
        set{p_HighlightColor = value;}
      }
      #endregion
      #region Ctors
      public HexViewPane(string name, byte[] initialData)
      {
        base.SetStyle(
          ControlStyles.UserPaint|
          ControlStyles.StandardClick|
          ControlStyles.ResizeRedraw|
          ControlStyles.Opaque|
          ControlStyles.FixedHeight|
          ControlStyles.FixedWidth|
          ControlStyles.AllPaintingInWmPaint
          ,true);

        p_Data = new ArrayList(initialData);
        BackColor = Color.AliceBlue;

        Name = name;
        Width = 1;
        Height = 1;
        
        foreach(byte b in p_Data)
          Controls.Add(new HexViewCell(b));
      }
      #endregion

      #region Overrides
      protected override void OnParentChanged(EventArgs e)
      {
        SetSize();
        base.OnParentChanged (e);
      }

      protected override void OnMouseEnter(EventArgs e)
      {
        p_MouseOver = true;
        Color tmp = BackColor;
        BackColor = p_HighlightColor;
        p_HighlightColor = tmp;
        Invalidate();

        base.OnMouseEnter (e);
      }

      protected override void OnMouseHover(EventArgs e)
      {
        base.OnMouseHover (e);
      }

      protected override void OnMouseLeave(EventArgs e)
      {
        p_MouseOver = false;

        Color tmp = BackColor;
        BackColor = p_HighlightColor;
        p_HighlightColor = tmp;

        Invalidate();
        base.OnMouseLeave (e);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
        base.OnPaint (e);

        Graphics g = e.Graphics;

        SetSize();
        SetLayout();


        //------> Render the control
        g.FillRectangle(Brushes.White,new Rectangle(0,0,Width,Height));
        g.FillRectangle(new SolidBrush(BackColor),new Rectangle(0,0,Width,Height));
        
        if(p_MouseOver)
          g.DrawRectangle(new Pen(new SolidBrush(p_TitleBackColor),1), new Rectangle(0,0,Width-1,Height-1));

        if(p_ShowTitle)
        {
          Rectangle titlerect = new Rectangle(0,0,p_TitleWidth,Height);
          g.FillRectangle(new SolidBrush(p_TitleBackColor),titlerect);
          g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

          StringFormat format = new StringFormat();
          format.Trimming = StringTrimming.None;
          format.FormatFlags |= StringFormatFlags.DirectionVertical;
          format.Alignment = StringAlignment.Center;
          DrawRotatedString(g,Name,Font,new SolidBrush(p_TitleForeColor),titlerect,format,180);
        }
      }
      #endregion

      #region Protected Methods
      /// <summary>
      /// Sets the size of the control based on the properties defined in the parent
      /// </summary>
      protected void SetSize()
      {
        HexView parent = (HexView)Parent;
        int width = (parent.CellWidth * parent.DataWidth)+1;
        if(p_ShowTitle)
          width+=p_TitleWidth;
        int height = (parent.CellHeight * ((p_Data.Count/parent.DataWidth))) + 2;
        if(p_Data.Count%parent.DataWidth!=0)
          height+=parent.CellHeight;

        if(Width!=width)
          Width=width;

        if(Height!=height)
          Height=height;
      }

      protected void SetLayout()
      {
        HexView parent = (HexView) Parent;
        int position = 0;
        int line = 0;
        int offset = 0;
        
        if(p_ShowTitle)
          offset += p_TitleWidth;

        foreach(Control c in Controls)
        {
          if(c is HexViewCell)
          {
            if(position==parent.DataWidth)
            {
              position = 0;
              line++;
            }

            Point p = new Point((position*parent.CellWidth)+offset,(line*parent.CellHeight)+1);
            if(c.Location!=p)
              c.Location=p;
            position++;
          }
        }
      }
      #endregion
      #region Public Methods
      public void SetColorScheme(Color baseColor)
      {
        if(baseColor==Color.Red)
        {
          BackColor = Color.MistyRose;
          HighlightColor = Color.White;
          TitleForeColor = Color.White;
          TitleBackColor = Color.Red;
          BorderColor = Color.Red;

          Invalidate();
          return;
        }
        if(baseColor==Color.Blue)
        {
          TitleForeColor = Color.AliceBlue;
          TitleBackColor = Color.DarkBlue;
          HighlightColor = Color.White;
          BorderColor = Color.AliceBlue;

          Invalidate();
          return;
        }
        throw new Exception("Color scheme for color " + baseColor.ToString() + " does not exist");
      }

      public void Append(byte[] data)
      {
        foreach(byte b in data)
          Controls.Add(new HexViewCell(b));

        p_Data.AddRange(data);
        Invalidate();
      }
      #endregion
      #region Private Methods
      void DrawRotatedString(Graphics g, string text, Font font, Brush br, Rectangle rect, StringFormat format, float angle) 
      { 
        Point center = new Point(rect.X+rect.Width/2, rect.Y+rect.Height/2); 
        g.TranslateTransform(center.X, center.Y); 
        g.RotateTransform(angle); 
        rect.Offset(-center.X, -center.Y); 
        g.DrawString(text, font, br, rect, format); 
        g.ResetTransform(); 
      } 
      #endregion
    }
    #endregion
    #region HexViewCell Subcontrol
    public class HexViewCell : Control
    {
      #region Private Variables
      private StringFormat p_Format;
      private Rectangle p_InnerBounds;
      private Brush p_BackgroundBrush;
      #endregion
      #region Private Property Declarations
      private byte p_Data;
      private bool p_MouseOver;
      private Color p_HighlightColor = Color.White;
      #endregion
      #region Public Property Declarations
      public byte Data
      {
        get{return p_Data;}
        set{p_Data = value;}
      }
      #endregion
      #region Ctors
      public HexViewCell(byte data)
      {
        base.SetStyle(
          ControlStyles.UserPaint|
          ControlStyles.SupportsTransparentBackColor|
          ControlStyles.StandardClick|
          ControlStyles.ResizeRedraw|
          ControlStyles.Opaque|
          ControlStyles.FixedHeight|
          ControlStyles.FixedWidth|
          ControlStyles.AllPaintingInWmPaint|
          ControlStyles.UserMouse
          ,true);
        BackColor = Color.Transparent;

        Width = 1;
        Height = 1;

        p_Format = new StringFormat();
        p_Format.Alignment = StringAlignment.Center;

        p_Data = data;
      }
      #endregion

      #region Overrides
      protected override void OnParentBackColorChanged(EventArgs e)
      {
        p_BackgroundBrush = new SolidBrush(Parent.BackColor); 
        base.OnParentBackColorChanged (e);
      }

      protected override void OnPaint(PaintEventArgs e)
      {
        Graphics g = e.Graphics;


        if(Width==1&&Height==1)
        {
          HexView top = (HexView)((HexViewPane)Parent).Parent;
          Width=top.CellWidth;
          Height=top.CellHeight;
          p_InnerBounds = new Rectangle(0,0,Width,Height);
          p_BackgroundBrush = new SolidBrush(Parent.BackColor); 
        }

        if(p_MouseOver)
        {
          g.FillRectangle(new SolidBrush(p_HighlightColor),new Rectangle(0,0,Width,Height));
          //g.DrawRectangle(new Pen(new SolidBrush(((HexViewPane)Parent).BorderColor),1),new Rectangle(0,0,Width-1,Height-1));
          g.DrawRectangle(new Pen(new SolidBrush(Color.Black),1),new Rectangle(0,0,Width-1,Height-1));
        }
        else
          g.FillRectangle(p_BackgroundBrush,p_InnerBounds);

        g.DrawString(p_Data.ToString("X2"),Font,Brushes.Black,p_InnerBounds,p_Format);

        base.OnPaint (e);
      }

      protected override void OnMouseEnter(EventArgs e)
      {
        p_MouseOver = true;
        
        Invalidate();
        base.OnMouseEnter (e);
      }

      protected override void OnMouseLeave(EventArgs e)
      {
        p_MouseOver = false;

        Invalidate();
        base.OnMouseLeave (e);
      }
      #endregion
    }

    #endregion

    #region HexViewPaneCollection
    public class HexViewPaneCollection : CollectionBase
    {
      #region Private Property Declarations
      private HexView p_Parent;
      #endregion
      #region Ctors
      public HexViewPaneCollection(HexView parent)
      {
        p_Parent = parent;
      }
      #endregion

      #region Public Methods
      public int Add(HexViewPane pane)
      {
        p_Parent.Controls.Add(pane);
        return List.Add(pane);
      }

      public HexViewPane Add(string name, byte[] data)
      {
        HexViewPane added = new HexView.HexViewPane(name,data);
        p_Parent.Controls.Add(added);
        return added;
      }
      #endregion
    }
    #endregion
	}
}
