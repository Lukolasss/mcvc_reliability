using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//класс, от которого наследуется все виды резервирования (по просьбе преподавателя каждый из видов резервирования выделить в отдельный класс, также выделить в отдельные классы схемы с учетом вероятности переключений)
namespace try_interface.Reserves
{
    [Serializable]
    class Reserve
    {
        protected double[] CoefNagr;/*устройство массива: длина - количество элементов
                                                            i-ый элемент соответствует нагрузке при работе i+1 элемента*/
        protected double lRab, lOj;//интенсивности отказов

        protected double[,] Experiments;//наработки каналов в каждом из экспериментов

        protected int N;//основные каналы
	protected int K;//резервные каналы
	protected int numOfExp;//количество экспериментов

        protected double[] Time;//массив наработок

        protected bool RandBSV;//использование постоянной/генерация новой Базовой Случайной Величины

        protected List<double> CurrentExperiment;//текущий эксперимент

        protected string Stat;
        protected string Result;

        protected int progress;

        public Reserve()
        {

        }
         public string Get_Stat()
        {
            return Stat;
        }

        public int Get_Progress()
        {
            return progress;
        }

        public Reserve(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr, bool BSV = false)
        {
            N = n;
            K = k;
            numOfExp = Exp;
            lRab = lambdaR;
            lOj = lambdaO;
            CoefNagr = Nagr;
            RandBSV = BSV;

            Time = new double[numOfExp];
            progress = 0;

        }

//генерация наработок
        public void Generate_Operating_Times(bool fl_1 = true)
        {
            Random nomer = new Random();
            if (fl_1)
            {
                Experiments = new double[N + 1 + K, numOfExp];

                for (int i = 0; i < N + 1 + K; i++)
                {
                    Random rnd;
                    if (RandBSV)//генерация базовой случайной величины
                        rnd = new Random(nomer.Next());
                    else
                        rnd = new Random(13 + i);//постоянная БСВ
                    for (int j = 0; j < numOfExp; j++)
                    {
                        Experiments[i, j] = -Math.Pow(lRab, -1) * Math.Log(rnd.NextDouble() * 1 + 0);
                        //нагрузка при работе всех
                        Experiments[i, j] *= 1 / CoefNagr[N + 1 + K - 1];
                    }

                }
            }
            else
            {
                Experiments = new double[N + K, numOfExp];

                for (int i = 0; i < N + K; i++)
                {
                    Random rnd;
                    if (RandBSV)
                        rnd = new Random(nomer.Next());
                    else
                        rnd = new Random(13 + i);
                    for (int j = 0; j < numOfExp; j++)
                    {
                        Experiments[i, j] = -Math.Pow(lRab, -1) * Math.Log(rnd.NextDouble() * 1 + 0);
                        //нагрузка при работе всех
                        Experiments[i, j] *= 1 / CoefNagr[N + K - 1];
                    }

                }
            }
        }

        public void AccumilateStatistics(string s)
        {
            Stat += s + '\n';
        }

        //пересчет нагрузки
        public List<double> CoefficientNagruzki(List<double> list)
        {
            int n = list.Count();// -1;
            for (int i = 0; i < n; i++)
                list[i] *= CoefNagr[n] / CoefNagr[n - 1];/////////
            return list;
        }
	//
	    public virtual double Get_Probability()
        {
            return 1;
        }

        //пересчет нагрузки с отдельным массивом коэффициентов
        public List<double> CoefficientNagruzki(List<double> list, double[] c)
        {
            int n = list.Count();
            for (int i = 0; i < n; i++)
                list[i] *= c[n] / c[n - 1];
            return list;
        }
	//среднее время простоя
        public double Get_Average_Dead_Time(double Time_e)
        {
            double sum = 0;
           // int kol = 0;
            foreach(double t in Time)
            {
                if (t < Time_e)
                {
                    sum += Time_e - t;
                   // kol++;
                }
            }
            return  sum /  numOfExp;
        }
	//расчет схемы
        public virtual void CalculateTime()
        {

        }
	//расчет схемы с возможностью отображения прогресса
        public virtual void CalculateTime(Main_Form form)
        {

        }
//расчет коэффициента готовности 
        public virtual double Get_Koef_Gotovnosti(double Time_e)
        {
            return Time_e /(Get_Average_Dead_Time(Time_e)+Time_e);
        }

	//СКО для доверительного интервала
        public double sko(double[] arr)
        {
            double m = arr.Average();
            double sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum = sum + Math.Pow((arr[i] - m), 2);
            }
            return Math.Sqrt(sum * Math.Pow(arr.Length, -1));
        }

	//доверительный интервал для вероятности
        public double safeintervalP(double t, double coeff)
        {
            double[] Poss = new double[Time.Length];
            for (int i = 0; i < Time.Length; i++)
            {
                if (t < Time[i])
                    Poss[i] = 1;
                else
                    Poss[i] = 0;
            }
            double s = sko(Poss);
            return coeff * s * Math.Pow(Math.Sqrt(Poss.Length), -1);
        }
	//вероятность безотказной работы в течение заданного времени
        public Tuple<double, double> Get_Pos(double PosTime)//Item1 - вероятность Item2 - доверительный интервал
        {
            double tgood = 0;
            for (int i = 0; i < numOfExp; i++)
            {
                if (Time[i] > PosTime)
                    tgood++;

            }
            double Epsilon = safeintervalP(PosTime, 3.05);
            return Tuple.Create((tgood / numOfExp), Epsilon);
        }
	//среднея наработка
        public double Get_Time_Average()
        {
            return Time.Average();
        }
	//количество основных элементов
        public int Get_N()
        {
            return N;
        }
        //рандомная БСВ
        public bool Get_const_BSV()
        {
            return !RandBSV;
        }
        //постоянный коэффициент нагрузки
        public bool Get_const_Nagr()
        {
            foreach (double k in CoefNagr)
                if (k < 1)
                    return false;
            return true;
        }
        //количество экспериментов
        public int Get_Num_of_Exp()
        {
            return numOfExp;
        }
	//количество резервов
        public int Get_K()
        {
            return K;
        }
	//интенсивность отказов в рабочем режиме
        public double Get_Lambdarab()
        {
            return lRab;
        }
	//интенсивность отказов в режиме ожидания
        public double Get_Lambdaoj()
        {
            return lOj;
        }
	//коэффициенты нагрузки
        public double[] Get_Knagr()
        {
            return CoefNagr;
        }
	//массив наработок
        public double[] Get_Time()
        {
            return Time;
        }


	//сохранение в файл
        public void Save(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(fs, this);
            fs.Close();
        }
	//загрузка из файла
        public Reserve Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Reserve res = (Reserve)bin.Deserialize(fs);
            fs.Close();
            return res;
        }
	//вывод в отчет
        public virtual string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Количество основных каналов:";
            Result += Convert.ToString(this.N) + " шт.\n";
            Result += "Количество резервных каналов:";
            Result += Convert.ToString(this.K) + " шт.\n";

            Result += "Интенсивность отказов в режиме работы:";
            Result += Convert.ToString(this.lRab) + " 1/ч\n";
            Result += "Интенсивность отказов в режиме ожидания:";
            Result += Convert.ToString(this.lOj) + " 1/ч\n";

            if (this.CoefNagr.Count() != 0)
            {
                Result += "Коэффициенты нагрузки:";
                for (int i = 0; i < this.CoefNagr.Count(); i++)
                {
                    Result += Convert.ToString(this.CoefNagr[i]) + " ";
                }
                Result += "\n";
            }
            Result += "Результаты:\n";
            Result += "Средняя наработка:" + Convert.ToString(Get_Time_Average()) + "ч.\n";
            Result += "Вероятность безотказной работы в течение " + Convert.ToString(needed_time) + " ч.: "+ Convert.ToString(Get_Pos(needed_time).Item1) + "+-" + Convert.ToString(Get_Pos(needed_time).Item2) + "\n";
            Result += "Среднее время простоя:" + Convert.ToString(Get_Average_Dead_Time(needed_time)) + "ч.\n";
            return Result;
        }
    }
}