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
using System.Runtime.InteropServices;

namespace TriStateTreeView
{
    /// <summary>
    /// CheckBox State
    /// </summary>
    public enum CheckBoxState
    {
        None = 0,
        Unchecked = 1,
        Checked = 2,
        Indeterminate = 3
    }

    public class CheckBoxStateChangedEventArgs : EventArgs
    {
        private CheckBoxState _State;
        private TreeNode _Node;

        public CheckBoxState State
        {
            get
            {
                return _State;
            }
        }

        public TreeNode Node
        {
            get
            {
                return _Node;
            }
        }

        public CheckBoxStateChangedEventArgs(CheckBoxState state, TreeNode node)
        {
            _State = state;
            _Node = node;
        }
    }

    abstract public class TriStateTreeView : System.Windows.Forms.TreeView
    {
        #region WIN32
        public const UInt32 TV_FIRST = 4352;
        public const UInt32 TVSIL_NORMAL = 0;
        public const UInt32 TVSIL_STATE = 2;
        public const UInt32 TVM_SETIMAGELIST = TV_FIRST + 9;
        public const UInt32 TVM_GETNEXTITEM = TV_FIRST + 10;
        public const UInt32 TVIF_HANDLE = 16;
        public const UInt32 TVIF_STATE = 8;
        public const UInt32 TVIS_STATEIMAGEMASK = 61440;

        public const UInt32 TVM_GETITEM = TV_FIRST + 12;
        public const UInt32 TVM_SETITEM = TV_FIRST + 13;
        private const UInt32 TVM_HITTEST = TV_FIRST + 17;

        public const UInt32 TVGN_ROOT = 0;

        //TVHITTESTINFO.flags flags
        public const int TVHT_ONITEMSTATEICON = 64;


        // Use a sequential structure layout to define TVITEM for the TreeView.
        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public uint mask;
            public IntPtr hItem;
            public uint state;
            public uint stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVHITTESTINFO
        {
            public POINTAPI pt;
            public int flags;
            public IntPtr hItem;
        }

        // Declare two overloaded SendMessage functions. The
        // difference is in the last parameter: one is ByVal and the
        // other is ByRef.
        [DllImport("user32.dll")]
        private static extern UInt32 SendMessage(IntPtr hWnd, UInt32 Msg,
            UInt32 wParam, UInt32 lParam);

        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg,
            UInt32 wParam, ref TVITEM lParam);

        [DllImport("User32", CharSet = CharSet.Auto)]
        private static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg,
            UInt32 wParam, ref TVHITTESTINFO lParam);
        #endregion

        #region Private fields
        private Dictionary<TreeNode, CheckBoxState> _NodeStateDict = new Dictionary<TreeNode, CheckBoxState>();
        #endregion 

        #region Protected fields
        protected ImageList _CtrlStateImageList;
        protected bool _ImageListSent = false;
        #endregion

        #region Public abstract members
        abstract public ImageList CheckBoxStateImageList{get; set;}
        abstract public CheckBoxState GetTreeNodeCheckBoxState(TreeNode treeNode);
        abstract protected void OnCheckBoxStateChanged(CheckBoxStateChangedEventArgs args);
        #endregion

        #region Private Methods

        /// <summary>
        /// Set tree node and his children checkbox state recursively
        /// </summary>
        /// <param name="treeNode">tree node</param>
        /// <param name="checkboxState">checkbox state</param>
        private void SetTreeNodeAndChildrenStateRecursively(TreeNode treeNode, CheckBoxState checkboxState)
        {
            if ((treeNode != null))
            {
                SetTreeNodeState(treeNode, checkboxState);

                foreach (TreeNode objChildTreeNode in treeNode.Nodes)
                {
                    SetTreeNodeAndChildrenStateRecursively(objChildTreeNode, checkboxState);
                }
            }
        }

        /// <summary>
        /// Set parent treenode checkbox state recursively
        /// </summary>
        /// <param name="parentTreeNode"></param>
        private void SetParentTreeNodeStateRecursively(TreeNode parentTreeNode)
        {
            CheckBoxState checkboxState;
            bool bAllChildrenChecked = true;
            bool bAllChildrenUnchecked = true;

            if ((parentTreeNode != null))
            {
                if (GetTreeNodeCheckBoxState(parentTreeNode) != CheckBoxState.None)
                {
                    foreach (TreeNode treeNode in parentTreeNode.Nodes)
                    {
                        checkboxState = GetTreeNodeCheckBoxState(treeNode);

                        switch (checkboxState)
                        {
                            case CheckBoxState.Checked:
                                bAllChildrenUnchecked = false;
                                break;
                            case CheckBoxState.Indeterminate:
                                bAllChildrenUnchecked = false;
                                bAllChildrenChecked = false;
                                break;
                            case CheckBoxState.Unchecked:
                                bAllChildrenChecked = false;
                                break;
                        }

                        if (bAllChildrenChecked == false & bAllChildrenUnchecked == false)
                        {
                            // This is an optimization
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }

                    if (bAllChildrenChecked)
                    {
                        SetTreeNodeState(parentTreeNode, CheckBoxState.Checked);
                    }
                    else if (bAllChildrenUnchecked)
                    {
                        SetTreeNodeState(parentTreeNode, CheckBoxState.Unchecked);
                    }
                    else
                    {
                        SetTreeNodeState(parentTreeNode, CheckBoxState.Indeterminate);
                    }

                    // Enter in recursion
                    if ((parentTreeNode.Parent != null))
                    {
                        SetParentTreeNodeStateRecursively(parentTreeNode.Parent);
                    }
                }
            }
        }

        /// <summary>
        /// Get treenode by screen position
        /// </summary>
        /// <param name="x">x position in screen</param>
        /// <param name="y">y position in screen</param>
        /// <returns></returns>
        private TreeNode GetTreeNodeHitAtCheckBoxByScreenPosition(int x, int y)
        {
            Point clientPoint;
            TreeNode treeNode;

            clientPoint = this.PointToClient(new Point(x, y));

            treeNode = GetTreeNodeHitAtCheckBoxByClientPosition(clientPoint.X, clientPoint.Y);

            return treeNode;
        }

        /// <summary>
        /// Get treenode by client position
        /// </summary>
        /// <param name="x">x position in client</param>
        /// <param name="y">y position in client</param>
        /// <returns></returns>
        private TreeNode GetTreeNodeHitAtCheckBoxByClientPosition(int x, int y)
        {
            TreeNode treeNode = null;
            UInt32 iTreeNodeHandle;
            TVHITTESTINFO tTVHITTESTINFO = new TVHITTESTINFO();

            // Get the hit info
            tTVHITTESTINFO.pt.x = x;
            tTVHITTESTINFO.pt.y = y;
            iTreeNodeHandle = SendMessage(this.Handle, TVM_HITTEST, 0, ref tTVHITTESTINFO);

            // Check if it has clicked on an item
            if (iTreeNodeHandle != 0)
            {
                // Check if it has clicked on the state image of the item
                if ((tTVHITTESTINFO.flags & TVHT_ONITEMSTATEICON) != 0)
                {
                    treeNode = TreeNode.FromHandle(this, new IntPtr(iTreeNodeHandle));
                }
            }

            return treeNode;
        }

        /// <summary>
        /// Set tree node checkbox state
        /// </summary>
        /// <param name="treeNode">tree node</param>
        /// <param name="checkboxState">checkbox state</param>
        private void SetTreeNodeState(TreeNode treeNode, CheckBoxState checkboxState)
        {
            IntPtr hWnd;
            TVITEM tvi;
            hWnd = this.Handle;

            // Send a TVM_SETIMAGELIST with TVSIL_STATE.
            if (!_ImageListSent)
            {
                SendMessage(hWnd, (UInt32)TVM_SETIMAGELIST, (UInt32)TVSIL_STATE, (UInt32)CheckBoxStateImageList.Handle);
                _ImageListSent = true;
            }

            // The following uses the TVM_SETITEM message to set the State 
            // of a given item. It uses the TVITEM structure.

            //  tvi.mask: include TVIF_HANDLE and TVIF_STATE
            tvi.mask = TVIF_HANDLE | TVIF_STATE;

            // To use the State image, tvi.State cannot be 0.  
            //Setting it to 1 means to use the second image in the image list.
            tvi.state = (uint)checkboxState;
            // Left shift 12 to put info in bits 12 to 15
            tvi.state = tvi.state << 12;
            // Set StateMask. -This is required to isolate State above.
            tvi.stateMask = TVIS_STATEIMAGEMASK;

            // Define the item we want to set the State in.
            tvi.hItem = treeNode.Handle;  //For example, try the root.

            //  Initialize the rest to zero.
            tvi.pszText = (IntPtr)0;
            tvi.cchTextMax = 0;
            tvi.iImage = 0;
            tvi.iSelectedImage = 0;
            tvi.cChildren = 0;
            tvi.lParam = (IntPtr)0;

            // Send the TVM_SETITEM message.
            //  TVM_SETITEM = 4365
            SendMessage(hWnd, (UInt32)TVM_SETITEM, (UInt32)0, ref tvi);

            //Set Node State
            SetNodeState(treeNode, checkboxState);
        }

        #endregion

        #region Override methods
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            TreeNode treeNode;

            base.OnMouseUp(e);

            treeNode = GetTreeNodeHitAtCheckBoxByClientPosition(e.X, e.Y);
            if ((treeNode != null))
            {
                ToggleTreeNodeState(treeNode);
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.Space)
            {
                if ((this.SelectedNode != null))
                {
                    ToggleTreeNodeState(this.SelectedNode);
                }
            }
        }

        protected override void OnBeforeExpand(System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            // PATCH: if the node is being expanded by a double click at the state image, cancel it
            if ((GetTreeNodeHitAtCheckBoxByScreenPosition(Control.MousePosition.X, Control.MousePosition.Y) != null))
            {
                e.Cancel = true;
            }

        }

        protected override void OnBeforeCollapse(System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            // PATCH: if the node is being collapsed by a double click at the state image, cancel it
            if ((GetTreeNodeHitAtCheckBoxByScreenPosition(Control.MousePosition.X, Control.MousePosition.Y) != null))
            {
                e.Cancel = true;
            }

        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Set treenode checkbox state 
        /// </summary>
        /// <param name="treeNode">treenode</param>
        /// <param name="state">checkbox state</param>
        protected void SetNodeState(TreeNode treeNode, CheckBoxState state)
        {
            if (!_NodeStateDict.ContainsKey(treeNode))
            {
                _NodeStateDict.Add(treeNode, state);
            }
            else
            {
                _NodeStateDict[treeNode] = state;
            }

            OnCheckBoxStateChanged(new CheckBoxStateChangedEventArgs(state, treeNode));
        }

        /// <summary>
        /// Get treenode checkbox state 
        /// </summary>
        /// <param name="treeNode">treenode</param>
        /// <returns>checkbox state</returns>
        protected CheckBoxState GetNodeState(TreeNode treeNode)
        {
            return _NodeStateDict[treeNode];
        }

        /// <summary>
        /// Toggle treenode state
        /// </summary>
        /// <param name="treeNode">tree node</param>
        protected void ToggleTreeNodeState(TreeNode treeNode)
        {
            CheckBoxState checkboxState = GetTreeNodeCheckBoxState(treeNode);

            this.BeginUpdate();

            switch (checkboxState)
            {
                case CheckBoxState.Unchecked:
                    SetTreeNodeAndChildrenStateRecursively(treeNode, CheckBoxState.Checked);
                    SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
                case CheckBoxState.Checked:
                case CheckBoxState.Indeterminate:
                    SetTreeNodeAndChildrenStateRecursively(treeNode, CheckBoxState.Unchecked);
                    SetParentTreeNodeStateRecursively(treeNode.Parent);
                    break;
            }

            this.EndUpdate();
        }

        /// <summary>
        /// Add TreeNode
        /// </summary>
        /// <param name="nodes">node collection</param>
        /// <param name="text">text</param>
        /// <param name="iImageIndex">image index</param>
        /// <param name="checkboxState">check box state</param>
        /// <returns></returns>
        protected TreeNode AddTreeNode(TreeNodeCollection nodes, string text, CheckBoxState checkboxState)
        {
            TreeNode treeNode = nodes.Add(text);

            return AddTreeNode(nodes, treeNode, checkboxState);
        }

        /// <summary>
        /// Add TreeNode
        /// </summary>
        /// <param name="nodes">node collection</param>
        /// <param name="treeNode">TreeNode</param>
        /// <param name="iImageIndex">image index</param>
        /// <param name="checkboxState">check box state</param>
        /// <returns></returns>
        protected TreeNode AddTreeNode(TreeNodeCollection nodes, TreeNode treeNode, CheckBoxState checkboxState)
        {
            if (!nodes.Contains(treeNode))
            {
                nodes.Add(treeNode);
            }

            this.SetTreeNodeState(treeNode, checkboxState);

            SetTreeNodeAndChildrenStateRecursively(treeNode, checkboxState);
            SetParentTreeNodeStateRecursively(treeNode.Parent);

            return treeNode;
        }

        protected TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex, CheckBoxState checkboxState)
        {
            TreeNode treeNode = nodes.Add(key, text, imageIndex);

            this.SetTreeNodeState(treeNode, checkboxState);
            SetTreeNodeAndChildrenStateRecursively(treeNode, checkboxState);
            SetParentTreeNodeStateRecursively(treeNode.Parent);

            return treeNode;
        }

        protected TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, int imageIndex, int selectedImageIndex, CheckBoxState checkboxState)
        {
            TreeNode treeNode = nodes.Add(key, text, imageIndex, selectedImageIndex);

            this.SetTreeNodeState(treeNode, checkboxState);
            SetTreeNodeAndChildrenStateRecursively(treeNode, checkboxState);
            SetParentTreeNodeStateRecursively(treeNode.Parent);

            return treeNode;
        }

        protected TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey, CheckBoxState checkboxState)
        {
            TreeNode treeNode = nodes.Add(key, text, imageKey);

            this.SetTreeNodeState(treeNode, checkboxState);
            SetTreeNodeAndChildrenStateRecursively(treeNode, checkboxState);
            SetParentTreeNodeStateRecursively(treeNode.Parent);

            return treeNode;
        }

        protected TreeNode AddTreeNode(TreeNodeCollection nodes, string key, string text, string imageKey, string selectedImageKey, CheckBoxState checkboxState)
        {
            TreeNode treeNode = nodes.Add(key, text, imageKey, selectedImageKey);

            this.SetTreeNodeState(treeNode, checkboxState);
            SetTreeNodeAndChildrenStateRecursively(treeNode, checkboxState);
            SetParentTreeNodeStateRecursively(treeNode.Parent);

            return treeNode;
        }

        #endregion

    }
}