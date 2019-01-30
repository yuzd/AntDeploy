using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ClientWebSocket = System.Net.WebSockets.Managed.ClientWebSocket;

namespace AntDeploy.Util
{
    public class WebSocketHelper
    {
        //private static UTF8Encoding encoding = new UTF8Encoding();

        public static async Task<ClientWebSocket> Connect(string uri,Action<string> receiveAction)
        {

            ClientWebSocket webSocket = null;
            try
            {
                webSocket =  new System.Net.WebSockets.Managed.ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                await ReceiveFirst(webSocket, receiveAction);
                new Task(async () =>
                {
                    try
                    {
                        await Receive(webSocket, receiveAction);
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
            }
            catch (Exception ex)
            {
                //ignore
            }

            return webSocket;
        }

        //private static async Task Send(ClientWebSocket webSocket)
        //{

        //    while (webSocket.State == WebSocketState.Open)
        //    {
        //        string stringtoSend = Console.ReadLine();
        //        byte[] buffer = encoding.GetBytes(stringtoSend);

        //        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
        //        Console.WriteLine("Sent:     " + stringtoSend);

        //        await Task.Delay(1000);
        //    }
        //}

        private static async Task ReceiveFirst(ClientWebSocket webSocket, Action<string> receiveAction)
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
                    receiveAction(text);
                }
            }
        }


        public static async Task SendText(ClientWebSocket webSocket,string text)
        {
            try
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> textBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(text));
                    await webSocket.SendAsync(textBytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception)
            {

            }
        }

        private static async Task Receive(ClientWebSocket webSocket, Action<string> receiveAction)
        {
            byte[] buffer = new byte[2048];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var text = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    if (!text.StartsWith("@hello@"))
                    {
                        var arr = text.Split(new string[] { "@_@" }, StringSplitOptions.None);
                        receiveAction(arr[0]);
                    }
                   
                }
            }
        }

    }
}
