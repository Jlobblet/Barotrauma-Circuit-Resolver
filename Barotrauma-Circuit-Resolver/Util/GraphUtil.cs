using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using QuickGraph;
using BaroLib;
using System.Reflection.Metadata.Ecma335;
using QuickGraph.Algorithms.TopologicalSort;

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
        /// Preprocesses the graph to change constraints based on sort setting
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="invertMemory"></param>
        /// <param name="retainParallel"></param>
        public static void PreprocessGraph(
            this AdjacencyGraph<Vertex, Edge<Vertex>> graph, bool invertMemory, bool retainParallel)
        {
            if (invertMemory)
            {
                InvertMemory(graph);
            }
            if (retainParallel)
            {
                ConstrainParallel(graph);
            }
        }

        /// <summary>
        /// Remove all links that go from logic components into data/state storage components.
        /// (data/storage will be assigned the lowest IDs to be updated first, so these links form no sorting constraint)
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static void InvertMemory(
            this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            graph.RemoveEdgeIf(e => e.Source.Name != "memorycomponent" &&
                                    e.Target.Name == "memorycomponent");
        }

        /// <summary>
        /// Add connections between multiple connections after the same component to constrain the update-order sorting such
        /// that the order between them remains the same after sorting. 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static void ConstrainParallel(
            this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            List<Edge<Vertex>> constraints = new List<Edge<Vertex>>();

            // TO-DO: Add connections
            foreach (Vertex source in graph.Vertices)
            {
                if(source.OutgoingEdges.Count() > 1)
                {
                    // Create list of outgoing edges sorted by ID
                    List<Vertex> sortedTargets = source.OutgoingEdges.Select(e => e.Target).OrderBy(v => v.Id).ToList();

                    // Remove any interconnected components as placing constraints on these components may cause conflicts
                    sortedTargets = RemoveInterconnected(graph, sortedTargets);

                    // Create connections between the outgoing edges
                    for(int i=0; i<sortedTargets.Count()-1; i++)
                    {
                        constraints.Add(new Edge<Vertex>(sortedTargets[i], sortedTargets[i + 1]));
                    }
                }
            }

            // Add the constraints to the graph
            graph.AddEdgeRange(constraints);
        }

        /// <summary>
        /// Remove from a list of vertices any vertices that are directly connected
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static List<Vertex> RemoveInterconnected(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, List<Vertex> vertices)
        {
            List<Vertex> unconnectedVertices = vertices;

            for (int i = 0; i < vertices.Count() - 1; i++)
            {
                if (!unconnectedVertices.Contains(vertices[i])) { continue; } // Vertex already determined interconnected

                for (int j = 0; j < vertices.Count() - 1; j++) // j starts as i+1 as any previous nodes need not be reconsidered
                {
                    if (j == i) { continue; }

                    if (graph.ContainsEdge(vertices[i], vertices[j]) ||
                        graph.ContainsEdge(vertices[j], vertices[i]))
                    {
                        unconnectedVertices.Remove(vertices[i]);
                        if (unconnectedVertices.Contains(vertices[i])) 
                        { 
                            unconnectedVertices.Remove(vertices[i]); // Might have been removed previously
                        }
                    }
                }
            }

            return unconnectedVertices;
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
            if(vertex.Name == "memorycomponent")
            {
                // Update memory components in reverse order
                sortedVertices[tail++] = vertex;
            }
            else
            {
                sortedVertices[head--] = vertex;
            }

            OnProgressUpdate?.Invoke(0.6f+.2f*(componentGraph.VertexCount - head + tail)/componentGraph.VertexCount, "Solving Update Order...");

            return true;

        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> SolveUpdateOrder(
            this AdjacencyGraph<Vertex, Edge<Vertex>> componentGraph,
            out Vertex[] sortedVertices, bool invertMemory, bool retainParallel)
        {
            OnProgressUpdate?.Invoke(0.4f, "Preprocessing graph...");
            // Remove loops containing memory from graph
            componentGraph.PreprocessGraph(invertMemory, retainParallel);

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
        public static (XDocument, AdjacencyGraph<Vertex, Edge<Vertex>>) ResolveCircuit(string inputSub, bool invertMemory, bool retainParallel)
        {
            OnProgressUpdate?.Invoke(0, "Loading Submarine...");
            XDocument submarine = IoUtil.LoadSub(inputSub);

            OnProgressUpdate?.Invoke(0.2f, "Extracting Component Graph...");
            AdjacencyGraph<Vertex, Util.Edge<Vertex>> graph =
                submarine.CreateComponentGraph();

            graph.SolveUpdateOrder(out Vertex[] sortedVertices, invertMemory, retainParallel);
            submarine.UpdateSubmarineIDs(graph, sortedVertices);

            return (submarine, graph);
        }
    }
}
