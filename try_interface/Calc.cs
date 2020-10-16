using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//класс для расчета величин циклов полной ротации ("Программа управления"), исходные формулы предоставлены преподавателем
namespace try_interface
{
    static class Calc
    {
        public static bool flag_rot;
        public static string Stat;


        // (1)
        public static double K_iep_m(int N, int M_m)
        {
            return ((double)N + 1) / ((double)N + (double)M_m);
        }

        // (2)
        public static double Lambda_tc_m(int N, int M_m, double lambda, double lambda_oj, double koef)
        {
            return K_iep_m(N, M_m) * lambda*koef + (1 - K_iep_m(N, M_m)) * lambda_oj;
        }

        // (3)
        public static double LAMBDA_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double koef)
        {
            double lambda_m = 0;
            if (m == 0)
            {
                lambda_m = Lambda_tc_m(N, M_m, lambda, lambda_oj, koef);
            }
            else
            {
                for (int i = 1; i <= m; i++)
                {
                    lambda_m += ((t_o[i] - t_o[i - 1]) / t_br) * Lambda_tc_m(N, M_m + i, lambda, lambda_oj, koef);
                }
                lambda_m += ((t_br - t_o[m]) / t_br) * Lambda_tc_m(N, M_m, lambda, lambda_oj, koef);
            }
            return lambda_m;
        }

        // (4)
        public static double P_k_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double koef)
        {
            return Math.Exp(-1 * LAMBDA_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef) * (t_br - t_o[m]));
        }

        //число сочетаний
        private static int CombCount(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }
        private static int Factorial(int num)
        {
            return (num == 0) ? 1 : num * Factorial(num - 1);
        }
        // (5)
        public static double P_b_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double koef)
        {
            double sum = 0;
            for (int j = 0; j <= M_m; j++)
            {
                sum += CombCount(N + M_m, j) * Math.Pow(P_k_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef), N + M_m - j) * Math.Pow(1 - P_k_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef), j);
            }
            return sum;
        }

        // (6)
        public static double P_0vkl_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double koef)
        {
            var res = p_tz / P_b_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef);
            flag_rot = (res < 1);
            return res;
        }

        // (7)
        public static double S_max_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double p_0vkltz, double koef)
        {
            return Math.Log(P_0vkl_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, p_tz, koef), p_0vkltz);
        }

        // (8)
        public static double S_1_m(int N, int M_m, double koef)
        {
            return 2 * (N + M_m);
        }

        // (9)
        public static int L_max_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double p_0vkltz, double koef)
        {
            return (int)Math.Truncate(S_max_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef) / S_1_m(N, M_m, koef)) + 1;

        }

        // (10)
        public static double Delta_p_m(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double p_0vkltz, double koef)
        {
            return (1 - P_k_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef)) / L_max_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef);
        }

        // (11)
        public static double T_tc_ml(int N, int M_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double p_0vkltz, int l, double koef)
        {
            return (Math.Log((1 - l * Delta_p_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef)) / (1 - (l - 1) * Delta_p_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef))) / LAMBDA_m(N, M_m, lambda, lambda_oj, t_o, m, t_br, koef)) * -1;
        }

        //результат
        public static double[] GetArr_T_tc_T_p(int n, int m_m, double lambda, double lambda_oj, double[] t_o, int m, double t_br, double p_tz, double p_0vkltz, double koef, bool fl = false)//, out double[] T_tc_arr, out double[] T_p_arr)
        {
            if (fl)
            {
                Stat += "-------------------------------------------------------------------"+Environment.NewLine;
                if (t_o[t_o.Count() - 1] == 0)
                    Stat += "Новый эксперимент" + Environment.NewLine;
                
                Stat += "Время последнего отказа: " +Convert.ToString(t_o[t_o.Count()-1] )+Environment.NewLine;
                Stat += "Число отказов: " + Convert.ToString(m) + Environment.NewLine;
                Stat += "1) K_iep_m = " + Convert.ToString(K_iep_m(n, m_m)) + Environment.NewLine;
                Stat += "2) Lambda_tc_m = " + Convert.ToString(Lambda_tc_m(n, m_m, lambda, lambda_oj, koef)) + Environment.NewLine;
                Stat += "3) LAMBDA_m = " + Convert.ToString(LAMBDA_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, koef)) + Environment.NewLine;
                Stat += "4) P_k_m = " + Convert.ToString(P_k_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, koef)) + Environment.NewLine;
                Stat += "5) P_b_m = " + Convert.ToString(P_b_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, koef)) + Environment.NewLine;
                Stat += "6) P_0vkl_m = " + Convert.ToString(P_0vkl_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, koef)) + Environment.NewLine;
                Stat += "7) S_max_m = " + Convert.ToString(S_max_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef)) + Environment.NewLine;
                Stat += "8) S_1_m = " + Convert.ToString(S_1_m(n, m_m, koef)) + Environment.NewLine;
                Stat += "9) L_max_m = " + Convert.ToString(L_max_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef)) + Environment.NewLine;
                Stat += "10) Delta_p_m = " + Convert.ToString(Delta_p_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef)) + Environment.NewLine;
            }
            int length = L_max_m(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, koef);
            double[] T_tc_arr;
            if (length > 0)
            {
                T_tc_arr = new double[length];
                //T_p_arr = new double[length];

                for (int l = 0; l < length; l++)
                {
                    T_tc_arr[l] = T_tc_ml(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, l, koef);
                    // T_p_arr[l] = T_tc_ml(n, m_m, lambda, lambda_oj, t_o, m, t_br, p_tz, p_0vkltz, l) / (n + m_m);
                }
            }
            else
                T_tc_arr = new double[0];

            return T_tc_arr;
        }

    }
}
