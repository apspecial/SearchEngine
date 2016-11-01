using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;



namespace SearchEngie
{
    public partial class Form1 : Form
    {
		public List<DataTable> allTables = new List<DataTable>();
        public Form1()
        {
            InitializeComponent();
            //adding the list frome file
            comboBox1.Items.Add(" what \"similarity laws\" must be obeyed when constructing aeroelastic models of heated high speed aircraft ");
            comboBox1.Items.Add("what are the structural and aeroelastic problems associated with flight of high speed aircraft ");
            comboBox1.Items.Add(" how can the aerodynamic performance of channel flow ground effect machines be calculated ");
            comboBox1.Items.Add("in summarizing theoretical and experimental work on the behaviour of a typical aircraft structure in a noise environment is it possible to develop a design procedure ");
            comboBox1.Items.Add("has anyone developed an analysis which accurately establishes the large deflection behaviour of \"conical shells\"");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
			string indexPath = label4.Text;
			string fileofPath = label3.Text;

			//check the path is valid or not
			if ((indexPath == "") || (indexPath == null) || (indexPath == "None"))
			{
				MessageBox.Show("Please input the directory of Index!");
			}
			else if ((fileofPath == "") || (fileofPath == null) || (fileofPath == "None"))
			{
				MessageBox.Show("Please input the directory of Files!");
			}
			else
			{
				LuceneApplication LuceneApp = new LuceneApplication(fileofPath, indexPath);
				string outInfo = LuceneApp.GenIndex();
				textBox1.Text = outInfo;
			}

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
            

            FolderBrowserDialog fbd = new FolderBrowserDialog();

			//show the directory choose dialogue
            fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
				//display the selected path
                label3.Text = fbd.SelectedPath;
                label3.MaximumSize = new Size(150, 0);
                label3.AutoSize = true;

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

			// show the directory choose dialogue
            fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
				//display the selected path
                label4.Text = fbd.SelectedPath;
                label4.MaximumSize = new Size(150, 0);
                label4.AutoSize = true;


            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            //clear old result
            textBox4.Text = "";
            allTables.Clear();


            string indexPath = label4.Text;
            string fileofPath = label3.Text;
            string searchText = comboBox1.Text;

			//setting properties of the table
            dataGridView1.Visible = true;
            dataGridView1.ReadOnly = true;
            //fill the whole area
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


			//check the path is valid or not
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
				//create LuceneApplication objects
                LuceneApplication LuceneApp = new LuceneApplication(fileofPath, indexPath, searchText);
				//get the satue of checkbox
                bool IScheck = checkBox1.Checked;
                int hitNum;
                DateTime startTime = DateTime.Now;

                DataTable table = LuceneApp.GenSearch(IScheck, out hitNum);

				//divide the table into  subtables
                var showtables = table.AsEnumerable().ToChunks(10).Select(rows => rows.CopyToDataTable());

                //display the current page
                int items = showtables.Count();
                string showMsg = "Page 1 of " + items + " pages";
                label7.Text = showMsg;

				//add each subtable to a big table
                foreach (DataTable innertable in showtables)
                {
                    allTables.Add(innertable);
                }

				// setting parameter of first subtable
                dataGridView1.DataSource = allTables[0];
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[6].Visible = false;
                dataGridView1.Columns[7].Visible = false;
                dataGridView1.Columns[0].Width = 50;
     			// filling all the area
                var lastColIndex = dataGridView1.Columns.Count - 1;
                var lastCol = dataGridView1.Columns[lastColIndex];
                lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

				//display the submitted query
                textBox4.Text += LuceneApp.SubmittedQuery;
                DateTime endTime = DateTime.Now;
                TimeSpan useTime = endTime - startTime;
                //display information about searching
                label9.Text = "Find " + hitNum + " " + " resluts.  ( In " + useTime.TotalSeconds + " Seconds ) ";
            }
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			
            //show the abstract message when clicked
			if ((dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null) && (e.ColumnIndex==4))
			{
				
				MessageBox.Show(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex+1].Value.ToString());
			}
		}

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Text|*.txt";
			saveFileDialog1.Title = "Save an Text File";
			saveFileDialog1.ShowDialog();
            string saveFileName = saveFileDialog1.FileName;
            //List<String> ListData = new List<String>();
            string[] listDocID = { "001", "002", "023", "157", "219" };
            string topicID;
            if ((comboBox1.SelectedIndex >= 0) && (comboBox1.SelectedIndex < 5))
            {
                topicID = listDocID[comboBox1.SelectedIndex];
            }
            else
            {
                topicID = "Invalid";
            }


            //check the file name is valid or not
              if (!string.IsNullOrEmpty(saveFileName))
             {
              
                //write file
                using (StreamWriter writetext = File.AppendText(saveFileName))
                {
                    foreach (DataTable table in allTables)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            string s = topicID + "\t" + "Q0" + "\t" + table.Rows[i][6] + "\t" + table.Rows[i][0] + "\t" + table.Rows[i][7]
                                + "\t" + "n9391576_n9835580_dreamers";
                            writetext.WriteLine(s);

                        }
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(label7.Text);
            string currentPage = match.Value;
			//get the current page
            int pageNum = int.Parse(currentPage);
			//control the page -1 
            if(pageNum>1)
            {
                dataGridView1.DataSource = allTables[pageNum - 2];

				//setting the display parameter of table
				dataGridView1.Columns[5].Visible = false;
				dataGridView1.Columns[6].Visible = false;
				dataGridView1.Columns[7].Visible = false;
				dataGridView1.Columns[0].Width = 50;
				var lastColIndex = dataGridView1.Columns.Count - 1;
				var lastCol = dataGridView1.Columns[lastColIndex];
				lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

				//update current page info
                string curPage = (pageNum - 1).ToString();
                label7.Text = "Page " + curPage + " of " + allTables.Count + " pages";
            }
            

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(label7.Text);
            string currentPage = match.Value;
			//get the current page
            int pageNum = int.Parse(currentPage);
            int items = allTables.Count;
			//control the page + 1
            if ((pageNum >= 1)&&(pageNum<items))
            {
                dataGridView1.DataSource = allTables[pageNum];

				//setting the display parameter of table
				dataGridView1.Columns[5].Visible = false;
				dataGridView1.Columns[6].Visible = false;
				dataGridView1.Columns[7].Visible = false;
				dataGridView1.Columns[0].Width = 50;
				var lastColIndex = dataGridView1.Columns.Count - 1;
				var lastCol = dataGridView1.Columns[lastColIndex];
				lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				//update current page info
                string curPage = (pageNum + 1).ToString();
                label7.Text = "Page " + curPage + " of " + allTables.Count + " pages";
            }
           
        }
    }
}
