using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgent.Model
{
    public class CheckExistResult
    {

        public string WebSiteName { get; set; }

        public string Level1Name { get; set; }
        public bool Level1Exist { get; set; }
        public bool Success { get; set; }

        public bool Level2Exist { get; set; }
    }


}
