using AntDeploy.Models;
using AntDeploy.Util;
using EnvDTE;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace AntDeploy.Winform
{
    public partial class Deploy : Form
    {

        private string ProjectConfigPath;
        private string ProjectFolderPath;
        private string ProjectPath;
        private Project _project;
        private NLog.Logger nlog_iis;
        private NLog.Logger nlog_windowservice;
        private NLog.Logger nlog_docker;

        private int ProgressPercentage = 0;
        private string ProgressCurrentHost = null;
        private int ProgressPercentageForWindowsService = 0;

        public Deploy(string projectPath, Project project)
        {

            InitializeComponent();


            this.Text += $"(Version:{Vsix.VERSION})";

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeploy.Resources.Logo1.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            ProjectPath = projectPath;
            _project = project;
            ReadPorjectConfig(projectPath);

            #region Nlog
            var config = new LoggingConfiguration();
            var richTarget = new RichTextBoxTarget
            {
                Name = "rich_iis_log",
                Layout = "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_iis_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_iis_log", richTarget);
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, richTarget);
            config.LoggingRules.Add(rule1);


            var richTarget2 = new RichTextBoxTarget
            {
                Name = "rich_windowservice_log",
                Layout = "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_windowservice_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_windowservice_log", richTarget2);
            LoggingRule rule2 = new LoggingRule("*", LogLevel.Debug, richTarget2);
            config.LoggingRules.Add(rule2);

            var richTarget3 = new RichTextBoxTarget
            {
                Name = "rich_docker_log",
                Layout = "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_docker_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_docker_log", richTarget3);
            LoggingRule rule3 = new LoggingRule("*", LogLevel.Debug, richTarget3);
            config.LoggingRules.Add(rule3);

            LogManager.Configuration = config;

            nlog_iis = NLog.LogManager.GetLogger("rich_iis_log");
            nlog_windowservice = NLog.LogManager.GetLogger("rich_windowservice_log");
            nlog_docker = NLog.LogManager.GetLogger("rich_docker_log");

            RichLogInit();
            #endregion

        }

        public DeployConfig DeployConfig { get; set; }

        #region Form


        private void Deploy_Load(object sender, EventArgs e)
        {



            if (DeployConfig == null) DeployConfig = new DeployConfig();
            DeployConfig.EnvChangeEvent += DeployConfigOnEnvChangeEvent;
            if (DeployConfig.Env != null && DeployConfig.Env.Any())
            {
                foreach (var env in DeployConfig.Env)
                {
                    this.combo_env_list.Items.Add(env.Name);
                    this.combo_iis_env.Items.Add(env.Name);
                    this.combo_windowservice_env.Items.Add(env.Name);
                    this.combo_docker_env.Items.Add(env.Name);
                }

                this.combo_env_list.SelectedIndex = 0;
            }

            if (DeployConfig.IgnoreList != null && DeployConfig.IgnoreList.Any())
            {
                foreach (var ignore in DeployConfig.IgnoreList)
                {
                    this.list_env_ignore.Items.Add(ignore);
                }
            }


            if (DeployConfig.IIsConfig != null)
            {
                if (this.combo_iis_sdk_type.Items.Count > 0 && !string.IsNullOrEmpty(DeployConfig.IIsConfig.SdkType)
                    && this.combo_iis_sdk_type.Items.Cast<string>().Contains(DeployConfig.IIsConfig.SdkType))
                {
                    this.combo_iis_sdk_type.SelectedItem = DeployConfig.IIsConfig.SdkType;
                }

                if (!string.IsNullOrEmpty(DeployConfig.IIsConfig.WebSiteName))
                {
                    this.txt_iis_web_site_name.Text = DeployConfig.IIsConfig.WebSiteName;
                }


                if (this.combo_iis_env.Items.Count > 0 &&
                    !string.IsNullOrEmpty(DeployConfig.IIsConfig.LastEnvName)
                    && this.combo_iis_env.Items.Cast<string>().Contains(DeployConfig.IIsConfig.LastEnvName))
                {
                    this.combo_iis_env.SelectedItem = DeployConfig.IIsConfig.LastEnvName;
                }
            }

            if (DeployConfig.WindowsServiveConfig != null)
            {
                if (this.combo_windowservice_sdk_type.Items.Count > 0 && !string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.SdkType)
                    && this.combo_windowservice_sdk_type.Items.Cast<string>().Contains(DeployConfig.WindowsServiveConfig.SdkType))
                {
                    this.combo_windowservice_sdk_type.SelectedItem = DeployConfig.WindowsServiveConfig.SdkType;
                }

                if (this.combo_windowservice_env.Items.Count > 0 &&
                    !string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.LastEnvName)
                    && this.combo_windowservice_env.Items.Cast<string>().Contains(DeployConfig.WindowsServiveConfig.LastEnvName))
                {
                    this.combo_windowservice_env.SelectedItem = DeployConfig.WindowsServiveConfig.LastEnvName;
                }

                if (!string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.ServiceName))
                {
                    this.txt_windowservice_name.Text = DeployConfig.WindowsServiveConfig.ServiceName;
                }

                if (!string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.StopTimeOutSeconds))
                {
                    this.txt_windowservice_timeout.Text = DeployConfig.WindowsServiveConfig.StopTimeOutSeconds;
                }
            }

            if (DeployConfig.DockerConfig != null)
            {
                if (this.combo_docker_env.Items.Count > 0 &&
                    !string.IsNullOrEmpty(DeployConfig.DockerConfig.LastEnvName)
                    && this.combo_docker_env.Items.Cast<string>().Contains(DeployConfig.DockerConfig.LastEnvName))
                {
                    this.combo_docker_env.SelectedItem = DeployConfig.DockerConfig.LastEnvName;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.Prot))
                {
                    this.txt_docker_port.Text = DeployConfig.DockerConfig.Prot;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.AspNetCoreEnv))
                {
                    this.txt_docker_envname.Text = DeployConfig.DockerConfig.AspNetCoreEnv;
                }
            }


            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
            this.txt_linux_host.Text = string.Empty;
            this.txt_linux_username.Text = string.Empty;
            this.txt_linux_pwd.Text = string.Empty;


        }


        public void RichLogInit()
        {
            RichTextBoxTarget.ReInitializeAllTextboxes(this);
            RichTextBoxTarget.GetTargetByControl(rich_iis_log).LinkClicked += LinkClicked;
            RichTextBoxTarget.GetTargetByControl(rich_windowservice_log).LinkClicked += LinkClicked;
            RichTextBoxTarget.GetTargetByControl(rich_docker_log).LinkClicked += LinkClicked;
        }


        private void LinkClicked(RichTextBoxTarget sender, string linktext, LogEventInfo logevent)
        {
            BeginInvokeLambda(() =>
                {
                    try
                    {
                        if (linktext.StartsWith("http") || linktext.StartsWith("file:"))
                        {
                            ProcessStartInfo sInfo = new ProcessStartInfo(linktext);
                            Process.Start(sInfo);
                        }
                        else
                        {
                            System.Windows.Forms.Clipboard.SetText(linktext);
                            MessageBox.Show("copy to Clipboard success", sender.Name);
                        }
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            );
        }


        private void Deploy_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeployConfig.IIsConfig.WebSiteName = this.txt_iis_web_site_name.Text.Trim();

            DeployConfig.WindowsServiveConfig.ServiceName = this.txt_windowservice_name.Text.Trim();
            DeployConfig.WindowsServiveConfig.StopTimeOutSeconds = this.txt_windowservice_timeout.Text.Trim();

            DeployConfig.DockerConfig.Prot = this.txt_docker_port.Text.Trim();
            DeployConfig.DockerConfig.AspNetCoreEnv = this.txt_docker_envname.Text.Trim();

            if (!string.IsNullOrEmpty(ProjectConfigPath))
            {
                var configJson = JsonConvert.SerializeObject(DeployConfig, Formatting.Indented);
                File.WriteAllText(ProjectConfigPath, configJson, Encoding.UTF8);
            }
            
        }


        #endregion

        #region setting page
        /// <summary>
        /// 添加环境
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_add_by_name_Click(object sender, EventArgs e)
        {
            var env_name = this.txt_env_name.Text.Trim();
            if (string.IsNullOrWhiteSpace(env_name))
            {
                MessageBox.Show("please input env name first");
                return;
            }

            if (this.combo_env_list.Items.Cast<string>().Contains(env_name))
            {
                this.combo_env_list.SelectedItem = env_name;
            }
            else
            {
                this.combo_env_list.Items.Add(env_name);
                DeployConfig.AddEnv(new Env
                {
                    Name = env_name,
                    ServerList = new List<Server>()
                });
                this.combo_env_list.SelectedItem = env_name;
                this.txt_env_server_host.Text = string.Empty;
                this.txt_env_server_token.Text = string.Empty;
                this.txt_linux_host.Text = string.Empty;
                this.txt_linux_username.Text = string.Empty;
                this.txt_linux_pwd.Text = string.Empty;
                this.txt_env_name.Text = string.Empty;
            }
        }

        /// <summary>
        /// 删除环境
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_remove_Click(object sender, EventArgs e)
        {
            var selectedEnv = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEnv))
            {
                return;
            }

            this.DeployConfig.RemoveEnv(this.combo_env_list.SelectedIndex);
            this.combo_env_list.Items.Remove(selectedEnv);
            if (this.combo_env_list.Items.Count > 0)
            {
                this.combo_env_list.SelectedIndex = 0;
            }
            else
            {
                combo_env_list_SelectedIndexChanged(null, null);
            }

        }

        /// <summary>
        /// 环境改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_env_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.combo_env_server_list.Items.Clear();
            this.combo_linux_server_list.Items.Clear();

            var selectedEnv = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEnv))
            {
                this.b_env_server_add.Enabled = false;
                this.b_env_server_remove.Enabled = false;
                this.b_env_server_test.Enabled = false;
                this.txt_env_server_host.Text = string.Empty;
                this.txt_env_server_token.Text = string.Empty;

                this.b_linux_server_test.Enabled = false;
                this.b_linux_server_remove.Enabled = false;
                this.b_add_linux_server.Enabled = false;

                this.txt_linux_username.Text = string.Empty;
                this.txt_linux_host.Text = string.Empty;
                this.txt_linux_pwd.Text = string.Empty;

                return;
            }

            this.b_env_server_add.Enabled = true;
            this.b_env_server_remove.Enabled = true;
            this.b_env_server_test.Enabled = true;

            this.b_linux_server_test.Enabled = true;
            this.b_linux_server_remove.Enabled = true;
            this.b_add_linux_server.Enabled = true;

            var env = this.DeployConfig.Env.FirstOrDefault(r => r.Name.Equals(selectedEnv));

            if (env != null)
            {
                // ReSharper disable once CoVariantArrayConversion
                this.combo_env_server_list.Items.AddRange(items: env.ServerList.Select(r => r.Host + "@_@" + r.Token).ToArray());

                // ReSharper disable once CoVariantArrayConversion
                this.combo_linux_server_list.Items.AddRange(items: env.LinuxServerList.Select(r => r.Host + "@_@" + r.UserName).ToArray());

            }
            if (this.combo_env_server_list.Items.Count > 0)
            {
                this.combo_env_server_list.Tag = "active";
                this.combo_env_server_list.SelectedIndex = 0;
            }
            if (this.combo_linux_server_list.Items.Count > 0)
            {
                 this.combo_linux_server_list.Tag = "active";
                 this.combo_linux_server_list.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// window server 改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_env_server_list_SelectedIndexChanged(object sender, EventArgs e)
        {


            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
            var seletedServer = this.combo_env_server_list.SelectedItem as string;
            if (string.IsNullOrEmpty(seletedServer))
            {
                return;
            }

            if (combo_env_server_list.Tag != null)
            {
                combo_env_server_list.Tag = null;
                return;
            }

            var arr = seletedServer.Split(new string[] { "@_@" }, StringSplitOptions.None);
            if (arr.Length == 2)
            {
                this.txt_env_server_host.Text = arr[0];
                this.txt_env_server_token.Text = arr[1];
            }

        }


        /// <summary>
        /// 删除window server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_server_remove_Click(object sender, EventArgs e)
        {
            var seletedServer = this.combo_env_server_list.SelectedItem as string;
            if (string.IsNullOrEmpty(seletedServer))
            {
                return;
            }

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].ServerList.RemoveAt(this.combo_env_server_list.SelectedIndex);
            this.combo_env_server_list.Items.Remove(seletedServer);
        }

        /// <summary>
        /// 添加window server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_server_add_Click(object sender, EventArgs e)
        {
            var serverHost = this.txt_env_server_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            var serverTolen = this.txt_env_server_token.Text.Trim();
            if (serverTolen.Length < 1)
            {
                MessageBox.Show("please input server Token");
                return;
            }

            var existServer = this.combo_env_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBox.Show("input server host is exist!" + Environment.NewLine + "if you want to modify,please remove and readd");
                return;
            }
            var newServer = serverHost + "@_@" + serverTolen;
            this.combo_env_server_list.Items.Add(newServer);
            DeployConfig.Env[this.combo_env_list.SelectedIndex].ServerList.Add(new Server
            {
                Host = serverHost,
                Token = serverTolen
            });
            this.combo_env_server_list.SelectedItem = newServer;
            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
        }

        /// <summary>
        /// window server 链接测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_server_test_Click(object sender, EventArgs e)
        {
            var serverHost = this.txt_env_server_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            var serverTolen = this.txt_env_server_token.Text.Trim();
            if (serverTolen.Length < 1)
            {
                MessageBox.Show("please input server Token");
                return;
            }

            try
            {
                WebClient client = new WebClient();
                var result = client.DownloadString($"http://{serverHost}/publish?Token={serverTolen}");
                if (result.Equals("success"))
                {
                    MessageBox.Show("Connect Sussess");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Connect Fail");
            }
        }

        /// <summary>
        /// 添加ignore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_ignore_add_Click(object sender, EventArgs e)
        {
            var ignoreTxt = this.txt_env_ignore.Text.Trim();
            if (ignoreTxt.Length < 1)
            {
                MessageBox.Show("please input ignore rule");
                return;
            }

            var existIgnore = this.list_env_ignore.Items.Cast<string>().FirstOrDefault(r => r.Equals(ignoreTxt));
            if (!string.IsNullOrEmpty(existIgnore))
            {
                this.list_env_ignore.SelectedItem = existIgnore;
            }
            else
            {
                DeployConfig.IgnoreList.Add(ignoreTxt);
                this.list_env_ignore.Items.Add(ignoreTxt);
                this.txt_env_ignore.Text = string.Empty;
                this.list_env_ignore.SelectedItem = ignoreTxt;
            }
        }

        /// <summary>
        /// 删除Ignore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_ignore_remove_Click(object sender, EventArgs e)
        {
            if (this.list_env_ignore.SelectedIndex < 0) return;
            DeployConfig.IgnoreList.RemoveAt(this.list_env_ignore.SelectedIndex);
            this.list_env_ignore.Items.RemoveAt(this.list_env_ignore.SelectedIndex);
            if (this.list_env_ignore.Items.Count >= 0)
                this.list_env_ignore.SelectedIndex = this.list_env_ignore.Items.Count - 1;
        }


        /// <summary>
        /// 添加linux server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_add_linux_server_Click(object sender, EventArgs e)
        {
            var serverHost = this.txt_linux_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            var userName = this.txt_linux_username.Text.Trim();
            if (userName.Length < 1)
            {
                MessageBox.Show("please input server userName");
                return;
            }

            var pwd = this.txt_linux_pwd.Text.Trim();
            if (pwd.Length < 1)
            {
                MessageBox.Show("please input server pwd");
                return;
            }

            var pwd2 = CodingHelper.AESEncrypt(pwd);
            var existServer = this.combo_linux_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBox.Show("input server host is exist!" + Environment.NewLine + "if you want to modify,please remove and readd");
                return;
            }
            var newServer = serverHost + "@_@" + userName;
            this.combo_linux_server_list.Items.Add(newServer);
            DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList.Add(new LinuxServr
            {
                Host = serverHost,
                UserName = userName,
                Pwd = pwd2
            });

            this.combo_linux_server_list.SelectedItem = newServer;
            this.txt_linux_host.Text = string.Empty;
            this.txt_linux_username.Text = string.Empty;
            this.txt_linux_pwd.Text = string.Empty;
        }

        /// <summary>
        /// 删除linux server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_linux_server_remove_Click(object sender, EventArgs e)
        {
            var seletedServer = this.combo_linux_server_list.SelectedItem as string;
            if (string.IsNullOrEmpty(seletedServer))
            {
                return;
            }

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList.RemoveAt(this.combo_linux_server_list.SelectedIndex);
            this.combo_linux_server_list.Items.Remove(seletedServer);
        }

        /// <summary>
        /// linux server 测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_linux_server_test_Click(object sender, EventArgs e)
        {
            //看ssh是否能链接的通
            var serverHost = this.txt_linux_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            var userName = this.txt_linux_username.Text.Trim();
            if (userName.Length < 1)
            {
                MessageBox.Show("please input server userName");
                return;
            }

            var pwd = this.txt_linux_pwd.Text.Trim();
            if (pwd.Length < 1)
            {
                MessageBox.Show("please input server pwd");
                return;
            }

            try
            {
                using (SSHClient sshClient = new SSHClient(serverHost, userName, pwd, Console.WriteLine))
                {
                    var r = sshClient.Connect(true);
                    if (r)
                    {
                        MessageBox.Show("Connect Success");
                    }
                    else
                    {
                        MessageBox.Show("Connect Fail");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Connect Fail");
            }
        }

        /// <summary>
        /// linux server 改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_linux_server_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt_linux_host.Text = string.Empty;
            this.txt_linux_username.Text = string.Empty;
            this.txt_linux_pwd.Text = string.Empty;

            var seletedServer = this.combo_linux_server_list.SelectedItem as string;
            if (string.IsNullOrEmpty(seletedServer))
            {
                return;
            }

            if (combo_linux_server_list.Tag != null)
            {
                combo_linux_server_list.Tag = null;
                return;
            }

            var arr = seletedServer.Split(new string[] { "@_@" }, StringSplitOptions.None);
            if (arr.Length >= 2)
            {
                this.txt_linux_host.Text = arr[0];
                this.txt_linux_username.Text = arr[1];
                //this.txt_linux_pwd.Text = CodingHelper.AESDecrypt(arr[2]);

            }
        }
        #endregion

        #region iis page
        private void combo_iis_sdk_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_iis_sdk_type.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.IIsConfig.SdkType = selectName;
            }
        }

        private void combo_iis_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_iis_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.IIsConfig.LastEnvName = selectName;

                //生成进度
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        this.tabPage_progress.Controls.Remove(box.Value);
                    }

                }

                var newBoxList = new Dictionary<string,ProgressBox>();

                var serverList = DeployConfig.Env.Where(r => r.Name.Equals(selectName)).Select(r => r.ServerList)
                    .FirstOrDefault();

                if (serverList == null || !serverList.Any())
                {
                    return;
                }

                var serverHostList = serverList.Select(r => r.Host).ToList();

                for (int i = 0; i < serverHostList.Count; i++)
                {
                    var serverHost = serverHostList[i];
                    ProgressBox newBox = new ProgressBox(new System.Drawing.Point(22, 15 + (i * 110)))
                    {
                        Text = serverHost,
                    };

                    newBoxList.Add(serverHost,newBox);
                    this.tabPage_progress.Controls.Add(newBox);
                }

                this.tabPage_progress.Tag = newBoxList;


            }
        }


        private void b_iis_deploy_Click(object sender, EventArgs e)
        {

            //判断当前项目是否是web项目
            bool isWeb = ProjectHelper.IsDotNetCoreProject(_project) || ProjectHelper.IsWebProject(_project);

            if (!isWeb)
            {
                //检查工程文件里面是否含有 WebProjectProperties字样
                if (!string.IsNullOrEmpty(ProjectPath) && File.Exists(ProjectPath))
                {
                    var fileInfo = File.ReadAllText(ProjectPath);
                    if (fileInfo.Contains("<WebProjectProperties>") && fileInfo.Contains("</WebProjectProperties>"))
                    {
                        isWeb = true;
                    }
                }
            }

            if (!isWeb)
            {
                MessageBox.Show("current project is not web project!");
                return;
            }

            var websiteName = this.txt_iis_web_site_name.Text.Trim();
            if (websiteName.Length < 1)
            {
                MessageBox.Show("please input web site name");
                return;
            }

            var sdkTypeName = this.combo_iis_sdk_type.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(sdkTypeName))
            {
                MessageBox.Show("please select sdk type");
                return;
            }

            var envName = this.combo_iis_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            var Port = this.txt_iis_port.Text.Trim();
            var PoolName = this.txt_pool_name.Text.Trim();

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBox.Show("Are you sure to deploy to Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_iis_env_SelectedIndexChanged(null,null);

            this.rich_iis_log.Text = "";
            DeployConfig.IIsConfig.WebSiteName = websiteName;
            new Task(async () =>
            {
                this.nlog_iis.Info("Start publish");
                Enable(false);//第一台开始编译
                try
                {
                    var isNetcore = false;
                    var publishPath = string.Empty;
                    if (DeployConfig.IIsConfig.SdkType.Equals("netcore"))
                    {
                        var publishLog = new List<string>();
                        //执行 publish
                        var isSuccess = CommandHelper.RunDotnetExternalExe(ProjectFolderPath, "dotnet",
                            "publish -c Release",
                            (r) =>
                            {
                                this.nlog_iis.Info(r);
                                publishLog.Add(r);
                            },
                            (er) => this.nlog_iis.Error(er));

                        if (!isSuccess)
                        {
                            this.nlog_iis.Error("publish error");
                            return;
                        }

                        var publishPathLine = publishLog
                            .FirstOrDefault(r => !string.IsNullOrEmpty(r) && r.EndsWith("\\publish\\"));

                        if (string.IsNullOrEmpty(publishPathLine))
                        {
                            this.nlog_iis.Error("can not find publishPath in log");
                            return;
                        }

                        var publishPathArr = publishPathLine.Split(new string[] { " ->" }, StringSplitOptions.None);
                        if (publishPathArr.Length != 2)
                        {
                            this.nlog_iis.Error("can not find publishPath in log");
                            return;
                        }

                        publishPath = publishPathArr[1].Trim();
                        isNetcore = true;
                    }
                    else
                    {
                        var isSuccess = CommandHelper.RunMsbuild(
                            ProjectPath,
                            (r) => { this.nlog_iis.Info(r); },
                            (er) => this.nlog_iis.Error(er));

                        if (!isSuccess)
                        {
                            this.nlog_iis.Error("publish error");
                            return;
                        }

                        publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", "publish");

                        if (Directory.Exists(publishPath))
                        {
                            var webPublishPath = Path.Combine(publishPath, "_PublishedWebsites");
                            if (Directory.Exists(webPublishPath))
                            {
                                var webPublishFolder = new DirectoryInfo(webPublishPath);
                                var targetFolder = webPublishFolder.GetDirectories().FirstOrDefault();
                                if (targetFolder != null)
                                {
                                    //targetFolder.MoveTo(Path.Combine(targetFolder.Parent.FullName, "publish"));
                                    publishPath = targetFolder.FullName;
                                }
                            }
                        }
                    }

                    IIsBuildEnd();//第一台结束编译
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success,  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    this.nlog_iis.Log(publisEvent);



                    //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis

                    if (isNetcore)
                    {
                        var webConfig = Path.Combine(publishPath, "Web.Config");
                        if (!File.Exists(webConfig))
                        {
                            LogEventInfo theEvent = new LogEventInfo(LogLevel.Warn, "", "publish sdkType:netcore ,but web.config file missing!");
                            theEvent.Properties["ShowLink"] = "https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis";
                            this.nlog_iis.Log(theEvent);
                        }
                    }


                    //执行 打包
                    this.nlog_iis.Info("Start package");
                    byte[] zipBytes;
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true, DeployConfig.IgnoreList,
                            (progressValue) =>
                            {
                                UpdateIIsPackageProgress(null, progressValue);//打印打包记录
                            });
                    }
                    catch (Exception ex)
                    {
                        this.nlog_iis.Error("package fail:" + ex.Message);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_iis.Error("package fail");
                        return;
                    }
                    this.nlog_iis.Info("package success");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_iis.Info("Deploy Start");
                    var index = 0;
                    foreach (var server in serverList)
                    {
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_iis.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            continue;
                        }

                        if (index != 0)//因为编译和打包只会占用第一台服务器的时间
                        {
                            IIsBuildEnd(server.Host);
                            UpdateIIsPackageProgress(server.Host,100);
                        }

                        ProgressPercentage = 0;
                        ProgressCurrentHost = server.Host;
                        this.nlog_iis.Info($"Start Uppload,Host:{server.Host}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "iis");
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.IIsConfig.SdkType);
                        httpRequestClient.SetFieldValue("port", Port);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("poolName", PoolName);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        System.Net.WebSockets.Managed.ClientWebSocket webSocket = null;
                        HttpLogger HttpLogger = null;
                        try
                        {
                            var hostKey = "";

                            HttpLogger = new HttpLogger
                            {
                                Key = loggerId,
                                Url = $"http://{server.Host}/logger?key=" + loggerId
                            };
                            webSocket = await WebSocketHelper.Connect($"ws://{server.Host}/socket", (receiveMsg) =>
                            {
                                if (!string.IsNullOrEmpty(receiveMsg))
                                {
                                    if (receiveMsg.StartsWith("hostKey@"))
                                    {
                                        hostKey = receiveMsg.Replace("hostKey@", "");
                                    }
                                    else
                                    {
                                        this.nlog_iis.Info($"【Server】{receiveMsg}");
                                    }
                                }
                            }, HttpLogger);

                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged; });

                            if(ProgressPercentage<100) UpdateIIsUploadProgress(ProgressCurrentHost, 100);//结束上传

                            if (uploadResult.Item1)
                            {
                                this.nlog_iis.Info($"Host:{server.Host},Response:{uploadResult.Item2}");

                                UpdateIIsDeployProgress(server.Host,true);
                            }
                            else
                            {
                                this.nlog_iis.Error($"Host:{server.Host},Response:{uploadResult.Item2},Skip to Next");
                                UpdateIIsDeployProgress(server.Host, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.nlog_iis.Error($"Fail Deploy,Host:{server.Host},Response:{ex.Message},Skip to Next");
                            UpdateIIsDeployProgress(server.Host, false);
                        }
                        finally
                        {
                            webSocket?.Dispose();
                            HttpLogger?.Dispose();

                        }

                        index++;
                    }
                    //交互
                }
                catch (Exception ex1)
                {
                    this.nlog_iis.Error(ex1);
                }
                finally
                {
                    ProgressPercentage = 0;
                    ProgressCurrentHost = null;
                    Enable(true);
                }


            }).Start();

        }

        private void ClientOnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > ProgressPercentage && e.ProgressPercentage != 100)
            {
                ProgressPercentage = e.ProgressPercentage;
                var showValue = (e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage);
                if(!string.IsNullOrEmpty(ProgressCurrentHost))UpdateIIsUploadProgress(ProgressCurrentHost, showValue);
                this.nlog_iis.Info($"Upload {showValue} % complete...");
            }
        }

        private void Enable(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                this.b_iis_deploy.Enabled = flag;
                this.txt_iis_web_site_name.Enabled = flag;
                this.txt_iis_port.Enabled = flag;
                this.txt_pool_name.Enabled = flag;
                this.combo_iis_env.Enabled = flag;
                this.combo_iis_sdk_type.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_window_service.Enabled = flag;

                if (flag)
                {
                    this.rich_windowservice_log.Text = "";
                }
                else
                {
                    page_.Tag = "0";
                    if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                    {
                        foreach (var box in progressBoxList)
                        {
                            box.Value.StartBuild();
                            break;
                        }

                    }
                }
            });

        }

        private void IIsBuildEnd(string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.BuildEnd();
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.BuildEnd();
                            break;
                        }
                    }
                }
            });
        }

        

        private void UpdateIIsPackageProgress(string host,int value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.UpdatePackageProgress(value);
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.UpdatePackageProgress(value);
                            break;
                        }
                    }
                }
            });
        }

        private void UpdateIIsUploadProgress(string host, int value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.UpdateUploadProgress(value);
                            break;
                        }
                        if (box.Key.Equals(host))
                        {
                            box.Value.UpdateUploadProgress(value);
                            break;
                        }
                    }
                }
            });
        }

        private void UpdateIIsDeployProgress(string host, bool value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {

                        if (box.Key.Equals(host))
                        {
                            box.Value.UpdateDeployProgress(value);
                            break;
                        }
                    }
                }
            });
        }

        #endregion

        #region windowsService page

        private void combo_windowservice_sdk_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_windowservice_sdk_type.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.WindowsServiveConfig.SdkType = selectName;
            }
        }


        private void combo_windowservice_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_windowservice_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.WindowsServiveConfig.LastEnvName = selectName;
            }
        }


        private void ClientOnUploadProgressChanged2(object sender, UploadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > ProgressPercentageForWindowsService && e.ProgressPercentage != 100)
            {
                ProgressPercentageForWindowsService = e.ProgressPercentage;
                this.nlog_windowservice.Info($"Upload {(e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage)} % complete...");
            }
        }



        private void EnableForWindowsService(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {
                this.b_windowservice_deploy.Enabled = flag;
                this.combo_windowservice_env.Enabled = flag;
                this.combo_windowservice_sdk_type.Enabled = flag;
                this.txt_windowservice_name.Enabled = flag;

                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_web_iis.Enabled = flag;

                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_docker_log.Text = "";
                }
                else
                {
                    page_.Tag = "2";
                }
            });

        }



        private void b_windowservice_deploy_Click(object sender, EventArgs e)
        {
            if (ProjectHelper.IsWebProject(_project))
            {
                MessageBox.Show("current project is not windows service project!");
                return;
            }

            var sdkTypeName = this.combo_windowservice_sdk_type.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(sdkTypeName))
            {
                MessageBox.Show("please select sdk type");
                return;
            }

            if (sdkTypeName.Equals("netcore"))
            {
                if (!ProjectHelper.IsDotNetCoreProject(_project))
                {
                    MessageBox.Show("current project is not netcore project!");
                    return;
                }
            }

            var serviceName = this.txt_windowservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBox.Show("please input serviceName");
                return;
            }

            DeployConfig.WindowsServiveConfig.ServiceName = serviceName;

            var stopSenconds = this.txt_windowservice_timeout.Text.Trim();
            if (!string.IsNullOrEmpty(stopSenconds))
            {
                int.TryParse(stopSenconds, out var stopSencondsInt);
                if (stopSencondsInt == 0)
                {
                    MessageBox.Show("please input right stopTimout value");
                    return;
                }
                else
                {
                    DeployConfig.WindowsServiveConfig.StopTimeOutSeconds = stopSenconds;
                }
            }
            else
            {
                DeployConfig.WindowsServiveConfig.StopTimeOutSeconds = "";
            }


            var envName = this.combo_windowservice_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

#if DEBUG
            var execFilePath = "AntDeployAgentWindowsService.exe";
#else
            var execFilePath = _project.GetProjectProperty("OutputFileName");
            if (string.IsNullOrEmpty(execFilePath))
            {
                MessageBox.Show("get current project property:outputfilename error");
                return;
            }

            if (!DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore") && !execFilePath.Trim().ToLower().EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("current project out file name is not exe!");
                return;
            }

            
#endif




            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBox.Show("Are you sure to deploy to Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            this.rich_windowservice_log.Text = "";
            this.nlog_windowservice.Info($"windows Service exe name:{execFilePath.Replace(".dll", ".exe")}");

            new Task(async () =>
             {
                 this.nlog_windowservice.Info("Start publish");
                 EnableForWindowsService(false);

                 try
                 {
                     var isNetcore = false;
                     var publishPath = string.Empty;
                     if (DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore"))
                     {
                         isNetcore = true;
                         var publishLog = new List<string>();
                         //执行 publish
                         var isSuccess = CommandHelper.RunDotnetExternalExe(ProjectFolderPath, "dotnet",
                             "publish -c Release --runtime win-x64",
                             (r) =>
                             {
                                 this.nlog_iis.Info(r);
                                 publishLog.Add(r);
                             },
                             (er) => this.nlog_windowservice.Error(er));

                         if (!isSuccess)
                         {
                             this.nlog_windowservice.Error("publish error");
                             return;
                         }

                         var publishPathLine = publishLog.FirstOrDefault(r => !string.IsNullOrEmpty(r) && r.EndsWith("\\publish\\"));

                         if (string.IsNullOrEmpty(publishPathLine))
                         {
                             this.nlog_windowservice.Error("can not find publishPath in log");
                             return;
                         }

                         var publishPathArr = publishPathLine.Split(new string[] { " ->" }, StringSplitOptions.None);
                         if (publishPathArr.Length != 2)
                         {
                             this.nlog_windowservice.Error("can not find publishPath in log");
                             return;
                         }

                         publishPath = publishPathArr[1].Trim();
                     }
                     else
                     {
                         //执行 publish
                         var isSuccess = CommandHelper.RunMsbuild(
                             ProjectPath,
                             (r) => { this.nlog_windowservice.Info(r); },
                             (er) => this.nlog_windowservice.Error(er));

                         if (!isSuccess)
                         {
                             this.nlog_windowservice.Error("publish error");
                             return;
                         }

                         publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", "publish");
                     }


                     if (string.IsNullOrEmpty(publishPath) || !Directory.Exists(publishPath))
                     {
                         this.nlog_windowservice.Error("can not find publishPath");
                         return;
                     }

                     var isProjectInstallService = false;
                     if (!isNetcore)
                     {
                         //判断是否是windows PorjectInstall的服务
                         var serviceFile = Path.Combine(publishPath, execFilePath);
                         if (!File.Exists(serviceFile))
                         {
                             this.nlog_windowservice.Error($"exe file can not find in publish folder: {serviceFile}");
                             return;
                         }

                         //读取这个文件有风险 把他copy到temp 目录下进行处理
                         var tempFolder = Path.GetTempPath();
                         var tempDeployFolder = Path.Combine(tempFolder, "antdeploy");
                         if (!Directory.Exists(tempDeployFolder))
                         {
                             Directory.CreateDirectory(tempDeployFolder);
                         }

                         var tempFolderForService = Path.Combine(tempDeployFolder, "deploy_window_service");
                         if (!Directory.Exists(tempFolderForService))
                         {
                             Directory.CreateDirectory(tempFolderForService);
                         }

                         //复制进去之前先把之前的删除掉
                         TempFileHelper.RemoveFileInTempFolder(tempFolderForService);

                         var newserviceFile = TempFileHelper.CopyFileToTempFolder(serviceFile, tempFolderForService);

                         var serviceNameByFile = ProjectHelper.GetServiceNameByFile(newserviceFile);
                         if (!string.IsNullOrEmpty(serviceNameByFile))
                         {
                             //this.nlog_windowservice.Error($"file: {serviceFile} is not a windows service! ");
                             //return;
                             isProjectInstallService = true;
                             serviceName = serviceNameByFile;
                         }

                     }
                     else
                     {
                         execFilePath = execFilePath.Replace(".dll", ".exe");
                         var serviceFile = Path.Combine(publishPath, execFilePath);
                         if (!File.Exists(serviceFile))
                         {
                             this.nlog_windowservice.Error($"exe file can not find in publish folder: {serviceFile}");
                             return;
                         }
                     }



                     LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success,  ==> ");
                     publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                     this.nlog_windowservice.Log(publisEvent);



                     //执行 打包
                     this.nlog_windowservice.Info("Start package");
                     byte[] zipBytes;
                     try
                     {
                         zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true,
                             DeployConfig.IgnoreList);
                     }
                     catch (Exception ex)
                     {
                         this.nlog_windowservice.Error("package fail:" + ex.Message);
                         return;
                     }

                     if (zipBytes == null || zipBytes.Length < 1)
                     {
                         this.nlog_windowservice.Error("package fail");
                         return;
                     }

                     this.nlog_windowservice.Info("package success");
                     var loggerId = Guid.NewGuid().ToString("N");
                     //执行 上传
                     this.nlog_windowservice.Info("Deploy Start");
                     foreach (var server in serverList)
                     {
                         if (string.IsNullOrEmpty(server.Token))
                         {
                             this.nlog_windowservice.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                             continue;
                         }

                         ProgressPercentageForWindowsService = 0;
                         this.nlog_windowservice.Info($"Start Uppload,Host:{server.Host}");
                         HttpRequestClient httpRequestClient = new HttpRequestClient();
                         httpRequestClient.SetFieldValue("publishType", "windowservice");
                         httpRequestClient.SetFieldValue("serviceName", serviceName);
                         httpRequestClient.SetFieldValue("id", loggerId);
                         httpRequestClient.SetFieldValue("sdkType", DeployConfig.WindowsServiveConfig.SdkType);
                         httpRequestClient.SetFieldValue("isProjectInstallService", isProjectInstallService ? "yes" : "no");
                         httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                         httpRequestClient.SetFieldValue("stopTimeOut", DeployConfig.WindowsServiveConfig.StopTimeOutSeconds);
                         httpRequestClient.SetFieldValue("Token", server.Token);
                         httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                         System.Net.WebSockets.Managed.ClientWebSocket webSocket = null;
                         HttpLogger HttpLogger = null;
                         try
                         {
                             var hostKey = "";
                             HttpLogger = new HttpLogger
                             {
                                 Key = loggerId,
                                 Url = $"http://{server.Host}/logger?key=" + loggerId
                             };
                             webSocket = await WebSocketHelper.Connect($"ws://{server.Host}/socket", (receiveMsg) =>
                             {
                                 if (!string.IsNullOrEmpty(receiveMsg))
                                 {
                                     if (receiveMsg.StartsWith("hostKey@"))
                                     {
                                         hostKey = receiveMsg.Replace("hostKey@", "");
                                     }
                                     else
                                     {
                                         this.nlog_windowservice.Info($"【Server】{receiveMsg}");
                                     }
                                 }
                             }, HttpLogger);

                             httpRequestClient.SetFieldValue("wsKey", hostKey);

                             var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                 (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged2; });

                             if (uploadResult.Item1)
                             {
                                 this.nlog_windowservice.Info($"Host:{server.Host},Response:{uploadResult.Item2}");
                             }
                             else
                             {
                                 this.nlog_windowservice.Error(
                                     $"Host:{server.Host},Response:{uploadResult.Item2},Skip to Next");
                             }
                         }
                         catch (Exception ex)
                         {

                             this.nlog_windowservice.Error(
                                 $"Fail Deploy,Host:{server.Host},Response:{ex.Message},Skip to Next");
                         }
                         finally
                         {
                             webSocket?.Dispose();
                             HttpLogger?.Dispose();
                         }

                     }

                     //交互
                 }
                 catch (Exception ex1)
                 {
                     this.nlog_windowservice.Error(ex1);
                 }
                 finally
                 {
                     EnableForWindowsService(true);
                 }



             }).Start();
        }


        #endregion

        #region Common
        private void page__SelectedIndexChanged(object sender, EventArgs e)
        {
            string pase = page_.Tag as string;
            if (string.IsNullOrEmpty(pase))
            {
                return;
            }

            int.TryParse(pase, out var excutePageIndex);
            if (excutePageIndex < 0)
            {
                return;
            }
            if (page_.SelectedIndex == 0)
            {
                if (excutePageIndex != page_.SelectedIndex)
                {
                    this.rich_iis_log.Text = "";
                }
            }
            else if (page_.SelectedIndex == 1)
            {
                if (excutePageIndex != page_.SelectedIndex)
                {
                    this.rich_docker_log.Text = "";
                }
            }
            else if (page_.SelectedIndex == 2)
            {
                if (excutePageIndex != page_.SelectedIndex)
                {
                    this.rich_windowservice_log.Text = "";
                }
            }
        }
        private void DeployConfigOnEnvChangeEvent(Env changeEnv, bool isremove)
        {
            this.combo_iis_env.Items.Clear();
            this.combo_windowservice_env.Items.Clear();
            this.combo_docker_env.Items.Clear();
            if (DeployConfig.Env.Any())
            {
                foreach (var env in DeployConfig.Env)
                {
                    this.combo_iis_env.Items.Add(env.Name);
                    this.combo_windowservice_env.Items.Add(env.Name);
                    this.combo_docker_env.Items.Add(env.Name);
                }
            }

        }

        private void ReadPorjectConfig(string projectPath)
        {
            if (File.Exists(projectPath))
            {
                ProjectFolderPath = new FileInfo(projectPath).DirectoryName;
                ProjectConfigPath = Path.Combine(ProjectFolderPath, "AntDeploy.json");
                if (File.Exists(ProjectConfigPath))
                {
                    var config = File.ReadAllText(ProjectConfigPath, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(config))
                    {
                        DeployConfig = JsonConvert.DeserializeObject<DeployConfig>(config);
                        if (DeployConfig.Env == null) DeployConfig.Env = new List<Env>();
                        if (DeployConfig.IIsConfig == null) DeployConfig.IIsConfig = new IIsConfig();
                        if (DeployConfig.IgnoreList == null) DeployConfig.IgnoreList = new List<string>();
                    }
                }
            }
        }

        private void BeginInvokeLambda(Action action)
        {
            BeginInvoke(action, null);
        }

        private void label_check_update_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://marketplace.visualstudio.com/items?itemName=nainaigu.AntDeploy");
            Process.Start(sInfo);
        }

        #endregion


        #region Docker

        private void b_docker_deploy_Click(object sender, EventArgs e)
        {
            var port = this.txt_docker_port.Text.Trim();
            if (!string.IsNullOrEmpty(port))
            {
                int.TryParse(port, out var dockerPort);
                if (dockerPort == 0)
                {
                    MessageBox.Show("please input right port value");
                    return;
                }
                else
                {
                    DeployConfig.DockerConfig.Prot = port;
                }
            }
            else
            {
                DeployConfig.DockerConfig.Prot = "";
            }

            var aspnetcoreEnvName = this.txt_docker_envname.Text.Trim();
            if (aspnetcoreEnvName.Length > 0)
            {
                DeployConfig.DockerConfig.AspNetCoreEnv = aspnetcoreEnvName;
            }

            var envName = this.combo_docker_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

#if DEBUG
            var ENTRYPOINT = "Lito.APP.dll";
            var SDKVersion = "2.1";
#else
            //必须是netcore应用
            var isNetcoreProject = ProjectHelper.IsDotNetCoreProject(_project);
            if (!isNetcoreProject)
            {
                MessageBox.Show("current project is not netcore");
                return;
            }

            var ENTRYPOINT = _project.GetProjectProperty("OutputFileName");
            if (string.IsNullOrEmpty(ENTRYPOINT))
            {
                MessageBox.Show("get current project property:outputfilename error");
                return;
            }

            var SDKVersion = ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            if (string.IsNullOrEmpty(SDKVersion))
            {
                MessageBox.Show("get current project skd version error");
                return;
            }

           
#endif

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no linux server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBox.Show("Are you sure to deploy to Linux Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            this.rich_docker_log.Text = "";
            this.nlog_docker.Info($"The Porject ENTRYPOINT name:{ENTRYPOINT}");

            new Task(async () =>
            {
                this.nlog_docker.Info("Start publish");
                EnableForDocker(false);

                try
                {
                    var publishLog = new List<string>();
                     //执行 publish
                     var isSuccess = CommandHelper.RunDotnetExternalExe(ProjectFolderPath, "dotnet",
                        "publish -c Release",
                        (r) =>
                        {
                            this.nlog_docker.Info(r);
                            publishLog.Add(r);
                        },
                        (er) => this.nlog_docker.Error(er));

                    if (!isSuccess)
                    {
                        this.nlog_docker.Error("publish error");
                        return;
                    }

                    var publishPathLine = publishLog.FirstOrDefault(r => !string.IsNullOrEmpty(r) && r.EndsWith("\\publish\\"));

                    if (string.IsNullOrEmpty(publishPathLine))
                    {
                        this.nlog_docker.Error("can not find publishPath in log");
                        return;
                    }

                    var publishPathArr = publishPathLine.Split(new string[] { " ->" }, StringSplitOptions.None);
                    if (publishPathArr.Length != 2)
                    {
                        this.nlog_docker.Error("can not find publishPath in log");
                        return;
                    }

                    var publishPath = publishPathArr[1].Trim();

                    if (string.IsNullOrEmpty(publishPath) || !Directory.Exists(publishPath))
                    {
                        this.nlog_docker.Error("can not find publishPath");
                        return;
                    }


                    var serviceFile = Path.Combine(publishPath, ENTRYPOINT);
                    if (!File.Exists(serviceFile))
                    {
                        this.nlog_docker.Error($"ENTRYPOINT file can not find in publish folder: {serviceFile}");
                        return;
                    }

                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success,  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    this.nlog_docker.Log(publisEvent);



                     //执行 打包
                     this.nlog_docker.Info("Start package");
                    MemoryStream zipBytes;
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory2(publishPath, CompressionLevel.Optimal, true,
                            DeployConfig.IgnoreList);
                    }
                    catch (Exception ex)
                    {
                        this.nlog_docker.Error("package fail:" + ex.Message);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_docker.Error("package fail");
                        return;
                    }

                    this.nlog_docker.Info("package success");
                     //执行 上传
                     this.nlog_docker.Info("Deploy Start");
                    foreach (var server in serverList)
                    {
                         #region 参数Check

                         if (string.IsNullOrEmpty(server.Host))
                        {
                            this.nlog_docker.Error("Server Host is Empty");
                            continue;
                        }
                        if (string.IsNullOrEmpty(server.UserName))
                        {
                            this.nlog_docker.Error("Server UserName is Empty");
                            continue;
                        }
                        if (string.IsNullOrEmpty(server.Pwd))
                        {
                            this.nlog_docker.Error("Server Pwd is Empty");
                            continue;
                        }
                        var pwd = CodingHelper.AESDecrypt(server.Pwd);
                        if (string.IsNullOrEmpty(pwd))
                        {
                            this.nlog_docker.Error("Server Pwd is Empty");
                            continue;
                        }
                         #endregion

                        zipBytes.Seek(0, SeekOrigin.Begin);
                        using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, (str) =>
                           {
                            this.nlog_docker.Info("【Server】" + str);
                        })
                        {
                            NetCoreENTRYPOINT = ENTRYPOINT,
                            NetCoreVersion = SDKVersion,
                            NetCorePort = DeployConfig.DockerConfig.Prot,
                            NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv
                        })
                        {
                            var connectResult = sshClient.Connect();
                            if (!connectResult)
                            {
                                this.nlog_docker.Error($"Deploy Host:{server.Host} Fail: connect fail");
                                continue;
                            }

                            try
                            {
                                sshClient.PublishZip(zipBytes, "publisher", "publish.zip");

                                this.nlog_docker.Info($"publish Host: {server.Host} End");
                            }
                            catch (Exception ex)
                            {
                                this.nlog_docker.Error($"Deploy Host:{server.Host} Fail:" + ex.Message);
                            }
                        }
                    }
                    this.nlog_docker.Info("Deploy End");
                }
                catch (Exception ex1)
                {
                    this.nlog_docker.Error(ex1);
                }
                finally
                {
                    EnableForDocker(true);
                }



            }).Start();
        }

        private void EnableForDocker(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {
                this.b_docker_deploy.Enabled = flag;
                this.combo_docker_env.Enabled = flag;
                this.txt_docker_port.Enabled = flag;
                this.txt_docker_envname.Enabled = flag;

                this.page_set.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.page_web_iis.Enabled = flag;

                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_windowservice_log.Text = "";
                }
                else
                {
                    page_.Tag = "1";
                }
            });

        }

        /// <summary>
        /// docker env 环境切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combo_docker_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_docker_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.DockerConfig.LastEnvName = selectName;
            }
        }

        #endregion

      
    }
}
