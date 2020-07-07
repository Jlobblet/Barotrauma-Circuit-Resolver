using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using QuickGraph;
using System.Xml.XPath;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        static readonly string[] PowerConnections = { "power", "power_in", "power_out" };
        private static bool FilterPower(XElement elt) => !PowerConnections.Contains(elt.Attribute("name").Value);

        public static IEnumerable<Vertex> GetEntryPoints(this XDocument submarine)
        {
            return submarine.Root.Elements()
                                 .Where(e =>
                                    !(e.Descendants("input").Where(FilterPower)
                                            .Elements("link").Any()
                                        && e.Descendants("input").Any())
                                    && e.Descendants("output").Where(FilterPower)
                                        .Elements("link").Any())
                                 .Select(e => new Vertex(int.Parse(e.Attribute("ID").Value), e.Attribute("identifier").Value));
        }


        public static IEnumerable<Vertex> GetNextComponentIDs(this XDocument submarine, Vertex vertex)
        {
            return submarine.GetNextComponentIDs(submarine.Root.Elements()
                .Where(e => e.Attribute("ID").Value == vertex.Id.ToString())
                .First());
        }

        public static IEnumerable<Vertex> GetNextComponentIDs(this XDocument submarine, XElement element)
        {
            return submarine.Root.Elements()
                .Where(e => element.Descendants("output").Where(FilterPower).Elements("link")
                    .Select(e => e.Attribute("w").Value)
                    .Intersect(e.Descendants("input").Elements("link")
                        .Select(i => i.Attribute("w").Value))
                    .Any())
                .Select(e => new Vertex(int.Parse(e.Attribute("ID").Value), e.Attribute("identifier").Value)); 
        }

        public static void AddDownstreamComponents (this AdjacencyGraph<Vertex, Edge<Vertex>> graph, XDocument submarine, Vertex vertex)
        {
            graph.AddVertex(vertex);
            IEnumerable<Vertex> next = submarine.GetNextComponentIDs(vertex);
            foreach (Vertex downstreamComponent in next)
            {
                if (!graph.TryGetEdge(vertex, downstreamComponent, out Edge<Vertex> _))
                {
                    Edge<Vertex> edge = new Edge<Vertex>(graph.EdgeCount, string.Format("{0}-{1}", vertex.Id, downstreamComponent.Id), vertex, downstreamComponent);
                    graph.AddEdge(edge);
                    vertex.Edges.Add(edge);
                    downstreamComponent.Edges.Add(edge);
                    graph.AddDownstreamComponents(submarine, downstreamComponent);
                }
            }
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(XDocument submarine)
        {
            IEnumerable<Vertex> entryPoints = submarine.GetEntryPoints();
            var graph = new AdjacencyGraph<Vertex, Edge<Vertex>> ();
            foreach (Vertex entryPoint in entryPoints)
            {
                graph.AddDownstreamComponents(submarine, entryPoint);
            }
            return graph;
        }
    }
}
