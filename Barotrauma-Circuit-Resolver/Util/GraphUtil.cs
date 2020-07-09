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

        public static IEnumerable<Vertex> GetComponents(this XDocument submarine)
        {
            return submarine.Root.Elements()
                                 .Where(e => e.Descendants("input")
                                    .Union(e.Descendants("output"))
                                    .Where(FilterPower)
                                    .Elements("link").Any())
                                 .Select(e => new Vertex(int.Parse(e.Attribute("ID").Value), e.Attribute("identifier").Value));
        }

        public static IEnumerable<Edge<Vertex>> GetEdges(this XDocument submarine, AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            return graph.Vertices.Select(s => submarine.GetNextVertices(s).Select(t => new Edge<Vertex>(s, graph.Vertices.First(v => v.Id == t.Id))))
                                 .SelectMany(e => e).Distinct();
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

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(XDocument submarine)
        {
            AdjacencyGraph<Vertex, Edge<Vertex>> graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            graph.EdgeAdded += Graph_EdgeAdded;
            graph.AddVertexRange(submarine.GetComponents());
            graph.AddEdgeRange(submarine.GetEdges(graph));
            return graph;
        }

        private static void Graph_EdgeAdded(Edge<Vertex> e)
        {
            e.Source.OutgoingEdges.Add(e);
            e.Target.IncomingEdges.Add(e);
        }

        public static bool VisitDownstream(Vertex vertex, Vertex[] sortedVertices, ref int head, AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, Dictionary<int, Mark> marks)
        {
            System.Diagnostics.Debug.WriteLine("Evaluating \t" + vertex);

            // Assign the new IDs to the vertices
            if (marks.ContainsKey(vertex.Id))
            {
                if (marks[vertex.Id] == Mark.Permanent)
                {
                    // End of branch
                    return true;
                }
                if (marks[vertex.Id] == Mark.Temporary)
                {
                    // Graph is not Acyclic
                    return false;
                }

                //Assign temporary mark
                marks[vertex.Id] = Mark.Temporary;
            }
            else
            {
                //Assign temporary mark
                marks.Add(vertex.Id, Mark.Temporary);
            }

            // Visit next components
            foreach (Vertex nextVertex in vertex.OutgoingEdges.Select(e => e.Target))
            {
                if (!VisitDownstream(nextVertex, sortedVertices, ref head, componentGraph, marks))
                {
                    // Graph is not acyclic
                    return false;
                }
            }

            // Assign permanent mark
            marks[vertex.Id] = Mark.Permanent;

            // Prepend n to sortedGuids
            sortedVertices[head--] = vertex;

            System.Diagnostics.Debug.WriteLine("Storing \t" + vertex);

            return true;
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph)
        {
            // Create GUID list for sorted vertices
            Vertex[] sortedVertices = new Vertex[componentGraph.VertexCount];
            int head = componentGraph.VertexCount-1;

            // Create Dictionary to allow marking of vertices
            Dictionary<int, Mark> marks = new Dictionary<int, Mark>();

            // Create Dictionary to find Vertices using GUIDs
            Dictionary<int, Vertex> Vertices = new Dictionary<int, Vertex>();
            foreach (Vertex vertex in componentGraph.Vertices)
            {
                Vertices.Add(vertex.Id, vertex);
            }

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            while (!(first is null)) 
            {
                if(!VisitDownstream(first, sortedVertices, ref head, componentGraph, marks))
                {
                    return componentGraph;
                }
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            }

            // Create sorted list of IDs
            var idPool = componentGraph.Vertices.Select(v => v.Id);
            var sortedIds = idPool.OrderBy(i => i);

            // Apply list to graph
            int i = 0;
            foreach (var id in sortedIds)
            {
                Vertices[sortedVertices[i++].Id].Id = id;
            }

            return componentGraph;
        }
    }
}
