using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections;
using MongoDB.Bson;

namespace JuristicMonitor
{
    

    public class StockDownloadManager
    {
        private String TargetDirectory = @"C:\htmldwl";
        private DateTime StartDate = new DateTime(2015,1,4);
        private String current_download_status;
        private Boolean checkDownload = false;

        void DownloadManager()
        {
            
        }

        public void setTargetDir(String td)
        {
            TargetDirectory = td;
        }

        public void setCheckDownload()
        {
            checkDownload = true;
        }

        public void Download_PerDay(DateTime date)
        {
            dynamic[] equility_pages = new dynamic[] { new StockPage(), new PageJuristic("foreign_") };//, new PageJuristic("selfoper_"), new PageJuristic("investtrust_"), };

            foreach (dynamic page in equility_pages)
            {
                current_download_status = "Downloading " + page.getFilePrefix() + date.ToShortDateString();

                page.download_html(date, TargetDirectory);
                Thread.Sleep(5000);
            }

        }

        public void DownloadToLatest()
        {
            BatchDownloadStockTrading();
            //BatchDownloadFinancialReport();
        }
            /*dynamic[] equility_pages = new dynamic[] { new StockPage(), new PageJuristic("foreign_"), new PageJuristic("selfoper_"), new PageJuristic("investtrust_"), };

            DateTime date = checkDownload ? StartDate : getLastUpdateDate();
            while (!date.Equals(DateTime.Today))
            {
                date = date.AddDays(1);
                Logger.v("Downloading data starting from "+ String.Format("{0}", date) ); 
                //ui_date = date;
                foreach (dynamic page in equility_pages)
                {
                    current_download_status = "Downloading " + page.getFilePrefix() + date.ToShortDateString();
                    
                    page.download_html(date, TargetDirectory);
                }
            }*/

        public void BatchDownloadStockTrading() { 
            dynamic[] equility_pages = new dynamic[] { new StockPage(), new PageJuristic("foreign_") };
            DBManager sql_db = new DBManager();
            sql_db.DBConnect();

            //DateTime date = checkDownload ? StartDate : equility_pages[0].getLastDBDate(sql_db, "2379");
            DateTime date = equility_pages[0].getLastDBDate(sql_db, "2379");
            date = date.CompareTo(StartDate) > 0 ? date : StartDate;

            while (!date.Equals(DateTime.Today))
            {
                date = date.AddDays(1);
                Logger.v("Downloading data starting from " + String.Format("{0}", date));
                if (!TradingDays.IsTradingDay(date))
                    continue;
                Download_PerDay(date);
                foreach (dynamic page in equility_pages)
                {
                    current_download_status = "Downloading " + page.getFilePrefix() + date.ToShortDateString();
                    using (StreamReader sr = new StreamReader(TargetDirectory + page.getFileName(date), Encoding.GetEncoding("utf-8")))
                    {
                        ArrayList stock_data = page.ParseHtml(sql_db.getConnection(), sr.ReadToEnd(), date);
                        if (stock_data.Count > 0)
                        {
                            page.DBSave(sql_db, stock_data);
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }

        /*
         * Season start with 1.
         * .Seasonal report is available 45 days after the season
         */
        public bool SeasonalReportReady(int year, int season)
        {
            int avail_year, avail_month;
            if( season == 4)
            {
                avail_year = year + 1;
                avail_month = 2;
            }
            else
            {
                avail_year = year;
                avail_month = (season)*3 + 2;
            }
            DateTime avail = new DateTime(avail_year+1911, avail_month, 16);

            return avail.CompareTo(DateTime.Now) < 0;

        }

        public void BatchDownloadFinancialReport()
        {
            DbMango mongo = new DbMango();
            mongo.connect();

            String[] stocks = new string[] {/*"2371",*/ "2373", "2374", "2375", "2376", "2377", "2379" };
            foreach (String stock in stocks)
            {
                YearSeason yearseason = mongo.FinancialReport_FindLatest(stock);
                yearseason = (yearseason == null) ? new YearSeason(104, 1) : yearseason.next();
                for (int year = 104; year <= 108; year++)
                    for (int season = 1; season <= 4; season++)
                    {
                        if (SeasonalReportReady(year, season))
                        {
                            int try_count = 1;
                            Boolean download_success = false;
                            do
                            {
                                download_success = DownloadSeasonalReport(stock, year, season);
                                Thread.Sleep(1000 * try_count);
                            } while (!download_success && (try_count < 10));
                        }
                    }
            }
        }

        public Boolean DownloadSeasonalReport(String stock_index, int year, int season)
        {
            String srcDir = @"C:\Temp\htmldwl\";
            Boolean result = false;
            PageFinancialReport page = new PageFinancialReport();

            String path = srcDir + page.getFileName(stock_index, year, season);
            if (File.Exists(path) == false)
            {
                String content = page.download_page(page.getPageUrl(), stock_index, year, season);
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8")))
                {
                    sw.Write(content);
                    sw.Close();
                }
            }

            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
            {
                Debug.Write("Parse Financial Report:" + stock_index + "," + year.ToString() + "/" + season.ToString());
                BsonDocument financial = page.ParseHtml( sr.ReadToEnd());
                if (financial.ElementCount != 0)
                {
                    result = true;
                    page.DBSave(financial, stock_index, year, season);
                }
            }
            if(result == false)
            {
                File.Delete(path);
            }
            return result;
        }

        public String getDownloadStatus()
        {
            return current_download_status;
        }

        public DateTime getLastUpdateDate()
        {
            DateTime[] last_dates = new DateTime[] { new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), new DateTime(1900, 1, 1) };
            EquiltyPage[] equilitys = new EquiltyPage[] { new StockPage(), new PageJuristic("foreign_"), new PageJuristic("selfoper_"), new PageJuristic("investtrust_"), };

            try
            {
                List<string> file = new List<string>(Directory.EnumerateFiles(TargetDirectory));

                foreach (var f in file)
                {
                    int i;
                    for (i = 0; i < 4; i++)
                    {
                        if (f.Contains( equilitys[i].getFilePrefix() ))
                        {
                            DateTime d = equilitys[i].pageFileNameToDate(f.Substring((TargetDirectory + "\\").Length));
                            if (d.CompareTo(last_dates[i]) > 0)
                                last_dates[i] = d;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Console.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Console.WriteLine(PathEx.Message);
            }

            for (int i = 0; i < 4; i++)
            {
                Logger.v(String.Format("{0} last date:{1}.", equilitys[i].getFilePrefix(), last_dates[i]));
                Debug.WriteLine("{0}:{1}.", equilitys[i].getFilePrefix(), last_dates[i]);
            }

            DateTime date = last_dates[0];
            foreach(DateTime d in last_dates)
            {
                if(d.CompareTo(date) < 0)
                    date = d;
            }
            Debug.WriteLine("latest date:{0}.", date);
            return date;
        }

        


    }
}
