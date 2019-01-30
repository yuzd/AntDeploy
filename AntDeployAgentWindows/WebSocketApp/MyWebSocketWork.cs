using System.Collections.Concurrent;
using System.Collections.Generic;


namespace AntDeployAgentWindows.WebSocketApp
{
    /// <summary>
    /// 自定义的WebSocket工作类
    /// </summary>
    public sealed class MyWebSocketWork : MyWebSocketWorkerBase
    {

        public static ConcurrentDictionary<string, WebSocket> WebSockets = new ConcurrentDictionary<string, WebSocket>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reqEnv"></param>
        public MyWebSocketWork(IDictionary<string, object> reqEnv) : base(reqEnv) { }


        /// <summary>
        /// 与客户端握手完成的事件
        /// </summary>
        /// <param name="sender"></param>
        protected override void OnAccept(object sender)
        {
            //可以做一些初始化工作，比如登记客户IP地址之类的事情

            WebSockets[WebSocket.RemoteIpAddress + ":" + WebSocket.RemotePort] = WebSocket;

            WebSocket.Send("hostKey@"+ WebSocket.RemoteIpAddress + ":" + WebSocket.RemotePort);
        }


        /// <summary>
        /// 数据接收完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected override void OnRead(object sender, string message)
        {

            //约定：如果客户端发来 "exit" "close" 字串
            //服务器就关闭这个连接
            if (message == "exit" || message == "close")
            {
                //服务器关闭会话
                WebSocket.Close();
                return;
            }

            //回应客户端发送过来的内容

            if (message == "@hello@")
            {
                WebSocket.Send(message);
            }
        }


        /// <summary>
        /// 数据发送完成的事件
        /// </summary>
        /// <param name="sender"></param>
        protected override void OnSend(object sender)
        {

            // your code ...

            /// ..... ////
        }



        /// <summary>
        /// 客户端关闭连接事件
        /// </summary>
        /// <param name="sender"></param>
        protected override void OnClose(object sender)
        {
            WebSockets.TryRemove(WebSocket.RemoteIpAddress + ":" + WebSocket.RemotePort, out _);
        }



    }
}
