using System;
using System.IO;
using System.Threading.Tasks;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;

namespace AntDeployCommand
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length !=1)
            {
                LogHelper.Error("arguments required");
                return -1;
            }

            var file = args[0];
            if (string.IsNullOrEmpty(file))
            {
                LogHelper.Error("arguments required");
                return -1;
            }

            if (!File.Exists(file))
            {
                LogHelper.Error($" {file} not found");
                return -1;
            }

            var fileContext = File.ReadAllText(file);
            if (string.IsNullOrEmpty(fileContext))
            {
                LogHelper.Error($" {file} is empty");
                return -1;
            }

            Arguments arguments = fileContext.JsonToObject<Arguments>();
            if (arguments == null)
            {

                LogHelper.Error("arguments required");
                return -1;
            }
            if (string.IsNullOrEmpty(arguments.DeployType))
            {
                LogHelper.Error("arguments DeployType required");
                return -1;
            }

            IOperations ops = OperationsFactory.Create(arguments);

            try
            {
                ops.ValidateArguments();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                LogHelper.Error(ex.StackTrace);
                return -1;
            }

            await ops.Execute();
            return 0;
        }
    }
}
