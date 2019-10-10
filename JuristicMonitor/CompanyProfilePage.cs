using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using MySql.Data.MySqlClient;

namespace JuristicMonitor
{
    class CompanyProfilePage
    {
        String companyProfile_db_name = "company_profile";
        String companyEarning_db_name = "company_earning";  //seasonly eps, profit ration
        String companyRevenue_db_name = "company_revenue";  //monthly revenue

        public CompanyData DownloadInfo(String stock_index)
        {
            CompanyData res = new CompanyData();
            String url = @"http://mops.twse.com.tw/mops/web/t05st03";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String data_src = "encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&TYPEK=all&co_id=" + stock_index; //2379";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    Elements table01_trs = doc.Select("#table01 .hasBorder tr");
                    if (table01_trs != null)
                    {
                        foreach (Element tr in table01_trs)
                        {
                            Elements ch = tr.Children;
                            int i;
                            for (i = 0; i < ch.Count; i++)
                            {
                                if (ch[i].Tag.ToString().Equals("th"))
                                {
                                    String name = ch[i].Text();
                                    String value = "";
                                    if ((i + 1) < ch.Count) //still more than 1 element behind
                                    {
                                        i++;
                                        if (ch[i].Tag.ToString().Equals("td"))
                                        {
                                            value = ch[i].Text();
                                            if (name.Equals("實收資本額"))
                                            {
                                                res.capital = value;
                                            }
                                            else if (name.Equals("公司名稱"))
                                            {
                                                res.FullName = value;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Logger.e("DownloadInfo capital:" + stock_index + "  failed");
            }
            return res;
        }

        /*public void DownloadEarningInfoAll(MySqlConnection conn, String stock_index)
        {
            DownloadEarningInfo(conn, @"http://mops.twse.com.tw/mops/web/t163sb08", stock_index, year);   //year 102-
            
        }*/

        public void DownloadEarningInfo_beforeIFRS(MySqlConnection conn, String stock_index, List<YearSeasonMonth> esp2download)
        {
            //DownloadEarningInfo(conn, @"http://mops.twse.com.tw/mops/web/t05st24", stock_index, 0);    //year 98-101
            DownloadEarningInfo(conn, stock_index, 0, esp2download);
        }

        public Boolean after_ifrs(int year)
        {
            return (year >= 102);
        }

        public void DownloadEarningInfo(MySqlConnection conn, String stock_index, int year, List<YearSeasonMonth> esp2download)
        {
            CompanyData res = new CompanyData();
            String url = after_ifrs(year) ? @"http://mops.twse.com.tw/mops/web/t163sb08" : @"http://mops.twse.com.tw/mops/web/t05st24";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String data_src = "encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&t05st29_c_ifrs=N&t05st30_c_ifrs=N&TYPEK=all&isnew=true&co_id=" + stock_index + "&year="+ String.Format("{0}", year);
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    Elements table01_trs = doc.Select("table .hasBorder tr");
                    if (table01_trs != null)
                    {
                        String year_str = "";
                        int season = 0;
                        //foreach (Element tr in table01_trs)
                        for(int row=1; row<table01_trs.Count ; row++)
                        {
                            Element tr = table01_trs[row];
                            Elements tds = tr.Select("td");
                            
                            int td_idx = 0;
                            
                            if (tds.Count == 7)     
                            {//first td is year
                                String td0str = tds[0].Text();
                                year_str = td0str.Split(new char[] { ' ' })[0];
                                td_idx = 2;
                                season = 1;
                            }
                            else if (tds.Count == 6)
                            {
                                td_idx = 1;
                            }
                            else if (tds.Count == 5)    //new format
                            {
                                Elements ths = tr.Select("th");
                                if (ths.Count > 1)
                                {   //first season
                                    String yesrstr = ths[0].Text();
                                    year_str = yesrstr.Split(new char[] { '年' })[0];
                                    season = 1;
                                }
                                
                            }

                            
                            String[] info = new String[5];
                            for(int i=0;i<5;i++)
                                info[i] = tds[td_idx++].Text();

                            info[0] = Util.numberString2IntString(info[0]);     // 123,456.78  -> 123456
                            //String[] segs = info[0].Split(new char[]{'.'});     // 123,456.78  -> 123456
                            //info[0] = segs[0].Replace(",", "");

                            //to do : check if duplicate
                            int year_int = int.Parse(year_str);
                            if (GetSeasonEarning(conn, stock_index, year_int, season) == null)
                            {
                                string sql_ins = "INSERT INTO " + companyEarning_db_name +
                                                "(stock_index, year, season, revenue, grossProfitMargin, operatingProfitMargin, netProfitMarginBeforeTax, netProfitMargin ) VALUE(" +
                                                "'" + stock_index + "'," + year_str + "," + String.Format("{0}", season) + "," + info[0] + "," + info[1] + "," + info[2] + "," + info[3] + "," + info[4] + " );";
                                MySqlCommand cmd_insert = new MySqlCommand(sql_ins, conn);
                                try
                                {
                                    cmd_insert.ExecuteNonQuery();
                                    Logger.v("add profitMargin: " + stock_index + "(" + year_str + "," + String.Format("{0}", season) + "):" + info[0] + "," + info[1] + "," + info[2] + "," + info[3] + "," + info[4]);
                                    esp2download.Add(new YearSeasonMonth(year_int, season));
                                }
                                catch (MySql.Data.MySqlClient.MySqlException ex)
                                {
                                    Logger.e("DownloadEarningInfo:" + stock_index + "(" + year_str + "," + season + ")  failed");
                                }
                            }
                            season++;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
            //return res;
        }

        public YearSeasonMonth GetLatestEarningInfoDate(MySqlConnection conn, String stock_index)
        {
            YearSeasonMonth res = new YearSeasonMonth();
            string qstr = "SELECT * FROM " + companyEarning_db_name + " WHERE stock_index=\'" + stock_index + "\' ORDER by season,year DESC LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                res.year = (int)rdr["year"];
                res.season = (int)rdr["season"];
            }
            else
                res = null;
            rdr.Close();

            return res;
        }

        public EarningInfo GetSeasonEarning(MySqlConnection conn, String stock_index, int year, int season)
        {
            EarningInfo res = new EarningInfo();
            string qstr = "SELECT * FROM " + companyEarning_db_name + " WHERE stock_index=\'" + stock_index + "\' and year=" + String.Format("{0}", year) + " and season=" + String.Format("{0}", season) + ";";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = null;
            try
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    res.grossProfitMargin = rdr.GetDouble(rdr.GetOrdinal("grossProfitMargin"));
                    res.operatingProfitMargin = rdr.GetDouble(rdr.GetOrdinal("operatingProfitMargin"));
                    res.netProfitMarginBeforeTax = rdr.GetDouble(rdr.GetOrdinal("netProfitMarginBeforeTax"));
                    res.netProfitMargin = rdr.GetDouble(rdr.GetOrdinal("netProfitMargin"));
                    if( rdr.IsDBNull(rdr.GetOrdinal("eps")) == false)
                    {
                        res.eps = rdr.GetDouble(rdr.GetOrdinal("eps"));
                    }
                }
                else
                    res = null;
                
            }
            catch (Exception ex)
            {
                Logger.e("GetSeasonEarning:" + stock_index + "(" + year + "," + season + ")  failed");
            }
            finally
            {
                if(rdr!=null)
                    rdr.Close();
            }
            return res;
        }

        /*public void DownloadEpsInfo(MySqlConnection conn, String stock_index, int year, int season)
        {
            CompanyData res = new CompanyData();
            String url = (year > 101) ? @"http://mops.twse.com.tw/mops/web/t163sb01" : @"http://mops.twse.com.tw/mops/web/t56sb01n_1";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String data_src = "encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&TYPEK=all&isnew=false&co_id=" + stock_index + "&year=" + String.Format("{0}", year) + "&season=" + String.Format("{0}", season);
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    Elements table01s = doc.Select("#table01");
                    if (table01s.Count == 0)
                        return;
                    Element table01 = table01s[0];
                    Elements tables = table01.Select("table");
                    if (tables != null)
                    {
                        Element last_table = tables[tables.Count-2];    //??
                        Elements trs = last_table.Select("tr");
                        foreach (Element tr in trs)
                        {
                            Elements tds = tr.Select("td");
                            if (tds.Count >= 2)
                            {
                                if("基本每股盈餘".Equals(tds[0].Text()) )
                                {
                                    String eps_str = tds[1].Text();


                                    // create the java mysql update preparedstatement
                                    String update = "update " + companyEarning_db_name + " set eps=" + eps_str + " where stock_index=" + stock_index + " AND year=" + year + " AND season=" + season + ";" ;
                                    MySqlCommand cmd_insert = new MySqlCommand(update, conn);
                                    try
                                    {
                                        cmd_insert.ExecuteNonQuery();
                                    }
                                    catch (MySql.Data.MySqlClient.MySqlException ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
            //return res;
        }*/
        public void DownloadEpsInfo(MySqlConnection conn, String stock_index, int year, int season)
        {
            CompanyData res = new CompanyData();
            String url = (year > 101) ? @"http://mops.twse.com.tw/mops/web/t163sb01" : @"http://mops.twse.com.tw/mops/web/t56sb01n_1";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String data_src = "encodeURIComponent=1&step=1&firstin=1&off=1&keyword4=&code1=&TYPEK2=&checkbtn=&queryName=co_id&TYPEK=all&isnew=false&co_id=" + stock_index + "&year=" + String.Format("{0}", year) + "&season=" + String.Format("{0}", season);
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    //Elements table01s = doc.Select("#table01");
                    //if (table01s.Count == 0)
                    //    return;
                    //Element table01 = table01s[0];
                    Elements tables = doc.Select("table .hasBorder");
                    foreach(Element table in tables)
                    {
                        Elements trs = table.Select("tr");
                        foreach (Element tr in trs)
                        {
                            Elements tds = tr.Select("td");
                            if (tds.Count >= 2)
                            {
                                //if ("基本每股盈餘".Equals(tds[0].Text()))
                                if (tds[0].Text().StartsWith("基本每股盈餘"))
                                {
                                    String eps_str = tds[1].Text();


                                    // create the java mysql update preparedstatement
                                    String update = "update " + companyEarning_db_name + " set eps=" + eps_str + " where stock_index=" + stock_index + " AND year=" + year + " AND season=" + season + ";";
                                    MySqlCommand cmd_insert = new MySqlCommand(update, conn);
                                    try
                                    {
                                        cmd_insert.ExecuteNonQuery();
                                        Logger.v("update eps:" + stock_index + "(" + year + "," + season +") : " + eps_str);
                                    }
                                    catch (MySql.Data.MySqlClient.MySqlException ex)
                                    {
                                        Logger.e("update eps:" + stock_index + "(" + year + "," + season + ")  failed");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Logger.e("update eps:" + stock_index + "(" + year + "," + season + ")  failed");
            }
            //return res;
        }

        public bool isEPS_Null(MySqlConnection conn, String stock_index, int year, int season)
        {
            bool res = true;
            string qstr = "SELECT * FROM " + companyEarning_db_name + " WHERE stock_index=\'" + stock_index + "\' and year=" + String.Format("{0}", year) + " and season=" + String.Format("{0}", season) + ";";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = null;
            try
            {
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (rdr.IsDBNull(rdr.GetOrdinal("eps")) == false)
                    {
                        res = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.e("isEPS_Null:" + stock_index + "(" + year + "," + season + ")  failed");
            }
            finally
            {
                if (rdr != null)
                    rdr.Close();
            }
            return res;
        }

        public int GetSeasonEarningEPS(MySqlConnection conn, String stock_index, int year, int season)
        {
            int res = 0;
            string qstr = "SELECT * FROM " + companyRevenue_db_name + " WHERE stock_index=\'" + stock_index + "\' and year=" + String.Format("{0}", year) + " and season=" + String.Format("{0}", season) + ";";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                res = (int)rdr["revenue"];
            }
            rdr.Close();

            return res;
        }

        /*
        public void DownloadMonthRevenue(MySqlConnection conn, String stock_index, int year, int month)
        {
            CompanyData res = new CompanyData();
            String url = (year > 101) ? @"http://mops.twse.com.tw/mops/web/t05st10" : @"http://mops.twse.com.tw/mops/web/t05st10";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String data_src = "encodeURIComponent=1&run=Y&step=0&yearmonth=10112&colorchg=&TYPEK=sii%20&co_id=" + stock_index + "&off=1&year=" + String.Format("{0}", year) + "&month=" + String.Format("{0}", month) + "&firstin=true";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    Elements table01s = doc.Select("#table01 .hasBorder");
                    if (table01s.Count == 0)
                        return;

                    Elements trs = table01s[0].Select("tr");
                    foreach (Element tr in trs)
                    {
                        Elements ths = tr.Select("th");
                        Elements tds = tr.Select("td");
                        if ( (ths.Count >= 1) && (tds.Count >= 2) )
                        {
                            if ("本月".Equals(ths[0].Text()))
                            {
                                String revenue_str = tds[1].Text();
                                revenue_str = Util.numberString2IntString(revenue_str);     // 123,456.78  -> 123456
                                string sql_ins = "INSERT INTO " + companyRevenue_db_name +
                                            "(stock_index, year, month, revenue ) VALUE(" +
                                            "'" + stock_index + "'," + year + "," + String.Format("{0}", month) + "," + revenue_str + ");";
                                MySqlCommand cmd_insert = new MySqlCommand(sql_ins, conn);
                                try
                                {
                                    cmd_insert.ExecuteNonQuery();
                                }
                                catch (MySql.Data.MySqlClient.MySqlException ex)
                                {
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }
            //return res;
        }*/

        public void DownloadMonthRevenue(MySqlConnection conn, String stock_index, int year, int month)
        {
            CompanyData res = new CompanyData();
            String url = (year > 101) ? @"http://mops.twse.com.tw/mops/web/t05st10_ifrs" : @"http://mops.twse.com.tw/mops/web/t05st10";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            CookieContainer cookieContainer = new CookieContainer();
            //req.CookieContainer = cookieContainer;
            req.UserAgent = "Mozilla/4.0";
            req.Method = "GET";
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    HttpWebRequest req_post = (HttpWebRequest)HttpWebRequest.Create(url);
                    req_post.Method = "POST";
                    req_post.ContentType = "application/x-www-form-urlencoded";
                    String year_str = String.Format("{0}", year);
                    String month_str = String.Format("{0:00}", month);
                    String data_src = "encodeURIComponent=1&run=Y&step=0&yearmonth=" + year_str + month_str + "&colorchg=&TYPEK=sii%20&co_id=" + stock_index + "&off=1&year=" + year_str + "&month=" + month_str + "&firstin=true";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(data_src);

                    req_post.ContentLength = data.Length;

                    using (Stream stream = req_post.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    HttpWebResponse response = (HttpWebResponse)req_post.GetResponse();

                    string responsehtml = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    Document doc = NSoup.NSoupClient.Parse(responsehtml);

                    Elements table01s = doc.Select("table .hasBorder");
                    if (table01s.Count == 0)
                        return;

                    String revenue_str = null;
                    //get data from last table and last row
                    foreach (Element table in table01s)
                    {
                        Elements trs = table.Select("tr");
                        foreach (Element tr in trs)
                        {
                            Elements ths = tr.Select("th");
                            Elements tds = tr.Select("td");
                            if ((ths.Count >= 1) && (tds.Count >= 1))
                            {
                                if ("本月".Equals(ths[0].Text()))
                                {
                                    revenue_str = tds[tds.Count-1].Text();   //last row
                                }
                            }
                        }
                    }

                    if (revenue_str != null)
                    {
                        revenue_str = Util.numberString2IntString(revenue_str);     // 123,456.78  -> 123456
                        string sql_ins = "INSERT INTO " + companyRevenue_db_name +
                                    "(stock_index, year, month, revenue ) VALUE(" +
                                    "'" + stock_index + "'," + year + "," + String.Format("{0}", month) + "," + revenue_str + ");";
                        MySqlCommand cmd_insert = new MySqlCommand(sql_ins, conn);
                        try
                        {
                            cmd_insert.ExecuteNonQuery();
                            Logger.v(" Add revenue:" + stock_index + "'," + year + "," + String.Format("{0}", month) + "," + revenue_str);
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            Logger.e("DownloadMonthRevenue " + stock_index + "'," + year + "," + String.Format("{0}", month) + "failed");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
                Logger.e("DownloadMonthRevenue " + stock_index + "'," + year + "," + String.Format("{0}", month) + "failed");
            }
            //return res;
        }

        public int GetMonthRevenue(MySqlConnection conn, String stock_index, int year, int month)
        {
            int res = 0;
            string qstr = "SELECT * FROM " + companyRevenue_db_name + " WHERE stock_index=\'" + stock_index + "\' and year=" + String.Format("{0}", year) + " and month=" + String.Format("{0}", month) + ";";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                res = (int)rdr["revenue"];
            }
            rdr.Close();

            return res;
        }

        public YearSeasonMonth GetLatestRevenueInfoDate(MySqlConnection conn, String stock_index)
        {
            YearSeasonMonth res = new YearSeasonMonth();
            string qstr = "SELECT * FROM " + companyRevenue_db_name + " WHERE stock_index=\'" + stock_index + "\' ORDER by month,year DESC LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(qstr, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                res.year = (int)rdr["year"];
                res.month = (int)rdr["month"];
            }
            else
                res = null;
            rdr.Close();

            return res;
        }

        public void SetupDatebase(MySqlConnection conn)
        {
            //create companyProfile table
            try
            {
                //string sql_create_db = "CREATE TABLE AsyncSampleTable (index int)";
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + companyProfile_db_name + " (" +
                                    "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "stock_name     VARCHAR(12), " +
                                        "stock_index    VARCHAR(10), " +
                                        "capital        INT ) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("SetupEarningDatebase " + companyProfile_db_name + "failed");
            }
        }

        public void SetupEarningDatebase(MySqlConnection conn)
        {
            //create companyProfile table
            try
            {
                //season database
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + companyEarning_db_name + " (" +
                                    "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "stock_index    VARCHAR(10), " +
                                        "year           int, " +
                                        "season         int, " +
                                        "revenue        int, " +
                                        "grossProfitMargin        DECIMAL(4,2), " +
                                        "operatingProfitMargin    DECIMAL(4,2), " +
                                        "netProfitMarginBeforeTax DECIMAL(4,2), " +
                                        "netProfitMargin          DECIMAL(4,2), " +
                                        "eps                      DECIMAL(4,2)  ) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("SetupEarningDatebase " + companyEarning_db_name + "failed");
            }

            try
            {
                //month database
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + companyRevenue_db_name + " (" +
                                    "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "stock_index    VARCHAR(10), " +
                                        "year           int, " +
                                        "month         int, " +
                                        "revenue        int  ) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("SetupEarningDatebase " + companyRevenue_db_name + "failed");
            }
        }

        public bool isExistCompanyProfile(MySqlConnection conn, String stock_index)
        {
            try
            {
                string sql = "select COUNT(*) from " + companyProfile_db_name + " where stock_index=" + stock_index + ";";
                MySqlCommand cmd_create = new MySqlCommand(sql, conn);
                int count = Convert.ToInt32(cmd_create.ExecuteScalar().ToString());

                return (count != 0);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("isExistCompanyProfile " + stock_index + "failed");
            }
            return false;
        }

        public void AddCompanyProfile(MySqlConnection conn, String stock_index, String stock_name, int capital)
        {
            try
            {
                string sql = "INSERT INTO " + companyProfile_db_name + "(stock_name, stock_index,capital) VALUE(" +
                                 "'" + stock_name + "','" + stock_index + "'," + capital.ToString() + ");";
                MySqlCommand cmd_insert = new MySqlCommand(sql, conn);
                cmd_insert.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("AddCompanyProfile " + stock_index + "failed");
            }
        }

        public void UpdateCompanyProfile(MySqlConnection conn, String stock_index, String stock_name, int capital)
        {
            try
            {
                string sql = "update " + companyProfile_db_name + " set capital=" + capital.ToString() + ",stock_name=\'" + stock_name + "\'  where stock_index=" + stock_index + ";";

                MySqlCommand cmd_update = new MySqlCommand(sql, conn);
                cmd_update.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("UpdateCompanyProfile " + stock_index + "failed");
            }
        }

        public String GetStockChineseName(MySqlConnection conn, String stock_index)
        {
            String res = "";

            String qstr = "select * from " + companyProfile_db_name + " where stock_index=\'" + stock_index + "\';";

            try{
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    res = (String)rdr["stock_name"];
                }

                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("GetStockChineseName " + stock_index + "failed");
            }

            return res;
        }

        public int GetStockCapital(MySqlConnection conn, String stock_index)
        {
            int res = 0;

            String qstr = "select * from " + companyProfile_db_name + " where stock_index=\'" + stock_index + "\';";

            try
            {
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    res = (int)rdr["capital"];
                }

                rdr.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Logger.e("GetStockCapital " + stock_index + "failed");
            }

            return res;
        }
    }

    public class CompanyData
    {
        public String FullName;
        public String capital;

        public CompanyData()
        {
            FullName = "";
            capital = "";
        }
    }

}
