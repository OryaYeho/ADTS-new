using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static ADTS.ADTSData;
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
            openFileDialog1.InitialDirectory = Tools.DBEDir;
            openFileDialog1.ShowDialog();
            
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
            foreach(string path in openFileDialog1.FileNames)
            {
                string filename = Tools.GetDocName(path);
                Document dc = new();
                //file is in dbe
                if (path.Contains(Tools.DBEDir))
                {
                    dc = new Document(path, textBox1.Text);
                    File.Move(path, Tools.fileDir + filename);
                    Tools.SaveDocument(dc);
                }
                //file is in dbi
                else if (path.Contains(Tools.fileDir))
                {
                    dc = Tools.GetDocumentByName(filename);
                    dc.catlist.Add(textBox1.Text);
                    Tools.UpdateDocument(dc);
                } //file is not from dbe nor dbi
                else
                {
                    dc = new Document(path, textBox1.Text);
                    File.Copy(path, Tools.fileDir + filename);
                    Tools.SaveDocument(dc);
                }


                /*string dbipath = str.Replace("DBE", "DBI");
                Document dc = Tools.GetDocumentByName(filename);
                if(dc == null)
                {
                    dc = new Document(str, textBox1.Text);
                    //dc.path = Tools.fileDir + filename;
                    Tools.SaveDocument(dc); 
                    File.Move(str, Tools.fileDir+filename);
                    
                }
                else
                {
                    dc.catlist.Add(textBox1.Text);
                    Tools.UpdateDocument(dc);
                }*/
                dclist.Add(dc);
            }
            Category ct1 = new(textBox1.Text, dclist);
            Tools.SaveCategory(ct1);
            var temp = Tools.GetAllCategories();
            this.Close();
            //new ADTS();
            
        }
    }
}
