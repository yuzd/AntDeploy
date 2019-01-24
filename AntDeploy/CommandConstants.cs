using System;

namespace AntDeploy
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandGuids
    {
        public const string guidDiffCmdSetString = "042f3349-1f02-4304-b18f-e1ac836fb026";


        public static readonly Guid guidDiffCmdSet = new Guid(guidDiffCmdSetString);


    }


    enum CommandId
    {
        Web_IIS = 0x1047,
        Web_Service = 0x1048
    }
}
