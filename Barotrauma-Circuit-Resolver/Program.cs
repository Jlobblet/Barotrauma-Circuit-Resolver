using Barotrauma_Circuit_Resolver.Util;
using QuickGraph;
using QuickGraph.Serialization;
using System;
using System.IO;
using System.Windows.Forms;
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
        private static void Main(string[] args)
        {
#if DEBUG
            string inputSub = args[0];
            string outputSub = args[1];
            string outputFile = args[2];

            XDocument submarine = SaveUtil.LoadSubmarine(inputSub);

            AdjacencyGraph<Vertex, Util.Edge<Vertex>> graph = submarine.CreateComponentGraph();

            graph.SolveUpdateOrder();

            static string VertexIdentity(Vertex v) => v.ToString();
            SaveUtil.SaveSubmarine(submarine, outputSub);

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
