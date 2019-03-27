using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AntDeployAgentWindows.Model;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace AntDeployAgentWindows.WebApiCore
{
    /// <summary>
    /// Fast WebApi基类
    /// </summary>
    public abstract class BaseWebApi
    {

        static BaseWebApi()
        {
            Token = System.Configuration.ConfigurationManager.AppSettings["Token"];
        }

        protected static  string Token;

        #region <子类可见的保护对象>

        /// <summary>
        /// HTML 请求（输入）对象
        /// </summary>
        protected IOwinRequest Request;

        /// <summary>
        /// HTML 应答（输出）对象
        /// </summary>
        protected IOwinResponse Response;

        /// <summary>
        /// OWIN上下文对象
        /// </summary>
        protected IOwinContext Context;



        #endregion

        public bool CheckToken()
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
                        return false;
                    }

                    if (!Token.Equals(token))
                    {
                        token = WebUtility.UrlDecode(token);
                        if (!Token.Equals(token))
                        {
                            Response.Write("token invaid");
                            return false;
                        }
                    }

                }


                Response.Write("success");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ProcessRequest(IOwinContext context)
        {

            Context = context;
            Response = context.Response;
            Request = context.Request;
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    ProcessRequest();
                }
                catch (Exception ex)
                {
                    DeployResult obj = new DeployResult {Success = false, Msg = ex.Message};
                    Response.ContentType = "application/json";
                    Response.Write(JsonConvert.SerializeObject(obj));
                }
            });
        }

        /// <summary>
        /// 具体处理请求
        /// </summary>
        protected abstract void ProcessRequest();

    }
}
