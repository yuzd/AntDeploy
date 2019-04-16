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

        public SelectFile(string dir) : this()
        {
            LoadDirectory(dir);
        }

        public SelectFile(List<string> fileList,string dir) : this()
        {
            var list = fileList.Select(r => '/' + r).ToList();
            progressBar1.Maximum = list.Count;
            DirectoryInfo di = new DirectoryInfo(dir);

            parentNode = treeView1.AddNode(treeView1.Nodes, "/", di.Name, di.FullName);
            //获取变更的文件路径列表
            PopulateTreeView(this.treeView1, dir, list, parentNode);
        }
        private void PopulateTreeView(TreeView treeView,string projectPath, IEnumerable<string> paths, TreeNode lastNode)
        {
            string subPathAgg;
            foreach (var path in paths)
            {
                UpdateProgress();
                subPathAgg = string.Empty;
                foreach (string subPath in path.Split('/'))
                {
                    subPathAgg += subPath + '/';
                    TreeNode[] nodes = treeView.Nodes.Find(subPathAgg, true);
                    if (nodes.Length == 0)
                    {
                        if (lastNode == null)
                        {
                            lastNode = treeView1.AddNode(treeView.Nodes, subPathAgg, subPath,"");
                        }
                        else
                        {
                            lastNode = treeView1.AddNode(lastNode.Nodes, subPathAgg, subPath,"");
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
                        var fullPath = Path.Combine(projectPath, path2.Replace("/","\\"));
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
        private void LoadDirectory(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            //Setting ProgressBar Maximum Value
            progressBar1.Maximum = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(dir, "**", SearchOption.AllDirectories).Length;

            var gitPath = Path.Combine(dir, ".git");
            if (Directory.Exists(gitPath))
            {
                var gistFilecounts = Directory.GetFiles(gitPath, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(gitPath, "**", SearchOption.AllDirectories).Length;
                progressBar1.Maximum = progressBar1.Maximum - gistFilecounts - 1;
            }

            parentNode = treeView1.AddNode(treeView1.Nodes,di.Name,"",di.FullName);
            LoadFiles(dir, parentNode);
            LoadSubDirectories(dir, parentNode);


        }

        private void LoadSubDirectories(string dir, TreeNode td)
        {
            // Get all subdirectories
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (subdirectory.EndsWith(".git")) continue;
                DirectoryInfo di = new DirectoryInfo(subdirectory);
                TreeNode tds = treeView1.AddNode(td.Nodes,di.Name,"",di.FullName);
                LoadFiles(subdirectory, tds);
                LoadSubDirectories(subdirectory, tds);
                UpdateProgress();

            }
        }

        private void LoadFiles(string dir, TreeNode td)
        {
            string[] Files = Directory.GetFiles(dir, "*.*");

            // Loop through them to see files
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = treeView1.AddNode(td.Nodes,fi.Name,"",fi.FullName);
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

    }
}
