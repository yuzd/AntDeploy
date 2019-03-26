using AntDeployAgentWindows.WebSocketApp;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.WebSocket.WebSocketApp
{
    class WebSocketMiddleware : OwinMiddleware
    {
        /// <summary>
        /// 下一个“中间件”对象
        /// </summary>
        OwinMiddleware _next;
        public WebSocketMiddleware(OwinMiddleware next) : base(next)
        {
            _next = next;
        }

        public override Task Invoke(IOwinContext context)
        {
            //检查是否是WebSocket会话请求
            if (AntDeployAgentWindows.WebSocketApp.WebSocket.IsWebSocket(context.Environment))
            {
                return new MyWebSocketWork(context.Environment).Open();
                //注：Open方法可以带参数
                //如：Open(new Dictionary<string, object> { {"websocket.SubProtocol","yourxxx"} });
            }
            else
            {
                return _next.Invoke(context);
            }

        }
    }

    static class WebSocketExtension
    {
        /// <summary>
        /// 启用 WebSocket 中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="router"></param>
        /// <returns></returns>
        public static IAppBuilder UseWebSocket(this IAppBuilder builder)
        {
            return builder.Use<WebSocketMiddleware>();
        }

    }
}
