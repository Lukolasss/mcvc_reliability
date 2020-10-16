namespace try_interface
{
    partial class Form_Graph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Graph));
            this.chart_Result_pdf = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart_Result_cdf = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart_Result_lambda = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_pdf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_cdf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_lambda)).BeginInit();
            this.SuspendLayout();
            // 
            // chart_Result_pdf
            // 
            this.chart_Result_pdf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Result_pdf.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.Name = "ChartArea1";
            this.chart_Result_pdf.ChartAreas.Add(chartArea1);
            this.chart_Result_pdf.Location = new System.Drawing.Point(13, 14);
            this.chart_Result_pdf.Name = "chart_Result_pdf";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chart_Result_pdf.Series.Add(series1);
            this.chart_Result_pdf.Size = new System.Drawing.Size(577, 272);
            this.chart_Result_pdf.TabIndex = 0;
            this.chart_Result_pdf.Text = "chart1";
            // 
            // chart_Result_cdf
            // 
            this.chart_Result_cdf.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Result_cdf.BackColor = System.Drawing.SystemColors.Control;
            chartArea2.Name = "ChartArea1";
            this.chart_Result_cdf.ChartAreas.Add(chartArea2);
            this.chart_Result_cdf.Location = new System.Drawing.Point(12, 293);
            this.chart_Result_cdf.Name = "chart_Result_cdf";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.chart_Result_cdf.Series.Add(series2);
            this.chart_Result_cdf.Size = new System.Drawing.Size(577, 272);
            this.chart_Result_cdf.TabIndex = 1;
            this.chart_Result_cdf.Text = "chart1";
            // 
            // chart_Result_lambda
            // 
            this.chart_Result_lambda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart_Result_lambda.BackColor = System.Drawing.SystemColors.Control;
            chartArea3.Name = "ChartArea1";
            this.chart_Result_lambda.ChartAreas.Add(chartArea3);
            this.chart_Result_lambda.Location = new System.Drawing.Point(12, 572);
            this.chart_Result_lambda.Name = "chart_Result_lambda";
            series3.ChartArea = "ChartArea1";
            series3.Name = "Series1";
            this.chart_Result_lambda.Series.Add(series3);
            this.chart_Result_lambda.Size = new System.Drawing.Size(577, 235);
            this.chart_Result_lambda.TabIndex = 2;
            this.chart_Result_lambda.Text = "chart1";
            // 
            // Form_Graph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(654, 749);
            this.Controls.Add(this.chart_Result_lambda);
            this.Controls.Add(this.chart_Result_cdf);
            this.Controls.Add(this.chart_Result_pdf);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Graph";
            this.Text = "Графики";
            this.Load += new System.EventHandler(this.Form_Graph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_pdf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_cdf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Result_lambda)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Result_pdf;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Result_cdf;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Result_lambda;
    }
}