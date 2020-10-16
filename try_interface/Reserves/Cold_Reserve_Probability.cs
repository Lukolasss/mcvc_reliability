using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//СКОЛЬЗЯЩЕЕ НЕНАГРУЖЕННОЕ РЕЗЕРВИРОВАНИЕ С УЧЕТОМ ВЕРОЯТНОСТИ ПЕРЕКЛЮЧЕНИЯ
namespace try_interface.Reserves
{
    [Serializable]
    class Cold_Reserve_Probability : Reserve
    {
        private bool lambdaO0;
        private double POSOFSWITCH;//значение вероятности переключения


        public Cold_Reserve_Probability(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr, double Probability, bool BSV = false) : base(n, k, Exp, lambdaR, lambdaO, Nagr, BSV)
        {
            
            lambdaO0 = (lOj == 0) ? true : false;
            POSOFSWITCH = Probability;

            Generate_Operating_Times(false);//генерация наработок

        }
	//вероятность переключения
        public override double Get_Probability()
        {
            return POSOFSWITCH;
        }

	//расчет схемы
        public override void CalculateTime(Main_Form form1)
        {
            if (!lambdaO0)//с учетом лямбды ожидания
            {
                for (int i = 0; i < numOfExp; i++)
                {

                    //лист значений в текущем эксперименте
                    CurrentExperiment = Enumerable.Range(0, Experiments.GetLength(0))
                                        .Select(x => Experiments[x, i])
                                        .ToList();

                    for (int x = N; x < N + K; x++)
                    {
                        CurrentExperiment[x] *= lRab / lOj;
                    }


                    for (; CurrentExperiment.Count > N;)
                    {
                        int min = CurrentExperiment.IndexOf(CurrentExperiment.Min());

                        //отказ элемента в режиме ожидания
                        if (min >= N)
                        {
                            CurrentExperiment.RemoveAt(min);
                            
                        }
                        //отказ элемента в режиме работы
                        else
                        {
                            Random rndposs = RandBSV ? new Random() : new Random(CurrentExperiment.Count());
				/*если сгенерированная величина больше заданной вероятности переключения, то происходит отказ при переключении, следовательно необходимо удалить отказавший и перейти к следующему резерву*/
                            for (int j = 0; j < CurrentExperiment.Count() - N && rndposs.NextDouble() > POSOFSWITCH; j++)
                            {
                                CurrentExperiment.RemoveAt(N);
                                
                            }
                            
                            if (CurrentExperiment.Count > N)//еще остались резервы
                            {
                                CurrentExperiment[min] += (CurrentExperiment[N] - CurrentExperiment[min]) * lOj / lRab;
                                CurrentExperiment.RemoveAt(N);

                                
                            }
                            

                        }

                    }
                    Time[i] += CurrentExperiment.Min();
                    form1.Update_Progress(i);
                }
            }
            else//без учета лямбды ожидания
            {
                for (int i = 0; i < numOfExp; i++)
                {

                    //лист значений в текущем эксперименте
                    CurrentExperiment = Enumerable.Range(0, Experiments.GetLength(0))
                                        .Select(x => Experiments[x, i])
                                        .ToList();

                    for (; CurrentExperiment.Count > N;)
                    {
                        
                        int min = (N > 1) ? CurrentExperiment.IndexOf(CurrentExperiment.GetRange(0, N ).Min()) : 0;


                        Random rndposs = RandBSV ? new Random() : new Random(CurrentExperiment.Count());
                        for (int j = 0; j < CurrentExperiment.Count() - N && rndposs.NextDouble() > POSOFSWITCH; j++)
                        {
                            CurrentExperiment.RemoveAt(N);
                        }
                        if (CurrentExperiment.Count > N)
                        {
                            CurrentExperiment[min] += CurrentExperiment[N];
                            CurrentExperiment.RemoveAt(N);
                        }

                    }

                    Time[i] += CurrentExperiment.Min();
                    form1.Update_Progress(i);

                }
            }

        }
	//загрузка из файла
        public Cold_Reserve_Probability Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Cold_Reserve_Probability res = (Cold_Reserve_Probability)bin.Deserialize(fs);
            fs.Close();
            return res;
        }

        public override string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Вид резервирования: скользящее ненагруженное резервирование\n";


            Result += "Количество основных каналов:";
            Result += Convert.ToString(this.N) + " шт.\n";
            Result += "Количество резервных каналов:";
            Result += Convert.ToString(this.K) + " шт.\n";

            Result += "Интенсивность отказов в режиме работы:";
            Result += Convert.ToString(this.lRab) + " 1/ч\n";
            Result += "Интенсивность отказов в режиме ожидания:";
            Result += Convert.ToString(this.lOj) + " 1/ч\n";




            Result += "Вероятность успешного переключения:";
            Result += Convert.ToString(this.POSOFSWITCH) + "\n";

            Result += "Количество экспериментов:";
            Result += Convert.ToString(this.numOfExp) + "\n";
            if (changed_num)
                Result += "(число экспериментов было уменьшено в связи с нехваткой памяти) \n";
            Result += "Время работы:";
            Result += Convert.ToString(needed_time) + "ч.\n";

            /*  if (this.CoefNagr.Count() != 0)
              {
                  Result += "Коэффициенты нагрузки:";
                  for (int i = 0; i < this.CoefNagr.Count(); i++)
                  {
                      Result += Convert.ToString(this.CoefNagr[i]) + " ";
                  }
                  Result += "\n";
              }*/
            Result += "Результаты:\n";
            Result += "Средняя наработка:" + Convert.ToString(Get_Time_Average()) + "ч.\n";
            Result += "Вероятность безотказной работы в течение " + Convert.ToString(needed_time) + " ч.: " + Convert.ToString(Get_Pos(needed_time).Item1) + "+-" + Convert.ToString(Get_Pos(needed_time).Item2) + "\n";
            Result += "Среднее время простоя:" + Convert.ToString(Get_Average_Dead_Time(needed_time)) + "ч.\n";
            Result += "Коэффициент готовности:" + Convert.ToString(Get_Koef_Gotovnosti(needed_time)) + "\n";
            return Result;
        }



    }
}

