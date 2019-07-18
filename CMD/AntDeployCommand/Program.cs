using System;
using AntDeployCommand.Interface;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;

namespace AntDeployCommand
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length !=1)
            {
                Console.WriteLine("arguments required");
                return -1;
            }

            Arguments arguments = args[0].JsonToObject<Arguments>();
            if (arguments == null)
            {

                Console.WriteLine("arguments required");
                return -1;
            }
            if (string.IsNullOrEmpty(arguments.DeployType))
            {

                Console.WriteLine("arguments DeployType required");
                return -1;
            }

            IOperations ops = OperationsFactory.Create(arguments);

            try
            {
                ops.ValidateArguments();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            ops.Execute();
            return 0;
        }
    }
}
