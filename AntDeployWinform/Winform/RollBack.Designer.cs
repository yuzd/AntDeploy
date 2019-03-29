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
            this.listbox_rollback_list = new System.Windows.Forms.ListBox();
            this.b_rollback_Rollback = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listbox_rollback_list
            // 
            this.listbox_rollback_list.Dock = System.Windows.Forms.DockStyle.Top;
            this.listbox_rollback_list.FormattingEnabled = true;
            this.listbox_rollback_list.ItemHeight = 12;
            this.listbox_rollback_list.Items.AddRange(new object[] {
            "1111",
            "222",
            "333",
            "2222"});
            this.listbox_rollback_list.Location = new System.Drawing.Point(0, 0);
            this.listbox_rollback_list.Name = "listbox_rollback_list";
            this.listbox_rollback_list.Size = new System.Drawing.Size(217, 232);
            this.listbox_rollback_list.TabIndex = 0;
            // 
            // b_rollback_Rollback
            // 
            this.b_rollback_Rollback.Location = new System.Drawing.Point(36, 238);
            this.b_rollback_Rollback.Name = "b_rollback_Rollback";
            this.b_rollback_Rollback.Size = new System.Drawing.Size(148, 43);
            this.b_rollback_Rollback.TabIndex = 2;
            this.b_rollback_Rollback.Text = "Do RollBack";
            this.b_rollback_Rollback.UseVisualStyleBackColor = true;
            this.b_rollback_Rollback.Click += new System.EventHandler(this.b_rollback_Rollback_Click);
            // 
            // RollBack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(217, 283);
            this.Controls.Add(this.b_rollback_Rollback);
            this.Controls.Add(this.listbox_rollback_list);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RollBack";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select RollBack Version";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RollBack_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listbox_rollback_list;
        private System.Windows.Forms.Button b_rollback_Rollback;
    }
}