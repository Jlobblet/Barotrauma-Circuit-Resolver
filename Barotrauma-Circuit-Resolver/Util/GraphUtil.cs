using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        public enum Mark
        {
            Unmarked,
            Temporary,
            Permanent
        }

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

        public static void AddDownstreamComponents(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, XDocument submarine, Vertex vertex)
        {
            graph.AddVertex(vertex);
            IEnumerable<Vertex> next = submarine.GetNextComponentIDs(vertex);
            foreach (Vertex downstreamComponent in next)
            {
                if (!graph.TryGetEdge(vertex, downstreamComponent, out Edge<Vertex> _))
                {
                    Edge<Vertex> edge = new Edge<Vertex>(graph.EdgeCount, string.Format("{0}-{1}", vertex.Id, downstreamComponent.Id), vertex, downstreamComponent);
                    graph.AddEdge(edge);
                    graph.AddDownstreamComponents(submarine, downstreamComponent);
                }
            }
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(XDocument submarine)
        {
            IEnumerable<Vertex> entryPoints = submarine.GetEntryPoints();
            AdjacencyGraph<Vertex, Edge<Vertex>> graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();
            graph.EdgeAdded += Graph_EdgeAdded;
            foreach (Vertex entryPoint in entryPoints)
            {
                graph.AddDownstreamComponents(submarine, entryPoint);
            }
            return graph;
        }

        private static void Graph_EdgeAdded(Edge<Vertex> e)
        {
            var source = e.Source;
            var target = e.Target;
            source.Edges.Add(e);
            target.Edges.Add(e);
        }

        public static IEnumerable<Vertex> getNextVertices(this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, Vertex vertex)
        {
            // Get outgoing edges
            if(componentGraph.TryGetOutEdges(vertex, out IEnumerable<Edge<Vertex>> nextEdges)){
                // Return nodes connected to these edges
                return nextEdges.Select(e => e.Target);
            }

            return Enumerable.Empty<Vertex>();
        }

        public static bool VisitDownstream(Vertex vertex, Guid[] sorted, ref int head, AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, Dictionary<Guid, Mark> marks)
        {
            // Assign the new IDs to the vertices
            if (marks.ContainsKey(vertex.Guid))
            {
                if (marks[vertex.Guid] == Mark.Permanent)
                {
                    // End of branch
                    return true;
                }
                if (marks[vertex.Guid] == Mark.Temporary)
                {
                    // Graph is not Acyclic
                    return false;
                }

                //Assign temporary mark
                marks[vertex.Guid] = Mark.Temporary;
            }
            else
            {
                //Assign temporary mark
                marks.Add(vertex.Guid, Mark.Temporary);
            }

            // Visit next components
            foreach (Vertex nextVertex in componentGraph.getNextVertices(vertex))
            {
                if (!VisitDownstream(nextVertex, sorted, ref head, componentGraph, marks))
                {
                    // Graph is not acyclic
                    return false;
                }
            }

            // Assign permanent mark
            marks[vertex.Guid] = Mark.Permanent;

            // Prepend n to sorted
            sorted[head--] = vertex.Guid;

            return true;
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph)
        {
            // Create GUID list for sorted vertices
            Guid[] sorted = new Guid[componentGraph.VertexCount];
            int head = componentGraph.VertexCount-1;

            // Create Dictionary to allow marking of vertices
            Dictionary<Guid, Mark> marks = new Dictionary<Guid, Mark>();

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Guid));
            while (first != null) // Assumes all graph vertices are non-default. 
            {
                if(!VisitDownstream(first, sorted, ref head, componentGraph, marks))
                {
                    return componentGraph;
                }
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Guid));
            }

            // Assign the new IDs to the vertices
            Dictionary<Guid, Pair<int, int>> newIds = new Dictionary<Guid, Pair<int, int>>();

            foreach (Vertex vertex in componentGraph.Vertices)
            {
                newIds.Add(vertex.Guid, new Pair<int, int>(vertex.Id, 0));
            }

            int i = 0;
            foreach (Vertex vertex in componentGraph.Vertices)
            {
                newIds[vertex.Guid].After = newIds[sorted[i]].Before;
                vertex.Id = newIds[vertex.Guid].After;
            }

            return componentGraph;
        }
    }
}
