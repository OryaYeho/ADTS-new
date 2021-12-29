using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ADTS.ADTSData;

namespace ADTS
{
    public partial class NonRelevant : Form
    {
        public NonRelevant()
        {
            InitializeComponent();
            this.CenterToScreen();
            Load();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        public void Load()
        {
            //clear all
            dataGridView1.Rows.Clear();
            checkedListBox1.Items.Clear();

            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.Anchor = AnchorStyles.Bottom;

            foreach (var cat in Tools.GetAllCategories())
            {
                checkedListBox1.Items.Add(cat.Name);
                
            }
            
            foreach (string path in Directory.GetFiles(Tools.NonRelevant))
            {
                dataGridView1.Rows.Add(Tools.GetDocName(path));
            }
            



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void NonRelevant_FormClosed(object sender, FormClosedEventArgs e)
        {
            new ADTS().Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please pick at least one category");
                return;
            }
            if(dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please pick at least one document");
                return;
            }
            var files = dataGridView1.SelectedRows;
            var categories = checkedListBox1.CheckedItems;
            foreach(DataGridViewRow file in dataGridView1.SelectedRows)
            {
                string fileName = file.Cells[0].Value.ToString();
                Document dc = new Document(Tools.NonRelevant + fileName);
                foreach(string cat in checkedListBox1.CheckedItems)
                {
                    dc.catlist.Add(cat);
                    var tempCategory = Tools.GetCategory(cat);
                    tempCategory.MergeDictnionaries(dc);
                    Tools.UpdateCategory(tempCategory);
                }
                File.Move(Tools.NonRelevant + fileName, Tools.fileDir+fileName);
                Tools.SaveDocument(dc);
            }
            Load();
            this.Cursor = Cursors.Default;
            if (dataGridView1.Rows.Count == 0)
            {
                this.Close();
            }
        }
    }
}
