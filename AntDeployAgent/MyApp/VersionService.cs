using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.Util;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AntDeployAgent.Model;
using AntDeployAgent.Util;

namespace AntDeployAgentWindows.MyApp
{
    public class VersionService : BaseWebApi
    {
        protected override void ProcessRequest()
        {
            try
            {
                var body = string.Empty;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    body = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(body))
                {
                    Response.ContentType = "text/plain";
                    Response.Write(Version.VERSION);
                    return;
                }

                var request = JsonConvert.DeserializeObject<GetVersionVm>(body);
                if (request == null)
                {
                    WriteError("request body is invaild");
                    return;
                }

                if (string.IsNullOrEmpty(request.Token) || (!string.IsNullOrEmpty(Token) && !request.Token.Equals(Token)))
                {
                    WriteError("request Token is invaild");
                    return;
                }

                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Type))
                {
                    WriteError("request param is invaild");
                    return;
                }

                switch (request.Type.ToLower())
                {
                    case "iis":
                        GetIisVersionList(request);
                        break;
                    case "checkiis":
                        CheckIIs(request);
                        break;
                    case "winservice":
                        GetWindowsServiceVersionList(request);
                        break;
                    case "checkwinservice":
                        CheckWinservice(request);
                        break;
                    default:
                        WriteError("request Type is invaild");
                        return;
                }
            }
            catch (System.Exception ex)
            {
                WriteError(ex.Message);
            }
        }

        /// <summary>
        /// 检查windows服务是否已存在
        /// </summary>
        /// <param name="request"></param>
        private void CheckWinservice(GetVersionVm request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                WriteError("service name required!");
                return;
            }

            if (!string.IsNullOrEmpty(request.Mac) && !Setting.CheckIsInWhiteMacList(request.Mac))
            {
                WriteError($"macAddress:[{request.Mac}] invalid");
                return;
            }

            var serviceName = request.Name.Trim();
            var service = WindowServiceHelper.GetWindowServiceByName(serviceName);

            if (!string.IsNullOrEmpty(service.Item2))
            {
                WriteError(service.Item2);
                return;
            }

            CheckExistResult result = new CheckExistResult {WebSiteName = serviceName, Success = service.Item1!=null};
            WriteSuccess(result);
        }

        /// <summary>
        /// 检查IIS中是否存在指定网站
        /// </summary>
        /// <param name="request"></param>
        private void CheckIIs(GetVersionVm request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                WriteError("web site name required!");
                return;
            }

            if (!string.IsNullOrEmpty(request.Mac) && !Setting.CheckIsInWhiteMacList(request.Mac))
            {
                WriteError($"macAddress:[{request.Mac}] invalid");
                return;
            }

            var webSiteName = request.Name.Trim();
            var siteNameArr = webSiteName.Split('/');
            if (siteNameArr.Length > 2)
            {
                WriteError("webSiteName level limit is 2");
                return;
            }

            var level1 = siteNameArr[0];
            var level2 = siteNameArr.Length == 2 ? siteNameArr[1] : string.Empty;

            var isSiteExistResult = IISHelper.IsSiteExist(level1, level2);
            if (!string.IsNullOrEmpty(isSiteExistResult.Item3))
            {
                WriteError(isSiteExistResult.Item3);
                return;
            }

            var iisVersion = IISHelper.GetIISVersion();
            if (iisVersion <= 6)
            {
                WriteError($"remote iis verison is too low!");
                return;
            }

            CheckExistResult result = new CheckExistResult();
            result.WebSiteName = webSiteName;
            result.Level1Name = level1;
            result.Level1Exist = isSiteExistResult.Item1;
            if (!isSiteExistResult.Item1)
            {
                //一级不存在 那肯定要输入了 端口号必填
                result.Level1Exist = false;
            }
            else if (isSiteExistResult.Item1 && !isSiteExistResult.Item2 && !string.IsNullOrEmpty(level2))
            {
                //一级存在二级不存在 不用填端口号
                result.Level2Exist =false;
            }
            else
            {
                result.Success = true;
            }

            WriteSuccess(result);
        }

        private void GetWindowsServiceVersionList(GetVersionVm request)
        {
            if (!string.IsNullOrEmpty(request.Mac) && !Setting.CheckIsInWhiteMacList(request.Mac))
            {
                WriteError($"macAddress:[{request.Mac}] invalid");
                return;
            }

            string requestName = request.Name;
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, requestName);
            if (!Directory.Exists(projectPath))
            {
                WriteError("publisher folder not found:" + projectPath + ",please deploy first!");
                return;
            }

            var all = Directory.GetDirectories(projectPath).ToList();
            if (all.Count < 1)
            {
                WriteError("there is no rollback version yet in publisher folder:" + projectPath);
                return;
            }

            var dic = new Dictionary<string,Tuple<string,DateTime,string>>();
            foreach (var item in all)
            {
                var itemD = new DirectoryInfo(item);
                var temp = itemD.Name.Replace("_", "");
                if (DateTime.TryParseExact(temp, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
                {
                    if (request.WithArgs)
                    {
                        var args = GetParamInArgsFile(request, item);
                        var data = new
                        {
                            Version = itemD.Name,
                            Args = args
                        };
                        var dataInfo = JsonConvert.SerializeObject(data);
                        if (dic.ContainsKey(temp))
                        {
                            //是重试版本 看下已存在的length是否
                            var infoValue = dic[temp];
                            if (infoValue.Item3.Length < itemD.Name.Length)
                            {
                                //是旧的 替换掉
                                dic[temp] = new Tuple<string, DateTime, string>(dataInfo, d, itemD.Name);
                            }
                        }
                        else
                        {
                            //添加
                            dic.Add(temp,new Tuple<string, DateTime,string>(dataInfo, d,itemD.Name));
                        }
                    }
                    else
                    {
                        dic.Add(temp,new Tuple<string, DateTime,string>(itemD.Name, d,itemD.Name));
                    }
                }
            }

            var result = dic.Values.ToList().OrderByDescending(r => r.Item2).Select(r => r.Item1).Take(11).ToList();
            WriteSuccess(result);
        }

        private void GetIisVersionList(GetVersionVm request)
        {
            string requestName = request.Name;
            var siteNameArr = requestName.Split('/');
            if (siteNameArr.Length > 2)
            {
                WriteError("webSiteName not found");
                return;
            }

            var projectName = IISHelper.GetCorrectFolderName(requestName);
            var projectPath = Path.Combine(Setting.PublishIIsPathFolder, projectName);
            if (!Directory.Exists(projectPath))
            {
                WriteError("publisher folder not found:" + projectPath + ",please deploy first!");
                return;
            }

            var all = Directory.GetDirectories(projectPath).ToList();
            if (all.Count < 1)
            {
                //只有当前版本
                WriteError("there is no rollback version yet in publisher folder:" + projectPath);
                return;
            }

            var dic = new Dictionary<string,Tuple<string,DateTime,string>>();
            foreach (var item in all)
            {
                var itemD = new DirectoryInfo(item);
                var temp = itemD.Name.Replace("_", "");
                if (DateTime.TryParseExact(temp, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
                {
                    if (request.WithArgs)
                    {
                        var args = GetParamInArgsFile(request, item);
                        var data = new
                        {
                            Version = itemD.Name,
                            Args = args
                        };
                        var dataInfo = JsonConvert.SerializeObject(data);
                        if (dic.ContainsKey(temp))
                        {
                            //是重试版本 看下已存在的length是否
                            var infoValue = dic[temp];
                            if (infoValue.Item3.Length < itemD.Name.Length)
                            {
                                //是旧的 替换掉
                                dic[temp] = new Tuple<string, DateTime, string>(dataInfo, d, itemD.Name);
                            }
                        }
                        else
                        {
                            //添加
                            dic.Add(temp,new Tuple<string, DateTime,string>(dataInfo, d,itemD.Name));
                        }
                    }
                    else
                    {
                        dic.Add(temp,new Tuple<string, DateTime,string>(itemD.Name, d,itemD.Name));
                    }
                }
            }

            //排除掉当前版本 然后拿最近的10条发布记录
            var result = dic.Values.ToList().OrderByDescending(r => r.Item2).Select(r => r.Item1).Take(11).ToList();
            WriteSuccess(result);
        }


        private string GetParamInArgsFile(GetVersionVm request, string folder)
        {
            try
            {
                string path = Path.Combine(folder, "antdeploy_args.json");
                if (!File.Exists(path))
                {
                    return String.Empty;
                }

                var content = File.ReadAllText(path, Encoding.UTF8);
                return content;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        private void WriteError(string errMsg)
        {
            DeployResult<List<string>> obj = new DeployResult<List<string>>();
            obj.Success = false;
            obj.Msg = errMsg;
            obj.Data = new List<string>();
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write(JsonConvert.SerializeObject(obj));
        }

        private void WriteSuccess(List<string> data = null)
        {
            DeployResult<List<string>> obj = new DeployResult<List<string>>();
            obj.Success = true;
            obj.Data = data ?? new List<string>();
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }

        private void WriteSuccess<T>(T data)
        {
            DeployResult<T> obj = new DeployResult<T>();
            obj.Success = true;
            obj.Data = data;
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }
    }
}