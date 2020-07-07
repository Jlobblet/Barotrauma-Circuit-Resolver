using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace Barotrauma_Circuit_Resolver.Util
{
    public class Vertex
    {
        private int id;
        private string name;
        private IEnumerable<Edge> edges;
        public Vertex(int id, string name)
        {
            Id = id;
            Name = name;
            Edges = Enumerable.Empty<Edge>();
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int Order { get => Edges.Count(); }
        internal IEnumerable<Edge> Edges { get => edges; set => edges = value; }

        public override bool Equals(object obj)
        {
            return obj is Vertex vertex &&
                   Id == vertex.Id;
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

    public class Edge : IEdge<Vertex>
    {
        private int id;
        private string name;
        private readonly Vertex source;
        private readonly Vertex target;

        public Edge(int id, string name, Vertex source, Vertex target)
        {
            Id = id;
            Name = name;
            this.source = source;
            this.target = target;
        }

        public Vertex Source => source;

        public Vertex Target => target;

        public string Name { get => name; set => name = value; }
        public int Id { get => id; set => id = value; }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Name, Id);
        }

        public override bool Equals(object obj)
        {
            return obj is Edge edge &&
                   Id == edge.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
