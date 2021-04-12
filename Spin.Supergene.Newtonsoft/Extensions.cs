using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Linq
{
  public static class Extensions
  {
    public static FluentPropertyParser<T> ParseProperty<T>(this JObject source, string property, bool required = true) => new FluentPropertyParser<T>(source, property, required);
  }
}
