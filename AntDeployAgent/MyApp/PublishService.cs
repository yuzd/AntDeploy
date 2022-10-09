using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntDeployAgentWindows.MyApp
{
    public class PublishService : BaseWebApi
    {
        protected override void ProcessRequest()
        {

            if (!CheckToken())
            {
                return;
            }



            FormHandler formHandler = new FormHandler(Context);


            var token = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("Token"));
            if (token == null || string.IsNullOrEmpty(token.TextValue) || (!string.IsNullOrEmpty(Token)&&!token.TextValue.Equals(Token)))
            {
                WriteError("token required");
                return;
            }

            var publishType = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("publishType"));
            if (publishType == null || string.IsNullOrEmpty(publishType.TextValue))
            {
                WriteError("publishType required");
                return;
            }

            var file = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("publish"));
            if (file == null || file.FileBody == null || file.FileBody.Length < 1)
            {
                WriteError("file required");
                return;
            }


            var publisher = PublishProviderFactory.GetProcessor(publishType.TextValue);
            if (publisher == null)
            {
                WriteError($"publishType: {publishType.TextValue} is not supported");
                return;
            }

            var loggerKeyValue = string.Empty;
            var loggerKey = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("id"));
            if (loggerKey != null && !string.IsNullOrEmpty(loggerKey.TextValue))
            {
                loggerKeyValue = publisher.LoggerKey = loggerKey.TextValue;
                LoggerService.loggerCollection.TryAdd(publisher.LoggerKey, new List<LoggerModel>());
            }

            try
            {
                var checkResult = publisher.Check(formHandler);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    WriteError(checkResult);
                    return;
                }


                var publisResult = publisher.Deploy(file);
                if (!string.IsNullOrEmpty(publisResult))
                {
                    WriteError(publisResult);
                    return;
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(loggerKeyValue))
                {
                    LoggerService.Remove(loggerKeyValue);
                }
            }

            WriteSuccess();
        }


        private void WriteError(string errMsg)
        {
            DeployResult obj = new DeployResult();
            obj.Success = false;
            obj.Msg = errMsg;
            Response.ContentType = "application/json";
            Response.StatusCode = 200;
            Response.Write(JsonConvert.SerializeObject(obj));
        }

        private void WriteSuccess()
        {
            DeployResult obj = new DeployResult();
            obj.Success = true;
            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }
    }


}
