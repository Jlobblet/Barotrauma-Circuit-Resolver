using Barotrauma_Circuit_Resolver.Util;
using QuickGraph;
using QuickGraph.Serialization;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Barotrauma_Circuit_Resolver
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
#if DEBUG
            const string inputSub = "C:/Users/MarketingMark/source/repos/Barotrauma-Circuit-Resolver/Barotrauma-Circuit-Resolver-Tests/Data/graph_tests_acyclic.sub";
            const string outputFile = "output.graphml";

            XDocument submarine = SaveUtil.LoadSubmarine(inputSub);

            AdjacencyGraph<Vertex, Util.Edge<Vertex>> graph = GraphUtil.CreateComponentGraph(submarine);

            graph = GraphUtil.SolveUpdateOrder(graph);

            VertexIdentity<Vertex> vertexIdentity = new VertexIdentity<Vertex>(v => v.ToString());
            EdgeIdentity<Vertex, Util.Edge<Vertex>> edgeIdentiter = new EdgeIdentity<Vertex, Util.Edge<Vertex>>(e => e.ToString());

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate);
            using XmlWriter xw = XmlWriter.Create(fs);
            graph.SerializeToGraphML(xw, vertexIdentity, edgeIdentiter);
#else
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif
        }
    }
}
