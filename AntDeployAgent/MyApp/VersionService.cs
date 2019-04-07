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
                    WriteError("request body is empty");
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
                    case "winservice":
                        GetWindowsServiceVersionList(request);
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

        private void GetWindowsServiceVersionList(GetVersionVm request)
        {
            string requestName = request.Name;
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, requestName);
            if (!Directory.Exists(projectPath))
            {
                WriteError("publisher folder not found:" + projectPath + ",please deploy first!");
                return;
            }

            var all = Directory.GetDirectories(projectPath).ToList();
            if (all.Count < 2)
            {
                WriteError("there is no rollback version yet in publisher folder:" + projectPath);
                return;
            }

            var list = new List<Tuple<string, DateTime>>();
            foreach (var item in all)
            {
                var itemD = new DirectoryInfo(item);
                if (DateTime.TryParseExact(itemD.Name, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
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
                        list.Add(new Tuple<string, DateTime>(dataInfo, d));
                    }
                    else
                    {
                        list.Add(new Tuple<string, DateTime>(itemD.Name, d));
                    }
                }
            }

            var result = list.OrderByDescending(r => r.Item2).Select(r => r.Item1).Skip(1).Take(10).ToList();
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
            if (all.Count < 2)
            {
                //只有当前版本
                WriteError("there is no rollback version yet in publisher folder:" + projectPath);
                return;
            }


            var list = new List<Tuple<string, DateTime>>();
            foreach (var item in all)
            {
                var itemD = new DirectoryInfo(item);
                if (DateTime.TryParseExact(itemD.Name, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
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
                        list.Add(new Tuple<string, DateTime>(dataInfo, d));
                    }
                    else
                    {
                        list.Add(new Tuple<string, DateTime>(itemD.Name, d));
                    }
                }
            }

            //排除掉当前版本 然后拿最近的10条发布记录
            var result = list.OrderByDescending(r => r.Item2).Select(r => r.Item1).Skip(1).Take(10).ToList();
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
            Response.Write(JsonConvert.SerializeObject(obj));
        }

        private void WriteSuccess(List<string> data = null)
        {
            DeployResult<List<string>> obj = new DeployResult<List<string>>();
            obj.Success = true;
            obj.Data = data ?? new List<string>();
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }
    }
}