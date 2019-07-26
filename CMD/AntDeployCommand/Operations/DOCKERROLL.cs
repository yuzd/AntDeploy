using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;

namespace AntDeployCommand.Operations
{
    public class DOCKERROLL : OperationsBase
    {
        public override string ValidateArgument()
        {

            return string.Empty;
        }

        public override async Task Run()
        {

            await Task.CompletedTask;
        }
    }
}
