﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using QuickGraph;
using QuickGraph.Serialization;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class SaveUtil
    {
        public static void SaveGraphML(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            static string VertexIdentity(Vertex v) => v.ToString();
            static string EdgeIdentifier(Edge<Vertex> e) => e.ToString();

            using var fs = new FileStream(filepath, FileMode.OpenOrCreate);
            using var xw = XmlWriter.Create(fs);
            graph.SerializeToGraphML(xw, VertexIdentity,
                                     (EdgeIdentity<Vertex, Edge<Vertex>>)
                                     EdgeIdentifier);
        }

        public static void UpdateSubmarineIDs(this XDocument submarine,
                                              AdjacencyGraph<Vertex, Edge<Vertex>>
                                                  graph,
                                              IEnumerable<Vertex> sortedVertices)
        {
            const string xpath = "//Item/@ID|//link/@w";

            Dictionary<int, int> ids = sortedVertices.Select(v => v.Id)
                                                     .Zip(graph.Vertices.Select(v => v.Id))
                                                     .ToDictionary(k => k.First, v => v.Second);

            foreach (XObject xObject in (IEnumerable)submarine.XPathEvaluate(xpath))
            {
                if (!(xObject is XAttribute attribute))
                {
                    continue;
                }

                int id = int.Parse(attribute.Value);
                if (ids.Any(t => t.Key == id))
                {
                    attribute.Value = ids.First(t => t.Value == id).Value.ToString();
                }
            }
        }
    }
}
