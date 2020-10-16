using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//СМЕШАННОЕ РЕЗЕРВИРОВАНИЕ С УЧЕТОМ ВЕРОЯТНОСТИ ПЕРЕКЛЮЧЕНИЯ
namespace try_interface.Reserves
{
    [Serializable]
    class Mixed_Reserve_Probability : Reserve
    {
        private bool lambdaO0;
        private double POSOFSWITCH;//величина вероятности переключения


        private double[] first_fail;//массив первых отказов

        public Mixed_Reserve_Probability(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr, double Probability, bool BSV = false) : base(n, k, Exp, lambdaR, lambdaO, Nagr, BSV)
        {
            

            lambdaO0 = (lOj == 0) ? true : false;
            POSOFSWITCH = Probability;

            first_fail = new double[numOfExp];
            Generate_Operating_Times(true);//генерация наработок

        }
	//вероятность переключения
        public override double Get_Probability()
        {
            return POSOFSWITCH;
        }
        //возврат минимального из первых отказов
        public double Get_Min_First_Fail()
        {
            return first_fail.Min();
        }
	//возврат среднего из первых отказов
        public double Get_Average_First_Fail()
        {
            return first_fail.Average();
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

                    for (int x = N + 1; x < N + 1 + K; x++)
                    {
                        CurrentExperiment[x] *= lRab / lOj;
                    }

/*пока есть резервные в ненагруженном резерве - схема рассчитывается как ненагруженное резервирование*/
                    for (; CurrentExperiment.Count > N+1;)
                    {
                        int min = CurrentExperiment.IndexOf(CurrentExperiment.Min());
                        if (CurrentExperiment.Count == N+ 1+ K)//заполнение массива первых отказов
                            first_fail[i] = CurrentExperiment[min];
                        //отказ элемента в режиме ожидания
                        if (min >= N +1)
                        {
                            CurrentExperiment.RemoveAt(min);
                            //пересчет нагрузки
                            CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
                        }
                        //отказ элемента в режиме работы
                        else
                        {
                            Random rndposs = RandBSV ? new Random() : new Random(CurrentExperiment.Count());
/*если сгенерированная величина больше заданной вероятности переключения, то происходит отказ при переключении, следовательно необходимо удалить отказавший и перейти к следующему резерву*/
                            for (int j = 0; j < CurrentExperiment.Count() - N - 1  && rndposs.NextDouble() > POSOFSWITCH; j++)
                            {
                                CurrentExperiment.RemoveAt(N+1);
                                CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
                            }
                            
                            if (CurrentExperiment.Count > N + 1)//еще остались резервы
                            {
                                CurrentExperiment[min] += (CurrentExperiment[N + 1] - CurrentExperiment[min]) * lOj / lRab;
                                CurrentExperiment.RemoveAt(N + 1);

                                CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
                            }
                        }

                    }
                    //когда остается N+1 элементов, схема переходит скользящее нагруженное
                    //расчет нагруженного
                    CurrentExperiment.Sort();
                    double delta = CurrentExperiment[0];
                    for (int g = 0; g < CurrentExperiment.Count; g++)
                        CurrentExperiment[g] -= delta;
                    CurrentExperiment.RemoveAt(0);
                    Time[i] += delta + CoefficientNagruzki(CurrentExperiment)[0];


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
/*пока есть резервные в ненагруженном резерве - схема рассчитывается как ненагруженное резервирование*/
                    for (; CurrentExperiment.Count > N+1;)
                    {
                        int min = CurrentExperiment.IndexOf(CurrentExperiment.GetRange(0, N+1).Min());
                        if (CurrentExperiment.Count == N+1+K)//первые отказы
                            first_fail[i] = CurrentExperiment[min];
                        
                        Random rndposs = RandBSV ? new Random() : new Random(CurrentExperiment.Count());
/*если сгенерированная величина больше заданной вероятности переключения, то происходит отказ при переключении, следовательно необходимо удалить отказавший и перейти к следующему резерву*/
                        for (int j = 0; j < CurrentExperiment.Count() - N -1 && rndposs.NextDouble() > POSOFSWITCH; j++)
                        {
                            
                            CurrentExperiment.RemoveAt(N+1);
                            CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
                        }
                        
                        if (CurrentExperiment.Count > N + 1)//еще остались резервы
                        {
                            CurrentExperiment[min] += CurrentExperiment[N + 1];
                            CurrentExperiment.RemoveAt(N + 1);
                            CurrentExperiment = CoefficientNagruzki(CurrentExperiment);
                        }

                    }
		
//когда остается N+1 элементов, схема переходит скользящее нагруженное
                    //расчет нагруженного

                    CurrentExperiment.Sort();
                    double delta = CurrentExperiment[0];
                    for (int g = 0; g < CurrentExperiment.Count; g++)
                        CurrentExperiment[g] -= delta;
                    CurrentExperiment.RemoveAt(0);
                    Time[i] += delta + CoefficientNagruzki(CurrentExperiment)[0];

                    form1.Update_Progress(i);

                }
            }

        }
//загрузка из файла
        public Mixed_Reserve_Probability Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Mixed_Reserve_Probability res = (Mixed_Reserve_Probability)bin.Deserialize(fs);
            fs.Close();
            return res;
        }
//вывод для отчета
        public override string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Вид резервирования: смешанное резервирование\n";


            Result += "Количество основных каналов:";
            Result += Convert.ToString(this.N) + " шт.\n";
            Result += "Количество резервных каналов: (";
            Result += Convert.ToString(this.K) + " + 1) шт.\n";

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
