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

namespace AntDeploy.Winform
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeploy.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }
        }

        private void About_Load(object sender, EventArgs e)
        {
            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeploy.Resources.qq.jpg"))
            {
                if (stream != null)
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
        }
    }
}
