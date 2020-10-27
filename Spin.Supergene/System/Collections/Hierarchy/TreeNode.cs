using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.Collections.Hierarchy
{
  public abstract class TreeNode : ITreeNode
  {
    #region Fields
    private ITreeNode _parent;
    private string _name;

    private ITreeNodes _children;
    #endregion

    #region Properties

    public ITreeNode Parent
    {
      get { return _parent; }
      set
      {
        if (_parent != null)
          _parent.Children.Remove(this);

        _parent = value;

        if (_parent != null)
          _parent.Children.Add(this);
      }
    }

    public ITreeNodes Children
    {
      get { return _children; }
      protected set { _children = value; }
    }

    public string Name
    {
      get { return _name; }
      set
      {
        //if (_name.Contains(PathDelimiter))
        //  throw new Exception("Name cannot contain a PathDelimiter character");
        if (String.IsNullOrWhiteSpace(value))
          throw new ArgumentException("Name is required");
        _name = value;
      }
    }

    public virtual string FullPath => String.Join("\\", this.Traverse<ITreeNode>(x => x.Parent).Select(x => x.Name));

    #endregion

    #region Constructors
    public TreeNode(string name)
    {
      Name = name;
      _children = new TreeNodes();
    }

    public TreeNode(string name, ITreeNodes children)
    {
      Name = name;
      _children = children;
    }

    public TreeNode(string name, ITreeNode parent, ITreeNodes children)
      : this(name)
    {
      Parent = parent;
      _children = children;
    }

    public TreeNode(string name, ITreeNode parent)
      : this(name)
    {
      Parent = parent;
      _children = new TreeNodes();
    }
    #endregion

    #region Indexers
    public ITreeNode this[string childName]
    {
      get
      {
        return _children[childName];
      }
    }
    #endregion

    #region Abstract Declarations
    #endregion

    #region Methods

    public ITreeNodes Traverse()
    {
      TreeNodes ret = new TreeNodes { this };
      Traverse(ret);
      return ret;
    }

    public void Traverse(ITreeNodes nodes)
    {
      nodes.AddRange(_children);
      foreach (var node in _children)
        node.Traverse(nodes);
    }
    #endregion
  }
}
