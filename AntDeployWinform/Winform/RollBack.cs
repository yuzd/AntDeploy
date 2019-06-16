using AntDeployWinform.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AntDeployWinform.Models;

namespace AntDeployWinform.Winform
{
    public partial class RollBack : Form
    {
        private readonly bool isRollBack = false;
        private string _lang = string.Empty;
        public RollBack()
        {
            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            _lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
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
                    if (string.IsNullOrEmpty(remark))
                    {
                        lv.SubItems.Add(remark);
                        lv.ForeColor = Color.Red;
                    }
                    else
                    {
                        var linuxRemark = remark.JsonToObject<LinuxArgModel>();
                        if (linuxRemark == null)
                        {
                            lv.SubItems.Add(remark);
                            lv.ForeColor = Color.Red;
                        }
                        else
                        {
                            lv.SubItems.Add(linuxRemark.Remark);
                            lv.SubItems.Add(linuxRemark.Pc);
                            lv.SubItems.Add(linuxRemark.Mac);
                            lv.SubItems.Add(linuxRemark.Ip);
                        }
                    }
                    
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
            
            foreach (var li in list)
            {
                if (!isNotRollback)
                {
                    
                    var version = string.Empty;
                    var remark = string.Empty;
                    var pc = string.Empty;
                    var mac = string.Empty;
                    var ip = string.Empty;
                    var isFail = false;
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

                        if (string.IsNullOrWhiteSpace(content.Args))
                        {
                            //可能是失败的
                            isFail = true;
                        }
                        if (content.FormItemList != null && content.FormItemList.Any())
                        {
                            var remarkItem = content.FormItemList.FirstOrDefault(r => r.FieldName.Equals("remark"));
                            if (remarkItem != null && !string.IsNullOrEmpty(remarkItem.TextValue))
                            {
                                remark = remarkItem.TextValue;
                            }

                            var pcItem = content.FormItemList.FirstOrDefault(r => r.FieldName.Equals("pc"));
                            if (pcItem != null && !string.IsNullOrEmpty(pcItem.TextValue))
                            {
                                pc = pcItem.TextValue;
                            }

                            var macItem = content.FormItemList.FirstOrDefault(r => r.FieldName.Equals("mac"));
                            if (macItem != null && !string.IsNullOrEmpty(macItem.TextValue))
                            {
                                mac = macItem.TextValue;
                            }

                            var ipItem = content.FormItemList.FirstOrDefault(r => r.FieldName.Equals("localIp"));
                            if (ipItem != null && !string.IsNullOrEmpty(ipItem.TextValue))
                            {
                                ip = ipItem.TextValue;
                            }
                        }
                    }

                    if(string.IsNullOrEmpty(version)) continue;
                    ListViewItem lv = new ListViewItem();
                    lv.Text = version;
                    lv.SubItems.Add(remark);
                    lv.SubItems.Add(pc);
                    lv.SubItems.Add(mac);
                    lv.SubItems.Add(ip);
                    if(isFail)lv.ForeColor = Color.Red;
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
                if (listView_rollback_version.CheckedItems.Count<1)
                {
                    MessageBoxEx.Show(this, (!string.IsNullOrEmpty(_lang) && _lang.StartsWith("zh-") ? "请选择要回滚的版本号" : "please select one rollback version!"));
                    return;
                }

                //var selectView = this.listView_rollback_version.SelectedItems[0];
                SelectRollBackVersion = listView_rollback_version.CheckedItems[0].Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                var selectItem = this.listbox_rollback_list.SelectedItem as string;
                if (string.IsNullOrEmpty(selectItem))
                {
                    MessageBoxEx.Show(this, (!string.IsNullOrEmpty(_lang) && _lang.StartsWith("zh-") ? "请选择" : "please select one!"));
                    return;
                }

                SelectRollBackVersion = selectItem;
                this.DialogResult = DialogResult.OK;
            }
           
        }

       
        public void SetTitle(string name)
        {
            this.label_server_name.Text = name;
        }

        public void ShowAsHistory(string name)
        {
            this.Text = (!string.IsNullOrEmpty(_lang) && _lang.StartsWith("zh-") ?"[发布历史]": "[History]") +name;
            this.label_server_name.Visible = false;
            this.b_rollback_Rollback.Visible = false;
            listView_rollback_version.Dock = DockStyle.Fill;
            //this.Text = "Deploy History：";
            this.ShowDialog();
           
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
        public string FieldName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }



        /// <summary>
        /// 文本内容
        /// </summary>
        public string TextValue { get; set; }
    }

}
