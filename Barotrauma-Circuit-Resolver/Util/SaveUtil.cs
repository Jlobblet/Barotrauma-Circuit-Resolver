using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

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
    }
}
