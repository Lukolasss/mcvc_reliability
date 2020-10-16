using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//НАЧАЛЬНАЯ ФОРМА 
namespace try_interface
{
    public partial class Welcome_Form : Form
    {
        public Welcome_Form()
        {
            InitializeComponent();
        }
//открытие основной формы по нажатию кнопки "Запуск"
        private void button_Start_Click(object sender, EventArgs e)
        {

            Main_Form Main_Form = new Main_Form();
            Main_Form.Show();

            this.Hide();
        }
    }
}
