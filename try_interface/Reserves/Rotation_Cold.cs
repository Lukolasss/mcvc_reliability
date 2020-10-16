using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace try_interface.Reserves
{
    [Serializable]
    class Rotation_Cold : Reserve
    {
        private List<double[]> Periods;//периоды для различного количества оставшихся резервов
        private double T_exp;//время эксперимента
        public string Stat_Short;
        public bool const_Per;//определение постоянный/переменный период
        private double[] first_fail;//первые отказы


        public Rotation_Cold(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr, List<double[]> Per, double T, bool const_P, bool BSV = false) : base(n, k, Exp, lambdaR, lambdaO, Nagr, BSV)
        {


            Generate_Operating_Times(false);
            first_fail = new double[numOfExp];
            Periods = new List<double[]>(Per);
            T_exp = T;
            const_Per = const_P;


        }
        //периоды для конкретного числа резервов
        public double[] Get_Periods(int res)
        {
            return Periods[Periods.Count() - res - 1];
        }
        //минимальный из первых отказов
        public double Get_Min_First_Fail()
        {

            return first_fail.Min();
        }
        //среднее первых отказов
        public double Get_Average_First_Fail()
        {
            return first_fail.Average();
        }

        //генерация матрицы распределения работы
        public double[,] generateMatrix(int currentReserves)
        {
            int e = N + 1 + currentReserves;
            double[,] result = new double[e, e];
            /*пример для схемы 1+1+1
            матрица:
            рор
            рро
            орр*/
            for (int i = 0, o = 1; i < e; i++, o++)
            {
                for (int j = 0; j < e; j++)
                {
                    if ((j >= o && j < o + currentReserves) && (o + currentReserves < e))
                    {
                        result[i, j] = lOj / lRab;//ожидание
                    }
                    else
                    {
                        if ((o + currentReserves >= e) && (j >= o || j < (o + currentReserves) - e))
                        {
                            result[i, j] = lOj / lRab;//ожидание
                        }
                        else result[i, j] = 1;//работа
                    }
                }
            }
            return result;
        }
        /*функция реконфигурации каналов после окончания ротации (окончание по отказу и окончание по истечении времени эксперимента)
при ротации каналы меняются местами; эта функция возвращает порядок расположения каналов при заданном числе периодов*/
        public int[] changingPlaces(int N, int K, int Kolperiods)
        {
            int[,] changingPattern = new int[N + K, Kolperiods];
            for (int i = 0; i < N + K; i++)
                for (int j = 0; j < Kolperiods; j++)
                    changingPattern[i, j] = i;
            for (int j = 1, ot = 0; j < Kolperiods; j++, ot++)
            {
                if (ot == N + K - 1)
                    ot = 0;

                for (int i = 0; i < N + K; i++)
                    changingPattern[i, j] = changingPattern[i, j - 1];//заполняем из предыдущего столбца
                int swap = changingPattern[ot, j];//смена отдых-работа
                changingPattern[ot, j] = changingPattern[N, j];
                for (int i = N; i < N + K - 1; i++)//сдвиг отдыхающих
                    changingPattern[i, j] = changingPattern[i + 1, j];
                changingPattern[N + K - 1, j] = swap;//смена работа-отдыз 
            }
            //возврат мест элементов после последней перестановки
            int[] d = Enumerable.Range(0, changingPattern.GetLength(0))
                                    .Select(x => changingPattern[x, Kolperiods - 1])
                                    .ToArray();
            return d;
        }
        //вычисление реального времени работы одного канала за целое число полных циклов ротации 
        //i-количество полный циклов ротации
        //currentReserves-текущее число резервов
        //CurrentPeriods-текущие циклы
        public double Partial_Sum_With_Wait(int i, int currentReserves, List<double> CurrentPeriods)
        {
            double Treal_Sum = (i > 0) ? (((N + currentReserves * lOj / lRab) / (N + currentReserves)) * CurrentPeriods.GetRange(0, i + 1).Sum())
                : (((N + currentReserves * lOj / lRab) / (N + currentReserves)) * CurrentPeriods[0]);
            return Treal_Sum;
        }
        //вычисление суммарного времени пройденных полных циклов ротации 
        public double Partial_Sum(int i, int currentReserves, List<double> CurrentPeriods)
        {
            double Treal_Sum = (i > 0) ? (CurrentPeriods.GetRange(0, i + 1).Sum())
                : (CurrentPeriods[0]);
            return Treal_Sum;
        }
        //вычитание величины s из всех элементов списка
        public List<double> Substruct(double s, List<double> a)
        {
            for (int i = 0; i < a.Count(); i++)
                a[i] -= s;
            return a;
        }
        //вычитание величины s, умноженной на значений из матрицы переключений, из всех элементов списка
        //nomer_per - номер переключения в одном цикле полной ротации
        //m - матрица переключений
        public List<double> Substruct_Matrix(double[,] m, int nomer_per, double s, List<double> a)
        {
            for (int i = 0; i < a.Count(); i++)
                a[i] -= m[i, nomer_per] * s;
            return a;
        }

        //функция поиска отказа во время ротации
        //функция возвращает измененные наработки каналов и true, если был отказ при ротации, иначе - false
        //при появлении отказа во время ротации наработки пересчитываются и отказавший канал удаляется
        //после отказа схема рассматривается как новая с меньшим числом каналов
        //nomerExperimenta - номер текущего эксперимента
        //currentReserves - текущее число резервов
        //CurrentPeriods - текущие циклы
        //T_exp - время эксперимента, в течение которого происходит ротация
        //если за T_exp отказа не было, ротация прекращается 
        public Tuple<bool, List<double>> PoiskOtkaza(double[] CurrentPeriods, int currentReserves, List<double> a, int nomerExperimenta, double T_exp)
        {
            double time = 0;
            double Totk = -1;//для случая, когда не будет ни одного отключения
            int numofperekl = 0;


            //находим минимальный элемент
            int min = a.IndexOf(a.Min());


            //расчитываем матрицу распределения работы
            double[,] m = generateMatrix(currentReserves);

            int i = 0;
            List<double> CurPer = CurrentPeriods.ToList();
            //цикл до предпоследнего периода (время эксперимента попадает в последний период, поэтому он рассматривается отдельно)
            for (; i < CurrentPeriods.Length - 1 && Totk < 0; i++)
            {

                if (a.Min() > Partial_Sum_With_Wait(i, currentReserves, CurPer))
                    continue;
                else
                {
                    //пересчет количества переключений
                    numofperekl += (i > 0) ? (i - 1) * (N + currentReserves) : 0;
                    //запоминание цикла, в котором был отказ
                    Totk = CurrentPeriods[i];
                    //пересчет времени работы
                    time += (i > 0) ? Partial_Sum(i - 1, currentReserves, CurPer) : 0;
                    //пересчет наработок каналов
                    a = Substruct(((i > 0) ? Partial_Sum_With_Wait(i - 1, currentReserves, CurPer) : 0), a);
                }
            }



            if (Totk > 0)//был отказ на каком-то из циклов
            {
                //проходим каждый период в следующем цикле полной ротации, чтобы выявить период, в котором произошел отказ
                int j = 0;
                bool flag = false;
                //находим, в какой именно момент произошел отказ (по матрице распределения работы)
                while (j < N + currentReserves && !flag)
                {
                    //поиск канала с минимальной наработкой
                    min = a.IndexOf(a.Min());
                    /*т.к. в текущем периоде канал с мин. наработкой может находиться в режиме ожидания, 
 его наработка может превзойти наработку одного из работающих каналов
 для этого поиск производится циклом по элементам*/
                    double srav = Totk / (N + currentReserves);

                    int d = 0;
                    for (; d < N + currentReserves && (a[d] - m[d, j] * srav) > 0; d++) ;
                    if (d < N + currentReserves)
                        min = d;
                    if (a[min] > m[min, j] * srav)//если данную часть цикла канал с мин наработкой отработал полностью - вычитаем из всех элементов время
                    {
                        time += srav;
                        numofperekl++;
                        a = Substruct_Matrix(m, j, srav, a);


                    }
                    //иначе - на этом моменте произошел отказ - вычитаем оставшееся время и выходим из цикла
                    else
                    {
                        time += a[min];
                        a = Substruct_Matrix(m, j, a[min], a);

                        flag = true;

                    }
                    j++;
                }
                //реконфигурация схемы в соответствии со сменой мест каналов во время ротации
                int[] f = changingPlaces(N, currentReserves, numofperekl + 1);
                List<double> c = new List<double>(a);
                for (int d = 0; d < a.Count(); d++)
                {
                    double swap = c[d];
                    c[d] = a[f[d]];
                    a[f[d]] = swap;
                }
                a = c;
                //удаление отказавшего канала
                min = a.IndexOf(a.Min());
                a.RemoveAt(min);
                //реконфигурация схемы 
                /*-на смену отказавшему встает первый из резервных
                -заменяющий канал становится первым из основных
                -остальные основные каналы сдвигаются
                -резервные каналы сдвигаются*/
                a.Insert(0, a.ElementAt(N-1));
                a.RemoveAt(N);
                Time[nomerExperimenta] += time;//увеличение наработки в текущем эксперименте
                return Tuple.Create(true, CoefficientNagruzki(a));//пересчет наработок и выход из функции

            }
            //Totk = -1 => после всех ротаций не было ни одного отказа
            else//рассматриваем последний цикл полной ротации
            {
                //пересчет количества переключений
                numofperekl += (CurrentPeriods.Length - 1) * (N + currentReserves);
                //пересчет времени работы
                time += (i > 0) ? Partial_Sum(i - 1, currentReserves, CurPer) : 0;
                //пересчет наработок каналов
                a = Substruct(((i > 0) ? Partial_Sum_With_Wait(i - 1, currentReserves, CurPer) : 0), a);


                //проходим по каждому периоду последнего цикла ротации
                int j = 0;
                bool flag = false;
                //определяем количество периодов до окончания времени эксперимента - после этого ротация прекращается
                int g = (int)Math.Floor((((T_exp - CurrentPeriods.Sum() + CurrentPeriods.Last()) * (N + currentReserves))) / CurrentPeriods.Last());
                while (j < g && !flag)
                {
                    //поиск мин наработки
                    min = a.IndexOf(a.Min());

                    double srav = CurrentPeriods.Last() / (N + currentReserves);
                    int d = 0;
                    for (; d < N + currentReserves && (a[d] - m[d, j] * srav) > 0; d++) ;
                    if (d < N + currentReserves)
                        min = d;

                    if (a[min] > m[min, j] * srav) //если данную часть цикла элемент отработал полностью - вычитаем из всех элементов время
                    {
                        time += srav;
                        numofperekl++;

                        a = Substruct_Matrix(m, j, srav, a);

                    }
                    //иначе - на этом моменте произошел отказ - вычитаем оставшееся время и выходим из цикла
                    else
                    {
                        time += a[min];
                        a = Substruct_Matrix(m, j, a[min], a);

                        flag = true;
                    }
                    j++;
                }
                if (flag)//был отказ до окончания времени эксперимента
                {

                    //реконфигурация схемы в соответствии со сменой мест каналов во время ротации
                    int[] f = changingPlaces(N, currentReserves, numofperekl + 1);

                    List<double> c = new List<double>(a);
                    for (int d = 0; d < a.Count(); d++)
                    {
                        double swap = c[d];
                        c[d] = a[f[d]];
                        a[f[d]] = swap;
                    }
                    a = c;

                    //удаление отказавшего канала
                    min = a.IndexOf(a.Min());
                    a.RemoveAt(min);
                    //реконфигурация схемы 
                    a.Insert(0, a.ElementAt(N-1));
                    a.RemoveAt(N);
                    Time[nomerExperimenta] += time;

                    return Tuple.Create(true, CoefficientNagruzki(a));//пересчет наработок и выход из функции




                }
                else//дошли до времени эксперимента без отказа -> ротация закончилась
                {
                    //реконфигурация схемы в соответствии со сменой мест каналов во время ротации
                    int[] f = changingPlaces(N, currentReserves, numofperekl + 1);
                    List<double> c = new List<double>(a);
                    for (int d = 0; d < a.Count(); d++)
                    {
                        double swap = c[d];
                        c[d] = a[f[d]];
                        a[f[d]] = swap;
                    }
                    a = c;
                    Time[nomerExperimenta] += time;

                    return Tuple.Create(false, a);//выход из функции

                }
            }
        }



        //функция расчета наработки после окончания ротации
        public double withoutRotation(List<double> a, int currentReserves, int nomerEXP)
        {
           
            //расчет аналогичен смешанному резервированию (см. Mixed_Reserve.Calculate_Time)
            List<double> b = new List<double>(a);
            if (!lOj)//с учетом лямбды ожидания
            {
                    for (int x = N; x < N + K; x++)//изменение наработок резервов
                    {
                       b[x] *= lRab / lOj;
                    }


                 for (int c = K; c > 0; c--)
                 {
                    int min = b.IndexOf(b.Min());
                    if (c == K)//первый отказ
                    {
                        first_fail[nomerEXP] = b[min];
                    }

                    //отказ элемента в режиме ожидания - отключение элемента
                    if (min >= N)
                    {
                         b.RemoveAt(min);

                    }
                        //отказ элемента в режиме работы - пересчет нагрузки
                    else
                    {
                            //увеличение наработки отказавшего на наработку первого из резервных с учетом времени нахождения того в режиме ожидания
                        b[min] += (b[N] - b[min]) * lOj / lRab;
                        b.RemoveAt(N);//удаление резервного

                    }

                 }
                 return b.Min();
            }
            else//без учета лямбды ожидания
            {
                for (int c = K; c > 0; c--)
                {
                        //поиск минимального из основных каналов
                    int min = (N > 1) ? b.IndexOf(b.GetRange(0, N).Min()) : 0;

                    if (c == K)//первый отказ
                    {
                        first_fail[nomerEXP] = b[min];
                    }

                    b[min] += b[N];//увеличение его наработки на наработку первого из резервных

                    b.RemoveAt(N);//удаление резервного

                }

               return b.Min();
            }

        }


        //функция расчета наработки
        public override void CalculateTime(Main_Form form1)
        {
            for (int i = 0; i < numOfExp; i++)//цикл по количеству экспериментов
            {

                //лист значений в текущем эксперименте
                CurrentExperiment = Enumerable.Range(0, Experiments.GetLength(0))
                                    .Select(x => Experiments[x, i])
                                    .ToList();


                int currentReserves = K;
                bool flag_withouRot = false;
                while (currentReserves > 0 && !flag_withouRot)//пока есть ротация
                {

                    //корректировка периодов в соответствие с Т_э (время эксперимента)
                    var w = Periods[currentReserves - 1].ToList();

                    while ((T_exp - Time[i]) < (w.Sum() - w.Last()))//если сумма циклов превышает время эксперимента
                        w.RemoveAt(w.Count() - 1);

                    while ((T_exp - Time[i]) > w.Sum())//если сумма циклов меньше времени эксперимента
                        w.Add(Periods[currentReserves - 1].Last());


                    //вызов функции поиска отказа
                    Tuple<bool, List<double>> s = PoiskOtkaza(w.ToArray(), currentReserves, CurrentExperiment, i, T_exp - Time[i]);

                    if (s.Item1)//отказ был
                    {
                        if (currentReserves == K)//первый отказ
                            first_fail[i] = Time[i];
                        //новые наработки каналов
                        CurrentExperiment = s.Item2;
                        //количество оставшихся резервов
                        currentReserves = CurrentExperiment.Count() - N;
                    }
                    else//отказа не было
                    {
                        flag_withouRot = true;
                        CurrentExperiment = s.Item2;
                    }

                }
                if (flag_withouRot)//окончание ротации по времени эксперимента без отказа
                {
                    Time[i] += withoutRotation(CurrentExperiment, currentReserves, i);
                }
                else//окончание ротации по причине отказа всех резервных каналов
                {
                    
                    Time[i] += CurrentExperiment.Min();
                }

                form1.Update_Progress(i);

            }
        }

        //загрузка из файла
        public Rotation_Cold Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Rotation_Cold res = (Rotation_Cold)bin.Deserialize(fs);
            fs.Close();
            return res;
        }
        //вывод для отчета
        public override string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Вид резервирования: скользящее ненагруженное с ротацией\n";


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

           
            if (!const_Per)
            {
                Result += "Циклы полной ротации:\n";
                for (int i = 0; i < this.Get_K(); i++)
                {
                    var per = this.Get_Periods(i);
                    foreach (double p in per)
                        Result += Convert.ToString(p) + " ч.";
                    Result += "\n";
                }
            }
            else
            {
                Result += "Цикл полной ротации: ";
                Result += Convert.ToString(this.Get_Periods(0)[0]) + " ч.\n";
                Result += "Период: ";
                Result += Convert.ToString(this.Get_Periods(0)[0] / (this.N + 1 + this.K)) + " ч.\n";

            }

            Result += "Результаты:\n";
            Result += "Средняя наработка:" + Convert.ToString(Get_Time_Average()) + "ч.\n";
            Result += "Вероятность безотказной работы в течение " + Convert.ToString(needed_time) + " ч.: " + Convert.ToString(Get_Pos(needed_time).Item1) + "+-" + Convert.ToString(Get_Pos(needed_time).Item2) + "\n";
            Result += "Среднее время простоя:" + Convert.ToString(Get_Average_Dead_Time(needed_time)) + "ч.\n";
            Result += "Коэффициент готовности:" + Convert.ToString(Get_Koef_Gotovnosti(needed_time)) + "\n";


            Result += "среднее первых отказов " + Convert.ToString(Get_Average_First_Fail()) + "ч.\n";
            Result += "100 первых отказов :\n";
            for (int i = 0; i < this.first_fail.Count() && i < 100; i++)
                Result += Convert.ToString(this.first_fail[i]) + "ч.\n";

            return Result;
        }

    }
}
