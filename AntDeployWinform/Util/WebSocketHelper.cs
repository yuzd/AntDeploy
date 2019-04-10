using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientWebSocket = System.Net.WebSockets.Managed.ClientWebSocket;

namespace AntDeployWinform.Util
{
    public class HttpLogger : IDisposable
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public bool IsDispose { get; set; }

        public void Dispose()
        {
            IsDispose = true;
        }
    }

    public class WebSocketClient
    {
        //private static UTF8Encoding encoding = new UTF8Encoding();

        public Logger receiveAction { get; set; }
        public HttpLogger HttpLogger { get; set; }
        public bool HasError { get; set; }
        private ClientWebSocket webSocket = null;
        private bool _dispose = false;
        public WebSocketClient(Logger _receiveAction, HttpLogger _loggerKey)
        {
            this.receiveAction = _receiveAction;
            this.HttpLogger = _loggerKey;
        }


        public async Task Dispose()
        {
            try
            {
                if (_dispose) return;

                _dispose = true;

                HttpLogger?.Dispose();

                await SendText("close");

                webSocket?.Dispose();

                Thread.Sleep(1000);

                ReceiveHttpAction();
            }
            catch (Exception)
            {

            }
        }

        public async Task<string> Connect(string uri)
        {
            string key = string.Empty;
            try
            {
                webSocket = new System.Net.WebSockets.Managed.ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                key = await ReceiveFirst();
                receiveAction.Debug(!string.IsNullOrEmpty(key)
                    ? $"WebSocket Connect Success:{key},Server excute logs will accept from websocket"
                    : $"WebSocket Connect Fail,Server excute logs will accept from http request");
                new Task(async () =>
                {
                    try
                    {
                        await Receive();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            webSocket.Dispose();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }).Start();

                new Task(() =>
               {
                   try
                   {
                       ReceiveHttp();
                   }
                   catch (Exception)
                   {
                   }
               }).Start();
            }
            catch (Exception ex)
            {
                //ignore
                receiveAction.Debug($"WebSocket Connect Fail,Server excute logs will accept from http request");
            }
            if (string.IsNullOrEmpty(key))
            {
                key = await ReceiveFirst();
            }
            return key;
        }


        private async Task<string> ReceiveFirst()
        {
            byte[] buffer = new byte[1024];
            if (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var text = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    if (text.StartsWith("hostKey@"))
                    {
                        return text.Replace("hostKey@", "");
                    }
                    return string.Empty;
                }
            }
            return string.Empty;
        }


        public async Task SendText(string text)
        {
            try
            {
                if (webSocket != null && webSocket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> textBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
                    await webSocket.SendAsync(textBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception)
            {

            }
        }


        public void ReceiveHttpAction(bool isLast = false)
        {
            var client = new WebClient();
            try
            {
                
                var rawData = client.DownloadData(new Uri(this.HttpLogger.Url));
                var encoding = WebUtil.GetEncodingFrom(client.ResponseHeaders, Encoding.UTF8);
                var result = encoding.GetString(rawData);

                //var result = client.DownloadString(new Uri(logger.Url));
                if (!string.IsNullOrEmpty(result))
                {
                    var list = JsonConvert.DeserializeObject<List<LoggerModel>>(result);
                    foreach (var li in list)
                    {
                        var receiveMsg = "**" + li.Msg;
                        if (!string.IsNullOrEmpty(receiveMsg))
                        {
                            if (receiveMsg.Contains("agent version ==>"))
                            {
                                this.receiveAction.Info($"【Server】{receiveMsg}");
                                AgentCheckVersion(receiveMsg);
                            }
                            else if (receiveMsg.Contains("【Error】"))
                            {
                                this.receiveAction.Warn($"【Server】{receiveMsg}");
                                HasError = true;
                            }
                            else
                            {
                                this.receiveAction.Info($"【Server】{receiveMsg}");
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
            finally
            {
              
                client.Dispose();
            }

            if (isLast)
            {
                LogAgentCheckVersion();
            }
        }

        private string _agentVersion;
        private void AgentCheckVersion(string receiveMsg)
        {
            _agentVersion = receiveMsg;
            var agentVersionArr = receiveMsg.Split(new string[] { "=>" }, StringSplitOptions.None);
            if (agentVersionArr.Length == 2)
            {
                var agentVersion = agentVersionArr[1];
                if (!agentVersion.Equals(Vsix.AGENTVERSION))
                {
                    _agentVersion = agentVersion;
                }
            }
        }

        private void LogAgentCheckVersion()
        {
            if (string.IsNullOrEmpty(_agentVersion))
            {
                return;
            }
            this.receiveAction.Warn($"【Server】You need update agent version To :【{Vsix.AGENTVERSION}】");
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Warn, "", "【Server】Download Agent Url:");
            theEvent.LoggerName = receiveAction.Name;
            theEvent.Properties["ShowLink"] = "https://github.com/yuzd/AntDeployAgent/issues/1";
            this.receiveAction.Log(theEvent);
        }

        /// <summary>
        /// 接受http轮训 log
        /// </summary>
        private void ReceiveHttp()
        {
            //在未结束之前 1秒钟拿1次
            while (!this.HttpLogger.IsDispose)
            {
                ReceiveHttpAction();

                Thread.Sleep(1000);
            }

            //结束之后拿一次
            ReceiveHttpAction();
        }

        /// <summary>
        /// 接受websocket日志
        /// </summary>
        /// <returns></returns>
        private async Task Receive()
        {
            byte[] buffer = new byte[2048];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        var text = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        if (!string.IsNullOrEmpty(text))
                        {
                            var arr = text.Split(new string[] { "@_@" }, StringSplitOptions.None);
                            var receiveMsg = "*" + arr[0];
                            if (!string.IsNullOrEmpty(receiveMsg))
                            {
                                if (receiveMsg.Contains("agent version ==>"))
                                {
                                    this.receiveAction.Info($"【Server】{receiveMsg}");
                                    AgentCheckVersion(receiveMsg);
                                }
                                else if (receiveMsg.Contains("【Error】"))
                                {
                                    this.receiveAction.Warn($"【Server】{receiveMsg}");
                                    HasError = true;
                                }
                                else
                                {
                                    this.receiveAction.Info($"【Server】{receiveMsg}");
                                }
                            }
                        }

                    }
                }
                catch (Exception)
                {

                }
            }
        }

    }

    class LoggerModel
    {
        public string Msg { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }

    }
}
