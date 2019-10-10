using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace JuristicMonitor
{
    public class PageJuristic  : EquiltyPage
    {
        string table_name = "juristic";

        public class JuristicConfig
        {
            public String fileNamePrefix;
            public String urlPath;
            public String dbTableName;
            public Boolean skip_tr;
            public int juristic_index;

            public JuristicConfig(string f, string u, string d, Boolean st, int idx) { fileNamePrefix = f; urlPath = u; dbTableName = d; skip_tr = st; juristic_index = idx; }
            public String getDBTableName() { return dbTableName; }
        }

        static JuristicConfig[] EquilitySource = new JuristicConfig[] { 
                    new JuristicConfig("foreign_",      @"https://www.twse.com.tw/fund/TWT38U?response=json&date=", "foreign_buyer", false, 1),
                    new JuristicConfig("selfoper_",     @"https://www.twse.com.tw/fund/TWT43U?response=json&date=", "self_operation", true, 2),
                    new JuristicConfig("investtrust_",  @"https://www.twse.com.tw/fund/TWT44U?response=json&date=", "invest_trust", false, 3),};

        public JuristicConfig config;

        public PageJuristic(string config_str)
            : base(config_str)
        {
            foreach (JuristicConfig cfg in EquilitySource)
            {
                if (config_str.Equals(cfg.fileNamePrefix))
                    config = cfg;
            }
        }


        public override String getPageUrl(DateTime date)
        {
            int tw_year = date.Year - 1911;
            string url_string = config.urlPath + string.Format("{0:D4}{1:D2}{2:D2}", date.Year, date.Month, date.Day) + "&_=1232123123123";
            return url_string;
        }

        public void ParseHtmlNew(MySqlConnection conn, string source, DateTime date)
        {
            Document doc = NSoup.NSoupClient.Parse(source);

            Elements table_tr = doc.Select("table tbody tr");

            foreach (Element tr in table_tr)
            {
                Elements tds = tr.Select("td");
                ArrayList parms = new ArrayList();
                foreach(Element td in tds)
                {
                    parms.Add(td.Text());
                }

                if(parms.Count < 5)
                    break;

                String stock_idx = (String)parms[1];
                String stock_name = (String)parms[2];
                int buy_volume; int.TryParse(((string)parms[3]).Replace(",", ""), out buy_volume);
                int sell_volume; int.TryParse(((string)parms[4]).Replace(",", ""), out sell_volume);
                int total_volume; int.TryParse(((string)parms[5]).Replace(",", ""), out total_volume);

                string sql_ins = "INSERT INTO " + table_name + "(juristic_type, trans_date, stock_index, stock_name, buy_volume, sell_volume, total_volume) VALUE(" +
                                                            config.juristic_index.ToString() + ", " +
                                                            "'" + Util.convertDate2mysql(date) + "'" + "," + "\'" + stock_idx + "\'" + "," + "\'" + stock_name +
                                                            "\'" + "," + buy_volume.ToString() + "," +
                                                            sell_volume.ToString() + "," + total_volume.ToString() + ");";
                MySqlCommand cmd_insert = new MySqlCommand(sql_ins, conn);
                try
                {
                    cmd_insert.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                    //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        public override ArrayList ParseHtml(MySqlConnection conn, string source, DateTime date)
        {
            JObject obj = JObject.Parse(source);
            ArrayList stocks = new ArrayList();

            if (!IsValidJson(source))
            {
                return stocks;
            }

            if (obj == null || obj["data"] == null)
            {
                return stocks;
            }
            
            foreach (JArray stk in obj["data"])
            {
                //String stock_str = stk.ToString();
                //stock_str = stock_str.Substring(1, stock_str.Length - 2);   //remove []
                JToken[] token_ary = stk.ToArray();
                String[] stk_strings = new string[token_ary.Length];
                for (int i = 0; i < token_ary.Length; i++)
                    stk_strings[i] = token_ary[i].ToString();

                JuristicTradeInfo stock_info = new JuristicTradeInfo(stk_strings, date);
                stocks.Add(stock_info);
            }

            return stocks;
        }

        /*
        public void ParseHtml(MySqlConnection conn, string source, DateTime date)
        {
            string db_table_name = config.dbTableName;

            string[] sp_str1 = { @"tbl-containerx" };                           //<div id='tbl-containerx' ....
            string[] sp_str_end = { @"</tbody>" };
            string[] str_segs1 = source.Split(sp_str1, StringSplitOptions.None);

            Logger.v("Parsing html : (" + date.ToShortDateString() + ")," + getFileName(date));

            if (str_segs1.Length <= 1)
            {
                //try new format
                ParseHtmlNew(conn, source, date);
                return;
            }

            string[] str_segs2 = str_segs1[1].Split(sp_str_end, StringSplitOptions.None);
            DateTime y2006_first_day = new DateTime(2006, 1, 1);

            //richTextBox3.Text = "Parse " + db_table_name + ":" + date.ToShortDateString();
            //UI_proc_stock_num = "Parse " + db_table_name + ":" + date.ToShortDateString();

            if (str_segs2.Length <= 1)
                return;
            //<tr><td align='center'> </td><td align='center'>1423  </td><td align='center'>利華  </td><td align='right'>0</td><td align='right'>3,000</td><td align='right'>-3,000</td></tr>
            Regex row_pattern = config.skip_tr ? new Regex(@"(.*?)</tr>") : new Regex(@"<tr.*?>(.*?)</tr>");
            MatchCollection rows = row_pattern.Matches(str_segs2[0]);

            foreach (Match match in rows)
            {
                Regex col_pattern = new Regex(@"<td.*?>(.*?)</td>");
                MatchCollection cols = col_pattern.Matches(match.Value);
                ArrayList parms = new ArrayList();

                foreach (Match match_col in cols)
                {
                    parms.Add(match_col.Groups[1].Value);
                    Logger.DbgMsg(match_col.Groups[1].Value + "  ");
                }
                Logger.DbgMsgLine("");

                if ((parms.Count == 6) || (parms.Count == 5))
                {
                    if (parms.Count == 6)
                        parms.RemoveAt(0);                      // 0 : space or *
                    DB_Add2Table(conn, parms, date);
                    //label_proc_index.Text = (string)parms[0];
                    //UI_proc_index = (string)parms[0];
                }
            }
        }
        */


        public void DB_Add2Table(MySqlConnection conn, ArrayList parms, DateTime date)
        {
            //String table_name = config.getDBTableName();

            int i;
            for (i = 0; i < parms.Count; i++)
            {
                string str = (string)parms[i];
                if (str.Equals("--") || str.Equals("") || str.Equals(" "))
                {
                    parms[i] = "0";
                }
            }
            char[] delim = new char[] { ',', ' ', '.' };
            string stock_idx = ((string)parms[0]).Trim();
            string stock_name = ((string)parms[1]).Trim();
            int buy_volume; int.TryParse(((string)parms[2]).Replace(",", ""), out buy_volume);
            int sell_volume; int.TryParse(((string)parms[3]).Replace(",", ""), out sell_volume);
            int total_volume; int.TryParse(((string)parms[4]).Replace(",", ""), out total_volume);


            /*if (true)    //check duplicate entry //to do: use date as primary key
            {

                string sql = "SELECT COUNT(*) FROM " + table_name +
                                " where trans_date=\"" + Util.convertDate2mysql(date) + "\" AND stock_index=\"" + stock_idx + "\";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int r = Convert.ToInt32(result);
                    if (r > 0)
                    {
                        Debug.WriteLine(table_name + ":" + stock_idx + ":" + date.ToShortDateString() + "already exist(" + r + ")");
                        return;
                    }
                }
            }*/

            string sql_ins = "INSERT INTO " + table_name + "(juristic_type, trans_date, stock_index, stock_name, buy_volume, sell_volume, total_volume) VALUE(" +
                                                            config.juristic_index.ToString() + ", " +
                                                            "'" + Util.convertDate2mysql(date) + "'" + "," + "\'" + stock_idx + "\'" + "," + "\'" + stock_name +
                                                            "\'" + "," + buy_volume.ToString() + "," +
                                                            sell_volume.ToString() + "," + total_volume.ToString() + ");";
            MySqlCommand cmd_insert = new MySqlCommand(sql_ins, conn);
            try
            {
                cmd_insert.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            int a = 0;
        }

        public DateTime getLastDBDate(DBManager db)
        {
            DateTime res = new DateTime(2004, 12, 18);        //first date
            /*string stock_db_name = config.dbTableName;

            String qstr = "SHOW TABLES LIKE '" + stock_db_name + "';";
            MySqlCommand cmd = new MySqlCommand(qstr, db.getConnection());
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows == false)
            {
                rdr.Close();
                return res;
            }
            rdr.Close();*/
            String qstr = "SELECT * from " + table_name + "  ORDER by trans_date DESC LIMIT 1;";
            MySqlCommand cmd = new MySqlCommand(qstr, db.getConnection());
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                res = (DateTime)rdr["trans_date"];
            }
            rdr.Close();
            return res;
        }

        public void SetupDatebase(MySqlConnection conn)
        {
            try
            {
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + table_name + " (" +
                                        "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "juristic_type  INT, " +
                                        "trans_date     DATE, " +
                                        "stock_index   VARCHAR(12), " +
                                        //"stock_name   VARCHAR(12). "+
                                        "stock_name   VARCHAR(12) CHARACTER SET big5, " +
                                        "buy_volume   INT, " +
                                        "sell_volume    INT, " +
                                        "total_volume   INT) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public JuristicTradeInfo[] GetStockTradeHistory(MySqlConnection conn, int type, int count)
        {
            // DateTime res = new DateTime(2010, 10, 24);        //first date

            ArrayList res = new ArrayList();

            try
            {
                String qstr = "SELECT * from " + table_name + "  where juristic_type=" + String.Format("{0}",type) +
                                " AND DATE_ADD(trans_date,INTERVAL 20 DAY) > DATE(NOW()) ORDER by trans_date DESC;";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(new JuristicTradeInfo(rdr));
                }

                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
            return (JuristicTradeInfo[])res.ToArray(typeof(JuristicTradeInfo));
        }

        /*public JuristicTradeInfo[] GetStockTradeHistory(MySqlConnection conn, String idx, int type, int count)
        {
            // DateTime res = new DateTime(2010, 10, 24);        //first date

            ArrayList res = new ArrayList();

            try
            {
                String qstr = "SELECT * from " + table_name + "  where juristic_type=" + String.Format("{0}", type) +
                                " AND stock_index=" + idx + ", " +
                                " AND DATE_ADD(trans_date,INTERVAL 20 DAY) > DATE(NOW()) ORDER by trans_date DESC;";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(new JuristicTradeInfo(rdr));
                }

                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
            return (JuristicTradeInfo[])res.ToArray(typeof(JuristicTradeInfo));
        }*/

        public override void DBSave(DBManager sql_db, ArrayList stock_data)
        {
            sql_db.StockData_SaveForeign(stock_data);
        }
    }
}
