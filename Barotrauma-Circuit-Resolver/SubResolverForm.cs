using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using BaroLib;
using Barotrauma_Circuit_Resolver.Util;

namespace Barotrauma_Circuit_Resolver
{
    public partial class SubResolverForm : Form
    {
        private readonly BackgroundWorker ResolveBackgroundWorker;
        private bool closePending;

        public SubResolverForm()
        {
            InitializeComponent();
            FormClosing += SubResolverForm_FormClosing;
            NewSubCheckBox.Checked = Settings.Default.NewSub;

            // Subscribe to GraphUtil progress event
            GraphUtil.OnProgressUpdate += OnResolveProgressUpdate;
            SaveUtil.OnProgressUpdate += OnResolveProgressUpdate;

            // Initialise backgroundworker
            ResolveBackgroundWorker = new BackgroundWorker();
            ResolveBackgroundWorker.WorkerSupportsCancellation = true;
            ResolveBackgroundWorker.DoWork += ResolveBackgroundWorker_DoWork;
            ResolveBackgroundWorker.RunWorkerCompleted += ResolveBackgroundWorker_RunWorkerCompleted;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            string result = FormUtil.ShowFileBrowserDialog();
            if (result == "") return;
            FilepathTextBox.Text = result;
            XDocument sub = IoUtil.LoadSub(result);
            pictureBox1.Image = FormUtil.GetImageFromString(sub.Root?.Attribute("previewimage")?.Value);
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            GoButton.Enabled = false;
            ResolveBackgroundWorker.RunWorkerAsync();
        }

        private void OnResolveProgressUpdate(float value, string label)
        {
            Invoke((Action) delegate
                            {
                                progressBar1.Value = (int) Math.Clamp(value * 100, 0, 100);
                                label1.Text = label;
                            });
        }

        private void ResolveBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string inputFilepath = FilepathTextBox.Text;
            string outputFilepath = NewSubCheckBox.Checked
                                        ? Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                                                       Path.GetFileNameWithoutExtension(inputFilepath) +
                                                       "_resolved.sub")
                                        : inputFilepath;
            string graphFilepath = Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                                                Path.GetFileNameWithoutExtension(inputFilepath) + ".graphml");

            (XDocument resolvedSubmarine, QuickGraph.AdjacencyGraph<Vertex, Edge<Vertex>> graph) =
                GraphUtil.ResolveCircuit(FilepathTextBox.Text);
            if (ResolveBackgroundWorker.CancellationPending) return;

            resolvedSubmarine.SaveSub(outputFilepath);
            graph.SaveGraphML(graphFilepath);
        }

        private void ResolveBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            label1.Text = "Done.";
            GoButton.Enabled = true;
            if (closePending) Close();
            closePending = false;
        }

        private void NewSubCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.NewSub = NewSubCheckBox.Checked;
            Settings.Default.Save();
        }

        private void SubResolverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ResolveBackgroundWorker.IsBusy)
            {
                closePending = true;
                ResolveBackgroundWorker.CancelAsync();
                e.Cancel = true;
                Enabled = false;
            }
        }

        private void SubResolverForm_Load(object sender, EventArgs e)
        {
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
        }
    }
}
