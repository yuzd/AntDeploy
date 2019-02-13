using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
