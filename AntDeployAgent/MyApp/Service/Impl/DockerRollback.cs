using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.WebApiCore;
using System.Runtime.InteropServices;
namespace AntDeployAgent.MyApp.Service.Impl
{
    public class DockerRollback : PublishProviderBasicAPI
    {

        public override string ProviderName => "docker";
        public override string ProjectName => "";
        public override string ProjectPublishFolder => "";
        public override string RollBack()
        {

            return string.Empty; 
        }


        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return null;
        }


        public override string CheckData(FormHandler formHandler)
        {



            return string.Empty;
        }
    }
}
