using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
  public class Attempt
  {
    public static Attempt Successful { get; } = new(true);
    public static Attempt Failure { get; } = new(false);
        public static Task<Attempt> AsTask(string error) => Task<Attempt>.FromResult(new Attempt(error));
    public static Task<Attempt> AsTask(Exception error) => Task<Attempt>.FromResult(new Attempt(error));
    public static Task<Attempt> AsTask(bool success = true) => Task<Attempt>.FromResult(new Attempt(success));

    private string _error;
    public bool Success { get; }
    public Exception Exception { get; }
    public string Error => _error ?? Exception?.Message;

    public Attempt(bool success) => Success = success;
    public Attempt(string error) => _error = error;
    public Attempt(Exception ex) => Exception = ex;

    public void Assert()
    {
      if (!Success)
        throw Exception ?? new Exception(Error ?? "Attempt failed");
    }

    public static implicit operator bool(Attempt d) => d.Success;

    public override string ToString() => Exception?.Message ?? Error;
  }
}
