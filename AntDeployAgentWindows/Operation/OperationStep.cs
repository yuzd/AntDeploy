using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Operation
{
    enum OperationStep
    {
        NONE,
        BACKEDUP,
        STOPPED,
        DEPLOYED,
        STARTED
    }
}
