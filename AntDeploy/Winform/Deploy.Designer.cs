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
            this.page_ = new System.Windows.Forms.TabControl();
            this.page_web_iis = new System.Windows.Forms.TabPage();
            this.b_iis_deploy = new System.Windows.Forms.Button();
            this.combo_iis_env = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rich_iis_log = new System.Windows.Forms.RichTextBox();
            this.txt_iis_web_site_name = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.combo_iis_sdk_type = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.page_docker = new System.Windows.Forms.TabPage();
            this.page_window_service = new System.Windows.Forms.TabPage();
            this.page_set = new System.Windows.Forms.TabPage();
            this.groupBoxIgnore = new System.Windows.Forms.GroupBox();
            this.b_env_ignore_remove = new System.Windows.Forms.Button();
            this.b_env_ignore_add = new System.Windows.Forms.Button();
            this.txt_env_ignore = new System.Windows.Forms.TextBox();
            this.list_env_ignore = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.b_env_server_test = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.b_env_server_add = new System.Windows.Forms.Button();
            this.b_env_server_remove = new System.Windows.Forms.Button();
            this.txt_env_server_token = new System.Windows.Forms.TextBox();
            this.combo_env_server_list = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_env_server_host = new System.Windows.Forms.TextBox();
            this.environment = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.b_env_remove = new System.Windows.Forms.Button();
            this.txt_env_name = new System.Windows.Forms.TextBox();
            this.b_env_add_by_name = new System.Windows.Forms.Button();
            this.combo_env_list = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.page_.SuspendLayout();
            this.page_web_iis.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.page_set.SuspendLayout();
            this.groupBoxIgnore.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.environment.SuspendLayout();
            this.SuspendLayout();
            // 
            // page_
            // 
            this.page_.Controls.Add(this.page_web_iis);
            this.page_.Controls.Add(this.page_docker);
            this.page_.Controls.Add(this.page_window_service);
            this.page_.Controls.Add(this.page_set);
            this.page_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page_.Location = new System.Drawing.Point(0, 0);
            this.page_.Name = "page_";
            this.page_.SelectedIndex = 0;
            this.page_.Size = new System.Drawing.Size(634, 448);
            this.page_.TabIndex = 0;
            // 
            // page_web_iis
            // 
            this.page_web_iis.Controls.Add(this.label9);
            this.page_web_iis.Controls.Add(this.b_iis_deploy);
            this.page_web_iis.Controls.Add(this.combo_iis_env);
            this.page_web_iis.Controls.Add(this.label8);
            this.page_web_iis.Controls.Add(this.groupBox2);
            this.page_web_iis.Controls.Add(this.txt_iis_web_site_name);
            this.page_web_iis.Controls.Add(this.label7);
            this.page_web_iis.Controls.Add(this.combo_iis_sdk_type);
            this.page_web_iis.Controls.Add(this.label6);
            this.page_web_iis.Location = new System.Drawing.Point(4, 22);
            this.page_web_iis.Name = "page_web_iis";
            this.page_web_iis.Padding = new System.Windows.Forms.Padding(3);
            this.page_web_iis.Size = new System.Drawing.Size(626, 422);
            this.page_web_iis.TabIndex = 0;
            this.page_web_iis.Text = "IIS_Web";
            this.page_web_iis.UseVisualStyleBackColor = true;
            // 
            // b_iis_deploy
            // 
            this.b_iis_deploy.Location = new System.Drawing.Point(450, 66);
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
            this.combo_iis_env.Location = new System.Drawing.Point(160, 105);
            this.combo_iis_env.Name = "combo_iis_env";
            this.combo_iis_env.Size = new System.Drawing.Size(193, 20);
            this.combo_iis_env.TabIndex = 8;
            this.combo_iis_env.SelectedIndexChanged += new System.EventHandler(this.combo_iis_env_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(83, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "Env Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rich_iis_log);
            this.groupBox2.Location = new System.Drawing.Point(6, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(614, 277);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LOG";
            // 
            // rich_iis_log
            // 
            this.rich_iis_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rich_iis_log.Location = new System.Drawing.Point(3, 17);
            this.rich_iis_log.Name = "rich_iis_log";
            this.rich_iis_log.Size = new System.Drawing.Size(608, 257);
            this.rich_iis_log.TabIndex = 0;
            this.rich_iis_log.Text = "";
            // 
            // txt_iis_web_site_name
            // 
            this.txt_iis_web_site_name.Location = new System.Drawing.Point(160, 57);
            this.txt_iis_web_site_name.Name = "txt_iis_web_site_name";
            this.txt_iis_web_site_name.Size = new System.Drawing.Size(193, 21);
            this.txt_iis_web_site_name.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(59, 60);
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
            this.combo_iis_sdk_type.Location = new System.Drawing.Point(160, 20);
            this.combo_iis_sdk_type.Name = "combo_iis_sdk_type";
            this.combo_iis_sdk_type.Size = new System.Drawing.Size(121, 20);
            this.combo_iis_sdk_type.TabIndex = 1;
            this.combo_iis_sdk_type.SelectedIndexChanged += new System.EventHandler(this.combo_iis_sdk_type_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(83, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "SDK Type:";
            // 
            // page_docker
            // 
            this.page_docker.Location = new System.Drawing.Point(4, 22);
            this.page_docker.Name = "page_docker";
            this.page_docker.Size = new System.Drawing.Size(626, 422);
            this.page_docker.TabIndex = 2;
            this.page_docker.Text = "Docker";
            this.page_docker.UseVisualStyleBackColor = true;
            // 
            // page_window_service
            // 
            this.page_window_service.Location = new System.Drawing.Point(4, 22);
            this.page_window_service.Name = "page_window_service";
            this.page_window_service.Size = new System.Drawing.Size(626, 422);
            this.page_window_service.TabIndex = 3;
            this.page_window_service.Text = "WindowsService";
            this.page_window_service.UseVisualStyleBackColor = true;
            // 
            // page_set
            // 
            this.page_set.Controls.Add(this.groupBoxIgnore);
            this.page_set.Controls.Add(this.groupBox1);
            this.page_set.Controls.Add(this.environment);
            this.page_set.Location = new System.Drawing.Point(4, 22);
            this.page_set.Name = "page_set";
            this.page_set.Padding = new System.Windows.Forms.Padding(3);
            this.page_set.Size = new System.Drawing.Size(626, 422);
            this.page_set.TabIndex = 1;
            this.page_set.Text = "Setting";
            this.page_set.UseVisualStyleBackColor = true;
            // 
            // groupBoxIgnore
            // 
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_remove);
            this.groupBoxIgnore.Controls.Add(this.b_env_ignore_add);
            this.groupBoxIgnore.Controls.Add(this.txt_env_ignore);
            this.groupBoxIgnore.Controls.Add(this.list_env_ignore);
            this.groupBoxIgnore.Location = new System.Drawing.Point(19, 269);
            this.groupBoxIgnore.Name = "groupBoxIgnore";
            this.groupBoxIgnore.Size = new System.Drawing.Size(575, 145);
            this.groupBoxIgnore.TabIndex = 2;
            this.groupBoxIgnore.TabStop = false;
            this.groupBoxIgnore.Text = "IgnoreRule";
            // 
            // b_env_ignore_remove
            // 
            this.b_env_ignore_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_ignore_remove.Location = new System.Drawing.Point(120, 89);
            this.b_env_ignore_remove.Name = "b_env_ignore_remove";
            this.b_env_ignore_remove.Size = new System.Drawing.Size(140, 23);
            this.b_env_ignore_remove.TabIndex = 12;
            this.b_env_ignore_remove.Text = "Remove Selected";
            this.b_env_ignore_remove.UseVisualStyleBackColor = true;
            this.b_env_ignore_remove.Click += new System.EventHandler(this.b_env_ignore_remove_Click);
            // 
            // b_env_ignore_add
            // 
            this.b_env_ignore_add.Location = new System.Drawing.Point(24, 88);
            this.b_env_ignore_add.Name = "b_env_ignore_add";
            this.b_env_ignore_add.Size = new System.Drawing.Size(59, 24);
            this.b_env_ignore_add.TabIndex = 17;
            this.b_env_ignore_add.Text = "Add";
            this.b_env_ignore_add.UseVisualStyleBackColor = true;
            this.b_env_ignore_add.Click += new System.EventHandler(this.b_env_ignore_add_Click);
            // 
            // txt_env_ignore
            // 
            this.txt_env_ignore.Location = new System.Drawing.Point(24, 43);
            this.txt_env_ignore.Name = "txt_env_ignore";
            this.txt_env_ignore.Size = new System.Drawing.Size(236, 21);
            this.txt_env_ignore.TabIndex = 4;
            // 
            // list_env_ignore
            // 
            this.list_env_ignore.FormattingEnabled = true;
            this.list_env_ignore.ItemHeight = 12;
            this.list_env_ignore.Location = new System.Drawing.Point(277, 15);
            this.list_env_ignore.Name = "list_env_ignore";
            this.list_env_ignore.Size = new System.Drawing.Size(292, 124);
            this.list_env_ignore.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.b_env_server_test);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.b_env_server_add);
            this.groupBox1.Controls.Add(this.b_env_server_remove);
            this.groupBox1.Controls.Add(this.txt_env_server_token);
            this.groupBox1.Controls.Add(this.combo_env_server_list);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txt_env_server_host);
            this.groupBox1.Location = new System.Drawing.Point(19, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(575, 150);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // b_env_server_test
            // 
            this.b_env_server_test.Enabled = false;
            this.b_env_server_test.Location = new System.Drawing.Point(211, 68);
            this.b_env_server_test.Name = "b_env_server_test";
            this.b_env_server_test.Size = new System.Drawing.Size(129, 23);
            this.b_env_server_test.TabIndex = 11;
            this.b_env_server_test.Text = "Connect Test";
            this.b_env_server_test.UseVisualStyleBackColor = true;
            this.b_env_server_test.Click += new System.EventHandler(this.b_env_server_test_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "Server List：";
            // 
            // b_env_server_add
            // 
            this.b_env_server_add.Enabled = false;
            this.b_env_server_add.Location = new System.Drawing.Point(18, 68);
            this.b_env_server_add.Name = "b_env_server_add";
            this.b_env_server_add.Size = new System.Drawing.Size(129, 23);
            this.b_env_server_add.TabIndex = 7;
            this.b_env_server_add.Text = "Add Server";
            this.b_env_server_add.UseVisualStyleBackColor = true;
            this.b_env_server_add.Click += new System.EventHandler(this.b_env_server_add_Click);
            // 
            // b_env_server_remove
            // 
            this.b_env_server_remove.Enabled = false;
            this.b_env_server_remove.ForeColor = System.Drawing.Color.Red;
            this.b_env_server_remove.Location = new System.Drawing.Point(418, 68);
            this.b_env_server_remove.Name = "b_env_server_remove";
            this.b_env_server_remove.Size = new System.Drawing.Size(140, 23);
            this.b_env_server_remove.TabIndex = 8;
            this.b_env_server_remove.Text = "Remove Selected";
            this.b_env_server_remove.UseVisualStyleBackColor = true;
            this.b_env_server_remove.Click += new System.EventHandler(this.b_env_server_remove_Click);
            // 
            // txt_env_server_token
            // 
            this.txt_env_server_token.Location = new System.Drawing.Point(266, 27);
            this.txt_env_server_token.Name = "txt_env_server_token";
            this.txt_env_server_token.Size = new System.Drawing.Size(303, 21);
            this.txt_env_server_token.TabIndex = 3;
            // 
            // combo_env_server_list
            // 
            this.combo_env_server_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_env_server_list.FormattingEnabled = true;
            this.combo_env_server_list.Location = new System.Drawing.Point(105, 119);
            this.combo_env_server_list.Name = "combo_env_server_list";
            this.combo_env_server_list.Size = new System.Drawing.Size(464, 20);
            this.combo_env_server_list.TabIndex = 7;
            this.combo_env_server_list.SelectedIndexChanged += new System.EventHandler(this.combo_env_server_list_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(213, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Token：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Host：";
            // 
            // txt_env_server_host
            // 
            this.txt_env_server_host.Location = new System.Drawing.Point(63, 27);
            this.txt_env_server_host.Name = "txt_env_server_host";
            this.txt_env_server_host.Size = new System.Drawing.Size(125, 21);
            this.txt_env_server_host.TabIndex = 2;
            // 
            // environment
            // 
            this.environment.Controls.Add(this.label2);
            this.environment.Controls.Add(this.label1);
            this.environment.Controls.Add(this.b_env_remove);
            this.environment.Controls.Add(this.txt_env_name);
            this.environment.Controls.Add(this.b_env_add_by_name);
            this.environment.Controls.Add(this.combo_env_list);
            this.environment.Location = new System.Drawing.Point(19, 17);
            this.environment.Name = "environment";
            this.environment.Size = new System.Drawing.Size(575, 90);
            this.environment.TabIndex = 0;
            this.environment.TabStop = false;
            this.environment.Text = "Environment";
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
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(158, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 12);
            this.label9.TabIndex = 10;
            this.label9.Text = "For example: Default Web Site/test";
            // 
            // Deploy
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(634, 448);
            this.Controls.Add(this.page_);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Deploy";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AntDeploy";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Deploy_FormClosing);
            this.Load += new System.EventHandler(this.Deploy_Load);
            this.page_.ResumeLayout(false);
            this.page_web_iis.ResumeLayout(false);
            this.page_web_iis.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.page_set.ResumeLayout(false);
            this.groupBoxIgnore.ResumeLayout(false);
            this.groupBoxIgnore.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.environment.ResumeLayout(false);
            this.environment.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl page_;
        private System.Windows.Forms.TabPage page_web_iis;
        private System.Windows.Forms.TabPage page_set;
        private System.Windows.Forms.GroupBox environment;
        private System.Windows.Forms.Button b_env_add_by_name;
        private System.Windows.Forms.ComboBox combo_env_list;
        private System.Windows.Forms.TextBox txt_env_name;
        private System.Windows.Forms.Button b_env_remove;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox combo_iis_env;
        private System.Windows.Forms.Button b_iis_deploy;
        private System.Windows.Forms.GroupBox groupBoxIgnore;
        private System.Windows.Forms.Button b_env_ignore_remove;
        private System.Windows.Forms.Button b_env_ignore_add;
        private System.Windows.Forms.TextBox txt_env_ignore;
        private System.Windows.Forms.ListBox list_env_ignore;
        private System.Windows.Forms.RichTextBox rich_iis_log;
        private System.Windows.Forms.Label label9;
    }
}