using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDeployAgentWindows.Operation
{
    interface IOperations
    {
        void ValidateArguments();
        void Backup();
        void Restore();
        void Stop();
        void Deploy();
        void Start();
        void Execute();
        void Rollback();
    }
}
