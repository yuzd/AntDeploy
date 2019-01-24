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
            CommandID webiisCommand = new CommandID(CommandGuids.guidDiffCmdSet, (int)CommandId.Web_IIS);
            OleMenuCommand sqlServer_jsCommand = new OleMenuCommand(CommandWebIIsInvoke, webiisCommand);
            sqlServer_jsCommand.BeforeQueryStatus += html_BeforeQueryStatus;
            _mcs.AddCommand(sqlServer_jsCommand);

            CommandID webserviceCommadn = new CommandID(CommandGuids.guidDiffCmdSet, (int)CommandId.Web_Service);
            OleMenuCommand mysql_jsCommand = new OleMenuCommand(CommandWebServiceInvoke, webserviceCommadn);
            mysql_jsCommand.BeforeQueryStatus += html_BeforeQueryStatus;
            _mcs.AddCommand(mysql_jsCommand);

        }


        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandWebIIsInvoke(object sender, EventArgs e)
        {
            try
            {

                //先pubslish

                //然后zip打包

                //然后传输zip到远程

                //拿到远程交互结果


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

        private void CommandWebServiceInvoke(object sender, EventArgs e)
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
