using Barotrauma_Circuit_Resolver.Util;
using QuickGraph;
using QuickGraph.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Barotrauma_Circuit_Resolver
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            const string inputSub = "Azimuth.sub";
            const string outputFile = "output.graphml";

            XDocument submarine = SaveUtil.LoadSubmarine(inputSub);

            var graph = GraphUtil.CreateComponentGraph(submarine);
            var vertexIdentity = new VertexIdentity<Vertex>(v => v.ToString());
            var edgeIdentiter = new EdgeIdentity<Vertex, Util.Edge<Vertex>>(e => e.ToString());
            
            if (File.Exists(outputFile))
                File.Delete(outputFile);
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
