using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.WebApiCore;

namespace AntDeployAgentWindows.MyApp
{
    public class PublishService : BaseWebApi
    {
        protected override async void ProcessRequest()
        {
            if (Request.Method == "POST")
            {
                var x = await Request.ReadFormAsync();
            }

            //定义返回的数据类型：文本
            Response.ContentType = "text/plain";

            //发送文本内容
            Response.Write(string.Format("This Is \"App1\", Your IP Address Is:{0}, Port:{1}", Request.RemoteIpAddress, Request.RemotePort));

        }
    }
}
