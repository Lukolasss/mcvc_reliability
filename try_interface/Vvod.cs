using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//класс для хранения введенных пользователем данных при работе с главной формой
namespace try_interface
{
    class Vvod
    {
        public string main;
        public string res;
        public string lrab;
        public string loj;
        public string Knagr_main;
        public string Knagr_res;
        public string Ver;
        //public int nom_radbut;
        public bool radiobutt_const;
        public bool radiobutt_change;
        public bool radiobutt_uprav;
        public string per_const;
        public string[] per_chan;
        public string ver_ts;

        public Vvod()
        {
            main = "4";
            res = "4";
            lrab = "50";
            loj = "50";
            Knagr_main = "1";
            Knagr_res = "0,799 0,734 0,694 0,678";
            //Knagr_res = "0,799";
            Ver = "0,995";
            //nom_radbut = 1;
            radiobutt_const = true;
            radiobutt_change = false;
            radiobutt_uprav = false;
            per_const = "216";
            per_chan = new string[Convert.ToInt32(res)];
            for (int i = 0; i < Convert.ToInt32(res); i++)
                per_chan[i] = "100 50";
            ver_ts = "0,9";
        }

        public void Change(string type, string value)
        {
            switch (type)
            {
                case "N":
                    main = value;
                    break;
                case "K":
                    res = value;
                    per_chan = new string[Convert.ToInt32(res)];
                    for (int k = 0; k < Convert.ToInt32(res); k++)
                        per_chan[k] = "100 50";
                    break;
                case "lrab":
                    lrab = value;
                    break;
                case "loj":
                    loj = value;
                    break;
                case "Knagr_main":
                    Knagr_main = value;
                    break;
                case "Knagr_res":
                    Knagr_res = value;
                    break;
                case "pos":
                    Ver = value;
                    break;
                case "const_per":
                    radiobutt_const = Convert.ToBoolean(value);
                    break;
                case "changing_per":
                    radiobutt_change = Convert.ToBoolean(value);
                    break;
                case "prog_uprav":
                    radiobutt_uprav = Convert.ToBoolean(value);
                    break;
                case "per":
                    per_const = value;
                    break;
               /* case "per"+Convert.ToString(i):
                    radiobutt_uprav = Convert.ToBoolean(value);
                    break;*/
                case "pos_TS":
                    ver_ts = value;
                    break;
                default:
                    int i = Convert.ToInt32(type.Last());
                    per_chan[i] = value;
                    break;


            }
        }


    }
}
