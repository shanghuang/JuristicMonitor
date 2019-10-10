using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
/*using NSoup;
using NSoup.Nodes;
using NSoup.Select;
*/
/*using AngleSharp;
using AngleSharp.Dom;
*/
using HtmlAgilityPack;
using MongoDB.Bson;

using System.Diagnostics;

namespace JuristicMonitor
{
    public class PageFinancialReport
    {
        public String getPageUrl()
        {
            //return @"https://mops.twse.com.tw/mops/web/ajax_t163sb01";
            return @"https://mops.twse.com.tw/mops/web/ajax_t164sb03";
        }

        public String getFileName(String stock_index, int year, int season)
        {
            String path = string.Format(@"FinReport_{0}_{1:D4}_{2:D2}.html", stock_index, year, season);
            return path;
        }

        /*
encodeURIComponent: 1
step: 1
firstin: 1
off: 1
keyword4: 
code1: 
TYPEK2: 
checkbtn: 
queryName: co_id
inpuType: co_id
TYPEK: all
isnew: false
co_id: 2379
year: 106
season: 01
         */

        public string download_page(string url, String stock_index, int year, int season)
        {
            string strResult = string.Empty;
            WebResponse objResponse;
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
            Dictionary<string, string> postParameters = new Dictionary<string, string>();
            postParameters.Add("encodeURIComponent", "1");
            postParameters.Add("step", "1");
            postParameters.Add("firstin", "1");
            postParameters.Add("off", "1");
            postParameters.Add("keyword4", "");
            postParameters.Add("code1", "");
            postParameters.Add("TYPEK2", "");
            postParameters.Add("checkbtn", "");

            postParameters.Add("queryName", "co_id");
            postParameters.Add("inpuType", "co_id");
            postParameters.Add("TYPEK", "all");
            postParameters.Add("isnew", "all");
            postParameters.Add("co_id", stock_index);
            postParameters.Add("year", year.ToString());
            postParameters.Add("season", season.ToString());

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

        public BsonDocument ParseHtml( string source)
        {
            int[] subtable_start = new int[] { 4, 14, 26, 38, 46, 49, 54, 59 };

            //Dictionary<string, string> data = new Dictionary<string, string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            HtmlNodeCollection tables = htmlDoc.DocumentNode.SelectNodes("//table");

            if ( (tables == null) || (tables.Count < 2))
                return new BsonDocument();

            BsonDocument financial = new BsonDocument();

            for (int table_index = 1; table_index < tables.Count; table_index++)
            {
                HtmlNodeCollection trs = tables[table_index].SelectNodes("tr");

                String subtable_key = "";
                BsonDocument section = new BsonDocument();
                for (int tr_index = 0; tr_index < trs.Count; tr_index++)
                {

                    HtmlNodeCollection tds = trs[tr_index].SelectNodes("td");
                    if (tds == null)
                        continue;
                    if (subtable_start.Contains(tr_index))
                    {
                        if (tr_index > 4)
                        {
                            financial.Add(subtable_key, section);
                        }
                        subtable_key = tds[0].InnerText.Replace(":", "").TrimStart();
                    }
                    else
                    {
                        String name = tds[0].InnerText.Replace(":", "").TrimStart();
                        String value = tds[1].InnerText.Replace(",", "").Trim();
                        section.Add(name, value);
                    }
                    Debug.Write(table_index.ToString() + " " + tr_index.ToString() + " ");

                    for (int td_index = 0; td_index < tds.Count; td_index++)
                    {
                        Debug.Write(tds[td_index].InnerText + " ");
                    }

                    Debug.WriteLine("");
                }
                financial.Add(subtable_key, section);
            }
            return financial;
        }

        public void DBSave(BsonDocument financial, String stock_index, int year, int season)
        {
            DbMango db = new DbMango();
            db.connect();
            db.FinancialReport_save(financial, stock_index, year,  season);
        }

        public DateTime getLastDBDate(DbMango db, String stock_index)
        {
            DateTime res = new DateTime(2014, 1, 1);

            return res;
        }
    }
}

