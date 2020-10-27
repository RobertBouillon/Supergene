using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;

namespace System.Diagnostics
{
  public class DataTableTraceListener : TraceListener
  {
    #region Fields
    private readonly DataTableTraceListenerData.LogDataTable _table;
    private string _defaultCategory = "General";

    #endregion
    #region Properties
    public string DefaultCategory
    {
      get { return _defaultCategory; }
      set { _defaultCategory = value; }
    }

    public DataTableTraceListenerData.LogDataTable DataTable
    {
      get { return _table; }
    }
    #endregion
    #region Constructors
    public DataTableTraceListener() : this(new DataTableTraceListenerData.LogDataTable())
    {
    }

    public DataTableTraceListener(DataTableTraceListenerData.LogDataTable table)
    {
      #region Validation
      if (table == null)
        throw new ArgumentNullException("table");
      #endregion
      _table = table;
    }
    #endregion

    #region Overrides

    public override void Write(string message)
    {
      lock(this)
        Write(message, _defaultCategory);
    }

    public override void WriteLine(string message)
    {
      lock (this)
        WriteLine(message, _defaultCategory);
    }

    public override void Write(object o, string category)
    {
      lock (this)
        Write(o.ToString(), category);
    }

    public override void Write(string message, string category)
    {
      lock (this)
        _table.AddLogRow(DateTime.Now, category, message);
    }

    public override void WriteLine(object o, string category)
    {
      lock (this)
        WriteLine(o.ToString(), category);
    }

    public override void WriteLine(string message, string category)
    {
      lock (this)
        Write(message + System.Environment.NewLine, category);
    }
    #endregion
  }
}
