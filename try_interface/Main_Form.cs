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
using System.Threading;

//ОСНОВНАЯ ФОРМА
using try_interface.Reserves;

namespace try_interface
{
    public partial class Main_Form : Form
    {
	//внутренние переменные для сохранения вводимых данных и корректной работы интерфейса 
        private Vvod vvod;
        private int kol_set;
        private int i;
        private int i_without_per;
        private int m = 40;
        public bool Enable_Return = false;
	//предварительные настройки (записываются и считываются из файла "private_settings.txt")
        public int N;
        public double Time_e;
        public bool Use_Coef_Nagr;
        public bool Use_Poss;
        public bool const_BSV;

        public bool changed_exp = false;/*внутренняя переменная для отслеживания переполнения памяти (если при генерации массивов наработок оперативная память выходит за 2 Гб, число экспериментов уменьшается)*/

        public double[] time;
        public double poss;

        Reserve Schema;//класс-предок для видов резервирования (приведение типов происходит после выбора пользователем вида резервирования)

        public Main_Form()
        {
            InitializeComponent();
            this.SetDesktopLocation(40,40);
            vvod = new Vvod();//заполнения всех полей ввода дефолтными значениями
	//настройки формы
            panel_sett.Visible = false;
            button_Calculate.Visible = false;
            panel_Result.Visible = false;
	//создание верхнего меню
            //Проект
            ToolStripMenuItem Project_Item = new ToolStripMenuItem("Проект");
            ToolStripMenuItem Create_New_Project = new ToolStripMenuItem("Создать проект",Properties.Resources.create_new);
            Project_Item.DropDownItems.Add(Create_New_Project);
            ToolStripMenuItem Open_Existing_Project = new ToolStripMenuItem("Открыть существующий", Properties.Resources.open);
            Project_Item.DropDownItems.Add(Open_Existing_Project);

            ToolStripMenuItem Save_Project = new ToolStripMenuItem("Сохранить", Properties.Resources.save);
            Save_Project.Enabled = false;
            Project_Item.DropDownItems.Add(Save_Project);

            menuStrip1.Items.Add(Project_Item);
            Create_New_Project.Click += Create_New_Project_Click;
            Open_Existing_Project.Click += Open_Existing_Project_Click;
            Save_Project.Click += button_Save_Click; 
 
            //Постоянные настройки
            ToolStripMenuItem Settings_Item = new ToolStripMenuItem("Общие настройки");
            Settings_Item.Click += Settings_Item_Click;
            menuStrip1.Items.Add(Settings_Item);

            //Справка
            ToolStripMenuItem About_Item = new ToolStripMenuItem("Справка");
            About_Item.Click += About_Item_Click;
            menuStrip1.Items.Add(About_Item);
        }
	//нажатие кнопки "Справка"
        void About_Item_Click(object sender, EventArgs e)
        {
            About_Form new_About_Form = new About_Form();
            new_About_Form.Show();//открытие формы справки
        }
	//нажатие кнопки "Общие настройки"
        void Settings_Item_Click(object sender, EventArgs e)
        {
            Settings_Form new_Settings_Form = new Settings_Form(this);
            new_Settings_Form.Show();//открытие формы настроек
        }
	//создание нового проекта
        void Create_New_Project_Click(object sender, EventArgs e)
        {
            panel_sett.Visible = true;
            button_Calculate.Visible = true;
            //button_Save.Visible = true;
            comboBox_type.Enabled = true;
            textBox_name.Text = "Новый проект";

        }
	//загрузка общих настроек из файла
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

            N = Convert.ToInt32(line[0]);
            changed_exp = false;
            Time_e = Convert.ToDouble(line[1]);
            Use_Coef_Nagr = Convert.ToBoolean(line[2]);
            Use_Poss = Convert.ToBoolean(line[3]);
            const_BSV = Convert.ToBoolean(line[4]);
        }

	/*ПРИ РЕЗЕРВИРОВАНИИ С РОТАЦИЕЙ событие изменения выбранного вида ротации
(при каждом изменении вида ротации происходит динамическое создание полей для ввода значений циклов полной ротации/ вероятности по ТЗ в соответствии с настройками)*/
        void Check_Changed(object sender, EventArgs e)
        {
		//удаление полей ввода
            while (panel_Settings_diff.Controls.Count > kol_set)
            {
                panel_Settings_diff.Controls.RemoveAt(panel_Settings_diff.Controls.Count - 1);
            }

            i = i_without_per;

            //создание новых полей в соответствии с выбранным видом ротации
            if (((RadioButton)panel_Settings_diff.Controls["radioButton_changing_per"]).Checked)//переменный период
            {
                panel_Settings_diff.Controls.Add(new Label() { Text = "Величины циклов полной ротации:", Name = "label_per_value", Location = new Point(0, m * (i++)), AutoSize = true });
                for (int x = 0; x < Convert.ToInt32(panel_Settings_diff.Controls["textBox_K"].Text); x++)
                    Generate_Settings("Для " + Convert.ToString(x + 1) + "-ого рабочего резерва", "_per" + Convert.ToString(x), Location = new Point(0, m * (i++)), "ч.", vvod.per_chan[x], 4, 0, true);
            }
            else
            {
                if (((RadioButton)panel_Settings_diff.Controls["radioButton_const_per"]).Checked)// постоянный период
                {
                    Generate_Settings("Цикл полной ротации", "_per", Location = new Point(0, m * (i++)), "ч.", vvod.per_const,2);
                }
                else//программа управления ротацией (класс Calc)
                {
                    Generate_Settings("Вероятность по ТЗ", "_pos_TS", new Point(0, m * (i++)), " ", vvod.ver_ts,3);
                }
            }

            ////////////////////////////////
            this.SetDesktopLocation(40, 40);

		//обновление данных, введенных пользователем 
            vvod.Change("const_per", Convert.ToString(((RadioButton)panel_Settings_diff.
                Controls["radioButton_const_per"]).Checked));

            vvod.Change("changing_per", Convert.ToString(((RadioButton)panel_Settings_diff.
                Controls["radioButton_changing_per"]).Checked));

            vvod.Change("prog_uprav", Convert.ToString(((RadioButton)panel_Settings_diff.
                Controls["radioButton_prog_uprav"]).Checked));
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {

        }



       
/*событие изменения выбранного вида резервирования в combobox 
(при каждом изменении вида резервирования и общих настроек происходит динамическое создание полей для ввода данных в соответствии с настройками)*/
        private void comboBox_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enable_Return = true;
            //int i = 0;
            //int m = 40;
            panel_Settings_diff.Controls.Clear();
            i = 0;
            Load_From_File();//обновление общих настроек
		//выбор изображения ССН
            if (comboBox_type.SelectedIndex == 0)
                pictureBox_schema.Image = Properties.Resources.pic_nagr;//System.Drawing.Image.FromFile("pic_nagr.png");
            if (comboBox_type.SelectedIndex == 1)
                pictureBox_schema.Image = Properties.Resources.pic_nenagr;//System.Drawing.Image.FromFile("pic_nenagr.png");
            if (comboBox_type.SelectedIndex == 2 || comboBox_type.SelectedIndex == 3)
                pictureBox_schema.Image = Properties.Resources.pic_mixed;//System.Drawing.Image.FromFile("pic_mixed.png");



		//генерация полей для ввода данных с подписями и размерностями
            panel_Settings_diff.Controls.Add(new Label() { Text = "Количество каналов:", Location = new Point(0, m * (i++)), AutoSize = true });
            Generate_Settings("основных ", "_N", new Point(0, m * (i++)), "шт.",vvod.main);
            Generate_Settings("резервных ", "_K", new Point(0, m * (i++)), "шт.", vvod.res);

            panel_Settings_diff.Controls.Add(new Label() { Text = "Интенсивность отказов:", Location = new Point(0, m * (i++)), AutoSize = true });
            Generate_Settings("в режиме работы ", "_lrab", new Point(0, m * (i++)), "x(10^6)1/ч", vvod.lrab,2,106);
//все виды резервирования, кроме скользящего нагруженного требуют интенсивности отказов в режиме ожидания и, в зависимости от общих настроек, вероятности успешного перехода на резерв
            if (comboBox_type.Text != "Скользящее нагруженное")
            {
                Generate_Settings("в режиме ожидания ", "_loj", new Point(0, m * (i++)), "x(10^8)1/ч",vvod.loj,2,108);/////////////////////////////////////////////////////////

                if (Use_Poss)
                {
                    Generate_Settings("Вероятность ", "_pos", new Point(0, m * (i++)), " ", vvod.Ver,3);
                }

               
            }
		//коэффициент нагрузки
            if (Use_Coef_Nagr && comboBox_type.SelectedIndex != 1)//коэффициент нагрузки не может изменяться при выборе скользящего ненагруженного резервирования
            {
                panel_Settings_diff.Controls.Add(new Label() { Text = "Коэффициенты нагрузки:", Name = "label_Knagr", Location = new Point(0, m * (i++)), AutoSize = true });
                //////////////////////////////////////////////////////////////////////////////
                Generate_Settings("без резервных", "_Knagr_main", new Point(0, m * (i++)), " ", vvod.Knagr_main, 5);// 4);
                Generate_Settings("с резервными", "_Knagr_res", new Point(0, m * (i++)), " ", vvod.Knagr_res,(comboBox_type.SelectedIndex == 2)?5:4, 0, true);//для смешанного можно ввести ТОЛЬКО одно значение, для остальных столько значений, сколько резервных каналов

            }

		//поля для выбора вида ротации
            if (comboBox_type.Text == "Смешанное с ротацией")
            {


                RadioButton radioButton_const_per = new RadioButton() { Text = "Постоянный период", Name = "radioButton_const_per", Location = new Point(0, m * (i++)), AutoSize = true };

                RadioButton radioButton_changing_per = new RadioButton() { Text = "Переменный период", Name = "radioButton_changing_per", Location = new Point(0, m * (i++)), AutoSize = true };

                RadioButton radioButton_prog_uprav = new RadioButton() { Text = "Программа управления", Name = "radioButton_prog_uprav", Location = new Point(0, m * (i++)), AutoSize = true };


                radioButton_const_per.Checked = false;
                radioButton_changing_per.Checked = false;
                radioButton_prog_uprav.Checked = false;


                radioButton_const_per.CheckedChanged += Check_Changed;
                radioButton_changing_per.CheckedChanged += Check_Changed;
                radioButton_prog_uprav.CheckedChanged += Check_Changed;

                

                panel_Settings_diff.Controls.Add(radioButton_changing_per);
                panel_Settings_diff.Controls.Add(radioButton_const_per);
                panel_Settings_diff.Controls.Add(radioButton_prog_uprav);
		//программа управления(Calc) требует задания вероятности успешного перехода на резерв
                if (!Use_Poss)
                {
                    radioButton_prog_uprav.Enabled = false;
                }

                kol_set = panel_Settings_diff.Controls.Count;
                i_without_per = i;

            	//добавление настроек

                radioButton_const_per.Checked = vvod.radiobutt_const;
                radioButton_changing_per.Checked = vvod.radiobutt_change;
                radioButton_prog_uprav.Checked = vvod.radiobutt_uprav;


                vvod.Change("const_per", Convert.ToString(((RadioButton)panel_Settings_diff.
                    Controls["radioButton_const_per"]).Checked));

                vvod.Change("changing_per", Convert.ToString(((RadioButton)panel_Settings_diff.
                    Controls["radioButton_changing_per"]).Checked));

                vvod.Change("prog_uprav", Convert.ToString(((RadioButton)panel_Settings_diff.
                    Controls["radioButton_prog_uprav"]).Checked));

            }
            panel_Settings_diff.Visible = true;
            panel_Settings_diff.AutoScroll = true;
            button_Calculate.Enabled = true;
            this.SetDesktopLocation(40, 40);
        }

//функция генерации полей для ввода следующего вида: название + textbox для ввода + размерность
//для каждого поля настраивается функция проверки корректности введенных данных
//после создания контролы добавляются на панель
        private void Generate_Settings(string type, string sett, Point loc, string podp, string text, int int_ = 1, int pic_ = 0, bool scroll = false)
        {
            panel_Settings_diff.Controls.Add(new Label() { Text = type, AutoSize = true, Name = "label" + sett + "_text", Location = loc });
            int w = panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Size.Width;
            if (w < 220)
                w = 220;
            panel_Settings_diff.Controls.Add(new TextBox() { Text = text, /* Multiline = scroll, ScrollBars = (scroll) ? ScrollBars.Horizontal : ScrollBars.None,*/ScrollBars =  ScrollBars.Horizontal, AutoSize = true, Name = "textBox" + sett, Location = new Point(loc.X + w, loc.Y) });

            if (int_ == 1)
                panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Leave += TextChanged_int;
            if (int_ == 2)
                panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Leave += TextChanged_double;
            if (int_ == 3)
                panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Leave += TextChanged_double_less_than_1;
            if (int_ == 4)
                panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Leave += TextChanged_array_double;
            if (int_ == 5)
                panel_Settings_diff.Controls[panel_Settings_diff.Controls.Count - 1].Leave += TextChanged_double_1;

            switch (pic_)//для подписи размерности интенсивности используется картинка
            {
                case 106:
                    panel_Settings_diff.Controls.Add(new Label() { Text = "            ", Image = Properties.Resources._106, AutoSize = true, Name = "label" + sett + "_podpis", Location = new Point(loc.X + w + 100, loc.Y) });
                    break;
                case 108:
                    panel_Settings_diff.Controls.Add(new Label() { Text = "            ", Image = Properties.Resources._108, AutoSize = true, Name = "label" + sett + "_podpis", Location = new Point(loc.X + w + 100, loc.Y) });
                    break;
                default://в остальных случаях текст
                    panel_Settings_diff.Controls.Add(new Label() { Text = podp, AutoSize = true, Name = "label" + sett + "_podpis", Location = new Point(loc.X + w + 100, loc.Y) });
                    break;
            }

        }

        

//валидация для полей с целыми значениями
        private void TextChanged_int(object sender, EventArgs e)
        {
            Check_TextBoxes();
            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;
               
                if(comboBox_type.Text == "Смешанное с ротацией" && panel_Settings_diff.Controls.Count >= 20)//при изменении числа резервов необходимо изменить количество полей для ввода значений периодов ротации
                {
                    Check_Changed(panel_Settings_diff.Controls.OfType<RadioButton>(), new EventArgs());
                }
                try
                {
                    int s = int.Parse(text);

                    vvod.Change(((TextBox)sender).Name.Remove(0,8),text);
                }
                catch (FormatException e1)
                {

                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;
            
            
        }

//валидация для полей с дробными значениями
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

                    vvod.Change(((TextBox)sender).Name.Remove(0, 8), text);
                }
                catch (FormatException e1)
                {
                    // ((TextBox)sender).BackColor = Color.Red;
                    ((TextBox)sender).BackColor = Color.Red;
                   // ((TextBox)sender).Clear();
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;
        }

//валидация для полей с дробными значениями не более 1
        private void TextChanged_double_1(object sender, EventArgs e)
        {
            Check_TextBoxes();
            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;

                try
                {
                    double s = double.Parse(text);
                    if (s > 1)
                    {
                        ((TextBox)sender).BackColor = Color.Red;
                        // ((TextBox)sender).Clear();
                        MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    vvod.Change(((TextBox)sender).Name.Remove(0, 8), text);
                }
                catch (FormatException e1)
                {
                    // ((TextBox)sender).BackColor = Color.Red;
                    ((TextBox)sender).BackColor = Color.Red;
                    // ((TextBox)sender).Clear();
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
                return;
        }

//валидация для полей с дробными значениями менее 1
        private void TextChanged_double_less_than_1(object sender, EventArgs e)
        {
            Check_TextBoxes();
            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;
                
                try
                {
                    double s = double.Parse(text);
                    if (s >= 1)
                    {
                        ((TextBox)sender).BackColor = Color.Red;
                       // ((TextBox)sender).Clear();
                         MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }vvod.Change(((TextBox)sender).Name.Remove(0, 8), text);
                }
                catch (FormatException e1)
                {
                    // ((TextBox)sender).BackColor = Color.Red;
                    ((TextBox)sender).BackColor = Color.Red;
                   // ((TextBox)sender).Clear();
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            else
                return;
        }
//валидация для полей с несколькими дробными значениями 
        private void TextChanged_array_double(object sender, EventArgs e)
        {

            Check_TextBoxes();

            string text = ((TextBox)sender).Text;
            if (text != "")
            {
                ((TextBox)sender).BackColor = Color.White;
                
                try
                {
                    double[] Koef = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(k => double.Parse(k.Trim())).ToArray();
                    vvod.Change(((TextBox)sender).Name.Remove(0, 8), text);
                }
                catch (FormatException e1)
                {
                    // ((TextBox)sender).BackColor = Color.Red;
                    ((TextBox)sender).BackColor = Color.Red;
                   // ((TextBox)sender).Clear();
                    MessageBox.Show("Некорректные данные", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                return;


        }
	//обновление полей ввода после изменения общих настроек
        public void Return()
        {
            Load_From_File();

            comboBox_type_SelectedIndexChanged(comboBox_type, new EventArgs());

        }



	//сигнал окончания расчета схемы и вывода результатов
        private void textBox_Result_TextChanged(object sender, EventArgs e)
        {
            button_Graph.Enabled = true;
            button_Summary.Enabled = true;
            progressBar_Calc.Visible = false;
        }

	//нажатие кнопки "Рассчитать"
        private void button_Calculate_Click(object sender, EventArgs e)
        {
            Load_From_File();//загрузка настроек
            panel_Result.Visible = true;
            button_Graph.Enabled = false;
            button_Summary.Enabled = false;
            textBox_Result.Text = "";
            ((ToolStripMenuItem)menuStrip1.Items[0]).DropDownItems[2].Enabled = true;


		//визуализация процесса расчета
            progressBar_Calc.Visible = true;
            progressBar_Calc.Maximum = N;

		//получение данных из полей ввода
            int main = Convert.ToInt32(panel_Settings_diff.Controls["textBox_N"].Text);
            int res = Convert.ToInt32(panel_Settings_diff.Controls["textBox_K"].Text);
            double lrab = Convert.ToDouble(panel_Settings_diff.Controls["textBox_lrab"].Text)* (Math.Pow(10, -6));

            double[] Koef;
            if (comboBox_type.Text == "Смешанное" || comboBox_type.Text == "Смешанное с ротацией")
                Koef = new double[main + 1 + res];//добавляется нагруженный резерв
            else
                Koef = new double[main + res];
            if (Use_Coef_Nagr && comboBox_type.Text != "Скользящее ненагруженное")
            {
                for (int k = 0; k < main; k++)
                    Koef[k] = Convert.ToDouble(panel_Settings_diff.Controls["textBox_Knagr_main"].Text);
                double[] Koef_res = panel_Settings_diff.Controls["textBox_Knagr_res"].Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(k => double.Parse(k.Trim())).ToArray();
                int k1 = main;
                int j = 0;
                for (; k1 < Koef.Count() && j < Koef_res.Count(); k1++, j++)
                    Koef[k1] = Koef_res[j];
                if (k1 < Koef.Count())
                    for(; k1 < Koef.Count(); k1++)
                        Koef[k1] = Koef_res.Last();
            }
            else
            {
                for (int i = 0; i < Koef.Count(); i++)
                    Koef[i] = 1;
            }
		//проверка на переполнение памяти (ограничение 2Гб)
            if (Koef.Count()*8*N*Math.Pow(10,-9) > 2)
            {
                N = (int)Math.Floor(2 * Math.Pow(10, 9) / (Koef.Count() * 8));
                changed_exp = true;
            }
		//приведение резерва к одному из типов
            if (comboBox_type.Text == "Скользящее нагруженное")//нагруженное
            {
                Schema = new Hot_Reserve(main, res, N, lrab , Koef, !const_BSV);
            }
            else
            {
                double loj = Convert.ToDouble(panel_Settings_diff.Controls["textBox_loj"].Text) * (Math.Pow(10, -8));
                if (comboBox_type.Text == "Скользящее ненагруженное")//ненагруженное
                {
                    if (!Use_Poss)
                        Schema = new Cold_Reserve(main, res, N, lrab, loj, Koef, !const_BSV);
                    else//с учетом вероятности
                        Schema = new Cold_Reserve_Probability(main, res, N, lrab, loj, Koef, 
                            Convert.ToDouble(panel_Settings_diff.Controls["textBox_pos"].Text), !const_BSV);
                }
                if (comboBox_type.Text == "Смешанное")//смешанное
                {
                    if (!Use_Poss)
                        Schema = new Mixed_Reserve(main, res, N, lrab, loj, Koef, !const_BSV);
                    else//с учетом вероятности
                        Schema = new Mixed_Reserve_Probability(main, res, N, lrab, loj, Koef,
                            Convert.ToDouble(panel_Settings_diff.Controls["textBox_pos"].Text), !const_BSV);
                }
                if (comboBox_type.Text == "Смешанное с ротацией")//с ротацией
                {
                    List<double[]> Periods = new List<double[]>();
                   
                    if (((RadioButton)panel_Settings_diff.Controls["radioButton_prog_uprav"]).Checked)
                    {
				// при выборе программы управления возможен расчет только с вероятностью
                            Schema = new Rotation_V2_Probability_yprav(main, res, N, lrab, loj, Koef, Time_e,
                                Convert.ToDouble(panel_Settings_diff.Controls["textBox_pos"].Text), 
                                Convert.ToDouble(panel_Settings_diff.Controls["textBox_pos_TS"].Text), !const_BSV);
                    }
                    else
                    {
                        bool const_per = true;
                        if (((RadioButton)panel_Settings_diff.Controls["radioButton_changing_per"]).Checked)//переменный период
                        {
                            const_per = false;
                            for (int x = 0; x < res; x++)
                                Periods.Add(panel_Settings_diff.Controls["textBox_per" + Convert.ToString(x)].Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(k => double.Parse(k.Trim())).ToArray());
                        }
                        else//постоянный период
                        {
                            for (int x = 0; x < res; x++)
                                Periods.Add(panel_Settings_diff.Controls["textBox_per"].Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(k => double.Parse(k.Trim())).ToArray());
                        }
                        if (!Use_Poss)
                             Schema = new Rotation_V2(main, res, N, lrab, loj, Koef, Periods, Time_e, const_per, !const_BSV);
                         else//с учетом вероятности
                             Schema = new Rotation_V2_Probability(main, res, N, lrab, loj, Koef, Periods, Time_e,
                                 Convert.ToDouble(panel_Settings_diff.Controls["textBox_pos"].Text), const_per, !const_BSV);

                       

                    }
                }
            }


            progressBar_Calc.Enabled = true;
            progressBar_Calc.Value = 0;
            //расчет схемы
            Schema.CalculateTime(this);
	//массив наработок
            time = Schema.Get_Time();
	//вывод результатов
            textBox_Result.Text = "Средняя наработка: " + Convert.ToString(Schema.Get_Time_Average()) + " ч." + Environment.NewLine;
            textBox_Result.Text += "Вероятность: " + Convert.ToString(Schema.Get_Pos(Time_e).Item1)
                + "+-" + Convert.ToString(Schema.Get_Pos(Time_e).Item2) + Environment.NewLine;
            textBox_Result.Text += "Среднее время простоя: " + Convert.ToString(Schema.Get_Average_Dead_Time(Time_e)) + " ч." + Environment.NewLine;
            textBox_Result.Text += "Коэффициент готовности: " + Convert.ToString(Schema.Get_Koef_Gotovnosti(Time_e))  + Environment.NewLine;
            
        }


	//обновление прогрессбара по результату расчета одного эксперимента (см. Schema.CalculateTime)
        public void Update_Progress(int value)
        {
            progressBar_Calc.Value = value;
        }
	//открытие проекта из файла
        void Open_Existing_Project_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            if (Dialog.ShowDialog() == DialogResult.Cancel)
                return;
            Reserve Schema1 = new Reserve();
            Schema = Schema1.Load(Dialog.FileName);
            textBox_name.Text = Dialog.FileName.Split('\\').Last();
            panel_sett.Visible = true;
            
            button_Calculate.Visible = true;

            Load_From_File();

            Use_Poss = (Convert.ToString(Schema.GetType())).Contains("Probability");


            if (Schema is Hot_Reserve)
            {
                comboBox_type.SelectedItem = "Скользящее нагруженное";
                pictureBox_schema.Image = Properties.Resources.pic_nagr;
            }
            if ((Convert.ToString(Schema.GetType())).Contains("Cold"))
            {
                comboBox_type.SelectedItem = "Скользящее ненагруженное";
                pictureBox_schema.Image = Properties.Resources.pic_nenagr;
            }
            if ((Convert.ToString(Schema.GetType())).Contains("Mixed"))
            {
                comboBox_type.SelectedItem = "Смешанное";
                pictureBox_schema.Image = Properties.Resources.pic_mixed;
            }
            if ((Convert.ToString(Schema.GetType())).Contains("Rotation"))
            {
                comboBox_type.SelectedItem = "Смешанное с ротацией";
                pictureBox_schema.Image = Properties.Resources.pic_mixed;
            }
            comboBox_type.Enabled = false;

            panel_Settings_diff.Controls.Clear();
            i = 0;
		//создание полей для отображения настроек проекта (см. comboBox_type_SelectedIndexChanged - аналог для нового проекта)
            panel_Settings_diff.Controls.Add(new Label() { Text = "Количество каналов:", Location = new Point(0, m * (i++)), AutoSize = true });
            Generate_Settings("основных ", "_N", new Point(0, m * (i++)), "шт.", Convert.ToString(Schema.Get_N()));
            Generate_Settings("резервных ", "_K", new Point(0, m * (i++)), "шт.", Convert.ToString(Schema.Get_K()));

            panel_Settings_diff.Controls.Add(new Label() { Text = "Интенсивность отказов:", Location = new Point(0, m * (i++)), AutoSize = true });
            Generate_Settings("в режиме работы ", "_lrab", new Point(0, m * (i++)), "x(10^6)1/ч", Convert.ToString(Schema.Get_Lambdarab()*Math.Pow(10,6)), 2,106);

            if (!(Schema is Hot_Reserve))
            {
                Generate_Settings("в режиме ожидания ", "_loj", new Point(0, m * (i++)), "x(10^8)1/ч", Convert.ToString(Schema.Get_Lambdaoj() * Math.Pow(10, 8)), 2,108);

                if ((Convert.ToString(Schema.GetType())).Contains("Probability"))
                {
                    Generate_Settings("Вероятность ", "_pos", new Point(0, m * (i++)), " ", Convert.ToString((Schema).Get_Probability()), 3);
                }


            }
            if (Use_Coef_Nagr && !(Convert.ToString(Schema.GetType())).Contains("Cold"))
            {
                panel_Settings_diff.Controls.Add(new Label() { Text = "Коэффициенты нагрузки:", Name = "label_Knagr", Location = new Point(0, m * (i++)), AutoSize = true });
                Generate_Settings("без резервных", "_Knagr_main", new Point(0, m * (i++)), " ", Convert.ToString(Schema.Get_Knagr()[0]), 5);// 4);
                string f = "";
                for (int h = Schema.Get_N(); h < Schema.Get_Knagr().Count(); h++)
                    f += Convert.ToString(Schema.Get_Knagr()[h]) + " ";
                if (!(Convert.ToString(Schema.GetType())).Contains("Mixed"))
                    Generate_Settings("с резервными", "_Knagr_res", new Point(0, m * (i++)), " ", f, 4, 0, true);
                else
                    Generate_Settings("с резервными", "_Knagr_res", new Point(0, m * (i++)), " ", Convert.ToString(Schema.Get_Knagr()[1]), 5, 0, true);

            }


            if ((Convert.ToString(Schema.GetType())).Contains("Rotation"))
            {


                RadioButton radioButton_const_per = new RadioButton() { Text = "Постоянный период", Name = "radioButton_const_per", Location = new Point(0, m * (i++)), AutoSize = true };

                RadioButton radioButton_changing_per = new RadioButton() { Text = "Переменный период", Name = "radioButton_changing_per", Location = new Point(0, m * (i++)), AutoSize = true };

                RadioButton radioButton_prog_uprav = new RadioButton() { Text = "Программа управления", Name = "radioButton_prog_uprav", Location = new Point(0, m * (i++)), AutoSize = true };

                


                panel_Settings_diff.Controls.Add(radioButton_changing_per);
                panel_Settings_diff.Controls.Add(radioButton_const_per);
                panel_Settings_diff.Controls.Add(radioButton_prog_uprav);


                kol_set = panel_Settings_diff.Controls.Count;
                //kol_i = i;
                i_without_per = i;



                if ((Convert.ToString(Schema.GetType())).Contains("yprav"))
                {

                    radioButton_prog_uprav.Checked = true;
                    Generate_Settings("Вероятность по ТЗ", "_pos_TS", new Point(0, m * (i++)), " ", Convert.ToString(((Rotation_V2_Probability_yprav)Schema).TS), 3);
                }
                else
                {
                    if (Schema is Rotation_V2)
                    {
                        if (((Rotation_V2)Schema).const_Per)
                        {
                            radioButton_const_per.Checked = true;
                            Generate_Settings("Цикл полной ротации", "_per", Location = new Point(0, m * (i++)), "ч.", Convert.ToString(((Rotation_V2)Schema).Get_Periods(0)[0]), 2);
                        }
                        else
                        {
                            radioButton_changing_per.Checked = true;

                            panel_Settings_diff.Controls.Add(new Label() { Text = "Величины циклов полной ротации:", Name = "label_per_value", Location = new Point(0, m * (i++)), AutoSize = true });
                            for (int x = 0; x < Convert.ToInt32(Schema.Get_K()); x++)
                            {
                                string f = "";
                                foreach (double a in ((Rotation_V2)Schema).Get_Periods(Schema.Get_K() - x - 1))
                                    f += Convert.ToString(a) + " ";
                                Generate_Settings("Для " + Convert.ToString(x + 1) + "-ого рабочего резерва", "_per" + Convert.ToString(x), Location = new Point(0, m * (i++)), "ч.", f, 4, 0, true);
                            }


                        }
                    }
                    else
                    {
                        if (((Rotation_V2_Probability)Schema).const_Per)
                        {
                            radioButton_const_per.Checked = true;
                            Generate_Settings("Цикл полной ротации", "_per", Location = new Point(0, m * (i++)), "ч.", Convert.ToString(((Rotation_V2_Probability)Schema).Get_Periods(0)[0]), 2);
                        }
                        else
                        {
                            radioButton_changing_per.Checked = true;

                            panel_Settings_diff.Controls.Add(new Label() { Text = "Величины циклов полной ротации:", Name = "label_per_value", Location = new Point(0, m * (i++)), AutoSize = true });
                            for (int x = 0; x < Convert.ToInt32(Schema.Get_K()); x++)
                            {
                                string f = "";
                                foreach (double a in ((Rotation_V2_Probability)Schema).Get_Periods(Schema.Get_K() - x - 1))
                                    f += Convert.ToString(a) + " ";
                                Generate_Settings("Для " + Convert.ToString(x + 1) + "-ого рабочего резерва", "_per" + Convert.ToString(x), Location = new Point(0, m * (i++)), "ч.", f, 4, 0, true);
                            }


                        }
                    }
                }

                ////////////////////////////////////////////////////////////
                radioButton_const_per.CheckedChanged += Check_Changed;
                radioButton_changing_per.CheckedChanged += Check_Changed;
                radioButton_prog_uprav.CheckedChanged += Check_Changed;



                this.SetDesktopLocation(40, 40);

            }
           
            panel_Settings_diff.Visible = true;
            panel_Settings_diff.AutoScroll = true;
            button_Calculate.Enabled = true;

            //на случай, если настройки загруженного проекта отличались
            using (StreamWriter sw = new StreamWriter("private_settings.txt"))
            {
                sw.WriteLine(Schema.Get_Num_of_Exp());
                sw.WriteLine(Time_e);
                sw.WriteLine(Convert.ToString(!Schema.Get_const_Nagr()));
                sw.WriteLine(Convert.ToString(Use_Poss));
                sw.WriteLine(Convert.ToString(Schema.Get_const_BSV()));
            }

        }
	//нажатие кнопки "Сохранить"
        private void button_Save_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            
             saveFileDialog1.RestoreDirectory = true;

             if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
             //сохранение проекта
            Schema.Save(saveFileDialog1.FileName);
            textBox_name.Text = saveFileDialog1.FileName.Split('\\').Last();

        }
	//формирование отчета
        private void button_Summary_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            //отчет
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName, false, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(Schema.Accumilate_Result(Time_e,changed_exp));
            }
           
        }
	//открытие формы "Графики"
        private void button_Graph_Click(object sender, EventArgs e)
        {
            Form_Graph new_Form_Graph = new Form_Graph(time);
            new_Form_Graph.Show();
        }
	//закрытие программы по закрытию формы
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }


	//проверка данных в полях (если одно из полей пусто или введены некорректные данные, то невозможно произвести расчет схемы)
        private void Check_TextBoxes()
        {
            bool enable = true;
            foreach (TextBox t in panel_Settings_diff.Controls.OfType<TextBox>())
                if (t.Text == "" )
                {
                    enable = false;
                  
                }
            if (enable)
            {
                button_Calculate.Enabled = true;
                
            }
            else
            {
                button_Calculate.Enabled = false;
               
            }
        }

       
    }
}
