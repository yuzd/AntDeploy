using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.WebApiCore;
using AntDeployAgentWindows.WebSocket.WebSocketApp;
using Microsoft.Owin.Builder;
using Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows
{
    /// <summary>
    /// WeB启动类
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Microsoft.Owin中间件处理管线的入口函数
        /// </summary>
        private readonly Func<IDictionary<string, object>, Task> _owinAppFunc;


        public Startup()
        {
            var builder = new AppBuilder();
            Configuration(builder);
            _owinAppFunc = Microsoft.Owin.Builder.AppBuilderExtensions.Build(builder);
            var deployDir = System.Configuration.ConfigurationManager.AppSettings["DeployDir"];
            if (!string.IsNullOrEmpty(deployDir))
            {
                Setting.InitWebRoot(deployDir, true);
            }
            else
            {
                Setting.InitWebRoot(AppDomain.CurrentDomain.BaseDirectory);
            }

        }



        /// <summary>
        /// 启动类的配置方法，格式是 Microsoft.Owin 所要求的
        /// <para>必须是public，而且方法名和参数类型、参数数量都固定</para>
        /// </summary>
        /// <param name="builder">App生成器</param>
        public void Configuration(IAppBuilder builder)
        {

            //websocket中间件
            builder.UseWebSocket();

            //预处理中间件，放在第一位
            builder.UseJwsIntegration();


            // 添加FastWebApi中间件，具体实现，在WebApiMiddleware.cs文件中
            ///////////////////////////////////////////////////////////////////////////
            builder.UseFastWebApi(new MyWebApiRouter());


            // 放在处理链中最后执行的方法（相当于前一个中间件的next对象的Invoke方法）
            // 如果前边的中件间成功处理了请求，工作就不会交到这个位置来。
            ////////////////////////////////////////////////////////////////////////////
            builder.Run((c) =>
            {
                // 能流传到这儿，表示前边的中件没有处理
                // 所以，应该返回找不到网页的信息（404）

                c.Response.StatusCode = 404;
                c.Response.Write(Encoding.ASCII.GetBytes(string.Format("Can't found The Path: {0}", c.Request.Path)));

                return Task.FromResult(0);
            });

        }




        /// <summary>
        /// TinyFox OWIN 主方法：所有的请求都会由经这儿开始处理
        /// </summary>
        /// <param name="env">OWIN会话环境</param>
        /// <returns></returns>
        public Task OwinMain(IDictionary<string, object> env)
        {
            // 客户所有的请求以及输入输出IO流都在字典参数中
            // 字典的 key/value 符合 OWIN 1.0 及其扩展标准

            // 由于本Demo使用的是 Microsoft.Owin 架构
            // 所以直接进入Microsoft.Owin 的处理管道 
            return _owinAppFunc != null ? _owinAppFunc(env) : null;
        }

        public void Stop()
        {
            Setting.StopWatchFolderTask();
        }
    }
}
