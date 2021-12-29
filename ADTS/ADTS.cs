using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using static ADTS.ADTSData;

namespace ADTS
{
    public partial class ADTS : Form
    {
        private ToolStripItem lastClicked;
        public ADTS()
        {
            InitializeComponent();
            this.CenterToScreen();
            Load();
            //lastClicked = toolStrip1.Items[0];
           // toolStrip1.Items[0].BackColor = Color.LightSteelBlue;
           
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

        }

        

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            
            lastClicked.BackColor = Color.Transparent;
            lastClicked = e.ClickedItem;
            e.ClickedItem.BackColor = Color.LightSteelBlue;
            dataGridView1.Rows.Clear();
            foreach (Document dc in Tools.GetDocumentByCatName(e.ClickedItem.Text))
            {
                dataGridView1.Rows.Add(dc.Name);
            }

        }
        private void setSimilairtyLevel(int level)
        {
            File.WriteAllTextAsync(Tools.similarityLevel, level.ToString());
        }

        private void ADTS_FormClosed(object sender, FormClosedEventArgs e)
        {
            var v = Tools.GetAllCategories();
            Tools.TempAdd(new Document() { });
            return;
            //Program.db.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.UpdateSystem();
            this.Refresh();
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text.Contains("New Category"))
            {
                AddCategory newcat = new();
                this.Hide();
                newcat.Show();
                return;
            }
             if (e.ClickedItem.Text.Contains("Set Similarity Level"))
            {
                new SimilairtyLevel().Show();
                this.Hide();
                return;
            }
             if (e.ClickedItem.Text.Contains("Refresh"))
            {
                if (Tools.GetAllCategories().Count == 0)
                {
                    MessageBox.Show("You haven't created categories yet");
                    return;
                }
                this.Cursor = Cursors.WaitCursor;
                Program.UpdateSystem();
                Load();
                this.Cursor = Cursors.Default;
                //new ADTS().Show();
                //this.Close();
            }
            if (e.ClickedItem.Text.Contains("Reset"))
            {
                this.Cursor = Cursors.WaitCursor;
                Program.ResetSystem();
                Load();
                this.Cursor = Cursors.Default;
                MessageBox.Show("System has been reset");
                //new ADTS().Show();
                //this.Close();
            }
            if (e.ClickedItem.Text.Contains("Non Relevant")){
                //check if there are categories or files in Nonrelevant
                if(Tools.GetAllCategories().Count == 0)
                {
                    MessageBox.Show("You haven't created categories yet");
                    return;
                }
                if (Directory.GetFiles(Tools.NonRelevant).Length==0)
                {
                    MessageBox.Show("No documents to classify");
                    return;
                }
                var classify = new NonRelevant();
                this.Hide();
                classify.Show();
                return;
            }
        }
        public void Load()
        {
            //clearing old data
            dataGridView1.Rows.Clear();
            toolStrip1.Items.Clear();
            toolStrip2.Items.Clear();
            //end of clearing data

            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;
            toolStrip2.Items.Add("New Category+");//.Alignment = ToolStripItemAlignment.Right;
            toolStrip2.Items.Add("Set Similarity Level (" + File.ReadAllText(Tools.similarityLevel) + "%)");//.Alignment = ToolStripItemAlignment.Right;
            toolStrip2.Items.Add("Refresh");
            toolStrip2.Items.Add("Reset");
            toolStrip2.Items.Add("Non Relevant");
            var cat_list = Tools.GetAllCategories();

            //if there are no categories in the database/system
            if (cat_list.Count == 0)
            {
                dataGridView1.Visible = false;
                return;
            }

            //else
            foreach (Category ct in cat_list)
            {
                toolStrip1.Items.Add(ct.Name);
            }
            toolStrip1.Items[0].BackColor = Color.LightSteelBlue;
            lastClicked = toolStrip1.Items[0];
            //lastClicked.BackColor = Color.LightSteelBlue; 
            //toolStrip1.Items[0].BackColor = Color.LightSteelBlue;
            foreach (Document dc in Tools.GetDocumentByCatName(cat_list[0].Name))
            {
                dataGridView1.Rows.Add(dc.Name);
            }
        }
        }
}
