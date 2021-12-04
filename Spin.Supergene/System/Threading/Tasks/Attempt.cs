using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
  public class Attempt
  {
    public static Attempt Successful => new(true);
    public static Task<Attempt> AsTask(string error) => Task<Attempt>.FromResult(new Attempt(error));
    public static Task<Attempt> AsTask(Exception error) => Task<Attempt>.FromResult(new Attempt(error));
    public static Task<Attempt> AsTask(bool success = true) => Task<Attempt>.FromResult(new Attempt(success));

    public bool Success { get; }
    public Exception Exception { get; }
    public string Error { get; }

    public Attempt(bool success) => Success = success;
    public Attempt(string error) => Error = error;
    public Attempt(Exception ex) => Exception = ex;
  }
}
