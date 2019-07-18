using System;
using System.Collections.Generic;
using System.Text;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;

namespace AntDeployCommand
{
    class OperationsFactory
    {
        public static IOperations Create(Arguments args)
        {
            switch (args.DeployType)
            {
                
                default:
                    throw new NotImplementedException("/deployType:" + (args.DeployType ?? string.Empty) + " is not implemented");
            }
        }
    }
}
