using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace AntDeployAgentWindows.WebApiCore
{
    /// <summary>
    /// POST数据处理类
    /// </summary>
   public class FormHandler
    {


        /// <summary>
        /// 表单元素对象
        /// </summary>
        public class FormItem
        {
            /// <summary>
            /// 字段名(表单域名称)
            /// </summary>
            public string FieldName;

            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName;

            /// <summary>
            /// 文件数据实体
            /// </summary>
            public byte[] FileBody;

            /// <summary>
            /// 文本内容
            /// </summary>
            public string TextValue;
        }


        /// <summary>
        /// 保存表单域的字典
        /// </summary>
        private IDictionary<string, FormItem> _formDict = new Dictionary<string, FormItem>();


        /// <summary>
        /// 表单字段名列表
        /// </summary>
        public List<string> FormNames
        {
            get { return _formDict.Keys.ToList(); }
        }

        /// <summary>
        /// 获得的表单对象列表
        /// </summary>
        public IList<FormItem> FormItems
        {
            get
            {
                if (_formDict.Count < 1) return new List<FormItem>();
                return _formDict.Values.ToList();
            }
        }

        /// <summary>
        /// 表单字段数量
        /// </summary>
        public int FieldCount { get { return _formDict.Count; } }


        /// <summary>
        /// 根据字段名获取表单域内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FormItem this[string name]
        {
            get
            {
                if (!_formDict.Keys.Contains(name)) return null;
                return _formDict[name];
            }
        }


        /// <summary>
        /// 实例化一个表单处理对象
        /// </summary>
        /// <param name="context"></param>
        public FormHandler(IOwinContext context)
        {
            if (context.Request.Method.ToUpper() != "POST") throw new Exception();
            var strLen = context.Request.Headers["Content-Length"];
            if (string.IsNullOrEmpty(strLen)) throw new Exception("Not 'Content-Length'");
            if (!uint.TryParse(strLen, out uint sumsize)) throw new Exception("Content-Length Error");
            if (string.IsNullOrEmpty(context.Request.ContentType)) throw new Exception("Error:ContentType is NULL.");


            //把数据读到缓冲区以备处理
            var buffer = new MemoryStream();
            var tmpbuffer = new byte[1024 * 64];
            uint nextsize = sumsize; //sumsize:POST正文的总长度

            //循环读取所有正文数据并写入内存流
            while (nextsize > 0)
            {
                var len = context.Request.Body.Read(tmpbuffer, 0, tmpbuffer.Length);
                if (len < 1) throw new Exception("Network Is Disconnect?");
                nextsize -= (uint)len;

                buffer.Write(tmpbuffer, 0, len);
            }

            //正文类型
            var contentTypes = context.Request.ContentType.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);


            //处理多实体
            /////////////////////////////

            if (contentTypes.Length > 1)
            {
                var ism = false;    //是否是多实体
                var boundar = "";   //分隔线
                foreach (var s in contentTypes)
                {
                    //Content-Type:multipart/form-data; boundary=abcdef
                    var ss = s.Trim().ToLower();
                    if (ss == "multipart/form-data") ism = true;
                    if (ss.StartsWith("boundary=")) boundar = "--" + s.Trim().Substring(9);
                }

                //如果是，处理
                if (ism && boundar != "")
                {
                    ParseMultipart(buffer.ToArray(), boundar);
                    return;
                }
            }

            //处理其它格式（x-www-form-urlencoded）
            /////////////////////////////
            ParseForm(buffer.ToArray(), null);

        }


        /// <summary>
        /// 处理多实体表单
        /// </summary>
        /// <param name="body">数据</param>
        /// <param name="boundar">分隔线</param>
        private unsafe void ParseMultipart(byte[] body, string boundar)
        {
            var byttag = Encoding.ASCII.GetBytes(boundar);

            //每个分隔线后正文起点位置
            var poslist = new List<int>();


            //找出每个元素的起点位置
            fixed (byte* bp = body, tp = byttag)
            {
                var keyNum = *(long*)tp;
                for (var i = 0; i < body.Length - byttag.Length; i++)
                {
                    var lngnum = *(long*)(bp + i);
                    if (lngnum == keyNum)
                    {
                        var strTag = Encoding.ASCII.GetString(body, i, byttag.Length);
                        if (strTag == boundar)
                        {
                            poslist.Add(i + byttag.Length + 2); //+2是跳过分隔线的回车
                            i += byttag.Length;
                        }

                        //结束标记
                        if (body[i + 1] == '-' && body[i + 2] == '-' && body[i + 3] == 0x0D && body[i + 4] == 0x0A) break;
                    }
                }
            }


            //如果元素为0不再继续处理
            if (poslist.Count < 2) return;

            //处理每一个元素
            for (var i = 0; i < poslist.Count - 1; i++)
            {
                var size = poslist[i + 1] - poslist[i] - byttag.Length - 4; //分隔线前后有回车符
                if (size <= 0) continue;

                var bytes = new byte[size];
                Buffer.BlockCopy(body, poslist[i], bytes, 0, size);

                ParseMulitPartItem(bytes);
            }

        }


        /// <summary>
        /// 处理一个MulitPart表单域
        /// </summary>
        /// <param name="body"></param>
        private void ParseMulitPartItem(byte[] body)
        {
            if (body.Length < 10) return;

            //每一节都是由一个“描述头+双回车+正文实体”组成

            //找出双回车起始位置
            var crlf2 = FindCrlf2(body);
            if (crlf2 < 1 || crlf2 >= body.Length - 4) return;

            //声明/说明部分（描述头）
            var bytHead = new byte[crlf2];
            Buffer.BlockCopy(body, 0, bytHead, 0, crlf2);

            //正文部分
            var bytBody = new byte[body.Length - crlf2 - 4];
            Buffer.BlockCopy(body, crlf2 + 4, bytBody, 0, bytBody.Length);


            //处理节点说明部分
            var strHandle = Encoding.ASCII.GetString(bytHead); //utf8?
            var strHandles = strHandle.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            var name = "";
            var fileName = "";
            foreach (var s in strHandles)
            {
                var ss = s.Trim();

                //解析 Content-Type
                //if (ss.StartsWith("Content-Type:")) {
                //    //Content-Type: text/plain; charset=UTF-8
                //    //Content-Type: application/octet-stream
                //    var ctyp = ss.Substring(13).Trim();
                //    var ctypItems = ctyp.Split(';', StringSplitOptions.RemoveEmptyEntries);
                //    foreach(){}
                //}

                //Content-Transfer-Encoding: binary
                //正文编码方式

                //关键看正文描述
                if (ss.StartsWith("Content-Disposition:"))
                {
                    var disp = ss.Substring(20).Trim();
                    var dispItems = disp.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in dispItems)
                    {
                        var itemx = item.Trim();
                        if (itemx.StartsWith("name=")) name = itemx.Substring(5).Trim(new[] { '\x20', '\"' });
                        if (itemx.StartsWith("filename=")) fileName = itemx.Substring(9).Trim(new[] { '\x20', '\"' });
                    }
                }
            }

            if (name == "") return;

            if (!string.IsNullOrEmpty(fileName))
            {
                //文件名不为空，表示是文件
                var v = new FormItem { FieldName = name, FileName = fileName, FileBody = bytBody };
                _formDict.Add(name, v);
            }
            else
            {
                //没有文件名，肯定是普通表单
                var txt = Encoding.UTF8.GetString(bytBody);
                var v = new FormItem { FieldName = name, TextValue = txt };
                _formDict.Add(name, v);
            }
            //Console.WriteLine("name:{0}, file:{1}", name, fileName);

        }


        /// <summary>
        /// 处理普通表单（键值对）
        /// </summary>
        /// <param name="body">正文数据</param>
        /// <param name="charset">目标数据字符集</param>
        void ParseForm(byte[] body, Encoding charset)
        {
            if (charset == null) charset = Encoding.UTF8;

            //ContentType = "application/x-www-form-urlencoded;charset=utf8"; 
            // name=value&name1=value1

            var src_string = Encoding.ASCII.GetString(body);
            var kvs = src_string.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var kv in kvs)
            {

                var pos = kv.IndexOf('=');
                if (pos < 1) continue;

                var key = kv.Substring(pos).Trim();
                if (string.IsNullOrEmpty(key)) continue;

                var val = kv.Substring(pos + 1).Trim();
                if (string.IsNullOrEmpty(val) == false && val.IndexOf("%") != -1) val = UrlHelper.UrlDecode(val, charset);

                var content = new FormItem { FieldName = key, TextValue = val };
                _formDict.Add(key, content);
            }
        }




        /// <summary>
        /// 双回车
        /// </summary>
        private static readonly byte[] _byt_crlf2 = new byte[] { 0x0d, 0x0a, 0x0d, 0x0a };

        /// <summary>
        /// 查找第一个又回车
        /// </summary>
        /// <param name="byts"></param>
        /// <returns></returns>
        private static unsafe int FindCrlf2(byte[] byts)
        {
            if (byts.Length < 4) return -1;
            fixed (byte* pcr = _byt_crlf2, bp = byts)
            {
                var v1 = *(int*)pcr;
                for (var i = 0; i < byts.Length - 3; i++)
                {
                    var v2 = *(int*)(bp + i);
                    if (v1 == v2) return i;
                }
            }
            return -1;
        }





    }
}
