using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployCommand.Interface
{
    public interface IOperations
    {

        void ValidateArguments();

        Task Execute();

        string Name { get; }
    }
}
