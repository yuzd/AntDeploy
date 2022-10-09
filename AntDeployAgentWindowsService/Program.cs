using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindowsService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            AntDeployAgentWindowsService s = new AntDeployAgentWindowsService();
            if (Environment.UserInteractive)
            {
                s.RunAsConsole(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    s
                };
                ServiceBase.Run(ServicesToRun);
            }
           
        }
    }
}
