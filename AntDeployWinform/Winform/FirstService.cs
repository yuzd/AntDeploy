using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntDeployWinform.Models;

namespace AntDeployWinform.Winform
{
    public partial class FirstService : Form
    {
        public FirstService()
        {
            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            comboBox_service_start_type.SelectedIndex = 0;
        }

        public FirstCreateParam WindowsServiceCreateParam { get; set; }

        private void btn_continue_Click(object sender, EventArgs e)
        {
            WindowsServiceCreateParam = new FirstCreateParam
            {
                Desc = this.txt_service_description.Text.Trim(),
                StartUp = this.comboBox_service_start_type.SelectedItem as string,
                PhysicalPath = this.txt_windows_service_PhysicalPath.Text.Trim(),
                Param = this.txt_service_param.Text.Trim(),
                useNssm = this.box_windows_service_nssm.Checked?"yes":"no"
            };
            this.DialogResult = DialogResult.OK;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/43");
            Process.Start(sInfo);
        }
    }
}
