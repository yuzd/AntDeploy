using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yuzd.AntDeploy
{
    public class MsBuildHelper
    {
        public  string GetMsBuildPath()
        {
            try
            {
                var getmS = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToBuildTools(Microsoft.Build.Utilities.ToolLocationHelper.CurrentToolsVersion);
                return getmS;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
