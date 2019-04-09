using AntDeployWinform.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AntDeployWinform.Winform
{
    public partial class RollBack : Form
    {
        private readonly bool isRollBack = false;

        public RollBack()
        {
            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }


            this.listbox_rollback_list.Items.Clear();
            this.listView_rollback_version.Items.Clear();
        }
        public RollBack(List<Tuple<string,string>> list, bool isNotRollback = false):this()
        {

            foreach (var li in list)
            {
                if (!isNotRollback)
                {
                    var version = li.Item1;
                    var remark = li.Item2;
                    if (string.IsNullOrEmpty(version)) continue;
                    ListViewItem lv = new ListViewItem();
                    lv.Text = version;
                    lv.SubItems.Add(remark);
                    this.listView_rollback_version.Items.Add(lv);
                }
                else
                {
                    this.listbox_rollback_list.Items.Add(li);
                }
            }


            isRollBack = !isNotRollback;
            this.listView_rollback_version.Visible = !isNotRollback;
            this.listbox_rollback_list.Visible = isNotRollback;

            SelectRollBackVersion = string.Empty;
        }

        public RollBack(List<string> list, bool isNotRollback = false) : this()
        {
            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }


            this.listbox_rollback_list.Items.Clear();
            this.listView_rollback_version.Items.Clear();
            
            foreach (var li in list)
            {
                if (!isNotRollback)
                {
                    var version = string.Empty;
                    var remark = string.Empty;
                    var content = li.JsonToObject<RollBackVersion>();
                    if (content == null)
                    {
                        version = li;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(content.Version))
                        {
                            version = content.Version;
                        }
                        if (content.FormItemList != null && content.FormItemList.Any())
                        {
                            var remarkItem = content.FormItemList.FirstOrDefault(r => r.FieldName.Equals("remark"));
                            if (remarkItem != null && !string.IsNullOrEmpty(remarkItem.TextValue))
                            {
                                remark = remarkItem.TextValue;
                            }
                        }
                    }

                    if(string.IsNullOrEmpty(version)) continue;
                    ListViewItem lv = new ListViewItem();
                    lv.Text = version;
                    lv.SubItems.Add(remark);
                    this.listView_rollback_version.Items.Add(lv);
                }
                else
                {
                    this.listbox_rollback_list.Items.Add(li);
                }
            }


            isRollBack = !isNotRollback;
            this.listView_rollback_version.Visible = !isNotRollback;
            this.listbox_rollback_list.Visible = isNotRollback;

            SelectRollBackVersion = string.Empty;
        }


        public string SelectRollBackVersion { get; set; }

        private void RollBack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectRollBackVersion))
            {
                this.DialogResult = DialogResult.Cancel;
            }

        }

        private void b_rollback_Rollback_Click(object sender, EventArgs e)
        {
            if (isRollBack)
            {
                if (lastItemChecked == null)
                {
                    MessageBox.Show("please select rollback version!");
                    return;
                }

                //var selectView = this.listView_rollback_version.SelectedItems[0];
                SelectRollBackVersion = lastItemChecked.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                var selectItem = this.listbox_rollback_list.SelectedItem as string;
                if (string.IsNullOrEmpty(selectItem))
                {
                    MessageBox.Show("please select one!");
                    return;
                }

                SelectRollBackVersion = selectItem;
                this.DialogResult = DialogResult.OK;
            }
           
        }

        public void SetButtonName(string name)
        {
            this.b_rollback_Rollback.Text = name;
        }

        // I need to know the last item checked
        private ListViewItem lastItemChecked;

        private void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if we have the lastItem set as checked, and it is different
            // item than the one that fired the event, uncheck it
            if (lastItemChecked != null && lastItemChecked.Checked
                                        && lastItemChecked != listView_rollback_version.Items[e.Index])
            {
                // uncheck the last item and store the new one
                lastItemChecked.Checked = false;
            }

            // store current item
            lastItemChecked = listView_rollback_version.Items[e.Index];
        }
    }

    class RollBackVersion
    {

        private string _args;
        public string Version { get; set; }

        public string Args
        {
            get { return this._args; }
            set
            {
                _args = value;
                if (!string.IsNullOrEmpty(value)) this.FormItemList = value.JsonToObject<List<FormItem>>();
            }
        }

        public List<FormItem> FormItemList { get; set; } = new List<FormItem>();
    }

    /// <summary>
    /// 表单元素对象
    /// </summary>
    class FormItem
    {
        /// <summary>
        /// 字段名(表单域名称)
        /// </summary>
        public string FieldName;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName;



        /// <summary>
        /// 文本内容
        /// </summary>
        public string TextValue;
    }

}
