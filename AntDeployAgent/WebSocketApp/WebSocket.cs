using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AntDeployAgentWindows.WebSocketApp
{


    #region <WebSocket委托定义>

    // 异步接受客户端连接（握手）的方法代理
    using WebSocketAccept =
        Action<IDictionary<string, object>,     //Accept字典，可以为null
        Func<                                  //握手成功后的回调函数
            IDictionary<string, object>,       //包含 SendAsync, ReceiveAsync, CloseAsync 等关键字的字典
            Task>                              //返回给服务器的表示本回调函数是否执行完成的字典
        >;


    // 异步关闭连的函数代理
    using WebSocketCloseAsync =
    Func<int,                   //关闭的状码代码
        string,                 //说明
        CancellationToken,      //任务是否取消
        Task                    //代表本操作是否完成的任务
    >;

    // 异步读取数据的函数代理
    using WebSocketReceiveAsync =
        Func<ArraySegment<byte>, // 接受数据的缓冲区
            CancellationToken,   // 传递操作是否取消
            Task<                // 返回值
                Tuple<
                    int,      // 第一分量，表示接收到的数据类型（1表示本文数据，2表示二进制数据，8表示对方关闭连接）
                    bool,     // 第二分量，表示是否是一个数据帖的最后一块或者独立块
                    int       // 第三分量，表示有效数据的长度
                >
            >
        >;


    // 异步发送数据的函数代表
    using WebSocketSendAsync =
        Func<ArraySegment<byte>,     // 待发送的缓冲区
            int,                     // 数据类型，只能是1、2、8
            bool,                    // 这一块数据是否是一条信息的最后一块
            CancellationToken,       // 取消任务的通知
            Task                     // 返回值
        >;


    #endregion



    /// <summary>
    /// WebSocket对象
    /// </summary>
    public sealed class WebSocket
    {

        


        /****************************************************************
         * 这是一个对OWIN WebSocket 进行了一定程度封装的对象，
         * 已经较为完整，不建议使用者随意修改，修改要特别小心！
         * ----------------------------------------------------------
         * 包括4个方法和三个代理（你可以改为事件）
         * ==========================================================
         * * 公开的4个方法分别是:
         * Accept:     接受远端WebSocket连接
         * StartRead： 开始接收数据
         * Send：      发送文本数据
         * Close：     关闭与完端的连接
         * -----------------------------------------------------------
         * 3个委托是：
         * OnSend：    表示数据发送完成
         * OnRead：    表示数据读取完成
         * OnClose：   表示远端主动提供断开连接
         **************************************************************/


        #region <共用委托定义>

        public delegate void DelegateReadComplete(object sender, string message);

        public delegate void DelegateWriteComplete(object sender);

        public delegate void DelegateCloseComplete(object sender);

        #endregion



        #region <私有变量>

        /// <summary>
        /// 进行连接的函数
        /// </summary>
        private WebSocketAccept _accept;


        /// <summary>
        /// 发送数据的函数
        /// </summary>
        private WebSocketSendAsync _sendAsync;

        /// <summary>
        /// 接收数据的函数
        /// </summary>
        private WebSocketReceiveAsync _receiveAsync;

        /// <summary>
        /// 关闭连接的函数
        /// </summary>
        private WebSocketCloseAsync _closeAsync;


        /// <summary>
        /// 是否已经关闭
        /// </summary>
        private bool _isClosed = true;

        /// <summary>
        /// 是否已经开始读取循环
        /// </summary>
        private int _reading;

        /// <summary>
        /// 用于保存前边收到的，还不完整的数据
        /// </summary>
        private byte[] _lastReadData;


        /// <summary>
        /// websocket工作任务完成状态
        /// </summary>
        private TaskCompletionSource<int> _webSocketCompSource;

        /// <summary>
        /// 异步Accept完成信号
        /// </summary>
        private AutoResetEvent _acceptwaiter = new AutoResetEvent(false);



        #endregion



        #region <共有字段>

        /// <summary>
        /// 数据读取完成
        /// </summary>
        public DelegateReadComplete OnRead;

        /// <summary>
        /// 连接已经断开
        /// </summary>
        public DelegateCloseComplete OnClose;

        /// <summary>
        /// 数据发送完成
        /// </summary>
        public DelegateWriteComplete OnSend;

        #endregion



        #region <共用属性>

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string RemoteIpAddress { get; private set; }

        /// <summary>
        /// 客户端端口
        /// </summary>
        public int RemotePort { get; private set; }

        /// <summary>
        /// 本地IP地址
        /// </summary>
        public string LocalIpAddress { get; private set; }

        /// <summary>
        /// 本地端口
        /// </summary>
        public int LocalPort { get; private set; }


        /// <summary>
        /// 请求的路径
        /// </summary>
        public string RequestPath { get; private set; }


        /// <summary>
        /// URL查询字串
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// WebSocket运行状态
        /// </summary>
        public Task WorkTask { get { return _webSocketCompSource.Task; } }

        #endregion



        #region <构造与析构>


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owinEnv"></param>
        public WebSocket(IDictionary<string, object> owinEnv)
        {
            // 获取Accept方法
            _accept = owinEnv.Get<WebSocketAccept>("websocket.Accept");
            if (_accept == null) throw new Exception("Not Is Websocket Request");

            // SERVER
            RemoteIpAddress = owinEnv.Get<string>("server.RemoteIpAddress");
            RemotePort = int.Parse(owinEnv.Get<string>("server.RemotePort"));
            LocalIpAddress = owinEnv.Get<string>("server.LocalIpAddress");
            LocalPort = int.Parse(owinEnv.Get<string>("server.LocalPort"));
            //var islocal = owinEnv.Get<bool>("server.IsLocal");

            // OWIN
            RequestPath = owinEnv.Get<string>("owin.RequestPath");
            Query = owinEnv.Get<string>("owin.RequestQueryString");

            _webSocketCompSource = new TaskCompletionSource<int>();
            // owinEnv.Get<string>("owin.RequestMethod");  GET/POST/....

          

        }

        #endregion



        #region <接受连接与关闭连接的操作>



        /// <summary>
        /// 响应客户端握手请求
        /// </summary>
        /// <returns>返回真表示按WebSocket的方式成功连接</returns>
        public bool Accept(IDictionary<string, object> param = null)
        {
            if (_accept == null) return false;
            try
            { _accept(param, AcceptCallback); }
            catch { return false; }
            _acceptwaiter.WaitOne();
            return _isClosed == false;

        }

        /// <summary>
        /// 握手完成后的回调函数
        /// </summary>
        /// <param name="env">底层提供的SendAsync、ReceiveAsync等方法</param>
        /// <returns></returns>
        private Task AcceptCallback(IDictionary<string, object> env)
        {
            if (env == null)
            {
                _acceptwaiter.Set();
                return Task.Delay(0);
            }

            //从字典中取出服务器提供的WebSocket操作方法
            _sendAsync = env.Get<WebSocketSendAsync>("websocket.SendAsync");
            _receiveAsync = env.Get<WebSocketReceiveAsync>("websocket.ReceiveAsync");
            _closeAsync = env.Get<WebSocketCloseAsync>("websocket.CloseAsync");

            //标记连接成功
            _isClosed = false;
            _acceptwaiter.Set();

            //通知服务器（容器），表示连接事件已经处理完成
            return Task.Delay(1);

        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (_isClosed) return;
            _isClosed = true;

            _closeAsync(1000, null, CancellationToken.None).ContinueWith(t =>
            {
                //清理异常
                if (t.IsFaulted && t.Exception != null) t.Exception.Handle(_ => true);

                //通知服务层，webscoket会话已经完成
                _webSocketCompSource.TrySetResult(1);
            });
        }


        #endregion



        #region <发送操作>

        /// <summary>
        /// 是否正在发送数据（发送还没有完成的标记）
        /// </summary>
        bool _writting = false;

        /// <summary>
        /// 发送以字节数组表示的文本内容
        /// <para>强调：必须是UTF8编码的文本数据</para>
        /// </summary>
        /// <param name="bytMessage">UTF8文本的字节数据</param>
        public void Send(byte[] bytMessage)
        {
            if (bytMessage == null || bytMessage.Length < 1)
            {
                var err = new ArgumentNullException();
                _webSocketCompSource.TrySetException(err);
                throw err;
            }

            if (_isClosed) throw new Exception("WebSocket Is Closed.");
            //if (_writting) throw new Exception("WebSocket Is Writting.");

            _writting = true;
            try
            {
                var t = _sendAsync(new ArraySegment<byte>(bytMessage), 1, true, CancellationToken.None);
                t.ContinueWith(InternalWriteComplete);
            }
            catch (Exception e)
            {
                _webSocketCompSource.TrySetException(e);
                throw e;
            }
        }


        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="message">UTF8文本内容</param>
        public void Send(string message)
        {
            Send(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// 发送完成的回调函数
        /// </summary>
        /// <param name="task"></param>
        private void InternalWriteComplete(Task task)
        {
            _writting = false;

            if (task.IsCanceled || task.IsFaulted)
            {
                _isClosed = true;
                if (task.IsFaulted && task.Exception != null) { task.Exception.Handle(_ => true); }

                OnClose?.Invoke(this); OnClose = null;
                _webSocketCompSource.TrySetResult(1);
                return;
            }

            if (OnSend == null) return;
            try
            {
                OnSend(this);
            }
            catch (Exception e)
            {
                _webSocketCompSource.TrySetException(e);
                throw e;
            }
        }


        #endregion



        #region <接收操作>

        /// <summary>
        /// 开始交互（无阻塞）
        /// </summary>
        public void Start()
        {
            if (_isClosed) throw new Exception("WebSocket Is Closed.");
            if (Interlocked.CompareExchange(ref _reading, 1, 0) == 1) return;

            InternalRealRead(new byte[1024 * 32]); //接收缓冲区
        }

        private void InternalRealRead(byte[] buffer)
        {
            if (_isClosed) return;
            var arraySeg = new ArraySegment<byte>(buffer);

            //开始异步接收数据
            _receiveAsync(arraySeg, CancellationToken.None).ContinueWith(_t =>
            {
                //如果出现了异常
                if (_t.IsFaulted || _t.IsCanceled)
                {
                    _isClosed = true;
                    OnClose?.Invoke(this); OnClose = null;
                    if (_t.IsFaulted) { if (_t.Exception != null) _t.Exception.Handle(_ => true); };
                    _webSocketCompSource.TrySetResult(1);
                    return;
                }

                //处理接收到的数据
                InternalReadComplte(arraySeg, _t.Result.Item1, _t.Result.Item2, _t.Result.Item3);
            });
        }


        /// <summary>
        /// 内部调用的，用于数据接受成功的回调函数
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="type"></param>
        /// <param name="endOfMessage"></param>
        /// <param name="size"></param>
        /// <param name="error"></param>
        private void InternalReadComplte(ArraySegment<byte> buffer, int type, bool endOfMessage, int size)
        {

            // 只接受客户端文本数据，否则关闭，如果需要接收二进制数据，请自行添加处理方法
            // type==8:对端已关闭; type=2:二进制数据
            if (type == 8 || type == 2)
            {
                _isClosed = true;

                try
                { _closeAsync(1000, null, CancellationToken.None); }
                catch { }

                if (OnClose != null) { OnClose(this); OnClose = null; }
                _webSocketCompSource.TrySetResult(1);
                return;
            }

            // 如果一帧数据已经完成
            if (endOfMessage)
            {
                var lastSize = _lastReadData == null ? 0 : _lastReadData.Length;
                var data = new byte[size + lastSize];
                if (lastSize != 0) Buffer.BlockCopy(_lastReadData, 0, data, 0, lastSize);
                Buffer.BlockCopy(buffer.Array, 0, data, lastSize, size);
                _lastReadData = null;

                if (OnRead != null)
                {
                    var s = Encoding.UTF8.GetString(data);
                    OnRead(this, s);
                }

                //继续接收
                InternalRealRead(buffer.Array);
                return;
            }


            //不完整数据帧,保存起来
            var oldSize = _lastReadData == null ? 0 : _lastReadData.Length;
            var tmpData = new byte[oldSize + size];
            if (oldSize > 0) Buffer.BlockCopy(_lastReadData, 0, tmpData, 0, oldSize);
            Buffer.BlockCopy(buffer.Array, 0, tmpData, oldSize, size);
            _lastReadData = tmpData;

            //继续接收
            InternalRealRead(buffer.Array);

        }

        #endregion



        #region <静态方法>

        /// <summary>
        /// 是否是WebSocket请求
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool IsWebSocket(IDictionary<string, object> env)
        {
            return env.IsWebSocket();
        }

        #endregion

    }


    #region <其它>


    /// <summary>
    /// Dictionary扩展类
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// 获取指定的键值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="dictionary">当前字典</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        internal static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;
            return dictionary.TryGetValue(key, out value) ? (T)value : default(T);
        }

        internal static bool IsWebSocket(this IDictionary<string, object> env)
        {
            return env != null && env.Keys.Contains("websocket.Accept");
        }
    }

    #endregion

}
