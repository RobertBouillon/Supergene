using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms
{
	/// <summary>
	/// Summary description for ImageColumnStyle.
	/// </summary>
	public class ImageColumnStyle : DataGridColumnStyle
	{
    #region Private Property Declarations
    private int p_ForceImageIndex = -1;
    private ImageList p_ImageList;
    #endregion
    #region Public Property Declarations
    public int ForceImageIndex
    {
      get{return p_ForceImageIndex;}
      set{p_ForceImageIndex=value;}
    }

    public ImageList ImageList
    {
      get{return p_ImageList;}
      set
      {
        p_ImageList = value;
        Invalidate();
      }
    }
    #endregion
    #region Ctors
		public ImageColumnStyle()
		{
		}
    #endregion

    #region Overrides
    [System.ComponentModel.ReadOnly(true)]
    public override bool ReadOnly
    {
      get{return true;}
      set{throw new NotSupportedException();}
    }

    protected override void Abort(int rowNum)
    {
    }

    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly)
    {
    }

    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText)
    {
    }

    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
    {
    }


    protected override bool Commit(CurrencyManager dataSource, int rowNum)
    {
      return true;
    }

    protected override int GetMinimumHeight()
    {
      return 16;
    }

    protected override int GetPreferredHeight(Graphics g, object value)
    {
      return 16;
    }

    protected override Size GetPreferredSize(Graphics g, object value)
    {
      return new Size(16, 16);
    }

    protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
    {
      int index = 0;
      if(p_ForceImageIndex>=0)
      {
        index = p_ForceImageIndex;
      }
      else
      {
        index = (int)GetColumnValueAtRow(source,rowNum);
        if(index<0)
          return;
      }
      if(index>p_ImageList.Images.Count-1)
        throw new IndexOutOfRangeException(String.Format("Image index at row {0} for column {1} was greater than the number of images in the Image List specified",rowNum,MappingName));
      Image image = p_ImageList.Images[index];

      int left = 0;
      switch(Alignment)
      {
        case HorizontalAlignment.Left:
          left = 0;
          break;
        case HorizontalAlignment.Center:
          left = (Width / 2) - (image.Width / 2);
          break;
        case HorizontalAlignment.Right:
          left = Width - image.Width;
          break;
      }

      g.FillRectangle(backBrush,bounds);  //Paint the bg
      g.DrawImage(image,bounds,left,0,16,16,GraphicsUnit.Pixel);
    }

    protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
    {
      Paint(g,bounds,source,rowNum,false);
    }

    protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
    {
      Paint(g,bounds,source,rowNum,Brushes.White,Brushes.Black,alignToRight);
    }
    #endregion
	}
}
