using QuickGraph;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            return graph.Vertices.Select(s => submarine.GetNextIDs(s).Select(i => new Edge<Vertex>(s, graph.Vertices.Where(v => v.Id == i).First())))
                                 .SelectMany(e => e).Distinct();
        }

        public static IEnumerable<int> GetNextIDs(this XDocument submarine, Vertex vertex)
        {
            return submarine.GetNextIDs(submarine.Root.Elements()
                .Where(e => e.Attribute("ID").Value == vertex.Id.ToString())
                .First());
        }

        public static IEnumerable<int> GetNextIDs(this XDocument submarine, XElement element)
        {
            return submarine.Root.Elements()
                .Where(e => element.Descendants("output").Where(FilterPower).Elements("link")
                    .Select(e => e.Attribute("w").Value)
                    .Intersect(e.Descendants("input").Elements("link")
                        .Select(i => i.Attribute("w").Value))
                    .Any())
                .Select(e => int.Parse(e.Attribute("ID").Value));
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

        public static AdjacencyGraph<Vertex, Edge<Vertex>> PreprocessGraph(this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            // Remove all links that go from logic components into data/state storage components.
            // (data/storage will be assigned the lowest IDs to be upated first, so these links form no sorting constraint)
            graph.RemoveEdgeIf(e => (e.Source.Name != "memorycomponent" && e.Source.Name != "relaycomponent") && (e.Target.Name == "memorycomponent" || e.Target.Name == "relaycomponent"));
            return graph;
        }

        public static bool VisitDownstream(Vertex vertex, Vertex[] sortedVertices, ref int head, ref int tail, AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, Dictionary<int, Mark> marks)
        {
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
                VisitDownstream(nextVertex, sortedVertices, ref head, ref tail, componentGraph, marks);
            }

            // Assign permanent mark
            marks[vertex.Id] = Mark.Permanent;

            // Prepend n to sortedGuids
            if(vertex.Name == "memorycomponent" || vertex.Name == "relaycomponent")
            {
                // Update memory components in reverse order
                sortedVertices[tail++] = vertex;
            }
            else
            {
                sortedVertices[head--] = vertex;
            }

            return true;
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, out Vertex[] sortedVertices)
        {
            // Create GUID list for sorted vertices
            sortedVertices = new Vertex[componentGraph.VertexCount];
            int head = componentGraph.VertexCount-1;
            int tail = 0;

            // Create Dictionary to allow marking of vertices
            Dictionary<int, Mark> marks = new Dictionary<int, Mark>();

            // Remove loops containing memory from graph
            componentGraph.PreprocessGraph();

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            while (!(first is null)) 
            {
                VisitDownstream(first, sortedVertices, ref head, ref tail, componentGraph, marks);
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            }

            // Create sorted list of IDs
            IEnumerable<int> idPool = componentGraph.Vertices.Select(v => v.Id);
            IOrderedEnumerable<int> sortedIds = idPool.OrderBy(i => i);

            // Apply list to graph
            int i = 0;
            foreach (int id in sortedIds)
            {
                sortedVertices[i++].Id = id;
            }

            return componentGraph;
        }
    }
}
