using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Barotrauma_Circuit_Resolver.Util
{
    public class Vertex
    {
        private int id;
        private string name;
        private HashSet<Edge<Vertex>> outgoingEdges;
        private HashSet<Edge<Vertex>> incomingEdges;
        private Guid guid;

        public Vertex(int id)
        {
            new Vertex(id, id.ToString());
        }

        public Vertex(int id, string name)
        {
            Id = id;
            Name = name;
            OutgoingEdges = new HashSet<Edge<Vertex>>();
            IncomingEdges = new HashSet<Edge<Vertex>>();
            guid = Guid.NewGuid();
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int Order => Edges.Count();
        internal HashSet<Edge<Vertex>> OutgoingEdges { get => outgoingEdges; set => outgoingEdges = value; }
        internal HashSet<Edge<Vertex>> IncomingEdges { get => incomingEdges; set => incomingEdges = value; }
        internal HashSet<Edge<Vertex>> Edges => OutgoingEdges.Union(IncomingEdges).ToHashSet<Edge<Vertex>>();
        public Guid Guid => guid;

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   Id == vertex.Id;
        }

        public static bool operator ==(Vertex lhs, Vertex rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Vertex lhs, Vertex rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name, Id);
        }

    }

    public class Edge<TVertex> : IEdge<TVertex>
    {
        private int id;
        private string name;
        private readonly TVertex source;
        private readonly TVertex target;

        public Edge(int id, string name, TVertex source, TVertex target)
        {
            Id = id;
            Name = name;
            this.source = source;
            this.target = target;
        }

        public TVertex Source => source;

        public TVertex Target => target;

        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name, Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Edge<TVertex> edge &&
                   Id == edge.Id;
        }

        public static bool operator ==(Edge<TVertex> lhs, Edge<TVertex> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Edge<TVertex> lhs, Edge<TVertex> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
