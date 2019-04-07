using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Model
{
    public class GetVersionVm
    {
        public string Token { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool WithArgs { get; set; }
    }
}
