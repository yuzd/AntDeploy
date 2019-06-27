namespace AntDeployWinform.Winform
{
    partial class FirstCreate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstCreate));
            this.label33 = new System.Windows.Forms.Label();
            this.txt_iis_port = new System.Windows.Forms.TextBox();
            this.txt_iis_PhysicalPath = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_pool_name = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_continue = new AltoControls.AltoButton();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // txt_iis_port
            // 
            resources.ApplyResources(this.txt_iis_port, "txt_iis_port");
            this.txt_iis_port.Name = "txt_iis_port";
            // 
            // txt_iis_PhysicalPath
            // 
            resources.ApplyResources(this.txt_iis_PhysicalPath, "txt_iis_PhysicalPath");
            this.txt_iis_PhysicalPath.Name = "txt_iis_PhysicalPath";
            // 
            // label30
            // 
            resources.ApplyResources(this.label30, "label30");
            this.label30.Name = "label30";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // txt_pool_name
            // 
            resources.ApplyResources(this.txt_pool_name, "txt_pool_name");
            this.txt_pool_name.Name = "txt_pool_name";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btn_continue
            // 
            this.btn_continue.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.btn_continue.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.btn_continue.BackColor = System.Drawing.Color.Transparent;
            this.btn_continue.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btn_continue, "btn_continue");
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Name = "label5";
            // 
            // FirstCreate
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_iis_port);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_continue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_iis_PhysicalPath);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txt_pool_name);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstCreate";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox txt_iis_port;
        private System.Windows.Forms.TextBox txt_iis_PhysicalPath;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_pool_name;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private AltoControls.AltoButton btn_continue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}