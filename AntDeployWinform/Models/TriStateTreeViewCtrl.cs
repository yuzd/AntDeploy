/////////////////////////////////////////////////////////////////////////////////////
//Writer: Bo Xiao
//Email: blog.ealget@gmail.com
//Blog: eaglet.cnblogs.com
//Copyright: Eaglet workroom 2008
/////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TriStateTreeView
{
    public partial class TriStateTreeViewCtrl : TriStateTreeView
    {
        #region Protected methods
        override protected void OnCheckBoxStateChanged(CheckBoxStateChangedEventArgs args)
        {
            EventHandler<CheckBoxStateChangedEventArgs> handler = CheckBoxStateChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// The imagelist for node state
        /// </summary>
        [
        Browsable(true),
        CategoryAttribute("Behavior"),
        Description("The ImageList control from which node checkbox state images are token")
        ]
        override public ImageList CheckBoxStateImageList
        {
            set
            {
                _ImageListSent = false;
                _CtrlStateImageList = value;
            }

            get
            {
                if (_CtrlStateImageList == null)
                {
                    _CtrlStateImageList = ctlStateImageList;
                }
                return _CtrlStateImageList;
            }
        }
        #endregion

        #region Events
        [
        Browsable(true),
        CategoryAttribute("Property Changed"),
        Description("Occurs when the checkbox state of node changed")
        ]
        public event EventHandler<CheckBoxStateChangedEventArgs> CheckBoxStateChanged;
        #endregion

        public TriStateTreeViewCtrl()
        {
            InitializeComponent();
            this.CheckBoxes = true;
        }

        #region Public Methods

        public TreeNode AddNode(TreeNodeCollection nodes, string key, string text, string tag)
        {
            CheckBoxState checkboxState = CheckBoxState.Unchecked;
            TreeNode treeNode = nodes.Add(key, string.IsNullOrEmpty(text) ? key : text);
            treeNode.Tag = tag;
            return AddTreeNode(nodes, treeNode, checkboxState);
        }
        public TreeNode AddTreeNode(TreeNodeCollection nodes, string text, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, text, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, TreeNode node, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, node, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, key, text, imageIndex, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex, int selectedImageIndex, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, key, text, imageIndex, selectedImageIndex, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, key, text, imageKey, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey, string selectedImageKey, bool checkboxChecked)
        {
            CheckBoxState checkboxState = checkboxChecked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return AddTreeNode(nodes, key, text, imageKey, selectedImageKey, checkboxState);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string text)
        {
            return AddTreeNode(nodes, text, false);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, TreeNode node)
        {
            return AddTreeNode(nodes, node, false);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex)
        {
            return AddTreeNode(nodes, key, text, imageIndex, false);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex, int selectedImageIndex)
        {
            return AddTreeNode(nodes, key, text, imageIndex, selectedImageIndex, false);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey)
        {
            return AddTreeNode(nodes, key, text, imageKey, false);
        }

        public TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey, string selectedImageKey)
        {
            return AddTreeNode(nodes, key, text, imageKey, selectedImageKey, false);
        }

        /// <summary>
        /// Get Tree Node CheckBox State
        /// </summary>
        /// <param name="treeNode">node</param>
        /// <returns>CheckBox State</returns>
        override public CheckBoxState GetTreeNodeCheckBoxState(TreeNode treeNode)
        {
            return GetNodeState(treeNode);
        }

        /// <summary>
        /// Set Tree Node CheckBox State
        /// </summary>
        /// <param name="treeNode">node</param>
        /// <param name="checkboxChecked">CheckBox State</param>
        /// <returns>New CheckBox State</returns>
        public CheckBoxState SetTreeNodeCheckBoxChecked(TreeNode treeNode, bool checkboxChecked)
        {
            CheckBoxState checkboxState = GetTreeNodeCheckBoxState(treeNode);
            bool done = false;

            switch (checkboxState)
            {
                case CheckBoxState.Unchecked:
                    if (checkboxChecked)
                    {
                        done = true;
                    }
                    break;
                case CheckBoxState.Checked:
                case CheckBoxState.Indeterminate:
                    if (!checkboxChecked)
                    {
                        done = true;
                    }
                    break;
            }

            if (done)
            {
                ToggleTreeNodeState(treeNode);
            }

            return GetTreeNodeCheckBoxState(treeNode);
        }

        #endregion

    }
}
