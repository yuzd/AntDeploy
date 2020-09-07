using AntDeployAgentWindows.WebApiCore;
using AntDeployAgentWindows.WebSocketApp;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AntDeployAgentWindows.Model;
using Newtonsoft.Json;

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
        public abstract string ProjectPublishFolder { get;}
        public abstract string DeployExcutor(FormHandler.FormItem fileItem);
        public abstract string CheckData(FormHandler formHandler);
        private FormHandler _formHandler;
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
                    return ExcuteDeploy(fileItem);
                }
                finally
                {
                    ReaderWriterLockSlim.ExitWriteLock();
                }
            }
            else
            {
                return ExcuteDeploy(fileItem);
            }
        }

        public virtual string RollBack()
        {
            throw new NotImplementedException();
        }

        private string ExcuteDeploy(FormHandler.FormItem fileItem)
        {
            var re = DeployExcutor(fileItem);
            try
            {
                if (_formHandler == null)
                {
                    return re;
                }

                if (!string.IsNullOrEmpty(re))
                {
                    //发布出错了 删除此次版本号
                    //new Task(() =>
                    //{
                    //    try
                    //    {
                    //        Directory.Delete(ProjectPublishFolder, true);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        //ignore
                    //    }
                    //}).Start();
                   
                    return re;
                }

                if (string.IsNullOrEmpty(ProjectPublishFolder) || !Directory.Exists(ProjectPublishFolder))
                {
                    return re;
                }

                //保存参数
                var formArgs = _formHandler.FormItems.Where(r => r.FileBody == null || r.FileBody.Length < 1).ToList();
                if (formArgs.Any())
                {
                    var path = Path.Combine(ProjectPublishFolder, "antdeploy_args.json");
                    var content = JsonConvert.SerializeObject(formArgs);
                    File.WriteAllText(path,content,Encoding.UTF8);
                }

                SaveCurrentVersion(ProjectPublishFolder);
            }
            catch (Exception)
            {
               //ignore
            }

            var projectRootPath = new DirectoryInfo(ProjectPublishFolder).Parent;
            if (projectRootPath == null || !projectRootPath.Exists) return re;
            //每次发布完成后清理老的发布历史记录 只清理自己项目的 
            //防止别的项目正在回滚到某个版本，你这边发现这个版本已经过时了就删除了
            Setting.ClearOldFolders(ProviderName.Equals("iis"), projectRootPath.Name);
            return re;
        }

        /// <summary>
        /// 记录当前正在使用的文件记录 目的是为了让删除任务排除这个文件夹
        /// </summary>
        /// <param name="projectPublishFolder">当前发布的版本目录 或者 回滚成的版本目录</param>
        protected void SaveCurrentVersion(string projectPublishFolder)
        {
            var projectRootPath = new DirectoryInfo(projectPublishFolder).Parent;
            if (projectRootPath == null || !projectRootPath.Exists) return;
            var currentVersionName = new DirectoryInfo(projectPublishFolder).Name;
            if (string.IsNullOrEmpty(currentVersionName)) return;
            File.WriteAllText(Path.Combine(projectRootPath.FullName, "current.txt"), currentVersionName, Encoding.UTF8);
        }

        public string Check(FormHandler formHandler)
        {
            _formHandler = formHandler;
            var wsKey = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("wsKey"));
            if (wsKey != null && !string.IsNullOrEmpty(wsKey.TextValue))
            {
                var _wsKey = wsKey.TextValue;

                if (MyWebSocketWork.WebSockets.TryGetValue(_wsKey, out var sockert))
                {
                    WebSocket = sockert;
                    WebSocket.OnClose += sender => { webSocketDisposed = true; };
                }
            }

            var macValue = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("mac"));
            if (macValue != null && !string.IsNullOrEmpty(macValue.TextValue))
            {
                if (!Setting.CheckIsInWhiteMacList(macValue.TextValue))
                {
                    return $"macAddress:[{macValue.TextValue}] invaild";
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
    }
}