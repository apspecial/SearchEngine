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
            textBox1.Clear();
            //textBox1.Text = "Start Index";
			string indexPath = label4.Text;
			string fileofPath = label3.Text;
			LuceneApplication LuceneApp = new LuceneApplication(fileofPath,indexPath);
			string outInfo = LuceneApp.GenIndex();
            textBox1.Text = outInfo;
			//LuceneApp.GenSearch();
			//IndexGen.ReadFile(fileofPath);

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



		private void button4_Click(object sender, EventArgs e)
		{
            //clear old result
            //label8.Text = "Submitted Query:";
            textBox4.Text = "";
            //richTextBox1.Text = "";

            string indexPath = label4.Text;
			string fileofPath = label3.Text;
			string searchText = textBox2.Text;
            // string searchField = comboBox1.SelectedItem.ToString();


            dataGridView1.Visible = true;
            //fill the whole area
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

			dataGridView1.Rows.Clear();
			dataGridView1.Refresh();
            //dataGridView1.AutoResizeColumn();
           // string[] row1 = new string[] { "Meatloaf", "Main Dish", "ground beef",
           //"**" };
           // dataGridView1.Rows.Add(row1);

           // //test list
           // List<string> test_grid = new List<string>();
           // test_grid.Add("1");
           // test_grid.Add("2");
           // test_grid.Add("3");


           // DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
           // row.Cells[0].Value ="1";
           // row.Cells[1].Value ="2";
           // row.Cells[2].Value = "3";

           // dataGridView1.Rows.Add(row);


            //// ... Create 2D array of strings.
            //string[,] array = new string[,]
            //{
            //    {"cat", "dog"},
            //    {"bird", "fish"},
            //};
            //// ... Print out values.
            //Console.WriteLine(array[0, 0]);
            //Console.WriteLine(array[0, 1]);
            //Console.WriteLine(array[1, 0]);
            //Console.WriteLine(array[1, 1]);



            //            // Create a new DataTable.
            //            DataTable table = new DataTable("Payments");

            //            // Declare variables for DataColumn and DataRow objects.
            //            DataColumn column;
            //            DataRow row;

            //            // Create new DataColumn, set DataType, 
            //            // ColumnName and add to DataTable.    
            //            column = new DataColumn();
            //            column.DataType = System.Type.GetType("System.Int32");
            //            column.ColumnName = "id";
            //            column.ReadOnly = true;
            //            column.Unique = true;
            //           // column.Caption = LocalizedCaption.get("id"); //LocalizedCaption is my library to retrieve the chinese caption

            //// Add the Column to the DataColumnCollection.
            //            table.Columns.Add(column);


            //            // Create three new DataRow objects and add them to the DataTable
            //            for (int i = 0; i <= 2; i++)
            //            {
            //                row = table.NewRow();
            //                row["id"] = i;
            //                table.Rows.Add(row);
            //            }

            //            //assign the DataTable as the datasource for a DataGridView
            //            dataGridView1.DataSource = table;




            //create the dictionary for the queryField and queryText
            //Dictionary<int, string> dic_search = new Dictionary<int, string>();
            //dic_search.Add(selcetField, searchText);

            
            if ((indexPath == "") || (indexPath == null) || (indexPath == "None")) 
			{
				MessageBox.Show("Please input the directory of Index!");
			}
            else if (searchText == "")
            {
                MessageBox.Show("Please input the query!");
            } 
			else
			{
				LuceneApplication LuceneApp = new LuceneApplication(fileofPath, indexPath,searchText);
               // LuceneApp.QueryText = searchText;

				//List<string> result = new List<string>();
				DataTable table = LuceneApp.GenSearch(); 
				//result= LuceneApp.GenSearch();
                LuceneApp.CleanUpSearch();
				//display the submitted query
				//label8.Text += LuceneApp.SubmittedQuery;
				dataGridView1.DataSource = table;
                textBox4.Text += LuceneApp.SubmittedQuery;
                //display the search result
    //            foreach (string s in result)
				//{
    //                //richTextBox1.Text += s;
				//}
			}
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Text|*.txt";
			saveFileDialog1.Title = "Save an Text File";
			saveFileDialog1.ShowDialog();
			//saveFileDialog1.FileName

			string indexPath = label4.Text;
            string fileofPath = label3.Text;
            LuceneApplication testApp = new LuceneApplication(fileofPath,indexPath);

            //testApp.ReadInfoNeed();
			testApp.TestFun(saveFileDialog1.FileName);

        }
    }
}
