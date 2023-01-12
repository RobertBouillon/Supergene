using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace System.Diagnostics;

public class TraceLog : TraceSource
{
  #region Fields
  #endregion
  #region Properties
  #endregion
  #region Constructors
  public TraceLog(string name) : base(name)
  {
  }

  public TraceLog(string name, SourceLevels sourceLevels) : base(name, sourceLevels)
  {
  }
  #endregion
  #region Public Methods
  public void Write(string format, params object[] formatParameters)
  {
    Write(String.Format(format, formatParameters));
  }

  public void Write(string message)
  {
    TraceEvent(TraceEventType.Verbose, -1, message);
  }

  public void Write(TraceEventType evt, string message)
  {
    TraceEvent(evt, -1, message);
  }

  public void Write(TraceEventType evt, string format, params object[] formatParameters)
  {
    TraceEvent(evt, -1, String.Format(format, formatParameters));
  }

  public void Write(Exception ex)
  {
    TraceData(TraceEventType.Error, -1, ex);
  }

  public void Write(Exception ex, string message)
  {
    TraceEvent(TraceEventType.Error, -1, message);
    TraceData(TraceEventType.Error, -1, ex);
  }

  public void Write(Exception ex, string format, params object[] formatParameters)
  {
    TraceEvent(TraceEventType.Error, -1, String.Format(format, formatParameters));
    TraceData(TraceEventType.Error, -1, ex);
  }
  #endregion
}
