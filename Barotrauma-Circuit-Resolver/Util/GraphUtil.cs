using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        private static readonly string[] PowerConnections = { "power", "power_in", "power_out" };

        private static bool FilterPower(XElement elt)
        {
            return !PowerConnections.Contains(elt.Attribute("name").Value);
        }

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

        public static IEnumerable<Vertex> GetNextVertices(this XDocument submarine, Vertex vertex)
        {
            return submarine.GetNextVertices(submarine.Root.Elements()
                .Where(e => e.Attribute("ID").Value == vertex.Id.ToString())
                .First());
        }

        public static IEnumerable<Vertex> GetNextVertices(this XDocument submarine, XElement element)
        {
            return submarine.Root.Elements()
                .Where(e => element.Descendants("output").Where(FilterPower).Elements("link")
                    .Select(e => e.Attribute("w").Value)
                    .Intersect(e.Descendants("input").Elements("link")
                        .Select(i => i.Attribute("w").Value))
                    .Any())
                .Select(e => new Vertex(int.Parse(e.Attribute("ID").Value), e.Attribute("identifier").Value));
        }

        public static void AddDownstreamComponents(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, XDocument submarine, Vertex vertex)
        {
            graph.AddVertex(vertex);
            IEnumerable<Vertex> next = submarine.GetNextVertices(vertex);
            foreach (Vertex downstreamComponent in next)
            {
                if (!graph.TryGetEdge(vertex, downstreamComponent, out Edge<Vertex> _))
                {
                    Edge<Vertex> edge = new Edge<Vertex>(graph.EdgeCount, vertex, downstreamComponent);
                    graph.AddEdge(edge);
                    graph.AddDownstreamComponents(submarine, downstreamComponent);
                }
            }
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(XDocument submarine)
        {
            IEnumerable<Vertex> entryPoints = submarine.GetEntryPoints();
            AdjacencyGraph<Vertex, Edge<Vertex>> graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            graph.EdgeAdded += Graph_EdgeAdded;
            foreach (Vertex entryPoint in entryPoints)
            {
                graph.AddDownstreamComponents(submarine, entryPoint);
            }
            return graph;
        }

        private static void Graph_EdgeAdded(Edge<Vertex> e)
        {
            e.Source.OutgoingEdges.Add(e);
            e.Target.IncomingEdges.Add(e);
        }

        public static void SortVertexIDs(this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            var Ids = graph.Vertices.Select(v => v.Id);
            var SortedIds = Ids.OrderBy(i => i);

            var EdgesBefore = graph.Edges;

            foreach ((Vertex v, int newId) in graph.Vertices.Zip(SortedIds))
            {
                v.Id = newId;
            }

            var EdgesAfter = graph.Edges;
        }
    }
}
