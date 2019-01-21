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
        SqlServer = 0x1047,
        Mysql = 0x1048
    }
}
