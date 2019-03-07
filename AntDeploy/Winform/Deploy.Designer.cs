namespace AntDeploy.Winform
{
    partial class Deploy
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
            this.tabcontrol = new System.Windows.Forms.TabControl();
            this.page_web_iis = new System.Windows.Forms.TabPage();
            this.b_iis_rollback = new AltoControls.AltoButton();
            this.checkBox_Increment_iis = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_pool_name = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txt_iis_port = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.b_iis_deploy = new System.Windows.Forms.Button();
            this.combo_iis_env = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_iis_web_site_name = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.combo_iis_sdk_type = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label_iis_demo = new System.Windows.Forms.Label();
            this.tab_iis = new System.Windows.Forms.TabControl();
            this.tabPage_progress = new System.Windows.Forms.TabPage();
            this.progress_iis_tip = new System.Windows.Forms.Label();
            this.tabPage_iis_log = new System.Windows.Forms.TabPage();
            this.rich_iis_log = new AntDeploy.ExRichTextBox();
            this.page_docker = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.t_docker_delete_days = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txt_docker_volume = new AltoControls.AltoTextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.txt_docker_envname = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txt_docker_port = new System.Windows.Forms.TextBox();
            this.b_docker_deploy = new System.Windows.Forms.Button();
            this.combo_docker_env = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label_docker_demo = new System.Windows.Forms.Label();
            this.b_docker_rollback = new AltoControls.AltoButton();
            this.tabControl_docker = new System.Windows.Forms.TabControl();
            this.tabPage_docker = new System.Windows.Forms.TabPage();
            this.progress_docker_tip = new System.Windows.Forms.Label();
            this.tabPage_docker_log = new System.Windows.Forms.TabPage();
            this.rich_docker_log = new AntDeploy.ExRichTextBox();
            this.page_window_service = new System.Windows.Forms.TabPage();
            this.b_windows_service_rollback = new AltoControls.AltoButton();
            this.checkBox_Increment_window_service = new System.Windows.Forms.CheckBox();
            this.label_windows_serivce_demo = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txt_windowservice_name = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.combo_windowservice_sdk_type = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.b_windowservice_deploy = new System.Windows.Forms.Button();
            this.combo_windowservice_env = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabControl_window_service = new System.Windows.Forms.TabControl();
            this.tabPage_windows_service = new System.Windows.Forms.TabPage();
            this.progress_window_service_tip = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rich_windowservice_log = new AntDeploy.ExRichTextBox();
            this.page_set = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.b_backUp_ignore_remove = new System.Windows.Forms.Button();
            this.b_backUp_ignore_add = new System.Windows.Forms.Button();
            this.txt_backUp_ignore = new System.Windows.Forms.TextBox();
            this.list_backUp_ignore = new System.Windows.Forms.ListBox();
            this.label_check_update = new System.Windows.Forms.Label();
            this.groupBoxIgnore = new System.Windows.Forms.GroupBox();
            this.b_env_ignore_remove = new System.Windows.Forms.Button();
            this.b_env_ignore_add = new System.Windows.Forms.Button();
            this.txt_env_ignore = new System.Windows.Forms.TextBox();
            this.list_env_ignore = new System.Windows.Forms.ListBox();
            this.environment = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.page_winserver = new System.Windows.Forms.TabPage();
            this.loading_win_server_test = new AltoControls.SpinningCircles();
            this.label5 = new System.Windows.Forms.Label();
            this.b_env_server_test = new System.Windows.Forms.Button();
            this.combo_env_server_list = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.b_env_server_add = new System.Windows.Forms.Button();
            this.txt_env_server_host = new System.Windows.Forms.TextBox();
            this.b_env_server_remove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_env_server_token = new System.Windows.Forms.TextBox();
            this.page_linux_server = new System.Windows.Forms.TabPage();
            this.loading_linux_server_test = new AltoControls.SpinningCircles();
            this.label20 = new System.Windows.Forms.Label();
            this.combo_linux_server_list = new System.Windows.Forms.ComboBox();
            this.txt_linux_pwd = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txt_linux_username = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.b_linux_server_test = new System.Windows.Forms.Button();
            this.b_add_linux_server = new System.Windows.Forms.Button();
            this.b_linux_server_remove = new System.Windows.Forms.Button();
            this.txt_linux_host = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.b_env_remove = new System.Windows.Forms.Button();
            this.txt_env_name = new System.Windows.Forms.TextBox();
            this.b_env_add_by_name = new System.Windows.Forms.Button();
            this.combo_env_list = new System.Windows.Forms.ComboBox();
            this.tabcontrol.SuspendLayout();
            this.page_web_iis.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tab_iis.SuspendLayout();
            this.tabPage_progress.SuspendLayout();
            this.tabPage_iis_log.SuspendLayout();
            this.page_docker.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl_docker.SuspendLayout();
            this.tabPage_docker.SuspendLayout();
            this.tabPage_docker_log.SuspendLayout();
            this.page_window_service.SuspendLayout();
            this.tabControl_window_service.SuspendLayout();
            this.tabPage_windows_service.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.page_set.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxIgnore.SuspendLayout();
            this.environment.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.page_winserver.SuspendLayout();
            this.page_linux_server.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrol
            // 
            this.tabcontrol.Controls.Add(this.page_web_iis);
            this.tabcontrol.Controls.Add(this.page_docker);
            this.tabcontrol.Controls.Add(this.page_window_service);
            this.tabcontrol.Controls.Add(this.page_set);
            this.tabcontrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabcontrol.Location = new System.Drawing.Point(0, 0);
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.Size = new System.Drawing.Size(630, 482);
            this.tabcontrol.TabIndex = 0;
            this.tabcontrol.SelectedIndexChanged += new System.EventHandler(this.page__SelectedIndexChanged);
            // 
            // page_web_iis
            // 
            this.page_web_iis.Controls.Add(this.b_iis_rollback);
            this.page_web_iis.Controls.Add(this.checkBox_Increment_iis);
            this.page_web_iis.Controls.Add(this.label25);
            this.page_web_iis.Controls.Add(this.groupBox4);
            this.page_web_iis.Controls.Add(this.label9);
            this.page_web_iis.Controls.Add(this.b_iis_deploy);
            this.page_web_iis.Controls.Add(this.combo_iis_env);
            this.page_web_iis.Controls.Add(this.label8);
            this.page_web_iis.Controls.Add(this.txt_iis_web_site_name);
            this.page_web_iis.Controls.Add(this.label7);
            this.page_web_iis.Controls.Add(this.combo_iis_sdk_type);
            this.page_web_iis.Controls.Add(this.label6);
            this.page_web_iis.Controls.Add(this.label_iis_demo);
            this.page_web_iis.Controls.Add(this.tab_iis);
            this.page_web_iis.Location = new System.Drawing.Point(4, 22);
            this.page_web_iis.Name = "page_web_iis";
            this.page_web_iis.Padding = new System.Windows.Forms.Padding(3);
            this.page_web_iis.Size = new System.Drawing.Size(622, 456);
            this.page_web_iis.TabIndex = 0;
            this.page_web_iis.Text = "IIS_Web";
            this.page_web_iis.UseVisualStyleBackColor = true;
            // 
            // b_iis_rollback
            // 
            this.b_iis_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_iis_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_iis_rollback.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this.b_iis_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_iis_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_iis_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_iis_rollback.Location = new System.Drawing.Point(554, 3);
            this.b_iis_rollback.Name = "b_iis_rollback";
            this.b_iis_rollback.Radius = 10;
            this.b_iis_rollback.Size = new System.Drawing.Size(62, 29);
            this.b_iis_rollback.Stroke = false;
            this.b_iis_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_iis_rollback.TabIndex = 33;
            this.b_iis_rollback.Text = "RollBack";
            this.b_iis_rollback.Transparency = false;
            this.b_iis_rollback.Click += new System.EventHandler(this.b_iis_rollback_Click);
            // 
            // checkBox_Increment_iis
            // 
            this.checkBox_Increment_iis.AutoSize = true;
            this.checkBox_Increment_iis.Location = new System.Drawing.Point(299, 93);
            this.checkBox_Increment_iis.Name = "checkBox_Increment_iis";
            this.checkBox_Increment_iis.Size = new System.Drawing.Size(120, 16);
            this.checkBox_Increment_iis.TabIndex = 29;
            this.checkBox_Increment_iis.Text = "Increment Deploy";
            this.checkBox_Increment_iis.UseVisualStyleBackColor = true;
            this.checkBox_Increment_iis.CheckedChanged += new System.EventHandler(this.checkBox_Increment_iis_CheckedChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(95, 117);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(137, 12);
            this.label25.TabIndex = 17;
            this.label25.Text = "PS:Windows Server Only";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.txt_pool_name);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.txt_iis_port);
            this.groupBox4.Location = new System.Drawing.Point(299, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(244, 81);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = " (create site required)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(27, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 12);
            this.label15.TabIndex = 12;
            this.label15.Text = "Port:";
            // 
            // txt_pool_name
            // 
            this.txt_pool_name.Location = new System.Drawing.Point(68, 51);
            this.txt_pool_name.Name = "txt_pool_name";
            this.txt_pool_name.Size = new System.Drawing.Size(170, 21);
            this.txt_pool_name.TabIndex = 5;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 57);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 14;
            this.label16.Text = "PoolName:";
            // 
            // txt_iis_port
            // 
            this.txt_iis_port.Location = new System.Drawing.Point(68, 22);
            this.txt_iis_port.Name = "txt_iis_port";
            this.txt_iis_port.Size = new System.Drawing.Size(62, 21);
            this.txt_iis_port.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(95, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(185, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "Example: Default Web Site/test";
            // 
            // b_iis_deploy
            // 
            this.b_iis_deploy.Location = new System.Drawing.Point(509, 131);
            this.b_iis_deploy.Name = "b_iis_deploy";
            this.b_iis_deploy.Size = new System.Drawing.Size(107, 43);
            this.b_iis_deploy.TabIndex = 9;
            this.b_iis_deploy.Text = "Deploy";
            this.b_iis_deploy.UseVisualStyleBackColor = true;
            this.b_iis_deploy.Click += new System.EventHandler(this.b_iis_deploy_Click);
            // 
            // combo_iis_env
            // 
            this.combo_iis_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_env.FormattingEnabled = true;
            this.combo_iis_env.Location = new System.Drawing.Point(97, 94);
            this.combo_iis_env.Name = "combo_iis_env";
            this.combo_iis_env.Size = new System.Drawing.Size(161, 20);
            this.combo_iis_env.TabIndex = 8;
            this.combo_iis_env.SelectedIndexChanged += new System.EventHandler(this.combo_iis_env_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "Env Name:";
            // 
            // txt_iis_web_site_name
            // 
            this.txt_iis_web_site_name.Location = new System.Drawing.Point(97, 46);
            this.txt_iis_web_site_name.Name = "txt_iis_web_site_name";
            this.txt_iis_web_site_name.Size = new System.Drawing.Size(161, 21);
            this.txt_iis_web_site_name.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "WebSite Name:";
            // 
            // combo_iis_sdk_type
            // 
            this.combo_iis_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_sdk_type.FormattingEnabled = true;
            this.combo_iis_sdk_type.Items.AddRange(new object[] {
            "netframework",
            "netcore"});
            this.combo_iis_sdk_type.Location = new System.Drawing.Point(97, 9);
            this.combo_iis_sdk_type.Name = "combo_iis_sdk_type";
            this.combo_iis_sdk_type.Size = new System.Drawing.Size(161, 20);
            this.combo_iis_sdk_type.TabIndex = 1;
            this.combo_iis_sdk_type.SelectedIndexChanged += new System.EventHandler(this.combo_iis_sdk_type_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "SDK Type:";
            // 
            // label_iis_demo
            // 
            this.label_iis_demo.AutoSize = true;
            this.label_iis_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_iis_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_iis_demo.Location = new System.Drawing.Point(5, 3);
            this.label_iis_demo.Name = "label_iis_demo";
            this.label_iis_demo.Size = new System.Drawing.Size(29, 12);
            this.label_iis_demo.TabIndex = 28;
            this.label_iis_demo.Text = "Demo";
            this.label_iis_demo.Click += new System.EventHandler(this.label_iis_demo_Click);
            // 
            // tab_iis
            // 
            this.tab_iis.Controls.Add(this.tabPage_progress);
            this.tab_iis.Controls.Add(this.tabPage_iis_log);
            this.tab_iis.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tab_iis.Location = new System.Drawing.Point(3, 168);
            this.tab_iis.Name = "tab_iis";
            this.tab_iis.SelectedIndex = 0;
            this.tab_iis.Size = new System.Drawing.Size(616, 285);
            this.tab_iis.TabIndex = 16;
            // 
            // tabPage_progress
            // 
            this.tabPage_progress.AutoScroll = true;
            this.tabPage_progress.Controls.Add(this.progress_iis_tip);
            this.tabPage_progress.Location = new System.Drawing.Point(4, 22);
            this.tabPage_progress.Name = "tabPage_progress";
            this.tabPage_progress.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_progress.Size = new System.Drawing.Size(608, 259);
            this.tabPage_progress.TabIndex = 0;
            this.tabPage_progress.Text = "Progress";
            this.tabPage_progress.UseVisualStyleBackColor = true;
            // 
            // progress_iis_tip
            // 
            this.progress_iis_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_iis_tip.Location = new System.Drawing.Point(126, 64);
            this.progress_iis_tip.Name = "progress_iis_tip";
            this.progress_iis_tip.Size = new System.Drawing.Size(345, 29);
            this.progress_iis_tip.TabIndex = 17;
            this.progress_iis_tip.Text = "Please 1. Add Windows Server into Env In Setting Page.          2. Select Env.";
            // 
            // tabPage_iis_log
            // 
            this.tabPage_iis_log.Controls.Add(this.rich_iis_log);
            this.tabPage_iis_log.Location = new System.Drawing.Point(4, 22);
            this.tabPage_iis_log.Name = "tabPage_iis_log";
            this.tabPage_iis_log.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_iis_log.Size = new System.Drawing.Size(608, 259);
            this.tabPage_iis_log.TabIndex = 1;
            this.tabPage_iis_log.Text = "LOG";
            this.tabPage_iis_log.UseVisualStyleBackColor = true;
            // 
            // rich_iis_log
            // 
            this.rich_iis_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_iis_log.HiglightColor = AntDeploy.RtfColor.White;
            this.rich_iis_log.Location = new System.Drawing.Point(3, 3);
            this.rich_iis_log.Name = "rich_iis_log";
            this.rich_iis_log.ReadOnly = true;
            this.rich_iis_log.Size = new System.Drawing.Size(602, 253);
            this.rich_iis_log.TabIndex = 0;
            this.rich_iis_log.Text = "";
            this.rich_iis_log.TextColor = AntDeploy.RtfColor.Black;
            // 
            // page_docker
            // 
            this.page_docker.Controls.Add(this.label13);
            this.page_docker.Controls.Add(this.label12);
            this.page_docker.Controls.Add(this.t_docker_delete_days);
            this.page_docker.Controls.Add(this.label24);
            this.page_docker.Controls.Add(this.groupBox5);
            this.page_docker.Controls.Add(this.b_docker_deploy);
            this.page_docker.Controls.Add(this.combo_docker_env);
            this.page_docker.Controls.Add(this.label22);
            this.page_docker.Controls.Add(this.label_docker_demo);
            this.page_docker.Controls.Add(this.b_docker_rollback);
            this.page_docker.Controls.Add(this.tabControl_docker);
            this.page_docker.Location = new System.Drawing.Point(4, 22);
            this.page_docker.Name = "page_docker";
            this.page_docker.Size = new System.Drawing.Size(622, 456);
            this.page_docker.TabIndex = 2;
            this.page_docker.Text = "Docker";
            this.page_docker.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(587, 128);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 12);
            this.label13.TabIndex = 35;
            this.label13.Text = "Days";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(352, 109);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(186, 43);
            this.label12.TabIndex = 16;
            this.label12.Text = "Remove backup version in remote server that have been published for more than:";
            // 
            // t_docker_delete_days
            // 
            this.t_docker_delete_days.ForeColor = System.Drawing.Color.Blue;
            this.t_docker_delete_days.Location = new System.Drawing.Point(544, 125);
            this.t_docker_delete_days.Name = "t_docker_delete_days";
            this.t_docker_delete_days.Size = new System.Drawing.Size(37, 21);
            this.t_docker_delete_days.TabIndex = 15;
            this.t_docker_delete_days.Text = "10";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(31, 140);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(305, 12);
            this.label24.TabIndex = 15;
            this.label24.Text = "PS:Linux Server Only And Required Docker installed";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.txt_docker_volume);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.txt_docker_envname);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.txt_docker_port);
            this.groupBox5.Location = new System.Drawing.Point(37, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(501, 103);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = " (create Dockerfile required)";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(79, 77);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(365, 12);
            this.label28.TabIndex = 18;
            this.label28.Text = "example:/root/data:/container/datadir  Multiple split with ;";
            // 
            // txt_docker_volume
            // 
            this.txt_docker_volume.BackColor = System.Drawing.Color.Transparent;
            this.txt_docker_volume.Br = System.Drawing.Color.White;
            this.txt_docker_volume.Font = new System.Drawing.Font("Comic Sans MS", 11F);
            this.txt_docker_volume.ForeColor = System.Drawing.Color.DimGray;
            this.txt_docker_volume.Location = new System.Drawing.Point(69, 49);
            this.txt_docker_volume.Name = "txt_docker_volume";
            this.txt_docker_volume.Size = new System.Drawing.Size(405, 25);
            this.txt_docker_volume.TabIndex = 13;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(16, 56);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(47, 12);
            this.label27.TabIndex = 16;
            this.label27.Text = "Volume:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(27, 25);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 12);
            this.label21.TabIndex = 12;
            this.label21.Text = "Port:";
            // 
            // txt_docker_envname
            // 
            this.txt_docker_envname.Location = new System.Drawing.Point(319, 20);
            this.txt_docker_envname.Name = "txt_docker_envname";
            this.txt_docker_envname.Size = new System.Drawing.Size(170, 21);
            this.txt_docker_envname.TabIndex = 12;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(158, 23);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(143, 12);
            this.label23.TabIndex = 14;
            this.label23.Text = "ASPNETCORE_ENVIRONMENT:";
            // 
            // txt_docker_port
            // 
            this.txt_docker_port.Location = new System.Drawing.Point(68, 22);
            this.txt_docker_port.Name = "txt_docker_port";
            this.txt_docker_port.Size = new System.Drawing.Size(62, 21);
            this.txt_docker_port.TabIndex = 11;
            // 
            // b_docker_deploy
            // 
            this.b_docker_deploy.Location = new System.Drawing.Point(512, 164);
            this.b_docker_deploy.Name = "b_docker_deploy";
            this.b_docker_deploy.Size = new System.Drawing.Size(106, 40);
            this.b_docker_deploy.TabIndex = 12;
            this.b_docker_deploy.Text = "Deploy";
            this.b_docker_deploy.UseVisualStyleBackColor = true;
            this.b_docker_deploy.Click += new System.EventHandler(this.b_docker_deploy_Click);
            // 
            // combo_docker_env
            // 
            this.combo_docker_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_docker_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_docker_env.FormattingEnabled = true;
            this.combo_docker_env.Location = new System.Drawing.Point(118, 116);
            this.combo_docker_env.Name = "combo_docker_env";
            this.combo_docker_env.Size = new System.Drawing.Size(193, 20);
            this.combo_docker_env.TabIndex = 11;
            this.combo_docker_env.SelectedIndexChanged += new System.EventHandler(this.combo_docker_env_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(41, 119);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 12);
            this.label22.TabIndex = 10;
            this.label22.Text = "Env Name:";
            // 
            // label_docker_demo
            // 
            this.label_docker_demo.AutoSize = true;
            this.label_docker_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_docker_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_docker_demo.Location = new System.Drawing.Point(2, 3);
            this.label_docker_demo.Name = "label_docker_demo";
            this.label_docker_demo.Size = new System.Drawing.Size(29, 12);
            this.label_docker_demo.TabIndex = 27;
            this.label_docker_demo.Text = "Demo";
            this.label_docker_demo.Click += new System.EventHandler(this.label_docker_demo_Click);
            // 
            // b_docker_rollback
            // 
            this.b_docker_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_docker_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_docker_rollback.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this.b_docker_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_docker_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_docker_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_docker_rollback.Location = new System.Drawing.Point(556, 3);
            this.b_docker_rollback.Name = "b_docker_rollback";
            this.b_docker_rollback.Radius = 10;
            this.b_docker_rollback.Size = new System.Drawing.Size(60, 30);
            this.b_docker_rollback.Stroke = false;
            this.b_docker_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_docker_rollback.TabIndex = 34;
            this.b_docker_rollback.Text = "RollBack";
            this.b_docker_rollback.Transparency = false;
            this.b_docker_rollback.Click += new System.EventHandler(this.btn_docker_rollback_Click);
            // 
            // tabControl_docker
            // 
            this.tabControl_docker.Controls.Add(this.tabPage_docker);
            this.tabControl_docker.Controls.Add(this.tabPage_docker_log);
            this.tabControl_docker.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl_docker.Location = new System.Drawing.Point(0, 188);
            this.tabControl_docker.Name = "tabControl_docker";
            this.tabControl_docker.SelectedIndex = 0;
            this.tabControl_docker.Size = new System.Drawing.Size(622, 268);
            this.tabControl_docker.TabIndex = 18;
            // 
            // tabPage_docker
            // 
            this.tabPage_docker.Controls.Add(this.progress_docker_tip);
            this.tabPage_docker.Location = new System.Drawing.Point(4, 22);
            this.tabPage_docker.Name = "tabPage_docker";
            this.tabPage_docker.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_docker.Size = new System.Drawing.Size(614, 242);
            this.tabPage_docker.TabIndex = 0;
            this.tabPage_docker.Text = "Progress";
            this.tabPage_docker.UseVisualStyleBackColor = true;
            // 
            // progress_docker_tip
            // 
            this.progress_docker_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_docker_tip.Location = new System.Drawing.Point(127, 54);
            this.progress_docker_tip.Name = "progress_docker_tip";
            this.progress_docker_tip.Size = new System.Drawing.Size(331, 30);
            this.progress_docker_tip.TabIndex = 18;
            this.progress_docker_tip.Text = "Please 1. Add Linux Server into Env In Setting Page.          2. Select Env.";
            // 
            // tabPage_docker_log
            // 
            this.tabPage_docker_log.Controls.Add(this.rich_docker_log);
            this.tabPage_docker_log.Location = new System.Drawing.Point(4, 22);
            this.tabPage_docker_log.Name = "tabPage_docker_log";
            this.tabPage_docker_log.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_docker_log.Size = new System.Drawing.Size(614, 242);
            this.tabPage_docker_log.TabIndex = 1;
            this.tabPage_docker_log.Text = "LOG";
            this.tabPage_docker_log.UseVisualStyleBackColor = true;
            // 
            // rich_docker_log
            // 
            this.rich_docker_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_docker_log.HiglightColor = AntDeploy.RtfColor.White;
            this.rich_docker_log.Location = new System.Drawing.Point(3, 3);
            this.rich_docker_log.Name = "rich_docker_log";
            this.rich_docker_log.ReadOnly = true;
            this.rich_docker_log.Size = new System.Drawing.Size(608, 236);
            this.rich_docker_log.TabIndex = 1;
            this.rich_docker_log.Text = "";
            this.rich_docker_log.TextColor = AntDeploy.RtfColor.Black;
            // 
            // page_window_service
            // 
            this.page_window_service.Controls.Add(this.b_windows_service_rollback);
            this.page_window_service.Controls.Add(this.checkBox_Increment_window_service);
            this.page_window_service.Controls.Add(this.label_windows_serivce_demo);
            this.page_window_service.Controls.Add(this.label26);
            this.page_window_service.Controls.Add(this.txt_windowservice_name);
            this.page_window_service.Controls.Add(this.label14);
            this.page_window_service.Controls.Add(this.combo_windowservice_sdk_type);
            this.page_window_service.Controls.Add(this.label11);
            this.page_window_service.Controls.Add(this.b_windowservice_deploy);
            this.page_window_service.Controls.Add(this.combo_windowservice_env);
            this.page_window_service.Controls.Add(this.label10);
            this.page_window_service.Controls.Add(this.tabControl_window_service);
            this.page_window_service.Location = new System.Drawing.Point(4, 22);
            this.page_window_service.Name = "page_window_service";
            this.page_window_service.Size = new System.Drawing.Size(622, 456);
            this.page_window_service.TabIndex = 3;
            this.page_window_service.Text = "WindowsService";
            this.page_window_service.UseVisualStyleBackColor = true;
            // 
            // b_windows_service_rollback
            // 
            this.b_windows_service_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_windows_service_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_windows_service_rollback.Font = new System.Drawing.Font("Comic Sans MS", 10F, System.Drawing.FontStyle.Bold);
            this.b_windows_service_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_windows_service_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_windows_service_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_windows_service_rollback.Location = new System.Drawing.Point(554, 5);
            this.b_windows_service_rollback.Name = "b_windows_service_rollback";
            this.b_windows_service_rollback.Radius = 10;
            this.b_windows_service_rollback.Size = new System.Drawing.Size(60, 30);
            this.b_windows_service_rollback.Stroke = false;
            this.b_windows_service_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_windows_service_rollback.TabIndex = 35;
            this.b_windows_service_rollback.Text = "RollBack";
            this.b_windows_service_rollback.Transparency = false;
            this.b_windows_service_rollback.Click += new System.EventHandler(this.b_windows_service_rollback_Click);
            // 
            // checkBox_Increment_window_service
            // 
            this.checkBox_Increment_window_service.AutoSize = true;
            this.checkBox_Increment_window_service.Location = new System.Drawing.Point(361, 78);
            this.checkBox_Increment_window_service.Name = "checkBox_Increment_window_service";
            this.checkBox_Increment_window_service.Size = new System.Drawing.Size(120, 16);
            this.checkBox_Increment_window_service.TabIndex = 30;
            this.checkBox_Increment_window_service.Text = "Increment Deploy";
            this.checkBox_Increment_window_service.UseVisualStyleBackColor = true;
            this.checkBox_Increment_window_service.CheckedChanged += new System.EventHandler(this.checkBox_Increment_window_service_CheckedChanged);
            // 
            // label_windows_serivce_demo
            // 
            this.label_windows_serivce_demo.AutoSize = true;
            this.label_windows_serivce_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_windows_serivce_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_windows_serivce_demo.Location = new System.Drawing.Point(3, 5);
            this.label_windows_serivce_demo.Name = "label_windows_serivce_demo";
            this.label_windows_serivce_demo.Size = new System.Drawing.Size(29, 12);
            this.label_windows_serivce_demo.TabIndex = 26;
            this.label_windows_serivce_demo.Text = "Demo";
            this.label_windows_serivce_demo.Click += new System.EventHandler(this.label_windows_serivce_demo_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(136, 98);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(137, 12);
            this.label26.TabIndex = 25;
            this.label26.Text = "PS:Windows Server Only";
            // 
            // txt_windowservice_name
            // 
            this.txt_windowservice_name.Location = new System.Drawing.Point(138, 48);
            this.txt_windowservice_name.Name = "txt_windowservice_name";
            this.txt_windowservice_name.Size = new System.Drawing.Size(167, 21);
            this.txt_windowservice_name.TabIndex = 22;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(54, 51);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(83, 12);
            this.label14.TabIndex = 21;
            this.label14.Text = "Service Name:";
            // 
            // combo_windowservice_sdk_type
            // 
            this.combo_windowservice_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_sdk_type.FormattingEnabled = true;
            this.combo_windowservice_sdk_type.Items.AddRange(new object[] {
            "netframework",
            "netcore"});
            this.combo_windowservice_sdk_type.Location = new System.Drawing.Point(138, 20);
            this.combo_windowservice_sdk_type.Name = "combo_windowservice_sdk_type";
            this.combo_windowservice_sdk_type.Size = new System.Drawing.Size(167, 20);
            this.combo_windowservice_sdk_type.TabIndex = 20;
            this.combo_windowservice_sdk_type.SelectedIndexChanged += new System.EventHandler(this.combo_windowservice_sdk_type_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(78, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 19;
            this.label11.Text = "SDK Type:";
            // 
            // b_windowservice_deploy
            // 
            this.b_windowservice_deploy.Location = new System.Drawing.Point(511, 114);
            this.b_windowservice_deploy.Name = "b_windowservice_deploy";
            this.b_windowservice_deploy.Size = new System.Drawing.Size(107, 43);
            this.b_windowservice_deploy.TabIndex = 14;
            this.b_windowservice_deploy.Text = "Deploy";
            this.b_windowservice_deploy.UseVisualStyleBackColor = true;
            this.b_windowservice_deploy.Click += new System.EventHandler(this.b_windowservice_deploy_Click);
            // 
            // combo_windowservice_env
            // 
            this.combo_windowservice_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_env.FormattingEnabled = true;
            this.combo_windowservice_env.Location = new System.Drawing.Point(138, 75);
            this.combo_windowservice_env.Name = "combo_windowservice_env";
            this.combo_windowservice_env.Size = new System.Drawing.Size(193, 20);
            this.combo_windowservice_env.TabIndex = 13;
            this.combo_windowservice_env.SelectedIndexChanged += new System.EventHandler(this.combo_windowservice_env_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(72, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 12;
            this.label10.Text = "Env Name:";
            // 
            // tabControl_window_service
            // 
            this.tabControl_window_service.Controls.Add(this.tabPage_windows_service);
            this.tabControl_window_service.Controls.Add(this.tabPage2);
            this.tabControl_window_service.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl_window_service.Location = new System.Drawing.Point(0, 150);
            this.tabControl_window_service.Name = "tabControl_window_service";
            this.tabControl_window_service.SelectedIndex = 0;
            this.tabControl_window_service.Size = new System.Drawing.Size(622, 306);
            this.tabControl_window_service.TabIndex = 24;
            // 
            // tabPage_windows_service
            // 
            this.tabPage_windows_service.Controls.Add(this.progress_window_service_tip);
            this.tabPage_windows_service.Location = new System.Drawing.Point(4, 22);
            this.tabPage_windows_service.Name = "tabPage_windows_service";
            this.tabPage_windows_service.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_windows_service.Size = new System.Drawing.Size(614, 280);
            this.tabPage_windows_service.TabIndex = 0;
            this.tabPage_windows_service.Text = "Progress";
            this.tabPage_windows_service.UseVisualStyleBackColor = true;
            // 
            // progress_window_service_tip
            // 
            this.progress_window_service_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_window_service_tip.Location = new System.Drawing.Point(132, 59);
            this.progress_window_service_tip.Name = "progress_window_service_tip";
            this.progress_window_service_tip.Size = new System.Drawing.Size(345, 30);
            this.progress_window_service_tip.TabIndex = 18;
            this.progress_window_service_tip.Text = "Please 1. Add Windows Server into Env In Setting Page.          2. Select Env.";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rich_windowservice_log);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(614, 280);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "LOG";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rich_windowservice_log
            // 
            this.rich_windowservice_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_windowservice_log.HiglightColor = AntDeploy.RtfColor.White;
            this.rich_windowservice_log.Location = new System.Drawing.Point(3, 3);
            this.rich_windowservice_log.Name = "rich_windowservice_log";
            this.rich_windowservice_log.ReadOnly = true;
            this.rich_windowservice_log.Size = new System.Drawing.Size(608, 274);
            this.rich_windowservice_log.TabIndex = 1;
            this.rich_windowservice_log.Text = "";
            this.rich_windowservice_log.TextColor = AntDeploy.RtfColor.Black;
            // 
            // page_set
            // 
            this.page_set.Controls.Add(this.groupBox1);
            this.page_set.Controls.Add(this.label_check_update);
            this.page_set.Controls.Add(this.groupBoxIgnore);
            this.page_set.Controls.Add(this.environment);
            this.page_set.Location = new System.Drawing.Point(4, 22);
            this.page_set.Name = "page_set";
            this.page_set.Padding = new System.Windows.Forms.Padding(3);
            this.page_set.Size = new System.Drawing.Size(622, 456);
            this.page_set.TabIndex = 1;
            this.page_set.Text = "Setting";
            this.page_set.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.b_backUp_ignore_remove);
            this.groupBox1.Controls.Add(this.b_backUp_ignore_add);
            this.groupBox1.Controls.Add(this.txt_backUp_ignore);
            this.groupBox1.Controls.Add(this.list_backUp_ignore);
            this.groupBox1.Location = new System.Drawing.Point(329, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 179);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Windows Server BackUp IgnoreRule";
            // 
            // b_backUp_ignore_remove
            // 
            this.b_backUp_ignore_remove.ForeColor = System.Drawing.Color.Red;
            this.b_backUp_ignore_remove.Location = new System.Drawing.Point(6, 141);
            this.b_backUp_ignore_remove.Name = "b_backUp_ignore_remove";
            this.b_backUp_ignore_remove.Size = new System.Drawing.Size(117, 23);
            this.b_backUp_ignore_remove.TabIndex = 12;
            this.b_backUp_ignore_remove.Text = "Remove Selected";
            this.b_backUp_ignore_remove.UseVisualStyleBackColor = true;
            this.b_backUp_ignore_remove.Click += new System.EventHandler(this.b_backUp_ignore_remove_Click);
            // 
            // b_backUp_ignore_add
            // 
            this.b_backUp_ignore_add.Location = new System.Drawing.Point(62, 79);
            this.b_backUp_ignore_add.Name = "b_backUp_ignore_add";
            this.b_backUp_ignore_add.Size = new System.Drawing.Size(59, 24);
            this.b_backUp_ignore_add.TabIndex = 17;
            this.b_backUp_ignore_add.Text = "Add";
            this.b_backUp_ignore_add.UseVisualStyleBackColor = true;
            this.b_backUp_ignore_add.Click += new System.EventHandler(this.b_backUp_ignore_add_Click);
            // 
            // txt_backUp_ignore
            // 
            this.txt_backUp_ignore.Location = new System.Drawing.Point(6, 43);
            this.txt_backUp_ignore.Name = "txt_backUp_ignore";
            this.txt_backUp_ignore.Size = new System.Drawing.Size(115, 21);
            this.txt_backUp_ignore.TabIndex = 4;
            // 
            // list_backUp_ignore
            // 
            this.list_backUp_ignore.FormattingEnabled = true;
            this.list_backUp_ignore.ItemHeight = 12;
            this.list_backUp_ignore.Location = new System.Drawing.Point(131, 13);
            this.list_backUp_ignore.Name = "list_backUp_ignore";
            this.list_backUp_ignore.Size = new System.Drawing.Size(152, 160);
            this.list_backUp_ignore.TabIndex = 15;
            // 
            // label_check_update
            // 
            this.label_check_update.AutoSize = true;
            this.label_check_update.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_check_update.Location = new System.Drawing.Point(543, 3);
            this.label_check_update.Name = "label_check_update";
            this.label_check_update.Size = new System.Drawing.Size(77, 12);
            this.label_check_update.TabIndex = 3;
            this.label_check_update.Text = "Check Update";
            this.label_check_update.Click += new System.EventHandler(this.label_check_update_Click);
            // 
            // groupBoxIgnore
            // 
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_remove);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_add);
            this.groupBoxIgnore.Controls.Add(this.txt_env_ignore);
            this.groupBoxIgnore.Controls.Add(this.list_env_ignore);
            this.groupBoxIgnore.Location = new System.Drawing.Point(19, 269);
            this.groupBoxIgnore.Name = "groupBoxIgnore";
            this.groupBoxIgnore.Size = new System.Drawing.Size(304, 179);
            this.groupBoxIgnore.TabIndex = 2;
            this.groupBoxIgnore.TabStop = false;
            this.groupBoxIgnore.Text = "Package IgnoreRule";
            // 
            // b_env_ignore_remove
            // 
            this.b_env_ignore_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_ignore_remove.Location = new System.Drawing.Point(21, 141);
            this.b_env_ignore_remove.Name = "b_env_ignore_remove";
            this.b_env_ignore_remove.Size = new System.Drawing.Size(117, 23);
            this.b_env_ignore_remove.TabIndex = 12;
            this.b_env_ignore_remove.Text = "Remove Selected";
            this.b_env_ignore_remove.UseVisualStyleBackColor = true;
            this.b_env_ignore_remove.Click += new System.EventHandler(this.b_env_ignore_remove_Click);
            // 
            // b_env_ignore_add
            // 
            this.b_env_ignore_add.Location = new System.Drawing.Point(79, 79);
            this.b_env_ignore_add.Name = "b_env_ignore_add";
            this.b_env_ignore_add.Size = new System.Drawing.Size(59, 24);
            this.b_env_ignore_add.TabIndex = 17;
            this.b_env_ignore_add.Text = "Add";
            this.b_env_ignore_add.UseVisualStyleBackColor = true;
            this.b_env_ignore_add.Click += new System.EventHandler(this.b_env_ignore_add_Click);
            // 
            // txt_env_ignore
            // 
            this.txt_env_ignore.Location = new System.Drawing.Point(6, 43);
            this.txt_env_ignore.Name = "txt_env_ignore";
            this.txt_env_ignore.Size = new System.Drawing.Size(132, 21);
            this.txt_env_ignore.TabIndex = 4;
            // 
            // list_env_ignore
            // 
            this.list_env_ignore.FormattingEnabled = true;
            this.list_env_ignore.ItemHeight = 12;
            this.list_env_ignore.Location = new System.Drawing.Point(144, 13);
            this.list_env_ignore.Name = "list_env_ignore";
            this.list_env_ignore.Size = new System.Drawing.Size(157, 160);
            this.list_env_ignore.TabIndex = 15;
            // 
            // environment
            // 
            this.environment.Controls.Add(this.tabControl1);
            this.environment.Controls.Add(this.label2);
            this.environment.Controls.Add(this.label1);
            this.environment.Controls.Add(this.b_env_remove);
            this.environment.Controls.Add(this.txt_env_name);
            this.environment.Controls.Add(this.b_env_add_by_name);
            this.environment.Controls.Add(this.combo_env_list);
            this.environment.Location = new System.Drawing.Point(19, 17);
            this.environment.Name = "environment";
            this.environment.Size = new System.Drawing.Size(599, 246);
            this.environment.TabIndex = 0;
            this.environment.TabStop = false;
            this.environment.Text = "Environment";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.page_winserver);
            this.tabControl1.Controls.Add(this.page_linux_server);
            this.tabControl1.Location = new System.Drawing.Point(9, 88);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 147);
            this.tabControl1.TabIndex = 3;
            // 
            // page_winserver
            // 
            this.page_winserver.Controls.Add(this.loading_win_server_test);
            this.page_winserver.Controls.Add(this.label5);
            this.page_winserver.Controls.Add(this.b_env_server_test);
            this.page_winserver.Controls.Add(this.combo_env_server_list);
            this.page_winserver.Controls.Add(this.label4);
            this.page_winserver.Controls.Add(this.b_env_server_add);
            this.page_winserver.Controls.Add(this.txt_env_server_host);
            this.page_winserver.Controls.Add(this.b_env_server_remove);
            this.page_winserver.Controls.Add(this.label3);
            this.page_winserver.Controls.Add(this.txt_env_server_token);
            this.page_winserver.Location = new System.Drawing.Point(4, 22);
            this.page_winserver.Name = "page_winserver";
            this.page_winserver.Padding = new System.Windows.Forms.Padding(3);
            this.page_winserver.Size = new System.Drawing.Size(576, 121);
            this.page_winserver.TabIndex = 0;
            this.page_winserver.Text = "win_server";
            this.page_winserver.UseVisualStyleBackColor = true;
            // 
            // loading_win_server_test
            // 
            this.loading_win_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_win_server_test.FullTransparent = true;
            this.loading_win_server_test.Increment = 1F;
            this.loading_win_server_test.Location = new System.Drawing.Point(222, 6);
            this.loading_win_server_test.N = 8;
            this.loading_win_server_test.Name = "loading_win_server_test";
            this.loading_win_server_test.Radius = 2.5F;
            this.loading_win_server_test.Size = new System.Drawing.Size(90, 100);
            this.loading_win_server_test.TabIndex = 12;
            this.loading_win_server_test.Text = "spinningCircles1";
            this.loading_win_server_test.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Server List：";
            // 
            // b_env_server_test
            // 
            this.b_env_server_test.Enabled = false;
            this.b_env_server_test.Location = new System.Drawing.Point(207, 53);
            this.b_env_server_test.Name = "b_env_server_test";
            this.b_env_server_test.Size = new System.Drawing.Size(129, 23);
            this.b_env_server_test.TabIndex = 11;
            this.b_env_server_test.Text = "Connect Test";
            this.b_env_server_test.UseVisualStyleBackColor = true;
            this.b_env_server_test.Click += new System.EventHandler(this.b_env_server_test_Click);
            // 
            // combo_env_server_list
            // 
            this.combo_env_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_server_list.FormattingEnabled = true;
            this.combo_env_server_list.Location = new System.Drawing.Point(98, 95);
            this.combo_env_server_list.Name = "combo_env_server_list";
            this.combo_env_server_list.Size = new System.Drawing.Size(464, 20);
            this.combo_env_server_list.TabIndex = 7;
            this.combo_env_server_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_server_list_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(209, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Token：";
            // 
            // b_env_server_add
            // 
            this.b_env_server_add.Enabled = false;
            this.b_env_server_add.Location = new System.Drawing.Point(14, 53);
            this.b_env_server_add.Name = "b_env_server_add";
            this.b_env_server_add.Size = new System.Drawing.Size(129, 23);
            this.b_env_server_add.TabIndex = 7;
            this.b_env_server_add.Text = "Add Server";
            this.b_env_server_add.UseVisualStyleBackColor = true;
            this.b_env_server_add.Click += new System.EventHandler(this.b_env_server_add_Click);
            // 
            // txt_env_server_host
            // 
            this.txt_env_server_host.Location = new System.Drawing.Point(59, 16);
            this.txt_env_server_host.Name = "txt_env_server_host";
            this.txt_env_server_host.Size = new System.Drawing.Size(125, 21);
            this.txt_env_server_host.TabIndex = 2;
            // 
            // b_env_server_remove
            // 
            this.b_env_server_remove.Enabled = false;
            this.b_env_server_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_server_remove.Location = new System.Drawing.Point(414, 53);
            this.b_env_server_remove.Name = "b_env_server_remove";
            this.b_env_server_remove.Size = new System.Drawing.Size(140, 23);
            this.b_env_server_remove.TabIndex = 8;
            this.b_env_server_remove.Text = "Remove Selected";
            this.b_env_server_remove.UseVisualStyleBackColor = true;
            this.b_env_server_remove.Click += new System.EventHandler(this.b_env_server_remove_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Host：";
            // 
            // txt_env_server_token
            // 
            this.txt_env_server_token.Location = new System.Drawing.Point(262, 16);
            this.txt_env_server_token.Name = "txt_env_server_token";
            this.txt_env_server_token.Size = new System.Drawing.Size(303, 21);
            this.txt_env_server_token.TabIndex = 3;
            // 
            // page_linux_server
            // 
            this.page_linux_server.Controls.Add(this.loading_linux_server_test);
            this.page_linux_server.Controls.Add(this.label20);
            this.page_linux_server.Controls.Add(this.combo_linux_server_list);
            this.page_linux_server.Controls.Add(this.txt_linux_pwd);
            this.page_linux_server.Controls.Add(this.label19);
            this.page_linux_server.Controls.Add(this.txt_linux_username);
            this.page_linux_server.Controls.Add(this.label18);
            this.page_linux_server.Controls.Add(this.b_linux_server_test);
            this.page_linux_server.Controls.Add(this.b_add_linux_server);
            this.page_linux_server.Controls.Add(this.b_linux_server_remove);
            this.page_linux_server.Controls.Add(this.txt_linux_host);
            this.page_linux_server.Controls.Add(this.label17);
            this.page_linux_server.Location = new System.Drawing.Point(4, 22);
            this.page_linux_server.Name = "page_linux_server";
            this.page_linux_server.Padding = new System.Windows.Forms.Padding(3);
            this.page_linux_server.Size = new System.Drawing.Size(576, 121);
            this.page_linux_server.TabIndex = 1;
            this.page_linux_server.Text = "linux_server";
            this.page_linux_server.UseVisualStyleBackColor = true;
            // 
            // loading_linux_server_test
            // 
            this.loading_linux_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_linux_server_test.FullTransparent = true;
            this.loading_linux_server_test.Increment = 1F;
            this.loading_linux_server_test.Location = new System.Drawing.Point(222, 11);
            this.loading_linux_server_test.N = 8;
            this.loading_linux_server_test.Name = "loading_linux_server_test";
            this.loading_linux_server_test.Radius = 2.5F;
            this.loading_linux_server_test.Size = new System.Drawing.Size(90, 100);
            this.loading_linux_server_test.TabIndex = 21;
            this.loading_linux_server_test.Text = "spinningCircles1";
            this.loading_linux_server_test.Visible = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(5, 98);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(83, 12);
            this.label20.TabIndex = 20;
            this.label20.Text = "Server List：";
            // 
            // combo_linux_server_list
            // 
            this.combo_linux_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_linux_server_list.FormattingEnabled = true;
            this.combo_linux_server_list.Location = new System.Drawing.Point(94, 95);
            this.combo_linux_server_list.Name = "combo_linux_server_list";
            this.combo_linux_server_list.Size = new System.Drawing.Size(464, 20);
            this.combo_linux_server_list.TabIndex = 19;
            this.combo_linux_server_list.SelectedIndexChanged += new System.EventHandler(this.combo_linux_server_list_SelectedIndexChanged);
            // 
            // txt_linux_pwd
            // 
            this.txt_linux_pwd.Location = new System.Drawing.Point(437, 11);
            this.txt_linux_pwd.Name = "txt_linux_pwd";
            this.txt_linux_pwd.PasswordChar = '*';
            this.txt_linux_pwd.Size = new System.Drawing.Size(125, 21);
            this.txt_linux_pwd.TabIndex = 11;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(398, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(35, 12);
            this.label19.TabIndex = 18;
            this.label19.Text = "Pwd：";
            // 
            // txt_linux_username
            // 
            this.txt_linux_username.Location = new System.Drawing.Point(264, 11);
            this.txt_linux_username.Name = "txt_linux_username";
            this.txt_linux_username.Size = new System.Drawing.Size(115, 21);
            this.txt_linux_username.TabIndex = 10;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(196, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(65, 12);
            this.label18.TabIndex = 16;
            this.label18.Text = "UserName：";
            // 
            // b_linux_server_test
            // 
            this.b_linux_server_test.Enabled = false;
            this.b_linux_server_test.Location = new System.Drawing.Point(211, 49);
            this.b_linux_server_test.Name = "b_linux_server_test";
            this.b_linux_server_test.Size = new System.Drawing.Size(129, 23);
            this.b_linux_server_test.TabIndex = 14;
            this.b_linux_server_test.Text = "Connect Test";
            this.b_linux_server_test.UseVisualStyleBackColor = true;
            this.b_linux_server_test.Click += new System.EventHandler(this.b_linux_server_test_Click);
            // 
            // b_add_linux_server
            // 
            this.b_add_linux_server.Enabled = false;
            this.b_add_linux_server.Location = new System.Drawing.Point(18, 49);
            this.b_add_linux_server.Name = "b_add_linux_server";
            this.b_add_linux_server.Size = new System.Drawing.Size(129, 23);
            this.b_add_linux_server.TabIndex = 12;
            this.b_add_linux_server.Text = "Add Server";
            this.b_add_linux_server.UseVisualStyleBackColor = true;
            this.b_add_linux_server.Click += new System.EventHandler(this.b_add_linux_server_Click);
            // 
            // b_linux_server_remove
            // 
            this.b_linux_server_remove.Enabled = false;
            this.b_linux_server_remove.ForeColor = System.Drawing.Color.Red;
            this.b_linux_server_remove.Location = new System.Drawing.Point(418, 49);
            this.b_linux_server_remove.Name = "b_linux_server_remove";
            this.b_linux_server_remove.Size = new System.Drawing.Size(140, 23);
            this.b_linux_server_remove.TabIndex = 13;
            this.b_linux_server_remove.Text = "Remove Selected";
            this.b_linux_server_remove.UseVisualStyleBackColor = true;
            this.b_linux_server_remove.Click += new System.EventHandler(this.b_linux_server_remove_Click);
            // 
            // txt_linux_host
            // 
            this.txt_linux_host.Location = new System.Drawing.Point(57, 11);
            this.txt_linux_host.Name = "txt_linux_host";
            this.txt_linux_host.Size = new System.Drawing.Size(128, 21);
            this.txt_linux_host.TabIndex = 9;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(41, 12);
            this.label17.TabIndex = 10;
            this.label17.Text = "Host：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Env Name：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Env List：";
            // 
            // b_env_remove
            // 
            this.b_env_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_remove.Location = new System.Drawing.Point(381, 59);
            this.b_env_remove.Name = "b_env_remove";
            this.b_env_remove.Size = new System.Drawing.Size(140, 23);
            this.b_env_remove.TabIndex = 4;
            this.b_env_remove.Text = "Remove Selected";
            this.b_env_remove.UseVisualStyleBackColor = true;
            this.b_env_remove.Click += new System.EventHandler(this.b_env_remove_Click);
            // 
            // txt_env_name
            // 
            this.txt_env_name.Location = new System.Drawing.Point(92, 20);
            this.txt_env_name.Name = "txt_env_name";
            this.txt_env_name.Size = new System.Drawing.Size(125, 21);
            this.txt_env_name.TabIndex = 1;
            // 
            // b_env_add_by_name
            // 
            this.b_env_add_by_name.Location = new System.Drawing.Point(235, 18);
            this.b_env_add_by_name.Name = "b_env_add_by_name";
            this.b_env_add_by_name.Size = new System.Drawing.Size(129, 23);
            this.b_env_add_by_name.TabIndex = 1;
            this.b_env_add_by_name.Text = "Add By Name";
            this.b_env_add_by_name.UseVisualStyleBackColor = true;
            this.b_env_add_by_name.Click += new System.EventHandler(this.b_env_add_by_name_Click);
            // 
            // combo_env_list
            // 
            this.combo_env_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_list.FormattingEnabled = true;
            this.combo_env_list.Location = new System.Drawing.Point(92, 61);
            this.combo_env_list.Name = "combo_env_list";
            this.combo_env_list.Size = new System.Drawing.Size(248, 20);
            this.combo_env_list.TabIndex = 0;
            this.combo_env_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_list_SelectedIndexChanged);
            // 
            // Deploy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(630, 482);
            this.Controls.Add(this.tabcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Deploy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AntDeploy";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Deploy_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Deploy_FormClosing);
            this.Load += new System.EventHandler(this.Deploy_Load);
            this.tabcontrol.ResumeLayout(false);
            this.page_web_iis.ResumeLayout(false);
            this.page_web_iis.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tab_iis.ResumeLayout(false);
            this.tabPage_progress.ResumeLayout(false);
            this.tabPage_iis_log.ResumeLayout(false);
            this.page_docker.ResumeLayout(false);
            this.page_docker.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl_docker.ResumeLayout(false);
            this.tabPage_docker.ResumeLayout(false);
            this.tabPage_docker_log.ResumeLayout(false);
            this.page_window_service.ResumeLayout(false);
            this.page_window_service.PerformLayout();
            this.tabControl_window_service.ResumeLayout(false);
            this.tabPage_windows_service.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.page_set.ResumeLayout(false);
            this.page_set.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxIgnore.ResumeLayout(false);
            this.groupBoxIgnore.PerformLayout();
            this.environment.ResumeLayout(false);
            this.environment.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.page_winserver.ResumeLayout(false);
            this.page_winserver.PerformLayout();
            this.page_linux_server.ResumeLayout(false);
            this.page_linux_server.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabcontrol;
        private System.Windows.Forms.TabPage page_web_iis;
        private System.Windows.Forms.TabPage page_set;
        private System.Windows.Forms.GroupBox environment;
        private System.Windows.Forms.Button b_env_add_by_name;
        private System.Windows.Forms.ComboBox combo_env_list;
        private System.Windows.Forms.TextBox txt_env_name;
        private System.Windows.Forms.Button b_env_remove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_env_server_host;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_env_server_token;
        private System.Windows.Forms.Button b_env_server_add;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button b_env_server_remove;
        private System.Windows.Forms.ComboBox combo_env_server_list;
        private System.Windows.Forms.Button b_env_server_test;
        private System.Windows.Forms.TabPage page_docker;
        private System.Windows.Forms.TabPage page_window_service;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox combo_iis_sdk_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_iis_web_site_name;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox combo_iis_env;
        private System.Windows.Forms.Button b_iis_deploy;
        private System.Windows.Forms.GroupBox groupBoxIgnore;
        private System.Windows.Forms.Button b_env_ignore_remove;
        private System.Windows.Forms.Button b_env_ignore_add;
        private System.Windows.Forms.TextBox txt_env_ignore;
        private System.Windows.Forms.ListBox list_env_ignore;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button b_windowservice_deploy;
        private System.Windows.Forms.ComboBox combo_windowservice_env;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox combo_windowservice_sdk_type;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_windowservice_name;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_iis_port;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_pool_name;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage page_winserver;
        private System.Windows.Forms.TabPage page_linux_server;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox combo_linux_server_list;
        private System.Windows.Forms.TextBox txt_linux_pwd;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txt_linux_username;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button b_linux_server_test;
        private System.Windows.Forms.Button b_add_linux_server;
        private System.Windows.Forms.Button b_linux_server_remove;
        private System.Windows.Forms.TextBox txt_linux_host;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button b_docker_deploy;
        private System.Windows.Forms.ComboBox combo_docker_env;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txt_docker_envname;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txt_docker_port;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label_check_update;
        private System.Windows.Forms.TabControl tab_iis;
        private System.Windows.Forms.TabPage tabPage_progress;
        private System.Windows.Forms.TabPage tabPage_iis_log;
        private System.Windows.Forms.TabControl tabControl_docker;
        private System.Windows.Forms.TabPage tabPage_docker;
        private System.Windows.Forms.TabPage tabPage_docker_log;
        private System.Windows.Forms.TabControl tabControl_window_service;
        private System.Windows.Forms.TabPage tabPage_windows_service;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label progress_iis_tip;
        private System.Windows.Forms.Label progress_docker_tip;
        private System.Windows.Forms.Label progress_window_service_tip;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label_windows_serivce_demo;
        private System.Windows.Forms.Label label_docker_demo;
        private System.Windows.Forms.Label label_iis_demo;
        private System.Windows.Forms.CheckBox checkBox_Increment_iis;
        private System.Windows.Forms.CheckBox checkBox_Increment_window_service;
        private AltoControls.AltoButton b_iis_rollback;
        private AltoControls.AltoButton b_docker_rollback;
        private AltoControls.AltoButton b_windows_service_rollback;
        private AltoControls.SpinningCircles loading_win_server_test;
        private AltoControls.SpinningCircles loading_linux_server_test;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox t_docker_delete_days;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button b_backUp_ignore_remove;
        private System.Windows.Forms.Button b_backUp_ignore_add;
        private System.Windows.Forms.TextBox txt_backUp_ignore;
        private System.Windows.Forms.ListBox list_backUp_ignore;
        private ExRichTextBox rich_iis_log;
        private ExRichTextBox rich_docker_log;
        private ExRichTextBox rich_windowservice_log;
        private AltoControls.AltoTextBox txt_docker_volume;
        private System.Windows.Forms.Label label28;
    }
}