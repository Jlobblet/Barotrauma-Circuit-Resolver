using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using QuickGraph;
using QuickGraph.Serialization;

namespace Barotrauma_Circuit_Resolver.Util
{
    public static class SaveUtil
    {

        public delegate void ProgressUpdate(float value, string label);
        public static event ProgressUpdate OnProgressUpdate;

        public static XDocument LoadSubmarine(string filepath)
        {
            using FileStream fileSteam = new FileStream(filepath, FileMode.Open);
            using GZipStream gZipStream = new GZipStream(fileSteam, CompressionMode.Decompress);
            return XDocument.Load(gZipStream);
        }

        public static void SaveSubmarine(this XDocument sub, string fileName)
        {
            string temp = Path.GetTempFileName();
            File.WriteAllText(temp, sub.ToString());
            byte[] b;
            using (FileStream fs = new FileStream(temp, FileMode.Open))
            {
                b = new byte[fs.Length];
                fs.Read(b, 0, (int)fs.Length);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            using (GZipStream gz = new GZipStream(fs, CompressionMode.Compress, false))
            {
                gz.Write(b, 0, b.Length);
            }
        }
        
        public static void SaveGraphML(this AdjacencyGraph<Vertex, Edge<Vertex>> graph, string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            static string VertexIdentity(Vertex v) => v.ToString();
            static string EdgeIdentifier(Util.Edge<Vertex> e) => e.ToString();

            using FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
            using XmlWriter xw = XmlWriter.Create(fs);
            graph.SerializeToGraphML(xw, VertexIdentity,
                (EdgeIdentity<Vertex, Edge<Vertex>>)
                EdgeIdentifier);

        }

        public static void UpdateSubmarineIDs(this XDocument submarine,
                                              AdjacencyGraph<Vertex, Edge<Vertex>>
                                                  graph,
                                              IEnumerable<Vertex> sortedVertices)
        {
            const string xpath = "//Item/@ID|//link/@w";

            IEnumerable<(int First, int Second)> ids = sortedVertices.Select(v => v.Id)
                .Zip(graph.Vertices.Select(v => v.Id));
            IEnumerable<Triplet<int, string, int>> idChangeTriplet =
                ids.Select(e => new Triplet<int, string, int>(e.First,
                               sortedVertices.First(v => v.Id == e.First)
                                             .GetStringHashCode(),
                               e.Second));

            IEnumerable<object> evaluatedPath = (IEnumerable<object>)submarine.XPathEvaluate(xpath);
            (int, int) progress = (0, evaluatedPath.Count());
            foreach (XObject xObject in evaluatedPath)
            {
                OnProgressUpdate?.Invoke(.5f*progress.Item1++/progress.Item2, "Updating IDs in submarine XML...");
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

            progress.Item1 = 0;
            foreach (XObject xObject in evaluatedPath)
            {
                OnProgressUpdate?.Invoke(.5f+.5f*progress.Item1++ / progress.Item2, "Updating IDs in submarine XML...");
                if (!(xObject is XAttribute attribute))
                {
                    continue;
                }

                string hash = attribute.Value;
                if (idChangeTriplet.Any(t => t.Second == hash))
                {
                    attribute.Value = idChangeTriplet.First(t => t.Second == hash).Third
                                                     .ToString();
                }
            }
        }
    }
}
