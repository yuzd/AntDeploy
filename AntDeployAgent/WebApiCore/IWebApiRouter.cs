using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace AntDeployAgentWindows.WebApiCore
{
    /// <summary>
    /// 路由配置
    /// </summary>
    interface IWebApiRouter
    {
        BaseWebApi RouteTo(IOwinContext c);
    }
}
