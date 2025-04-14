using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BaroLib;
using QuickGraph;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class GraphUtil
    {
        public delegate void ProgressUpdate(float value, string label);

        public enum Mark
        {
            Unmarked,
            Temporary,
            Permanent
        }

        private static readonly string[] PowerConnections =
            {"power", "power_in", "power_out"};

        public static event ProgressUpdate OnProgressUpdate;

        private static bool FilterPower(XElement elt)
        {
            return !PowerConnections.Contains(elt.Attribute("name")?.Value);
        }

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
                                                   e.Attribute("identifier")?.Value,
                                                   float.Parse(e.Descendants()
                                                    .FirstOrDefault()
                                                    .Attribute("pickingtime")?.Value ?? "0.0"))); // As far as I see all comps have their "Component" item first
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
            var graph =
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
                InvertMemory(graph);

            if (retainParallel)
                ConstrainParallel(graph);
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
        /// Add connections between the components after each component to constrain the update-order sorting such
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

                    // To-do: Pretty sure this can or will create arithmetic loops in some circuits... could use a disclaimer
                    // but I'm about to implement the better picking-time based sort control and I'm not really seeing anyone else
                    // caring either.
                    for (int i=0; i<sortedTargets.Count()-1; i++)
                        constraints.Add(new Edge<Vertex>(sortedTargets[i], sortedTargets[i + 1]));
                }
            }

            // Add the constraints to the graph
            graph.AddEdgeRange(constraints);
        }

        /// <summary>
        /// Add constraints to further sort components by the value of their "pickingtime" attribute, which can be set manually in-game
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static void ConstrainByPickingTime(
            this AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            List<Edge<Vertex>> constraints = new List<Edge<Vertex>>();

            // Get all components with nonzero picking time, in ascending order
            List < Vertex > pickingTimeComps = graph.Vertices.Where(v => v.PickingTime > 0).OrderBy(v => v.PickingTime).ToList();

            // Add a helper vertex for each unique time
            List <float> UniquePickingTimes = pickingTimeComps.Select(v => v.PickingTime).Distinct().ToList();
            int maxId = graph.Vertices.Select(v => v.Id).Max();
            List<int> helperIds = Enumerable.Range(maxId + 1, UniquePickingTimes.Count - 1).ToList();
            List<Vertex> helpervertices = helperIds.Select(i => new Vertex(i)).ToList();

            // Add connections between comps to order and helper vertices
            for (int i=0; i < UniquePickingTimes.Count - 1; i++)
            {
                foreach(Vertex v in pickingTimeComps.Where(v => v.PickingTime == UniquePickingTimes[i]))
                {
                    constraints.Add(new Edge<Vertex>(v, helpervertices[i]));
                }
            }

            graph.AddVertexRange(helpervertices);

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
                for (int j = 0; j < vertices.Count() - 1; j++)
                {
                    if (j == i) continue; 

                    if (graph.ContainsEdge(vertices[i], vertices[j]) || 
                        graph.ContainsEdge(vertices[j], vertices[i]))
                    {
                        if (unconnectedVertices.Contains(vertices[i]))
                            unconnectedVertices.Remove(vertices[i]);

                        break;
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
                                           Dictionary<int, Mark> marks,
                                           bool invertMemory)
        {
            // Assign the new IDs to the vertices
            if (marks.ContainsKey(vertex.Id))
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
            else
                //Assign temporary mark
                marks.Add(vertex.Id, Mark.Temporary);

            // Visit next components
            foreach (Vertex nextVertex in vertex.OutgoingEdges.Select(e => e.Target))
                VisitDownstream(nextVertex, sortedVertices, ref head, ref tail, componentGraph, marks, invertMemory);

            // Assign permanent mark
            marks[vertex.Id] = Mark.Permanent;

            // Prepend n to sortedGuids
            if(invertMemory && vertex.Name == "memorycomponent")
                // Update memory components in reverse order
                sortedVertices[tail++] = vertex;
            else
                sortedVertices[head--] = vertex;

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
            var tail = 0;

            // Create Dictionary to allow marking of vertices
            var marks = new Dictionary<int, Mark>();

            // Visit first unmarked Vertex
            Vertex first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            while (!(first is null))
            {
                VisitDownstream(first, sortedVertices, ref head, ref tail, componentGraph, marks, invertMemory);
                first = componentGraph.Vertices.FirstOrDefault(vertex => !marks.ContainsKey(vertex.Id));
            }

            // Apply sorted list of IDs to graph
            List<int> sortedIDs = componentGraph.Vertices.OrderBy(v => v.Id).Select(v => v.Id).ToList();
            int i = 0;
            foreach (int id in sortedIDs)
                sortedVertices[i++].Id = id; // Note: Can't sortedIDs in the foreach directly as componentGraph.Vertices and sortedVertices are linked

            return componentGraph;
        }

        public static (XDocument, AdjacencyGraph<Vertex, Edge<Vertex>>) ResolveCircuit(
            XDocument inputDoc, bool invertMemory, bool retainParallel)
        {
            OnProgressUpdate?.Invoke(0, "Extracting Component Graph...");
            AdjacencyGraph<Vertex, Edge<Vertex>> graph =
                inputDoc.CreateComponentGraph();

            graph.SolveUpdateOrder(out Vertex[] sortedVertices, invertMemory, retainParallel);
            inputDoc.UpdateSubmarineIDs(graph, sortedVertices);

            return (inputDoc, graph);
            
        }
        public static (XDocument, AdjacencyGraph<Vertex, Edge<Vertex>>) ResolveCircuit(
            string inputSub, bool invertMemory, bool retainParallel)
        {
            OnProgressUpdate?.Invoke(0, "Loading Submarine...");
            XDocument submarine = IoUtil.LoadSub(inputSub);

            OnProgressUpdate?.Invoke(0.2f, "Extracting Component Graph...");
            AdjacencyGraph<Vertex, Edge<Vertex>> graph =
                submarine.CreateComponentGraph();

            graph.SolveUpdateOrder(out Vertex[] sortedVertices, invertMemory, retainParallel);
            submarine.UpdateSubmarineIDs(graph, sortedVertices);

            return (submarine, graph);
        }
    }
}
