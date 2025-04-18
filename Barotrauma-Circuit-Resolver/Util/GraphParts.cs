﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Barotrauma_Circuit_Resolver.Util
{
    public class Vertex
    {
        public Vertex(int id) : this(id, id.ToString(), 0.0f)
        {
        }

        public Vertex(int id, string name, float pickingTime)
        {
            Id = id;
            Name = name;
            PickingTime = pickingTime;
            OutgoingEdges = new HashSet<Edge<Vertex>>();
            IncomingEdges = new HashSet<Edge<Vertex>>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public float PickingTime { get; set; }

        public int Order => Edges.Count();
        internal HashSet<Edge<Vertex>> OutgoingEdges { get; set; }

        internal HashSet<Edge<Vertex>> IncomingEdges { get; set; }

        internal HashSet<Edge<Vertex>> Edges =>
            OutgoingEdges.Union(IncomingEdges).ToHashSet();

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   Id == vertex.Id;
        }

        public static bool operator ==(Vertex lhs, Vertex rhs)
        {
            return lhs is { } && lhs.Equals(rhs);
        }

        public static bool operator !=(Vertex lhs, Vertex rhs)
        {
            return lhs is { } && !lhs.Equals(rhs);
        }

        public string GetStringHashCode()
        {
            return $"{{{GetHashCode()}}}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return $"{Name}_{Id}";
        }
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

        public override string ToString()
        {
            return $"{Source}-{Target}";
        }

        public override bool Equals(object obj)
        {
            return obj is Edge<TVertex> edge &&
                   EqualityComparer<TVertex>.Default.Equals(Source, edge.Source) &&
                   EqualityComparer<TVertex>.Default.Equals(Source, edge.Source) &&
                   EqualityComparer<TVertex>.Default.Equals(Target, edge.Target);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Target);
        }

        public static bool operator ==(Edge<TVertex> lhs, Edge<TVertex> rhs)
        {
            return lhs is { } && lhs.Equals(rhs);
        }

        public static bool operator !=(Edge<TVertex> lhs, Edge<TVertex> rhs)
        {
            return lhs is { } && !lhs.Equals(rhs);
        }
    }
}
