using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AntDeployWinform.Models;

namespace AntDeployWinform.Winform
{
    public partial class SelectProject : Form
    {
        private string _lang  = string.Empty;
        public SelectProject(List<string> projectList = null)
        {
            InitializeComponent();
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            if (projectList != null)
            {
                foreach (var item in projectList)
                {
                    if (File.Exists(item))
                    {
                        var fileInfo = new FileInfo(item);
                        this.listBox_project.Items.Add(fileInfo.Name + "<==>" + item);
                    }

                    if (Directory.Exists(item))
                    {
                        this.listBox_project.Items.Add("[Folder Deploy]<==>" + item);
                    }

                }
            }

            SelectProjectPath = string.Empty;

            _lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        }

        public string SelectProjectPath { get; set; }

        private void btn_select_folder_Click(object sender, EventArgs e)
        {
            using (var fsd = new FolderSelectDialog())
            {
                fsd.Title = !string.IsNullOrEmpty(_lang) && _lang.StartsWith("zh-")? "选择指定的文件夹发布" : "Select Folder To Deploy";
                if (fsd.ShowDialog(this.Handle))
                {
                    var folder = fsd.FileName;
                    if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
                    {
                        this.SelectProjectPath = folder;
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
                
            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();

            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        var folder = fbd.SelectedPath;
            //        this.SelectProjectPath = folder;
            //        this.DialogResult = DialogResult.OK;
            //    }
            //}
        }
        private void btn_select_project_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = !string.IsNullOrEmpty(_lang) && _lang.StartsWith("zh-") ? "选择指定的项目发布" : "Choose Project";
            fdlg.Filter = "(.csproj)|*.csproj|(.vsproj)|*.vsproj";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                if (!fdlg.FileName.ToLower().EndsWith(".csproj") && !fdlg.FileName.ToLower().EndsWith(".vbproj"))
                {
                    MessageBoxEx.Show(this,"Err:must be csproj file！");
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

            if (Directory.Exists(path))
            {
                this.SelectProjectPath = path;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox_project_MouseDoubleClick(null, null);
        }

        private void openLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectItem = this.listBox_project.SelectedItem as string;
            if (string.IsNullOrEmpty(selectItem)) return;
            var file = selectItem.Split(new string[] { "<==>" }, StringSplitOptions.None);
            if (file.Length != 2) return;
            var path = file[1];
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                ProcessStartInfo sInfo = new ProcessStartInfo(fileInfo.DirectoryName);
                Process.Start(sInfo);
            }

            if (Directory.Exists(path))
            {
                ProcessStartInfo sInfo = new ProcessStartInfo(path);
                Process.Start(sInfo);
            }
        }

        private void listBox_project_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int posindex = listBox_project.IndexFromPoint(new Point(e.X, e.Y));
                listBox_project.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < listBox_project.Items.Count)
                {
                    listBox_project.SelectedIndex = posindex;
                    contextMenuStrip.Show(listBox_project, new Point(e.X, e.Y));
                }
            }
            listBox_project.Refresh();
        }
    }
}
