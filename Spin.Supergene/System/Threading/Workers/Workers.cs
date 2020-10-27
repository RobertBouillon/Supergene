using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Workers
{
  public class Workers : CollectionBase<Worker>
  {
    #region Constructors
    public Workers()
    {

    }
    public Workers(IEnumerable<Worker> source) : base(source)
    {

    }
    #endregion

    #region Methods
    public bool Start()
    {
      Stack<Worker> started = new Stack<Worker>();
      bool error = false;

      foreach (Worker w in this)
      {
        try
        {
          w.Start();
        }
        catch (Exception)
        {
          error = true;
        }

        if (error)
        {
          foreach (Worker sw in started)
            sw.Stop(TimeSpan.FromMilliseconds(Timeout.Infinite), true);

          return false;
        }
      }
      return true;
    }

    public void Stop()
    {
      foreach (Worker w in this)
        w.Stop(TimeSpan.FromMilliseconds(Timeout.Infinite), true);
    }
    #endregion
  }
}
