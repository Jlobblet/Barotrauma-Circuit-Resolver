using System;
using System.Windows.Forms;
#if DEBUG
using System.Xml.Linq;
using Barotrauma_Circuit_Resolver.Util;
using BaroLib;

#endif


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
            if (args.Length > 1)
            {
                (XDocument resolvedSubmarine, QuickGraph.AdjacencyGraph<Vertex, Edge<Vertex>> graph) =
                    GraphUtil.ResolveCircuit(args[0]);
                resolvedSubmarine.SaveSub(args[1]);
                if (args.Length > 2) graph.SaveGraphML(args[2]);
            }
#endif
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SubResolverForm());
        }
    }
}
