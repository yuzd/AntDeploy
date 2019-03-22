using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows;
using TinyFox;

namespace AntDeployAgentWindowsService
{
    public partial class AntDeployAgentWindowsService : ServiceBase
    {
        private Startup _startup;
        public AntDeployAgentWindowsService()
        {
            InitializeComponent();
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Current Version：" + AntDeployAgentWindows.Version.VERSION);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            OnStop();
        }

        public void start(string[] args)
        {
            this.OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            var port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            TinyFoxService.Port = string.IsNullOrEmpty(port) ? 8088 : int.Parse(port);
            _startup = new AntDeployAgentWindows.Startup();
            TinyFoxService.Start(_startup.OwinMain);
        }

        protected override void OnStop()
        {
            _startup?.Stop();
            TinyFoxService.Stop();
        }
    }
}
