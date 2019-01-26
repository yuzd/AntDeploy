using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyFox;

namespace AntDeployAgentWindows
{
    class Program
    {
        static void Main(string[] args)
        {
            //--------------------------------------------------------
            // * OWIN WEB服务配置与启动
            //--------------------------------------------------------

            // 一个启动对象
            var startup = new Startup();

            //TinyFoxService.Port = 8088;                           //服务端口（默认是8088）
            TinyFoxService.WebRoot = @"A:\Users\Administrator\AppData\Local\tx\1877682825\FileRecv\TinyFox181113\TinyFox\Demo\Demo0\bin\Debug\site";           //网站根文件夹(默认路径是：应用程序/site/wwwroot)
            // 启动服务
            TinyFoxService.Start(startup.OwinMain);       //启动服务（非阻塞的）


            //----------------------------------------------------
            // * 等候退出
            //-----------------------------------------------------

            Console.WriteLine("如要退出，请按 Ctrl+c 键");    //提示
            (new AutoResetEvent(false)).WaitOne();            //阻塞与等候
            Console.WriteLine("正在退出，拜拜！");            //提示
            TinyFoxService.Stop();                            //这句还是比较重要的
        }
    }
}
