using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace System.Xml;

public static class XmlReaderExtensions
{
  public static void ForEachAttribute(this XmlReader reader, Action<string> action)
  {
    if (reader.NodeType == XmlNodeType.EndElement)
      return;

    if (reader.MoveToFirstAttribute())
      do
        action(reader.LocalName);
      while (reader.MoveToNextAttribute());

    //DO NOT DO THIS.
    //If the element has child elements, this moves to the first child, throwing off ForEachElement interations.
    //If no children are expected, use reader.Read to move on to the next element.
    //reader.Read(); //When MoveToNext returns false, it remains on the last attribute.
  }

  public static void ForEachElement(this XmlReader reader, Action<string> action)
  {
    if (reader.IsStartElement() && reader.IsEmptyElement)
      return;

    while (reader.Read())
    {
      if (reader.NodeType == XmlNodeType.EndElement)
        return;
      else if (reader.NodeType == XmlNodeType.Element)
        if (reader.IsStartElement())
          action(reader.LocalName);
    }

    //while (reader.Read())
    //  if (reader.NodeType == XmlNodeType.EndElement)
    //    return;
    //  else if (reader.IsStartElement())
    //    action(reader.LocalName);
  }
}
