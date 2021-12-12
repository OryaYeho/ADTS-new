using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ADTS.Tools;

namespace ADTS
{
    public partial class AddCategory : Form
    {   private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        public AddCategory()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AddCategory_FormClosed(object sender, FormClosedEventArgs e)
        {
            new ADTS().Show();
        }

        private void AddCategory_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.Multiselect = true;
            openFileDialog1.ShowDialog();
            openFileDialog1.InitialDirectory = Tools.DBEDir;
            foreach(string str in openFileDialog1.FileNames)
            {
                
                listBox1.Items.Add(str);
            }
            


        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
            if (textBox1.Text.Equals(""))
            {
                MessageBox.Show("Name of category can't be empty!");
                return;
            }
            if(openFileDialog1.FileNames.Count() == 0)
            {
                
                MessageBox.Show("Pick at least 1 file for the category!");
                return;
            }
            foreach(Category ct in GetAllCategories())
            {
                if (ct.Name.Equals(textBox1.Text))
                {
                    MessageBox.Show("This category name already exists!");
                    return;
                }
            }
            List<Document> dclist = new();
            foreach(string str in openFileDialog1.FileNames)
            {
                string filename = Tools.GetDocName(str);
                string dbipath = str.Replace("DBE", "DBI");
                Document dc = Tools.GetDocument(filename);
                if(dc == null)
                {
                    dc = new Document(str, textBox1.Text);
                    dc.path = Tools.fileDir + filename;
                    Tools.SaveDocument(dc); 
                    File.Move(str, Tools.fileDir+filename);
                }
                else
                {
                    dc.catlist.Add(textBox1.Text);
                    Tools.UpdateDocument(dc);
                }
                dclist.Add(dc);
                
            }
            Category ct1 = new(textBox1.Text, dclist);
            Tools.SaveCategory(ct1);
            this.Close();
            
        }
    }
}
