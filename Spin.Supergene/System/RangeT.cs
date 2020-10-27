using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public class Range<T>
  {
    #region Fields
    private T _start;
    private T _end;
    #endregion

    #region Properties

    public T Start
    {
      get { return _start; }
      set { _start = value; }
    }

    public T End
    {
      get { return _end; }
      set { _end = value; }
    }
    #endregion

    #region Constructors
    public Range()
    {
        
    }

    public Range(T start, T end)
    {
      _start = start;
      _end = end;
    }
    #endregion
  }
}
