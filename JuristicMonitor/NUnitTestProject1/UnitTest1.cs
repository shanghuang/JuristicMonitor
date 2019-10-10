using NUnit.Framework;
using JuristicMonitor;

using System;
using System.IO;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using MongoDB.Bson;
using System.Collections;


namespace Tests
{
    public class Tests
    {

        DBManager db_manager = new DBManager();
        MySqlConnection conn;

        [SetUp]
        public void Setup()
        {
            db_manager.DBConnect();
            /*if (db_manager.isConnected() == false)
            {
                return;
            }

            db_manager.getConnection();*/
        }

        [Test]
        public void Test1()
        {
            StockDownloadManager dm = new StockDownloadManager();
            DateTime date = new DateTime(2019, 1, 3);
            dm.Download_PerDay(date);
            Assert.Pass();
        }

        [Test]
        public void TestParseStockHtml()
        {
            StockPage page = new StockPage();


            DateTime date = new DateTime(2019, 1, 3);
            String path = page.getFileName(date);

            String srcDir = @"C:\Temp\htmldwl";
            if (File.Exists(srcDir + path) == false)
            {
                //Logger.e("File " + srcDir + path + " not found!!");
                //continue;
            }

            using (StreamReader sr = new StreamReader(srcDir + path, Encoding.GetEncoding("utf-8")))
            {
                page.ParseHtml(conn, sr.ReadToEnd(), date);
            }

            Assert.Pass();
        }

        [Test]
        public void TestParseForeignHtml()
        {
            PageJuristic page = new PageJuristic("foreign_");


            DateTime date = new DateTime(2019, 1, 3);
            String path = page.getFileName(date);

            String srcDir = @"C:\Temp\htmldwl";
            if (File.Exists(srcDir + path) == false)
            {
                //Logger.e("File " + srcDir + path + " not found!!");
                //continue;
            }

            using (StreamReader sr = new StreamReader(srcDir + path, Encoding.GetEncoding("utf-8")))
            {
                page.ParseHtml(conn, sr.ReadToEnd(), date);
            }

            Assert.Pass();
        }

        [Test]
        public void TestParseFinancialReport()
        {
            String srcDir = @"C:\Temp\htmldwl\";

            PageFinancialReport page = new PageFinancialReport();

            String path = srcDir + page.getFileName("2379", 108, 1);
            if (File.Exists(path) == false)
            {
                String content = page.download_page(page.getPageUrl(), "2379", 108, 1);
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8")))
                {
                    sw.Write(content);
                    sw.Close();
                }
            }

            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
            {
                BsonDocument res = page.ParseHtml( sr.ReadToEnd());
            }

            Assert.Pass();
        }

        [Test]
        public void TestMongoConnection()
        {
            DbMango mongo = new DbMango();
            mongo.connect();

            PageFinancialReport page = new PageFinancialReport();

            String path = @"C:\Temp\htmldwl\" + page.getFileName("2379", 108, 1);
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("utf-8")))
            {
                BsonDocument financial = page.ParseHtml( sr.ReadToEnd());
                mongo.FinancialReport_save(financial,"2379", 108, 1);
            }

        }

        [Test]
        public void TestMongo_FinancialReport_GetLatest()
        {
            DbMango mongo = new DbMango();
            mongo.connect();
            mongo.FinancialReport_FindLatest("2371");
        }


        [Test]
        public void TestSQLSaveStock()
        {
            DBManager sql_db = new DBManager();
            sql_db.DBConnect();

            StockPage page = new StockPage();
            DateTime date = new DateTime(2019, 1, 3);
            String path = page.getFileName(date);

            using (StreamReader sr = new StreamReader(@"C:\Temp\htmldwl" + path, Encoding.GetEncoding("utf-8")))
            {
                ArrayList  stock_data = page.ParseHtml(conn, sr.ReadToEnd(), date);
                sql_db.StockData_Save(stock_data);
            }
        }

        [Test]
        public void TestSQLSaveForeign()
        {
            DBManager sql_db = new DBManager();
            sql_db.DBConnect();

            PageJuristic page = new PageJuristic("foreign_");
            DateTime date = new DateTime(2019, 1, 3);
            String path = page.getFileName(date);

            using (StreamReader sr = new StreamReader(@"C:\Temp\htmldwl" + path, Encoding.GetEncoding("utf-8")))
            {
                ArrayList stock_data = page.ParseHtml(conn, sr.ReadToEnd(), date);
                sql_db.StockData_SaveForeign(stock_data);
            }
        }

        [Test]
        public void TestGetLatestDownload()
        {
            DBManager sql_db = new DBManager();
            sql_db.DBConnect();

            StockPage sp = new StockPage();
            DateTime latest = sp.getLastDBDate(sql_db, "2379");
        }

        [Test]
        public void TestSeasonalReportReady()
        {
            StockDownloadManager dm = new StockDownloadManager();

            Boolean res = dm.SeasonalReportReady(108, 3);
            Boolean res2 = dm.SeasonalReportReady(107, 4);
        }

        [Test]
        public void TestTradingDay()
        {
            Boolean d1 = TradingDays.IsTradingDay(new DateTime(2019, 10, 8));
            Boolean d2 = TradingDays.IsTradingDay(new DateTime(2019, 10, 10));
            Boolean d3 = TradingDays.IsTradingDay(new DateTime(2018, 12, 22));

            Boolean d4 = TradingDays.IsTradingDay(new DateTime(2015, 10, 8));

            /*TradingDays td = new TradingDays();
            Boolean d1 = td.IsTradingDay(new DateTime(2019, 10, 8));
            Boolean d2 = td.IsTradingDay(new DateTime(2019, 10, 10));*/

        }

    }
}