using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.WebSocketApp
{

    /// <summary>
    /// 自定义WebSocket工作器的基类
    /// </summary>
    public abstract class MyWebSocketWorkerBase
    {

        /***************************************************************
         * 注意：不建议开发者随意修改本类；
         * 即命名修改，也要小心，要尽量简洁，确保无错
         * *************************************************************/



        #region <变量定义>


        /// <summary>
        /// 与本自定义绑定的WebSocket实例（子类可读）
        /// </summary>
        protected readonly WebSocket WebSocket;


        #endregion;



        #region <初始化与开始WebSocket交互的方法实现>

        /// <summary>
        /// MyWebSocket构造函数
        /// </summary>
        /// <param name="owinenv"></param>
        public MyWebSocketWorkerBase(IDictionary<string, object> owinenv)
        {

            WebSocket = new WebSocket(owinenv)
            {
                OnSend = OnSend,
                OnClose = OnClose,
                OnRead = OnRead
            };
        }



        /// <summary>
        /// 完成与客户端握手并开启数据交流过程
        /// </summary>
        ///<param name="param">用于WebSocket握手响应用的参数</param>
        /// <returns></returns>
        public Task Open(IDictionary<string, object> param = null)
        {
            //尝试握手，同意请求
            //如果握手成功
            if (WebSocket.Accept(param))
            {

                //激活OnAccept事件，通知应用层握手已经完成
                OnAccept(WebSocket);

                
                //开始接受远端数据
                //本方法只需在连接成功后调用一次，然后就能不断继续。
                WebSocket.Start();

                
                //返回WebSocket工作任务
                return WebSocket.WorkTask;
            }

            //如果握手失败
            Console.WriteLine("Error: 与客户端握手失败, 客户端 IP 地址是: {0}", WebSocket.RemoteIpAddress);

            //返回（失败的）完成任务
            return Task.FromResult(new Exception("WebSocket Accept Error."));
        }

        #endregion



        #region <需要子类的方法>


        /// <summary>
        /// 与客户端握手完成事件
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnAccept(object sender) { }

        /// <summary>
        /// 发送完成事件
        /// </summary>
        /// <param name="sender"></param>
        protected abstract void OnSend(object sender);


        /// <summary>
        /// 接收到客户端数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        protected abstract void OnRead(object sender, string message);


        /// <summary>
        /// 客户端关闭事件
        /// </summary>
        /// <param name="sender"></param>
        protected abstract void OnClose(object sender);


        #endregion

    }
}
