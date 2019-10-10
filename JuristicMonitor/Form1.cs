using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace JuristicMonitor
{
    public partial class Form1 : Form
    {
        System.Timers.Timer StatusUpdateTimer;

        public Form1()
        {
            InitializeComponent();

            StatusUpdateTimer = new System.Timers.Timer();
            StatusUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 2 seconds (2000 milliseconds).
            StatusUpdateTimer.Interval = 200;
            StatusUpdateTimer.Enabled = true;

            comboBox_source.SelectedIndex = 0;
            comboBox_WightingFunc.Items.Add("total volume");
            comboBox_WightingFunc.Items.Add("total volume*price");
            comboBox_WightingFunc.Items.Add("total volume/capital");
            comboBox_WightingFunc.SelectedIndex = 0;
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StockDownloadManager dm = new StockDownloadManager();
            dm.getLastUpdateDate();

            ThreadStart ts = new ThreadStart(dm.DownloadToLatest);
            Thread t = new Thread(ts);
            t.Start();
        }

        int current_added_index=0;
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //UpdateLog();
            if( backgroundWorker1_inProgress)
                backgroundWorker1.ReportProgress(current_added_index);

            if(companyProf_inprogress)
                backgroundWorker_companyProf.ReportProgress(current_added_index);

            if (backgroundWorker_downloadrevenue_inprogress)
                backgroundWorker_downloadrevenue.ReportProgress(downloadrevenue_index);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DBManager dbm = new DBManager();
            dbm.DBConnect();
            DateTime d = dbm.getLastUpdateDate();
        }

        StockDownloadManager download_manager;
        DBManager db_manager = new DBManager();
        private BackgroundWorker backgroundWorker1;
        Boolean backgroundWorker1_inProgress = false;

        private void button_SyncDatabase_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Text.Equals("Download Data"))
            {
                btn.Text = "Stop";
                Logger.setDebugMessageOnOff(checkBox_debug_message.Checked);

                download_manager = new StockDownloadManager();
                //db_manager = new DBManager();

                if (checkBox_check_all.Checked)
                    download_manager.setCheckDownload();

                StatusUpdateTimer = new System.Timers.Timer();
                StatusUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                // Set the Interval to 2 seconds (2000 milliseconds).
                StatusUpdateTimer.Interval = 200;
                StatusUpdateTimer.Enabled = true;

                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
                backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
                backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;

                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                btn.Text = "Download Data";

                if (backgroundWorker1 != null)
                    backgroundWorker1.CancelAsync();
            }
            /*hreadStart ts = new ThreadStart(syncTask);
            Thread t = new Thread(ts);
            t.Start();*/
        }

        // This event handler is where the time-consuming work is done. 
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker; 

            //worker.CancellationPending
            //worker.ReportProgress(i * 10);
            download_manager.setTargetDir(@"C:\Temp\htmldwl");
            download_manager.DownloadToLatest();
            db_manager.DBConnect();
            if (db_manager.isConnected() == false)
            {
                return;
            }
            backgroundWorker1_inProgress = true;
            db_manager.UpdateToLatest(worker, @"C:\Temp\htmldwl");
        }

        // This event handler updates the progress. 
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateLog();
        }

        public void UpdateLog()
        {
            for (; current_added_index < Logger.get_msg_count(); current_added_index++)
            {
                Logger.LogItem logitem = Logger.get_message(current_added_index);
                ListViewItem lvi = new ListViewItem( logitem.gettime().ToShortTimeString());
                lvi.SubItems.Add(logitem.getlevel());
                lvi.SubItems.Add(logitem.getmessage());
                
                listView_message.Items.Add(lvi);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(
			object sender, 
			RunWorkerCompletedEventArgs e)
		{
            backgroundWorker1_inProgress = false;
		}

        private void checkBox_debug_message_CheckedChanged(object sender, EventArgs e)
        {
            Logger.setDebugMessageOnOff(checkBox_debug_message.Checked);
        }

        StockTradeInfo[] sti = null;
        Boolean Update_STI = false;

        private void button_BuildTable_Click(object sender, EventArgs e)
        {
            StockPage stk = new StockPage();

            db_manager.DBConnect();

            //DateTime last_date = stk.getLastDBDate(db_manager);
            int period = (int)long.Parse( textBox_period.Text);
            sti = stk.GetStockTradeHistory(db_manager.getConnection(), textBox_stock_number.Text, period);
            Update_STI = true;
            panel1.Invalidate();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            var g = e.Graphics;
            Size draw_area = p.Size;
            if (Update_STI)
            {
                if (sti == null)
                    return;
                //int day_count = sti.Length;

                float ymax, ymin;
                getPriceRange(sti, out ymax, out ymin);

                int x_step = draw_area.Width / sti.Length;
                int x_center_init = draw_area.Width / (2 * sti.Length);


                int i;
                for (i = 0; i < sti.Length; i++)
                {
                    Color color = (sti[i].close >= sti[i].open) ? Color.Red : Color.Green;
                    Pen thinPen = new Pen(color, 1);
                    Pen thickPen = new Pen(color, x_step);

                    int x = draw_area.Width - (x_center_init + x_step * i);

                    int y_high = map_y(sti[i].high, ymax, ymin, draw_area.Height);
                    int y_low = map_y(sti[i].low, ymax, ymin, draw_area.Height);
                    g.DrawLine(thinPen, x, y_low, x, y_high);

                    int y_open = map_y(sti[i].open, ymax, ymin, draw_area.Height);
                    int y_close = map_y(sti[i].close, ymax, ymin, draw_area.Height);
                    g.DrawLine(thickPen, x, y_open, x, y_close);

                }
            }

        }

        private int map_y(float input, float max, float min, int height)
        {
            float y_range = max - min;
            float y_ratio = height / y_range;
            int map_value = (int)((input - min) * y_ratio);
            return height - map_value;
        }

        private void getPriceRange(StockTradeInfo[] sti, out float ymax, out float ymin)
        {
            int i;
            ymax = sti[0].high;
            ymin = sti[0].low;
            for (i = 1; i < sti.Length; i++)
            {
                if (sti[i].high > ymax)
                    ymax = sti[i].high;
                if (sti[i].low < ymin)
                    ymin = sti[i].low;
            }
        }

        int getViewDays(int d)
        {
            if (d < 30)
                return 30;
            if (d < 90)
                return 90;
            //if (d < 180)
            return 180;
        }

        JuristicTradeInfo[] jti = null;
        JuristicTradeInfo[] graph_jti = null;

        private void button_analyze_juristic_Click(object sender, EventArgs e)
        {
            PageJuristic stk = new PageJuristic("foreign_");

            db_manager.DBConnect();

            //DateTime last_date = stk.getLastDBDate(db_manager);
            juristic_days = (int)long.Parse(textBox_jur_period.Text);
            int view_days = getViewDays(juristic_days);
            jti = stk.GetStockTradeHistory(db_manager.getConnection(), comboBox_source.SelectedIndex + 1, view_days);
            AnaLyzeJuristic(jti, juristic_days);
            panel_juristic.Invalidate();
        }

        public class StockRatingComparer : IComparer<StockRatingInfo>
        {
            public int Compare(StockRatingInfo x, StockRatingInfo y)
            {
                // Compare y and x in reverse order.
                return y.weighting.CompareTo(x.weighting);
            }
        }

        private void AnaLyzeJuristic(JuristicTradeInfo[] jti, int days2analyse)
        {
            Hashtable weight = new Hashtable();
            CompanyProfilePage cpp = new CompanyProfilePage();
            StockPage sp = new StockPage();
            int view_days = getViewDays(days2analyse);
            //int days2analyse = int.Parse(textBox_jur_period.Text);

            Hashtable last_price = sp.getLastClosePriceTable(db_manager.getConnection());

            foreach (JuristicTradeInfo ji in jti)
            {
                if (weight.ContainsKey(ji.stock_index) == false)
                {
                    //query stock info
                    int cap = cpp.GetStockCapital(db_manager.getConnection(), ji.stock_index);
                    //float last_price = sp.getLastClosePrice(db_manager.getConnection(),  ji.stock_index);
                    float lp = 0;
                    if( last_price.ContainsKey(ji.stock_index) )
                        lp = (float)last_price[ji.stock_index];
                    StockRatingInfo sr = new StockRatingInfo(ji.stock_index, cap, lp , 0);
                    weight.Add(ji.stock_index, sr);
                }

                DateTime ref_date = DateTime.Now.AddDays(-1*days2analyse);
                if (ji.trans_date.CompareTo(ref_date) >= 0)
                {
                    ((StockRatingInfo)weight[ji.stock_index]).weighting += (float)(ji.total_volume / 1000.0);
                }
            }

            StockRatingInfo[] juristic_stocks = new StockRatingInfo[weight.Count];

            weight.Values.CopyTo(juristic_stocks,0);

            foreach (StockRatingInfo sri in juristic_stocks)
            {
                if (comboBox_WightingFunc.SelectedIndex == 1)
                {
                    sri.weighting *= sri.price_close;
                }
                else if (comboBox_WightingFunc.SelectedIndex == 2)
                {
                    if (sri.capital != 0)
                        sri.weighting /= sri.capital;
                }
            }

            Array.Sort(juristic_stocks, new StockRatingComparer());

            listView_weighting.Items.Clear();

            int i;
            for (i = 0; i < juristic_stocks.Length; i++ )
            {
                ListViewItem lvi = new ListViewItem(juristic_stocks[i].stock_index);
                lvi.SubItems.Add(String.Format("{0}", juristic_stocks[i].capital));
                lvi.SubItems.Add(String.Format("{0}", juristic_stocks[i].price_close));
                lvi.SubItems.Add(String.Format("{0}", juristic_stocks[i].weighting));
                listView_weighting.Items.Add(lvi);
            }
        }


        private void panel_juristic_Paint(object sender, PaintEventArgs e)
        {
            if (UpdateJuristicGraph == false)
                return;

            if (jti == null)
                return;

            var p = sender as Panel;
            var g = e.Graphics;
            Size draw_area = p.Size;
            int x_step = draw_area.Width / juristic_days;
            int x_center_init = draw_area.Width / (2 * juristic_days);


            foreach(Object obj in match_juristic_trade_info)
            {
                JuristicTradeInfo ti = (JuristicTradeInfo)obj;

                Color color = (ti.total_volume > 0) ? Color.Red : Color.Green;
                Pen thickPen = new Pen(color, x_step-1);
                int day_diff = (DateTime.Now - ti.trans_date).Days;
                int x = draw_area.Width - (x_center_init + x_step * day_diff);
                double y_ratio = draw_area.Height / (2.0 * y_height);
                int map_value = (int)(ti.total_volume * y_ratio);
                int y = draw_area.Height/2 - map_value;

                g.DrawLine(thickPen, x, draw_area.Height / 2, x, y);
            }
            UpdateJuristicGraph = false;
        }

        private BackgroundWorker backgroundWorker_companyProf;
        private bool companyProf_inprogress = false;
        private bool UpdateJuristicGraph = false;
        private String juristic_stock_idx="";
        ArrayList match_juristic_trade_info;
        private int juristic_max_vol = 0;
        private int juristic_min_vol = 0;
        private int y_height;
        private int juristic_days=0;

        private void button_companyProfile_Click(object sender, EventArgs e)
        {
            if (button_companyProfile.Text == "CompanyProfile")
            {
                button_companyProfile.Text = "Stop";
                db_manager.DBConnect();
                //UpdateCompanyInfo();
                backgroundWorker_companyProf = new BackgroundWorker();
                backgroundWorker_companyProf.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_companyProf_RunWorkerCompleted);
                backgroundWorker_companyProf.DoWork += new DoWorkEventHandler(this.backgroundWorker_companyProf_DoWork);
                backgroundWorker_companyProf.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_companyProf_ProgressChanged);
                backgroundWorker_companyProf.WorkerReportsProgress = true;
                backgroundWorker_companyProf.WorkerSupportsCancellation = true;
                
                backgroundWorker_companyProf.RunWorkerAsync();
            }
            else
            {
                button_companyProfile.Text = "CompanyProfile";
                if (backgroundWorker_companyProf != null)
                    backgroundWorker_companyProf.CancelAsync();
            }
        }

        public List<String> GetAllStocks()
        {
            List<String> res = new List<String>();

            String qstr = "SELECT Distinct(stock_index) from tw_stock order by stock_index";
            MySqlCommand cmd = new MySqlCommand(qstr, db_manager.getConnection());
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                String idx = (String)rdr["stock_index"];
                if (idx.StartsWith("0") == false)
                    res.Add(idx);
            }
            rdr.Close();

            return res;
        }

        //public void UpdateCompanyInfo()
        private void backgroundWorker_companyProf_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> stock_ary = new List<String>();
            companyProf_inprogress = true;

            try
            {
                String qstr = "SELECT Distinct(stock_index) from tw_stock order by stock_index";
                MySqlCommand cmd = new MySqlCommand(qstr, db_manager.getConnection());
                MySqlDataReader rdr = cmd.ExecuteReader();
                while( rdr.Read() )
                {
                    String idx = (String)rdr["stock_index"];
                    if(idx.StartsWith("0") == false)
                        stock_ary.Add(idx);
                }
                rdr.Close();

                CompanyProfilePage cpp = new CompanyProfilePage();
                //foreach(object obj in stock_ary)
                while(stock_ary.Count > 0)
                {
                    String idx = stock_ary.ElementAt(0);
                    stock_ary.RemoveAt(0);
                    Thread.Sleep(10 * 1000);
                    CompanyData company_data = cpp.DownloadInfo(idx);
                    if (company_data.capital.Equals(""))
                    {
                        Logger.v("company profile " + idx + "download fail");
                        stock_ary.Add(idx);
                        continue;
                    }

                    String capital_str = company_data.capital.Replace(",", "").Replace("元", "");
                    float capital_float = float.Parse(capital_str);
                    int capital = (int)(capital_float / 100000000);
                    if (cpp.isExistCompanyProfile(db_manager.getConnection(), idx))
                    {
                        cpp.UpdateCompanyProfile(db_manager.getConnection(), idx, company_data.FullName, capital);
                        Logger.v("company profile " + idx + ", update to "+ String.Format("{0}", capital) );
                    }
                    else
                    {
                        cpp.AddCompanyProfile(db_manager.getConnection(), idx, company_data.FullName, capital);
                        Logger.v("company profile " + idx + "added: "+ String.Format("{0}", capital) );
                    }

                    if (backgroundWorker_companyProf.CancellationPending)
                        break;
                }
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void backgroundWorker_companyProf_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateLog();
        }

        private void backgroundWorker_companyProf_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            companyProf_inprogress = false;
        }

        private void listView_weighting_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selected_stocks = listView_weighting.SelectedItems;
            
            if (selected_stocks.Count > 0)
            {
                CompanyProfilePage cpp = new CompanyProfilePage();
                ListViewItem select_stock = selected_stocks[0];
                juristic_stock_idx = select_stock.Text;
                label_StockName.Text = cpp.GetStockChineseName(db_manager.getConnection(), juristic_stock_idx);
                match_juristic_trade_info = new ArrayList();
                juristic_max_vol = 0;
                juristic_min_vol = 0;
                foreach (JuristicTradeInfo trade_info in jti)
                {
                    if (trade_info.stock_index.Equals(juristic_stock_idx))
                    {
                        match_juristic_trade_info.Add(trade_info);
                        if (trade_info.total_volume > juristic_max_vol)
                            juristic_max_vol = trade_info.total_volume;
                        if (trade_info.total_volume < juristic_min_vol)
                            juristic_min_vol = trade_info.total_volume;
                    }
                }
                y_height = Math.Max(juristic_max_vol, Math.Abs(juristic_min_vol));
                label_StockName.Text = cpp.GetStockChineseName(db_manager.getConnection(), juristic_stock_idx);

                UpdateJuristicGraph = true;
                panel_juristic.Invalidate();
            }
        }

        private void button_QueryProfile_Click(object sender, EventArgs e)
        {
            db_manager.DBConnect();
            CompanyProfilePage cpp = new CompanyProfilePage();
            String res = cpp.GetStockChineseName(db_manager.getConnection(), "1101");
            label_company_profile_result.Text = res;
        }

        private void button_LastPrice_Click(object sender, EventArgs e)
        {
            db_manager.DBConnect();
            StockPage sp = new StockPage();
            //float close_price = sp.getLastClosePrice(db_manager.getConnection(), "1101");
            Hashtable last_price = sp.getLastClosePriceTable(db_manager.getConnection());
            float sss = (float)last_price["1101"];
            label_LastClosePrice.Text = String.Format("{0}", (float)last_price["1101"]);
        }

        private void button_companyEarning_Click(object sender, EventArgs e)
        {
            CompanyProfilePage cpp = new CompanyProfilePage();
            db_manager.DBConnect();
            //cpp.DownloadEarningInfoAll(db_manager.getConnection(), "2379");

        }

        private void button_Test_eps_Click(object sender, EventArgs e)
        {
            CompanyProfilePage cpp = new CompanyProfilePage();
            db_manager.DBConnect();
            //cpp.DownloadEpsAll(db_manager.getConnection(), "2379");
        }

        private void button_monthly_revenue_Click(object sender, EventArgs e)
        {
            CompanyProfilePage cpp = new CompanyProfilePage();
            db_manager.DBConnect();
            //cpp.DownloadMonthRevenueAll(db_manager.getConnection(), "2379");
        }

        private void button_DownloadEarning_Click(object sender, EventArgs e)
        {
            db_manager.DBConnect();
            CompanyProfilePage cpp = new CompanyProfilePage();
            // cpp.DownloadEarningInfoAll(db_manager.getConnection(), "2379");             //from 98-102. profit margin
            cpp.DownloadEpsInfo(db_manager.getConnection(), "2379", 100, 1);   //eps
            cpp.DownloadMonthRevenue(db_manager.getConnection(), "2379", 100, 1);  //revenue

            cpp.DownloadEpsInfo(db_manager.getConnection(), "2379", 102, 1);   //eps
            cpp.DownloadMonthRevenue(db_manager.getConnection(), "2379", 102, 1);  //revenue
            int rev = cpp.GetMonthRevenue(db_manager.getConnection(), "2379", 102, 1);
            EarningInfo ei = cpp.GetSeasonEarning(db_manager.getConnection(), "2379", 102, 1);

            YearSeasonMonth ysm = cpp.GetLatestRevenueInfoDate(db_manager.getConnection(), "2379");
            ysm = cpp.GetLatestEarningInfoDate(db_manager.getConnection(), "2379");
        }

        private void button_GetEarning_Click(object sender, EventArgs e)
        {
            db_manager.DBConnect();
            CompanyProfilePage cpp = new CompanyProfilePage();
            int year;
            int season;

            for(year=98;year<102;year++)
            {
                for(season=1;season<=4;season++)
                {
                    cpp.DownloadMonthRevenue(db_manager.getConnection(), "2379", year, season);
                }
            }
        }

        private BackgroundWorker backgroundWorker_downloadrevenue;
        private bool backgroundWorker_downloadrevenue_inprogress = false;
        private int downloadrevenue_index = 0;

        private void button_DownloadRevenue_Click(object sender, EventArgs e)
        {
            if (button_DownloadRevenue.Text == "DownloadRevenue")
            {
                button_DownloadRevenue.Text = "Stop";
                db_manager.DBConnect();
                //UpdateCompanyInfo();
                backgroundWorker_downloadrevenue = new BackgroundWorker();
                backgroundWorker_downloadrevenue.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_downloadrevenue_RunWorkerCompleted);
                backgroundWorker_downloadrevenue.DoWork += new DoWorkEventHandler(this.backgroundWorker_downloadrevenue_DoWork);
                backgroundWorker_downloadrevenue.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_companyProf_ProgressChanged);
                backgroundWorker_downloadrevenue.WorkerReportsProgress = true;
                backgroundWorker_downloadrevenue.WorkerSupportsCancellation = true;

                backgroundWorker_downloadrevenue.RunWorkerAsync();
            }
            else
            {
                button_DownloadRevenue.Text = "DownloadRevenue";
                if (backgroundWorker_downloadrevenue != null)
                    backgroundWorker_downloadrevenue.CancelAsync();
            }
        }

        YearSeasonMonth EarningStart = new YearSeasonMonth(98,1,1);
        YearSeasonMonth IfrsStart = new YearSeasonMonth(102, 1, 1);

        private void backgroundWorker_downloadrevenue_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker downloadrevenue_worker = sender as BackgroundWorker; 

            List<String> stocks = GetAllStocks();
            DateTime current = DateTime.Now;
            YearSeasonMonth currentYSM = new YearSeasonMonth(current ,true);

            CompanyProfilePage cpp = new CompanyProfilePage();

            backgroundWorker_downloadrevenue_inprogress = true;

            foreach (String stock_idx in stocks)
            {

                if (downloadrevenue_worker.CancellationPending)
                {
                    Logger.v("cancelling databases update");
                    backgroundWorker_downloadrevenue_inprogress = false;
                    break;
                }

                //earning
                YearSeasonMonth latestEarning = cpp.GetLatestEarningInfoDate(db_manager.getConnection(), stock_idx);
                List<YearSeasonMonth> EPS2download = new List<YearSeasonMonth>();
                YearSeasonMonth nextEarning;
                if ( (latestEarning == null) || (latestEarning.year < 102) )
                {
                    cpp.DownloadEarningInfo_beforeIFRS(db_manager.getConnection(), stock_idx, EPS2download);
                    nextEarning = IfrsStart;
                }
                else
                    nextEarning = latestEarning.NextSeason();

                downloadrevenue_index++;
                for (int y = nextEarning.year; y <= (current.Year-1911) ; y++)
                {
                    cpp.DownloadEarningInfo(db_manager.getConnection(), stock_idx, y, EPS2download);
                    downloadrevenue_index++;
                }

                //eps info is from another page
                foreach (YearSeasonMonth season in EPS2download)
                {
                    cpp.DownloadEpsInfo(db_manager.getConnection(), stock_idx, season.year, season.season);
                    downloadrevenue_index++;
                    int loop;
                    for (loop = 0; loop < 10; loop++)
                    {
                        if (cpp.isEPS_Null(db_manager.getConnection(), stock_idx, season.year, season.season))
                        {
                            cpp.DownloadEpsInfo(db_manager.getConnection(), stock_idx, season.year, season.season);
                            downloadrevenue_index++;
                        }
                    }
                }
                
                YearSeasonMonth latestRevenue = cpp.GetLatestRevenueInfoDate(db_manager.getConnection(), stock_idx);
                if (latestRevenue == null)
                    latestRevenue = EarningStart;
                for (YearSeasonMonth month = latestRevenue; month < currentYSM; month.NextMonth())
                {
                    cpp.DownloadMonthRevenue(db_manager.getConnection(), stock_idx, month.year, month.month);
                    downloadrevenue_index++;
                }
                
            }
        }

        private void backgroundWorker_downloadrevenue_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateLog();
        }

        private void backgroundWorker_downloadrevenue_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker_downloadrevenue_inprogress = false;
        }


        //temp 
        public class stockSeason
        {
            public string stock_index;
            public int year;
            public int season;

            public stockSeason(String s, int y, int season)
            {
                stock_index = s;
                year = y;
                this.season = season;
            }
        }

        private void backgroundWorker_check_download_EPS_DoWork(object sender, DoWorkEventArgs e)
        {
            db_manager.DBConnect();
            MySqlConnection conn = db_manager.getConnection();
            CompanyProfilePage cpp = new CompanyProfilePage();

            while (true)
            {
                List<stockSeason> nulleps = new List<stockSeason>();
                string qstr = "SELECT * FROM company_earning WHERE eps is NULL order by stock_index;";
                MySqlCommand cmd = new MySqlCommand(qstr, conn);
                MySqlDataReader rdr = null;
                try
                {
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string idx = (string)rdr["stock_index"];
                        int year = (int)rdr["year"];
                        int season = (int)rdr["season"];
                        nulleps.Add(new stockSeason(idx, year, season));
                    }
                }
                catch (Exception ex)
                {
                    //Logger.e("isEPS_Null:" + stock_index + "(" + year + "," + season + ")  failed");
                }
                finally
                {
                    if (rdr != null)
                        rdr.Close();
                }

                Logger.v("total " + nulleps.Count + " eps items are null!");
                backgroundWorker_downloadrevenue_inprogress = true;
                foreach (stockSeason stkseason in nulleps)
                {
                    cpp.DownloadEpsInfo(db_manager.getConnection(), stkseason.stock_index, stkseason.year, stkseason.season);
                    downloadrevenue_index++;
                    //Thread.Sleep(3);
                }
            }
        }

        private void buttondownload_EPS_Click(object sender, EventArgs e)
        {
            if (buttondownload_EPS.Text == "download_EPS")
            {
                button_DownloadRevenue.Text = "Stop";
                db_manager.DBConnect();
                //UpdateCompanyInfo();
                backgroundWorker_downloadrevenue = new BackgroundWorker();
                backgroundWorker_downloadrevenue.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker_downloadrevenue_RunWorkerCompleted);
                backgroundWorker_downloadrevenue.DoWork += new DoWorkEventHandler(this.backgroundWorker_check_download_EPS_DoWork);
                backgroundWorker_downloadrevenue.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker_companyProf_ProgressChanged);
                backgroundWorker_downloadrevenue.WorkerReportsProgress = true;
                backgroundWorker_downloadrevenue.WorkerSupportsCancellation = true;

                backgroundWorker_downloadrevenue.RunWorkerAsync();
            }
            else
            {
                buttondownload_EPS.Text = "download_EPS";
                if (backgroundWorker_downloadrevenue != null)
                    backgroundWorker_downloadrevenue.CancelAsync();
            }
        }

        private void button_new_juristic_Click(object sender, EventArgs e)
        {
            StockPage stk = new StockPage();
            DateTime date = new DateTime(2015, 5, 28);
            String res = stk.download_page(stk.getPageUrl(date), date);

            using (StreamWriter sw = new StreamWriter(@"c:\temp\test.html", false, Encoding.GetEncoding("utf-8")))
            {
                sw.Write(res);
                sw.Close();
            }

            db_manager.DBConnect();
            stk.ParseHtml(db_manager.getConnection(), res, date);
            /*PageJuristic p_juristic = new PageJuristic("foreign_");

            using (StreamReader sr = new StreamReader(@"c:\temp\TWSE 臺灣證券交易所 _ 外資及陸資買賣超彙總表.html", Encoding.GetEncoding("big5")))
            {
                        p_juristic.ParseHtmlNew(db_manager.getConnection(), sr.ReadToEnd(), new DateTime());
            }
             * */


        }
    }
}
