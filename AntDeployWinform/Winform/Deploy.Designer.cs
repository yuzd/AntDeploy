namespace AntDeployWinform.Winform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Deploy));
            this.tabcontrol = new System.Windows.Forms.TabControl();
            this.page_web_iis = new System.Windows.Forms.TabPage();
            this.btn_iis_stop = new AltoControls.AltoButton();
            this.btn_iis_retry = new AltoControls.AltoButton();
            this.checkBox_select_deploy_iis = new System.Windows.Forms.CheckBox();
            this.b_iis_rollback = new AltoControls.AltoButton();
            this.checkBox_Increment_iis = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.b_iis_deploy = new AltoControls.AltoButton();
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
            this.rich_iis_log = new AntDeployWinform.ExRichTextBox();
            this.page_docker = new System.Windows.Forms.TabPage();
            this.checkBox_select_deploy_docker = new System.Windows.Forms.CheckBox();
            this.checkBox_Increment_docker = new System.Windows.Forms.CheckBox();
            this.btn_docker_stop = new AltoControls.AltoButton();
            this.btn_docker_retry = new AltoControls.AltoButton();
            this.b_docker_deploy = new AltoControls.AltoButton();
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
            this.combo_docker_env = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label_docker_demo = new System.Windows.Forms.Label();
            this.tabControl_docker = new System.Windows.Forms.TabControl();
            this.tabPage_docker = new System.Windows.Forms.TabPage();
            this.progress_docker_tip = new System.Windows.Forms.Label();
            this.tabPage_docker_log = new System.Windows.Forms.TabPage();
            this.rich_docker_log = new AntDeployWinform.ExRichTextBox();
            this.b_docker_rollback = new AltoControls.AltoButton();
            this.page_window_service = new System.Windows.Forms.TabPage();
            this.btn_windows_serivce_stop = new AltoControls.AltoButton();
            this.btn_windows_service_retry = new AltoControls.AltoButton();
            this.b_windowservice_deploy = new AltoControls.AltoButton();
            this.checkBox_select_deploy_service = new System.Windows.Forms.CheckBox();
            this.checkBox_Increment_window_service = new System.Windows.Forms.CheckBox();
            this.label_windows_serivce_demo = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txt_windowservice_name = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.combo_windowservice_sdk_type = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.combo_windowservice_env = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabControl_window_service = new System.Windows.Forms.TabControl();
            this.tabPage_windows_service = new System.Windows.Forms.TabPage();
            this.progress_window_service_tip = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rich_windowservice_log = new AntDeployWinform.ExRichTextBox();
            this.b_windows_service_rollback = new AltoControls.AltoButton();
            this.page_set = new System.Windows.Forms.TabPage();
            this.label_how_to_set = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.b_copy_backup_ignore = new System.Windows.Forms.Button();
            this.b_backUp_ignore_remove = new System.Windows.Forms.Button();
            this.b_backUp_ignore_add = new System.Windows.Forms.Button();
            this.txt_backUp_ignore = new System.Windows.Forms.TextBox();
            this.list_backUp_ignore = new System.Windows.Forms.ListBox();
            this.label_check_update = new System.Windows.Forms.Label();
            this.groupBoxIgnore = new System.Windows.Forms.GroupBox();
            this.b_copy_pack_ignore = new System.Windows.Forms.Button();
            this.b_env_ignore_remove = new System.Windows.Forms.Button();
            this.b_env_ignore_add = new System.Windows.Forms.Button();
            this.txt_env_ignore = new System.Windows.Forms.TextBox();
            this.list_env_ignore = new System.Windows.Forms.ListBox();
            this.environment = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.page_winserver = new System.Windows.Forms.TabPage();
            this.txt_winserver_nickname = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.loading_win_server_test = new AltoControls.SpinningCircles();
            this.label5 = new System.Windows.Forms.Label();
            this.b_env_server_test = new System.Windows.Forms.Button();
            this.combo_env_server_list = new System.Windows.Forms.ComboBox();
            this.b_env_server_add = new System.Windows.Forms.Button();
            this.txt_env_server_host = new System.Windows.Forms.TextBox();
            this.b_env_server_remove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_env_server_token = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.page_linux_server = new System.Windows.Forms.TabPage();
            this.txt_linux_server_nickname = new System.Windows.Forms.TextBox();
            this.loading_linux_server_test = new AltoControls.SpinningCircles();
            this.label20 = new System.Windows.Forms.Label();
            this.combo_linux_server_list = new System.Windows.Forms.ComboBox();
            this.txt_linux_pwd = new System.Windows.Forms.TextBox();
            this.txt_linux_username = new System.Windows.Forms.TextBox();
            this.b_linux_server_test = new System.Windows.Forms.Button();
            this.b_add_linux_server = new System.Windows.Forms.Button();
            this.b_linux_server_remove = new System.Windows.Forms.Button();
            this.txt_linux_host = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.b_env_remove = new System.Windows.Forms.Button();
            this.txt_env_name = new System.Windows.Forms.TextBox();
            this.b_env_add_by_name = new System.Windows.Forms.Button();
            this.combo_env_list = new System.Windows.Forms.ComboBox();
            this.pag_advance_setting = new System.Windows.Forms.TabPage();
            this.checkBox_multi_deploy = new System.Windows.Forms.CheckBox();
            this.checkBox_save_deploy_log = new System.Windows.Forms.CheckBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txt_http_proxy = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.btn_shang = new AltoControls.AltoButton();
            this.label16 = new System.Windows.Forms.Label();
            this.btn_folder_clear = new System.Windows.Forms.Button();
            this.btn_choose_folder = new System.Windows.Forms.Button();
            this.txt_folder_deploy = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.combo_netcore_publish_mode = new System.Windows.Forms.ComboBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label_without_vs = new System.Windows.Forms.Label();
            this.checkBox_Chinese = new System.Windows.Forms.CheckBox();
            this.btn_choose_msbuild = new System.Windows.Forms.Button();
            this.label36 = new System.Windows.Forms.Label();
            this.txt_msbuild_path = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tabcontrol.SuspendLayout();
            this.page_web_iis.SuspendLayout();
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
            this.pag_advance_setting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcontrol
            // 
            resources.ApplyResources(this.tabcontrol, "tabcontrol");
            this.tabcontrol.Controls.Add(this.page_web_iis);
            this.tabcontrol.Controls.Add(this.page_docker);
            this.tabcontrol.Controls.Add(this.page_window_service);
            this.tabcontrol.Controls.Add(this.page_set);
            this.tabcontrol.Controls.Add(this.pag_advance_setting);
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.SelectedIndexChanged += new System.EventHandler(this.page__SelectedIndexChanged);
            // 
            // page_web_iis
            // 
            resources.ApplyResources(this.page_web_iis, "page_web_iis");
            this.page_web_iis.Controls.Add(this.btn_iis_stop);
            this.page_web_iis.Controls.Add(this.btn_iis_retry);
            this.page_web_iis.Controls.Add(this.checkBox_select_deploy_iis);
            this.page_web_iis.Controls.Add(this.b_iis_rollback);
            this.page_web_iis.Controls.Add(this.checkBox_Increment_iis);
            this.page_web_iis.Controls.Add(this.label25);
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
            this.page_web_iis.Name = "page_web_iis";
            this.page_web_iis.UseVisualStyleBackColor = true;
            // 
            // btn_iis_stop
            // 
            resources.ApplyResources(this.btn_iis_stop, "btn_iis_stop");
            this.btn_iis_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_iis_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_iis_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_iis_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_iis_stop.ForeColor = System.Drawing.Color.Red;
            this.btn_iis_stop.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_iis_stop.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_iis_stop.Name = "btn_iis_stop";
            this.btn_iis_stop.Radius = 10;
            this.btn_iis_stop.Stroke = false;
            this.btn_iis_stop.StrokeColor = System.Drawing.Color.Gray;
            this.btn_iis_stop.Transparency = false;
            this.btn_iis_stop.Click += new System.EventHandler(this.btn_iis_stop_Click);
            // 
            // btn_iis_retry
            // 
            resources.ApplyResources(this.btn_iis_retry, "btn_iis_retry");
            this.btn_iis_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_iis_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_iis_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_iis_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_iis_retry.ForeColor = System.Drawing.Color.Fuchsia;
            this.btn_iis_retry.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_iis_retry.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_iis_retry.Name = "btn_iis_retry";
            this.btn_iis_retry.Radius = 10;
            this.btn_iis_retry.Stroke = false;
            this.btn_iis_retry.StrokeColor = System.Drawing.Color.Gray;
            this.btn_iis_retry.Transparency = false;
            this.btn_iis_retry.Click += new System.EventHandler(this.btn_iis_retry_Click);
            // 
            // checkBox_select_deploy_iis
            // 
            resources.ApplyResources(this.checkBox_select_deploy_iis, "checkBox_select_deploy_iis");
            this.checkBox_select_deploy_iis.Name = "checkBox_select_deploy_iis";
            this.checkBox_select_deploy_iis.UseVisualStyleBackColor = true;
            this.checkBox_select_deploy_iis.Click += new System.EventHandler(this.checkBox_selectDeplot_iis_CheckedChanged);
            // 
            // b_iis_rollback
            // 
            resources.ApplyResources(this.b_iis_rollback, "b_iis_rollback");
            this.b_iis_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_iis_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_iis_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_iis_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_iis_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_iis_rollback.Name = "b_iis_rollback";
            this.b_iis_rollback.Radius = 10;
            this.b_iis_rollback.Stroke = false;
            this.b_iis_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_iis_rollback.Transparency = false;
            this.b_iis_rollback.Click += new System.EventHandler(this.b_iis_rollback_Click);
            // 
            // checkBox_Increment_iis
            // 
            resources.ApplyResources(this.checkBox_Increment_iis, "checkBox_Increment_iis");
            this.checkBox_Increment_iis.Name = "checkBox_Increment_iis";
            this.checkBox_Increment_iis.UseVisualStyleBackColor = true;
            this.checkBox_Increment_iis.Click += new System.EventHandler(this.checkBox_Increment_iis_CheckedChanged);
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // b_iis_deploy
            // 
            resources.ApplyResources(this.b_iis_deploy, "b_iis_deploy");
            this.b_iis_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_iis_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_iis_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_iis_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_iis_deploy.ForeColor = System.Drawing.Color.Black;
            this.b_iis_deploy.Inactive1 = System.Drawing.SystemColors.Control;
            this.b_iis_deploy.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.b_iis_deploy.Name = "b_iis_deploy";
            this.b_iis_deploy.Radius = 10;
            this.b_iis_deploy.Stroke = false;
            this.b_iis_deploy.StrokeColor = System.Drawing.Color.Gray;
            this.b_iis_deploy.Transparency = false;
            this.b_iis_deploy.Click += new System.EventHandler(this.b_iis_deploy_Click);
            // 
            // combo_iis_env
            // 
            resources.ApplyResources(this.combo_iis_env, "combo_iis_env");
            this.combo_iis_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_env.FormattingEnabled = true;
            this.combo_iis_env.Name = "combo_iis_env";
            this.combo_iis_env.SelectedIndexChanged += new System.EventHandler(this.combo_iis_env_SelectedIndexChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txt_iis_web_site_name
            // 
            resources.ApplyResources(this.txt_iis_web_site_name, "txt_iis_web_site_name");
            this.txt_iis_web_site_name.Name = "txt_iis_web_site_name";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // combo_iis_sdk_type
            // 
            resources.ApplyResources(this.combo_iis_sdk_type, "combo_iis_sdk_type");
            this.combo_iis_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_sdk_type.FormattingEnabled = true;
            this.combo_iis_sdk_type.Items.AddRange(new object[] {
            resources.GetString("combo_iis_sdk_type.Items"),
            resources.GetString("combo_iis_sdk_type.Items1")});
            this.combo_iis_sdk_type.Name = "combo_iis_sdk_type";
            this.combo_iis_sdk_type.SelectedIndexChanged += new System.EventHandler(this.combo_iis_sdk_type_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label_iis_demo
            // 
            resources.ApplyResources(this.label_iis_demo, "label_iis_demo");
            this.label_iis_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_iis_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_iis_demo.Name = "label_iis_demo";
            this.label_iis_demo.Click += new System.EventHandler(this.label_iis_demo_Click);
            // 
            // tab_iis
            // 
            resources.ApplyResources(this.tab_iis, "tab_iis");
            this.tab_iis.Controls.Add(this.tabPage_progress);
            this.tab_iis.Controls.Add(this.tabPage_iis_log);
            this.tab_iis.Name = "tab_iis";
            this.tab_iis.SelectedIndex = 0;
            // 
            // tabPage_progress
            // 
            resources.ApplyResources(this.tabPage_progress, "tabPage_progress");
            this.tabPage_progress.Controls.Add(this.progress_iis_tip);
            this.tabPage_progress.Name = "tabPage_progress";
            this.tabPage_progress.UseVisualStyleBackColor = true;
            // 
            // progress_iis_tip
            // 
            resources.ApplyResources(this.progress_iis_tip, "progress_iis_tip");
            this.progress_iis_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_iis_tip.Name = "progress_iis_tip";
            // 
            // tabPage_iis_log
            // 
            resources.ApplyResources(this.tabPage_iis_log, "tabPage_iis_log");
            this.tabPage_iis_log.Controls.Add(this.rich_iis_log);
            this.tabPage_iis_log.Name = "tabPage_iis_log";
            this.tabPage_iis_log.UseVisualStyleBackColor = true;
            // 
            // rich_iis_log
            // 
            resources.ApplyResources(this.rich_iis_log, "rich_iis_log");
            this.rich_iis_log.HiglightColor = AntDeployWinform.RtfColor.White;
            this.rich_iis_log.Name = "rich_iis_log";
            this.rich_iis_log.ReadOnly = true;
            this.rich_iis_log.TextColor = AntDeployWinform.RtfColor.Black;
            // 
            // page_docker
            // 
            resources.ApplyResources(this.page_docker, "page_docker");
            this.page_docker.Controls.Add(this.checkBox_select_deploy_docker);
            this.page_docker.Controls.Add(this.checkBox_Increment_docker);
            this.page_docker.Controls.Add(this.btn_docker_stop);
            this.page_docker.Controls.Add(this.btn_docker_retry);
            this.page_docker.Controls.Add(this.b_docker_deploy);
            this.page_docker.Controls.Add(this.label13);
            this.page_docker.Controls.Add(this.label12);
            this.page_docker.Controls.Add(this.t_docker_delete_days);
            this.page_docker.Controls.Add(this.label24);
            this.page_docker.Controls.Add(this.groupBox5);
            this.page_docker.Controls.Add(this.combo_docker_env);
            this.page_docker.Controls.Add(this.label22);
            this.page_docker.Controls.Add(this.label_docker_demo);
            this.page_docker.Controls.Add(this.tabControl_docker);
            this.page_docker.Controls.Add(this.b_docker_rollback);
            this.page_docker.Name = "page_docker";
            this.page_docker.UseVisualStyleBackColor = true;
            // 
            // checkBox_select_deploy_docker
            // 
            resources.ApplyResources(this.checkBox_select_deploy_docker, "checkBox_select_deploy_docker");
            this.checkBox_select_deploy_docker.Name = "checkBox_select_deploy_docker";
            this.checkBox_select_deploy_docker.UseVisualStyleBackColor = true;
            this.checkBox_select_deploy_docker.Click += new System.EventHandler(this.checkBox_selectDeplot_docker_CheckedChanged);
            // 
            // checkBox_Increment_docker
            // 
            resources.ApplyResources(this.checkBox_Increment_docker, "checkBox_Increment_docker");
            this.checkBox_Increment_docker.Name = "checkBox_Increment_docker";
            this.checkBox_Increment_docker.UseVisualStyleBackColor = true;
            this.checkBox_Increment_docker.Click += new System.EventHandler(this.checkBox_Increment_docker_CheckedChanged);
            // 
            // btn_docker_stop
            // 
            resources.ApplyResources(this.btn_docker_stop, "btn_docker_stop");
            this.btn_docker_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_docker_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_docker_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_docker_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_docker_stop.ForeColor = System.Drawing.Color.Red;
            this.btn_docker_stop.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_docker_stop.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_docker_stop.Name = "btn_docker_stop";
            this.btn_docker_stop.Radius = 10;
            this.btn_docker_stop.Stroke = false;
            this.btn_docker_stop.StrokeColor = System.Drawing.Color.Gray;
            this.btn_docker_stop.Transparency = false;
            this.btn_docker_stop.Click += new System.EventHandler(this.btn_docker_stop_Click);
            // 
            // btn_docker_retry
            // 
            resources.ApplyResources(this.btn_docker_retry, "btn_docker_retry");
            this.btn_docker_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_docker_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_docker_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_docker_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_docker_retry.ForeColor = System.Drawing.Color.Fuchsia;
            this.btn_docker_retry.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_docker_retry.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_docker_retry.Name = "btn_docker_retry";
            this.btn_docker_retry.Radius = 10;
            this.btn_docker_retry.Stroke = false;
            this.btn_docker_retry.StrokeColor = System.Drawing.Color.Gray;
            this.btn_docker_retry.Transparency = false;
            this.btn_docker_retry.Click += new System.EventHandler(this.btn_docker_retry_Click);
            // 
            // b_docker_deploy
            // 
            resources.ApplyResources(this.b_docker_deploy, "b_docker_deploy");
            this.b_docker_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_docker_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_docker_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_docker_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_docker_deploy.ForeColor = System.Drawing.Color.Black;
            this.b_docker_deploy.Inactive1 = System.Drawing.SystemColors.Control;
            this.b_docker_deploy.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.b_docker_deploy.Name = "b_docker_deploy";
            this.b_docker_deploy.Radius = 10;
            this.b_docker_deploy.Stroke = false;
            this.b_docker_deploy.StrokeColor = System.Drawing.Color.Gray;
            this.b_docker_deploy.Transparency = false;
            this.b_docker_deploy.Click += new System.EventHandler(this.b_docker_deploy_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label12.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label12.Name = "label12";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // t_docker_delete_days
            // 
            resources.ApplyResources(this.t_docker_delete_days, "t_docker_delete_days");
            this.t_docker_delete_days.ForeColor = System.Drawing.Color.Blue;
            this.t_docker_delete_days.Name = "t_docker_delete_days";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.txt_docker_volume);
            this.groupBox5.Controls.Add(this.label27);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.txt_docker_envname);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.txt_docker_port);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // txt_docker_volume
            // 
            resources.ApplyResources(this.txt_docker_volume, "txt_docker_volume");
            this.txt_docker_volume.BackColor = System.Drawing.Color.Transparent;
            this.txt_docker_volume.Br = System.Drawing.Color.White;
            this.txt_docker_volume.ForeColor = System.Drawing.Color.DimGray;
            this.txt_docker_volume.Name = "txt_docker_volume";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // txt_docker_envname
            // 
            resources.ApplyResources(this.txt_docker_envname, "txt_docker_envname");
            this.txt_docker_envname.Name = "txt_docker_envname";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // txt_docker_port
            // 
            resources.ApplyResources(this.txt_docker_port, "txt_docker_port");
            this.txt_docker_port.Name = "txt_docker_port";
            // 
            // combo_docker_env
            // 
            resources.ApplyResources(this.combo_docker_env, "combo_docker_env");
            this.combo_docker_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_docker_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_docker_env.FormattingEnabled = true;
            this.combo_docker_env.Name = "combo_docker_env";
            this.combo_docker_env.SelectedIndexChanged += new System.EventHandler(this.combo_docker_env_SelectedIndexChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label_docker_demo
            // 
            resources.ApplyResources(this.label_docker_demo, "label_docker_demo");
            this.label_docker_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_docker_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_docker_demo.Name = "label_docker_demo";
            this.label_docker_demo.Click += new System.EventHandler(this.label_docker_demo_Click);
            // 
            // tabControl_docker
            // 
            resources.ApplyResources(this.tabControl_docker, "tabControl_docker");
            this.tabControl_docker.Controls.Add(this.tabPage_docker);
            this.tabControl_docker.Controls.Add(this.tabPage_docker_log);
            this.tabControl_docker.Name = "tabControl_docker";
            this.tabControl_docker.SelectedIndex = 0;
            // 
            // tabPage_docker
            // 
            resources.ApplyResources(this.tabPage_docker, "tabPage_docker");
            this.tabPage_docker.Controls.Add(this.progress_docker_tip);
            this.tabPage_docker.Name = "tabPage_docker";
            this.tabPage_docker.UseVisualStyleBackColor = true;
            // 
            // progress_docker_tip
            // 
            resources.ApplyResources(this.progress_docker_tip, "progress_docker_tip");
            this.progress_docker_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_docker_tip.Name = "progress_docker_tip";
            // 
            // tabPage_docker_log
            // 
            resources.ApplyResources(this.tabPage_docker_log, "tabPage_docker_log");
            this.tabPage_docker_log.Controls.Add(this.rich_docker_log);
            this.tabPage_docker_log.Name = "tabPage_docker_log";
            this.tabPage_docker_log.UseVisualStyleBackColor = true;
            // 
            // rich_docker_log
            // 
            resources.ApplyResources(this.rich_docker_log, "rich_docker_log");
            this.rich_docker_log.HiglightColor = AntDeployWinform.RtfColor.White;
            this.rich_docker_log.Name = "rich_docker_log";
            this.rich_docker_log.ReadOnly = true;
            this.rich_docker_log.TextColor = AntDeployWinform.RtfColor.Black;
            // 
            // b_docker_rollback
            // 
            resources.ApplyResources(this.b_docker_rollback, "b_docker_rollback");
            this.b_docker_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_docker_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_docker_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_docker_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_docker_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_docker_rollback.Name = "b_docker_rollback";
            this.b_docker_rollback.Radius = 10;
            this.b_docker_rollback.Stroke = false;
            this.b_docker_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_docker_rollback.Transparency = false;
            this.b_docker_rollback.Click += new System.EventHandler(this.btn_docker_rollback_Click);
            // 
            // page_window_service
            // 
            resources.ApplyResources(this.page_window_service, "page_window_service");
            this.page_window_service.Controls.Add(this.btn_windows_serivce_stop);
            this.page_window_service.Controls.Add(this.btn_windows_service_retry);
            this.page_window_service.Controls.Add(this.b_windowservice_deploy);
            this.page_window_service.Controls.Add(this.checkBox_select_deploy_service);
            this.page_window_service.Controls.Add(this.checkBox_Increment_window_service);
            this.page_window_service.Controls.Add(this.label_windows_serivce_demo);
            this.page_window_service.Controls.Add(this.label26);
            this.page_window_service.Controls.Add(this.txt_windowservice_name);
            this.page_window_service.Controls.Add(this.label14);
            this.page_window_service.Controls.Add(this.combo_windowservice_sdk_type);
            this.page_window_service.Controls.Add(this.label11);
            this.page_window_service.Controls.Add(this.combo_windowservice_env);
            this.page_window_service.Controls.Add(this.label10);
            this.page_window_service.Controls.Add(this.tabControl_window_service);
            this.page_window_service.Controls.Add(this.b_windows_service_rollback);
            this.page_window_service.Name = "page_window_service";
            this.page_window_service.UseVisualStyleBackColor = true;
            // 
            // btn_windows_serivce_stop
            // 
            resources.ApplyResources(this.btn_windows_serivce_stop, "btn_windows_serivce_stop");
            this.btn_windows_serivce_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_windows_serivce_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_windows_serivce_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_windows_serivce_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_windows_serivce_stop.ForeColor = System.Drawing.Color.Red;
            this.btn_windows_serivce_stop.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_windows_serivce_stop.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_windows_serivce_stop.Name = "btn_windows_serivce_stop";
            this.btn_windows_serivce_stop.Radius = 10;
            this.btn_windows_serivce_stop.Stroke = false;
            this.btn_windows_serivce_stop.StrokeColor = System.Drawing.Color.Gray;
            this.btn_windows_serivce_stop.Transparency = false;
            this.btn_windows_serivce_stop.Click += new System.EventHandler(this.btn_windows_serivce_stop_Click);
            // 
            // btn_windows_service_retry
            // 
            resources.ApplyResources(this.btn_windows_service_retry, "btn_windows_service_retry");
            this.btn_windows_service_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_windows_service_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_windows_service_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_windows_service_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_windows_service_retry.ForeColor = System.Drawing.Color.Fuchsia;
            this.btn_windows_service_retry.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_windows_service_retry.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_windows_service_retry.Name = "btn_windows_service_retry";
            this.btn_windows_service_retry.Radius = 10;
            this.btn_windows_service_retry.Stroke = false;
            this.btn_windows_service_retry.StrokeColor = System.Drawing.Color.Gray;
            this.btn_windows_service_retry.Transparency = false;
            this.btn_windows_service_retry.Click += new System.EventHandler(this.btn_windows_service_retry_Click);
            // 
            // b_windowservice_deploy
            // 
            resources.ApplyResources(this.b_windowservice_deploy, "b_windowservice_deploy");
            this.b_windowservice_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_windowservice_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_windowservice_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_windowservice_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_windowservice_deploy.ForeColor = System.Drawing.Color.Black;
            this.b_windowservice_deploy.Inactive1 = System.Drawing.SystemColors.Control;
            this.b_windowservice_deploy.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.b_windowservice_deploy.Name = "b_windowservice_deploy";
            this.b_windowservice_deploy.Radius = 10;
            this.b_windowservice_deploy.Stroke = false;
            this.b_windowservice_deploy.StrokeColor = System.Drawing.Color.Gray;
            this.b_windowservice_deploy.Transparency = false;
            this.b_windowservice_deploy.Click += new System.EventHandler(this.b_windowservice_deploy_Click);
            // 
            // checkBox_select_deploy_service
            // 
            resources.ApplyResources(this.checkBox_select_deploy_service, "checkBox_select_deploy_service");
            this.checkBox_select_deploy_service.Name = "checkBox_select_deploy_service";
            this.checkBox_select_deploy_service.UseVisualStyleBackColor = true;
            this.checkBox_select_deploy_service.Click += new System.EventHandler(this.checkBox_SelectDeploy_window_service_CheckedChanged);
            // 
            // checkBox_Increment_window_service
            // 
            resources.ApplyResources(this.checkBox_Increment_window_service, "checkBox_Increment_window_service");
            this.checkBox_Increment_window_service.Name = "checkBox_Increment_window_service";
            this.checkBox_Increment_window_service.UseVisualStyleBackColor = true;
            this.checkBox_Increment_window_service.Click += new System.EventHandler(this.checkBox_Increment_window_service_CheckedChanged);
            // 
            // label_windows_serivce_demo
            // 
            resources.ApplyResources(this.label_windows_serivce_demo, "label_windows_serivce_demo");
            this.label_windows_serivce_demo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_windows_serivce_demo.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_windows_serivce_demo.Name = "label_windows_serivce_demo";
            this.label_windows_serivce_demo.Click += new System.EventHandler(this.label_windows_serivce_demo_Click);
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // txt_windowservice_name
            // 
            resources.ApplyResources(this.txt_windowservice_name, "txt_windowservice_name");
            this.txt_windowservice_name.Name = "txt_windowservice_name";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // combo_windowservice_sdk_type
            // 
            resources.ApplyResources(this.combo_windowservice_sdk_type, "combo_windowservice_sdk_type");
            this.combo_windowservice_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_sdk_type.FormattingEnabled = true;
            this.combo_windowservice_sdk_type.Items.AddRange(new object[] {
            resources.GetString("combo_windowservice_sdk_type.Items"),
            resources.GetString("combo_windowservice_sdk_type.Items1")});
            this.combo_windowservice_sdk_type.Name = "combo_windowservice_sdk_type";
            this.combo_windowservice_sdk_type.SelectedIndexChanged += new System.EventHandler(this.combo_windowservice_sdk_type_SelectedIndexChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // combo_windowservice_env
            // 
            resources.ApplyResources(this.combo_windowservice_env, "combo_windowservice_env");
            this.combo_windowservice_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_env.FormattingEnabled = true;
            this.combo_windowservice_env.Name = "combo_windowservice_env";
            this.combo_windowservice_env.SelectedIndexChanged += new System.EventHandler(this.combo_windowservice_env_SelectedIndexChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // tabControl_window_service
            // 
            resources.ApplyResources(this.tabControl_window_service, "tabControl_window_service");
            this.tabControl_window_service.Controls.Add(this.tabPage_windows_service);
            this.tabControl_window_service.Controls.Add(this.tabPage2);
            this.tabControl_window_service.Name = "tabControl_window_service";
            this.tabControl_window_service.SelectedIndex = 0;
            // 
            // tabPage_windows_service
            // 
            resources.ApplyResources(this.tabPage_windows_service, "tabPage_windows_service");
            this.tabPage_windows_service.Controls.Add(this.progress_window_service_tip);
            this.tabPage_windows_service.Name = "tabPage_windows_service";
            this.tabPage_windows_service.UseVisualStyleBackColor = true;
            // 
            // progress_window_service_tip
            // 
            resources.ApplyResources(this.progress_window_service_tip, "progress_window_service_tip");
            this.progress_window_service_tip.ForeColor = System.Drawing.Color.Blue;
            this.progress_window_service_tip.Name = "progress_window_service_tip";
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.rich_windowservice_log);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rich_windowservice_log
            // 
            resources.ApplyResources(this.rich_windowservice_log, "rich_windowservice_log");
            this.rich_windowservice_log.HiglightColor = AntDeployWinform.RtfColor.White;
            this.rich_windowservice_log.Name = "rich_windowservice_log";
            this.rich_windowservice_log.ReadOnly = true;
            this.rich_windowservice_log.TextColor = AntDeployWinform.RtfColor.Black;
            // 
            // b_windows_service_rollback
            // 
            resources.ApplyResources(this.b_windows_service_rollback, "b_windows_service_rollback");
            this.b_windows_service_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_windows_service_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.b_windows_service_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_windows_service_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_windows_service_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_windows_service_rollback.Name = "b_windows_service_rollback";
            this.b_windows_service_rollback.Radius = 10;
            this.b_windows_service_rollback.Stroke = false;
            this.b_windows_service_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_windows_service_rollback.Transparency = false;
            this.b_windows_service_rollback.Click += new System.EventHandler(this.b_windows_service_rollback_Click);
            // 
            // page_set
            // 
            resources.ApplyResources(this.page_set, "page_set");
            this.page_set.Controls.Add(this.label_how_to_set);
            this.page_set.Controls.Add(this.groupBox1);
            this.page_set.Controls.Add(this.label_check_update);
            this.page_set.Controls.Add(this.groupBoxIgnore);
            this.page_set.Controls.Add(this.environment);
            this.page_set.Name = "page_set";
            this.page_set.UseVisualStyleBackColor = true;
            // 
            // label_how_to_set
            // 
            resources.ApplyResources(this.label_how_to_set, "label_how_to_set");
            this.label_how_to_set.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_how_to_set.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_how_to_set.Name = "label_how_to_set";
            this.label_how_to_set.Click += new System.EventHandler(this.label_how_to_set_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.b_copy_backup_ignore);
            this.groupBox1.Controls.Add(this.b_backUp_ignore_remove);
            this.groupBox1.Controls.Add(this.b_backUp_ignore_add);
            this.groupBox1.Controls.Add(this.txt_backUp_ignore);
            this.groupBox1.Controls.Add(this.list_backUp_ignore);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // b_copy_backup_ignore
            // 
            resources.ApplyResources(this.b_copy_backup_ignore, "b_copy_backup_ignore");
            this.b_copy_backup_ignore.ForeColor = System.Drawing.Color.Blue;
            this.b_copy_backup_ignore.Name = "b_copy_backup_ignore";
            this.b_copy_backup_ignore.UseVisualStyleBackColor = true;
            this.b_copy_backup_ignore.Click += new System.EventHandler(this.b_copy_backup_ignore_Click);
            // 
            // b_backUp_ignore_remove
            // 
            resources.ApplyResources(this.b_backUp_ignore_remove, "b_backUp_ignore_remove");
            this.b_backUp_ignore_remove.ForeColor = System.Drawing.Color.Red;
            this.b_backUp_ignore_remove.Name = "b_backUp_ignore_remove";
            this.b_backUp_ignore_remove.UseVisualStyleBackColor = true;
            this.b_backUp_ignore_remove.Click += new System.EventHandler(this.b_backUp_ignore_remove_Click);
            // 
            // b_backUp_ignore_add
            // 
            resources.ApplyResources(this.b_backUp_ignore_add, "b_backUp_ignore_add");
            this.b_backUp_ignore_add.Name = "b_backUp_ignore_add";
            this.b_backUp_ignore_add.UseVisualStyleBackColor = true;
            this.b_backUp_ignore_add.Click += new System.EventHandler(this.b_backUp_ignore_add_Click);
            // 
            // txt_backUp_ignore
            // 
            resources.ApplyResources(this.txt_backUp_ignore, "txt_backUp_ignore");
            this.txt_backUp_ignore.Name = "txt_backUp_ignore";
            // 
            // list_backUp_ignore
            // 
            resources.ApplyResources(this.list_backUp_ignore, "list_backUp_ignore");
            this.list_backUp_ignore.FormattingEnabled = true;
            this.list_backUp_ignore.Name = "list_backUp_ignore";
            // 
            // label_check_update
            // 
            resources.ApplyResources(this.label_check_update, "label_check_update");
            this.label_check_update.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_check_update.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_check_update.Name = "label_check_update";
            this.label_check_update.Click += new System.EventHandler(this.label_check_update_Click);
            // 
            // groupBoxIgnore
            // 
            resources.ApplyResources(this.groupBoxIgnore, "groupBoxIgnore");
            this.groupBoxIgnore.Controls.Add(this.b_copy_pack_ignore);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_remove);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_add);
            this.groupBoxIgnore.Controls.Add(this.txt_env_ignore);
            this.groupBoxIgnore.Controls.Add(this.list_env_ignore);
            this.groupBoxIgnore.Name = "groupBoxIgnore";
            this.groupBoxIgnore.TabStop = false;
            // 
            // b_copy_pack_ignore
            // 
            resources.ApplyResources(this.b_copy_pack_ignore, "b_copy_pack_ignore");
            this.b_copy_pack_ignore.ForeColor = System.Drawing.Color.Blue;
            this.b_copy_pack_ignore.Name = "b_copy_pack_ignore";
            this.b_copy_pack_ignore.UseVisualStyleBackColor = true;
            this.b_copy_pack_ignore.Click += new System.EventHandler(this.b_copy_pack_ignore_Click);
            // 
            // b_env_ignore_remove
            // 
            resources.ApplyResources(this.b_env_ignore_remove, "b_env_ignore_remove");
            this.b_env_ignore_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_ignore_remove.Name = "b_env_ignore_remove";
            this.b_env_ignore_remove.UseVisualStyleBackColor = true;
            this.b_env_ignore_remove.Click += new System.EventHandler(this.b_env_ignore_remove_Click);
            // 
            // b_env_ignore_add
            // 
            resources.ApplyResources(this.b_env_ignore_add, "b_env_ignore_add");
            this.b_env_ignore_add.Name = "b_env_ignore_add";
            this.b_env_ignore_add.UseVisualStyleBackColor = true;
            this.b_env_ignore_add.Click += new System.EventHandler(this.b_env_ignore_add_Click);
            // 
            // txt_env_ignore
            // 
            resources.ApplyResources(this.txt_env_ignore, "txt_env_ignore");
            this.txt_env_ignore.Name = "txt_env_ignore";
            // 
            // list_env_ignore
            // 
            resources.ApplyResources(this.list_env_ignore, "list_env_ignore");
            this.list_env_ignore.FormattingEnabled = true;
            this.list_env_ignore.Name = "list_env_ignore";
            // 
            // environment
            // 
            resources.ApplyResources(this.environment, "environment");
            this.environment.Controls.Add(this.tabControl1);
            this.environment.Controls.Add(this.label2);
            this.environment.Controls.Add(this.label1);
            this.environment.Controls.Add(this.b_env_remove);
            this.environment.Controls.Add(this.txt_env_name);
            this.environment.Controls.Add(this.b_env_add_by_name);
            this.environment.Controls.Add(this.combo_env_list);
            this.environment.Name = "environment";
            this.environment.TabStop = false;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.page_winserver);
            this.tabControl1.Controls.Add(this.page_linux_server);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // page_winserver
            // 
            resources.ApplyResources(this.page_winserver, "page_winserver");
            this.page_winserver.Controls.Add(this.txt_winserver_nickname);
            this.page_winserver.Controls.Add(this.label32);
            this.page_winserver.Controls.Add(this.label29);
            this.page_winserver.Controls.Add(this.loading_win_server_test);
            this.page_winserver.Controls.Add(this.label5);
            this.page_winserver.Controls.Add(this.b_env_server_test);
            this.page_winserver.Controls.Add(this.combo_env_server_list);
            this.page_winserver.Controls.Add(this.b_env_server_add);
            this.page_winserver.Controls.Add(this.txt_env_server_host);
            this.page_winserver.Controls.Add(this.b_env_server_remove);
            this.page_winserver.Controls.Add(this.label3);
            this.page_winserver.Controls.Add(this.txt_env_server_token);
            this.page_winserver.Controls.Add(this.label4);
            this.page_winserver.Name = "page_winserver";
            this.page_winserver.UseVisualStyleBackColor = true;
            // 
            // txt_winserver_nickname
            // 
            resources.ApplyResources(this.txt_winserver_nickname, "txt_winserver_nickname");
            this.txt_winserver_nickname.Name = "txt_winserver_nickname";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label32.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label32.Name = "label32";
            this.label32.Click += new System.EventHandler(this.label32_Click);
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // loading_win_server_test
            // 
            resources.ApplyResources(this.loading_win_server_test, "loading_win_server_test");
            this.loading_win_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_win_server_test.FullTransparent = true;
            this.loading_win_server_test.Increment = 1F;
            this.loading_win_server_test.N = 8;
            this.loading_win_server_test.Name = "loading_win_server_test";
            this.loading_win_server_test.Radius = 2.5F;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // b_env_server_test
            // 
            resources.ApplyResources(this.b_env_server_test, "b_env_server_test");
            this.b_env_server_test.Name = "b_env_server_test";
            this.b_env_server_test.UseVisualStyleBackColor = true;
            this.b_env_server_test.Click += new System.EventHandler(this.b_env_server_test_Click);
            // 
            // combo_env_server_list
            // 
            resources.ApplyResources(this.combo_env_server_list, "combo_env_server_list");
            this.combo_env_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_server_list.FormattingEnabled = true;
            this.combo_env_server_list.Name = "combo_env_server_list";
            this.combo_env_server_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_server_list_SelectedIndexChanged);
            // 
            // b_env_server_add
            // 
            resources.ApplyResources(this.b_env_server_add, "b_env_server_add");
            this.b_env_server_add.Name = "b_env_server_add";
            this.b_env_server_add.UseVisualStyleBackColor = true;
            this.b_env_server_add.Click += new System.EventHandler(this.b_env_server_add_Click);
            // 
            // txt_env_server_host
            // 
            resources.ApplyResources(this.txt_env_server_host, "txt_env_server_host");
            this.txt_env_server_host.Name = "txt_env_server_host";
            // 
            // b_env_server_remove
            // 
            resources.ApplyResources(this.b_env_server_remove, "b_env_server_remove");
            this.b_env_server_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_server_remove.Name = "b_env_server_remove";
            this.b_env_server_remove.UseVisualStyleBackColor = true;
            this.b_env_server_remove.Click += new System.EventHandler(this.b_env_server_remove_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txt_env_server_token
            // 
            resources.ApplyResources(this.txt_env_server_token, "txt_env_server_token");
            this.txt_env_server_token.Name = "txt_env_server_token";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // page_linux_server
            // 
            resources.ApplyResources(this.page_linux_server, "page_linux_server");
            this.page_linux_server.Controls.Add(this.txt_linux_server_nickname);
            this.page_linux_server.Controls.Add(this.loading_linux_server_test);
            this.page_linux_server.Controls.Add(this.label20);
            this.page_linux_server.Controls.Add(this.combo_linux_server_list);
            this.page_linux_server.Controls.Add(this.txt_linux_pwd);
            this.page_linux_server.Controls.Add(this.txt_linux_username);
            this.page_linux_server.Controls.Add(this.b_linux_server_test);
            this.page_linux_server.Controls.Add(this.b_add_linux_server);
            this.page_linux_server.Controls.Add(this.b_linux_server_remove);
            this.page_linux_server.Controls.Add(this.txt_linux_host);
            this.page_linux_server.Controls.Add(this.label17);
            this.page_linux_server.Controls.Add(this.label18);
            this.page_linux_server.Controls.Add(this.label19);
            this.page_linux_server.Controls.Add(this.label35);
            this.page_linux_server.Name = "page_linux_server";
            this.page_linux_server.UseVisualStyleBackColor = true;
            // 
            // txt_linux_server_nickname
            // 
            resources.ApplyResources(this.txt_linux_server_nickname, "txt_linux_server_nickname");
            this.txt_linux_server_nickname.Name = "txt_linux_server_nickname";
            // 
            // loading_linux_server_test
            // 
            resources.ApplyResources(this.loading_linux_server_test, "loading_linux_server_test");
            this.loading_linux_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_linux_server_test.FullTransparent = true;
            this.loading_linux_server_test.Increment = 1F;
            this.loading_linux_server_test.N = 8;
            this.loading_linux_server_test.Name = "loading_linux_server_test";
            this.loading_linux_server_test.Radius = 2.5F;
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // combo_linux_server_list
            // 
            resources.ApplyResources(this.combo_linux_server_list, "combo_linux_server_list");
            this.combo_linux_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_linux_server_list.FormattingEnabled = true;
            this.combo_linux_server_list.Name = "combo_linux_server_list";
            this.combo_linux_server_list.SelectedIndexChanged += new System.EventHandler(this.combo_linux_server_list_SelectedIndexChanged);
            // 
            // txt_linux_pwd
            // 
            resources.ApplyResources(this.txt_linux_pwd, "txt_linux_pwd");
            this.txt_linux_pwd.Name = "txt_linux_pwd";
            // 
            // txt_linux_username
            // 
            resources.ApplyResources(this.txt_linux_username, "txt_linux_username");
            this.txt_linux_username.Name = "txt_linux_username";
            // 
            // b_linux_server_test
            // 
            resources.ApplyResources(this.b_linux_server_test, "b_linux_server_test");
            this.b_linux_server_test.Name = "b_linux_server_test";
            this.b_linux_server_test.UseVisualStyleBackColor = true;
            this.b_linux_server_test.Click += new System.EventHandler(this.b_linux_server_test_Click);
            // 
            // b_add_linux_server
            // 
            resources.ApplyResources(this.b_add_linux_server, "b_add_linux_server");
            this.b_add_linux_server.Name = "b_add_linux_server";
            this.b_add_linux_server.UseVisualStyleBackColor = true;
            this.b_add_linux_server.Click += new System.EventHandler(this.b_add_linux_server_Click);
            // 
            // b_linux_server_remove
            // 
            resources.ApplyResources(this.b_linux_server_remove, "b_linux_server_remove");
            this.b_linux_server_remove.ForeColor = System.Drawing.Color.Red;
            this.b_linux_server_remove.Name = "b_linux_server_remove";
            this.b_linux_server_remove.UseVisualStyleBackColor = true;
            this.b_linux_server_remove.Click += new System.EventHandler(this.b_linux_server_remove_Click);
            // 
            // txt_linux_host
            // 
            resources.ApplyResources(this.txt_linux_host, "txt_linux_host");
            this.txt_linux_host.Name = "txt_linux_host";
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // label35
            // 
            resources.ApplyResources(this.label35, "label35");
            this.label35.Name = "label35";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // b_env_remove
            // 
            resources.ApplyResources(this.b_env_remove, "b_env_remove");
            this.b_env_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_remove.Name = "b_env_remove";
            this.b_env_remove.UseVisualStyleBackColor = true;
            this.b_env_remove.Click += new System.EventHandler(this.b_env_remove_Click);
            // 
            // txt_env_name
            // 
            resources.ApplyResources(this.txt_env_name, "txt_env_name");
            this.txt_env_name.Name = "txt_env_name";
            // 
            // b_env_add_by_name
            // 
            resources.ApplyResources(this.b_env_add_by_name, "b_env_add_by_name");
            this.b_env_add_by_name.Name = "b_env_add_by_name";
            this.b_env_add_by_name.UseVisualStyleBackColor = true;
            this.b_env_add_by_name.Click += new System.EventHandler(this.b_env_add_by_name_Click);
            // 
            // combo_env_list
            // 
            resources.ApplyResources(this.combo_env_list, "combo_env_list");
            this.combo_env_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_list.FormattingEnabled = true;
            this.combo_env_list.Name = "combo_env_list";
            this.combo_env_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_list_SelectedIndexChanged);
            // 
            // pag_advance_setting
            // 
            resources.ApplyResources(this.pag_advance_setting, "pag_advance_setting");
            this.pag_advance_setting.Controls.Add(this.checkBox_multi_deploy);
            this.pag_advance_setting.Controls.Add(this.checkBox_save_deploy_log);
            this.pag_advance_setting.Controls.Add(this.label33);
            this.pag_advance_setting.Controls.Add(this.txt_http_proxy);
            this.pag_advance_setting.Controls.Add(this.label30);
            this.pag_advance_setting.Controls.Add(this.btn_shang);
            this.pag_advance_setting.Controls.Add(this.label16);
            this.pag_advance_setting.Controls.Add(this.btn_folder_clear);
            this.pag_advance_setting.Controls.Add(this.btn_choose_folder);
            this.pag_advance_setting.Controls.Add(this.txt_folder_deploy);
            this.pag_advance_setting.Controls.Add(this.label42);
            this.pag_advance_setting.Controls.Add(this.label41);
            this.pag_advance_setting.Controls.Add(this.label39);
            this.pag_advance_setting.Controls.Add(this.label38);
            this.pag_advance_setting.Controls.Add(this.combo_netcore_publish_mode);
            this.pag_advance_setting.Controls.Add(this.label37);
            this.pag_advance_setting.Controls.Add(this.label_without_vs);
            this.pag_advance_setting.Controls.Add(this.checkBox_Chinese);
            this.pag_advance_setting.Controls.Add(this.btn_choose_msbuild);
            this.pag_advance_setting.Controls.Add(this.label36);
            this.pag_advance_setting.Controls.Add(this.txt_msbuild_path);
            this.pag_advance_setting.Controls.Add(this.label15);
            this.pag_advance_setting.Name = "pag_advance_setting";
            this.pag_advance_setting.UseVisualStyleBackColor = true;
            // 
            // checkBox_multi_deploy
            // 
            resources.ApplyResources(this.checkBox_multi_deploy, "checkBox_multi_deploy");
            this.checkBox_multi_deploy.Name = "checkBox_multi_deploy";
            this.checkBox_multi_deploy.UseVisualStyleBackColor = true;
            this.checkBox_multi_deploy.Click += new System.EventHandler(this.checkBox_multi_deploy_Click);
            // 
            // checkBox_save_deploy_log
            // 
            resources.ApplyResources(this.checkBox_save_deploy_log, "checkBox_save_deploy_log");
            this.checkBox_save_deploy_log.Name = "checkBox_save_deploy_log";
            this.checkBox_save_deploy_log.UseVisualStyleBackColor = true;
            this.checkBox_save_deploy_log.Click += new System.EventHandler(this.checkBox_save_deploy_log_Click);
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label33.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label33.Name = "label33";
            // 
            // txt_http_proxy
            // 
            resources.ApplyResources(this.txt_http_proxy, "txt_http_proxy");
            this.txt_http_proxy.Name = "txt_http_proxy";
            this.txt_http_proxy.TextChanged += new System.EventHandler(this.txt_http_proxy_TextChanged);
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // btn_shang
            // 
            resources.ApplyResources(this.btn_shang, "btn_shang");
            this.btn_shang.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_shang.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_shang.BackColor = System.Drawing.Color.Transparent;
            this.btn_shang.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_shang.ForeColor = System.Drawing.Color.Black;
            this.btn_shang.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.btn_shang.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(167)))), ((int)(((byte)(188)))));
            this.btn_shang.Name = "btn_shang";
            this.btn_shang.Radius = 10;
            this.btn_shang.Stroke = false;
            this.btn_shang.StrokeColor = System.Drawing.Color.Gray;
            this.btn_shang.Transparency = false;
            this.btn_shang.Click += new System.EventHandler(this.btn_shang_Click);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label16.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label16.Name = "label16";
            // 
            // btn_folder_clear
            // 
            resources.ApplyResources(this.btn_folder_clear, "btn_folder_clear");
            this.btn_folder_clear.ForeColor = System.Drawing.Color.Red;
            this.btn_folder_clear.Name = "btn_folder_clear";
            this.btn_folder_clear.UseVisualStyleBackColor = true;
            this.btn_folder_clear.Click += new System.EventHandler(this.btn_folder_clear_Click);
            // 
            // btn_choose_folder
            // 
            resources.ApplyResources(this.btn_choose_folder, "btn_choose_folder");
            this.btn_choose_folder.Name = "btn_choose_folder";
            this.btn_choose_folder.UseVisualStyleBackColor = true;
            this.btn_choose_folder.Click += new System.EventHandler(this.btn_choose_folder_Click);
            // 
            // txt_folder_deploy
            // 
            resources.ApplyResources(this.txt_folder_deploy, "txt_folder_deploy");
            this.txt_folder_deploy.Name = "txt_folder_deploy";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // label41
            // 
            resources.ApplyResources(this.label41, "label41");
            this.label41.Name = "label41";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label39.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label39.Name = "label39";
            this.label39.Click += new System.EventHandler(this.label39_Click);
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // combo_netcore_publish_mode
            // 
            resources.ApplyResources(this.combo_netcore_publish_mode, "combo_netcore_publish_mode");
            this.combo_netcore_publish_mode.BackColor = System.Drawing.SystemColors.Window;
            this.combo_netcore_publish_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_netcore_publish_mode.FormattingEnabled = true;
            this.combo_netcore_publish_mode.Items.AddRange(new object[] {
            resources.GetString("combo_netcore_publish_mode.Items"),
            resources.GetString("combo_netcore_publish_mode.Items1"),
            resources.GetString("combo_netcore_publish_mode.Items2"),
            resources.GetString("combo_netcore_publish_mode.Items3"),
            resources.GetString("combo_netcore_publish_mode.Items4")});
            this.combo_netcore_publish_mode.Name = "combo_netcore_publish_mode";
            this.combo_netcore_publish_mode.SelectedIndexChanged += new System.EventHandler(this.combo_netcore_publish_mode_SelectedIndexChanged);
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // label_without_vs
            // 
            resources.ApplyResources(this.label_without_vs, "label_without_vs");
            this.label_without_vs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_without_vs.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_without_vs.Name = "label_without_vs";
            this.label_without_vs.Click += new System.EventHandler(this.label_without_vs_Click);
            // 
            // checkBox_Chinese
            // 
            resources.ApplyResources(this.checkBox_Chinese, "checkBox_Chinese");
            this.checkBox_Chinese.Name = "checkBox_Chinese";
            this.checkBox_Chinese.UseVisualStyleBackColor = true;
            this.checkBox_Chinese.Click += new System.EventHandler(this.checkBox_Chinese_Click);
            // 
            // btn_choose_msbuild
            // 
            resources.ApplyResources(this.btn_choose_msbuild, "btn_choose_msbuild");
            this.btn_choose_msbuild.Name = "btn_choose_msbuild";
            this.btn_choose_msbuild.UseVisualStyleBackColor = true;
            this.btn_choose_msbuild.Click += new System.EventHandler(this.btn_choose_msbuild_Click);
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // txt_msbuild_path
            // 
            resources.ApplyResources(this.txt_msbuild_path, "txt_msbuild_path");
            this.txt_msbuild_path.Name = "txt_msbuild_path";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // Deploy
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Deploy";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Deploy_HelpButtonClicked);
            this.Activated += new System.EventHandler(this.Deploy_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Deploy_FormClosing);
            this.Load += new System.EventHandler(this.Deploy_Load);
            this.tabcontrol.ResumeLayout(false);
            this.page_web_iis.ResumeLayout(false);
            this.page_web_iis.PerformLayout();
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
            this.pag_advance_setting.ResumeLayout(false);
            this.pag_advance_setting.PerformLayout();
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
        private AltoControls.AltoButton b_iis_deploy;
        private System.Windows.Forms.GroupBox groupBoxIgnore;
        private System.Windows.Forms.Button b_env_ignore_remove;
        private System.Windows.Forms.Button b_env_ignore_add;
        private System.Windows.Forms.TextBox txt_env_ignore;
        private System.Windows.Forms.ListBox list_env_ignore;
        private System.Windows.Forms.Label label9;
        private AltoControls.AltoButton b_windowservice_deploy;
        private System.Windows.Forms.ComboBox combo_windowservice_env;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox combo_windowservice_sdk_type;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_windowservice_name;
        private System.Windows.Forms.Label label14;
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
        private AltoControls.AltoButton b_docker_deploy;
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
        private System.Windows.Forms.Button b_copy_backup_ignore;
        private System.Windows.Forms.Button b_copy_pack_ignore;
        private System.Windows.Forms.Label label_how_to_set;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox txt_winserver_nickname;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox txt_linux_server_nickname;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TabPage pag_advance_setting;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox txt_msbuild_path;
        private System.Windows.Forms.Button btn_choose_msbuild;
        private System.Windows.Forms.CheckBox checkBox_Chinese;
        private System.Windows.Forms.Label label_without_vs;
        private System.Windows.Forms.ComboBox combo_netcore_publish_mode;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.CheckBox checkBox_select_deploy_iis;
        private System.Windows.Forms.CheckBox checkBox_select_deploy_service;
        private AltoControls.AltoButton btn_iis_stop;
        private AltoControls.AltoButton btn_iis_retry;
        private AltoControls.AltoButton btn_windows_service_retry;
        private AltoControls.AltoButton btn_windows_serivce_stop;
        private AltoControls.AltoButton btn_docker_retry;
        private AltoControls.AltoButton btn_docker_stop;
        private System.Windows.Forms.Button btn_choose_folder;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_folder_deploy;
        private System.Windows.Forms.Button btn_folder_clear;
        private System.Windows.Forms.Label label16;
        private AltoControls.AltoButton btn_shang;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.TextBox txt_http_proxy;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox checkBox_Increment_docker;
        private System.Windows.Forms.CheckBox checkBox_select_deploy_docker;
        private System.Windows.Forms.CheckBox checkBox_save_deploy_log;
        private System.Windows.Forms.CheckBox checkBox_multi_deploy;
    }
}