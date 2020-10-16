using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//ФОРМА ОБЩИХ НАСТРОЕК
namespace try_interface
{
    public partial class Settings_Form : Form
    {
        Main_Form Parent;
        public Settings_Form(Main_Form f)
        {
            InitializeComponent();
            Load_From_File();
            textBox_N.Leave += TextChanged_int;
            textBox_time_e.Leave += TextChanged_double;
            Parent = f;


        }
        public Settings_Form()
        {
            InitializeComponent();
            Load_From_File();
            


        }
        //изменить файл
        private void button_Apply_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("private_settings.txt"))
            {
                sw.WriteLine(textBox_N.Text);
                sw.WriteLine(textBox_time_e.Text);
                sw.WriteLine(Convert.ToString(checkBox_Knagr.Checked));
                sw.WriteLine(Convert.ToString(checkBox_Poss.Checked));
                sw.WriteLine(Convert.ToString(checkBox_BSV.Checked));
            }
            this.Close();

        }
        //загрузка предварительных настроек
        private void Load_From_File()
        {
            string[] line = new string[5];
            // Read the file and display it line by line.  
            using (var streamReader = new StreamReader("private_settings.txt"))
            {
                line[0] = streamReader.ReadLine(); //после функции к int
                line[1] = streamReader.ReadLine(); // после функции к double
                line[2] = streamReader.ReadLine(); // bool
                line[3] = streamReader.ReadLine(); // bool
                line[4] = streamReader.ReadLine(); // bool
            }
            textBox_N.Text = line[0];
            textBox_time_e.Text = line[1];
            checkBox_Knagr.Checked = Convert.ToBoolean(line[2]);
            checkBox_Poss.Checked = Convert.ToBoolean(line[3]);
            checkBox_BSV.Checked = Convert.ToBoolean(line[4]);
        }
	//при закрытии формы обновляются поля с настройками резервирования основной формы
        private void Settings_Form_FormClosing(object sender, FormClosingEventArgs e)
        {

            if(Parent.Enable_Return)
                Parent.Return();
        }

//валидация полей с целыми значениями
        private void TextChanged_int(object sender, EventArgs e)
        {
            Check_TextBoxes();
            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;
                
                try
                {
                    int s = int.Parse(text);
                }
                catch (FormatException e1)
                {
                    ((TextBox)sender).BackColor = Color.Red;
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;


        }
//валидация полей с дробными значениями
        private void TextChanged_double(object sender, EventArgs e)
        {
            Check_TextBoxes();
            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;
                
                try
                {
                    double s = double.Parse(text);
                }
                catch (FormatException e1)
                {
                    ((TextBox)sender).BackColor = Color.Red;
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;
        }
//проверка заполненности полей
        private void Check_TextBoxes()
        {
            bool enable = true;
            foreach (TextBox t in this.Controls.OfType<TextBox>())
                if (t.Text == "")
                {
                    enable = false;
                }
            if (enable)
            {
                button_Apply.Enabled = true;
            }
            else
            {
                button_Apply.Enabled = false;
            }
        }
    }
}
