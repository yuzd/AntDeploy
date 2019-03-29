using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployWinform.Models
{
    public class GetVersionResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public List<string> Data { get; set; }
    }

}
