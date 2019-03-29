using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AntDeployWinform.Winform
{
    public partial class RollBack : Form
    {
        public RollBack(List<string> list)
        {
            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            this.listbox_rollback_list.Items.Clear();;
            foreach (var li in list)
            {
                this.listbox_rollback_list.Items.Add(li);
            }

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
            var selectItem = this.listbox_rollback_list.SelectedItem as string;
            if (string.IsNullOrEmpty(selectItem))
            {
                MessageBox.Show("please select rollback version!");
                return;
            }

            SelectRollBackVersion = selectItem;
            this.DialogResult = DialogResult.OK;
        }

        public void SetButtonName(string name)
        {
            this.b_rollback_Rollback.Text = name;
        }
    }
}
