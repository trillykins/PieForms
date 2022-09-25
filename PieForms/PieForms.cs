using PieForms;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace WinFormsApp1
{
    public partial class PieForms : Form
    {
        private string filename = "RunConfiguration.sdf";

        private string workingDirecory = Directory.GetCurrentDirectory();
        private string whereToCreate = $"{Directory.GetCurrentDirectory()}/RunConfiguration.sdf";

        private string[] GetSelectedItems() => listBox1.SelectedItems.Cast<string>().ToArray();

        List<ListViewItem> selectedItems = new List<ListViewItem>();
        private readonly PieWrapper wrapper;

        public PieForms()
        {
            InitializeComponent();

            // Define the border style of the form to a dialog box.
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Set the MaximizeBox to false to remove the maximize box.
            this.MaximizeBox = false;

            // Set the start position of the form to the center of the screen.
            this.StartPosition = FormStartPosition.CenterScreen;

            Text = nameof(PieForms);
            wrapper = new PieWrapper();
            button1.Enabled = false;    // Export
            button5.Enabled = false;    // Remove
            button6.Enabled = false;    // Save To...


            listBox1.Items.AddRange(Directory.GetFiles(@"C:\Users\trill\Downloads"));
            listBox1.SelectionMode = SelectionMode.MultiExtended;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                button1.Enabled = true;
                button5.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                button5.Enabled = false;
            }
        }

        // Export
        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var filesToExport = GetSelectedItems();
                    wrapper.PieExport(filesToExport);
                    foreach (var file in filesToExport)
                    {
                        var fileInfo = new FileInfo(file);
                        File.Copy(file, $"{fbd.SelectedPath}/{fileInfo.Name}");
                    }
                }
            }
        }

        // Create
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Spatial Data File|*.sdf";
            saveFileDialog1.Title = "Save Spatial Data File";
            saveFileDialog1.ShowDialog();
            listBox1.Items.Clear();
            wrapper.PieCreate(saveFileDialog1.FileName);
        }

        // Open file
        private void button4_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Spatial Data File|*.sdf";
                DialogResult result = fd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fd.FileName))
                {
                    filename = fd.FileName;
                    File.Copy(fd.FileName, "RunConfiguration.sdf");
                    var files = wrapper.PieList();
                    listBox1.Items.AddRange(files);
                }
            }
        }

        // Import files
        private void button2_Click(object sender, EventArgs e)
        {
            // Copy files to working directory
            // Run import command
            // Refresh list
            using (var fd = new OpenFileDialog())
            {
                fd.Multiselect = true;
                DialogResult result = fd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var files = new List<FileInfo>();
                    foreach (var file in fd.FileNames)
                    {
                        var fileInfo = new FileInfo(file);
                        File.Copy(file, $"{workingDirecory}/{fileInfo.Name}");
                        files.Add(new FileInfo(fileInfo.Name));
                    }

                    wrapper.PieImport(files.Select(x => x.Name));
                    var list = wrapper.PieList();

                    foreach (var filename in list)
                    {
                        var lvi = new ListViewItem
                        {
                            Text = filename
                        };
                        listBox1.Items.Add(lvi);
                    }
                }
            }

        }

        // Remove
        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems == null || listBox1.SelectedIndex == -1 || listBox1.SelectedItems.Count == 0) return;
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove selected files?", "Remove files", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var filesToRemove = GetSelectedItems();
                wrapper.PieRemove(filesToRemove);
            }
        }

        // Save To...
        private void button6_Click(object sender, EventArgs e)
        {
            File.Copy("RunConfiguration.sdf", whereToCreate);
        }
    }
}