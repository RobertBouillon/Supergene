using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Attempt<T> : Attempt
    {
        public T Result { get; }

        private Attempt(T result) : base(true) => Result = result;
        public Attempt(T result, Func<T, bool> success) : base(success(result)) => Result = result;
        public Attempt(bool success) : base(success) { if (success) throw new Exception("A value must be returned if the operation succeeded"); }
        public Attempt(string error) : base(error) { }
        public Attempt(Exception ex) : base(ex) { }

        public T OrDefault(T defaultValue) => Success ? Result : defaultValue;

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

        public new T Assert()
        {
            base.Assert();
            return Result;
        }

        public static implicit operator Attempt<T>(T d) => new(d);
    }
}
