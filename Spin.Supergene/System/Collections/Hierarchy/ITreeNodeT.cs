using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public interface ITreeNode<TNode, TNodes> : ITreeNode where TNode: ITreeNode where TNodes: ITreeNodes
  {
    new TNodes Children { get; }
    new TNode Parent { get; set; }
    new TNode this[string childName] { get; }
  }
}
