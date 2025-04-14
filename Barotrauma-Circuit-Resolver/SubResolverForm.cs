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
            SaveGraphCheckBox.Checked = Settings.Default.SaveGraph;
            InvertMemoryCheckBox.Checked = Settings.Default.InvertMemory;
            RetainParallelCheckBox.Checked = Settings.Default.RetainParallel;
            PickingTimeSortBox.Checked = Settings.Default.PickingTimeSort;

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
            string inputFilepath = FormUtil.ShowFileBrowserDialog();
            if (string.IsNullOrWhiteSpace(inputFilepath)) return;
            FilepathTextBox.Text = inputFilepath;

            bool isSubFile = Path.GetExtension(inputFilepath)!.Equals(".sub", StringComparison.OrdinalIgnoreCase);
            XDocument inputDocument = isSubFile ? IoUtil.LoadSub(inputFilepath) : XDocument.Load(inputFilepath);

            if (isSubFile)
            {
                string preview = inputDocument.Root?.Attribute("previewimage")?.Value;
                if (!string.IsNullOrWhiteSpace(preview))
                    pictureBox1.Image = FormUtil.GetImageFromString(inputDocument.Root?.Attribute("previewimage")?.Value);
            }
            else
                pictureBox1.Image = null; // Assemblies do not contain preview images
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            GoButton.Enabled = false;
            ResolveBackgroundWorker.RunWorkerAsync();
        }

        private void OnResolveProgressUpdate(float value, string label)
        {
            Invoke((Action)delegate
                            {
                                progressBar1.Value = (int)Math.Clamp(value * 100, 0, 100);
                                label1.Text = label;
                            });
        }

        private void ResolveBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string inputFilepath = FilepathTextBox.Text;
            if (string.IsNullOrWhiteSpace(inputFilepath))
            {
                ResolveBackgroundWorker.CancelAsync();
                return;
            }

            string outputFilepath = NewSubCheckBox.Checked
                                        ? Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                                                       $"{Path.GetFileNameWithoutExtension(inputFilepath)} resolved{Path.GetExtension(inputFilepath)}")
                                        : inputFilepath;

            string graphFilepath = Path.Combine(Path.GetDirectoryName(inputFilepath)!,
                                                $"{Path.GetFileNameWithoutExtension(inputFilepath)}.graphml");

            bool isSubFile = Path.GetExtension(inputFilepath)!.Equals(".sub", StringComparison.OrdinalIgnoreCase);
            XDocument inputDocument = isSubFile ? IoUtil.LoadSub(inputFilepath) : XDocument.Load(inputFilepath);

            (XDocument resolvedSubmarine, QuickGraph.AdjacencyGraph<Vertex, Edge<Vertex>> graph) =
                GraphUtil.ResolveCircuit(inputDocument, InvertMemoryCheckBox.Checked, RetainParallelCheckBox.Checked, PickingTimeSortBox.Checked);

            if (ResolveBackgroundWorker.CancellationPending) { return; }

            // Update Submarine Name if a new file is made
            if (NewSubCheckBox.Checked)
            {
                resolvedSubmarine.Root.Attribute("name").Value = resolvedSubmarine.Root.Attribute("name").Value + " resolved";
            }

            if (isSubFile)
            {
                resolvedSubmarine.SaveSub(outputFilepath);
            }
            else
            {
                File.WriteAllText(outputFilepath, resolvedSubmarine.ToString());
            }

            if (SaveGraphCheckBox.Checked)
            {
                graph.SaveGraphML(graphFilepath);
            }
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
        private void SaveGraphCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SaveGraph = SaveGraphCheckBox.Checked;
            Settings.Default.Save();
        }
        private void InvertMemoryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SaveGraph = InvertMemoryCheckBox.Checked;
            Settings.Default.Save();
        }
        private void RetainParallelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.SaveGraph = RetainParallelCheckBox.Checked;
            Settings.Default.Save();
        }
        private void PickingTimeSort_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.PickingTimeSort = PickingTimeSortBox.Checked;
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

        private void SettingGroupBox_Enter(object sender, EventArgs e)
        {

        }

    }
}
