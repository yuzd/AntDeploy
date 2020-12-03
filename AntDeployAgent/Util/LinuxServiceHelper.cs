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
        public static string CreateServiceFile(string serviceName, string deployFolder, string fileName, string desc, string env,bool notify, Action<string> logger = null)
        {
            string filePath = Path.Combine(deployFolder, $"{serviceName}.service");
            if (File.Exists(filePath))
            {
                logger?.Invoke("【Warn】【systemctl】" + $"found serviceFile: [{filePath}] ");
                //如果是自己有描述文件 看看有没有要替换的占位符
                var allLines = File.ReadAllLines(filePath);
                if (!allLines.Any())
                {
                    logger?.Invoke("【Error】【systemctl】" + $"[{filePath}] is empty");
                    return string.Empty;
                }

                var newLines = new Dictionary<string,List<string>>();
                var index = 0;
                foreach (var line in allLines)
                {
                    index++;
                    if (string.IsNullOrEmpty(line))
                    {
                        newLines.Add(index + "", new List<string> { line });
                        continue;
                    }

                    if (line.Contains("{WorkingDirectory}"))
                    {
                        newLines.Add(index+"",new List<string>{ line.Replace("{WorkingDirectory}", deployFolder) });
                    }
                    else if (line.Contains("{ExecStart}"))
                    {
                        newLines.Add(index + "", new List<string>{ line.Replace("{ExecStart}", Path.Combine(deployFolder, fileName)) });
                    }
                    else if (line.Contains("[Install]"))
                    {
                        if (!newLines.ContainsKey("env"))
                        {
                            newLines.Add("env", new List<string> { "" });
                        }
                        newLines.Add(index + "", new List<string> { line });
                    }
                    else if (line.Contains("[Service]"))
                    {
                        newLines.Add(index + "", new List<string> { line });
                        if (!newLines.ContainsKey("notify"))
                        {
                            newLines.Add("notify", new List<string> { notify? "Type=notify":  "" });
                        }
                    }
                    else if (line.StartsWith("Type="))
                    {
                        var typeValue = line.Substring(5, line.Length - 5);//原来有配置值 本次运行也没要求notify 就用原本的
                        if (!string.IsNullOrEmpty(typeValue) && newLines.ContainsKey("notify"))
                        {
                            var typeA = newLines["notify"].First();
                            if (string.IsNullOrEmpty(typeA) && typeValue != "notify")
                            {
                                //那就用原来的
                                newLines["notify"] = new List<string> {"Type=" + typeValue};
                            }
                        }
                    }
                    else if (line.StartsWith("Environment="))
                    {
                        var envValue = line.Substring(12,line.Length-12);
                        if (string.IsNullOrEmpty(envValue))
                        {
                            newLines.Add(index + "", new List<string> { line });
                            continue;
                        }

                        var newValue = "Environment=" + envValue;
                        if (!newLines.TryGetValue("env",out var envList))
                        {
                            newLines.Add("env",new List<string> { newValue });
                            continue;
                        }

                        if (!envList.Contains(newValue))
                        {
                            envList.Add(newValue);
                        }
                    }
                    else
                    {
                        newLines.Add(index + "", new List<string> { line });
                    }
                }

                if (!newLines.ContainsKey("env"))
                {
                    newLines.Add("env",new List<string>());
                }

                var envold = newLines["env"];
                if (!string.IsNullOrEmpty(env))
                {
                    var envList = env.Split(';');
                    foreach (var item in envList)
                    {
                        if(string.IsNullOrEmpty(item))continue;
                        var value = $"Environment={item.Trim()}";
                        if (!envold.Contains(value))
                        {
                            envold.Add(value);
                        }
                    }
                }

                if (envold.Any())
                {
                    //检查是否有重复的 有重复的就用只保留最后一个
                    var dic = new Dictionary<string, string>();
                    foreach (var old in envold)
                    {
                        var envValue = old.Substring(12, old.Length - 12);
                        var arr = envValue.Split('=');
                        if (arr.Length < 2)
                        {
                            continue;
                        }

                        var key = arr[0];
                        var value = string.Join("", arr.Skip(1));
                        if (!dic.ContainsKey(key))
                        {
                            dic.Add(key, value);
                        }
                        else
                        {
                            dic[key] = value;
                        }
                    }

                    newLines["env"] = dic.Select(r => $"Environment={r.Key}={r.Value}").ToList();
                }
              

                File.WriteAllLines(filePath, newLines.SelectMany(r=>r.Value));

                foreach (var item in File.ReadAllLines(filePath))
                {
                    if(!string.IsNullOrEmpty(item))
                        logger?.Invoke("【systemctl】" + item);
                }

                return string.Empty;
            }


            var all = new List<string>();
            all.Add("[Unit]");
            all.Add("Description=" + desc.Trim());
            all.Add("[Service]");
            if(notify) all.Add("Type=notify");
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
                    all.Add($"Environment={item.Trim()}");
                }
            }

            all.Add("[Install]");
            all.Add("WantedBy=multi-user.target");


            File.WriteAllLines(filePath, all);

            foreach (var item in all)
            {
                logger?.Invoke("【systemctl】" + item);
            }

            return string.Empty;

        }
        public static string CreateServiceFileAndRun(string serviceName, string deployFolder, string fileName, string desc,string env,string execFullPath, 
            bool autoStart,bool notify,Action<string> logger = null)
        {
            string filePath = Path.Combine(deployFolder, $"{serviceName}.service");

            CreateServiceFile(serviceName, deployFolder, fileName, desc, env,notify, logger);

            ServiceRun(serviceName, filePath, autoStart, execFullPath,logger);

            return string.Empty;

        }


        public static string ServiceRun(string serviceName, string filePath,bool autoStart,string execFullPath2,  Action<string> logger = null)
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


            //linux service
            if (!File.Exists(execFullPath2))
            {
                return $"systemctl service exec file not found : {execFullPath2}";
            }

            if (!execFullPath2.EndsWith(".dll"))
            {
                logger?.Invoke($"【Command】sudo chmod +x {execFullPath2}");
                CopyHelper.RunCommand($"sudo chmod +x {execFullPath2}");
            }

            //sudo cp Worker.service /etc/systemd/system/Worker.service
            //sudo systemctl daemon - reload
            //sudo systemctl start Worker
            //防止有存在并运行中相同名字的有冲突
            logger?.Invoke("【Command】" + $"sudo systemctl start {serviceName}.service");
            result = CopyHelper.RunCommand($"sudo systemctl start {serviceName}.service", null, logger);
            if (!result)
            {
                logger?.Invoke("【Command】" + $"sudo systemctl start {serviceName}.service" + "--->Fail");
                return "Excute command Fail";
            }

            if (autoStart)
            {
                logger?.Invoke("【Command】" + $"sudo systemctl enable {serviceName}.service");
                CopyHelper.RunCommand($"sudo systemctl enable {serviceName}.service", null, null);
            }
            return string.Empty;

        }



        /// <summary>
        /// 获取systemctl里面有没有这个服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static Tuple<string,string,string> GetLinuxService(string serviceName, Action<string> logger = null)
        {
            try
            {
                var folder = string.Empty;
                var exeute = string.Empty;
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
                        return new Tuple<string, string, string>(null, folder, exeute);
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
                                return new Tuple<string, string, string>(null, folder, exeute);
                            }

                            folder = folderArr[1].Trim();
                        }

                        if (line.StartsWith("ExecStart"))
                        {
                            var folderArr = line.Split('=');
                            if (folderArr.Length != 2)
                            {
                                logger?.Invoke($"【Warn】【systemctl】 ExecStart not found");
                                return new Tuple<string, string, string>(null, folder,exeute);
                            }

                            exeute = folderArr[1].Trim();
                        }
                    }

                }

                return new Tuple<string, string,string>(null, folder, exeute);
            }
            catch (Exception e)
            {
                return new Tuple<string, string, string>(e.Message,null,null);
            }
            
        }
    }
}
