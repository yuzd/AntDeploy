using AntDeployAgentWindows.WebApiCore;
using AntDeployAgentWindows.WebSocketApp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AntDeployAgentWindows.Model;

namespace AntDeployAgentWindows.MyApp.Service
{
    public abstract class PublishProviderBasicAPI : CommonProcessor, IPublishProviderAPI
    {
        private object obj = new object();
        private bool webSocketDisposed = false;
       
        private static readonly ConcurrentDictionary<string, ReaderWriterLockSlim> locker = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
        public abstract string ProviderName { get; }
        public string LoggerKey { get; set; }
        public WebSocketApp.WebSocket WebSocket { get; set; }
        public abstract string ProjectName { get; }
        public abstract string DeployExcutor(FormHandler.FormItem fileItem);
        public abstract string CheckData(FormHandler formHandler);
        public string Deploy(FormHandler.FormItem fileItem)
        {
            //按照项目名称 不能并发发布
            if (!string.IsNullOrEmpty(ProjectName))
            {
                var key = (ProviderName ?? string.Empty) + ProjectName;
                if (!locker.TryGetValue(key, out var ReaderWriterLockSlim))
                {
                    ReaderWriterLockSlim = new ReaderWriterLockSlim();
                    locker.TryAdd(key, ReaderWriterLockSlim);
                }

                if (ReaderWriterLockSlim.IsWriteLockHeld)
                {
                    return $"{ProjectName} is deploying!,please wait for senconds!";
                }

                ReaderWriterLockSlim.EnterWriteLock();

                try
                {
                    return DeployExcutor(fileItem);
                }
                finally
                {
                    ReaderWriterLockSlim.ExitWriteLock();
                }

            }
            else
            {
                return DeployExcutor(fileItem);
            }
        }

        public virtual string RollBack()
        {
            throw new NotImplementedException();
        }


        public string Check(FormHandler formHandler)
        {
            var wsKey = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("wsKey"));
            if (wsKey != null  && !string.IsNullOrEmpty(wsKey.TextValue))
            {
                var _wsKey = wsKey.TextValue;

                if (MyWebSocketWork.WebSockets.TryGetValue(_wsKey, out var sockert))
                {
                    WebSocket = sockert;
                    WebSocket.OnClose += sender => { webSocketDisposed = true; };
                }
            }

            return CheckData(formHandler);
        }

        protected void EnsureProjectFolder(string path)
        {
            try
            {
                lock (obj)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        protected void Log(string str)
        {
            try
            {
                if (WebSocket != null && !webSocketDisposed)
                {
                    try
                    {
                        WebSocket.Send(str + "@_@" + str.Length);
                    }
                    catch (Exception ex)
                    {
                        //WebSocket发送失败
                        if (!string.IsNullOrEmpty(LoggerKey))
                        {
                            if (LoggerService.loggerCollection.TryGetValue(LoggerKey, out var list))
                            {
                                list.Add(new LoggerModel
                                {
                                    Date = DateTime.Now,
                                    IsActive = false,
                                    Msg = str
                                });
                            }
                        }
                    }
                }
                else
                {
                    //WebSocket发送失败
                    if (!string.IsNullOrEmpty(LoggerKey))
                    {
                        if (LoggerService.loggerCollection.TryGetValue(LoggerKey, out var list))
                        {
                            list.Add(new LoggerModel
                            {
                                Date = DateTime.Now,
                                IsActive = false,
                                Msg = str
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ignore

            }
        }

        protected string getCorrectFolderName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(System.Char.ToString(c), "");
            }
            var aa = Regex.Replace(name, "[ \\[ \\] \\^ \\-_*×――(^)（^）$%~!@#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
            aa = aa.Replace(" ", "").Replace("　", "");
            aa = Regex.Replace(aa, @"[~!@#\$%\^&\*\(\)\+=\|\\\}\]\{\[:;<,>\?\/""]+", "");
            return aa;
        }
    }


}
