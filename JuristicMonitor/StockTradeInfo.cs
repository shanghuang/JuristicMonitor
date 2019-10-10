using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace JuristicMonitor
{
    public class StockTradeInfo
    {
        public String stock_name;
        public String stock_index;
        public DateTime trans_date;
        public int trans_volume;
        public int trans_count;
        public long trans_value;
        public float open;
        public float high;
        public float low;
        public float close;
        public float diff;
        public float close_buy_price;
        public long close_buy_volume;
        public float close_sell_price;
        public long close_sell_volume;
        public float pe_ratio;

        public StockTradeInfo(MySqlDataReader rdr)
        {
            stock_name = (String)rdr["stock_name"];
            stock_index = (String)rdr["stock_index"];
            trans_date = (DateTime)rdr["trans_date"];
            trans_volume = (int)rdr["trans_volume"];
            trans_count = (int)rdr["trans_count"];
            trans_value = (int)rdr["trans_value"];
            open = float.Parse(rdr["open"].ToString());
            high = float.Parse(rdr["high"].ToString());
            low = float.Parse(rdr["low"].ToString());
            close = float.Parse(rdr["close"].ToString());
            diff = float.Parse(rdr["diff"].ToString());
            close_buy_price = float.Parse(rdr["close_buy_price"].ToString());
            close_sell_price = float.Parse(rdr["close_sell_price"].ToString());
            pe_ratio = float.Parse(rdr["pe_ratio"].ToString());
            close_buy_volume = (int)rdr["close_buy_volume"];
            close_sell_volume = (int)rdr["close_sell_volume"];
        }

        //0         1           2       3       4       5       6       7   8                   9           10      11              12             13       14
        //證券代號 證券名稱 成交股數 成交筆數 成交金額 開盤價 最高價 最低價 收盤價 漲跌(+/-) 漲跌價差 最後揭示買價 最後揭示買量 最後揭示賣價 最後揭示賣量 本益比 
        public StockTradeInfo(String[] parms, DateTime date)
        {
            //String[] parms = src.Split(new char[] {'\r', '\n','\"', ' '}, StringSplitOptions.RemoveEmptyEntries);

            int i;
            for (i = 0; i < parms.Length; i++)
            {
                string str = (string)parms[i];
                if (str.Equals("--") || str.Equals("") || str.Equals(" "))
                {
                    parms[i] = "0";
                }
            }
            char[] delim = new char[] { ',', ' ', '.' };
            trans_date = date;
            stock_index = ((string)parms[0]).Trim();
            stock_name = ((string)parms[1]).Trim();

            int.TryParse(((string)parms[2]).Replace(",", ""), out trans_volume);
            int.TryParse(((string)parms[3]).Replace(",", ""), out trans_count);
            Int64.TryParse(((string)parms[4]).Replace(",", ""), out trans_value);
            float.TryParse(((string)parms[5]).Replace(",", ""), out open);
            float.TryParse(((string)parms[6]).Replace(",", ""), out high);
            float.TryParse(((string)parms[7]).Replace(",", ""), out low);
            float.TryParse(((string)parms[8]).Replace(",", ""), out close);
            float.TryParse(((string)parms[10]).Replace(",", ""), out diff);
            if (((string)parms[9]).Contains("<p style= color:green>-</p>"))
                diff = (-1)*diff;
            float.TryParse(((string)parms[11]).Replace(",", ""), out close_buy_price);
            long.TryParse(((string)parms[12]).Replace(",", ""), out close_buy_volume);
            float.TryParse(((string)parms[13]).Replace(",", ""), out close_sell_price);
            long.TryParse(((string)parms[14]).Replace(",", ""), out close_sell_volume);
            float.TryParse(((string)parms[15]).Replace(",", ""), out pe_ratio);
        }
    }
}
