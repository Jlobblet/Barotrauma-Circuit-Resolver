using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Barotrauma_Circuit_Resolver.Util;
using QuickGraph.Serialization;

namespace Barotrauma_Circuit_Resolver
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG
            string inputSub = args[0];
            string outputSub = args[1];
            string outputFile = args[2];

            XDocument submarine = SaveUtil.LoadSubmarine(inputSub);

            QuickGraph.AdjacencyGraph<Vertex, Edge<Vertex>> graph =
                submarine.CreateComponentGraph();

            graph.SolveUpdateOrder(out Vertex[] sortedVertices);

            static string VertexIdentity(Vertex v) => v.ToString();

            SaveUtil.SaveSubmarine(submarine, outputSub);

            static string EdgeIdentifier(Edge<Vertex> e) => e.ToString();

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate);
            using XmlWriter xw = XmlWriter.Create(fs);
            graph.SerializeToGraphML(xw, VertexIdentity,
                                     (QuickGraph.EdgeIdentity<Vertex, Edge<Vertex>>)
                                     EdgeIdentifier);
#else
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif
        }
    }
}
