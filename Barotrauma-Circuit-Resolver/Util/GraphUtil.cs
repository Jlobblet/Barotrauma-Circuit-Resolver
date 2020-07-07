using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using QuickGraph;
using System.Xml.XPath;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        public static IEnumerable<string> GetNextComponentIDs(this XDocument submarine, XElement element)
        {
            return submarine.Elements()
                            .Where(e => e.XPathSelectElements("//input/link")
                                         .Select(i => element.XPathSelectElements("//output/link")
                                                              .Select(o => (o.Attribute("w")?.Value))
                                                              .Contains(i.Attribute("w")?.Value))
                                         .Any())
                            .Select(e => e.Attribute("ID").Value);
        }
    }
}
