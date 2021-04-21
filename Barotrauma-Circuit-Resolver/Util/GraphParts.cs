using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Barotrauma_Circuit_Resolver.Util
{
    public class Vertex
    {
        public Vertex(int id) : this(id, id.ToString())
        {
        }

        public Vertex(int id, string name)
        {
            Id = id;
            Name = name;
            OutgoingEdges = new HashSet<Edge<Vertex>>();
            IncomingEdges = new HashSet<Edge<Vertex>>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Order => Edges.Count();
        internal HashSet<Edge<Vertex>> OutgoingEdges { get; set; }

        internal HashSet<Edge<Vertex>> IncomingEdges { get; set; }

        internal HashSet<Edge<Vertex>> Edges =>
            OutgoingEdges.Union(IncomingEdges).ToHashSet();

        public override bool Equals(object obj) =>
            obj is Vertex vertex &&
            Id == vertex.Id;

        public static bool operator ==(Vertex lhs, Vertex rhs) =>
            lhs is { } && lhs.Equals(rhs);

        public static bool operator !=(Vertex lhs, Vertex rhs) =>
            lhs is { } && !lhs.Equals(rhs);

        public string GetStringHashCode() => $"{{{GetHashCode()}}}";

        public override int GetHashCode() => HashCode.Combine(Id);

        public override string ToString() => $"{Name}_{Id}";
    }

    public class Edge<TVertex> : IEdge<TVertex>
    {
        public Edge(TVertex source, TVertex target)
        {
            Source = source;
            Target = target;
        }

        public string Name => ToString();

        public TVertex Source { get; }

        public TVertex Target { get; }

        public override string ToString() => $"{Source}-{Target}";

        public override bool Equals(object obj) =>
            obj is Edge<TVertex> edge &&
            EqualityComparer<TVertex>.Default.Equals(Source, edge.Source) &&
            EqualityComparer<TVertex>.Default.Equals(Source, edge.Source) &&
            EqualityComparer<TVertex>.Default.Equals(Target, edge.Target);

        public override int GetHashCode() => HashCode.Combine(Source, Target);

        public static bool operator ==(Edge<TVertex> lhs, Edge<TVertex> rhs) =>
            lhs is { } && lhs.Equals(rhs);

        public static bool operator !=(Edge<TVertex> lhs, Edge<TVertex> rhs) =>
            lhs is { } && !lhs.Equals(rhs);
    }
}
