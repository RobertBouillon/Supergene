using System;
using System.ComponentModel;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Specialized;
using System.Collections;
using CISToolset.System.Windows.Forms;

namespace System.Windows.Forms
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>
  /// http://msdn.microsoft.com/en-us/library/8eyb2ct1.aspx
  /// </remarks>
  public partial class MruList : Component, IBindableComponent, INotifyPropertyChanged
  {
    //TODO: This has no designer support. Add it.
    #region Private Members
    private int _maxSize = 4;
    private MruListItems _itemCollection;
    private List<ToolStripItem> _items = new List<ToolStripItem>();
    private string _itemFormat = "{0}. {1}";
    private string _emptyCollectionText = "No Recent Files";
    private ToolStripItem _target;
    private int maxWidth = 40;
    private BindingContext _bindingContext;
    private ControlBindingsCollection _bindings;
    private MruChildBehavior _childBehavior = MruChildBehavior.AppendChildren;
    #endregion
    #region Public Property Declarations

    public MruChildBehavior ChildBehavior
    {
      get { return _childBehavior; }
      set { _childBehavior = value; }
    }

    [SettingsBindable(true)]
    public MruListItems ItemCollection
    {
      get { return _itemCollection; }
      set 
      {
        if (_itemCollection != null)
          _itemCollection.ListChanged -= new EventHandler(_itemCollection_ListChanged);

        _itemCollection.ListChanged += new EventHandler(_itemCollection_ListChanged);
        _itemCollection = value;
        PopulateDropdown();
      }
    }

    void _itemCollection_ListChanged(object sender, EventArgs e)
    {
      OnPropertyChanged("ItemCollection");
    }

    public int MaxWidth
    {
      get { return maxWidth; }
      set { maxWidth = value; }
    }

    public string ItemFormat
    {
      get { return _itemFormat; }
      set { _itemFormat = value; }
    }

    public int MaxSize
    {
      get { return _maxSize; }
      set { _maxSize = value; }
    }

    //public ToolStripMenuItem Target
    public ToolStripItem Target
    {
      get { return _target; }
      set 
      {
        if(value!=null)
        {
        _target = value;
        _target.Click += new EventHandler(_target_Click);
        PopulateDropdown();
        }
      }
    }

    void _target_Click(object sender, EventArgs e)
    {
      OnClick((string)_target.Tag);
    }

    public string EmptyCollectionText
    {
      get { return _emptyCollectionText; }
      set { _emptyCollectionText = value; }
    }
    #endregion

    #region Constructors
    public MruList(IContainer container)
    {
      container.Add(this);
      _bindings = new ControlBindingsCollection(this);
      if (_itemCollection == null)
        _itemCollection = new MruListItems();
      InitializeComponent();
    }
    #endregion
    #region Event Handles

    void button_Click(object sender, EventArgs e)
    {
      OnClick((string)((ToolStripItem)sender).Tag);
    }

    public void Clear()
    {
      _itemCollection.Clear();
      PopulateDropdown();
    }
    #endregion
    #region Events
    public event EventHandler<MruClickEventArgs> Click;

    #region MruClickEventArgs
    public class MruClickEventArgs : EventArgs
    {
      #region Private Fields
      private string _text;
      #endregion

      #region Public Property Declarations
      public string Text
      {
        get { return _text; }
        set { _text = value; }
      }
      #endregion

      #region Constructors
      public MruClickEventArgs(string text)
      {
        _text = text;
      }
      #endregion
    }
    #endregion

    protected virtual void OnClick(MruClickEventArgs e)
    {
      if (Click != null)
        Click(this, e);
    }

    private void OnClick(string text)
    {
      OnClick(new MruClickEventArgs(text));
    }
    #endregion
    #region Public Methods

    public void Add(string text)
    {
      if (_itemCollection.Contains(text))
        _itemCollection.Remove(text);

      _itemCollection.Insert(0, text);

      if (_itemCollection.Count > _maxSize)
        _itemCollection.RemoveAt(_itemCollection.Count - 1);

      PopulateDropdown();
    }

    public void PopulateDropdown()
    {
      if (_target == null || _itemCollection == null)
        return;

      if (_target.Owner == null)
        return;

      while (_items.Count > 0)
      {
        this._target.Owner.Items.Remove(_items[0]);
        _items.RemoveAt(0);
      }

      if (_itemCollection.Count == 0)
      {
        _target.Text = _emptyCollectionText;
        _target.Enabled = false;
        return;
      }

      _target.Enabled= true;

      int index = 0;
      int baseindex = _target.Owner.Items.IndexOf(_target);

      if (baseindex == -1)
        return;

      foreach (string text in _itemCollection)
      {
        string displaytext = GetDisplayText(index + 1, text);

        ToolStripMenuItem button = new ToolStripMenuItem(displaytext);
        button.Click += new EventHandler(button_Click);
        button.Tag = text;    //Keep the actual string seperate from any form-formatting.

        if (_childBehavior == MruChildBehavior.Replace)
        {
          if (index == 0)
          {
            _target.Text = GetDisplayText(1, text);
            _target.Tag = text;
            index++;
            continue;
          }

          _target.Owner.Items.Insert(baseindex + index++, button);
          _items.Add(button);
        }
        else
        {
          if (_target is ToolStripDropDownItem)
          {
            ToolStripDropDownItem dd = _target as ToolStripDropDownItem;
            dd.DropDownItems.Add(button);
          }
          else
          {
            throw new NotImplementedException();
          }
        }
      }
    }
    #endregion
    #region Private Methods

    private string GetDisplayText(int number, string text)
    {
      if (text.Length > maxWidth)
        text = "..." + text.Substring(text.Length - (maxWidth - 3));

      string ret = String.Format(_itemFormat, number, text);

      return ret;
    }
    #endregion

    #region IBindableComponent Members

    public BindingContext BindingContext
    {
      get
      {
        return _bindingContext;
      }
      set
      {
        _bindingContext = value;
      }
    }

    public ControlBindingsCollection DataBindings
    {
      get { return _bindings; }
    }

    #endregion

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, e);
    }
    #endregion
  }
}
