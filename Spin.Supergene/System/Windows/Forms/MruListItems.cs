using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Forms
{
  public class MruListItems : CollectionBase<string>
  {
    #region Constructors
    internal MruListItems()
    {

    }
    #endregion


    #region Overrides
    protected override void OnClearComplete()
    {
      OnListChanged();
      base.OnClearComplete();
    }

    protected override void OnInserted(int index, string value)
    {
      OnListChanged();
      base.OnInserted(index, value);
    }

    protected override void OnRemoved(int index, string value)
    {
      OnListChanged();
      base.OnRemoved(index, value);
    }

    protected override void OnSetComplete(int index, string oldValue, string newValue)
    {
      OnListChanged();
      base.OnSetComplete(index, oldValue, newValue);
    }
    #endregion
    #region Events

    public event EventHandler ListChanged;

    protected void OnListChanged()
    {
      OnListChanged(EventArgs.Empty);
    }

    protected virtual void OnListChanged(EventArgs e)
    {
      if (ListChanged != null)
        ListChanged(this, e);
    }
    #endregion
  }
}
