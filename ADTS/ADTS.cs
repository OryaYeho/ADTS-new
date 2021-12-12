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
using Microsoft.VisualBasic;


namespace ADTS
{
    public partial class ADTS : Form
    {
        private ToolStripItem lastClicked;
        public ADTS()
        {
            InitializeComponent();
            this.CenterToScreen();
            
            //lastClicked = toolStrip1.Items[0];
           // toolStrip1.Items[0].BackColor = Color.LightSteelBlue;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;
            
            //toolStrip1.Items.Add("Set Similarity Level (" + File.ReadAllText(Tools.similarityLevel)+"%)").Alignment = ToolStripItemAlignment.Right;
            toolStrip1.Items.Add("New Category+").Alignment = ToolStripItemAlignment.Right;


            //temp filling gui
            toolStrip1.Items.Add("cat");
            for (int i =0;i<30; i++)
            {
                TextBox textBox1 = new TextBox();
                //textBox1.Text = arr[i];
                dataGridView1.Rows.Add(i.ToString());
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

        }

        

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem.Text.Contains("New Category"))
            {
                AddCategory newcat = new();
                this.Hide();
                newcat.Show();
                return;
            };
            if (e.ClickedItem.Text.Contains("Set Similarity Level"))
            {
                new SimilairtyLevel().Show();
                this.Hide();
                return;
            }
            lastClicked.BackColor = Color.Transparent;
            lastClicked = e.ClickedItem;
            e.ClickedItem.BackColor = Color.LightSteelBlue;
            
           
     
        }
        private void setSimilairtyLevel(int level)
        {
            File.WriteAllTextAsync(Tools.similarityLevel, level.ToString());
        }

        private void ADTS_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
