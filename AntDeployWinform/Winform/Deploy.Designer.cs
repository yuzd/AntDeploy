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
            CCWin.CmSysButton cmSysButton1 = new CCWin.CmSysButton();
            CCWin.CmSysButton cmSysButton2 = new CCWin.CmSysButton();
            this.tabcontrol = new System.Windows.Forms.TabControl();
            this.page_web_iis = new System.Windows.Forms.TabPage();
            this.checkBox_iis_use_offlinehtm = new System.Windows.Forms.CheckBox();
            this.checkBox_iis_restart_site = new System.Windows.Forms.CheckBox();
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
            this.b_docker_deploy = new AltoControls.AltoButton();
            this.btn_docker_retry = new AltoControls.AltoButton();
            this.btn_docker_stop = new AltoControls.AltoButton();
            this.checkBox_select_deploy_docker = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label53 = new System.Windows.Forms.Label();
            this.txt_docker_volume = new AltoControls.AltoTextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.txt_docker_other = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txt_docker_port = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.b_docker_rollback = new AltoControls.AltoButton();
            this.txt_docker_envname = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.combo_docker_env = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label_docker_demo = new System.Windows.Forms.Label();
            this.tabControl_docker = new System.Windows.Forms.TabControl();
            this.tabPage_docker = new System.Windows.Forms.TabPage();
            this.progress_docker_tip = new System.Windows.Forms.Label();
            this.tabPage_docker_log = new System.Windows.Forms.TabPage();
            this.rich_docker_log = new AntDeployWinform.ExRichTextBox();
            this.tabPage_docker_repo = new System.Windows.Forms.TabPage();
            this.label56 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label54 = new System.Windows.Forms.Label();
            this.checkBoxdocker_rep_uploadOnly = new System.Windows.Forms.CheckBox();
            this.checkBoxdocker_rep_enable = new System.Windows.Forms.CheckBox();
            this.txt_docker_rep_image = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.txt_docker_rep_namespace = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txt_docker_rep_domain = new System.Windows.Forms.TextBox();
            this.txt_docker_rep_pwd = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txt_docker_rep_name = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.tabPage_ssh = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label68 = new System.Windows.Forms.Label();
            this.t_docker_delete_days = new System.Windows.Forms.TextBox();
            this.txt_docker_workspace = new System.Windows.Forms.TextBox();
            this.chk_use_AsiaShanghai_timezone = new System.Windows.Forms.CheckBox();
            this.checkBox_Increment_docker = new System.Windows.Forms.CheckBox();
            this.checkBox_sudo_docker = new System.Windows.Forms.CheckBox();
            this.page_docker_img = new System.Windows.Forms.TabPage();
            this.btn_docker_image_stop = new AltoControls.AltoButton();
            this.altoButton1 = new AltoControls.AltoButton();
            this.tabControl_dockerimage = new System.Windows.Forms.TabControl();
            this.tabPage_docker_image = new System.Windows.Forms.TabPage();
            this.rich_docker_image_log = new AntDeployWinform.ExRichTextBox();
            this.tabPage_docker_image_ignore = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label67 = new System.Windows.Forms.Label();
            this.btn_dockerImage_ignore_remove = new System.Windows.Forms.Button();
            this.btn_dockerImage_ignore_add = new System.Windows.Forms.Button();
            this.txt_dockerImage_ignore = new System.Windows.Forms.TextBox();
            this.list_dockerImage_ignore = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_how_to_dockerimage = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.txt_HttpProxy = new System.Windows.Forms.TextBox();
            this.txt_BaseImage_pwd = new System.Windows.Forms.TextBox();
            this.txt_BaseImage = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.txt_BaseImage_username = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label69 = new System.Windows.Forms.Label();
            this.txt_TargetHttpProxy = new System.Windows.Forms.TextBox();
            this.txt_Cmd = new System.Windows.Forms.TextBox();
            this.label65 = new System.Windows.Forms.Label();
            this.txt_Entrypoint = new System.Windows.Forms.TextBox();
            this.cmbo_ImageFormat = new System.Windows.Forms.ComboBox();
            this.txt_TargetImage_pwd = new System.Windows.Forms.TextBox();
            this.txt_TargetImage_username = new System.Windows.Forms.TextBox();
            this.txt_TargetImage = new System.Windows.Forms.TextBox();
            this.label64 = new System.Windows.Forms.Label();
            this.txt_TargetImage_tag = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
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
            this.page_linux_service = new System.Windows.Forms.TabPage();
            this.label_how_to_linuxservice = new System.Windows.Forms.Label();
            this.checkBox_select_type_linuxservice = new System.Windows.Forms.CheckBox();
            this.label51 = new System.Windows.Forms.Label();
            this.txt_linux_service_env = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.tabControl_linux_service = new System.Windows.Forms.TabControl();
            this.tabPage_linux_service = new System.Windows.Forms.TabPage();
            this.progress_linux_service_tip = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.rich_linuxservice_log = new AntDeployWinform.ExRichTextBox();
            this.checkBox_select_deploy_linuxservice = new System.Windows.Forms.CheckBox();
            this.checkBox_Increment_linux_service = new System.Windows.Forms.CheckBox();
            this.label47 = new System.Windows.Forms.Label();
            this.txt_linuxservice_name = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.combo_linux_env = new System.Windows.Forms.ComboBox();
            this.label50 = new System.Windows.Forms.Label();
            this.btn_linux_serivce_stop = new AltoControls.AltoButton();
            this.b_linuxservice_deploy = new AltoControls.AltoButton();
            this.btn_linux_service_retry = new AltoControls.AltoButton();
            this.b_linux_service_rollback = new AltoControls.AltoButton();
            this.page_set = new System.Windows.Forms.TabPage();
            this.panel_rich_config_log = new System.Windows.Forms.Panel();
            this.btn_rich_config_log_close = new AltoControls.AltoButton();
            this.rich_config_log = new AntDeployWinform.ExRichTextBox();
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
            this.btn_win_server_testAll = new AltoControls.AltoButton();
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
            this.btn_linux_server_testAll = new AltoControls.AltoButton();
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
            this.chk_global_saveconfig_in_projectFolder = new System.Windows.Forms.CheckBox();
            this.btn_auto_find_msbuild = new System.Windows.Forms.Button();
            this.label52 = new System.Windows.Forms.Label();
            this.checkBox_multi_deploy = new System.Windows.Forms.CheckBox();
            this.checkBox_save_deploy_log = new System.Windows.Forms.CheckBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txt_http_proxy = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
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
            this.btn_shang = new AltoControls.AltoButton();
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
            this.tabPage_docker_repo.SuspendLayout();
            this.tabPage_ssh.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.page_docker_img.SuspendLayout();
            this.tabControl_dockerimage.SuspendLayout();
            this.tabPage_docker_image.SuspendLayout();
            this.tabPage_docker_image_ignore.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.page_window_service.SuspendLayout();
            this.tabControl_window_service.SuspendLayout();
            this.tabPage_windows_service.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.page_linux_service.SuspendLayout();
            this.tabControl_linux_service.SuspendLayout();
            this.tabPage_linux_service.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.page_set.SuspendLayout();
            this.panel_rich_config_log.SuspendLayout();
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
            this.tabcontrol.Controls.Add(this.page_web_iis);
            this.tabcontrol.Controls.Add(this.page_docker);
            this.tabcontrol.Controls.Add(this.page_docker_img);
            this.tabcontrol.Controls.Add(this.page_window_service);
            this.tabcontrol.Controls.Add(this.page_linux_service);
            this.tabcontrol.Controls.Add(this.page_set);
            this.tabcontrol.Controls.Add(this.pag_advance_setting);
            resources.ApplyResources(this.tabcontrol, "tabcontrol");
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.SelectedIndexChanged += new System.EventHandler(this.page__SelectedIndexChanged);
            // 
            // page_web_iis
            // 
            this.page_web_iis.Controls.Add(this.checkBox_iis_use_offlinehtm);
            this.page_web_iis.Controls.Add(this.checkBox_iis_restart_site);
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
            resources.ApplyResources(this.page_web_iis, "page_web_iis");
            this.page_web_iis.Name = "page_web_iis";
            this.page_web_iis.UseVisualStyleBackColor = true;
            // 
            // checkBox_iis_use_offlinehtm
            // 
            resources.ApplyResources(this.checkBox_iis_use_offlinehtm, "checkBox_iis_use_offlinehtm");
            this.checkBox_iis_use_offlinehtm.Name = "checkBox_iis_use_offlinehtm";
            this.checkBox_iis_use_offlinehtm.UseVisualStyleBackColor = true;
            this.checkBox_iis_use_offlinehtm.Click += new System.EventHandler(this.checkBox_iis_use_offlinehtm_Click);
            // 
            // checkBox_iis_restart_site
            // 
            resources.ApplyResources(this.checkBox_iis_restart_site, "checkBox_iis_restart_site");
            this.checkBox_iis_restart_site.Name = "checkBox_iis_restart_site";
            this.checkBox_iis_restart_site.UseVisualStyleBackColor = true;
            this.checkBox_iis_restart_site.Click += new System.EventHandler(this.checkBox_iis_restart_site_Click);
            // 
            // btn_iis_stop
            // 
            this.btn_iis_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_iis_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_iis_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_iis_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_iis_stop, "btn_iis_stop");
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
            this.btn_iis_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_iis_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_iis_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_iis_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_iis_retry, "btn_iis_retry");
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
            this.b_iis_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_iis_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_iis_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_iis_rollback, "b_iis_rollback");
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
            this.b_iis_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_iis_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_iis_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_iis_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_iis_deploy, "b_iis_deploy");
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
            this.combo_iis_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_env.FormattingEnabled = true;
            resources.ApplyResources(this.combo_iis_env, "combo_iis_env");
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
            this.combo_iis_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_iis_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_iis_sdk_type.FormattingEnabled = true;
            this.combo_iis_sdk_type.Items.AddRange(new object[] {
            resources.GetString("combo_iis_sdk_type.Items"),
            resources.GetString("combo_iis_sdk_type.Items1")});
            resources.ApplyResources(this.combo_iis_sdk_type, "combo_iis_sdk_type");
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
            this.tab_iis.Controls.Add(this.tabPage_progress);
            this.tab_iis.Controls.Add(this.tabPage_iis_log);
            resources.ApplyResources(this.tab_iis, "tab_iis");
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
            this.progress_iis_tip.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.progress_iis_tip, "progress_iis_tip");
            this.progress_iis_tip.Name = "progress_iis_tip";
            // 
            // tabPage_iis_log
            // 
            this.tabPage_iis_log.Controls.Add(this.rich_iis_log);
            resources.ApplyResources(this.tabPage_iis_log, "tabPage_iis_log");
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
            this.page_docker.Controls.Add(this.b_docker_deploy);
            this.page_docker.Controls.Add(this.btn_docker_retry);
            this.page_docker.Controls.Add(this.btn_docker_stop);
            this.page_docker.Controls.Add(this.checkBox_select_deploy_docker);
            this.page_docker.Controls.Add(this.groupBox5);
            this.page_docker.Controls.Add(this.combo_docker_env);
            this.page_docker.Controls.Add(this.label22);
            this.page_docker.Controls.Add(this.label_docker_demo);
            this.page_docker.Controls.Add(this.tabControl_docker);
            this.page_docker.Controls.Add(this.checkBox_Increment_docker);
            this.page_docker.Controls.Add(this.checkBox_sudo_docker);
            resources.ApplyResources(this.page_docker, "page_docker");
            this.page_docker.Name = "page_docker";
            this.page_docker.UseVisualStyleBackColor = true;
            // 
            // b_docker_deploy
            // 
            this.b_docker_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_docker_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_docker_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_docker_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_docker_deploy, "b_docker_deploy");
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
            // btn_docker_retry
            // 
            this.btn_docker_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_docker_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_docker_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_docker_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_docker_retry, "btn_docker_retry");
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
            // btn_docker_stop
            // 
            this.btn_docker_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_docker_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_docker_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_docker_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_docker_stop, "btn_docker_stop");
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
            // checkBox_select_deploy_docker
            // 
            resources.ApplyResources(this.checkBox_select_deploy_docker, "checkBox_select_deploy_docker");
            this.checkBox_select_deploy_docker.Name = "checkBox_select_deploy_docker";
            this.checkBox_select_deploy_docker.UseVisualStyleBackColor = true;
            this.checkBox_select_deploy_docker.Click += new System.EventHandler(this.checkBox_selectDeplot_docker_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label53);
            this.groupBox5.Controls.Add(this.txt_docker_volume);
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.label46);
            this.groupBox5.Controls.Add(this.txt_docker_other);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.txt_docker_port);
            this.groupBox5.Controls.Add(this.label45);
            this.groupBox5.Controls.Add(this.b_docker_rollback);
            this.groupBox5.Controls.Add(this.txt_docker_envname);
            this.groupBox5.Controls.Add(this.label27);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // label53
            // 
            resources.ApplyResources(this.label53, "label53");
            this.label53.Name = "label53";
            // 
            // txt_docker_volume
            // 
            this.txt_docker_volume.BackColor = System.Drawing.Color.Transparent;
            this.txt_docker_volume.Br = System.Drawing.Color.White;
            resources.ApplyResources(this.txt_docker_volume, "txt_docker_volume");
            this.txt_docker_volume.ForeColor = System.Drawing.Color.DimGray;
            this.txt_docker_volume.Name = "txt_docker_volume";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // txt_docker_other
            // 
            resources.ApplyResources(this.txt_docker_other, "txt_docker_other");
            this.txt_docker_other.Name = "txt_docker_other";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
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
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // b_docker_rollback
            // 
            this.b_docker_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_docker_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_docker_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_docker_rollback, "b_docker_rollback");
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
            // txt_docker_envname
            // 
            resources.ApplyResources(this.txt_docker_envname, "txt_docker_envname");
            this.txt_docker_envname.Name = "txt_docker_envname";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // combo_docker_env
            // 
            this.combo_docker_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_docker_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_docker_env.FormattingEnabled = true;
            resources.ApplyResources(this.combo_docker_env, "combo_docker_env");
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
            this.tabControl_docker.Controls.Add(this.tabPage_docker);
            this.tabControl_docker.Controls.Add(this.tabPage_docker_log);
            this.tabControl_docker.Controls.Add(this.tabPage_docker_repo);
            this.tabControl_docker.Controls.Add(this.tabPage_ssh);
            resources.ApplyResources(this.tabControl_docker, "tabControl_docker");
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
            this.progress_docker_tip.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.progress_docker_tip, "progress_docker_tip");
            this.progress_docker_tip.Name = "progress_docker_tip";
            // 
            // tabPage_docker_log
            // 
            this.tabPage_docker_log.Controls.Add(this.rich_docker_log);
            resources.ApplyResources(this.tabPage_docker_log, "tabPage_docker_log");
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
            // tabPage_docker_repo
            // 
            this.tabPage_docker_repo.Controls.Add(this.label56);
            this.tabPage_docker_repo.Controls.Add(this.label55);
            this.tabPage_docker_repo.Controls.Add(this.label54);
            this.tabPage_docker_repo.Controls.Add(this.checkBoxdocker_rep_uploadOnly);
            this.tabPage_docker_repo.Controls.Add(this.checkBoxdocker_rep_enable);
            this.tabPage_docker_repo.Controls.Add(this.txt_docker_rep_image);
            this.tabPage_docker_repo.Controls.Add(this.label44);
            this.tabPage_docker_repo.Controls.Add(this.label43);
            this.tabPage_docker_repo.Controls.Add(this.txt_docker_rep_namespace);
            this.tabPage_docker_repo.Controls.Add(this.label40);
            this.tabPage_docker_repo.Controls.Add(this.txt_docker_rep_domain);
            this.tabPage_docker_repo.Controls.Add(this.txt_docker_rep_pwd);
            this.tabPage_docker_repo.Controls.Add(this.label31);
            this.tabPage_docker_repo.Controls.Add(this.txt_docker_rep_name);
            this.tabPage_docker_repo.Controls.Add(this.label34);
            resources.ApplyResources(this.tabPage_docker_repo, "tabPage_docker_repo");
            this.tabPage_docker_repo.Name = "tabPage_docker_repo";
            this.tabPage_docker_repo.UseVisualStyleBackColor = true;
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            // 
            // label55
            // 
            resources.ApplyResources(this.label55, "label55");
            this.label55.Name = "label55";
            // 
            // label54
            // 
            resources.ApplyResources(this.label54, "label54");
            this.label54.Name = "label54";
            // 
            // checkBoxdocker_rep_uploadOnly
            // 
            resources.ApplyResources(this.checkBoxdocker_rep_uploadOnly, "checkBoxdocker_rep_uploadOnly");
            this.checkBoxdocker_rep_uploadOnly.Name = "checkBoxdocker_rep_uploadOnly";
            this.checkBoxdocker_rep_uploadOnly.UseVisualStyleBackColor = true;
            this.checkBoxdocker_rep_uploadOnly.Click += new System.EventHandler(this.checkBoxdocker_rep_uploadOnly_Click);
            // 
            // checkBoxdocker_rep_enable
            // 
            resources.ApplyResources(this.checkBoxdocker_rep_enable, "checkBoxdocker_rep_enable");
            this.checkBoxdocker_rep_enable.Name = "checkBoxdocker_rep_enable";
            this.checkBoxdocker_rep_enable.UseVisualStyleBackColor = true;
            this.checkBoxdocker_rep_enable.Click += new System.EventHandler(this.checkBoxdocker_rep_enable_Click);
            // 
            // txt_docker_rep_image
            // 
            resources.ApplyResources(this.txt_docker_rep_image, "txt_docker_rep_image");
            this.txt_docker_rep_image.Name = "txt_docker_rep_image";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // label43
            // 
            resources.ApplyResources(this.label43, "label43");
            this.label43.Name = "label43";
            // 
            // txt_docker_rep_namespace
            // 
            resources.ApplyResources(this.txt_docker_rep_namespace, "txt_docker_rep_namespace");
            this.txt_docker_rep_namespace.Name = "txt_docker_rep_namespace";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // txt_docker_rep_domain
            // 
            resources.ApplyResources(this.txt_docker_rep_domain, "txt_docker_rep_domain");
            this.txt_docker_rep_domain.Name = "txt_docker_rep_domain";
            // 
            // txt_docker_rep_pwd
            // 
            resources.ApplyResources(this.txt_docker_rep_pwd, "txt_docker_rep_pwd");
            this.txt_docker_rep_pwd.Name = "txt_docker_rep_pwd";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // txt_docker_rep_name
            // 
            resources.ApplyResources(this.txt_docker_rep_name, "txt_docker_rep_name");
            this.txt_docker_rep_name.Name = "txt_docker_rep_name";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // tabPage_ssh
            // 
            this.tabPage_ssh.Controls.Add(this.groupBox6);
            this.tabPage_ssh.Controls.Add(this.chk_use_AsiaShanghai_timezone);
            resources.ApplyResources(this.tabPage_ssh, "tabPage_ssh");
            this.tabPage_ssh.Name = "tabPage_ssh";
            this.tabPage_ssh.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.label68);
            this.groupBox6.Controls.Add(this.t_docker_delete_days);
            this.groupBox6.Controls.Add(this.txt_docker_workspace);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // label12
            // 
            this.label12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label12.ForeColor = System.Drawing.Color.RoyalBlue;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label68
            // 
            resources.ApplyResources(this.label68, "label68");
            this.label68.Name = "label68";
            // 
            // t_docker_delete_days
            // 
            this.t_docker_delete_days.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.t_docker_delete_days, "t_docker_delete_days");
            this.t_docker_delete_days.Name = "t_docker_delete_days";
            // 
            // txt_docker_workspace
            // 
            resources.ApplyResources(this.txt_docker_workspace, "txt_docker_workspace");
            this.txt_docker_workspace.Name = "txt_docker_workspace";
            // 
            // chk_use_AsiaShanghai_timezone
            // 
            resources.ApplyResources(this.chk_use_AsiaShanghai_timezone, "chk_use_AsiaShanghai_timezone");
            this.chk_use_AsiaShanghai_timezone.Name = "chk_use_AsiaShanghai_timezone";
            this.chk_use_AsiaShanghai_timezone.UseVisualStyleBackColor = true;
            // 
            // checkBox_Increment_docker
            // 
            resources.ApplyResources(this.checkBox_Increment_docker, "checkBox_Increment_docker");
            this.checkBox_Increment_docker.Name = "checkBox_Increment_docker";
            this.checkBox_Increment_docker.UseVisualStyleBackColor = true;
            this.checkBox_Increment_docker.Click += new System.EventHandler(this.checkBox_Increment_docker_CheckedChanged);
            // 
            // checkBox_sudo_docker
            // 
            resources.ApplyResources(this.checkBox_sudo_docker, "checkBox_sudo_docker");
            this.checkBox_sudo_docker.Name = "checkBox_sudo_docker";
            this.checkBox_sudo_docker.UseVisualStyleBackColor = true;
            this.checkBox_sudo_docker.Click += new System.EventHandler(this.checkBox_sudo_docker_CheckedChanged);
            // 
            // page_docker_img
            // 
            this.page_docker_img.Controls.Add(this.btn_docker_image_stop);
            this.page_docker_img.Controls.Add(this.altoButton1);
            this.page_docker_img.Controls.Add(this.tabControl_dockerimage);
            this.page_docker_img.Controls.Add(this.groupBox2);
            this.page_docker_img.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.page_docker_img, "page_docker_img");
            this.page_docker_img.Name = "page_docker_img";
            this.page_docker_img.UseVisualStyleBackColor = true;
            // 
            // btn_docker_image_stop
            // 
            this.btn_docker_image_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_docker_image_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_docker_image_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_docker_image_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_docker_image_stop, "btn_docker_image_stop");
            this.btn_docker_image_stop.ForeColor = System.Drawing.Color.Red;
            this.btn_docker_image_stop.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_docker_image_stop.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_docker_image_stop.Name = "btn_docker_image_stop";
            this.btn_docker_image_stop.Radius = 10;
            this.btn_docker_image_stop.Stroke = false;
            this.btn_docker_image_stop.StrokeColor = System.Drawing.Color.Gray;
            this.btn_docker_image_stop.Transparency = false;
            this.btn_docker_image_stop.Click += new System.EventHandler(this.btn_docker_image_stop_Click);
            // 
            // altoButton1
            // 
            this.altoButton1.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.altoButton1.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.altoButton1.BackColor = System.Drawing.Color.Transparent;
            this.altoButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.altoButton1, "altoButton1");
            this.altoButton1.ForeColor = System.Drawing.Color.Black;
            this.altoButton1.Inactive1 = System.Drawing.SystemColors.Control;
            this.altoButton1.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.altoButton1.Name = "altoButton1";
            this.altoButton1.Radius = 10;
            this.altoButton1.Stroke = false;
            this.altoButton1.StrokeColor = System.Drawing.Color.Gray;
            this.altoButton1.Transparency = false;
            this.altoButton1.Click += new System.EventHandler(this.altoButton1_Click);
            // 
            // tabControl_dockerimage
            // 
            this.tabControl_dockerimage.Controls.Add(this.tabPage_docker_image);
            this.tabControl_dockerimage.Controls.Add(this.tabPage_docker_image_ignore);
            resources.ApplyResources(this.tabControl_dockerimage, "tabControl_dockerimage");
            this.tabControl_dockerimage.Name = "tabControl_dockerimage";
            this.tabControl_dockerimage.SelectedIndex = 0;
            // 
            // tabPage_docker_image
            // 
            this.tabPage_docker_image.Controls.Add(this.rich_docker_image_log);
            resources.ApplyResources(this.tabPage_docker_image, "tabPage_docker_image");
            this.tabPage_docker_image.Name = "tabPage_docker_image";
            this.tabPage_docker_image.UseVisualStyleBackColor = true;
            // 
            // rich_docker_image_log
            // 
            resources.ApplyResources(this.rich_docker_image_log, "rich_docker_image_log");
            this.rich_docker_image_log.HiglightColor = AntDeployWinform.RtfColor.White;
            this.rich_docker_image_log.Name = "rich_docker_image_log";
            this.rich_docker_image_log.ReadOnly = true;
            this.rich_docker_image_log.TextColor = AntDeployWinform.RtfColor.Black;
            // 
            // tabPage_docker_image_ignore
            // 
            this.tabPage_docker_image_ignore.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.tabPage_docker_image_ignore, "tabPage_docker_image_ignore");
            this.tabPage_docker_image_ignore.Name = "tabPage_docker_image_ignore";
            this.tabPage_docker_image_ignore.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label67);
            this.groupBox4.Controls.Add(this.btn_dockerImage_ignore_remove);
            this.groupBox4.Controls.Add(this.btn_dockerImage_ignore_add);
            this.groupBox4.Controls.Add(this.txt_dockerImage_ignore);
            this.groupBox4.Controls.Add(this.list_dockerImage_ignore);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label67
            // 
            resources.ApplyResources(this.label67, "label67");
            this.label67.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label67.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label67.Name = "label67";
            this.label67.Click += new System.EventHandler(this.label_how_to_set_Click);
            // 
            // btn_dockerImage_ignore_remove
            // 
            this.btn_dockerImage_ignore_remove.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.btn_dockerImage_ignore_remove, "btn_dockerImage_ignore_remove");
            this.btn_dockerImage_ignore_remove.Name = "btn_dockerImage_ignore_remove";
            this.btn_dockerImage_ignore_remove.UseVisualStyleBackColor = true;
            this.btn_dockerImage_ignore_remove.Click += new System.EventHandler(this.btn_dockerImage_ignore_remove_Click);
            // 
            // btn_dockerImage_ignore_add
            // 
            resources.ApplyResources(this.btn_dockerImage_ignore_add, "btn_dockerImage_ignore_add");
            this.btn_dockerImage_ignore_add.Name = "btn_dockerImage_ignore_add";
            this.btn_dockerImage_ignore_add.UseVisualStyleBackColor = true;
            this.btn_dockerImage_ignore_add.Click += new System.EventHandler(this.btn_dockerImage_ignore_add_Click);
            // 
            // txt_dockerImage_ignore
            // 
            resources.ApplyResources(this.txt_dockerImage_ignore, "txt_dockerImage_ignore");
            this.txt_dockerImage_ignore.Name = "txt_dockerImage_ignore";
            // 
            // list_dockerImage_ignore
            // 
            this.list_dockerImage_ignore.FormattingEnabled = true;
            resources.ApplyResources(this.list_dockerImage_ignore, "list_dockerImage_ignore");
            this.list_dockerImage_ignore.Name = "list_dockerImage_ignore";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_how_to_dockerimage);
            this.groupBox2.Controls.Add(this.label59);
            this.groupBox2.Controls.Add(this.label60);
            this.groupBox2.Controls.Add(this.txt_HttpProxy);
            this.groupBox2.Controls.Add(this.txt_BaseImage_pwd);
            this.groupBox2.Controls.Add(this.txt_BaseImage);
            this.groupBox2.Controls.Add(this.label58);
            this.groupBox2.Controls.Add(this.label57);
            this.groupBox2.Controls.Add(this.txt_BaseImage_username);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label_how_to_dockerimage
            // 
            this.label_how_to_dockerimage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_how_to_dockerimage.ForeColor = System.Drawing.Color.RoyalBlue;
            resources.ApplyResources(this.label_how_to_dockerimage, "label_how_to_dockerimage");
            this.label_how_to_dockerimage.Name = "label_how_to_dockerimage";
            this.label_how_to_dockerimage.Click += new System.EventHandler(this.label_how_to_dockerimage_Click);
            // 
            // label59
            // 
            resources.ApplyResources(this.label59, "label59");
            this.label59.Name = "label59";
            // 
            // label60
            // 
            resources.ApplyResources(this.label60, "label60");
            this.label60.Name = "label60";
            // 
            // txt_HttpProxy
            // 
            resources.ApplyResources(this.txt_HttpProxy, "txt_HttpProxy");
            this.txt_HttpProxy.Name = "txt_HttpProxy";
            // 
            // txt_BaseImage_pwd
            // 
            resources.ApplyResources(this.txt_BaseImage_pwd, "txt_BaseImage_pwd");
            this.txt_BaseImage_pwd.Name = "txt_BaseImage_pwd";
            // 
            // txt_BaseImage
            // 
            resources.ApplyResources(this.txt_BaseImage, "txt_BaseImage");
            this.txt_BaseImage.Name = "txt_BaseImage";
            // 
            // label58
            // 
            resources.ApplyResources(this.label58, "label58");
            this.label58.Name = "label58";
            // 
            // label57
            // 
            resources.ApplyResources(this.label57, "label57");
            this.label57.Name = "label57";
            // 
            // txt_BaseImage_username
            // 
            resources.ApplyResources(this.txt_BaseImage_username, "txt_BaseImage_username");
            this.txt_BaseImage_username.Name = "txt_BaseImage_username";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label69);
            this.groupBox3.Controls.Add(this.txt_TargetHttpProxy);
            this.groupBox3.Controls.Add(this.txt_Cmd);
            this.groupBox3.Controls.Add(this.label65);
            this.groupBox3.Controls.Add(this.txt_Entrypoint);
            this.groupBox3.Controls.Add(this.cmbo_ImageFormat);
            this.groupBox3.Controls.Add(this.txt_TargetImage_pwd);
            this.groupBox3.Controls.Add(this.txt_TargetImage_username);
            this.groupBox3.Controls.Add(this.txt_TargetImage);
            this.groupBox3.Controls.Add(this.label64);
            this.groupBox3.Controls.Add(this.txt_TargetImage_tag);
            this.groupBox3.Controls.Add(this.label63);
            this.groupBox3.Controls.Add(this.label66);
            this.groupBox3.Controls.Add(this.label62);
            this.groupBox3.Controls.Add(this.label61);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label69
            // 
            resources.ApplyResources(this.label69, "label69");
            this.label69.Name = "label69";
            // 
            // txt_TargetHttpProxy
            // 
            resources.ApplyResources(this.txt_TargetHttpProxy, "txt_TargetHttpProxy");
            this.txt_TargetHttpProxy.Name = "txt_TargetHttpProxy";
            // 
            // txt_Cmd
            // 
            resources.ApplyResources(this.txt_Cmd, "txt_Cmd");
            this.txt_Cmd.Name = "txt_Cmd";
            // 
            // label65
            // 
            resources.ApplyResources(this.label65, "label65");
            this.label65.Name = "label65";
            // 
            // txt_Entrypoint
            // 
            resources.ApplyResources(this.txt_Entrypoint, "txt_Entrypoint");
            this.txt_Entrypoint.Name = "txt_Entrypoint";
            // 
            // cmbo_ImageFormat
            // 
            this.cmbo_ImageFormat.BackColor = System.Drawing.SystemColors.Window;
            this.cmbo_ImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbo_ImageFormat.FormattingEnabled = true;
            this.cmbo_ImageFormat.Items.AddRange(new object[] {
            resources.GetString("cmbo_ImageFormat.Items"),
            resources.GetString("cmbo_ImageFormat.Items1")});
            resources.ApplyResources(this.cmbo_ImageFormat, "cmbo_ImageFormat");
            this.cmbo_ImageFormat.Name = "cmbo_ImageFormat";
            // 
            // txt_TargetImage_pwd
            // 
            resources.ApplyResources(this.txt_TargetImage_pwd, "txt_TargetImage_pwd");
            this.txt_TargetImage_pwd.Name = "txt_TargetImage_pwd";
            // 
            // txt_TargetImage_username
            // 
            resources.ApplyResources(this.txt_TargetImage_username, "txt_TargetImage_username");
            this.txt_TargetImage_username.Name = "txt_TargetImage_username";
            // 
            // txt_TargetImage
            // 
            resources.ApplyResources(this.txt_TargetImage, "txt_TargetImage");
            this.txt_TargetImage.Name = "txt_TargetImage";
            // 
            // label64
            // 
            resources.ApplyResources(this.label64, "label64");
            this.label64.Name = "label64";
            // 
            // txt_TargetImage_tag
            // 
            resources.ApplyResources(this.txt_TargetImage_tag, "txt_TargetImage_tag");
            this.txt_TargetImage_tag.Name = "txt_TargetImage_tag";
            // 
            // label63
            // 
            resources.ApplyResources(this.label63, "label63");
            this.label63.Name = "label63";
            // 
            // label66
            // 
            resources.ApplyResources(this.label66, "label66");
            this.label66.Name = "label66";
            // 
            // label62
            // 
            resources.ApplyResources(this.label62, "label62");
            this.label62.Name = "label62";
            // 
            // label61
            // 
            resources.ApplyResources(this.label61, "label61");
            this.label61.Name = "label61";
            // 
            // page_window_service
            // 
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
            resources.ApplyResources(this.page_window_service, "page_window_service");
            this.page_window_service.Name = "page_window_service";
            this.page_window_service.UseVisualStyleBackColor = true;
            // 
            // btn_windows_serivce_stop
            // 
            this.btn_windows_serivce_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_windows_serivce_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_windows_serivce_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_windows_serivce_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_windows_serivce_stop, "btn_windows_serivce_stop");
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
            this.btn_windows_service_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_windows_service_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_windows_service_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_windows_service_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_windows_service_retry, "btn_windows_service_retry");
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
            this.b_windowservice_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_windowservice_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_windowservice_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_windowservice_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_windowservice_deploy, "b_windowservice_deploy");
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
            this.combo_windowservice_sdk_type.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_sdk_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_sdk_type.FormattingEnabled = true;
            this.combo_windowservice_sdk_type.Items.AddRange(new object[] {
            resources.GetString("combo_windowservice_sdk_type.Items"),
            resources.GetString("combo_windowservice_sdk_type.Items1")});
            resources.ApplyResources(this.combo_windowservice_sdk_type, "combo_windowservice_sdk_type");
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
            this.combo_windowservice_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_windowservice_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_windowservice_env.FormattingEnabled = true;
            resources.ApplyResources(this.combo_windowservice_env, "combo_windowservice_env");
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
            this.tabControl_window_service.Controls.Add(this.tabPage_windows_service);
            this.tabControl_window_service.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl_window_service, "tabControl_window_service");
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
            this.progress_window_service_tip.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.progress_window_service_tip, "progress_window_service_tip");
            this.progress_window_service_tip.Name = "progress_window_service_tip";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rich_windowservice_log);
            resources.ApplyResources(this.tabPage2, "tabPage2");
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
            this.b_windows_service_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_windows_service_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_windows_service_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_windows_service_rollback, "b_windows_service_rollback");
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
            // page_linux_service
            // 
            this.page_linux_service.Controls.Add(this.label_how_to_linuxservice);
            this.page_linux_service.Controls.Add(this.checkBox_select_type_linuxservice);
            this.page_linux_service.Controls.Add(this.label51);
            this.page_linux_service.Controls.Add(this.txt_linux_service_env);
            this.page_linux_service.Controls.Add(this.label49);
            this.page_linux_service.Controls.Add(this.tabControl_linux_service);
            this.page_linux_service.Controls.Add(this.checkBox_select_deploy_linuxservice);
            this.page_linux_service.Controls.Add(this.checkBox_Increment_linux_service);
            this.page_linux_service.Controls.Add(this.label47);
            this.page_linux_service.Controls.Add(this.txt_linuxservice_name);
            this.page_linux_service.Controls.Add(this.label48);
            this.page_linux_service.Controls.Add(this.combo_linux_env);
            this.page_linux_service.Controls.Add(this.label50);
            this.page_linux_service.Controls.Add(this.btn_linux_serivce_stop);
            this.page_linux_service.Controls.Add(this.b_linuxservice_deploy);
            this.page_linux_service.Controls.Add(this.btn_linux_service_retry);
            this.page_linux_service.Controls.Add(this.b_linux_service_rollback);
            resources.ApplyResources(this.page_linux_service, "page_linux_service");
            this.page_linux_service.Name = "page_linux_service";
            this.page_linux_service.UseVisualStyleBackColor = true;
            // 
            // label_how_to_linuxservice
            // 
            resources.ApplyResources(this.label_how_to_linuxservice, "label_how_to_linuxservice");
            this.label_how_to_linuxservice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_how_to_linuxservice.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_how_to_linuxservice.Name = "label_how_to_linuxservice";
            this.label_how_to_linuxservice.Click += new System.EventHandler(this.label_how_to_linuxservice_Click_1);
            // 
            // checkBox_select_type_linuxservice
            // 
            resources.ApplyResources(this.checkBox_select_type_linuxservice, "checkBox_select_type_linuxservice");
            this.checkBox_select_type_linuxservice.Name = "checkBox_select_type_linuxservice";
            this.checkBox_select_type_linuxservice.UseVisualStyleBackColor = true;
            this.checkBox_select_type_linuxservice.CheckedChanged += new System.EventHandler(this.checkBox_select_type_linuxservice_CheckedChanged);
            // 
            // label51
            // 
            resources.ApplyResources(this.label51, "label51");
            this.label51.Name = "label51";
            // 
            // txt_linux_service_env
            // 
            resources.ApplyResources(this.txt_linux_service_env, "txt_linux_service_env");
            this.txt_linux_service_env.Name = "txt_linux_service_env";
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.Name = "label49";
            // 
            // tabControl_linux_service
            // 
            this.tabControl_linux_service.Controls.Add(this.tabPage_linux_service);
            this.tabControl_linux_service.Controls.Add(this.tabPage3);
            resources.ApplyResources(this.tabControl_linux_service, "tabControl_linux_service");
            this.tabControl_linux_service.Name = "tabControl_linux_service";
            this.tabControl_linux_service.SelectedIndex = 0;
            // 
            // tabPage_linux_service
            // 
            resources.ApplyResources(this.tabPage_linux_service, "tabPage_linux_service");
            this.tabPage_linux_service.Controls.Add(this.progress_linux_service_tip);
            this.tabPage_linux_service.Name = "tabPage_linux_service";
            this.tabPage_linux_service.UseVisualStyleBackColor = true;
            // 
            // progress_linux_service_tip
            // 
            this.progress_linux_service_tip.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.progress_linux_service_tip, "progress_linux_service_tip");
            this.progress_linux_service_tip.Name = "progress_linux_service_tip";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.rich_linuxservice_log);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // rich_linuxservice_log
            // 
            resources.ApplyResources(this.rich_linuxservice_log, "rich_linuxservice_log");
            this.rich_linuxservice_log.HiglightColor = AntDeployWinform.RtfColor.White;
            this.rich_linuxservice_log.Name = "rich_linuxservice_log";
            this.rich_linuxservice_log.ReadOnly = true;
            this.rich_linuxservice_log.TextColor = AntDeployWinform.RtfColor.Black;
            // 
            // checkBox_select_deploy_linuxservice
            // 
            resources.ApplyResources(this.checkBox_select_deploy_linuxservice, "checkBox_select_deploy_linuxservice");
            this.checkBox_select_deploy_linuxservice.Name = "checkBox_select_deploy_linuxservice";
            this.checkBox_select_deploy_linuxservice.UseVisualStyleBackColor = true;
            this.checkBox_select_deploy_linuxservice.CheckedChanged += new System.EventHandler(this.checkBox_select_deploy_linuxservice_CheckedChanged);
            // 
            // checkBox_Increment_linux_service
            // 
            resources.ApplyResources(this.checkBox_Increment_linux_service, "checkBox_Increment_linux_service");
            this.checkBox_Increment_linux_service.Name = "checkBox_Increment_linux_service";
            this.checkBox_Increment_linux_service.UseVisualStyleBackColor = true;
            this.checkBox_Increment_linux_service.CheckedChanged += new System.EventHandler(this.checkBox_Increment_linux_service_CheckedChanged);
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // txt_linuxservice_name
            // 
            resources.ApplyResources(this.txt_linuxservice_name, "txt_linuxservice_name");
            this.txt_linuxservice_name.Name = "txt_linuxservice_name";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.Name = "label48";
            // 
            // combo_linux_env
            // 
            this.combo_linux_env.BackColor = System.Drawing.SystemColors.Window;
            this.combo_linux_env.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_linux_env.FormattingEnabled = true;
            resources.ApplyResources(this.combo_linux_env, "combo_linux_env");
            this.combo_linux_env.Name = "combo_linux_env";
            this.combo_linux_env.SelectedIndexChanged += new System.EventHandler(this.combo_linux_env_SelectedIndexChanged);
            // 
            // label50
            // 
            resources.ApplyResources(this.label50, "label50");
            this.label50.Name = "label50";
            // 
            // btn_linux_serivce_stop
            // 
            this.btn_linux_serivce_stop.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_linux_serivce_stop.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_linux_serivce_stop.BackColor = System.Drawing.Color.Transparent;
            this.btn_linux_serivce_stop.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_linux_serivce_stop, "btn_linux_serivce_stop");
            this.btn_linux_serivce_stop.ForeColor = System.Drawing.Color.Red;
            this.btn_linux_serivce_stop.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_linux_serivce_stop.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_linux_serivce_stop.Name = "btn_linux_serivce_stop";
            this.btn_linux_serivce_stop.Radius = 10;
            this.btn_linux_serivce_stop.Stroke = false;
            this.btn_linux_serivce_stop.StrokeColor = System.Drawing.Color.Gray;
            this.btn_linux_serivce_stop.Transparency = false;
            this.btn_linux_serivce_stop.Click += new System.EventHandler(this.btn_linux_serivce_stop_Click);
            // 
            // b_linuxservice_deploy
            // 
            this.b_linuxservice_deploy.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_linuxservice_deploy.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_linuxservice_deploy.BackColor = System.Drawing.Color.Transparent;
            this.b_linuxservice_deploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_linuxservice_deploy, "b_linuxservice_deploy");
            this.b_linuxservice_deploy.ForeColor = System.Drawing.Color.Black;
            this.b_linuxservice_deploy.Inactive1 = System.Drawing.SystemColors.Control;
            this.b_linuxservice_deploy.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.b_linuxservice_deploy.Name = "b_linuxservice_deploy";
            this.b_linuxservice_deploy.Radius = 10;
            this.b_linuxservice_deploy.Stroke = false;
            this.b_linuxservice_deploy.StrokeColor = System.Drawing.Color.Gray;
            this.b_linuxservice_deploy.Transparency = false;
            this.b_linuxservice_deploy.Click += new System.EventHandler(this.b_linuxservice_deploy_Click);
            // 
            // btn_linux_service_retry
            // 
            this.btn_linux_service_retry.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_linux_service_retry.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_linux_service_retry.BackColor = System.Drawing.Color.Transparent;
            this.btn_linux_service_retry.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_linux_service_retry, "btn_linux_service_retry");
            this.btn_linux_service_retry.ForeColor = System.Drawing.Color.Fuchsia;
            this.btn_linux_service_retry.Inactive1 = System.Drawing.SystemColors.Control;
            this.btn_linux_service_retry.Inactive2 = System.Drawing.SystemColors.ControlLight;
            this.btn_linux_service_retry.Name = "btn_linux_service_retry";
            this.btn_linux_service_retry.Radius = 10;
            this.btn_linux_service_retry.Stroke = false;
            this.btn_linux_service_retry.StrokeColor = System.Drawing.Color.Gray;
            this.btn_linux_service_retry.Transparency = false;
            this.btn_linux_service_retry.Click += new System.EventHandler(this.btn_linux_service_retry_Click);
            // 
            // b_linux_service_rollback
            // 
            this.b_linux_service_rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_linux_service_rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_linux_service_rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_linux_service_rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_linux_service_rollback, "b_linux_service_rollback");
            this.b_linux_service_rollback.ForeColor = System.Drawing.Color.Black;
            this.b_linux_service_rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_linux_service_rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.b_linux_service_rollback.Name = "b_linux_service_rollback";
            this.b_linux_service_rollback.Radius = 10;
            this.b_linux_service_rollback.Stroke = false;
            this.b_linux_service_rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_linux_service_rollback.Transparency = false;
            this.b_linux_service_rollback.Click += new System.EventHandler(this.b_linux_service_rollback_Click);
            // 
            // page_set
            // 
            this.page_set.Controls.Add(this.panel_rich_config_log);
            this.page_set.Controls.Add(this.label_how_to_set);
            this.page_set.Controls.Add(this.groupBox1);
            this.page_set.Controls.Add(this.label_check_update);
            this.page_set.Controls.Add(this.groupBoxIgnore);
            this.page_set.Controls.Add(this.environment);
            resources.ApplyResources(this.page_set, "page_set");
            this.page_set.Name = "page_set";
            this.page_set.UseVisualStyleBackColor = true;
            // 
            // panel_rich_config_log
            // 
            this.panel_rich_config_log.Controls.Add(this.btn_rich_config_log_close);
            this.panel_rich_config_log.Controls.Add(this.rich_config_log);
            resources.ApplyResources(this.panel_rich_config_log, "panel_rich_config_log");
            this.panel_rich_config_log.Name = "panel_rich_config_log";
            // 
            // btn_rich_config_log_close
            // 
            this.btn_rich_config_log_close.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_rich_config_log_close.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_rich_config_log_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_rich_config_log_close.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_rich_config_log_close, "btn_rich_config_log_close");
            this.btn_rich_config_log_close.ForeColor = System.Drawing.Color.Black;
            this.btn_rich_config_log_close.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_rich_config_log_close.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_rich_config_log_close.Name = "btn_rich_config_log_close";
            this.btn_rich_config_log_close.Radius = 10;
            this.btn_rich_config_log_close.Stroke = false;
            this.btn_rich_config_log_close.StrokeColor = System.Drawing.Color.Gray;
            this.btn_rich_config_log_close.Transparency = false;
            this.btn_rich_config_log_close.Click += new System.EventHandler(this.btn_rich_config_log_close_Click);
            // 
            // rich_config_log
            // 
            this.rich_config_log.HiglightColor = AntDeployWinform.RtfColor.White;
            resources.ApplyResources(this.rich_config_log, "rich_config_log");
            this.rich_config_log.Name = "rich_config_log";
            this.rich_config_log.TextColor = AntDeployWinform.RtfColor.Black;
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
            this.groupBox1.Controls.Add(this.b_copy_backup_ignore);
            this.groupBox1.Controls.Add(this.b_backUp_ignore_remove);
            this.groupBox1.Controls.Add(this.b_backUp_ignore_add);
            this.groupBox1.Controls.Add(this.txt_backUp_ignore);
            this.groupBox1.Controls.Add(this.list_backUp_ignore);
            resources.ApplyResources(this.groupBox1, "groupBox1");
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
            this.list_backUp_ignore.FormattingEnabled = true;
            resources.ApplyResources(this.list_backUp_ignore, "list_backUp_ignore");
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
            this.groupBoxIgnore.Controls.Add(this.b_copy_pack_ignore);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_remove);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_add);
            this.groupBoxIgnore.Controls.Add(this.txt_env_ignore);
            this.groupBoxIgnore.Controls.Add(this.list_env_ignore);
            resources.ApplyResources(this.groupBoxIgnore, "groupBoxIgnore");
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
            this.list_env_ignore.FormattingEnabled = true;
            resources.ApplyResources(this.list_env_ignore, "list_env_ignore");
            this.list_env_ignore.Name = "list_env_ignore";
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
            resources.ApplyResources(this.environment, "environment");
            this.environment.Name = "environment";
            this.environment.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.page_winserver);
            this.tabControl1.Controls.Add(this.page_linux_server);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // page_winserver
            // 
            this.page_winserver.Controls.Add(this.btn_win_server_testAll);
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
            resources.ApplyResources(this.page_winserver, "page_winserver");
            this.page_winserver.Name = "page_winserver";
            this.page_winserver.UseVisualStyleBackColor = true;
            // 
            // btn_win_server_testAll
            // 
            this.btn_win_server_testAll.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_win_server_testAll.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_win_server_testAll.BackColor = System.Drawing.Color.Transparent;
            this.btn_win_server_testAll.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_win_server_testAll, "btn_win_server_testAll");
            this.btn_win_server_testAll.ForeColor = System.Drawing.Color.Black;
            this.btn_win_server_testAll.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.btn_win_server_testAll.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(167)))), ((int)(((byte)(188)))));
            this.btn_win_server_testAll.Name = "btn_win_server_testAll";
            this.btn_win_server_testAll.Radius = 10;
            this.btn_win_server_testAll.Stroke = false;
            this.btn_win_server_testAll.StrokeColor = System.Drawing.Color.Gray;
            this.btn_win_server_testAll.Transparency = false;
            this.btn_win_server_testAll.Click += new System.EventHandler(this.btn_win_server_testAll_Click);
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
            this.loading_win_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_win_server_test.FullTransparent = true;
            this.loading_win_server_test.Increment = 1F;
            resources.ApplyResources(this.loading_win_server_test, "loading_win_server_test");
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
            this.combo_env_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_server_list.FormattingEnabled = true;
            resources.ApplyResources(this.combo_env_server_list, "combo_env_server_list");
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
            this.page_linux_server.Controls.Add(this.btn_linux_server_testAll);
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
            resources.ApplyResources(this.page_linux_server, "page_linux_server");
            this.page_linux_server.Name = "page_linux_server";
            this.page_linux_server.UseVisualStyleBackColor = true;
            // 
            // btn_linux_server_testAll
            // 
            this.btn_linux_server_testAll.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_linux_server_testAll.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_linux_server_testAll.BackColor = System.Drawing.Color.Transparent;
            this.btn_linux_server_testAll.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_linux_server_testAll, "btn_linux_server_testAll");
            this.btn_linux_server_testAll.ForeColor = System.Drawing.Color.Black;
            this.btn_linux_server_testAll.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.btn_linux_server_testAll.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(167)))), ((int)(((byte)(188)))));
            this.btn_linux_server_testAll.Name = "btn_linux_server_testAll";
            this.btn_linux_server_testAll.Radius = 10;
            this.btn_linux_server_testAll.Stroke = false;
            this.btn_linux_server_testAll.StrokeColor = System.Drawing.Color.Gray;
            this.btn_linux_server_testAll.Transparency = false;
            this.btn_linux_server_testAll.Click += new System.EventHandler(this.btn_linux_server_testAll_Click);
            // 
            // txt_linux_server_nickname
            // 
            resources.ApplyResources(this.txt_linux_server_nickname, "txt_linux_server_nickname");
            this.txt_linux_server_nickname.Name = "txt_linux_server_nickname";
            // 
            // loading_linux_server_test
            // 
            this.loading_linux_server_test.BackColor = System.Drawing.Color.Transparent;
            this.loading_linux_server_test.FullTransparent = true;
            this.loading_linux_server_test.Increment = 1F;
            resources.ApplyResources(this.loading_linux_server_test, "loading_linux_server_test");
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
            this.combo_linux_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_linux_server_list.FormattingEnabled = true;
            resources.ApplyResources(this.combo_linux_server_list, "combo_linux_server_list");
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
            this.b_env_remove.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.b_env_remove, "b_env_remove");
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
            this.combo_env_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_list.FormattingEnabled = true;
            resources.ApplyResources(this.combo_env_list, "combo_env_list");
            this.combo_env_list.Name = "combo_env_list";
            this.combo_env_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_list_SelectedIndexChanged);
            // 
            // pag_advance_setting
            // 
            this.pag_advance_setting.Controls.Add(this.chk_global_saveconfig_in_projectFolder);
            this.pag_advance_setting.Controls.Add(this.btn_auto_find_msbuild);
            this.pag_advance_setting.Controls.Add(this.label52);
            this.pag_advance_setting.Controls.Add(this.checkBox_multi_deploy);
            this.pag_advance_setting.Controls.Add(this.checkBox_save_deploy_log);
            this.pag_advance_setting.Controls.Add(this.label33);
            this.pag_advance_setting.Controls.Add(this.txt_http_proxy);
            this.pag_advance_setting.Controls.Add(this.label30);
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
            this.pag_advance_setting.Controls.Add(this.btn_shang);
            resources.ApplyResources(this.pag_advance_setting, "pag_advance_setting");
            this.pag_advance_setting.Name = "pag_advance_setting";
            this.pag_advance_setting.UseVisualStyleBackColor = true;
            // 
            // chk_global_saveconfig_in_projectFolder
            // 
            resources.ApplyResources(this.chk_global_saveconfig_in_projectFolder, "chk_global_saveconfig_in_projectFolder");
            this.chk_global_saveconfig_in_projectFolder.Name = "chk_global_saveconfig_in_projectFolder";
            this.chk_global_saveconfig_in_projectFolder.UseVisualStyleBackColor = true;
            this.chk_global_saveconfig_in_projectFolder.Click += new System.EventHandler(this.chk_global_saveconfig_in_projectFolder_Click);
            // 
            // btn_auto_find_msbuild
            // 
            this.btn_auto_find_msbuild.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.btn_auto_find_msbuild, "btn_auto_find_msbuild");
            this.btn_auto_find_msbuild.Name = "btn_auto_find_msbuild";
            this.btn_auto_find_msbuild.UseVisualStyleBackColor = true;
            this.btn_auto_find_msbuild.Click += new System.EventHandler(this.btn_auto_find_msbuild_Click);
            // 
            // label52
            // 
            resources.ApplyResources(this.label52, "label52");
            this.label52.Name = "label52";
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
            // label16
            // 
            this.label16.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label16.ForeColor = System.Drawing.Color.RoyalBlue;
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // btn_folder_clear
            // 
            this.btn_folder_clear.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.btn_folder_clear, "btn_folder_clear");
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
            this.combo_netcore_publish_mode.BackColor = System.Drawing.SystemColors.Window;
            this.combo_netcore_publish_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_netcore_publish_mode.FormattingEnabled = true;
            this.combo_netcore_publish_mode.Items.AddRange(new object[] {
            resources.GetString("combo_netcore_publish_mode.Items"),
            resources.GetString("combo_netcore_publish_mode.Items1"),
            resources.GetString("combo_netcore_publish_mode.Items2"),
            resources.GetString("combo_netcore_publish_mode.Items3"),
            resources.GetString("combo_netcore_publish_mode.Items4"),
            resources.GetString("combo_netcore_publish_mode.Items5"),
            resources.GetString("combo_netcore_publish_mode.Items6"),
            resources.GetString("combo_netcore_publish_mode.Items7"),
            resources.GetString("combo_netcore_publish_mode.Items8")});
            resources.ApplyResources(this.combo_netcore_publish_mode, "combo_netcore_publish_mode");
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
            // btn_shang
            // 
            this.btn_shang.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_shang.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_shang.BackColor = System.Drawing.Color.Transparent;
            this.btn_shang.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_shang, "btn_shang");
            this.btn_shang.ForeColor = System.Drawing.Color.Black;
            this.btn_shang.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_shang.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btn_shang.Name = "btn_shang";
            this.btn_shang.Radius = 10;
            this.btn_shang.Stroke = false;
            this.btn_shang.StrokeColor = System.Drawing.Color.Gray;
            this.btn_shang.Transparency = false;
            this.btn_shang.Click += new System.EventHandler(this.btn_shang_Click);
            // 
            // Deploy
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.tabcontrol);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MiniDownBack = ((System.Drawing.Image)(resources.GetObject("$this.MiniDownBack")));
            this.MiniMouseBack = ((System.Drawing.Image)(resources.GetObject("$this.MiniMouseBack")));
            this.MiniNormlBack = ((System.Drawing.Image)(resources.GetObject("$this.MiniNormlBack")));
            this.Name = "Deploy";
            cmSysButton1.Bounds = new System.Drawing.Rectangle(552, 0, 23, 20);
            cmSysButton1.BoxState = CCWin.ControlBoxState.Normal;
            cmSysButton1.Location = new System.Drawing.Point(552, 0);
            cmSysButton1.Name = "btn_question";
            cmSysButton1.OwnerForm = this;
            cmSysButton1.Size = new System.Drawing.Size(23, 20);
            cmSysButton1.SysButtonDown = ((System.Drawing.Image)(resources.GetObject("cmSysButton1.SysButtonDown")));
            cmSysButton1.SysButtonMouse = ((System.Drawing.Image)(resources.GetObject("cmSysButton1.SysButtonMouse")));
            cmSysButton1.SysButtonNorml = ((System.Drawing.Image)(resources.GetObject("cmSysButton1.SysButtonNorml")));
            cmSysButton1.ToolTip = "使用上遇到问题？";
            cmSysButton2.Bounds = new System.Drawing.Rectangle(529, 0, 23, 20);
            cmSysButton2.BoxState = CCWin.ControlBoxState.Normal;
            cmSysButton2.Location = new System.Drawing.Point(529, 0);
            cmSysButton2.Name = "btn_open_new";
            cmSysButton2.OwnerForm = this;
            cmSysButton2.Size = new System.Drawing.Size(23, 20);
            cmSysButton2.SysButtonDown = ((System.Drawing.Image)(resources.GetObject("cmSysButton2.SysButtonDown")));
            cmSysButton2.SysButtonMouse = ((System.Drawing.Image)(resources.GetObject("cmSysButton2.SysButtonMouse")));
            cmSysButton2.SysButtonNorml = ((System.Drawing.Image)(resources.GetObject("cmSysButton2.SysButtonNorml")));
            cmSysButton2.ToolTip = "Open new Window";
            this.SysButtonItems.AddRange(new CCWin.CmSysButton[] {
            cmSysButton1,
            cmSysButton2});
            this.SysBottomClick += new CCWin.CCSkinMain.SysBottomEventHandler(this.Deploy_SysBottomClick);
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
            this.tabPage_docker_repo.ResumeLayout(false);
            this.tabPage_docker_repo.PerformLayout();
            this.tabPage_ssh.ResumeLayout(false);
            this.tabPage_ssh.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.page_docker_img.ResumeLayout(false);
            this.tabControl_dockerimage.ResumeLayout(false);
            this.tabPage_docker_image.ResumeLayout(false);
            this.tabPage_docker_image_ignore.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.page_window_service.ResumeLayout(false);
            this.page_window_service.PerformLayout();
            this.tabControl_window_service.ResumeLayout(false);
            this.tabPage_windows_service.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.page_linux_service.ResumeLayout(false);
            this.page_linux_service.PerformLayout();
            this.tabControl_linux_service.ResumeLayout(false);
            this.tabPage_linux_service.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.page_set.ResumeLayout(false);
            this.page_set.PerformLayout();
            this.panel_rich_config_log.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox checkBox_iis_restart_site;
        private System.Windows.Forms.CheckBox checkBox_iis_use_offlinehtm;
        private System.Windows.Forms.TabPage tabPage_docker_repo;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.TextBox txt_docker_rep_name;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox txt_docker_rep_pwd;
        private System.Windows.Forms.TextBox txt_docker_rep_image;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox txt_docker_rep_namespace;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox txt_docker_rep_domain;
        private System.Windows.Forms.CheckBox checkBoxdocker_rep_enable;
        private System.Windows.Forms.CheckBox checkBoxdocker_rep_uploadOnly;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.TextBox txt_docker_other;
        private System.Windows.Forms.Label label45;
        private AltoControls.AltoButton btn_win_server_testAll;
        private AltoControls.AltoButton btn_linux_server_testAll;
        private System.Windows.Forms.TabPage page_linux_service;
        private AltoControls.AltoButton btn_linux_serivce_stop;
        private AltoControls.AltoButton btn_linux_service_retry;
        private System.Windows.Forms.TabControl tabControl_linux_service;
        private System.Windows.Forms.TabPage tabPage_linux_service;
        private System.Windows.Forms.Label progress_linux_service_tip;
        private System.Windows.Forms.TabPage tabPage3;
        private ExRichTextBox rich_linuxservice_log;
        private System.Windows.Forms.CheckBox checkBox_select_deploy_linuxservice;
        private System.Windows.Forms.CheckBox checkBox_Increment_linux_service;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.TextBox txt_linuxservice_name;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.ComboBox combo_linux_env;
        private System.Windows.Forms.Label label50;
        private AltoControls.AltoButton b_linux_service_rollback;
        private AltoControls.AltoButton b_linuxservice_deploy;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox txt_linux_service_env;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.CheckBox checkBox_select_type_linuxservice;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.CheckBox checkBox_sudo_docker;
        private System.Windows.Forms.Label label54;
        private System.Windows.Forms.Label label55;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.TabPage page_docker_img;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label60;
        private System.Windows.Forms.TextBox txt_HttpProxy;
        private System.Windows.Forms.TextBox txt_BaseImage_pwd;
        private System.Windows.Forms.TextBox txt_BaseImage;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.TextBox txt_BaseImage_username;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label61;
        private System.Windows.Forms.TextBox txt_TargetImage_pwd;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.TextBox txt_TargetImage_username;
        private System.Windows.Forms.TextBox txt_TargetImage;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox txt_TargetImage_tag;
        private System.Windows.Forms.Label label66;
        private System.Windows.Forms.TextBox txt_Cmd;
        private System.Windows.Forms.Label label65;
        private System.Windows.Forms.TextBox txt_Entrypoint;
        private System.Windows.Forms.ComboBox cmbo_ImageFormat;
        private System.Windows.Forms.TabControl tabControl_dockerimage;
        private System.Windows.Forms.TabPage tabPage_docker_image;
        private ExRichTextBox rich_docker_image_log;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label label69;
        private System.Windows.Forms.TextBox txt_TargetHttpProxy;
        private AltoControls.AltoButton btn_docker_image_stop;
        private AltoControls.AltoButton altoButton1;
        private System.Windows.Forms.TabPage tabPage_docker_image_ignore;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_dockerImage_ignore_add;
        private System.Windows.Forms.TextBox txt_dockerImage_ignore;
        private System.Windows.Forms.ListBox list_dockerImage_ignore;
        private System.Windows.Forms.Button btn_dockerImage_ignore_remove;
        private System.Windows.Forms.Panel panel_rich_config_log;
        private AltoControls.AltoButton btn_rich_config_log_close;
        private ExRichTextBox rich_config_log;
        private System.Windows.Forms.Label label67;
        private System.Windows.Forms.Button btn_auto_find_msbuild;
        private System.Windows.Forms.Label label_how_to_linuxservice;
        private System.Windows.Forms.Label label_how_to_dockerimage;
        private System.Windows.Forms.CheckBox chk_global_saveconfig_in_projectFolder;
        private System.Windows.Forms.TabPage tabPage_ssh;
        private System.Windows.Forms.Label label68;
        private System.Windows.Forms.TextBox txt_docker_workspace;
        private System.Windows.Forms.CheckBox chk_use_AsiaShanghai_timezone;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}