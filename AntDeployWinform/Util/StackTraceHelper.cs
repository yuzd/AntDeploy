using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployWinform.Util
{
    public class StackTraceHelper
    {
        /// <summary>
        /// 获取当前堆栈的上级调用方法列表,直到最终调用者,只会返回调用的各方法,而不会返回具体的出错行数
        /// </summary>
        /// <returns></returns>
        public static string GetStackTraceModelName()
        {
            try
            {
                //当前堆栈信息
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                System.Diagnostics.StackFrame[] sfs = st.GetFrames(); //1代表上级，2代表上上级，以此类推
                if (sfs == null || sfs.Length <= 0)
                {
                    return "";
                }
                //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
                string filterdName = "ResponseWrite,ResponseWriteError,";
                string result = string.Empty;
                for (int i = 1; i < sfs.Length; ++i)
                {
                    //非用户代码,系统方法及后面的都是系统调用，不获取,用户代码调用结束
                    if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset())
                    {
                        break;
                    }
                    MethodBase method = sfs[i].GetMethod();
                    string methodName = method.Name;//方法名称
                    if (filterdName.Contains(methodName))
                    {
                        continue;
                    }
                    string className = "";
                    if (method.ReflectedType != null)
                    {
                        className = method.ReflectedType.Name;
                    }
                    //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0

                    result = className + "." + methodName + "()->" + result;
                }
                st = null;
                sfs = null;
                filterdName = null;
                return result.TrimEnd('-', '>');
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
