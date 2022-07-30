using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AntDeployWinform.Models
{
    public class ExTreeView : TreeView
    {
        private const int WM_LBUTTONDBLCLK = 0x0203;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                var info = this.HitTest(PointToClient(Cursor.Position));
                if (info.Location == TreeViewHitTestLocations.StateImage)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
    public static class TreeExtensions
    {
        public static List<TreeNode> Descendants(this TreeView tree)
        {
            var nodes = tree.Nodes.Cast<TreeNode>();
            return nodes.SelectMany(x => x.Descendants()).Concat(nodes).ToList();
        }
        public static List<TreeNode> Descendants(this TreeNode node)
        {
            var nodes = node.Nodes.Cast<TreeNode>().ToList();
            return nodes.SelectMany(x => Descendants(x)).Concat(nodes).ToList();
        }
        public static List<TreeNode> Ancestors(this TreeNode node)
        {
            return AncestorsInternal(node).ToList();
        }
        private static IEnumerable<TreeNode> AncestorsInternal(TreeNode node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
                yield return node;
            }
        }
    }
}
