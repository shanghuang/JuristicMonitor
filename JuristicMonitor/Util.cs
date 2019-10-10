using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    class Util
    {
        public static string convertDate2mysql(DateTime dt)
        {
            return dt.Year.ToString() + "-" + dt.Month.ToString() + "-" + dt.Day.ToString();
        }

        public static string double2dec(double val, int dec_total, int dec_rem)
        {
            int mul = (int)Math.Pow(10, dec_rem);
            int scaled = (int)(val * mul);
            int upper = scaled / mul;
            int low = scaled % mul;

            string res = upper.ToString() + "." + low.ToString("D2");

            return res;
        }

        public static String numberString2IntString(String src)       // 123,456.78  -> 123456
        {
            String res_str;

            String[] segs = src.Split(new char[] { '.' });     
            res_str = segs[0].Replace(",", "");

            return res_str;
        }
    }
}
