using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization
{
  public interface ISerializer<TEntity, TWriter>
  {
    void Write(TEntity entity, TWriter writer);
  }
}
