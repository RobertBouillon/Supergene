using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public class TreeNode<TNode, TNodes> : TreeNode where TNode : ITreeNode where TNodes : ITreeNodes
  {
    #region Constructors
    public TreeNode(string name, TNodes children) : base(name, children) { }

    public TreeNode(string name, TNode parent, TNodes children) : base(name, parent, children) { }
    #endregion
    #region New Overloads
    public new TNodes Children => (TNodes)base.Children;
    public new TNode Parent
    {
      get => (TNode)base.Parent;
      set => base.Parent = (TNode)value;
    }
    public new TNode this[string childName] => (TNode)base[childName];
    #endregion
  }
}
