using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace AntDeployAgentWindows.Util
{
    public static class LinuxServiceHelper
    {
        public static string CreateServiceFile(string serviceName, string deployFolder, string fileName, string desc, string env, Action<string> logger = null)
        {
            string filePath = Path.Combine(deployFolder, $"{serviceName}.service");
            if (File.Exists(filePath))
            {
                logger?.Invoke("【Warn】【systemctl】" + $"create service file canceled!,use [{filePath}] instead");
            }
            else
            {
                var all = new List<string>();
                all.Add("[Unit]");
                all.Add("Description=" + desc.Trim());
                all.Add("[Service]");
                all.Add("Type=notify");
                all.Add("User=root");
                all.Add("WorkingDirectory=" + deployFolder);
                all.Add("ExecStart=" + Path.Combine(deployFolder, fileName));
                all.Add("SyslogIdentifier=" + serviceName);
                all.Add("Restart=always");
                all.Add("RestartSec=5");

                if (!string.IsNullOrEmpty(env))
                {
                    var envList = env.Split(';');
                    foreach (var item in envList)
                    {
                        all.Add($"Environment={item}");
                    }
                }

                all.Add("[Install]");
                all.Add("WantedBy=multi-user.target");


                File.WriteAllLines(filePath, all);

                foreach (var item in all)
                {
                    logger?.Invoke("【systemctl】" + item);
                }
            }

            return string.Empty;

        }
        public static string CreateServiceFileAndRun(string serviceName, string deployFolder, string fileName, string desc,string env, bool autoStart,Action<string> logger = null)
        {
            string filePath = Path.Combine(deployFolder, $"{serviceName}.service");

            CreateServiceFile(serviceName, deployFolder, fileName, desc, env, logger);

            ServiceRun(serviceName, filePath, autoStart, logger);

            return string.Empty;

        }


        public static string ServiceRun(string serviceName, string filePath,bool autoStart,  Action<string> logger = null)
        {
            CopyHelper.RunCommand($"sudo systemctl stop {serviceName}.service", null, null);

            var result = false;
            if (!string.IsNullOrEmpty(filePath))
            {
                //非回滚
                logger?.Invoke("【Command】" + $"sudo cp -f {filePath} /etc/systemd/system/{serviceName}.service");
                result = CopyHelper.RunCommand($"sudo cp -f {filePath} /etc/systemd/system/{serviceName}.service", null, logger);
                if (!result)
                {
                    logger?.Invoke("【Command】" + $"sudo cp -f {filePath} /etc/systemd/system/{serviceName}.service" + "--->Fail");
                    return "Excute command Fail";
                }

                result = CopyHelper.RunCommand($"sudo systemctl daemon-reload", null, logger);
                if (!result)
                {
                    logger?.Invoke("【Command】" + $"sudo systemctl daemon-reload" + "--->Fail");
                    return "Excute command Fail";
                }
            }

            //sudo cp Worker.service /etc/systemd/system/Worker.service
            //sudo systemctl daemon - reload
            //sudo systemctl start Worker
            //防止有存在并运行中相同名字的有冲突
            result = CopyHelper.RunCommand($"sudo systemctl start {serviceName}.service", null, logger);
            if (!result)
            {
                logger?.Invoke("【Command】" + $"sudo systemctl start {serviceName}.service" + "--->Fail");
                return "Excute command Fail";
            }

            if(autoStart) CopyHelper.RunCommand($"sudo systemctl enable {serviceName}.service", null, null);
            return string.Empty;

        }



        /// <summary>
        /// 获取systemctl里面有没有这个服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static Tuple<string,string> GetLinuxService(string serviceName, Action<string> logger = null)
        {
            try
            {
                var folder = string.Empty;
                var result = CopyHelper.RunCommand($"sudo systemctl status {serviceName}.service", null, logger);
                if (!result)
                {
                    logger?.Invoke($"【Warn】【systemctl】 {serviceName} not exist");
                }
                else
                {
                    //说明存在这个服务的

                    //读出来 service 文件的内容 然后拿到他的 WorkingDirectory
                    var filePath = $"/etc/systemd/system/{serviceName}.service";
                    if (!File.Exists(filePath))
                    {
                        logger?.Invoke($"【Warn】【systemctl】 {filePath} not exist");
                        return new Tuple<string, string>(null, folder); 
                    }

                    var content = File.ReadAllLines(filePath);
                    //WorkingDirectory
                    foreach (var line in content)
                    {
                        if (string.IsNullOrEmpty(line)) continue;
                        logger?.Invoke($"【systemctl】 {line}");
                        if (line.StartsWith("WorkingDirectory"))
                        {
                            var folderArr = line.Split('=');
                            if (folderArr.Length != 2)
                            {
                                logger?.Invoke($"【Warn】【systemctl】 WorkingDirectory not found");
                                return new Tuple<string, string>(null, folder);
                            }

                            folder = folderArr[1].Trim();
                        }
                    }

                }

                return new Tuple<string, string>(null, folder);
            }
            catch (Exception e)
            {
                return new Tuple<string, string>(e.Message,null);
            }
            
        }
    }
}
