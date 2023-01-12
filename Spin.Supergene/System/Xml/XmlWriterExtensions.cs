using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Xml;

public static class XmlWriterExtensions
{
  public static void WriteElement(this XmlWriter writer, string name, Action action)
  {
    writer.WriteStartElement(name);
    action();
    writer.WriteEndElement();
  }
}
