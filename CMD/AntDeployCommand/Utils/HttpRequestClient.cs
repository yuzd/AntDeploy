using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployCommand.Utils
{
    /// <summary>
    /// description：http post请求客户端
    /// last-modified-date：2012-02-28
    /// </summary>
    public class HttpRequestClient
    {
        #region //字段
        private ArrayList bytesArray;
        private Encoding encoding = Encoding.UTF8;
        private string boundary = String.Empty;
        #endregion

        #region //构造方法
        public HttpRequestClient()
        {
            bytesArray = new ArrayList();
            string flag = DateTime.Now.Ticks.ToString("x");
            //boundary = "---------------------------" + flag;
            boundary = flag;
        }
        #endregion

        #region //方法
        /// <summary>
        /// 合并请求数据
        /// </summary>
        /// <returns></returns>
        private byte[] MergeContent()
        {
            int length = 0;
            int readLength = 0;
            string endBoundary = "--" + boundary + "--\r\n";
            byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);

            bytesArray.Add(endBoundaryBytes);

            foreach (byte[] b in bytesArray)
            {
                length += b.Length;
            }

            byte[] bytes = new byte[length];

            foreach (byte[] b in bytesArray)
            {
                b.CopyTo(bytes, readLength);
                readLength += b.Length;
            }

            return bytes;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> Upload(String requestUrl, Action<long> progress, IWebProxy proxy = null)
        {
            byte[] bytes = MergeContent();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.Timeout = 1000*60*30;//30分钟
                request.ReadWriteTimeout = 1000*60*30;//30分钟
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Method = "POST";
                request.AllowWriteStreamBuffering = false;
                request.AllowReadStreamBuffering = false;
                request.ContentLength = bytes.Length;
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                using (var req = await request.GetRequestStreamAsync())
                {
                    var bytesArr = BufferSplit(bytes, 4096);
                    var i = 0;
                    foreach (var arr in bytesArr)
                    {
                        await req.WriteAsync(arr, 0, arr.Length);
                        i += arr.Length;
                        await req.FlushAsync(); // flushing is required or else we jump to 100% very fast
                        progress?.Invoke((int)(100.0 * i / bytes.Length));
                    }
                }

                HttpWebResponse WResp = (HttpWebResponse)request.GetResponse();
                using (Stream stream = WResp.GetResponseStream())
                {
                    var reader = new StreamReader(stream);
                    var responseText = reader.ReadToEnd();


                    if (!string.IsNullOrEmpty(responseText))
                    {
                        var model = responseText.JsonToObject<DeployResult>();
                        if (model == null)
                        {
                            return new Tuple<bool, string>(false, responseText);
                        }

                        if (model.Success)
                        {
                            return new Tuple<bool, string>(true, "Deploy Success");
                        }
                        else
                        {
                            return new Tuple<bool, string>(false, responseText);
                        }
                    }
                }
                WResp.Close();
                return new Tuple<bool, string>(true, "Deploy Fail");

            }
            catch (WebException ex)
            {
                try
                {
                    Stream responseStream = ex.Response.GetResponseStream();
                    byte[] responseBytes = new byte[ex.Response.ContentLength];
                    responseStream?.Read(responseBytes, 0, responseBytes.Length);
                    var responseText2 = System.Text.Encoding.UTF8.GetString(responseBytes);
                    return new Tuple<bool, string>(false, responseText2 + "==>exception:" + ex.Message);
                }
                catch (Exception)
                {
                    return new Tuple<bool, string>(false, ex.Message);
                }
            }
            catch (Exception ex1)
            {
                return new Tuple<bool, string>(false, ex1.Message);
            }
            finally
            {
                bytesArray = null;
            }
        }
        public static byte[][] BufferSplit(byte[] buffer, int blockSize)
        {
            byte[][] blocks = new byte[(buffer.Length + blockSize - 1) / blockSize][];

            for (int i = 0, j = 0; i < blocks.Length; i++, j += blockSize)
            {
                blocks[i] = new byte[Math.Min(blockSize, buffer.Length - j)];
                Array.Copy(buffer, j, blocks[i], 0, blocks[i].Length);
            }

            return blocks;
        }

        /// <summary>
        /// 设置表单数据字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        public void SetFieldValue(String fieldName, String fieldValue)
        {
            string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            string httpRowData = String.Format(httpRow, fieldName, fieldValue);

            bytesArray.Add(encoding.GetBytes(httpRowData));
        }

        /// <summary>
        /// 设置表单文件数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="filename">字段值</param>
        /// <param name="contentType">内容内型</param>
        /// <param name="fileBytes">文件字节流</param>
        /// <returns></returns>
        public void SetFieldValue(String fieldName, String filename, String contentType, Byte[] fileBytes)
        {
            string end = "\r\n";
            string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string httpRowData = String.Format(httpRow, fieldName, filename, contentType);

            byte[] headerBytes = encoding.GetBytes(httpRowData);
            byte[] endBytes = encoding.GetBytes(end);
            byte[] fileDataBytes = new byte[headerBytes.Length + fileBytes.Length + endBytes.Length];

            headerBytes.CopyTo(fileDataBytes, 0);
            fileBytes.CopyTo(fileDataBytes, headerBytes.Length);
            endBytes.CopyTo(fileDataBytes, headerBytes.Length + fileBytes.Length);

            bytesArray.Add(fileDataBytes);
        }
        #endregion
    }

    public class AntDeplopyWebClient : System.Net.WebClient
    {

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            if (lWebRequest is HttpWebRequest)
            {

                ((HttpWebRequest)lWebRequest).KeepAlive = false;
                ((HttpWebRequest)lWebRequest).ServicePoint.Expect100Continue = false;
            }
            return lWebRequest;
        }
    }
    public class DeployResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }

    }
}
