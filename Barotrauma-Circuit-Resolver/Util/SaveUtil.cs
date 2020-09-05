using System.Collections;
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
            static string EdgeIdentifier(Util.Edge<Vertex> e) => e.ToString();

            using FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
            using XmlWriter xw = XmlWriter.Create(fs);
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

            IEnumerable<(int First, int Second)> ids = sortedVertices.Select(v => v.Id)
                .Zip(graph.Vertices.Select(v => v.Id));
            IEnumerable<Triplet<int, string, int>> idChangeTriplet =
                ids.Select(e => new Triplet<int, string, int>(e.First,
                               sortedVertices.First(v => v.Id == e.First)
                                             .GetStringHashCode(),
                               e.Second));

            foreach (XObject xObject in (IEnumerable)submarine.XPathEvaluate(xpath))
            {
                if (!(xObject is XAttribute attribute))
                {
                    continue;
                }

                int id = int.Parse(attribute.Value);
                if (idChangeTriplet.Any(t => t.First == id))
                {
                    attribute.Value = idChangeTriplet.First(t => t.First == id).Second;
                }
            }

            foreach (XObject xObject in (IEnumerable)submarine.XPathEvaluate(xpath))
            {
                if (!(xObject is XAttribute attribute))
                {
                    continue;
                }

                string hash = attribute.Value;
                if (idChangeTriplet.Any(t => t.Second == hash))
                {
                    attribute.Value = idChangeTriplet.First(t => t.Second == hash).Third
                                                     .ToString();
                }
            }
        }
    }
}
