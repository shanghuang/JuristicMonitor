using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace JuristicMonitor
{
    public class JuristicTradeInfo
    {
        public int juristic_type;
        public DateTime trans_date;
        public String stock_index;
                                        //"stock_name   VARCHAR(12). "+
        public String stock_name;
        public int buy_volume;
        public int sell_volume;
        public int total_volume;

        public JuristicTradeInfo(MySqlDataReader rdr)
        {
            juristic_type = (int)rdr["juristic_type"];
            trans_date = (DateTime)rdr["trans_date"];
            stock_index = (String)rdr["stock_index"];
            stock_name = (String)rdr["stock_name"];
            buy_volume = (int)rdr["buy_volume"];
            sell_volume = (int)rdr["sell_volume"];
            total_volume = (int)rdr["total_volume"];
        }

        public JuristicTradeInfo(String[] parms, DateTime date)
        {
            trans_date = date;
            stock_index = ((string)parms[1]).Trim();
            stock_name = ((string)parms[2]).Trim();
            int.TryParse(((string)parms[3]).Replace(",", ""), out buy_volume);
            int.TryParse(((string)parms[4]).Replace(",", ""), out sell_volume);
            int.TryParse(((string)parms[5]).Replace(",", ""), out total_volume);
        }
    }
}
