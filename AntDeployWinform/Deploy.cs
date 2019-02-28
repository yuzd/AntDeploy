using AntDeploy.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace AntDeploy.Winform
{
    public partial class Deploy : Form
    {

        private string ProjectConfigPath;
        private string ProjectFolderPath;
        private string ProjectPath;
        private string PluginConfigPath;
       

        private int ProgressPercentage = 0;
        private string ProgressCurrentHost = null;
        private string ProgressCurrentHostForWindowsService = null;
        private int ProgressPercentageForWindowsService = 0;
        private int ProgressBoxLocationLeft = 20;
        public Deploy()
        {

            InitializeComponent();

            //设定按字体来缩放控件
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //设定字体大小为12px     
            //this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));



        }

        private void Deploy_Load(object sender, EventArgs e)
        {
            //计算pannel的起始位置
            var size = this.ClientSize.Width - 650;
            if(size > 0)
            {
                ProgressBoxLocationLeft += size;
            }
            
        }

        private void b_iis_deploy_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ProgressBoxLocationLeft + "");

            for (int i = 0; i < 3; i++)
            {
                ProgressBox newBox = new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 110)))
                {
                    Text = "host"+i,
                };

                this.tabPage_progress.Controls.Add(newBox);
            }
            this.progress_iis_tip.SendToBack();
        }
    }
}
