namespace JuristicMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button_update = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttondownload_EPS = new System.Windows.Forms.Button();
            this.button_DownloadRevenue = new System.Windows.Forms.Button();
            this.button_TestEarning = new System.Windows.Forms.Button();
            this.button_monthly_revenue = new System.Windows.Forms.Button();
            this.button_Test_eps = new System.Windows.Forms.Button();
            this.button_companyEarning = new System.Windows.Forms.Button();
            this.label_LastClosePrice = new System.Windows.Forms.Label();
            this.button_LastPrice = new System.Windows.Forms.Button();
            this.label_company_profile_result = new System.Windows.Forms.Label();
            this.button_QueryProfile = new System.Windows.Forms.Button();
            this.button_companyProfile = new System.Windows.Forms.Button();
            this.checkBox_debug_message = new System.Windows.Forms.CheckBox();
            this.checkBox_check_all = new System.Windows.Forms.CheckBox();
            this.button_SyncDatabase = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button_BuildTable = new System.Windows.Forms.Button();
            this.textBox_period = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_stock_number = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.comboBox_WightingFunc = new System.Windows.Forms.ComboBox();
            this.label_StockName = new System.Windows.Forms.Label();
            this.button_analyze_juristic = new System.Windows.Forms.Button();
            this.panel_juristic = new System.Windows.Forms.Panel();
            this.textBox_jur_period = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_source = new System.Windows.Forms.ComboBox();
            this.listView_weighting = new System.Windows.Forms.ListView();
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.button_GetEarning = new System.Windows.Forms.Button();
            this.textBox_earnStockIndex = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.listView_message = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_new_juristic = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(17, 36);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1272, 498);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button_update);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1264, 469);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(32, 30);
            this.button_update.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(100, 31);
            this.button_update.TabIndex = 0;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button_new_juristic);
            this.tabPage2.Controls.Add(this.buttondownload_EPS);
            this.tabPage2.Controls.Add(this.button_DownloadRevenue);
            this.tabPage2.Controls.Add(this.button_TestEarning);
            this.tabPage2.Controls.Add(this.button_monthly_revenue);
            this.tabPage2.Controls.Add(this.button_Test_eps);
            this.tabPage2.Controls.Add(this.button_companyEarning);
            this.tabPage2.Controls.Add(this.label_LastClosePrice);
            this.tabPage2.Controls.Add(this.button_LastPrice);
            this.tabPage2.Controls.Add(this.label_company_profile_result);
            this.tabPage2.Controls.Add(this.button_QueryProfile);
            this.tabPage2.Controls.Add(this.button_companyProfile);
            this.tabPage2.Controls.Add(this.checkBox_debug_message);
            this.tabPage2.Controls.Add(this.checkBox_check_all);
            this.tabPage2.Controls.Add(this.button_SyncDatabase);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(1264, 469);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttondownload_EPS
            // 
            this.buttondownload_EPS.Location = new System.Drawing.Point(799, 286);
            this.buttondownload_EPS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttondownload_EPS.Name = "buttondownload_EPS";
            this.buttondownload_EPS.Size = new System.Drawing.Size(140, 28);
            this.buttondownload_EPS.TabIndex = 15;
            this.buttondownload_EPS.Text = "download_EPS";
            this.buttondownload_EPS.UseVisualStyleBackColor = true;
            this.buttondownload_EPS.Click += new System.EventHandler(this.buttondownload_EPS_Click);
            // 
            // button_DownloadRevenue
            // 
            this.button_DownloadRevenue.Location = new System.Drawing.Point(119, 367);
            this.button_DownloadRevenue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_DownloadRevenue.Name = "button_DownloadRevenue";
            this.button_DownloadRevenue.Size = new System.Drawing.Size(115, 31);
            this.button_DownloadRevenue.TabIndex = 14;
            this.button_DownloadRevenue.Text = "DownloadRevenue";
            this.button_DownloadRevenue.UseVisualStyleBackColor = true;
            this.button_DownloadRevenue.Click += new System.EventHandler(this.button_DownloadRevenue_Click);
            // 
            // button_TestEarning
            // 
            this.button_TestEarning.Location = new System.Drawing.Point(799, 175);
            this.button_TestEarning.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_TestEarning.Name = "button_TestEarning";
            this.button_TestEarning.Size = new System.Drawing.Size(140, 31);
            this.button_TestEarning.TabIndex = 13;
            this.button_TestEarning.Text = "TestEarning";
            this.button_TestEarning.UseVisualStyleBackColor = true;
            this.button_TestEarning.Click += new System.EventHandler(this.button_DownloadEarning_Click);
            // 
            // button_monthly_revenue
            // 
            this.button_monthly_revenue.Location = new System.Drawing.Point(545, 400);
            this.button_monthly_revenue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_monthly_revenue.Name = "button_monthly_revenue";
            this.button_monthly_revenue.Size = new System.Drawing.Size(133, 31);
            this.button_monthly_revenue.TabIndex = 12;
            this.button_monthly_revenue.Text = "MonthlyRevenue";
            this.button_monthly_revenue.UseVisualStyleBackColor = true;
            this.button_monthly_revenue.Click += new System.EventHandler(this.button_monthly_revenue_Click);
            // 
            // button_Test_eps
            // 
            this.button_Test_eps.Location = new System.Drawing.Point(545, 337);
            this.button_Test_eps.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_Test_eps.Name = "button_Test_eps";
            this.button_Test_eps.Size = new System.Drawing.Size(133, 31);
            this.button_Test_eps.TabIndex = 11;
            this.button_Test_eps.Text = "Test eps";
            this.button_Test_eps.UseVisualStyleBackColor = true;
            this.button_Test_eps.Click += new System.EventHandler(this.button_Test_eps_Click);
            // 
            // button_companyEarning
            // 
            this.button_companyEarning.Location = new System.Drawing.Point(545, 283);
            this.button_companyEarning.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_companyEarning.Name = "button_companyEarning";
            this.button_companyEarning.Size = new System.Drawing.Size(133, 31);
            this.button_companyEarning.TabIndex = 10;
            this.button_companyEarning.Text = "CompanyEarning";
            this.button_companyEarning.UseVisualStyleBackColor = true;
            this.button_companyEarning.Click += new System.EventHandler(this.button_companyEarning_Click);
            // 
            // label_LastClosePrice
            // 
            this.label_LastClosePrice.AutoSize = true;
            this.label_LastClosePrice.Location = new System.Drawing.Point(707, 188);
            this.label_LastClosePrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_LastClosePrice.Name = "label_LastClosePrice";
            this.label_LastClosePrice.Size = new System.Drawing.Size(46, 17);
            this.label_LastClosePrice.TabIndex = 9;
            this.label_LastClosePrice.Text = "label4";
            // 
            // button_LastPrice
            // 
            this.button_LastPrice.Location = new System.Drawing.Point(545, 175);
            this.button_LastPrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_LastPrice.Name = "button_LastPrice";
            this.button_LastPrice.Size = new System.Drawing.Size(133, 31);
            this.button_LastPrice.TabIndex = 8;
            this.button_LastPrice.Text = "Last Close Price";
            this.button_LastPrice.UseVisualStyleBackColor = true;
            this.button_LastPrice.Click += new System.EventHandler(this.button_LastPrice_Click);
            // 
            // label_company_profile_result
            // 
            this.label_company_profile_result.AutoSize = true;
            this.label_company_profile_result.Location = new System.Drawing.Point(955, 110);
            this.label_company_profile_result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_company_profile_result.Name = "label_company_profile_result";
            this.label_company_profile_result.Size = new System.Drawing.Size(46, 17);
            this.label_company_profile_result.TabIndex = 7;
            this.label_company_profile_result.Text = "label4";
            // 
            // button_QueryProfile
            // 
            this.button_QueryProfile.Location = new System.Drawing.Point(799, 95);
            this.button_QueryProfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_QueryProfile.Name = "button_QueryProfile";
            this.button_QueryProfile.Size = new System.Drawing.Size(140, 31);
            this.button_QueryProfile.TabIndex = 6;
            this.button_QueryProfile.Text = "QueryProfile";
            this.button_QueryProfile.UseVisualStyleBackColor = true;
            this.button_QueryProfile.Click += new System.EventHandler(this.button_QueryProfile_Click);
            // 
            // button_companyProfile
            // 
            this.button_companyProfile.Location = new System.Drawing.Point(545, 96);
            this.button_companyProfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_companyProfile.Name = "button_companyProfile";
            this.button_companyProfile.Size = new System.Drawing.Size(133, 31);
            this.button_companyProfile.TabIndex = 5;
            this.button_companyProfile.Text = "CompanyProfile";
            this.button_companyProfile.UseVisualStyleBackColor = true;
            this.button_companyProfile.Click += new System.EventHandler(this.button_companyProfile_Click);
            // 
            // checkBox_debug_message
            // 
            this.checkBox_debug_message.AutoSize = true;
            this.checkBox_debug_message.Location = new System.Drawing.Point(292, 337);
            this.checkBox_debug_message.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox_debug_message.Name = "checkBox_debug_message";
            this.checkBox_debug_message.Size = new System.Drawing.Size(133, 21);
            this.checkBox_debug_message.TabIndex = 4;
            this.checkBox_debug_message.Text = "Debug Message";
            this.checkBox_debug_message.UseVisualStyleBackColor = true;
            this.checkBox_debug_message.CheckedChanged += new System.EventHandler(this.checkBox_debug_message_CheckedChanged);
            // 
            // checkBox_check_all
            // 
            this.checkBox_check_all.AutoSize = true;
            this.checkBox_check_all.Location = new System.Drawing.Point(292, 306);
            this.checkBox_check_all.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox_check_all.Name = "checkBox_check_all";
            this.checkBox_check_all.Size = new System.Drawing.Size(184, 21);
            this.checkBox_check_all.TabIndex = 3;
            this.checkBox_check_all.Text = "Check missing download";
            this.checkBox_check_all.UseVisualStyleBackColor = true;
            // 
            // button_SyncDatabase
            // 
            this.button_SyncDatabase.Location = new System.Drawing.Point(119, 299);
            this.button_SyncDatabase.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_SyncDatabase.Name = "button_SyncDatabase";
            this.button_SyncDatabase.Size = new System.Drawing.Size(115, 31);
            this.button_SyncDatabase.TabIndex = 2;
            this.button_SyncDatabase.Text = "Download Data";
            this.button_SyncDatabase.UseVisualStyleBackColor = true;
            this.button_SyncDatabase.Click += new System.EventHandler(this.button_SyncDatabase_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(119, 188);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 31);
            this.button2.TabIndex = 1;
            this.button2.Text = "TestDB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(119, 96);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 31);
            this.button1.TabIndex = 0;
            this.button1.Text = "TestDownload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button_BuildTable);
            this.tabPage3.Controls.Add(this.textBox_period);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.textBox_stock_number);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Size = new System.Drawing.Size(1264, 469);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button_BuildTable
            // 
            this.button_BuildTable.Location = new System.Drawing.Point(517, 21);
            this.button_BuildTable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_BuildTable.Name = "button_BuildTable";
            this.button_BuildTable.Size = new System.Drawing.Size(100, 31);
            this.button_BuildTable.TabIndex = 5;
            this.button_BuildTable.Text = "Query";
            this.button_BuildTable.UseVisualStyleBackColor = true;
            this.button_BuildTable.Click += new System.EventHandler(this.button_BuildTable_Click);
            // 
            // textBox_period
            // 
            this.textBox_period.Location = new System.Drawing.Point(321, 21);
            this.textBox_period.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_period.Name = "textBox_period";
            this.textBox_period.Size = new System.Drawing.Size(132, 22);
            this.textBox_period.TabIndex = 4;
            this.textBox_period.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(265, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Period";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Stock";
            // 
            // textBox_stock_number
            // 
            this.textBox_stock_number.Location = new System.Drawing.Point(73, 22);
            this.textBox_stock_number.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_stock_number.Name = "textBox_stock_number";
            this.textBox_stock_number.Size = new System.Drawing.Size(132, 22);
            this.textBox_stock_number.TabIndex = 1;
            this.textBox_stock_number.Text = "2379";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(9, 74);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1244, 383);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.comboBox_WightingFunc);
            this.tabPage4.Controls.Add(this.label_StockName);
            this.tabPage4.Controls.Add(this.button_analyze_juristic);
            this.tabPage4.Controls.Add(this.panel_juristic);
            this.tabPage4.Controls.Add(this.textBox_jur_period);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.comboBox_source);
            this.tabPage4.Controls.Add(this.listView_weighting);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Size = new System.Drawing.Size(1264, 469);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // comboBox_WightingFunc
            // 
            this.comboBox_WightingFunc.FormattingEnabled = true;
            this.comboBox_WightingFunc.Location = new System.Drawing.Point(385, 28);
            this.comboBox_WightingFunc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox_WightingFunc.Name = "comboBox_WightingFunc";
            this.comboBox_WightingFunc.Size = new System.Drawing.Size(160, 24);
            this.comboBox_WightingFunc.TabIndex = 7;
            // 
            // label_StockName
            // 
            this.label_StockName.AutoSize = true;
            this.label_StockName.Location = new System.Drawing.Point(1061, 32);
            this.label_StockName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_StockName.Name = "label_StockName";
            this.label_StockName.Size = new System.Drawing.Size(46, 17);
            this.label_StockName.TabIndex = 6;
            this.label_StockName.Text = "label4";
            // 
            // button_analyze_juristic
            // 
            this.button_analyze_juristic.Location = new System.Drawing.Point(953, 14);
            this.button_analyze_juristic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_analyze_juristic.Name = "button_analyze_juristic";
            this.button_analyze_juristic.Size = new System.Drawing.Size(100, 31);
            this.button_analyze_juristic.TabIndex = 5;
            this.button_analyze_juristic.Text = "Analyze";
            this.button_analyze_juristic.UseVisualStyleBackColor = true;
            this.button_analyze_juristic.Click += new System.EventHandler(this.button_analyze_juristic_Click);
            // 
            // panel_juristic
            // 
            this.panel_juristic.Location = new System.Drawing.Point(385, 68);
            this.panel_juristic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel_juristic.Name = "panel_juristic";
            this.panel_juristic.Size = new System.Drawing.Size(853, 388);
            this.panel_juristic.TabIndex = 4;
            this.panel_juristic.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_juristic_Paint);
            // 
            // textBox_jur_period
            // 
            this.textBox_jur_period.Location = new System.Drawing.Point(269, 28);
            this.textBox_jur_period.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_jur_period.Name = "textBox_jur_period";
            this.textBox_jur_period.Size = new System.Drawing.Size(84, 22);
            this.textBox_jur_period.TabIndex = 3;
            this.textBox_jur_period.Text = "7";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "period";
            // 
            // comboBox_source
            // 
            this.comboBox_source.FormattingEnabled = true;
            this.comboBox_source.Items.AddRange(new object[] {
            "Foreign",
            "InvestTrust",
            "SelfOper"});
            this.comboBox_source.Location = new System.Drawing.Point(44, 28);
            this.comboBox_source.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox_source.Name = "comboBox_source";
            this.comboBox_source.Size = new System.Drawing.Size(160, 24);
            this.comboBox_source.TabIndex = 1;
            // 
            // listView_weighting
            // 
            this.listView_weighting.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Index,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView_weighting.Location = new System.Drawing.Point(43, 68);
            this.listView_weighting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView_weighting.Name = "listView_weighting";
            this.listView_weighting.Size = new System.Drawing.Size(311, 387);
            this.listView_weighting.TabIndex = 0;
            this.listView_weighting.UseCompatibleStateImageBehavior = false;
            this.listView_weighting.View = System.Windows.Forms.View.Details;
            this.listView_weighting.SelectedIndexChanged += new System.EventHandler(this.listView_weighting_SelectedIndexChanged);
            // 
            // Index
            // 
            this.Index.Text = "Index";
            this.Index.Width = 40;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Capital";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "LastPrice";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.listView1);
            this.tabPage5.Controls.Add(this.button_GetEarning);
            this.tabPage5.Controls.Add(this.textBox_earnStockIndex);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage5.Size = new System.Drawing.Size(1264, 469);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(9, 78);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(365, 377);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // button_GetEarning
            // 
            this.button_GetEarning.Location = new System.Drawing.Point(151, 5);
            this.button_GetEarning.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_GetEarning.Name = "button_GetEarning";
            this.button_GetEarning.Size = new System.Drawing.Size(100, 31);
            this.button_GetEarning.TabIndex = 1;
            this.button_GetEarning.Text = "Earning";
            this.button_GetEarning.UseVisualStyleBackColor = true;
            this.button_GetEarning.Click += new System.EventHandler(this.button_GetEarning_Click);
            // 
            // textBox_earnStockIndex
            // 
            this.textBox_earnStockIndex.Location = new System.Drawing.Point(8, 9);
            this.textBox_earnStockIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_earnStockIndex.Name = "textBox_earnStockIndex";
            this.textBox_earnStockIndex.Size = new System.Drawing.Size(132, 22);
            this.textBox_earnStockIndex.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1305, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // listView_message
            // 
            this.listView_message.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_message.GridLines = true;
            this.listView_message.Location = new System.Drawing.Point(17, 544);
            this.listView_message.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listView_message.Name = "listView_message";
            this.listView_message.Size = new System.Drawing.Size(1271, 237);
            this.listView_message.TabIndex = 2;
            this.listView_message.UseCompatibleStateImageBehavior = false;
            this.listView_message.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Level";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Message";
            this.columnHeader3.Width = 600;
            // 
            // button_new_juristic
            // 
            this.button_new_juristic.Location = new System.Drawing.Point(799, 374);
            this.button_new_juristic.Name = "button_new_juristic";
            this.button_new_juristic.Size = new System.Drawing.Size(75, 23);
            this.button_new_juristic.TabIndex = 16;
            this.button_new_juristic.Text = "TestNewJuristic";
            this.button_new_juristic.UseVisualStyleBackColor = true;
            this.button_new_juristic.Click += new System.EventHandler(this.button_new_juristic_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1305, 785);
            this.Controls.Add(this.listView_message);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView listView_message;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button button_SyncDatabase;
        private System.Windows.Forms.CheckBox checkBox_check_all;
        private System.Windows.Forms.CheckBox checkBox_debug_message;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button_BuildTable;
        private System.Windows.Forms.TextBox textBox_period;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_stock_number;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button_analyze_juristic;
        private System.Windows.Forms.Panel panel_juristic;
        private System.Windows.Forms.TextBox textBox_jur_period;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_source;
        private System.Windows.Forms.ListView listView_weighting;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button button_companyProfile;
        private System.Windows.Forms.Label label_StockName;
        private System.Windows.Forms.Button button_QueryProfile;
        private System.Windows.Forms.Label label_company_profile_result;
        private System.Windows.Forms.Label label_LastClosePrice;
        private System.Windows.Forms.Button button_LastPrice;
        private System.Windows.Forms.ComboBox comboBox_WightingFunc;
        private System.Windows.Forms.Button button_companyEarning;
        private System.Windows.Forms.Button button_Test_eps;
        private System.Windows.Forms.Button button_monthly_revenue;
        private System.Windows.Forms.Button button_TestEarning;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button_GetEarning;
        private System.Windows.Forms.TextBox textBox_earnStockIndex;
        private System.Windows.Forms.Button button_DownloadRevenue;
        private System.Windows.Forms.Button buttondownload_EPS;
        private System.Windows.Forms.Button button_new_juristic;
    }
}

