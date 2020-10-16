using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//СКОЛЬЗЯЩЕЕ НЕНАГРУЖЕННОЕ РЕЗЕРВИРОВАНИЕ
namespace try_interface.Reserves
{
    [Serializable]
    class Cold_Reserve : Reserve//наследуется от базового резерва
    {
        private bool lambdaO0;//проверка 0 в интенсивности отказов в режиме ожидания
        public Cold_Reserve(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr,  bool BSV = false) : base(n, k, Exp, lambdaR, lambdaO, Nagr, BSV)
        {

            lambdaO0 = (lOj == 0) ? true: false;

            Generate_Operating_Times(false);//генерация наработок

        }

	//расчет массива наработок
        public override void CalculateTime(Main_Form form1)
        {
            
            if (!lambdaO0)//с учетом лямбды ожидания
            {
                for (int i = 0; i < numOfExp; i++)//цикл по экспериментам
                {

                    //лист значений в текущем эксперименте
                    CurrentExperiment = Enumerable.Range(0, Experiments.GetLength(0))
                                        .Select(x => Experiments[x, i])
                                        .ToList();

                    for (int x = N; x < N + K; x++)//изменение наработок резервов
                    {
                        CurrentExperiment[x] *= lRab / lOj;
                    }


                    for (int c = K; c > 0; c--)
                    {
                        int min = CurrentExperiment.IndexOf(CurrentExperiment.Min());

                        //отказ элемента в режиме ожидания - отключение элемента
                        if (min >= N)
                        {
                            CurrentExperiment.RemoveAt(min);
                            
                        }
                        //отказ элемента в режиме работы - пересчет нагрузки
                        else
                        {
				//увеличение наработки отказавшего на наработку первого из резервных с учетом времени нахождения того в режиме ожидания
                            CurrentExperiment[min] += (CurrentExperiment[N] - CurrentExperiment[min]) * lOj / lRab;

                            CurrentExperiment.RemoveAt(N);//удаление резервного


                        }

                    }
                    Time[i] = CurrentExperiment.Min();
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


                    for (int c = K; c > 0; c--)
                    {
			//поиск минимального из основных каналов
                        int min = (N > 1) ? CurrentExperiment.IndexOf(CurrentExperiment.GetRange(0, N).Min()) : 0;

                        CurrentExperiment[min] += CurrentExperiment[N];//увеличение его наработки на наработку первого из резервных

                        CurrentExperiment.RemoveAt(N);//удаление резервного

                    }
                  
                    Time[i] = CurrentExperiment.Min();
                    form1.Update_Progress(i);

                }


            }

        }
	//загрузка из файла
        public Cold_Reserve Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Cold_Reserve res = (Cold_Reserve)bin.Deserialize(fs);
            fs.Close();
            return res;
        }
	//вывод для отчета
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

            Result += "Количество экспериментов:";
            Result += Convert.ToString(this.numOfExp) + "\n";
            if (changed_num)
                Result += "(число экспериментов было уменьшено в связи с нехваткой памяти) \n";
            Result += "Время работы:";
            Result += Convert.ToString(needed_time) + "ч.\n";
            
            Result += "Результаты:\n";
            Result += "Средняя наработка:" + Convert.ToString(Get_Time_Average()) + "ч.\n";
            Result += "Вероятность безотказной работы в течение " + Convert.ToString(needed_time) + " ч.: " + Convert.ToString(Get_Pos(needed_time).Item1) + "+-" + Convert.ToString(Get_Pos(needed_time).Item2) + "\n";
            Result += "Среднее время простоя:" + Convert.ToString(Get_Average_Dead_Time(needed_time)) + "ч.\n";
            Result += "Коэффициент готовности:" + Convert.ToString(Get_Koef_Gotovnosti(needed_time)) + "\n";
            return Result;
        }

    }
}
