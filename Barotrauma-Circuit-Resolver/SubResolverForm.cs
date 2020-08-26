using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Barotrauma_Circuit_Resolver.Util;

namespace Barotrauma_Circuit_Resolver
{
    public partial class SubResolverForm : Form
    {
        public SubResolverForm()
        {
            InitializeComponent();
            NewSubCheckBox.Checked = Settings.Default.NewSub;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            string result = FormUtils.ShowFileBrowserDialog();
            if (result == "") return;
            FilepathTextBox.Text = result;
            XDocument sub = SaveUtil.LoadSubmarine(result);
            pictureBox1.Image = FormUtils.GetImageFromString(sub.Root?.Attribute("previewimage")?.Value);
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            string inputFilepath = FilepathTextBox.Text;
            string outputFilepath = NewSubCheckBox.Checked ? Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                Path.GetFileNameWithoutExtension(inputFilepath) + "_resolved.sub") : inputFilepath;
            string graphFilepath = Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                Path.GetFileNameWithoutExtension(inputFilepath) + "_graphml");
            var (resolvedSubmarine, graph) = GraphUtil.ResolveCircuit(FilepathTextBox.Text);
            resolvedSubmarine.Save(outputFilepath);
            graph.SaveGraphML(graphFilepath);
        }

        private void NewSubCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.NewSub = NewSubCheckBox.Checked;
            Settings.Default.Save();
        }
    }
}
