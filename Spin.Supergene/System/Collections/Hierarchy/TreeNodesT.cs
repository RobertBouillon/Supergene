using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Hierarchy
{
  public abstract class TreeNodes<T> : TreeNodes where T: ITreeNode
  {
    public TreeNodes()
    {

    }

    public TreeNodes(IEnumerable<ITreeNode> source) : base(source)
    {

    }

    #region Overrides
    public abstract T Add(string name);

    //public override ITreeNode this[string name]
    //{
    //  get { return null; }
    //}

    public virtual new T this[string name]
    {
      get { return (T)base[name]; }
    }

    public new int IndexOf(T item)
    {
      return base.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      base.Insert(index, item);
    }

    new T this[int index]
    {
      get
      {
        return (T)base[index];
      }
      set
      {
        base[index] = value;
      }
    }

    public void Add(T item)
    {
      base.Add(item);
    }

    public bool Contains(T item)
    {
      return base.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      base.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      return base.Remove(item);
    }

    public new IEnumerator<T> GetEnumerator()
    {
      //return (IEnumerator<T>) base.GetEnumerator();
      for (int i = 0; i < Count; i++)
        yield return this[i];
    }
    #endregion
  }
}
