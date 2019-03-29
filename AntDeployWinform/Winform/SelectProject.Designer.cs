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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProject));
            this.btn_select_project = new System.Windows.Forms.Button();
            this.listBox_project = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
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
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Name = "label1";
            // 
            // SelectProject
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox_project);
            this.Controls.Add(this.btn_select_project);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProject";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_select_project;
        private System.Windows.Forms.ListBox listBox_project;
        private System.Windows.Forms.Label label1;
    }
}