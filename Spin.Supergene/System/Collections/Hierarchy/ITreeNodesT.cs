using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public interface ITreeNodes<T> where T: ITreeNode
  {
    T Add(string name);
    T this[string name] { get; }
  }
}
