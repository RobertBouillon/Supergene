using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Windows.Forms
{
  [Serializable]
  public class MruListToolStripMenuItem : ToolStripMenuItem
  {
    #region Fields
    private string _itemFormat = "{0}. {1}";
    private MruList<string> _items = new MruList<string>();
    private Dictionary<String, ToolStripMenuItem> _mruItems = new Dictionary<string, ToolStripMenuItem>();
    private EventHandler _parentChangedHandler;
    private EventHandler<MruList<string>.MruListItemEventArgs> _mruListWatcher;
    #endregion
    #region Properties
    public MruList<string> Items
    {
      get { return _items; }
      set
      {
        if (_items != null)
        {
          _items.Added -= _mruListWatcher;
          _items.Removed -= _mruListWatcher;
        }

        _items = value;

        if (_items != null)
        {
          _items.Added -= _mruListWatcher;
          _items.Removed -= _mruListWatcher;
        }
        RenderMruItems();
      }
    }

    void _items_Added(object sender, MruList<string>.MruListItemEventArgs e)
    {
      RenderMruItems();
    }

    [DefaultValue("{0}. {1}")]
    public string ItemFormat
    {
      get { return _itemFormat; }
      set { _itemFormat = value; }
    }
    #endregion

    #region Constructors
    public MruListToolStripMenuItem()
    {
      _parentChangedHandler = new EventHandler(newParent_VisibleChanged);
      _mruListWatcher = new EventHandler<MruList<string>.MruListItemEventArgs>(_items_Added);
      Text = "No Recent Items";

    }
    #endregion

    #region Overrides
    protected override void OnClick(EventArgs e)
    {
      OnItemClicked(Tag as string);
      base.OnClick(e);
    }

    [DefaultValue("No Recent Items")]
    public override string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
      }
    }

    protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
    {
      if(oldParent!=null)
        oldParent.VisibleChanged -= _parentChangedHandler;
      if (newParent != null)
      {
        newParent.VisibleChanged += _parentChangedHandler;
        RenderMruItems();
      }

      base.OnParentChanged(oldParent, newParent);
    }

    void newParent_VisibleChanged(object sender, EventArgs e)
    {
      RenderMruItems();
    }

    public new ToolStripMenuItem OwnerItem
    {
      get
      {
        return base.OwnerItem as ToolStripMenuItem;
      }
    }

    #endregion

    #region Private Methods


    public void RenderMruItems()
    {
      foreach (string item in _mruItems.Keys.Where(x => !_items.Contains(x)))
        Owner.Items.Remove(_mruItems[item]);

      for (int i = 0; i < Items.Count; i++)
      {
        if (i == 0)
          RenderItem(this,i+1, Items[i]);
        else
        {
          //ToolStripMenuItem item = (ToolStripMenuItem) Owner.Items.Add(Items[i]);
          if (_mruItems.ContainsKey(Items[i]))
            continue;

          ToolStripMenuItem item = new ToolStripMenuItem();
          RenderItem(item, i+1, Items[i]);
          Owner.Items.Insert(ThisIndex + Items.Count - 1, item);

          _mruItems.Add(Items[i], item);
          item.Click += new EventHandler((x, y) => { OnItemClicked(Items[i]); });
        }
      }
    }

    private void RenderItem(ToolStripMenuItem item, int index, string text)
    {
      item.Text = String.Format(ItemFormat, index, text);
      item.Enabled = true;
      item.Tag = text;
    }

    void item_Click(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }

    private int ThisIndex
    {
      get
      {
        return OwnerItem.DropDownItems.IndexOf(this);
      }
    }

    #endregion
    #region Events

    #region ItemClickedEventArgs Subclass
    public class ItemClickedEventArgs : EventArgs
    {
      #region Fields
      private readonly string _item;
      #endregion
      #region Properties
      public string Item
      {
        get { return _item; }
      }
      #endregion
      #region Constructors
      internal ItemClickedEventArgs(string item)
      {
        #region Validation
        if (item == null)
          throw new ArgumentNullException("item");
        #endregion
        _item = item;
      }
      #endregion
    }
    #endregion

    public event EventHandler<ItemClickedEventArgs> ItemClicked;

    protected void OnItemClicked(string item)
    {
      OnItemClicked(new ItemClickedEventArgs(item));
    }

    protected virtual void OnItemClicked(ItemClickedEventArgs e)
    {
      if (ItemClicked != null)
        ItemClicked(this, e);
    }


    #endregion
  }
}
