using System;
using System.Collections.Generic;
using System.Text;

namespace AntDeployCommand.Interface
{
    public interface IOperations
    {

        void ValidateArguments();

        void Execute();

    }
}
