using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization;

public interface IDeserializer<TEntity, TReader>
{
  TEntity Create(TReader reader);
}
