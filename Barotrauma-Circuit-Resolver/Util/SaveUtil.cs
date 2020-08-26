using Barotrauma_Circuit_Resolver.Util;
using QuickGraph;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Barotrauma_Circuit_Resolver
{
    public static class SaveUtil
    {

        public static XDocument LoadSubmarine(string filepath)
        {
            using FileStream fileStream = new FileStream(filepath, FileMode.Open);
            using GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            return XDocument.Load(gZipStream);
        }

        public static void SaveSubmarine(XDocument submarine, string filepath)
        {
            string temp = Path.GetTempFileName();
            File.WriteAllText(temp, submarine.ToString());
            byte[] b;
            using (FileStream fs = new FileStream(temp, FileMode.Open))
            {
                b = new byte[fs.Length];
                fs.Read(b, 0, (int)fs.Length);
            }
            using FileStream fileStream = new FileStream(filepath, FileMode.OpenOrCreate);
            using GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Compress, false);
            gZipStream.Write(b, 0, b.Length);
        }

        public static void UpdateSubmarineIDs(XDocument submarine, AdjacencyGraph<Vertex, Util.Edge<Vertex>> graph, IEnumerable<Vertex> SortedVertices)
        {
            const string xpath = "//Item/@ID|//link/@w";

            IEnumerable<(int First, int Second)> ids = graph.Vertices.Select(v => v.Id).Zip(SortedVertices.Select(v => v.Id));
            IEnumerable<Triplet<int, string, int>> idChangeTriplet = ids.Select(e => new Triplet<int, string, int>(e.First, SortedVertices.First(v => v.Id == e.First).GetStringHashCode(), e.Second));

            foreach (XObject xObject in (IEnumerable)submarine.XPathEvaluate(xpath))
            {
                if (!(xObject is XAttribute attribute))
                {
                    continue;
                }

                int id = int.Parse(attribute.Value);
                if (idChangeTriplet.Any(t => t.First == id))
                {
                    attribute.Value = idChangeTriplet.First(t => t.First == id).Second;
                }
            }

            foreach (XObject xObject in (IEnumerable)submarine.XPathEvaluate(xpath))
            {
                if (xObject is XAttribute attribute)
                {
                    string Hash = attribute.Value;
                    if (idChangeTriplet.Any(t => t.Second == Hash))
                    {
                        attribute.Value = idChangeTriplet.First(t => t.Second == Hash).Third.ToString();
                    }
                }
            }

        }
    }
}
