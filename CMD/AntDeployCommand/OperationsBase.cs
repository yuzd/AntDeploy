using System;
using System.Collections.Generic;
using System.Text;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;

namespace AntDeployCommand
{
    class OperationsBase : IOperations
    {
        protected Arguments args;
        public OperationsBase(Arguments args)
        {
            this.args = args;
        }
        public void ValidateArguments()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
