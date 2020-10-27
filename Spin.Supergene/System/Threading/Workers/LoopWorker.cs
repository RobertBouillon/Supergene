using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Workers
{
  //Obsoleted by the TPL. Excluded from project.
  public class LoopWorker<T> : Worker
  {
    #region Fields
    private Action<T> _action;
    private int _position;
    private IList<T> _collection;
    private int _count;
    #endregion
    public int Position
    {
      get { return _position; }
      set { _position = value; }
    }

    #region Properties
    public Action<T> Action
    {
      get { return _action; }
      set { _action = value; }
    }

    public IList<T> Collection
    {
      get { return _collection; }
      set { _collection = value; }
    }
    #endregion

    protected LoopWorker(string name)
      : base(name)
    {
    }

    public LoopWorker(string name, IEnumerable<T> collection)
      : base(name)
    {
      _collection = new List<T>(collection);
    }

    public LoopWorker(string name, IList<T> collection, Action<T> action)
      : base(name)
    {
      _collection = collection;
      _action = action;
    }

    protected override void CancelWork()
    {
    }

    protected override void Work()
    {
      var count = _collection.Count;
      if (count == 0)
        return;

      if (++_position == count)
        _position = 0;

      if(_action!=null)
        _action(_collection[_position]);
    }


  }
}
