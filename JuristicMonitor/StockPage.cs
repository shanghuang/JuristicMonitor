using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using NSoup;
using NSoup.Nodes;
using NSoup.Select;

using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace JuristicMonitor
{
    public class StockPage : EquiltyPage
    {
        string stock_db_name = "tw_stock";
        DateTime format_2015_start = new DateTime(2015, 1, 1);

        public StockPage() : base("stock_")
        {

        }

        public override String getPageUrl(DateTime date)
        {
            string url_string = @"https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date=" +
                string.Format("{0:D4}{1:D2}{2:D2}", date.Year, date.Month, date.Day) +
                @"&type=ALLBUT0999&_=1568988527622";
            return url_string;
            /*if (date.CompareTo(format_2015_start)>0)
                return @"http://www.twse.com.tw/ch/trading/exchange/MI_INDEX/MI_INDEX.php";

            int tw_year = date.Year - 1911;

            string url_string = @"http://www.twse.com.tw/ch/trading/exchange/MI_INDEX/genpage/Report" +
                                        date.Year.ToString() +
                                        string.Format("{0:D2}", date.Month) +
                                        @"/A112" +
                                        string.Format("{0:D4}{1:D2}{2:D2}", date.Year, date.Month, date.Day) +
                                        @"ALLBUT0999_1.php?select2=ALLBUT0999&chk_date=" +
                                        string.Format("{0:D2}/{1:D2}/{2:D2}", tw_year, date.Month, date.Day);
            return url_string;*/
        }

        public void ParseHtml_2015(MySqlConnection conn, string source, DateTime date)
        {
            Document doc = NSoup.NSoupClient.Parse(source);
            Elements tables = doc.Select("table");
            if (tables.Count < 2)
                return;

            Elements table_tr = tables[1].Select("tbody tr");

            foreach (Element tr in table_tr)
            {
                Elements tds = tr.Select("td");
                ArrayList parms = new ArrayList();
                int index = 0;
                foreach(Element td in tds)
                {
                    parms.Add(td.Text());
                    index++;
                }

                if (parms.Count == 16)
                {
                    String sign = (String)parms[9];
                    parms.RemoveAt(9);
                    if (sign.Contains("-"))
                    {
                        parms[9] = "-" + parms[9];
                    }
                    //UI_proc_stock_num = ((string)parms[0]) + ((string)parms[1]);
                    DB_Add2Table(conn, parms, date);
                }

                /*String stock_idx = (String)parms[1];
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
                 * */
            }
        }


        public override ArrayList ParseHtml(MySqlConnection conn, string source, DateTime date)
        {
            ArrayList stocks = new ArrayList();

            if( !IsValidJson(source) )
            {
                return stocks;
            }

            JObject obj = JObject.Parse(source);

            if(obj==null || obj["data9"]==null)
            {
                //no stock data
                return stocks;
            }
            
            foreach (JArray stk in obj["data9"])
            {
                //String stock_str = stk.ToString();
                //stock_str = stock_str.Substring(1, stock_str.Length - 2);   //remove []
                JToken[] token_ary = stk.ToArray();
                String[] stk_strings = new string[token_ary.Length];
                for (int i = 0; i < token_ary.Length; i++)
                    stk_strings[i] = token_ary[i].ToString();

                StockTradeInfo stock_info = new StockTradeInfo(stk_strings,date);
                stocks.Add(stock_info);
            }
            return stocks;
        }

        /*public void ParseHtml(MySqlConnection conn, string source, DateTime date)
        {
            string[] sp_str1 = { @"<!-- REPORT START -->" };
            string[] str_segs = source.Split(sp_str1, StringSplitOptions.None);

            if (str_segs.Length <= 1)
            {  // 'REPORT START' not found
                ParseHtml_2015(conn, source, date);
                return;
            }

            DateTime y2006_first_day = new DateTime(2006, 1, 1);

            Logger.v("Parsing html : (" + date.ToShortDateString() + ")," + getFileName(date));

            if (str_segs.Length > 1)
            {
                //richTextBox2.Text = str_segs[1];

                string[] sp_str2 = { @"<table" };
                string[] strarys = str_segs[1].Split(sp_str2, StringSplitOptions.None);

                //richTextBox3.Text = "Parse stock data " + strarys[0];
                // ParseHtmlTable(strarys[0]);
                if ((date >= y2006_first_day) && (strarys.Length > 3))
                {
                    ParseHtmlTable(conn, strarys[3], date);
                }
                else
                {
                    ParseHtmlTable(conn, strarys[2], date);
                }
                //richTextBox4.Text = strarys[1];
                //int a = 1;
            }
        }
        */

        void ParseHtmlTable(MySqlConnection conn, string source, DateTime date)
        {
            Regex row_pattern = new Regex(@"<tr.*?>(.*?)</tr>");
            MatchCollection rows = row_pattern.Matches(source);

            foreach (Match match in rows)
            {
                Regex col_pattern = new Regex(@"<td.*?>(.*?)</td>");
                MatchCollection cols = col_pattern.Matches(match.Value);
                ArrayList parms = new ArrayList();

                foreach (Match match_col in cols)
                {
                    String ext_val = string.Empty;
                    Regex div_pattern = new Regex(@"<div.*?>(.*?)</div>");
                    Match divs = div_pattern.Match(match_col.Groups[1].Value);
                    if (divs.Success)
                        ext_val = divs.Groups[1].Value;
                    else
                        ext_val = match_col.Groups[1].Value;

                    //string val = match_col.Groups[1].Value;
                    //string val = ext_val;
                    bool sign_ext = false;
                    string sign_str = string.Empty;
                    if (ext_val.Contains("<"))
                    {
                        Regex sign_pattern = new Regex(@"<.*?><.*?>(.*?)<.*>");
                        Match sign = sign_pattern.Match(ext_val);
                        if (sign.Success)
                        {
                            Logger.DbgMsg(sign.Groups[1].Value + " ");
                            sign_ext = true;
                            sign_str = sign.Groups[1].Value;
                            continue;
                        }
                    }
                    //Debug.Write(match_col.Value + " ");
                    Logger.DbgMsg(ext_val + " ");
                    parms.Add(sign_ext ? (sign_str + ext_val) : ext_val);
                }

                Logger.DbgMsgLine("");


                if (parms.Count == 15)
                {
                    //UI_proc_stock_num = ((string)parms[0]) + ((string)parms[1]);
                    DB_Add2Table(conn, parms, date);
                }
            }
        }

        //0         1           2       3       4       5       6       7   8                   9           10      11              12             13       14
        //證券代號 證券名稱 成交股數 成交筆數 成交金額 開盤價 最高價 最低價 收盤價 漲跌(+/-) 漲跌價差 最後揭示買價 最後揭示買量 最後揭示賣價 最後揭示賣量 本益比 
        public void DB_Add2Table(MySqlConnection conn, ArrayList parms, DateTime date)
        {
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

            int trans_volume; int.TryParse(((string)parms[2]).Replace(",", ""), out trans_volume);
            int trans_count; int.TryParse(((string)parms[3]).Replace(",", ""), out trans_count);
            Int64 trans_value; Int64.TryParse(((string)parms[4]).Replace(",", ""), out trans_value);
            double open; double.TryParse(((string)parms[5]).Replace(",", ""), out open);
            double high; double.TryParse(((string)parms[6]).Replace(",", ""), out high);
            double low; double.TryParse(((string)parms[7]).Replace(",", ""), out low);
            double close; double.TryParse(((string)parms[8]).Replace(",", ""), out close);
            double diff; double.TryParse(((string)parms[9]).Replace(",", ""), out diff);
            double close_buy_price; double.TryParse(((string)parms[10]).Replace(",", ""), out close_buy_price);
            int close_buy_volume; int.TryParse(((string)parms[11]).Replace(",", ""), out close_buy_volume);
            double close_sell_price; double.TryParse(((string)parms[12]).Replace(",", ""), out close_sell_price);
            int close_sell_volume; int.TryParse(((string)parms[13]).Replace(",", ""), out close_sell_volume);
            double pe_ratio; double.TryParse(((string)parms[14]).Replace(",", ""), out pe_ratio);

            

            
            //}
            /*if (true)    //check duplicate entry //to do: use date as primary key
            {
                string sql = "SELECT COUNT(*) FROM " + stock_db_name +
                                " where trans_date=\"" + Util.convertDate2mysql(date) + "\";";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int r = Convert.ToInt32(result);
                    if (r > 0)
                    {
                        Debug.WriteLine(stock_db_name + ":" + date.ToShortDateString() + "already exist(" + r + ")");
                        return;
                    }
                }
            }*/

            string sql_ins = "INSERT INTO " + stock_db_name + "(stock_name, stock_index, trans_date, trans_volume, trans_count, trans_value, open, high, low ," +
                                                            "close, diff, close_buy_price, close_buy_volume, close_sell_price, close_sell_volume, pe_ratio) VALUE(" +
                                                            "'" + stock_name + "','" + stock_idx + "'," +
                                                            "'" + Util.convertDate2mysql(date) + "'" + "," + trans_volume.ToString() + "," + trans_count.ToString() + "," + trans_value.ToString() + "," +
                                                           Util.double2dec(open, 6, 2) + "," + Util.double2dec(high, 6, 2) + "," + Util.double2dec(low, 5, 2) + "," + Util.double2dec(close, 5, 2) + "," +
                                                            Util.double2dec(diff, 5, 2) + "," + Util.double2dec(close_buy_price, 5, 2) + "," + close_buy_volume.ToString() + "," +
                                                            Util.double2dec(close_sell_price, 5, 2) + "," + close_sell_volume.ToString() + "," + Util.double2dec(pe_ratio, 5, 2) + ");";
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

        static public String StockDbName(String stock_index)
        {
            return "s" + stock_index;
        }

        public DateTime getLastDBDate(DBManager db, String stock_index)
        {
            DateTime res = new DateTime();        //first date
            //string stock_idx = "1101";  //"台泥"
            string stock_db_name = StockDbName(stock_index);

            try
            {
                String qstr = "SELECT * from " + stock_db_name + "  ORDER by trans_date DESC;";
                MySqlCommand cmd = new MySqlCommand(qstr, db.getConnection());
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    res = (DateTime)rdr["trans_date"];
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
            return res;
        }



        public void SetupDatebase(MySqlConnection conn)//, String stock_index)
        {
            String[] stocks = new string[] {"2370","2371", "2372", "2373", "2374", "2375", "2376", "2377", "2378", "2379" };

            foreach (String stock_index in stocks)
            {
                try
                {
                    //string sql_create_db = "CREATE TABLE AsyncSampleTable (index int)";
                    string sql_create_db = "CREATE TABLE IF NOT EXISTS " + "S" + stock_index + " (" +
                                        "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                            "trans_date     DATE, " +
                                            "trans_volume   INT, " +
                                            "trans_count    INT, " +
                                            "trans_value    INT(64), " +
                                            "open           DECIMAL(6,2), " +
                                            "high           DECIMAL(6,2), " +
                                            "low            DECIMAL(6,2), " +
                                            "close          DECIMAL(6,2), " +
                                            "diff           DECIMAL(6,2), " +
                                            "close_buy_price    DECIMAL(6,2), " +
                                            "close_buy_volume   INT, " +
                                            "close_sell_price   DECIMAL(6,2), " +
                                            "close_sell_volume  INT, " +
                                            "foreign_buy    INT, " +
                                            "foreign_sell    INT, " +
                                            "foreign_total    INT " +
                                            ") ";

                    MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                    cmd_create.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                    //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public StockTradeInfo[] GetStockTradeHistory(MySqlConnection conn, String StockNumber, int count)
        {
           // DateTime res = new DateTime(2010, 10, 24);        //first date

            ArrayList res = new ArrayList();

            try
            {
                String qstr = "SELECT * from " + stock_db_name + "  where stock_index=" + StockNumber + " ORDER by trans_date DESC LIMIT " + String.Format("{0}", count) + ";";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    res.Add(new StockTradeInfo(rdr));
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
            return (StockTradeInfo[])res.ToArray(typeof(StockTradeInfo));
        }

        public float getLastClosePrice(MySqlConnection conn, String stock_index)
        {
            float res = 0;

            try
            {
                String qstr = "SELECT * from " + stock_db_name + "  where stock_index=" + stock_index + " ORDER by trans_date DESC LIMIT 1;";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    res = float.Parse(rdr["close"].ToString());
                }

                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return res;
        }

        public Hashtable getLastClosePriceTable(MySqlConnection conn)
        {
            float res = 0;
            Hashtable lsstPriceMap = new Hashtable();

            try
            {
                DateTime last_date = new DateTime();
                String qstr = "SELECT * from " + stock_db_name + " ORDER by trans_date DESC LIMIT 1;";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    last_date = (DateTime)rdr["trans_date"];
                }
                rdr.Close();

                String qallstr = "SELECT * from " + stock_db_name + " where trans_date=\'" + Util.convertDate2mysql(last_date) + "\';";
                MySqlCommand qallcmd = new MySqlCommand(qallstr, conn);
                rdr = qallcmd.ExecuteReader();
                while (rdr.Read())
                {
                    String idx = (String)rdr["stock_index"];
                    float price = float.Parse(rdr["close"].ToString());
                    lsstPriceMap[idx] = price;
                }
                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lsstPriceMap;
        }

        /*
        private string download_page(string url)
        {
            DateTime format_2015_start = new DateTime(2015, 1, 1);

            string strResult = string.Empty;

            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);

            try
            {
                objResponse = objRequest.GetResponse();

                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), Encoding.GetEncoding("big5"))) //, Encoding.Unicode
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            return strResult;
        }*/

        //public String download_2015(DateTime date, String TargetDirectory)
        public override string download_page(string url,DateTime date)
        {

            //DateTime format_2015_start = new DateTime(2015, 1, 1);

            string strResult = string.Empty;

            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);

            try
            {
                objResponse = objRequest.GetResponse();

                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"))) //, Encoding.Unicode
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            return strResult;
        }

        /*public override string download_page(string url, DateTime date)
        {

            string strResult = string.Empty;

            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);

            if (date.CompareTo(format_2015_start) > 0)
            {//later than
                Dictionary<string, string> postParameters = new Dictionary<string, string>();
                postParameters.Add("selectType", "ALLBUT0999");
                postParameters.Add("qdate", String.Format("{0}/{1:d2}/{2:d2}", date.Year - 1911, date.Month, date.Day));
                postParameters.Add("download", "");

                String postData = "";
                Boolean first = true;
                foreach (string key in postParameters.Keys)
                {
                    if (first)
                        first = false;
                    else
                        postData += "&";

                    postData += HttpUtility.UrlEncode(key) + "="
                          + HttpUtility.UrlEncode(postParameters[key]);

                }

                byte[] data = Encoding.ASCII.GetBytes(postData);
                objRequest.Method = "POST";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                objRequest.ContentLength = data.Length;

                Stream requestStream = objRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            try
            {
                objResponse = objRequest.GetResponse();

                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"))) //, Encoding.Unicode
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            return strResult;
        }*/

        public override void DBSave(DBManager sql_db, ArrayList stock_data)
        {
            sql_db.StockData_Save(stock_data);
        }
    }

    
}
