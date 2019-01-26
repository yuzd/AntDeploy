using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TinyFox;

namespace AntDeployAgentWindowsService
{
    public partial class AntDeployAgentWindowsService : ServiceBase
    {
        public AntDeployAgentWindowsService()
        {
            InitializeComponent();
        }

        public void start(string[] args)
        {
            this.OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            var port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            TinyFoxService.Port = string.IsNullOrEmpty(port) ? 8088 : int.Parse(port);
            var startup = new AntDeployAgentWindows.Startup();
            TinyFoxService.Start(startup.OwinMain);
        }

        protected override void OnStop()
        {
            TinyFoxService.Stop();
        }
    }
}
