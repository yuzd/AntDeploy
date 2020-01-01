using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDeployCommand.Model;
using AntDeployCommand.Utils;
using LibGit2Sharp;

namespace AntDeployCommand.Operations
{
    public class IISDEPLOY : OperationsBase
    {
        public override string ValidateArgument()
        {
            if (string.IsNullOrEmpty(Arguments.WebSiteName))
            {
                return $"{Name}{nameof(Arguments.WebSiteName)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.Host))
            {
                return $"{Name}{nameof(Arguments.Host)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.Token))
            {
                return $"{Name}{nameof(Arguments.Token)} required!";
            }
            if (string.IsNullOrEmpty(Arguments.PackageZipPath))
            {
                return $"{Name}{nameof(Arguments.PackageZipPath)} required!";
            }
            if (!File.Exists(Arguments.PackageZipPath))
            {
                return $"{Name}{nameof(Arguments.PackageZipPath)} not found!";
            }
            if (string.IsNullOrEmpty(Arguments.LoggerId))
            {
                Arguments.LoggerId = Guid.NewGuid().ToString("N");
            }
            if (string.IsNullOrEmpty(Arguments.DeployFolderName))
            {
                Arguments.DeployFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            return string.Empty;
        }
        long ProgressPercentage = 0;
        public override async Task<bool> Run()
        {
            byte[] zipBytes = File.ReadAllBytes(Arguments.PackageZipPath);
            if (zipBytes.Length < 1)
            {
                Error("package file is empty");
                return await Task.FromResult(false);
            }

            this.Info($"Start Uppload,Host:{Arguments.Host}");

            HttpRequestClient httpRequestClient = new HttpRequestClient();

            httpRequestClient.SetFieldValue("publishType", "iis");
            httpRequestClient.SetFieldValue("isIncrement", (Arguments.IsSelectedDeploy || Arguments.IsIncrementDeploy)?"true":"false");
            httpRequestClient.SetFieldValue("useOfflineHtm", (Arguments.UseAppOffineHtm)?"true":"false");
            httpRequestClient.SetFieldValue("sdkType", "netcore");
            httpRequestClient.SetFieldValue("port", Arguments.Port);
            httpRequestClient.SetFieldValue("id", Arguments.LoggerId);
            httpRequestClient.SetFieldValue("remark", Arguments.Remark);
            httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
            httpRequestClient.SetFieldValue("pc", string.IsNullOrEmpty(Arguments.Email) ? System.Environment.MachineName : Arguments.Email);
            httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
            httpRequestClient.SetFieldValue("poolName", Arguments.PoolName);
            httpRequestClient.SetFieldValue("physicalPath", Arguments.PhysicalPath);
            httpRequestClient.SetFieldValue("webSiteName", Arguments.WebSiteName);
            httpRequestClient.SetFieldValue("deployFolderName", Arguments.DeployFolderName);
            httpRequestClient.SetFieldValue("Token", Arguments.Token);
            httpRequestClient.SetFieldValue("backUpIgnore", (Arguments.BackUpIgnore != null && Arguments.BackUpIgnore.Any()) ? string.Join("@_@", Arguments.BackUpIgnore) : "");
            httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);


            HttpLogger HttpLogger = new HttpLogger
            {
                Key = Arguments.LoggerId,
                Url = $"http://{Arguments.Host}/logger?key=" + Arguments.LoggerId
            };

            //IDisposable _subcribe = null;
            WebSocketClient webSocket = new WebSocketClient(this.Log, HttpLogger);
            var isSuccess = true;
            try
            {

                var hostKey = await webSocket.Connect($"ws://{Arguments.Host}/socket");

                httpRequestClient.SetFieldValue("wsKey", hostKey);

                var uploadResult = await httpRequestClient.Upload($"http://{Arguments.Host}/publish",ClientOnUploadProgressChanged, GetProxy());

                if (ProgressPercentage == 0)
                {
                    isSuccess = false;
                }
                else
                {
                    webSocket.ReceiveHttpAction(true);
                    if (webSocket.HasError)
                    {
                        this.Error($"Host:{Arguments.Host},Deploy Fail,Skip to Next");
                        isSuccess = false;
                    }
                    else
                    {
                        if (uploadResult.Item1)
                        {
                            this.Info($"【deploy success】Host:{Arguments.Host},Response:{uploadResult.Item2}");
                        }
                        else
                        {
                            isSuccess = false;
                            this.Error($"Host:{Arguments.Host},Response:{uploadResult.Item2},Skip to Next");
                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.Error($"Fail Deploy,Host:{Arguments.Host},Response:{ex.Message},Skip to Next");
            }
            finally
            {
                await webSocket?.Dispose();
                //_subcribe?.Dispose();
            }

            return await Task.FromResult(isSuccess);
        }

        private void ClientOnUploadProgressChanged(long progress)
        {
            if (progress > ProgressPercentage)
            {
                ProgressPercentage = progress;
                this.Info($"【Upload progress】 {progress} %");
            }
        }
    }
}
