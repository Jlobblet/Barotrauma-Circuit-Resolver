using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        public static void AddDownstreamComponents(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, XDocument submarine, Vertex vertex, Dictionary<int, Vertex> idTable)
        {

            graph.AddVertex(vertex);
            IEnumerable<Vertex> next = submarine.GetNextComponentIDs(vertex);
            foreach (Vertex downstreamComponent in next)
            {
                if (!graph.TryGetEdge(vertex, downstreamComponent, out Edge<Vertex> _))
                {
                    // Prevent formation of duplicate nodes
                    if (idTable.ContainsKey(downstreamComponent.Id))
                    {
                        Edge<Vertex> edge = new Edge<Vertex>(graph.EdgeCount, string.Format("{0}-{1}", vertex.Id, downstreamComponent.Id), vertex, idTable[downstreamComponent.Id]);
                        graph.AddEdge(edge);
                        continue;
                    }
                    else
                    {
                        idTable.Add(downstreamComponent.Id, downstreamComponent);
                        Edge<Vertex> edge = new Edge<Vertex>(graph.EdgeCount, string.Format("{0}-{1}", vertex.Id, downstreamComponent.Id), vertex, downstreamComponent);
                        graph.AddEdge(edge);
                        graph.AddDownstreamComponents(submarine, downstreamComponent, idTable);

                    }
                }
            }
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(XDocument submarine)
        {
            Dictionary<int, Vertex> idTable = new Dictionary<int, Vertex>();

            IEnumerable<Vertex> entryPoints = submarine.GetEntryPoints();
            AdjacencyGraph<Vertex, Edge<Vertex>> graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();
            graph.EdgeAdded += Graph_EdgeAdded;
            foreach (Vertex entryPoint in entryPoints)
            {
                graph.AddDownstreamComponents(submarine, entryPoint, idTable);
            }
            return graph;
        }

        private static void Graph_EdgeAdded(Edge<Vertex> e)
        {
            var source = e.Source;
            var target = e.Target;
            source.OutgoingEdges.Add(e);
            target.IncomingEdges.Add(e);
        }

        public static bool VisitDownstream(Vertex vertex, Guid[] sortedGuids, ref int head, AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph, Dictionary<Guid, Mark> marks)
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
            foreach (Vertex nextVertex in vertex.OutgoingEdges.Select(e => e.Target))
            {
                if (!VisitDownstream(nextVertex, sortedGuids, ref head, componentGraph, marks))
                {
                    // Graph is not acyclic
                    return false;
                }
            }

            // Assign permanent mark
            marks[vertex.Guid] = Mark.Permanent;

            // Prepend n to sortedGuids
            sortedGuids[head--] = vertex.Guid;

            return true;
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph)
        {
            // Create GUID list for sorted vertices
            Guid[] sortedGuids = new Guid[componentGraph.VertexCount];
            int head = componentGraph.VertexCount-1;

            // Create Dictionary to allow marking of vertices
            Dictionary<Guid, Mark> marks = new Dictionary<Guid, Mark>();

            // Create Dictionary to find Vertices using GUIDs
            Dictionary<Guid, Vertex> Vertices = new Dictionary<Guid, Vertex>();
            foreach (Vertex vertex in componentGraph.Vertices)
            {
                Vertices.Add(vertex.Guid, vertex);
            }

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Guid));
            while (!(first is null)) 
            {
                if(!VisitDownstream(first, sortedGuids, ref head, componentGraph, marks))
                {
                    return componentGraph;
                }
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Guid));
            }

            // Create sorted list of IDs
            var idPool = componentGraph.Vertices.Select(v => v.Id);
            var sortedIds = idPool.OrderBy(i => i);

            // Apply list to graph
            int i = 0;
            foreach (var id in sortedIds)
            {
                Vertices[sortedGuids[i++]].Id = id;
            }

            return componentGraph;
        }
    }
}
