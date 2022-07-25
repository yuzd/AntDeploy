using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployWinform.Models
{
    /// <summary>
    /// 文件结构
    /// </summary>
    [Serializable]
    public class FileStruct
    {
        /// <summary>
        /// 文件绝对路径
        /// </summary>
        public string FileFullName { get; set; }
        /// <summary>
        /// 文件修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = new DateTime(1970, 1, 1);
        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath { get; set; }
        /// <summary>
        /// 是文件
        /// </summary>
        public bool IsFile { get; set; } = true;
    }
}
