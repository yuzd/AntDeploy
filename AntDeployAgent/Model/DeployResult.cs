using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Model
{
    public class DeployResult<T>: DeployResult
    {
       
        public T Data { get; set; }
    }

    public class DeployResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }

    }
}
