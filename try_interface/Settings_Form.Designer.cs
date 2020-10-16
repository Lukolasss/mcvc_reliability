namespace try_interface
{
    partial class Settings_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings_Form));
            this.button_Apply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_N = new System.Windows.Forms.TextBox();
            this.textBox_time_e = new System.Windows.Forms.TextBox();
            this.checkBox_Knagr = new System.Windows.Forms.CheckBox();
            this.checkBox_Poss = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_BSV = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button_Apply
            // 
            this.button_Apply.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_Apply.Location = new System.Drawing.Point(114, 289);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(244, 56);
            this.button_Apply.TabIndex = 0;
            this.button_Apply.Text = "Обновить настройки";
            this.button_Apply.UseVisualStyleBackColor = true;
            this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(25, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Количество экспериментов";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // textBox_N
            // 
            this.textBox_N.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_N.Location = new System.Drawing.Point(186, 74);
            this.textBox_N.Name = "textBox_N";
            this.textBox_N.Size = new System.Drawing.Size(172, 32);
            this.textBox_N.TabIndex = 2;
            // 
            // textBox_time_e
            // 
            this.textBox_time_e.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_time_e.Location = new System.Drawing.Point(186, 114);
            this.textBox_time_e.Name = "textBox_time_e";
            this.textBox_time_e.Size = new System.Drawing.Size(172, 32);
            this.textBox_time_e.TabIndex = 6;
            // 
            // checkBox_Knagr
            // 
            this.checkBox_Knagr.AutoSize = true;
            this.checkBox_Knagr.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_Knagr.Location = new System.Drawing.Point(30, 168);
            this.checkBox_Knagr.Name = "checkBox_Knagr";
            this.checkBox_Knagr.Size = new System.Drawing.Size(314, 28);
            this.checkBox_Knagr.TabIndex = 7;
            this.checkBox_Knagr.Text = "Учет коэффициента нагрузки";
            this.checkBox_Knagr.UseVisualStyleBackColor = true;
            // 
            // checkBox_Poss
            // 
            this.checkBox_Poss.AutoSize = true;
            this.checkBox_Poss.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_Poss.Location = new System.Drawing.Point(30, 206);
            this.checkBox_Poss.Name = "checkBox_Poss";
            this.checkBox_Poss.Size = new System.Drawing.Size(345, 28);
            this.checkBox_Poss.TabIndex = 8;
            this.checkBox_Poss.Text = "Учет вероятности переключения";
            this.checkBox_Poss.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(25, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 24);
            this.label2.TabIndex = 5;
            this.label2.Text = "Время работы";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(364, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 24);
            this.label3.TabIndex = 9;
            this.label3.Text = "ч";
            // 
            // checkBox_BSV
            // 
            this.checkBox_BSV.AutoSize = true;
            this.checkBox_BSV.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_BSV.Location = new System.Drawing.Point(28, 243);
            this.checkBox_BSV.Name = "checkBox_BSV";
            this.checkBox_BSV.Size = new System.Drawing.Size(193, 28);
            this.checkBox_BSV.TabIndex = 10;
            this.checkBox_BSV.Text = "Постоянная БСВ";
            this.checkBox_BSV.UseVisualStyleBackColor = true;
            // 
            // Settings_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 358);
            this.Controls.Add(this.checkBox_BSV);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox_Poss);
            this.Controls.Add(this.checkBox_Knagr);
            this.Controls.Add(this.textBox_time_e);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_N);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Apply);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(426, 397);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(426, 397);
            this.Name = "Settings_Form";
            this.Text = "Предварительные настройки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_Form_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Apply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_N;
        private System.Windows.Forms.TextBox textBox_time_e;
        private System.Windows.Forms.CheckBox checkBox_Knagr;
        private System.Windows.Forms.CheckBox checkBox_Poss;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_BSV;
    }
}