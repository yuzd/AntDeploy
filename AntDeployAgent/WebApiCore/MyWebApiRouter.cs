using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.MyApp;
using Microsoft.Owin;

namespace AntDeployAgentWindows.WebApiCore
{
    /// <summary>
    /// 路由器：自定义URL路由规则
    /// </summary>
    class MyWebApiRouter : IWebApiRouter
    {
        /// <summary>
        /// 返回一个处理对象，如果当前请求不在处理之列则返回空
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public BaseWebApi RouteTo(IOwinContext c)
        {

            //用户请求的URL路径
            var path = c.Request.Path.Value;

            //将不同的请求路径交给不同的处理模块处理
            if (path == "/publish" || path.StartsWith("/publish/")) return new PublishService();
            if (path == "/rollback" || path.StartsWith("/rollback/")) return new RollbackService();
            if (path == "/version" || path.StartsWith("/version/")) return new VersionService();
            if (path == "/logger" || path.StartsWith("/logger/")) return new LoggerService();


            return null;

        }

    }
}
