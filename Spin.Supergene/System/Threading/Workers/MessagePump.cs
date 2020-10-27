using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Workers
{
  public class MessagePump : QueueWorker<Delegate>
  {
    #region Fields
    private System.ComponentModel.ISynchronizeInvoke _context;
    private object[] _emptyArgs = new object[] { };
    #endregion
    #region Constructors

    public MessagePump(System.ComponentModel.ISynchronizeInvoke context) : this()
    {
      #region Validation
      if (context == null)
        throw new ArgumentNullException("context");
      #endregion
      _context = context;
    }

    public MessagePump() : base("Message Pump") { }
    #endregion


    #region Overrides

    public override void Work(Delegate item)
    {
      if (_context != null && _context.InvokeRequired)
      {
        _context.Invoke(item, _emptyArgs);
        return;
      }

      item.DynamicInvoke(_emptyArgs);
    }
    #endregion
  }
}
