using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace try_interface.Reserves
{
    [Serializable]
    class Hot_Reserve : Reserve
    {
        public Hot_Reserve(int n, int k, int Exp, double lambdaR, double[] Nagr, bool BSV = false) : base(n, k, Exp, lambdaR, 0, Nagr, BSV)
        {
          /*  N = n;
            K = k;
            numOfExp = Exp;
            lRab = lambdaR;
            CoefNagr = Nagr;
            RandBSV = BSV;

            Time = new double[numOfExp];*/

            Generate_Operating_Times(false);

        }


        public override void CalculateTime(Main_Form form1)
        {
            for (int i = 0; i < numOfExp; i++)
            {

                //лист значений в текущем эксперименте
                CurrentExperiment = Enumerable.Range(0, Experiments.GetLength(0))
                                    .Select(x => Experiments[x, i])
                                    .ToList();


                CurrentExperiment.Sort();

               for (int j = 0; j < K + 1; j++)////////////////////////////////////////////////////////
               {
                    Time[i] += CurrentExperiment[0];
                    for (int g = 1; g < CurrentExperiment.Count(); g++)
                        CurrentExperiment[g] -= CurrentExperiment[0];

                    CurrentExperiment.RemoveAt(0);
                    CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
               }

                //  progress = i;
                form1.Update_Progress(i);
                
            }

        }
        public Hot_Reserve Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Hot_Reserve res = (Hot_Reserve)bin.Deserialize(fs);
            fs.Close();
            return res;
        }

        public override string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Вид резервирования: скользящее нагруженное резервирование\n";
            
            Result += "Количество основных каналов:";
            Result += Convert.ToString(this.N) + " шт.\n";
            Result += "Количество резервных каналов:";
            Result += Convert.ToString(this.K) + " шт.\n";
            Result += "Интенсивность отказов в режиме работы:";
            Result += Convert.ToString(this.lRab) + " 1/ч\n";




            Result += "Количество экспериментов:";
            Result += Convert.ToString(this.numOfExp) + "\n";
            if (changed_num)
                Result += "(число экспериментов было уменьшено в связи с нехваткой памяти) \n";

            Result += "Время работы:";
            Result += Convert.ToString(needed_time) + "ч.\n";

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
            Result += "Вероятность безотказной работы в течение " + Convert.ToString(needed_time) + " ч.: " + Convert.ToString(Get_Pos(needed_time).Item1) + "+-" + Convert.ToString(Get_Pos(needed_time).Item2) + "\n";
            Result += "Среднее время простоя:" + Convert.ToString(Get_Average_Dead_Time(needed_time)) + "ч.\n";
            Result += "Коэффициент готовности:" + Convert.ToString(Get_Koef_Gotovnosti(needed_time)) + "\n";
            return Result;
        }

    }
}
