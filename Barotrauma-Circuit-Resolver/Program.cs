using Barotrauma_Circuit_Resolver.Util;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Serialization;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

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

            graph.SolveUpdateOrder();

            static string VertexIdentity(Vertex v) => v.ToString();
            SaveUtil.SaveSubmarine(submarine, "output.sub");

            static string EdgeIdentifier(Util.Edge<Vertex> e) => e.ToString();

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate);
            using XmlWriter xw = XmlWriter.Create(fs);
            graph.SerializeToGraphML(xw, VertexIdentity, (EdgeIdentity<Vertex, Util.Edge<Vertex>>) EdgeIdentifier);
#else
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif
        }
    }
}
