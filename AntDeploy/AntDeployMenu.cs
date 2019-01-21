using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace AntDeploy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AntDeployMenu
    {


        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly DTE2 _dte;
        private readonly OleMenuCommandService _mcs;
        private readonly Func<string[], bool> _itemToHandleFunc;
        private Package _package;


        public AntDeployMenu(DTE2 dte, OleMenuCommandService mcs, Func<string[], bool> itemToHandleFunc, Package package)
        {

            _dte = dte;
            _mcs = mcs;
            _itemToHandleFunc = itemToHandleFunc;
            _package = package;
        }


        public void SetupCommands()
        {
            CommandID sqlserverCommand = new CommandID(CommandGuids.guidDiffCmdSet, (int)CommandId.SqlServer);
            OleMenuCommand sqlServer_jsCommand = new OleMenuCommand(CommandInvoke, sqlserverCommand);
            sqlServer_jsCommand.BeforeQueryStatus += html_BeforeQueryStatus;
            _mcs.AddCommand(sqlServer_jsCommand);

            CommandID mysqlCommadn = new CommandID(CommandGuids.guidDiffCmdSet, (int)CommandId.Mysql);
            OleMenuCommand mysql_jsCommand = new OleMenuCommand(CommandInvoke, mysqlCommadn);
            mysql_jsCommand.BeforeQueryStatus += html_BeforeQueryStatus;
            _mcs.AddCommand(mysql_jsCommand);

        }


        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandInvoke(object sender, EventArgs e)
        {
            try
            {

                MessageBox.Show("append success",
                    "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                ProjectHelpers.AddError(_package, ex.ToString());
                MessageBox.Show("Error happens: " + ex,
                    "Fail", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }






        void html_BeforeQueryStatus(object sender, System.EventArgs e)
        {
            OleMenuCommand oCommand = (OleMenuCommand)sender;

            oCommand.Visible = _itemToHandleFunc(new[] { ".json" });
        }




    }
}
