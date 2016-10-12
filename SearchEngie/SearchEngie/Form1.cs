using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace SearchEngie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
			LuceneApplication IndexGen = new LuceneApplication();
			string indexPath = label3.Text;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             // Create an instance of the open file dialog box.
             OpenFileDialog openFileDialog1 = new OpenFileDialog();

             // Set filter options and filter index.
             openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
             openFileDialog1.FilterIndex = 1;

             openFileDialog1.Multiselect = true;

             // Call the ShowDialog method to show the dialog box.

             DialogResult userClickedOK = openFileDialog1.ShowDialog();

             // Process input if the user clicked OK.
             if (userClickedOK == true)
             {
                 // Open the selected file to read.
                 System.IO.Stream fileStream = openFileDialog1.File.OpenRead();

                 using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                 {
                     // Read the first line from the file and write it the textbox.
                     tbResults.Text = reader.ReadLine();
                 }
                 fileStream.Close();
             }
             */

            /*
            OpenFileDialog OF = new OpenFileDialog();
            OF.InitialDirectory = "C:\\";
            OF.Filter = "All files (*) | *.*";

            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OF.FilterIndex = 0;
                OF.RestoreDirectory = true;
                label3.Text = (OF.FileName);
            }
            */

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                label3.Text = fbd.SelectedPath;

                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You Sure To Exit Programme ?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                label4.Text = fbd.SelectedPath;

                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");

            }
        }
    }
}
