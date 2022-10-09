using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;

namespace AntDeployAgentWindows.MyApp
{
    public class LoggerService: BaseWebApi
    {
        public static readonly ConcurrentDictionary<string,List<LoggerModel>>  loggerCollection = new ConcurrentDictionary<string, List<LoggerModel>>();
        private static readonly ConcurrentQueue<string> removeLoggerConllection = new ConcurrentQueue<string>();


        private static readonly System.Threading.Timer mDetectionTimer;
        private static readonly object lockObject = new  object();

        static LoggerService()
        {
            mDetectionTimer = new System.Threading.Timer(OnVerifyClients , null, 1000*60*10, 1000*60*10);
        }

        protected override void ProcessRequest()
        {
            Response.ContentType = "text/plain";
            try
            {
                if (Request.Method.ToUpper() != "GET")
                {
                    Response.Write("");
                    return;
                }

                var key = Request.Query.Get("key");
                if (string.IsNullOrEmpty(key))
                {
                    Response.Write("key required");
                    return;
                }

                var now = DateTime.Now;
                lock (lockObject)
                {
                    if (loggerCollection.TryGetValue(key, out List<LoggerModel> logList))
                    {
                        if (logList != null)
                        {
                            var result = logList.Where(r => !r.IsActive && r.Date <= now).ToList();
                            if (result.Any())
                            {
                                result.ForEach(r => r.IsActive = true);
                                Response.StatusCode = 200;
                                Response.Write(JsonConvert.SerializeObject(result));
                                return;
                            }
                        }
                    }
                }

                Response.Write("");
            }
            catch (Exception)
            {
                Response.Write("");
            }
        }


        public static void Remove(string key)
        {
            try
            {
                removeLoggerConllection.Enqueue(key);
            }
            catch (Exception)
            {

              //igrnoe
            }
        }

        private static void OnVerifyClients(object state)
        {
            try
            {
                lock (lockObject)
                {
                    mDetectionTimer.Change(-1, -1);
                    try
                    {
                        if (removeLoggerConllection.TryDequeue(out var key))
                        {
                            loggerCollection.TryRemove(key, out _);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        mDetectionTimer.Change(1000 * 60 * 10, 1000 * 60 * 10);
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
