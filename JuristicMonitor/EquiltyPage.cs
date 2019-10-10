using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Collections;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;


namespace JuristicMonitor
{
    abstract public class EquiltyPage
    {
        String fileNameStart;

        public EquiltyPage(String fn)
        {
            fileNameStart = fn;
        }

        public abstract String getPageUrl(DateTime date);//{ return "";}

        public String getFilePrefix()
        {
            return fileNameStart;
        }

        public String getFileName(DateTime date)
        {
            String path = string.Format(@"\{0}{1:D4}_{2:D2}_{3:D2}.html", fileNameStart, date.Year, date.Month, date.Day);
            return path;
        }

        public DateTime pageFileNameToDate(string fn)
        {
            String str_date = fn.Substring(fileNameStart.Length, "2000_01_01".Length);
            DateTime date = new DateTime(1900, 1, 1);
            String[] segs = str_date.Split(new char[] { '_' });
            if (segs.Length < 3)
                return date;

            try
            {
                int year = int.Parse(segs[0]);
                int month = int.Parse(segs[1]);
                int day = int.Parse(segs[2]);
                date = new DateTime(year, month, day);
            }
            catch (Exception e)
            {
            }
            return date;
        }

        public bool download_html(DateTime date, string target_dir)
        {
            string url_string = getPageUrl(date);
            string save_file_name = target_dir+getFileName(date);

            Debug.WriteLine(save_file_name);
            Logger.v("downloading html : (" + date.ToShortDateString() + ")," + save_file_name);

            if (!File.Exists(save_file_name))
            {
                Debug.WriteLine("downloading " + url_string);
                string res = download_page(url_string, date);
                if (res.Equals(string.Empty))
                {
                    Debug.WriteLine("download page failed::" + url_string);
                    return false;
                }

                using (StreamWriter sw = new StreamWriter(save_file_name, false, Encoding.GetEncoding("utf-8")))
                {
                    sw.Write(res);
                    sw.Close();
                }
            }

            return true;
        }

        virtual public string download_page(string url,DateTime date)
        {
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

        public static bool IsValidJson(string value)
        {
            try
            {
                var json = JContainer.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
        abstract public ArrayList ParseHtml(MySqlConnection conn, string source, DateTime date);
        abstract public void DBSave(DBManager sql_db, ArrayList stock_data);
    }
}
