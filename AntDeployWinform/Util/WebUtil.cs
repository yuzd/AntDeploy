using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntDeployWinform.Models;

namespace AntDeployWinform.Util
{
    public class WebUtil
    {
        public static Encoding GetEncodingFrom(
        NameValueCollection responseHeaders,
        Encoding defaultEncoding = null)
        {
            try
            {
                if (responseHeaders == null)
                    return Encoding.UTF8;

                //Note that key lookup is case-insensitive
                var contentType = responseHeaders["Content-Type"];
                if (contentType == null)
                    return defaultEncoding;

                var contentTypeParts = contentType.Split(';');
                if (contentTypeParts.Length <= 1)
                    return defaultEncoding;

                var charsetPart =
                    contentTypeParts.Skip(1).FirstOrDefault(
                        p => p.TrimStart().StartsWith("charset", StringComparison.InvariantCultureIgnoreCase));
                if (charsetPart == null)
                    return defaultEncoding;

                var charsetPartParts = charsetPart.Split('=');
                if (charsetPartParts.Length != 2)
                    return defaultEncoding;

                var charsetName = charsetPartParts[1].Trim();
                if (charsetName == "")
                    return defaultEncoding;


                return Encoding.GetEncoding(charsetName);
            }
            catch (ArgumentException )
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// http://geekswithblogs.net/THines01/archive/2010/12/03/responsestatusline.aspx
        /// https://stackoverflow.com/questions/2482715/the-server-committed-a-protocol-violation-section-responsestatusline-error/3977369
        /// https://o2platform.wordpress.com/2010/10/20/dealing-with-the-server-committed-a-protocol-violation-sectionresponsestatusline/
        /// </summary>
        /// <returns></returns>
        public static bool SetAllowUnsafeHeaderParsing20()
        {
            try
            {
                //ServicePointManager.Expect100Continue = false;
                //ServicePointManager.MaxServicePointIdleTime = 2000;

                //Get the assembly that contains the internal class
                Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
                if (aNetAssembly != null)
                {
                    //Use the assembly in order to get the internal type for the internal class
                    Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                    if (aSettingsType != null)
                    {
                        //Use the internal static property to get an instance of the internal settings class.
                        //If the static instance isn't created allready the property will create it for us.
                        object anInstance = aSettingsType.InvokeMember("Section",
                            BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
                        if (anInstance != null)
                        {
                            //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                            FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (aUseUnsafeHeaderParsing != null)
                            {
                                aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static bool IsHttpGetOk(string url, Logger logger,string agentNewVersion = null)
        {
            var retyrTimes = 0;
            var needLog = agentNewVersion != null;
            RETRY:
            var needRetry = false;
            try
            {

                HttpWebRequest WReq = (HttpWebRequest)WebRequest.Create(url);

                if (url.StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                WReq.Method = "GET";
                WReq.Timeout = 20000;
                WReq.ReadWriteTimeout = 20000;
                WReq.KeepAlive = false;
                WReq.AllowAutoRedirect = false;
                HttpWebResponse WResp = (HttpWebResponse)WReq.GetResponse();
                //if (WResp.StatusCode == HttpStatusCode.Redirect || WResp.StatusCode == HttpStatusCode.MovedPermanently)
                //{
                //    logger.Warn($"Fire WebSite Url Fail: the page was redirected!");
                //}
                
                logger.Info($"Fire WebSite Url Response StatusCode:{(int)WResp.StatusCode}");

                if (needLog && (((int)WResp.StatusCode) == 200))
                {
                    using (Stream stream = WResp.GetResponseStream())
                    {
                        var reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(result))
                        {
                            WResp.Close();

                            if (agentNewVersion.StartsWith(result))
                            {
                                logger.Info("Website Response:" + result);
                                return true;
                            }
                            else
                            {
                                throw new Exception("agent check fail");
                            }
                        }
                        else
                        {
                            throw new Exception("agent check fail");
                        }
                    }
                }

                if (((int)WResp.StatusCode) < 400)
                {
                    WResp.Close();
                    return true;
                }
                WResp.Close();
            }
            catch (Exception ex1)
            {
                retyrTimes++;
                
                if (retyrTimes > 6)
                {
                    logger.Warn($"Fire WebSite Url Fail:{ex1.Message}");
                }
                else
                {
                    logger.Warn($"Fire WebSite Url Fail:{ex1.Message}，Retry:{retyrTimes}");
                    needRetry = true;
                }
                //ignore
            }
            if (needRetry)
            {
                if (needLog)
                {
                    Thread.Sleep(2000);
                }
                goto RETRY;
            }
            return false;
        }

        public static async Task<T> HttpPostAsync<T>(string url, object json, Logger logger, bool ignore = false) 
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
                    try
                    {
                        var rt = JsonConvert.DeserializeObject<T>(result);
                        return rt;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            if (ignore)
                            {
                                return default(T);
                            }

                            var rr = JsonConvert.DeserializeObject<GetVersionResult>(result);
                            if (rr != null && !string.IsNullOrEmpty(rr.Msg))
                            {
                                logger.Error(rr.Msg);
                                return default(T);
                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                        logger.Error(result);
                    }
                }

                return default(T);
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
