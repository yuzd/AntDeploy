namespace AntDeployWinform.Winform
{
    partial class RollBack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RollBack));
            this.listbox_rollback_list = new System.Windows.Forms.ListBox();
            this.b_rollback_Rollback = new AltoControls.AltoButton();
            this.listView_rollback_version = new System.Windows.Forms.ListView();
            this.version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.remark = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mac = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label_server_name = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listbox_rollback_list
            // 
            this.listbox_rollback_list.FormattingEnabled = true;
            resources.ApplyResources(this.listbox_rollback_list, "listbox_rollback_list");
            this.listbox_rollback_list.Name = "listbox_rollback_list";
            // 
            // b_rollback_Rollback
            // 
            this.b_rollback_Rollback.Active1 = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(168)))), ((int)(((byte)(183)))));
            this.b_rollback_Rollback.Active2 = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(164)))), ((int)(((byte)(183)))));
            this.b_rollback_Rollback.BackColor = System.Drawing.Color.Transparent;
            this.b_rollback_Rollback.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.b_rollback_Rollback, "b_rollback_Rollback");
            this.b_rollback_Rollback.ForeColor = System.Drawing.Color.Black;
            this.b_rollback_Rollback.Inactive1 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.b_rollback_Rollback.Inactive2 = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(188)))), ((int)(((byte)(210)))));
            this.b_rollback_Rollback.Name = "b_rollback_Rollback";
            this.b_rollback_Rollback.Radius = 10;
            this.b_rollback_Rollback.Stroke = false;
            this.b_rollback_Rollback.StrokeColor = System.Drawing.Color.Gray;
            this.b_rollback_Rollback.Transparency = false;
            this.b_rollback_Rollback.Click += new System.EventHandler(this.b_rollback_Rollback_Click);
            // 
            // listView_rollback_version
            // 
            this.listView_rollback_version.CheckBoxes = true;
            this.listView_rollback_version.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.version,
            this.date,
            this.remark,
            this.pc,
            this.mac,
            this.ip});
            this.listView_rollback_version.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listView_rollback_version.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            resources.ApplyResources(this.listView_rollback_version, "listView_rollback_version");
            this.listView_rollback_version.MultiSelect = false;
            this.listView_rollback_version.Name = "listView_rollback_version";
            this.listView_rollback_version.ShowItemToolTips = true;
            this.listView_rollback_version.UseCompatibleStateImageBehavior = false;
            this.listView_rollback_version.View = System.Windows.Forms.View.Details;
            this.listView_rollback_version.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            // 
            // version
            // 
            resources.ApplyResources(this.version, "version");
            // 
            // date
            // 
            resources.ApplyResources(this.date, "date");
            // 
            // remark
            // 
            resources.ApplyResources(this.remark, "remark");
            // 
            // pc
            // 
            resources.ApplyResources(this.pc, "pc");
            // 
            // mac
            // 
            resources.ApplyResources(this.mac, "mac");
            // 
            // ip
            // 
            resources.ApplyResources(this.ip, "ip");
            // 
            // label_server_name
            // 
            resources.ApplyResources(this.label_server_name, "label_server_name");
            this.label_server_name.ForeColor = System.Drawing.Color.Blue;
            this.label_server_name.Name = "label_server_name";
            // 
            // RollBack
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label_server_name);
            this.Controls.Add(this.listView_rollback_version);
            this.Controls.Add(this.b_rollback_Rollback);
            this.Controls.Add(this.listbox_rollback_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RollBack";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RollBack_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listbox_rollback_list;
        private AltoControls.AltoButton b_rollback_Rollback;
        private System.Windows.Forms.ListView listView_rollback_version;
        private System.Windows.Forms.ColumnHeader version;
        private System.Windows.Forms.ColumnHeader remark;
        private System.Windows.Forms.Label label_server_name;
        private System.Windows.Forms.ColumnHeader pc;
        private System.Windows.Forms.ColumnHeader mac;
        private System.Windows.Forms.ColumnHeader ip;
        private System.Windows.Forms.ColumnHeader date;
    }
}