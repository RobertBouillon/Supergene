using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public interface ITreeNodes : IList<ITreeNode>
  {
    //ITreeNode Add(string name);
    ITreeNode this[string name] { get; }
    void AddRange(IEnumerable<ITreeNode> nodes);

    event EventHandler<TreeNodeEventArgs> Added;
    event EventHandler<TreeNodeEventArgs> Removed;
  }
}
