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
                        GetIisVersionList(request.Name);
                        break;
                    case "winservice":
                        GetWindowsServiceVersionList(request.Name);
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

        private void GetWindowsServiceVersionList(string requestName)
        {
            var projectPath = Path.Combine(Setting.PublishWindowServicePathFolder, requestName);
            if (!Directory.Exists(projectPath))
            {
                WriteError("publisher folder not found:" + projectPath  + ",please deploy first!");
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
                    list.Add(new Tuple<string, DateTime>(itemD.Name, d));
                }
            }
            var result = list.OrderByDescending(r => r.Item2).Select(r => r.Item1).Skip(1).Take(10).ToList();
            WriteSuccess(result);
        }

        private void GetIisVersionList(string requestName)
        {
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
                WriteError("there is no rollback version yet in publisher folder:" + projectPath);
                return;
            }

            var list = new List<Tuple<string, DateTime>>();
            foreach (var item in all)
            {
                var itemD = new DirectoryInfo(item);
                if (DateTime.TryParseExact(itemD.Name, "yyyyMMddHHmmss", null, DateTimeStyles.None, out DateTime d))
                {
                    list.Add(new Tuple<string, DateTime>(itemD.Name, d));
                }
            }
            var result = list.OrderByDescending(r => r.Item2).Select(r => r.Item1).Skip(1).Take(10).ToList();
            WriteSuccess(result);

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
