using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace AntDeploy.Util
{
    public class WebUtil
    {
        public static async Task<T> HttpPostAsync<T>(string url, object json, Logger logger)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest WReq = (HttpWebRequest)WebRequest.Create(url);
                WReq.Method ="POST";
                WReq.Timeout = 5000;
                var st = JsonConvert.SerializeObject(json);
                byte[] byteArray = Encoding.UTF8.GetBytes(st);
                WReq.ContentType = "application/json";
                WReq.ContentLength = byteArray.Length;
                using (var newStream = await WReq.GetRequestStreamAsync())
                {
                    await newStream.WriteAsync(byteArray, 0, byteArray.Length);
                }
                HttpWebResponse WResp = (HttpWebResponse)await WReq.GetResponseAsync();
                if (WResp != null)
                {
                    Stream stream = WResp.GetResponseStream();
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        result = await reader.ReadToEndAsync();
                        reader.Close();
                        stream.Close();
                    }
                    WResp.Close();
                }
                if (!string.IsNullOrEmpty(result))
                {
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (Exception ex1)
            {
                logger.Error(ex1.Message);
                //ignore
            }
            return default(T);
        }
    }
}
