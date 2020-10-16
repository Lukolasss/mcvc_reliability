namespace try_interface
{
    partial class Main_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.panel_sett = new System.Windows.Forms.Panel();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.panel_Settings_diff = new System.Windows.Forms.Panel();
            this.button_Calculate = new System.Windows.Forms.Button();
            this.comboBox_type = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel_Result = new System.Windows.Forms.Panel();
            this.progressBar_Calc = new System.Windows.Forms.ProgressBar();
            this.textBox_Result = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Summary = new System.Windows.Forms.Button();
            this.button_Graph = new System.Windows.Forms.Button();
            this.pictureBox_schema = new System.Windows.Forms.PictureBox();
            this.panel_sett.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_Result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_schema)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1043, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // panel_sett
            // 
            this.panel_sett.Controls.Add(this.textBox_name);
            this.panel_sett.Controls.Add(this.panel_Settings_diff);
            this.panel_sett.Controls.Add(this.button_Calculate);
            this.panel_sett.Controls.Add(this.comboBox_type);
            this.panel_sett.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_sett.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel_sett.Location = new System.Drawing.Point(0, 25);
            this.panel_sett.Margin = new System.Windows.Forms.Padding(4);
            this.panel_sett.Name = "panel_sett";
            this.panel_sett.Size = new System.Drawing.Size(488, 627);
            this.panel_sett.TabIndex = 2;
            // 
            // textBox_name
            // 
            this.textBox_name.Enabled = false;
            this.textBox_name.Location = new System.Drawing.Point(9, 4);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(475, 31);
            this.textBox_name.TabIndex = 9;
            // 
            // panel_Settings_diff
            // 
            this.panel_Settings_diff.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel_Settings_diff.Location = new System.Drawing.Point(9, 88);
            this.panel_Settings_diff.Name = "panel_Settings_diff";
            this.panel_Settings_diff.Size = new System.Drawing.Size(476, 439);
            this.panel_Settings_diff.TabIndex = 8;
            // 
            // button_Calculate
            // 
            this.button_Calculate.Enabled = false;
            this.button_Calculate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Calculate.Image = global::try_interface.Properties.Resources.calc;
            this.button_Calculate.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_Calculate.Location = new System.Drawing.Point(376, 534);
            this.button_Calculate.Margin = new System.Windows.Forms.Padding(4);
            this.button_Calculate.Name = "button_Calculate";
            this.button_Calculate.Size = new System.Drawing.Size(108, 76);
            this.button_Calculate.TabIndex = 7;
            this.button_Calculate.Text = "Расчет схемы";
            this.button_Calculate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Calculate.UseVisualStyleBackColor = true;
            this.button_Calculate.Click += new System.EventHandler(this.button_Calculate_Click);
            // 
            // comboBox_type
            // 
            this.comboBox_type.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox_type.FormattingEnabled = true;
            this.comboBox_type.Items.AddRange(new object[] {
            "Скользящее нагруженное",
            "Скользящее ненагруженное",
            "Смешанное",
            "Смешанное с ротацией"});
            this.comboBox_type.Location = new System.Drawing.Point(9, 49);
            this.comboBox_type.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_type.Name = "comboBox_type";
            this.comboBox_type.Size = new System.Drawing.Size(409, 32);
            this.comboBox_type.TabIndex = 0;
            this.comboBox_type.Text = "Вид резервирования";
            this.comboBox_type.SelectedIndexChanged += new System.EventHandler(this.comboBox_type_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel_Result);
            this.panel1.Controls.Add(this.pictureBox_schema);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(507, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(536, 627);
            this.panel1.TabIndex = 3;
            // 
            // panel_Result
            // 
            this.panel_Result.Controls.Add(this.progressBar_Calc);
            this.panel_Result.Controls.Add(this.textBox_Result);
            this.panel_Result.Controls.Add(this.label1);
            this.panel_Result.Controls.Add(this.button_Summary);
            this.panel_Result.Controls.Add(this.button_Graph);
            this.panel_Result.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel_Result.Location = new System.Drawing.Point(9, 318);
            this.panel_Result.Margin = new System.Windows.Forms.Padding(4);
            this.panel_Result.Name = "panel_Result";
            this.panel_Result.Size = new System.Drawing.Size(523, 304);
            this.panel_Result.TabIndex = 7;
            // 
            // progressBar_Calc
            // 
            this.progressBar_Calc.Location = new System.Drawing.Point(4, 216);
            this.progressBar_Calc.Name = "progressBar_Calc";
            this.progressBar_Calc.Size = new System.Drawing.Size(349, 23);
            this.progressBar_Calc.TabIndex = 4;
            // 
            // textBox_Result
            // 
            this.textBox_Result.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_Result.Location = new System.Drawing.Point(4, 35);
            this.textBox_Result.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_Result.Multiline = true;
            this.textBox_Result.Name = "textBox_Result";
            this.textBox_Result.ReadOnly = true;
            this.textBox_Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Result.Size = new System.Drawing.Size(510, 173);
            this.textBox_Result.TabIndex = 3;
            this.textBox_Result.TextChanged += new System.EventHandler(this.textBox_Result_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(278, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Результаты моделирования";
            // 
            // button_Summary
            // 
            this.button_Summary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Summary.Image = global::try_interface.Properties.Resources.doc;
            this.button_Summary.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_Summary.Location = new System.Drawing.Point(360, 216);
            this.button_Summary.Margin = new System.Windows.Forms.Padding(4);
            this.button_Summary.Name = "button_Summary";
            this.button_Summary.Size = new System.Drawing.Size(73, 75);
            this.button_Summary.TabIndex = 1;
            this.button_Summary.Text = "Отчет";
            this.button_Summary.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Summary.UseVisualStyleBackColor = true;
            this.button_Summary.Click += new System.EventHandler(this.button_Summary_Click);
            // 
            // button_Graph
            // 
            this.button_Graph.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Graph.Image = global::try_interface.Properties.Resources.graph;
            this.button_Graph.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_Graph.Location = new System.Drawing.Point(441, 216);
            this.button_Graph.Margin = new System.Windows.Forms.Padding(4);
            this.button_Graph.Name = "button_Graph";
            this.button_Graph.Size = new System.Drawing.Size(73, 75);
            this.button_Graph.TabIndex = 0;
            this.button_Graph.Text = "Графики";
            this.button_Graph.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Graph.UseVisualStyleBackColor = true;
            this.button_Graph.Click += new System.EventHandler(this.button_Graph_Click);
            // 
            // pictureBox_schema
            // 
            this.pictureBox_schema.Location = new System.Drawing.Point(9, 88);
            this.pictureBox_schema.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox_schema.Name = "pictureBox_schema";
            this.pictureBox_schema.Size = new System.Drawing.Size(523, 222);
            this.pictureBox_schema.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_schema.TabIndex = 6;
            this.pictureBox_schema.TabStop = false;
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 652);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_sett);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(200, 200);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1059, 691);
            this.MinimumSize = new System.Drawing.Size(1059, 691);
            this.Name = "Main_Form";
            this.Text = "Расчет безотказности ММПН";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.panel_sett.ResumeLayout(false);
            this.panel_sett.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel_Result.ResumeLayout(false);
            this.panel_Result.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_schema)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panel_sett;
        private System.Windows.Forms.Button button_Calculate;
        private System.Windows.Forms.ComboBox comboBox_type;
        private System.Windows.Forms.Panel panel_Settings_diff;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel_Result;
        private System.Windows.Forms.TextBox textBox_Result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Summary;
        private System.Windows.Forms.Button button_Graph;
        private System.Windows.Forms.PictureBox pictureBox_schema;
        private System.Windows.Forms.ProgressBar progressBar_Calc;
        private System.Windows.Forms.TextBox textBox_name;
    }
}