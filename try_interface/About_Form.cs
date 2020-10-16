using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace try_interface
{
    public partial class About_Form : Form
    {
        public About_Form()
        {
            InitializeComponent();
        }

        private void About_Form_Load(object sender, EventArgs e)
        {
            // richTextBox_About.LoadFile("about.rtf", RichTextBoxStreamType.RichText);
            radioButton_res.Checked = true;
            richTextBox_About.Rtf = Properties.Resources.about_res;
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_res.Checked == true)
            {
                richTextBox_About.Rtf = Properties.Resources.about_res;
            }
            else
            {
                richTextBox_About.Rtf = Properties.Resources.about_prog;
            }
        }
    }
}
