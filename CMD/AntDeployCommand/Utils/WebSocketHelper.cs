using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using LibGit2Sharp;
using ClientWebSocket = System.Net.WebSockets.Managed.ClientWebSocket;

namespace AntDeployCommand.Utils
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

        public Action<string,LogLevel> receiveAction { get; set; }
        public HttpLogger HttpLogger { get; set; }
        public bool HasError { get; set; }
        private ClientWebSocket webSocket = null;
        private bool _dispose = false;
        private WebClient client;
        public WebSocketClient(Action<string, LogLevel> _receiveAction, HttpLogger _loggerKey)
        {
            this.receiveAction = _receiveAction;
            this.HttpLogger = _loggerKey;
            client = new WebClientExtended();
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
            catch
            {
                //ignore
            }
            finally
            {
                try
                {
                    client.Dispose();
                }
                catch
                {
                    //ignore
                }
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
                receiveAction.Invoke(!string.IsNullOrEmpty(key)
                    ? $"WebSocket Connect Success:{key},Server excute logs will accept from websocket"
                    : $"WebSocket Connect Fail,Server excute logs will accept from http request",LogLevel.Debug);
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
            catch (Exception)
            {
                //ignore
                receiveAction.Invoke($"WebSocket Connect Fail,Server excute logs will accept from http request",LogLevel.Debug);
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

            try
            {

                var rawData = client.DownloadData(new Uri(this.HttpLogger.Url));
                if (rawData.Length < 1) return;
                var encoding = GetEncodingFrom(client.ResponseHeaders, Encoding.UTF8);
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
                                this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Info);
                                AgentCheckVersion(receiveMsg);
                            }
                            else if (receiveMsg.Contains("【Error】"))
                            {
                                this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Info);
                                HasError = true;
                            }
                            else
                            {
                                this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Info);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            if (isLast)
            {
                LogAgentCheckVersion();
            }
        }

        private string _agentVersion;
        private bool _needUpdate;
        private void AgentCheckVersion(string receiveMsg)
        {
            var agentVersionArr = receiveMsg.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
            if (agentVersionArr.Length == 2)
            {
                var agentVersion = agentVersionArr[1];
                if (!agentVersion.Equals(Vsix.AGENTVERSION))
                {
                    var versionTemp = Vsix.AGENTVERSION.Replace(".", "");
                    if (int.TryParse(versionTemp, out int versionTempInt))
                    {
                        var temp = agentVersion.Replace(".", "");
                        if (int.TryParse(temp, out int _tempInt))
                        {
                            //如果服务端的agent 大于 本地的版本号 说明 客户端是旧的
                            if (_tempInt > versionTempInt)
                            {
                                _needUpdate = true;
                                return;
                            }
                        }
                    }
                    _agentVersion = agentVersion;
                }
            }
        }

        private void LogAgentCheckVersion()
        {
            if (_needUpdate)
            {
                this.receiveAction.Invoke($"【Server】You need update AntDeploy!",LogLevel.Warning);
                this.receiveAction.Invoke("【Server】<a href=\"https://marketplace.visualstudio.com/items?itemName=nainaigu.AntDeploy\">Download AntDeploy</a>", LogLevel.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_agentVersion))
            {
                return;
            }
            this.receiveAction.Invoke($"【Server】You need update agent version To :【{Vsix.AGENTVERSION}】",LogLevel.Warning);
            this.receiveAction.Invoke("【Server】<a href=\"https://github.com/yuzd/AntDeployAgent/issues/1\">Download Agent</a>", LogLevel.Warning);
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
                                    this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Info);
                                    AgentCheckVersion(receiveMsg);
                                }
                                else if (receiveMsg.Contains("【Error】"))
                                {
                                    this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Warning);
                                    HasError = true;
                                }
                                else
                                {
                                    this.receiveAction.Invoke($"【Server】{receiveMsg}",LogLevel.Info);
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
        public static Encoding GetEncodingFrom(
            NameValueCollection responseHeaders,
            Encoding defaultEncoding = null)
        {
            try
            {
                if (responseHeaders == null)
                    return Encoding.UTF8;

                //Note that key lookup is case-insensitive
                var contentType = responseHeaders["Content-Type"];
                if (contentType == null)
                    return defaultEncoding;

                var contentTypeParts = contentType.Split(';');
                if (contentTypeParts.Length <= 1)
                    return defaultEncoding;

                var charsetPart =
                    contentTypeParts.Skip(1).FirstOrDefault(
                        p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
                if (charsetPart == null)
                    return defaultEncoding;

                var charsetPartParts = charsetPart.Split('=');
                if (charsetPartParts.Length != 2)
                    return defaultEncoding;

                var charsetName = charsetPartParts[1].Trim();
                if (charsetName == "")
                    return defaultEncoding;


                return Encoding.GetEncoding(charsetName);
            }
            catch (ArgumentException)
            {
                return Encoding.UTF8;
            }
        }
    }

    class LoggerModel
    {
        public string Msg { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }

    }

    public class WebClientExtended : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            var w = (HttpWebRequest)base.GetWebRequest(uri);
            w.Timeout = 5000;      // Set timeout
            w.KeepAlive = true;    // Set keepalive true or false
            w.ServicePoint.SetTcpKeepAlive(true, 1000, 5000);  // Set tcp keepalive
            return w;
        }
    }


}
