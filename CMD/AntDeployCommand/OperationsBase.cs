using System;
using System.Collections.Generic;
using System.Text;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;

namespace AntDeployCommand
{
    public abstract class OperationsBase : IOperations
    {
      
        public Arguments Arguments;



        public abstract string ValidateArgument();
        public abstract void Run();

        public void ValidateArguments()
        {
        

            var re = ValidateArgument();
            if (!string.IsNullOrEmpty(re))
            {
                throw new ArgumentException(re);
            }
        }

        public void Execute()
        {
            Run();
        }

    }

}
