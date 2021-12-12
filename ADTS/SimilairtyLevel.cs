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

namespace ADTS
{
    public partial class SimilairtyLevel : Form
    {
        public SimilairtyLevel()
        {

            InitializeComponent();
            this.CenterToScreen();
            textBox1.Text = File.ReadAllText(Tools.similarityLevel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(maskedTextBox1.Text.Equals(""))
            {
                MessageBox.Show("Enter a number!");
                return;
            }
            int value = int.Parse(maskedTextBox1.Text);
            if (value > 100)
            {
                 MessageBox.Show("Enter a number below 100");
                return;
            }
            File.WriteAllText(Tools.similarityLevel, value.ToString());
            this.Close();
            
        }

        private void SimilairtyLevel_FormClosed(object sender, FormClosedEventArgs e)
        {
            new ADTS().Show();
        }
    }
}
