using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using QuickGraph;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        public delegate void ProgressUpdate(float value, string label);
        public static event ProgressUpdate OnProgressUpdate;

        public enum Mark
        {
            Unmarked,
            Temporary,
            Permanent,
        }

        private static readonly string[] PowerConnections =
            {"power", "power_in", "power_out"};

        private static bool FilterPower(XElement elt) =>
            !PowerConnections.Contains(elt.Attribute("name")?.Value);

        public static IEnumerable<Vertex> GetComponents(this XDocument submarine)
        {
            return submarine.Root?.Elements()
                            .Where(e => e.Descendants("input")
                                         .Union(e.Descendants("output"))
                                         .Where(FilterPower)
                                         .Elements("link")
                                         .Any())
                            .Select(e =>
                                        new Vertex(int.Parse(e.Attribute("ID")?.Value!),
                                                   e.Attribute("identifier")?.Value));
        }

        public static IEnumerable<Edge<Vertex>> GetEdges(
            this XDocument submarine, AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            return graph.Vertices.Select(s => submarine.GetNextIDs(s)
                                                       .Select(i =>
                                                           new Edge<Vertex>(s,
                                                               graph
                                                                   .Vertices
                                                                   .First(v =>
                                                                       v.Id == i))))
                        .SelectMany(e => e)
                        .Distinct();
        }

        public static IEnumerable<int> GetNextIDs(this XDocument submarine,
                                                  Vertex vertex)
        {
            return submarine.GetNextIDs(submarine.Root?.Elements()
                                                 .First(e => e.Attribute("ID")?.Value ==
                                                            vertex.Id.ToString()));
        }

        public static IEnumerable<int> GetNextIDs(this XDocument submarine,
                                                  XElement element)
        {
            return submarine.Root?.Elements()
                            .Where(e => element
                                        .Descendants("output").Where(FilterPower)
                                        .Elements("link")
                                        .Select(e2 => e2.Attribute("w")?.Value)
                                        .Intersect(e.Descendants("input")
                                                    .Elements("link")
                                                    .Select(i => i.Attribute("w")
                                                        ?.Value))
                                        .Any())
                            .Select(e => int.Parse(e.Attribute("ID")?.Value!));
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> CreateComponentGraph(
            this XDocument submarine)
        {
            AdjacencyGraph<Vertex, Edge<Vertex>> graph =
                new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
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

        /// <summary>
        /// Remove all links that go from logic components into data/state storage components.
        /// (data/storage will be assigned the lowest IDs to be updated first, so these links form no sorting constraint)
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static void PreprocessGraph(
            this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            graph.RemoveEdgeIf(e => e.Source.Name != "memorycomponent" &&
                                    e.Source.Name != "relaycomponent" &&
                                    (e.Target.Name == "memorycomponent" ||
                                     e.Target.Name == "relaycomponent"));
        }

        public static bool VisitDownstream(Vertex vertex,
                                           Vertex[] sortedVertices,
                                           ref int head,
                                           ref int tail,
                                           AdjacencyGraph<Vertex, Edge<Vertex>>
                                               componentGraph,
                                           Dictionary<int, Mark> marks)
        {
            // Assign the new IDs to the vertices
            if (marks.ContainsKey(vertex.Id))
            {
                switch (marks[vertex.Id])
                {
                    case Mark.Permanent:
                        // End of branch
                        return true;
                    case Mark.Temporary:
                        // Graph is not Acyclic
                        return false;
                    case Mark.Unmarked:
                        break;
                    default:
                        //Assign temporary mark
                        marks[vertex.Id] = Mark.Temporary;
                        break;
                }
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

            OnProgressUpdate?.Invoke((componentGraph.VertexCount - head + tail)/componentGraph.VertexCount, "Solving Update Order...");

            return true;

        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(
            this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph,
            out Vertex[] sortedVertices)
        {
            // Remove loops containing memory from graph
            componentGraph.PreprocessGraph();

            // Create Vertex list for sorted vertices
            sortedVertices = new Vertex[componentGraph.VertexCount];
            int head = componentGraph.VertexCount - 1;
            int tail = 0;

            // Create Dictionary to allow marking of vertices
            Dictionary<int, Mark> marks = new Dictionary<int, Mark>();

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            while (!(first is null)) 
            {
                VisitDownstream(first, sortedVertices, ref head, ref tail, componentGraph, marks);
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            }

            // Apply sorted list of IDs to graph
            foreach ((int id, int i) in componentGraph.Vertices
                                                      .Select((v, i) => (v.Id, i))
                                                      .OrderBy(t => t.Id))
            {
                sortedVertices[i].Id = id;
            }

            return componentGraph;
        }
        public static (XDocument, AdjacencyGraph<Vertex, Edge<Vertex>>) ResolveCircuit(string inputSub)
        {
            OnProgressUpdate?.Invoke(0, "Loading Submarine...");
            XDocument submarine = SaveUtil.LoadSubmarine(inputSub);

            OnProgressUpdate?.Invoke(0, "Extracting Component Graph...");
            AdjacencyGraph<Vertex, Util.Edge<Vertex>> graph =
                submarine.CreateComponentGraph();

            graph.SolveUpdateOrder(out Vertex[] sortedVertices);
            submarine.UpdateSubmarineIDs(graph, sortedVertices);

            return (submarine, graph);
        }
    }
}
