namespace try_interface
{
    partial class About_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About_Form));
            this.richTextBox_About = new System.Windows.Forms.RichTextBox();
            this.radioButton_res = new System.Windows.Forms.RadioButton();
            this.radioButton_prog = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // richTextBox_About
            // 
            this.richTextBox_About.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_About.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox_About.Location = new System.Drawing.Point(193, 12);
            this.richTextBox_About.Name = "richTextBox_About";
            this.richTextBox_About.Size = new System.Drawing.Size(618, 426);
            this.richTextBox_About.TabIndex = 0;
            this.richTextBox_About.Text = "";
            // 
            // radioButton_res
            // 
            this.radioButton_res.AutoSize = true;
            this.radioButton_res.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButton_res.Location = new System.Drawing.Point(12, 12);
            this.radioButton_res.Name = "radioButton_res";
            this.radioButton_res.Size = new System.Drawing.Size(175, 26);
            this.radioButton_res.TabIndex = 1;
            this.radioButton_res.TabStop = true;
            this.radioButton_res.Text = "Резервирование";
            this.radioButton_res.UseVisualStyleBackColor = true;
            this.radioButton_res.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButton_prog
            // 
            this.radioButton_prog.AutoSize = true;
            this.radioButton_prog.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButton_prog.Location = new System.Drawing.Point(12, 35);
            this.radioButton_prog.Name = "radioButton_prog";
            this.radioButton_prog.Size = new System.Drawing.Size(127, 26);
            this.radioButton_prog.TabIndex = 2;
            this.radioButton_prog.TabStop = true;
            this.radioButton_prog.Text = "Программа";
            this.radioButton_prog.UseVisualStyleBackColor = true;
            // 
            // About_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 450);
            this.Controls.Add(this.radioButton_prog);
            this.Controls.Add(this.radioButton_res);
            this.Controls.Add(this.richTextBox_About);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About_Form";
            this.Text = "Справка";
            this.Load += new System.EventHandler(this.About_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_About;
        private System.Windows.Forms.RadioButton radioButton_res;
        private System.Windows.Forms.RadioButton radioButton_prog;
    }
}