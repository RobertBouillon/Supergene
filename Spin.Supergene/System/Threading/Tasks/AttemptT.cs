using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
  public class Attempt<T> : Attempt
  {
    public T Result { get; }

    private Attempt(T result) : base(true) => Result = result;
    public Attempt(T result, Func<T, bool> success) : base(success(result)) => Result = result;
    public Attempt(bool success) : base(success) { if (success) throw new Exception("A value must be returned if the operation succeeded"); }
    public Attempt(string error) : base(error) { }
    public Attempt(Exception ex) : base(ex) { }

    public bool Failed(out T result)
    {
      result = Result;
      return !Success;
    }

    public bool Succeeded(out T result)
    {
      result = Result;
      return Success;
    }

    public static implicit operator Attempt<T>(T d) => new(d);
  }
}
