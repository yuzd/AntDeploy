using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntDeploy.Util
{
    public class WebUtil
    {
        public static bool IsHttpGetOk(string url, Logger logger)
        {
            try
            {
                HttpWebRequest WReq = (HttpWebRequest)WebRequest.Create(url);
                WReq.Method = "GET";
                WReq.Timeout = 10000;
                HttpWebResponse WResp = (HttpWebResponse)WReq.GetResponse();
                if (WResp != null)
                {
                    logger.Info($"Response StatusCode:{(int)WResp.StatusCode}");
                    if (((int)WResp.StatusCode)<400)
                    {
                        WResp.Close();
                        return true;
                    }
                    WResp.Close();
                }
            }
            catch (Exception ex1)
            {
                logger.Warn(ex1.Message);
                //ignore
            }
            return false;
        }

        public static async Task<T> HttpPostAsync<T>(string url, object json, Logger logger)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest WReq = (HttpWebRequest)WebRequest.Create(url);
                WReq.Method = "POST";
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
