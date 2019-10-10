using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;

namespace JuristicMonitor
{
    public class DBManager
    {
        MySql.Data.MySqlClient.MySqlConnection conn;


        public void DBConnect()
        {
            string myConnectionString = "server=localhost;uid=shang;" +
                "pwd=king3697;database=twstock;";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                try
                {
                    setupUser();
                    SetupDatabase();
                    conn.Open();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex_in)
                {
                }
            }

            StockPage sp = new StockPage();
            PageJuristic jp = new PageJuristic("foreign_");
            sp.SetupDatebase(conn);
            jp.SetupDatebase(conn);
            CompanyProfilePage cpp = new CompanyProfilePage();
            cpp.SetupDatebase(conn);
            cpp.SetupEarningDatebase(conn);

            UserManager userManager = new UserManager();
            userManager.SetupDatebase(conn);

        }

        public Boolean isConnected()
        {
            return (conn != null);
        }

        public void SetupDatabase()
        {
            string myConnectionString = "server=127.0.0.1;uid=shang; pwd=king3697;";
            String check_database_cmd = @"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'twstock'";
            string create_db_cmd = "CREATE DATABASE IF NOT EXISTS twstock;";
            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();

                //check database
                MySqlCommand cmd = new MySqlCommand(check_database_cmd, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    cmd = new MySqlCommand(create_db_cmd, conn);
                    int res = cmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }


        }

        public void setupUser()
        {
            string myConnectionString = "server=127.0.0.1;uid=root; pwd=kingston;";
            string check_user_cmd = @"select * from mysql.user where User='shang@localhost'";
            String user_cmd = @"create user shang@localhost identified by 'king3697'";
            String grant_cmd = @"GRANT ALL on *.* to shang@localhost";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();

                //check database
                MySqlCommand cmd = new MySqlCommand(check_user_cmd, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows == false)
                {
                    rdr.Close();
                    //create user
                    cmd = new MySqlCommand(user_cmd, conn);
                    int res = cmd.ExecuteNonQuery();

                    //grant access
                    cmd = new MySqlCommand(grant_cmd, conn);
                    res = cmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        public MySqlConnection getConnection()
        {
            return conn;
        }

        public DateTime getLastUpdateDate()
        {
            StockPage stk = new StockPage();
            DateTime stk_date = stk.getLastDBDate(this, "2379");
            Logger.v("Stock database last date:" + String.Format("{0}", stk_date));

            PageJuristic foreign = new PageJuristic("foreign_");
            DateTime foreign_date = foreign.getLastDBDate(this);
            Logger.v("Foreign database last date:" + String.Format("{0}", foreign_date));

            DateTime res = foreign_date.CompareTo(stk_date) > 0 ? stk_date : foreign_date;
            Debug.WriteLine("Last DB date : " + res.ToShortDateString());
            return res;
        }

        public void UpdateToLatest(BackgroundWorker worker, String srcDir)
        {
            dynamic[] equility_pages = new dynamic[] { new StockPage(), new PageJuristic("foreign_"), new PageJuristic("selfoper_"), new PageJuristic("investtrust_"), };

            DateTime date = getLastUpdateDate();

            //debug
            if (date.Equals(DateTime.Today))
            {
                Logger.v("Stock/foreign/selfoper/investtrust databases are up to date");
            }

            while (!date.Equals(DateTime.Today))
            {
                if (worker.CancellationPending)
                {
                    Logger.v("cancelling databases update");
                    break;
                }

                date = date.AddDays(1);
                //ui_date = date;
                foreach (dynamic page in equility_pages)
                {
                    //current_download_status = "Downloading " + page.getFilePrefix() + date.ToShortDateString();
                    String path = page.getFileName(date);
                    if (File.Exists(srcDir + path) == false)
                    {
                        Logger.e("File " + srcDir + path + " not found!!");
                        continue;
                    }

                    using (StreamReader sr = new StreamReader(srcDir + path, Encoding.GetEncoding("big5")))
                    {
                        page.ParseHtml(conn, sr.ReadToEnd(), date);
                    }
                }
            }

            //debug
            if (date.Equals(DateTime.Today))
            {
                Logger.v("Exit database update");
            }
        }

        /*public String StockDbName(String stock_index)
        {
            return "s" + stock_index;
        }*/

        public void StockData_Save(ArrayList data)
        {
            foreach (Object obj in data)
            {
                StockTradeInfo stock_info = (StockTradeInfo)obj;
                String stock_db_name = stock_info.stock_index;

                if (stock_db_name.StartsWith("237") == false)   //debug
                    continue;

                string sql_ins = "INSERT INTO " + StockPage.StockDbName(stock_db_name) + "( trans_date, trans_volume, trans_count, trans_value, open, high, low ," +
                                                                "close, diff, close_buy_price, close_buy_volume, close_sell_price, close_sell_volume) VALUE(" +
                                                                "'" + Util.convertDate2mysql(stock_info.trans_date) + "'" + "," + stock_info.trans_volume.ToString() + "," + stock_info.trans_count.ToString() + "," + stock_info.trans_value.ToString() + "," +
                                                               Util.double2dec(stock_info.open, 6, 2) + "," + Util.double2dec(stock_info.high, 6, 2) + "," + Util.double2dec(stock_info.low, 5, 2) + "," + Util.double2dec(stock_info.close, 5, 2) + "," +
                                                                Util.double2dec(stock_info.diff, 5, 2) + "," + Util.double2dec(stock_info.close_buy_price, 5, 2) + "," + stock_info.close_buy_volume.ToString() + "," +
                                                                Util.double2dec(stock_info.close_sell_price, 5, 2) + "," + stock_info.close_sell_volume.ToString() +  ");";
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

        public void StockData_SaveForeign(ArrayList data)
        {
            foreach (Object obj in data)
            {
                JuristicTradeInfo stock_info = (JuristicTradeInfo)obj;
                String stock_db_name = stock_info.stock_index;

                if (stock_db_name.StartsWith("237") == false)   //debug
                    continue;
                /*string sql_update = "UPDATE @stock_db_name " +
                                " SET foreign_buy=@buy_volume, foreign_sell=@sell_volume, foreign_total=@total_volume " +
                                " WHERE trans_date =@trans_date;";*/
                string sql_update = "UPDATE " + StockPage.StockDbName(stock_db_name) +
                " SET foreign_buy=@buy_volume, foreign_sell=@sell_volume, foreign_total=@total_volume " +
                " WHERE trans_date =@trans_date;";

                MySqlCommand cmd_insert = new MySqlCommand(sql_update, conn);
                cmd_insert.Parameters.AddWithValue("@stock_db_name", StockPage.StockDbName(stock_db_name));
                cmd_insert.Parameters.AddWithValue("@buy_volume", stock_info.buy_volume);
                cmd_insert.Parameters.AddWithValue("@sell_volume", stock_info.sell_volume);
                cmd_insert.Parameters.AddWithValue("@total_volume", stock_info.total_volume);
                cmd_insert.Parameters.AddWithValue("@trans_date", Util.convertDate2mysql(stock_info.trans_date) );

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
    }
}
