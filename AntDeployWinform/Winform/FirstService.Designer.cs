namespace AntDeployWinform.Winform
{
    partial class FirstService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstService));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_service_description = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txt_windows_service_PhysicalPath = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.btn_continue = new AltoControls.AltoButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_service_start_type = new System.Windows.Forms.ComboBox();
            this.txt_service_param = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.box_windows_service_nssm = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txt_service_description
            // 
            resources.ApplyResources(this.txt_service_description, "txt_service_description");
            this.txt_service_description.Name = "txt_service_description";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // txt_windows_service_PhysicalPath
            // 
            resources.ApplyResources(this.txt_windows_service_PhysicalPath, "txt_windows_service_PhysicalPath");
            this.txt_windows_service_PhysicalPath.Name = "txt_windows_service_PhysicalPath";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // btn_continue
            // 
            resources.ApplyResources(this.btn_continue, "btn_continue");
            this.btn_continue.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_continue.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_continue.BackColor = System.Drawing.Color.Transparent;
            this.btn_continue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_continue.ForeColor = System.Drawing.Color.Black;
            this.btn_continue.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.btn_continue.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(167)))), ((int)(((byte)(188)))));
            this.btn_continue.Name = "btn_continue";
            this.btn_continue.Radius = 10;
            this.btn_continue.Stroke = false;
            this.btn_continue.StrokeColor = System.Drawing.Color.Gray;
            this.btn_continue.Transparency = false;
            this.btn_continue.Click += new System.EventHandler(this.btn_continue_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBox_service_start_type
            // 
            resources.ApplyResources(this.comboBox_service_start_type, "comboBox_service_start_type");
            this.comboBox_service_start_type.FormattingEnabled = true;
            this.comboBox_service_start_type.Items.AddRange(new object[] {
            resources.GetString("comboBox_service_start_type.Items"),
            resources.GetString("comboBox_service_start_type.Items1")});
            this.comboBox_service_start_type.Name = "comboBox_service_start_type";
            // 
            // txt_service_param
            // 
            resources.ApplyResources(this.txt_service_param, "txt_service_param");
            this.txt_service_param.Name = "txt_service_param";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // box_windows_service_nssm
            // 
            resources.ApplyResources(this.box_windows_service_nssm, "box_windows_service_nssm");
            this.box_windows_service_nssm.Name = "box_windows_service_nssm";
            this.box_windows_service_nssm.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Name = "label5";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // FirstService
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.box_windows_service_nssm);
            this.Controls.Add(this.txt_service_param);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_service_start_type);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_continue);
            this.Controls.Add(this.txt_windows_service_PhysicalPath);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.txt_service_description);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstService";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_service_description;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txt_windows_service_PhysicalPath;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label33;
        private AltoControls.AltoButton btn_continue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_service_start_type;
        private System.Windows.Forms.TextBox txt_service_param;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox box_windows_service_nssm;
        private System.Windows.Forms.Label label5;
    }
}