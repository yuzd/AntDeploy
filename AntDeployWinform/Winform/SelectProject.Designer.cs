namespace AntDeployWinform.Winform
{
    partial class SelectProject
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProject));
            this.btn_select_project = new System.Windows.Forms.Button();
            this.listBox_project = new System.Windows.Forms.ListBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_select_folder = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contextMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_select_project
            // 
            resources.ApplyResources(this.btn_select_project, "btn_select_project");
            this.btn_select_project.Name = "btn_select_project";
            this.btn_select_project.UseVisualStyleBackColor = true;
            this.btn_select_project.Click += new System.EventHandler(this.btn_select_project_Click);
            // 
            // listBox_project
            // 
            resources.ApplyResources(this.listBox_project, "listBox_project");
            this.listBox_project.FormattingEnabled = true;
            this.listBox_project.Name = "listBox_project";
            this.listBox_project.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_project_MouseDoubleClick);
            this.listBox_project.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox_project_MouseUp);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem,
            this.openLocationToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            resources.ApplyResources(this.selectToolStripMenuItem, "selectToolStripMenuItem");
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // openLocationToolStripMenuItem
            // 
            this.openLocationToolStripMenuItem.Name = "openLocationToolStripMenuItem";
            resources.ApplyResources(this.openLocationToolStripMenuItem, "openLocationToolStripMenuItem");
            this.openLocationToolStripMenuItem.Click += new System.EventHandler(this.openLocationToolStripMenuItem_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Name = "label1";
            // 
            // btn_select_folder
            // 
            resources.ApplyResources(this.btn_select_folder, "btn_select_folder");
            this.btn_select_folder.Name = "btn_select_folder";
            this.btn_select_folder.UseVisualStyleBackColor = true;
            this.btn_select_folder.Click += new System.EventHandler(this.btn_select_folder_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_select_project);
            this.panel1.Controls.Add(this.btn_select_folder);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // SelectProject
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox_project);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProject";
            this.ShowInTaskbar = false;
            this.contextMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_select_project;
        private System.Windows.Forms.ListBox listBox_project;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_select_folder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLocationToolStripMenuItem;
    }
}