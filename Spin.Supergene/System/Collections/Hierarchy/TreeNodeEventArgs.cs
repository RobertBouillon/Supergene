using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public class TreeNodeEventArgs : EventArgs
  {
    #region Properties
    public ITreeNode Node { get; private set; }
    #endregion
    #region Constructors
    public TreeNodeEventArgs(ITreeNode node)
    {
      #region Validation
      if (node == null)
        throw new ArgumentNullException("node");
      #endregion
      Node = node;
    }
    #endregion
  }
}
