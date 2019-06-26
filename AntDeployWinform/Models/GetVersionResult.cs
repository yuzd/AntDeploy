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

    public class IIsSiteCheck
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public IIsSiteCheckResult Data { get; set; }
    }
    public class IIsSiteCheckResult
    {

        public string WebSiteName { get; set; }

        public string Level1Name { get; set; }
        public bool Level1Exist { get; set; }
        public bool Success { get; set; }

        public bool Level2Exist { get; set; }
    }

    public class FirstCreateParam
    {
        public string Port { get; set; }
        public string PoolName { get; set; }
        public string PhysicalPath { get; set; }
        public string Desc { get; set; }
        public string StartUp { get; set; }
    }
 

}