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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }
        }

        private void About_Load(object sender, EventArgs e)
        {
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.qq.jpg"))
            {
                if (stream != null)
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.zan.png"))
            {
                if (stream != null)
                {
                    pictureBox2.Image = Image.FromStream(stream);
                }
            }
        }
    }
}
