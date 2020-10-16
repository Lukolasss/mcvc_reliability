using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//СМЕШАННОЕ РЕЗЕРВИРОВАНИЕ С РОТАЦИЕЙ С УЧЕТОМ ВЕРОЯТНОСТИ ПЕРЕКЛЮЧЕНИЯ
namespace try_interface.Reserves
{
    [Serializable]
    class Rotation_V2_Probability : Reserve
    {
        private double POSOFSWITCH;//величина вероятности переключения

        private List<double[]> Periods;//периоды для различного количества оставшихся резервов
        private double T_exp;//время эксперимента
        public string Stat_Short;
        public bool const_Per;//определение постоянный/переменный период


        private double[] first_fail;//первые отказы

        private new List<List<double>> slegka;


        public Rotation_V2_Probability(int n, int k, int Exp, double lambdaR, double lambdaO, double[] Nagr, List<double[]> Per, double T, double possofswitch, bool const_P, bool BSV = false) : base(n, k, Exp, lambdaR, lambdaO, Nagr, BSV)
        {
 
            POSOFSWITCH = possofswitch;

            first_fail = new double[numOfExp];

            Generate_Operating_Times(true);

            Periods = new List<double[]>(Per);
            T_exp = T;
            const_Per = const_P;

        }
	//вероятность переключения
        public override double Get_Probability()
        {
            return POSOFSWITCH;
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
	//статистика для отладки
        public void AccumilateStatistics_Short(string s)
        {
            if (numOfExp <= 1000)
                Stat_Short += s + '\n';
        }


        public void AccumilateResult(double r)
        {
            Result += Convert.ToString(r) + " + "; //+"\n";
        }


        //генерация матрицы распределения работы
        public double[,] generateMatrix(int currentReserves)
        {
/*пример для схемы 1+1+1
матрица:
рор
рро
орр*/
            int e = N + 1 + currentReserves;
            double[,] result = new double[e, e];

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
            int[,] changingPattern = new int[N + 1 + K, Kolperiods];
            for (int i = 0; i < N + 1 + K; i++)
                for (int j = 0; j < Kolperiods; j++)
                    changingPattern[i, j] = i;
            for (int j = 1, ot = 0; j < Kolperiods; j++, ot++)
            {
                if (ot == N + 1 + K - 1)
                    ot = 0;

                for (int i = 0; i < N + 1 + K; i++)
                    changingPattern[i, j] = changingPattern[i, j - 1];//заполняем из предыдущего столбца
                int swap = changingPattern[ot, j];//смена отдых-работа
                changingPattern[ot, j] = changingPattern[N + 1, j];
                for (int i = N + 1; i < N + 1 + K - 1; i++)//сдвиг отдыхающих
                    changingPattern[i, j] = changingPattern[i + 1, j];
                changingPattern[N + 1 + K - 1, j] = swap;//смена работа-отдыз 
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
            double Treal_Sum = (i > 0) ? (((N + 1 + currentReserves * lOj / lRab) / (N + 1 + currentReserves)) * CurrentPeriods.GetRange(0,i+1).Sum())
                : (((N + 1 + currentReserves * lOj / lRab) / (N + 1 + currentReserves)) * CurrentPeriods[0]);
            return Treal_Sum;
        }
//вычисление суммарного времени пройденных полных циклов ротации 
        public double Partial_Sum(int i, int currentReserves, List<double> CurrentPeriods)
        {
            double Treal_Sum = (i > 0) ? ( CurrentPeriods.GetRange(0, i+1).Sum())
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
        public List<double> Substruct_Matrix(double[,] m, int nomer_per, double s,  List<double> a)
        {
            for (int i = 0; i < a.Count(); i++)
                a[i] -= m[i, nomer_per] * s;
            return a;
        }
//функция заполнения матрицы переключений для полных циклов
        public List<List<Tuple<bool, double>>> Fill_SwitchTab(int i_otk, int currentReserves, List<List<Tuple<bool, double>>> switchtab, List<List<Tuple<bool, int>>> helper, double[] CurrentPeriods)
        {
            if (i_otk > 0)
            {
                double alltime = 0;
                for (int i = 0; i < i_otk; i++)
                {
                    //Заполнение матрицы переключений для полных циклов
                    int tb = 0;
                    foreach (List<Tuple<bool, int>> sublist in helper)
                    {
                        if (switchtab.Count() != N + 1 + currentReserves)
                        {
                            List<Tuple<bool, double>> l = new List<Tuple<bool, double>>();
                            foreach (Tuple<bool, int> tpl in sublist)
                            {
                                l.Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * CurrentPeriods[i] / (N + 1 + currentReserves)));
                            }
                            switchtab.Add(l);
                        }
                        else
                        {
                            foreach (Tuple<bool, int> tpl in sublist)
                            {
                                switchtab[tb].Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * CurrentPeriods[i] / (N + 1 + currentReserves)));
                            }
                            tb++;
                        }
                    }
                    alltime += CurrentPeriods[i];
                }
            }
            //Заполнение матрицы переключений для полных циклов
            return switchtab;
        }

//функция поиска отказа во время ротации (включая отказ при переключении)
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


            //копируем наработки для проверки вероятностей
            double[] b = a.ToArray();


            //таблица пар времени переключений и вида переключений для каждого элемента(True - 01, False - 10)
            List<List<Tuple<bool, double>>> switchtab = new List<List<Tuple<bool, double>>>();

            //расчитываем матрицу распределения работы
            double[,] m = generateMatrix(currentReserves);
		//номера переключений и их тип для каждого канала
            List<List<Tuple<bool, int>>> helper = numsofswitch(m);

            double alltime = 0;//время от начала эксперимента

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
                    numofperekl += (i > 0) ? (i - 1) * (N + 1 + currentReserves) : 0;
			//запоминание цикла, в котором был отказ
                    Totk = CurrentPeriods[i];
                    
                }
            }
//пересчет времени работы
            time += (i - 1 > 0) ? Partial_Sum(i - 2, currentReserves, CurPer) : 0;
 //пересчет наработок каналов           
		a = Substruct(((i - 1 > 0) ? Partial_Sum_With_Wait(i - 2, currentReserves, CurPer) : 0), a);
            
//заполение таблицы переключений
            switchtab = Fill_SwitchTab(i - 2, currentReserves, switchtab, helper, CurrentPeriods);
            alltime += time;
            
            
            if (Totk > 0)//был отказ на каком-то из циклов
            {

                //проходим каждый период в следующем цикле полной ротации, чтобы выявить период, в котором произошел отказ
                int j = 0;
                bool flag = false;
                //находим, в какой именно момент произошел отказ (по матрице распределения работы)
                while (j < N + 1 + currentReserves && !flag)
                {
                    //поиск канала с минимальной наработкой
                    min = a.IndexOf(a.Min());
                   /*т.к. в текущем периоде канал с мин. наработкой может находиться в режиме ожидания, 
его наработка может превзойти наработку одного из работающих каналов
для этого поиск производится циклом по элементам*/                    
		    double srav = Totk / (N + 1 + currentReserves);
                    int d = 0;
                    for (; d < N + 1 + currentReserves && (a[d] - m[d, j] * srav) > 0; d++) ;
                    if (d < N + 1 + currentReserves)
                        min = d;

                    if (a[min] > m[min, j] * srav)//если данную часть цикла канал с мин наработкой отработал полностью - вычитаем из всех элементов время
                    {
                        time += srav;
                        numofperekl++;
                        a = Substruct_Matrix(m, j, srav, a);
                        
                        //заполнение матрицы переключений для неполного цикла
                        int p = 0;
                        foreach (List<Tuple<bool, int>> sublist in helper)
                        {
                            if (switchtab.Count() != N + 1 + currentReserves)
                            {
                                List<Tuple<bool, double>> l = new List<Tuple<bool, double>>();
                                foreach (Tuple<bool, int> tpl in sublist)
                                {
                                    if (tpl.Item2 == j)
                                        l.Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * Totk / (N + 1 + currentReserves)));
                                }
                                switchtab.Add(l);
                            }
                            else
                            {
                                foreach (Tuple<bool, int> tpl in sublist)
                                {
                                    switchtab[p].Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * Totk / (N + 1 + currentReserves)));
                                }
                                p++;
                            }
                        }
                        
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
                a.Insert(0, a.ElementAt(N));
                a.RemoveAt(N + 1);

                //расчет времени отказа по вероятности (отключение в момент переключения)
                List<List<double>> posstab = switchPossibility(switchtab);
                Tuple<double, List<double>> s = CountPossibility(currentReserves, b, posstab, switchtab, CurrentPeriods, m);

                if (s.Item1 < time && s.Item1 > 0)//отказ при переключении произошел раньше отказа по наработке
                {
                   
                    Time[nomerExperimenta] += s.Item1;
                   
                    return Tuple.Create(true, CoefficientNagruzki(s.Item2));
                }
                else//отказа при переключении не произошло
                {
                   
                    Time[nomerExperimenta] += time;
                    

                    return Tuple.Create(true, CoefficientNagruzki(a));
                }

            }
            //Totk = -1 => после всех ротаций не было ни одного отказа
            else//рассматриваем последний цикл полной ротации
            {
                //проходим по каждому периоду последнего цикла ротации
                int j = 0;
                bool flag = false;
//определяем количество периодов до окончания времени эксперимента - после этого ротация прекращается
                int g = (int)Math.Floor((((T_exp - CurrentPeriods.Sum() + CurrentPeriods.Last()) * (N + 1 + currentReserves))) / CurrentPeriods.Last());
                while (j < g && !flag)
                {
//поиск минимальной наработки
                    min = a.IndexOf(a.Min());
                    
                    double srav = CurrentPeriods.Last() / (N + 1 + currentReserves);
                    
                    int d = 0;
                    for (; d < N + 1 + currentReserves && (a[d] - m[d, j] * srav) > 0; d++) ;
                    if (d < N + 1 + currentReserves)
                        min = d;
                    
                    if (a[min] > m[min, j] * srav)//если данную часть цикла элемент отработал полностью - вычитаем из всех элементов время
                    {
                        time += srav;
                        numofperekl++;

                        a = Substruct_Matrix(m, j, srav, a);

                        //заполнение матрицы переключений для неполного цикла
                        int p = 0;
                        foreach (List<Tuple<bool, int>> sublist in helper)
                        {
                            if (switchtab.Count() != N + 1 + currentReserves)
                            {
                                List<Tuple<bool, double>> l = new List<Tuple<bool, double>>();
                                foreach (Tuple<bool, int> tpl in sublist)
                                {
                                    if (tpl.Item2 == j)
                                        l.Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * CurrentPeriods.Last() / (N + 1 + currentReserves)));
                                }
                                switchtab.Add(l);
                            }
                            else
                            {
                                foreach (Tuple<bool, int> tpl in sublist)
                                {
                                    switchtab[p].Add(Tuple.Create(tpl.Item1, alltime + (tpl.Item2 + 1) * CurrentPeriods.Last() / (N + 1 + currentReserves)));
                                }
                                p++;
                            }
                        }
                        //заполнение матрицы переключений для неполного цикла

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
                    //var c = a;
                    List<double> c = new List<double>(a);
                    for (int d = 0; d < a.Count(); d++)
                    {
                        double swap = c[d];
                        c[d] = a[f[d]];
                        a[f[d]] = swap;
                    }
                    a = c;

                    min = a.IndexOf(a.Min());
                    //удаляем отказавший элемент
                    a.RemoveAt(min);
                    //реконфигурация схемы
                    a.Insert(0, a.ElementAt(N));
                    a.RemoveAt(N + 1);

                    //расчет времени отказа по вероятности (отключение в момент переключения)
                    List<List<double>> posstab = switchPossibility(switchtab);
                    Tuple<double, List<double>> s = CountPossibility(currentReserves, b, posstab, switchtab, CurrentPeriods, m);

                    if (s.Item1 < time && s.Item1 > 0)//отказ при переключении произошел раньше отказа по наработке
                    {
                        
                        Time[nomerExperimenta] += s.Item1;
                        

                        return Tuple.Create(true, CoefficientNagruzki(s.Item2));
                    }
                    else//не было отказа при переключении
                    {
                        
                        Time[nomerExperimenta] += time;
                       
                        return Tuple.Create(true, CoefficientNagruzki(a));
                    }

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

                    
                    //расчет времени отказа по вероятности (отключение в момент переключения)
                    List<List<double>> posstab = switchPossibility(switchtab);
                    Tuple<double, List<double>> s = CountPossibility(currentReserves, b, posstab, switchtab, CurrentPeriods, m);

                    if (s.Item1 > 0 && s.Item1 < time)//отказ при переключении произошел раньше отказа по наработке
                    {
                       
                        Time[nomerExperimenta] += s.Item1;
                        
                        return Tuple.Create(true, CoefficientNagruzki(s.Item2));
                    }
                    else//не было отказа при переключении
                    {
                       
                        Time[nomerExperimenta] += time;
                        
                        return Tuple.Create(false, a);
                    }

                }
            }
        }

        //функция расчета наработки после окончания ротации
        public double withoutRotation(List<double> a, int currentReserves, int nomerEXP)
        {
            /*после окончания ротации ССН переходит в смешанное резервирование
		при смешанном резервировании коэффициент нагрузки остается неизменным вплоть до отказа всех ненагруженных резервов
		поэтому массив коэффициентов нагрузки изменяется*/
            double[] NewCoefNagr = CoefNagr;
            for (int l = N + 1; l < N + 1 + currentReserves - 1; l++)
            {
                NewCoefNagr[l] = CoefNagr[N];
            }
            //расчет аналогичен смешанному резервированию (см. Mixed_Reserve_Probability.Calculate_Time)

            List<double> b = new List<double>(a);
            if (lOj != 0)
            {
                for (int x = N + 1; x < N + 1 + currentReserves; x++)
                {
                    b[x] *= lRab / lOj;
                }
                for (; b.Count > N + 1;)
                {
                   
                    int min = b.IndexOf(b.Min());
                    if (b.Count == N+1+K)//первый отказ
                        first_fail[nomerEXP] = b[min];
                    //отказ элемента в режиме ожидания
                    if (min >= N + 1)
                    {
                        b.RemoveAt(min);
                        //пересчет нагрузки
                        b = CoefficientNagruzki(b, NewCoefNagr);
                    }
                    //отказ элемента в режиме работы
                    else
                    {
                        
                        Random rndposs = RandBSV ? new Random() : new Random(b.Count());
/*если сгенерированная величина больше заданной вероятности переключения, то происходит отказ при переключении, следовательно необходимо удалить отказавший и перейти к следующему резерву*/
                        for (int j = 0; j < b.Count() - N - 1 && rndposs.NextDouble() > POSOFSWITCH; j++)
                        {
                            b.RemoveAt(N + 1);
                            b = CoefficientNagruzki(b, NewCoefNagr);
                        }
                        
                        if (b.Count > N + 1)
                        {
                            b[min] += (b[N + 1] - b[min]) * lOj / lRab;
                            b.RemoveAt(N + 1);

                            b = CoefficientNagruzki(b, NewCoefNagr);
                        }
                    }

                }
                /*b.Sort();
                b.RemoveAt(0);
                return CoefficientNagruzki(b, NewCoefNagr)[0];*/

                b.Sort();
                double delta = b[0];
                for (int i = 0; i < b.Count; i++)
                    b[i] -= delta;
                b.RemoveAt(0);
                return delta + CoefficientNagruzki(b, NewCoefNagr)[0];
            }
            else//0 при ожидании
            {

                for (; b.Count > N + 1;)
                {
                    
                    int min = b.IndexOf(b.GetRange(0, N+1).Min());
                    if (b.Count == N + 1 + K)
                        first_fail[nomerEXP] = b[min];


                    Random rndposs = RandBSV ? new Random() : new Random(b.Count());
                    for (int j = 0; j < b.Count() - N - 1 && rndposs.NextDouble() > POSOFSWITCH; j++)
                    {
                       
                        b.RemoveAt(N + 1);
                        b = CoefficientNagruzki(b, NewCoefNagr);
                    }
                    
                    if (b.Count > N + 1)
                    {
                        b[min] += b[N + 1];
                        b.RemoveAt(N + 1);
                        b = CoefficientNagruzki(b, NewCoefNagr);
                    }

                }
                /*b.Sort();
                b.RemoveAt(0);
                return CoefficientNagruzki(b, NewCoefNagr)[0];*/

                b.Sort();
                double delta = b[0];
                for (int i = 0; i < b.Count; i++)
                    b[i] -= delta;
                b.RemoveAt(0);
                return delta + CoefficientNagruzki(b, NewCoefNagr)[0];
            }

        }

//функция расчета наработки
        public override void CalculateTime( Main_Form form1)
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

                    while ((T_exp - Time[i]) > w.Sum())
                        w.Add(Periods[currentReserves - 1].Last());//если сумма циклов меньше времени эксперимента



                    //вызов функции поиска отказа
                    Tuple<bool, List<double>> s = PoiskOtkaza(w.ToArray(),
                        currentReserves, CurrentExperiment, i, T_exp - Time[i]);

                    if (s.Item1)//отказ был (при ротации или при переключении)
                    {
                        if (currentReserves == K)//первый отказ
                            first_fail[i] = Time[i];

                        CurrentExperiment = s.Item2;
                        currentReserves = CurrentExperiment.Count() - (N + 1);
                    }
                    else//отказа не было -> ротация прекращается
                    {
                        flag_withouRot = true;
       
                        CurrentExperiment = s.Item2;
                    }



                }
                if (flag_withouRot)//окончание ротации по времени эксперимента без отказа
                    Time[i] += withoutRotation(CurrentExperiment, currentReserves,i);
                else//окончание ротации по причине отказа всех резервных каналов
                {
                    /*CurrentExperiment.Sort();
                    Time[i] += CoefficientNagruzki(CurrentExperiment)[1];*/
		//расчет нагруженного
		    CurrentExperiment.Sort();
                    double delta = CurrentExperiment[0];
                    for (int g = 0; g < CurrentExperiment.Count; g++)
                        CurrentExperiment[g] -= delta;
                    CurrentExperiment.RemoveAt(0);
                    Time[i] += delta + CoefficientNagruzki(CurrentExperiment)[0];
                }
                form1.Update_Progress(i);
            }
        }

//загрузка из файла
        public Rotation_V2_Probability Load(string filename)
        {
            BinaryFormatter bin = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Rotation_V2_Probability res = (Rotation_V2_Probability)bin.Deserialize(fs);
            fs.Close();
            return res;
        }
//вывод для отчета
        public override string Accumilate_Result(double needed_time, bool changed_num)
        {
            Result += "Исходные данные:\n";
            Result += "Вид резервирования: смешанное с ротацией\n";


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
            return Result;
        }


        //генерация вероятностей переключений 
        public List<List<double>> switchPossibility(List<List<Tuple<bool, double>>> tab)
        {
            Random rndposs = new Random(tab.Count);
            List<List<double>> posstab = new List<List<double>>();
            int x = 0;
            foreach (List<Tuple<bool, double>> list in tab)
            {
                if (posstab.Count() != tab.Count())
                {
                    List<double> l = new List<double>();
                    foreach (Tuple<bool, double> tup in list)
                    {
                        l.Add(rndposs.NextDouble());
                    }
                    posstab.Add(l);
                }
                else
                {
                    foreach (Tuple<bool, double> tup in list)
                    {
                        posstab[x].Add(rndposs.NextDouble());
                    }
                    x++;
                }
            }
            slegka = new List<List<double>>(posstab);
           
            return posstab;
        }

        //функция определения номеров переключений и их типов для каждого канала
        public List<List<Tuple<bool, int>>> numsofswitch(double[,] m)
        {
            List<List<Tuple<bool, int>>> numlist = new List<List<Tuple<bool, int>>>();
            for (int i = 0; i < m.GetLength(0); i++)
            {
                List<Tuple<bool, int>> list = new List<Tuple<bool, int>>();
                for (int j = 0; j < m.GetLength(1) - 1; j++)
                {
                    if (m[i, j] < m[i, j + 1])
                    {
                        list.Add(Tuple.Create(true, j));
                    }
                    if (m[i, j] > m[i, j + 1])
                    {
                        list.Add(Tuple.Create(false, j));
                    }
                    if (j == 0 && m[i, j] < m[i, j + m.GetLength(1) - 1])
                    {
                        list.Add(Tuple.Create(false, m.GetLength(1) - 1));
                    }
                    if (j == 0 && m[i, j] > m[i, j + m.GetLength(1) - 1])
                    {
                        list.Add(Tuple.Create(true, m.GetLength(1) - 1));
                    }
                }
                list = list.OrderBy(u => u.Item2).ToList();
                numlist.Add(list);
            }
            return numlist;
        }

        //функция проверки наличия отказа при переключении
        public Tuple<double, List<double>> CountPossibility(int currentReserves, double[] b, List<List<double>> posstab, List<List<Tuple<bool, double>>> switchtab, double[] CurrentPeriods, double[,] m)
        {
            double timee = 0;
            List<double> a = new List<double>();
            //Код для возможного пересчета времени из за вероятности переключения

            int per = N + 2 + currentReserves;
            int z = 0;//номер элемента
            int y = -1;
            foreach (List<double> list in posstab)
            {
                int numofsw = -1;
                foreach (double poss in list)
                {
                    numofsw++;
                    if (poss > POSOFSWITCH && numofsw < per)
                    {
                        per = numofsw;
                        y = z;
                    }
                }
                z++;
            }
            if (y != -1)//switchtab[y][per].Item2 - время переключения, на котором сломалось
            {
//был отказ при переключении 
//необходимо пересчитать наработки каналов
                int l = 0;
                double operiod = CurrentPeriods[0];
                while (switchtab[y][per].Item2 > operiod)
                {
                    for (int x = 0; x < b.Count(); x++)
                    {
                        b[x] -= CurrentPeriods[l] * ((N + 1 + currentReserves * lOj / lRab) / (N + 1 + currentReserves));
                    }
                    timee += CurrentPeriods[l];
                    operiod += CurrentPeriods[++l];
                }
                int j = 0, s = 0;
                
                while (j <= per) 
                {
                    if (s == N + 1 + currentReserves)
                        s = 0;
                    for (int x = 0; x < b.Count(); x++)
                    {
                        b[x] -= m[x, s] * CurrentPeriods[l] / b.Count();
                    }
                    timee += CurrentPeriods[l] / b.Count();
                    j++; s++;
                }
                b[y] = double.MinValue;
                a = b.ToList();
//реконфигурация схемы в соответствии со сменой мест каналов во время ротации (для отказа при переключении)
                int[] f = changingPlaces_possibility(N, currentReserves, per, y);
                for (int d = 0; d < a.Count(); d++)
                {
                    double swap = a[d];
                    a[d] = b[f[d]];
                    b[f[d]] = swap;
                }
                a.Remove(a.Min());

            }


            return Tuple.Create(timee, a);
        }
 /*функция реконфигурации каналов после окончания ротации (окончание по отказу и окончание по истечении времени эксперимента)
при ротации каналы меняются местами; эта функция возвращает порядок расположения каналов при заданном числе периодов*/
        public int[] changingPlaces_possibility(int N, int K, int Kolper, int nomer_elementa)
        {
            int[,] changingPattern = new int[N + 1 + K, (Kolper + 1) * (N + 1 + K)];
            int q = 0;
            for (int i = 0; i < N + 1 + K; i++)
                changingPattern[i, 0] = i;
            for (int j = 1, ot = 0, f = 0; j < (Kolper + 1) * (N + 1 + K) && f < Kolper + 1; j++, ot++)
            {
                if (ot == N + 1 + K - 1)
                    ot = 0;


                for (int i = 0; i < N + 1 + K; i++)
                    changingPattern[i, j] = changingPattern[i, j - 1];

                if (changingPattern[ot, j] == nomer_elementa || changingPattern[N + 1 + K - 1, j] == nomer_elementa)
                    f++;

                int swap = changingPattern[ot, j];
                changingPattern[ot, j] = changingPattern[N + 1, j];
                for (int i = N + 1; i < N + 1 + K - 1; i++)
                    changingPattern[i, j] = changingPattern[i + 1, j];
                changingPattern[N + 1 + K - 1, j] = swap;
                q++;

            }
            int[] d = Enumerable.Range(0, changingPattern.GetLength(0))
                                    .Select(x => changingPattern[x, q])
                                    .ToArray();

            return d;
        }
    }
}
