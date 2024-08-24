using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDIPaint
{
    public partial class StarCreation : Form
    {
        public StarCreation()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != null)
            {
                MainForm.Tool = "Star";
                if(int.TryParse(textBox1.Text, out int n))
                {
                    DocumentForm.starEndCount = n;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Введено нечисло или неверное число.");
                }
            }
        }

    }
}
