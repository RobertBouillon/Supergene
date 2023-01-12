using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text;

public class DisplayFormat
{
  public static DisplayFormat Text = new DisplayFormat(DisplayFormatType.Text);
  public static DisplayFormat ID = new DisplayFormat(DisplayFormatType.Number, style: NumberStyles.Integer);
  public static DisplayFormat Number = new DisplayFormat(DisplayFormatType.Number, 0, 0);
  public static DisplayFormat Currency = new DisplayFormat(DisplayFormatType.Currency, 2, 0);
  public static DisplayFormat Currency0 = new DisplayFormat(DisplayFormatType.Currency, 0, 0);
  public static DisplayFormat Date = new DisplayFormat(DisplayFormatType.Date);
  public static DisplayFormat Time = new DisplayFormat(DisplayFormatType.Time);
  public static DisplayFormat Percent = new DisplayFormat(DisplayFormatType.Percentage, 2, 0);
  public static DisplayFormat Percent0 = new DisplayFormat(DisplayFormatType.Percentage, 0, 0);

  private DisplayFormatType _format;
  private int? _precision;
  private int? _scale;
  private NumberStyles? _numberStyle;


  //Properties are read-only to prevent a change to one display format from inadvertently affecting another reference to this instance.
  public int? Scale
  {
    get { return _scale; }
  }

  public DisplayFormatType Format
  {
    get { return _format; }
  }

  public int? Precision
  {
    get { return _precision; }
  }

  public NumberStyles? NumberStyle
  {
    get { return _numberStyle; }
  }

  public DisplayFormat(DisplayFormatType format, int? precision = null, int? scale = null, NumberStyles? style = null)
  {
    _format = format;
    _precision = precision;
    _scale = scale;
    _numberStyle = style;
  }

  public string ToString(object value)
  {
    switch (_format)
    {
      case DisplayFormatType.Date:
        return ((DateTime)value).ToLongDateString();
      case DisplayFormatType.Time:
        return ((DateTime)value).ToShortTimeString();
      case DisplayFormatType.Text:
        return value.ToString();
      case DisplayFormatType.Number:
        return String.Format("{0:g" + Precision.Value + "}", value);
      case DisplayFormatType.Decimal:
        return String.Format("{0:g" + Precision.Value + "}", value);
      case DisplayFormatType.Currency:
        return String.Format("{0:c" + Precision.Value + "}", value);
      case DisplayFormatType.Percentage:
        return String.Format("{0:p" + Precision.Value + "}", value);
      default:
        throw new NotImplementedException($"Format type '{_format}' not implemented");
    }
  }
}
