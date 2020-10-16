using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//ФОРМА ПОСТРОЕНИЯ ГРАФИКОВ
namespace try_interface
{
    public partial class Form_Graph : Form
    {
        public Form_Graph(double[] time)
        {
            InitializeComponent();
            graph(chart_Result_pdf, 0, time, pdf(time), Color.Red, "Плотность вероятности", "ч", "1/ч");
            graph(chart_Result_cdf, 0, time, cdf(time), Color.Green, "Функция распределения", "ч", "    ");
            graph(chart_Result_lambda, 0, time, lambda(time), Color.DarkBlue, "Интенсивность отказов", "ч", "1/ч");
        }

        private void Form_Graph_Load(object sender, EventArgs e)
        {

        }
	//плотность вероятности
        private double[] pdf(double[] times)
        {
            int num_otr = 1 + (int)(Math.Floor(3.322 * Math.Log10(times.Count())));
            double time_otr = times.Max() / num_otr;
            double[] freq_points = new double[num_otr];
            for (int i = 0; i < num_otr; i++)
            {
                for (int j = 0; j < times.Count(); j++)
                {
                    if (times[j] < (i + 1) * time_otr && times[j] >= i * time_otr)
                    {
                        freq_points[i]++;
                    }
                }
                freq_points[i] = freq_points[i] / time_otr;
            }
            return freq_points;
        }
	//функция распределения
        private double[] cdf(double[] times)
        {
            int num_otr = 1 + (int)(Math.Floor(3.322 * Math.Log10(times.Count())));
            double time_otr = times.Max() / num_otr;
            double[] freq_points = new double[num_otr];
            for (int i = 0; i < num_otr; i++)
            {
                for (int j = 0; j < times.Count(); j++)
                {
                    if (times[j] < (i + 1) * time_otr)
                    {
                        freq_points[i]++;
                    }
                }
                freq_points[i] = freq_points[i] / times.Count();
            }
            return freq_points;
        }

	//интенсивность отказов
        private double[] lambda(double[] times)
        {
            int num_otr = 1 + (int)(Math.Floor(3.322 * Math.Log10(times.Count())));
            double time_otr = times.Max() / num_otr;
            double[] freq_points = new double[num_otr];
            double[] fmal = pdf(times);
            double[] fbig = cdf(times);
            for (int i = 0; i < num_otr; i++)
            {
                freq_points[i] = (fmal[i] / (1 - fbig[i]));
            }
            return freq_points;
        }

	//настройки графиков 
        private void graph(System.Windows.Forms.DataVisualization.Charting.Chart chart, int spl, double[] times, double[] points, Color c, string Title, string xt, string yt)
        {
            int num_otr = 1 + (int)(Math.Floor(3.322 * Math.Log10(times.Count())));//правило Стерджеса
            double time_otr = times.Max() / num_otr;
            chart.Series[spl].Points.Clear();
            chart.Series[spl].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart.Series[spl].Color = c;
            chart.Series[spl].BorderWidth = 4;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.Series[spl].Points.AddXY(0, 0);
            chart.Titles.Clear();
            chart.Titles.Add(Title);
            chart.Titles[0].Font = new Font("Arial", 14, FontStyle.Bold);
            chart.Titles[0].Alignment = ContentAlignment.TopCenter;
            chart.ChartAreas[0].AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chart.ChartAreas[0].AxisX.Title = xt;
            chart.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 11, FontStyle.Bold);
            chart.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Far;
            chart.ChartAreas[0].AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 8);
            chart.ChartAreas[0].AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chart.ChartAreas[0].AxisY.Title = yt;
            chart.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 11, FontStyle.Bold);
            chart.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Far;
            chart.ChartAreas[0].AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 8);
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorX.AutoScroll = true;
            chart.ChartAreas[0].CursorY.AutoScroll = true;
            for (int i = 0; i < num_otr; i++)
            {
                chart.Series[spl].Points.AddXY((i + 1) * time_otr / 2, points[i]);//ставим точки на середины отрезков из правила Стерджеса
            }
        }
    }
}
