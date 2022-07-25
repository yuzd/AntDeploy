using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AntDeployWinform.Winform
{
    public partial class SelectFile : Form
    {

        public SelectFile()
        {
            InitializeComponent();
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            // Setting Inital Value of Progress Bar
            progressBar1.Value = 0;
            // Clear All Nodes if Already Exists
            treeView1.Nodes.Clear();
            //treeView1.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            treeView1.CheckBoxes = true;
            //treeView1.Enabled = false;

        }

        private string _dir = "";
        private List<string> _ignoreList = null;
        private List<string> _fileList = null;

        public SelectFile(string dir, List<string> ignoreList) : this()
        {
            this._dir = dir;
            this._ignoreList = ignoreList;
            //LoadDirectory(dir, ignoreList);
        }

        public SelectFile(List<string> fileList, string dir, List<string> ignoreList) : this()
        {
            this._fileList = fileList;
            this._dir = dir;
            this._ignoreList = ignoreList;
        }

        //显示
        private void SelectFile_Shown(object sender, EventArgs e)
        {
            if (this._fileList == null)
            {
                this.LoadDirectory(this._dir, this._ignoreList);
            }
            else
            {
                var list = this._fileList.Select(r => '/' + r).ToList();
                progressBar1.Maximum = list.Count;
                DirectoryInfo di = new DirectoryInfo(this._dir);

                parentNode = treeView1.AddNode(treeView1.Nodes, "/", di.Name, di.FullName);
                //获取变更的文件路径列表
                PopulateTreeView(this.treeView1, this._dir, list, parentNode, this._ignoreList);
            }
        }


        private void PopulateTreeView(TreeView treeView, string projectPath, IEnumerable<string> paths, TreeNode lastNode, List<string> ignoreList)
        {
            //重新排序
            List<Models.FileStruct> fileStructs = new List<Models.FileStruct>();
            if (paths != null && paths.Count() > 0)
            {
                foreach (string path in paths)
                {
                    string filePath = Path.Combine(projectPath, path.TrimStart('/').Replace("/", "\\"));
                    if (Util.ZipHelper.IsIgnore(path, ignoreList))
                    {
                        continue;
                    }
                    if (File.Exists(filePath))
                    {
                        FileInfo fi = new FileInfo(filePath);
                        fileStructs.Add(new Models.FileStruct
                        {
                            FileFullName = filePath,
                            RelativePath = path,
                            UpdateTime = fi.LastWriteTime,
                            IsFile = true
                        });
                    }
                    else
                    {
                        fileStructs.Add(new Models.FileStruct
                        {
                            FileFullName = filePath,
                            RelativePath = path,
                            IsFile = false
                        });
                    }
                }
            }
            Util.FileHelper.ResortFileList(fileStructs, projectPath);
            if (fileStructs.Count != paths.Count())
            {
                progressBar1.Maximum = fileStructs.Count;
            }

            string subPathAgg;
            foreach (var fileStruct in fileStructs)
            {
                UpdateProgress();
                subPathAgg = string.Empty;
                var subPaths = fileStruct.RelativePath.Split('/');
                foreach (string subPath in subPaths)
                {
                    subPathAgg += subPath + '/';
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                    {
                        string text = subPath;
                        if (fileStruct.UpdateTime > new DateTime(1970, 1, 1) && fileStruct.IsFile)
                        {
                            text = $"{subPath} ({fileStruct.UpdateTime:yyyy-MM-dd HH:mm})";
                        }
                        if (lastNode == null)
                        {
                            lastNode = treeView1.AddNode(treeView.Nodes, subPathAgg, text, "");
                        }
                        else
                        {
                            lastNode = treeView1.AddNode(lastNode.Nodes, subPathAgg, text, "");
                        }

                        var path2 = subPathAgg;
                        if (path2.StartsWith("/"))
                        {
                            path2 = path2.Substring(1);
                        }

                        if (path2.EndsWith("/"))
                        {
                            path2 = path2.Substring(0, path2.Length - 1);
                        }
                        var fullPath = Path.Combine(projectPath, path2.Replace("/", "\\"));
                        if (File.Exists(fullPath))
                        {
                            lastNode.Tag = fullPath;
                        }
                        else
                        {
                            lastNode.Tag = fullPath;
                        }
                    }
                    else
                    {
                        lastNode = nodes[0];
                    }
                }
            }
        }

        public List<string> SelectedFileList { get; set; }

        private void SelectFile_FormClosing(object sender, FormClosingEventArgs e)
        {
            SelectedFileList = GetSelectFiles();
        }

        private List<string> GetSelectFiles()
        {
            var fileList = new List<string>();
            GetCheckedNodes(treeView1.Nodes, fileList);
            return fileList;
        }

        private TreeNode parentNode;
        private void LoadDirectory(string dir, List<string> ignoreList)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            //Setting ProgressBar Maximum Value
            //progressBar1.Maximum = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(dir, "**", SearchOption.AllDirectories).Length;
            List<Models.FileStruct> fileStructs = new List<Models.FileStruct>();
            Util.FileHelper.GetAllFileInfos(fileStructs, dir, ignoreList);
            progressBar1.Maximum = fileStructs.Count;

            var gitPath = Path.Combine(dir, ".git");
            if (Directory.Exists(gitPath))
            {
                //var gistFilecounts = Directory.GetFiles(gitPath, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(gitPath, "**", SearchOption.AllDirectories).Length;
                var gistFilecounts = fileStructs.Count(u => u.FileFullName.StartsWith($"{gitPath}\\", StringComparison.OrdinalIgnoreCase));
                progressBar1.Maximum = progressBar1.Maximum - gistFilecounts - 1;
            }

            parentNode = treeView1.AddNode(treeView1.Nodes, di.Name, "", di.FullName);
            Util.FileHelper.ResortFileList(fileStructs, dir);
            LoadFiles(dir, parentNode, fileStructs);
            LoadSubDirectories(dir, parentNode, fileStructs);
        }

        private void LoadSubDirectories(string dir, TreeNode td, List<Models.FileStruct> fileStructs)
        {
            // Get all subdirectories
            //string[] subdirectoryEntries = Directory.GetDirectories(dir);
            var subdirectoryEntries = fileStructs.Where(u => !u.IsFile && String.Equals(dir, Path.GetDirectoryName(u.FileFullName), StringComparison.OrdinalIgnoreCase)).ToList();
            //Util.FileHelper.ResortFileList(subdirectoryEntries, dir);
            // Loop through them to see if they have any other subdirectories
            foreach (var subdirectory in subdirectoryEntries)
            {
                if (subdirectory.FileFullName.EndsWith(".git")) continue;
                DirectoryInfo di = new DirectoryInfo(subdirectory.FileFullName);
                TreeNode tds = treeView1.AddNode(td.Nodes, di.Name, "", di.FullName);
                LoadFiles(subdirectory.FileFullName, tds, fileStructs);
                LoadSubDirectories(subdirectory.FileFullName, tds, fileStructs);
                UpdateProgress();
            }
        }

        private void LoadFiles(string dir, TreeNode td, List<Models.FileStruct> fileStructs)
        {
            //string[] Files = Directory.GetFiles(dir, "*.*");
            var files = fileStructs.Where(u => u.IsFile && String.Equals(dir, Path.GetDirectoryName(u.FileFullName), StringComparison.OrdinalIgnoreCase)).ToList();
            //Util.FileHelper.ResortFileList(fileStructs, dir);

            // Loop through them to see files
            foreach (var file in files)
            {
                //FileInfo fi = new FileInfo(file);
                //TreeNode tds = treeView1.AddNode(td.Nodes,fi.Name,"",fi.FullName);
                string fileName = Path.GetFileName(file.FileFullName);
                string text = $"{fileName} ({file.UpdateTime:yyyy-MM-dd HH:mm})";
                TreeNode tds = treeView1.AddNode(td.Nodes, fileName, text, file.FileFullName);
                UpdateProgress();

            }
        }

        private void UpdateProgress()
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
                //int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                //progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));

                Application.DoEvents();
                if (progressBar1.Value >= progressBar1.Maximum)
                {
                    var thread = new System.Threading.Thread(p =>
                    {
                        lock (progressBar1)
                        {
                            Action action = () =>
                            {
                                treeView1.Enabled = true;
                                progressBar1.Visible = false;
                                parentNode?.Expand();
                            };
                            System.Threading.Thread.Sleep(1000);
                            this.Invoke(action);

                        }
                    });
                    thread.Start();
                }
            }
        }
        private void GetCheckedNodes(TreeNodeCollection nodes, List<string> fileList)
        {
            foreach (System.Windows.Forms.TreeNode aNode in nodes)
            {
                TriStateTreeView.CheckBoxState state = treeView1.GetTreeNodeCheckBoxState(aNode);
                //edit
                if (state == TriStateTreeView.CheckBoxState.Checked && aNode.Nodes.Count == 0)
                    fileList.Add(aNode.Tag as string);

                if (aNode.Nodes.Count != 0)
                    GetCheckedNodes(aNode.Nodes, fileList);
            }
        }

        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref TVITEM lParam);

        /// <summary>
        /// Hides the checkbox for the specified node on a TreeView control.
        /// </summary>
        private void HideCheckBox(TreeView tvw, TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
        }



        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.ImageIndex == 0)
                HideCheckBox(treeView1, e.Node);
            e.DrawDefault = true;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //通过鼠标或者键盘触发事件，防止修改节点的Checked状态时候再次进入
            if (e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.ByKeyboard)
            {
                bool state = e.Node.Checked;
                //this.SetParentNodeCheckedState(e.Node, state);
                //this.SetChildNodeCheckedState(e.Node, state);
                this.treeView1.SetTreeNodeCheckBoxChecked(e.Node, state);
            }
        }

        /// <summary>
        /// 取消节点选中状态之后，取消所有父节点的选中状态
        /// </summary>
        /// <param name="currNode"></param>
        /// <param name="state"></param>
        private void SetParentNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNode parentNode = currNode.Parent;
            if (parentNode != null)
            {

                if (state)
                {
                    bool allChecked = true;
                    foreach (TreeNode brother in parentNode.Nodes)
                    {
                        if (!brother.Checked)
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    if (allChecked)
                    {
                        parentNode.Checked = state;
                        this.SetParentNodeCheckedState(parentNode, state);
                    }
                }
                else
                {
                    parentNode.Checked = state;
                    this.SetParentNodeCheckedState(parentNode, state);
                }
            }
        }

        /// <summary>
        /// 选中节点之后，选中节点的所有子节点
        /// </summary>
        /// <param name="currNode"></param>
        /// <param name="state"></param>
        private void SetChildNodeCheckedState(TreeNode currNode, bool state)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count > 0)
            {
                foreach (TreeNode tn in nodes)
                {
                    tn.Checked = state;
                    this.SetChildNodeCheckedState(tn, state);
                }
            }
        }

    }
}
