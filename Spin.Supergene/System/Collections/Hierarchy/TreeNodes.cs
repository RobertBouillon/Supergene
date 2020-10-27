using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public class TreeNodes : CollectionBase<ITreeNode>, ITreeNodes
  {
    #region Fields

    #endregion

    #region Constructors
    public TreeNodes()
    {

    }

    public TreeNodes(IEnumerable<ITreeNode> node) : base(node)
    {

    }
    #endregion

    #region Properties

    public virtual char PathDelimiter
    {
      get { return '\\'; }
    }
    #endregion

    #region Indexers
    public virtual ITreeNode this[string name]
    {
      get
      {
        var i = name.IndexOf(PathDelimiter);
        if (i < 0)
          return this.FirstOrDefault(x => x.Name == name);
        else
        {
          var shortname = name.Substring(0, i);
          var child = name.Substring(i + 1, name.Length - i - 1);
          var ret = this.FirstOrDefault(x => x.Name == name);
          return ret.Children[shortname];
        }
      }
    }
    #endregion
    #region Methods
    
    #endregion

    #region Events
    public new event EventHandler<TreeNodeEventArgs> Removed;
    public event EventHandler<TreeNodeEventArgs> Added;

    protected virtual void OnRemoved(TreeNodeEventArgs e)
    {
      if (Removed != null)
        Removed(this, e);
    }

    protected virtual void OnAdded(TreeNodeEventArgs e)
    {
      if (Added != null)
        Added(this, e);
    }


    #endregion

    #region Overrides
    protected override void OnInserted(int index, ITreeNode value)
    {
      OnAdded(new TreeNodeEventArgs(value));
      base.OnInserted(index, value);
    }

    protected override void OnRemoved(int index, ITreeNode value)
    {
      OnRemoved(new TreeNodeEventArgs(value));
      base.OnRemoved(index, value);
    }
    #endregion


    //public event EventHandler<TreeNodeEventArgs> Added;
    //public new event EventHandler<TreeNodeEventArgs> Removed;
  }
}
