using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    public class StockRatingInfo
    {
        public String stock_index;
        public int capital;        //股本 (億)
        public float price_close;    //收盤價
        public float weighting;

        public StockRatingInfo(String i, int c, float p, float w)
        {
            stock_index = i;
            capital = c;
            price_close = p;
            weighting = w;
        }
    }
}
