using System;
using System.Collections.Generic;
using System.Text;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;
using AntDeployCommand.Operations;

namespace AntDeployCommand
{
    class OperationsFactory
    {
        public static IOperations Create(Arguments args)
        {
            #region DeployType

            if (args.DeployType.Equals("IISDEPLOY"))
            {
                return new IISDEPLOY()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("IISROLL"))
            {
                return new IISROLL()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("DOCKERDEPLOY"))
            {
                return new DOCKERDEPLOY()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("DOCKERROLL"))
            {
                return new DOCKERROLL()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("SERVICEDEPLOY"))
            {
                return new SERVICEDEPLOY()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("SERVICEROLL"))
            {
                return new SERVICEROLL()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("BUILD"))
            {
                return new Build()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("PACKAGE"))
            {
                return new PACKAGE()
                {
                    Arguments = args,
                };
            }
            else if (args.DeployType.Equals("GETINCRE"))
            {
                return new GETINCRE()
                {
                    Arguments = args,
                };
            }
            else
            {
                throw new NotImplementedException("/deployType:" + (args.DeployType ?? string.Empty) + " is not implemented");
            }
           
            #endregion
        }
    }
}
