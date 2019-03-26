using System;
using System.ServiceProcess;

namespace AntDeployAgentService
{
    class Program
    {
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
