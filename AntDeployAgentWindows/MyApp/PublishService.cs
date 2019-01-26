using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.WebApiCore;
using Newtonsoft.Json;
using System.Linq;

namespace AntDeployAgentWindows.MyApp
{
    public class PublishService : BaseWebApi
    {
        private static readonly string Token;
        static PublishService()
        {
            Token = System.Configuration.ConfigurationManager.AppSettings["Token"];
        }
        protected override async void ProcessRequest()
        {

            if (Request.Method.ToUpper() != "POST")
            {
                Response.ContentType = "text/plain";

                if (!string.IsNullOrEmpty(Token))
                {
                    //验证Token
                    var token = Request.Query.Get("Token");
                    if (string.IsNullOrEmpty(token))
                    {
                        Response.Write("token required");
                        return;
                    }

                    if (!Token.Equals(token))
                    {
                        Response.Write("token invaid");
                        return;
                    }
                }


                Response.Write("success");
                return;
            }


            
            FormHandler formHandler = new FormHandler(Context);

            


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

            WriteSuccess();
        }


        private void WriteError(string errMsg)
        {
            DeployResult obj = new DeployResult();
            obj.Success = false;
            obj.Msg = errMsg;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }

        private void WriteSuccess()
        {
            DeployResult obj = new DeployResult();
            obj.Success = true;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(obj));
        }
    }


}
