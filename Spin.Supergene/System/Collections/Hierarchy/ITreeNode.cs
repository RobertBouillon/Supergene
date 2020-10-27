using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public interface ITreeNode
  {
    ITreeNodes Children { get; }
    string FullPath { get; }
    string Name { get; set; }
    ITreeNode Parent { get; set; }
    ITreeNode this[string childName] { get; }

    ITreeNodes Traverse();
    void Traverse(ITreeNodes nodes);
  }
}
