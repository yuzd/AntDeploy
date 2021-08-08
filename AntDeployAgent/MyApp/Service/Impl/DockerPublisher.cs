using AntDeployAgentWindows.WebApiCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace AntDeployAgentWindows.MyApp.Service.Impl
{
    public class DockerPublisher : PublishProviderBasicAPI
    {

        public override string ProviderName => "docker";
        public override string ProjectName => "";
        public override string ProjectPublishFolder => "";

        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            return string.Empty;
        }






        public override string CheckData(FormHandler formHandler)
        {

            return string.Empty;
        }

    }
}
