using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AntDeployWinform.Winform
{
    public partial class SelectProject : Form
    {
        public SelectProject(List<string> projectList = null)
        {
            InitializeComponent();

            if (projectList != null)
            {
                foreach (var item in projectList)
                {
                    if (File.Exists(item))
                    {
                        var fileInfo = new FileInfo(item);
                        this.listBox_project.Items.Add(fileInfo.Name + "<==>" + item);
                    }

                }
            }

            SelectProjectPath = string.Empty;
        }

        public string SelectProjectPath { get; set; }

        private void btn_select_project_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Choose MSBuild.exe";
            fdlg.Filter = "(.csproj)|*.csproj|(.vsproj)|*.vsproj";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                if (!fdlg.FileName.ToLower().EndsWith(".csproj") && !fdlg.FileName.ToLower().EndsWith(".vbproj"))
                {
                    MessageBox.Show("Err:must be csproj file！");
                    return;
                }

                this.SelectProjectPath = fdlg.FileName;

                this.DialogResult = DialogResult.OK;

            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        private void listBox_project_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var selectItem = this.listBox_project.SelectedItem as string;
            if (string.IsNullOrEmpty(selectItem)) return;
            var file = selectItem.Split(new string[] { "<==>" }, StringSplitOptions.None);
            if (file.Length != 2) return;
            var path = file[1];
            if (File.Exists(path))
            {
                this.SelectProjectPath = path;
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
