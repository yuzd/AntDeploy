using AntDeployAgentWindows.Model;
using AntDeployAgentWindows.MyApp.Service;
using AntDeployAgentWindows.WebApiCore;
using System.IO;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using AntDeployAgent.Model;
using Newtonsoft.Json;
using AntDeployAgentWindows.Operation.OperationTypes;
using AntDeployAgentWindows.Operation;
using AntDeployAgentWindows.Util;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace AntDeployAgent.MyApp.Service.Impl
{
    public class DockerPublisher : PublishProviderBasicAPI
    {

        public override string ProviderName => "docker";
        public override string ProjectName => DockerParamModel.PorjectName;
        public override string ProjectPublishFolder => _projectPublishFolder;

        private string _projectPublishFolder; //发布目录
        private string _dateTimeFolderName; //版本
        private bool _isIncrement; //是否增量
        private string _physicalPath; //指定的创建的时候用的服务器路径
        private List<string> _backUpIgnoreList = new List<string>(); //需要排除backup的列表
        internal DockerParamModel DockerParamModel = new DockerParamModel();


        public override string DeployExcutor(FormHandler.FormItem fileItem)
        {
            var projectPath = Path.Combine(Setting.PublishDockerPathFolder, ProjectName);
            _projectPublishFolder = Path.Combine(projectPath,
                !string.IsNullOrEmpty(_dateTimeFolderName) ? _dateTimeFolderName : DateTime.Now.ToString("yyyyMMddHHmmss"));
            EnsureProjectFolder(projectPath);
            EnsureProjectFolder(_projectPublishFolder);
            var deployFolder = string.Empty;
            try
            {
                var _zipFile = Path.Combine(_projectPublishFolder, fileItem.FileName);
                using (var fs = new FileStream(_zipFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(fileItem.FileBody, 0, fileItem.FileBody.Length);
                }


                if (!File.Exists(_zipFile))
                {
                    return "publish file save fail";
                }
#if NETCORE
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Log("linux agent version ==>" + Version.VERSION);
                }
                else
                {
                    Log("netcore agent version ==>" + Version.VERSION);
                }

#else
                Log("netframework agent version ==>" + Version.VERSION);
#endif
                Log("upload success ==>" + _zipFile);
                //解压
                try
                {
                    Log("start unzip file");
                    ZipFile.ExtractToDirectory(_zipFile, _projectPublishFolder);
                }
                catch (Exception ex)
                {
                    return "unzip publish file error:" + ex.Message;
                }

                Log("unzip success ==>" + _projectPublishFolder);

                deployFolder = findUploadFolder(_projectPublishFolder, true);

                if (!Directory.Exists(deployFolder))
                {
                    return "unzip publish file error,Path not found:" + deployFolder;
                }

                var deploySaveFolder  = string.IsNullOrEmpty(_physicalPath) ? Path.Combine(projectPath, "deploy") : _physicalPath;
                DockerParamModel.ProjectDeployRoot = deploySaveFolder;

                EnsureProjectFolder(deploySaveFolder);

                Arguments args = new Arguments
                {
                    DeployType = "Dokcer",
                    BackupFolder = Setting.BackUpDockerPathFolder,
                    AppName = ProjectName,
                    AppFolder = deploySaveFolder,
                    DeployFolder = deployFolder,
                    BackUpIgnoreList = this._backUpIgnoreList,
                    NoBackup = !Setting.NeedBackUp,
                    Extention = DockerParamModel,
                    TempPhysicalPath= _dateTimeFolderName
                };

                Log("Start to deploy docker Service:");
                Log("ImageName ===>" + ProjectName);
                Log("DeployFolder ===> " + deploySaveFolder);

                var ops = new OperationsDocker(args, Log);

                try
                {
                    ops.Execute();

                    try
                    {
                        //如果是增量的话 要把复制过来
                        if (_isIncrement)
                        {
                            Log("Increment deploy start to backup...");
                            //projectLocation.Item1 转到 increment 的目录
                            var incrementFolder = Path.Combine(_projectPublishFolder, "increment");
                            EnsureProjectFolder(incrementFolder);

                            if (this._backUpIgnoreList != null && this._backUpIgnoreList.Any())
                            {
                                var excludFrom = Path.Combine(_projectPublishFolder, "exclude.log");
                                File.WriteAllLines(excludFrom, this._backUpIgnoreList);
                                //存到文件里面去 要指定排除
                                CopyHelper.ProcessXcopy(deploySaveFolder, incrementFolder, excludFrom, Log);
                            }
                            else
                            {
                                CopyHelper.ProcessXcopy(deploySaveFolder, incrementFolder, Log);
                            }

                            Log("Increment deploy backup success...");
                        }
                    }
                    catch (Exception ex3)
                    {
                        Log("Increment deploy folder backup fail:" + ex3.Message);
                    }

                    Log("Deploy docker service Execute Success");
                }
                catch (Exception ex)
                {
                    try
                    {
                        //ops.Rollback();

                        return $"publish to docker err:{ex.Message}";
                    }
                    catch (Exception ex2)
                    {
                        return $"publish to docker err:{ex.Message},rollback fail:{ex2.Message}";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(deployFolder) && Directory.Exists(deployFolder))
                {
                    new Task(() =>
                    {
                        try
                        {
                            //需要重新打包成publish.zip 因为Dockerfile的备份
                            Directory.Delete(deployFolder, true);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }).Start();
                }
            }
        }






        public override string CheckData(FormHandler formHandler)
        {
            var _param = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("param"));
            if (_param == null || string.IsNullOrEmpty(_param.TextValue))
            {
                return "projectName required";
            }

            DockerParamModel = JsonConvert.DeserializeObject<DockerParamModel>(_param.TextValue);


            var serviceNameItem = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("projectName"));
            if (serviceNameItem == null || string.IsNullOrEmpty(serviceNameItem.TextValue))
            {
                return "projectName required";
            }
            DockerParamModel.NetCoreENTRYPOINT = serviceNameItem.TextValue.Trim();

            //发布版本
            var dateTimeFolderName = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("deployFolderName"));
            if (dateTimeFolderName != null && !string.IsNullOrEmpty(dateTimeFolderName.TextValue))
            {
                _dateTimeFolderName = dateTimeFolderName.TextValue;
            }

            //是否是增量发布
            var isIncrement = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("isIncrement"));
            if (isIncrement != null && !string.IsNullOrEmpty(isIncrement.TextValue) && isIncrement.TextValue.ToLower().Equals("true"))
            {
                _isIncrement = true;
            }

            //指定的物理路径
            var physicalPath = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("physicalPath"));
            if (physicalPath != null && !string.IsNullOrEmpty(physicalPath.TextValue))
            {
                _physicalPath = physicalPath.TextValue;
            }

            var backUpIgnoreList = formHandler.FormItems.FirstOrDefault(r => r.FieldName.Equals("backUpIgnore"));
            if (backUpIgnoreList != null && !string.IsNullOrEmpty(backUpIgnoreList.TextValue))
            {
                this._backUpIgnoreList = backUpIgnoreList.TextValue.Split(new string[] { "@_@" }, StringSplitOptions.None).ToList();
            }
            return string.Empty;
        }

    }
}
