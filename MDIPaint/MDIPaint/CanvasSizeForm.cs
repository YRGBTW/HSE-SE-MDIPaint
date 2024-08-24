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
    public partial class CanvasSizeForm : Form
    {
        public int height, width;
        DocumentForm dForm;
        public CanvasSizeForm(DocumentForm documentForm)
        {
            InitializeComponent();
            dForm = documentForm; 
            textBox1.Text = documentForm.Width.ToString();
            textBox2.Text = documentForm.Height.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (height > 10 && width > 10)
            {
                dForm.SizeChange(height, width);
            }
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(!int.TryParse(textBox1.Text,out width))
            {
                textBox3.Text = "Неверный ввод";
            }
            else
            {
                textBox3.Text = "";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out height))
            {
                textBox3.Text = "Неверный ввод";
            }
            else
            {
                textBox3.Text = "";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Отмена_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
