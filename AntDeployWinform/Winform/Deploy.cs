using AntDeployWinform.Models;
using AntDeployWinform.Util;
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

namespace AntDeployWinform.Winform
{
    [Serializable]
    public partial class Deploy : Form
    {

        private string ProjectConfigPath;
        private string ProjectFolderPath;
        private string ProjectName;
        private string ProjectPath;
        private string PluginConfigPath;//这个是按照项目来的
        private string ConfigPath;//这个是全局配置
        private ProjectParam _project;
        private NLog.Logger nlog_iis;
        private NLog.Logger nlog_windowservice;
        private NLog.Logger nlog_docker;

        private int ProgressPercentage = 0;
        private string ProgressCurrentHost = null;
        private string ProgressCurrentHostForWindowsService = null;
        private int ProgressPercentageForWindowsService = 0;
        private int ProgressBoxLocationLeft = 30;
        public Deploy(string projectPath = null, ProjectParam project = null)
        {

            ConfigPath = ProjectHelper.GetPluginConfigPath();
            ReadConfig(ConfigPath);

            LoadLanguage();

            InitializeComponent();

            Assembly assembly = typeof(Deploy).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo12.ico"))
            {
                if (stream != null) this.Icon = new Icon(stream);
            }

            Init(projectPath, project);

            NlogConfig();
        }



        public DeployConfig DeployConfig { get; set; } = new DeployConfig();
        public PluginConfig PluginConfig { get; set; } = new PluginConfig();
        public GlobalConfig GlobalConfig { get; set; } = new GlobalConfig();

        #region Form

        private void Init(string projectPath, ProjectParam project = null, bool isFirst = true)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                if (isFirst)
                {
                    return;
                }

                this.page_set.Enabled = false;
                this.page_docker.Enabled = false;
                this.page_window_service.Enabled = false;
                this.page_web_iis.Enabled = false;
                this.pag_advance_setting.Enabled = false;
                SelectProject selectProject = new SelectProject(GlobalConfig.ProjectPathList);
                var r = selectProject.ShowDialog();
                if (r == DialogResult.Cancel)
                {
                    this.Close();
                    return;
                }
                else
                {
                    projectPath = selectProject.SelectProjectPath;
                    //保存记录


                    this.page_set.Enabled = true;
                    this.page_docker.Enabled = true;
                    this.page_window_service.Enabled = true;
                    this.page_web_iis.Enabled = true;
                    this.pag_advance_setting.Enabled = true;

                    this.BringToFront();
                }

            }
            else
            {
                if (!isFirst) return;
            }
            ProjectName = Path.GetFileNameWithoutExtension(projectPath);
            this.Text += $"(Version:{Vsix.VERSION})[{ProjectName}]";
            ProjectPath = projectPath;
            if (project == null)
            {
                //读配置
                project = ProjectHelper.GetNetCoreParamInCsprojectFile(projectPath);
            }
            _project = project;
            CommandHelper.MsBuildPath = _project?.MsBuildPath;
            ReadPorjectConfig(projectPath);
            PluginConfigPath = ProjectHelper.GetPluginConfigPath(projectPath);
            ReadPluginConfig(PluginConfigPath);
            if (!isFirst)
            {
                if (GlobalConfig.ProjectPathList == null) GlobalConfig.ProjectPathList = new List<string>();
                GlobalConfig.ProjectPathList.Insert(0, projectPath);
                GlobalConfig.ProjectPathList = GlobalConfig.ProjectPathList.Distinct().ToList();
                Reload();
            }
        }

        private void NlogConfig()
        {

            #region Nlog

            var config = new LoggingConfiguration();
            var richTarget = new RichTextBoxTarget
            {
                Name = "rich_iis_log",
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
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
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
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
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
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


        private void LoadLanguage()
        {
            try
            {

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GlobalConfig.IsChinease ? "zh-CN" : "");
            }
            catch (Exception)
            {

            }
        }

        private void Deploy_Load(object sender, EventArgs e)
        {
            Reload();
            WebUtil.SetAllowUnsafeHeaderParsing20();
        }
        private void Deploy_Activated(object sender, EventArgs e)
        {
            this.Activated -= Deploy_Activated;
            Init(ProjectPath, _project, false);
        }
        private void Reload()
        {
            this.checkBox_Chinese.Checked = GlobalConfig.IsChinease;

            //只要有传了msbuild来初始化的话 就覆盖原来配置的
            if (!string.IsNullOrEmpty(CommandHelper.MsBuildPath))
            {
                GlobalConfig.MsBuildPath = CommandHelper.MsBuildPath;
            }

            if (!string.IsNullOrEmpty(GlobalConfig.MsBuildPath))
            {
                this.txt_msbuild_path.Text = GlobalConfig.MsBuildPath;
                CommandHelper.MsBuildPath = GlobalConfig.MsBuildPath;
            }
            else
            {
                var msbuildPath = CommandHelper.GetMsBuildPath();
                if (!string.IsNullOrEmpty(msbuildPath))
                {
                    this.txt_msbuild_path.Text = msbuildPath;
                    CommandHelper.MsBuildPath = GlobalConfig.MsBuildPath = msbuildPath;
                }
            }

            //计算pannel的起始位置
            var size = this.ClientSize.Width - 646;
            if (size > 0)
            {
                ProgressBoxLocationLeft += size;
            }

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


            if (DeployConfig.IIsConfig != null)
            {
                if (this.combo_iis_sdk_type.Items.Count > 0 && !string.IsNullOrEmpty(DeployConfig.IIsConfig.SdkType)
                                                            && this.combo_iis_sdk_type.Items.Cast<string>()
                                                                .Contains(DeployConfig.IIsConfig.SdkType))
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
                if (this.combo_windowservice_sdk_type.Items.Count > 0 && !string.IsNullOrEmpty(DeployConfig
                                                                          .WindowsServiveConfig.SdkType)
                                                                      && this.combo_windowservice_sdk_type.Items
                                                                          .Cast<string>()
                                                                          .Contains(DeployConfig.WindowsServiveConfig
                                                                              .SdkType))
                {
                    this.combo_windowservice_sdk_type.SelectedItem = DeployConfig.WindowsServiveConfig.SdkType;
                }

                if (this.combo_windowservice_env.Items.Count > 0 &&
                    !string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.LastEnvName)
                    && this.combo_windowservice_env.Items.Cast<string>()
                        .Contains(DeployConfig.WindowsServiveConfig.LastEnvName))
                {
                    this.combo_windowservice_env.SelectedItem = DeployConfig.WindowsServiveConfig.LastEnvName;
                }

                if (!string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.ServiceName))
                {
                    this.txt_windowservice_name.Text = DeployConfig.WindowsServiveConfig.ServiceName;
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

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.RemoveDaysFromPublished))
                {
                    this.t_docker_delete_days.Text = DeployConfig.DockerConfig.RemoveDaysFromPublished;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.Volume))
                {
                    this.txt_docker_volume.Text = DeployConfig.DockerConfig.Volume;
                }
            }

            if (PluginConfig.LastTabIndex >= 0 && PluginConfig.LastTabIndex < this.tabcontrol.TabPages.Count)
            {
                this.tabcontrol.SelectedIndex = PluginConfig.LastTabIndex;
            }

            if (!string.IsNullOrEmpty(PluginConfig.NetCorePublishMode))
            {
                this.combo_netcore_publish_mode.SelectedItem = PluginConfig.NetCorePublishMode;
            }

            this.checkBox_Increment_iis.Checked = PluginConfig.IISEnableIncrement;
            this.checkBox_Increment_window_service.Checked = PluginConfig.WindowsServiceEnableIncrement;
            this.checkBox_select_deploy_service.Checked = PluginConfig.WindowsServiceEnableSelectDeploy;
            this.checkBox_select_deploy_iis.Checked = PluginConfig.IISEnableSelectDeploy;


            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
            this.txt_linux_host.Text = string.Empty;
            this.txt_linux_username.Text = string.Empty;
            this.txt_linux_pwd.Text = string.Empty;
            this.txt_winserver_nickname.Text = string.Empty;
            this.txt_linux_server_nickname.Text = string.Empty;
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
            Unload();

            RichTextBoxTarget.GetTargetByControl(rich_iis_log)?.Dispose();
            RichTextBoxTarget.GetTargetByControl(rich_windowservice_log)?.Dispose();
            RichTextBoxTarget.GetTargetByControl(rich_docker_log)?.Dispose();



            this.b_iis_rollback.Dispose();
            this.b_docker_rollback.Dispose();
            this.b_windows_service_rollback.Dispose();

            this.txt_docker_volume.Dispose();

            this.loading_win_server_test.Dispose();
            this.loading_linux_server_test.Dispose();

            this.rich_docker_log.Dispose();
            this.rich_iis_log.Dispose();
            this.rich_windowservice_log.Dispose();
        }

        private void Unload()
        {
            try
            {
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        //this.tabPage_docker.Controls.Remove(box.Value);
                    }
                }

                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList2)
                {
                    foreach (var box in progressBoxList2)
                    {
                        box.Value.Dispose();
                        //this.tabPage_progress.Controls.Remove(box.Value);
                    }
                }

                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList3)
                {
                    foreach (var box in progressBoxList3)
                    {
                        box.Value.Dispose();
                        //this.tabPage_windows_service.Controls.Remove(box.Value);
                    }
                }

                GlobalConfig.MsBuildPath = this.txt_msbuild_path.Text.Trim();
                GlobalConfig.ProjectPathList = GlobalConfig.ProjectPathList.Take(10).ToList();
                PluginConfig.LastTabIndex = this.tabcontrol.SelectedIndex;
                PluginConfig.IISEnableIncrement = this.checkBox_Increment_iis.Checked;
                PluginConfig.WindowsServiceEnableIncrement = this.checkBox_Increment_window_service.Checked;
                PluginConfig.IISEnableSelectDeploy = this.checkBox_select_deploy_iis.Checked;
                PluginConfig.WindowsServiceEnableSelectDeploy = this.checkBox_select_deploy_service.Checked;

                DeployConfig.IIsConfig.WebSiteName = this.txt_iis_web_site_name.Text.Trim();

                DeployConfig.WindowsServiveConfig.ServiceName = this.txt_windowservice_name.Text.Trim();

                DeployConfig.DockerConfig.Prot = this.txt_docker_port.Text.Trim();
                DeployConfig.DockerConfig.AspNetCoreEnv = this.txt_docker_envname.Text.Trim();
                DeployConfig.DockerConfig.RemoveDaysFromPublished = this.t_docker_delete_days.Text.Trim();
                DeployConfig.DockerConfig.Volume = this.txt_docker_volume.Text.Trim();

                if (!string.IsNullOrEmpty(ProjectConfigPath))
                {
                    var configJson = JsonConvert.SerializeObject(DeployConfig, Formatting.Indented);
                    File.WriteAllText(ProjectConfigPath, configJson, Encoding.UTF8);
                }

                if (!string.IsNullOrEmpty(PluginConfigPath))
                {
                    var configJson = JsonConvert.SerializeObject(PluginConfig, Formatting.Indented);
                    File.WriteAllText(PluginConfigPath, configJson, Encoding.UTF8);
                }
                if (!string.IsNullOrEmpty(ConfigPath))
                {
                    var configJson = JsonConvert.SerializeObject(GlobalConfig, Formatting.Indented);
                    File.WriteAllText(ConfigPath, configJson, Encoding.UTF8);
                }


            }
            catch (Exception)
            {
                //ignore
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

            //检查特殊字符
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (env_name.Contains(c))
                {
                    MessageBox.Show("env name contains invalid char：" + c);
                    return;
                }
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
                this.txt_winserver_nickname.Text = string.Empty;
                this.txt_linux_server_nickname.Text = string.Empty;
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
            this.list_env_ignore.Items.Clear();
            this.list_backUp_ignore.Items.Clear();

            var selectedEnv = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEnv))
            {
                this.b_env_server_add.Enabled = false;
                this.b_env_server_remove.Enabled = false;
                this.b_env_server_test.Enabled = false;
                this.txt_env_server_host.Text = string.Empty;
                this.txt_env_server_token.Text = string.Empty;
                this.txt_winserver_nickname.Text = string.Empty;

                this.b_linux_server_test.Enabled = false;
                this.b_linux_server_remove.Enabled = false;
                this.b_add_linux_server.Enabled = false;

                this.txt_linux_username.Text = string.Empty;
                this.txt_linux_host.Text = string.Empty;
                this.txt_linux_pwd.Text = string.Empty;
                this.txt_linux_server_nickname.Text = string.Empty;

                this.txt_env_ignore.Text = string.Empty;
                this.txt_backUp_ignore.Text = string.Empty;

                this.b_env_ignore_add.Enabled = false;
                this.b_copy_pack_ignore.Enabled = false;
                this.b_env_ignore_remove.Enabled = false;
                this.b_backUp_ignore_add.Enabled = false;
                this.b_copy_backup_ignore.Enabled = false;
                this.b_backUp_ignore_remove.Enabled = false;

                return;
            }


            this.b_env_server_add.Enabled = true;
            this.b_env_server_remove.Enabled = true;
            this.b_env_server_test.Enabled = true;

            this.b_linux_server_test.Enabled = true;
            this.b_linux_server_remove.Enabled = true;
            this.b_add_linux_server.Enabled = true;


            this.b_env_ignore_add.Enabled = true;
            this.b_copy_pack_ignore.Enabled = true;
            this.b_env_ignore_remove.Enabled = true;
            this.b_backUp_ignore_add.Enabled = true;
            this.b_copy_backup_ignore.Enabled = true;
            this.b_backUp_ignore_remove.Enabled = true;

            var env = this.DeployConfig.Env.FirstOrDefault(r => r.Name.Equals(selectedEnv));

            if (env != null)
            {
                env.ServerList.ForEach(r =>
                {
                    this.combo_env_server_list.Items.Add(r.Host + "@_@" + r.Token + (!string.IsNullOrWhiteSpace(r.NickName) ? "@_@" + r.NickName : ""));
                });


                env.LinuxServerList.ForEach(r =>
                {
                    this.combo_linux_server_list.Items.Add(r.Host + "@_@" + r.UserName + (!string.IsNullOrWhiteSpace(r.NickName) ? "@_@" + r.NickName : ""));
                });

                foreach (var item in env.IgnoreList)
                {
                    this.list_env_ignore.Items.Add(item);
                }

                foreach (var item in env.WindowsBackUpIgnoreList)
                {
                    this.list_backUp_ignore.Items.Add(item);
                }
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


            if (this.list_env_ignore.Items.Count > 0)
            {
                this.list_env_ignore.Tag = "active";
                this.list_env_ignore.SelectedIndex = 0;
            }
            if (this.list_backUp_ignore.Items.Count > 0)
            {
                this.list_backUp_ignore.Tag = "active";
                this.list_backUp_ignore.SelectedIndex = 0;
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
            this.txt_winserver_nickname.Text = string.Empty;
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
            if (arr.Length >= 2)
            {
                this.txt_env_server_host.Text = arr[0];
                this.txt_env_server_token.Text = arr[1];
                if (arr.Length == 3)
                {
                    this.txt_winserver_nickname.Text = arr[2];
                }
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

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].ServerList
                .RemoveAt(this.combo_env_server_list.SelectedIndex);
            this.combo_env_server_list.Items.Remove(seletedServer);
            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);
        }

        /// <summary>
        /// 添加window server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_env_server_add_Click(object sender, EventArgs e)
        {
            var nickName = this.txt_winserver_nickname.Text.Trim();

            if (!string.IsNullOrWhiteSpace(nickName) && nickName.Contains("@_@"))
            {
                MessageBox.Show("nickName can not Contains @_@");
                return;
            }

            var serverHost = this.txt_env_server_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            if (serverHost.Contains("@_@"))
            {
                MessageBox.Show("server host can not Contains @_@");
                return;
            }

            var serverTolen = this.txt_env_server_token.Text.Trim();
            if (serverTolen.Length < 1)
            {
                MessageBox.Show("please input server Token");
                return;
            }

            if (serverTolen.Contains("@_@"))
            {
                MessageBox.Show("server Token can not Contains @_@");
                return;
            }

            var existServer = this.combo_env_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBox.Show("input server host is exist!" + Environment.NewLine +
                                "if you want to modify,please remove and readd");
                return;
            }

            var newServer = serverHost + "@_@" + serverTolen + (!string.IsNullOrWhiteSpace(nickName) ? "@_@" + nickName : "");
            this.combo_env_server_list.Items.Add(newServer);
            DeployConfig.Env[this.combo_env_list.SelectedIndex].ServerList.Add(new Server
            {
                Host = serverHost,
                Token = serverTolen,
                NickName = nickName
            });

            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);

            this.combo_env_server_list.SelectedItem = newServer;
            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
            this.txt_winserver_nickname.Text = string.Empty;
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

            this.loading_win_server_test.Visible = true;
            new Task(() =>
            {
                EnableForTestWinServer(false);
                WebClient client = new WebClient();
                try
                {


                    client.Proxy = null;
#if DEBUG
                    //client.Proxy = new WebProxy("127.0.0.1:5389");
#endif
                    var result = client.DownloadString($"http://{serverHost}/publish?Token={WebUtility.UrlEncode(serverTolen)}");

                    this.BeginInvokeLambda(() =>
                    {
                        if (result.Equals("success"))
                        {
                            MessageBox.Show("Connect Sussess");
                        }
                        else
                        {
                            MessageBox.Show("Connect fail");
                        }
                    });
                }
                catch (Exception)
                {
                    this.BeginInvokeLambda(() => { MessageBox.Show("Connect Fail"); });
                }
                finally
                {
                    EnableForTestWinServer(true);
                    client.Dispose();
                }

            }).Start();

        }

        private void EnableForTestWinServer(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                this.txt_env_server_host.Enabled = flag;
                this.txt_env_server_token.Enabled = flag;
                this.b_env_server_add.Enabled = flag;
                this.b_env_server_test.Enabled = flag;
                this.b_env_server_remove.Enabled = flag;
                this.combo_env_server_list.Enabled = flag;
                this.txt_winserver_nickname.Enabled = flag;
                if (flag)
                {
                    this.loading_win_server_test.Visible = false;
                }
            });

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

            if (ignoreTxt.Contains("@_@"))
            {
                MessageBox.Show("can not contains @_@");
                return;
            }

            var existIgnore = this.list_env_ignore.Items.Cast<string>().FirstOrDefault(r => r.Equals(ignoreTxt));
            if (!string.IsNullOrEmpty(existIgnore))
            {
                this.list_env_ignore.SelectedItem = existIgnore;
            }
            else
            {
                this.DeployConfig.Env[this.combo_env_list.SelectedIndex].IgnoreList.Add(ignoreTxt);
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
            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].IgnoreList.RemoveAt(this.list_env_ignore.SelectedIndex);
            this.list_env_ignore.Items.RemoveAt(this.list_env_ignore.SelectedIndex);
            if (this.list_env_ignore.Items.Count >= 0)
                this.list_env_ignore.SelectedIndex = this.list_env_ignore.Items.Count - 1;
        }

        private void b_backUp_ignore_add_Click(object sender, EventArgs e)
        {
            var ignoreTxt = this.txt_backUp_ignore.Text.Trim();
            if (ignoreTxt.Length < 1)
            {
                MessageBox.Show("please input backUp ignore rule");
                return;
            }
            if (ignoreTxt.Contains("@_@"))
            {
                MessageBox.Show("can not contains @_@");
                return;
            }

            var existIgnore = this.list_backUp_ignore.Items.Cast<string>().FirstOrDefault(r => r.Equals(ignoreTxt));
            if (!string.IsNullOrEmpty(existIgnore))
            {
                this.list_backUp_ignore.SelectedItem = existIgnore;
            }
            else
            {
                this.DeployConfig.Env[this.combo_env_list.SelectedIndex].WindowsBackUpIgnoreList.Add(ignoreTxt);
                this.list_backUp_ignore.Items.Add(ignoreTxt);
                this.txt_backUp_ignore.Text = string.Empty;
                this.list_backUp_ignore.SelectedItem = ignoreTxt;
            }
        }

        private void b_backUp_ignore_remove_Click(object sender, EventArgs e)
        {
            if (this.list_backUp_ignore.SelectedIndex < 0) return;
            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].WindowsBackUpIgnoreList.RemoveAt(this.list_backUp_ignore.SelectedIndex);
            this.list_backUp_ignore.Items.RemoveAt(this.list_backUp_ignore.SelectedIndex);
            if (this.list_backUp_ignore.Items.Count >= 0)
                this.list_backUp_ignore.SelectedIndex = this.list_backUp_ignore.Items.Count - 1;
        }

        /// <summary>
        /// 添加linux server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_add_linux_server_Click(object sender, EventArgs e)
        {
            var nickName = this.txt_linux_server_nickname.Text.Trim();
            if (!string.IsNullOrWhiteSpace(nickName) && nickName.Contains("@_@"))
            {
                MessageBox.Show("nickName can not Contains @_@");
                return;
            }
            var serverHost = this.txt_linux_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBox.Show("please input server host");
                return;
            }

            if (serverHost.Contains("@_@"))
            {
                MessageBox.Show("server host can not Contains @_@");
                return;
            }

            var userName = this.txt_linux_username.Text.Trim();
            if (userName.Length < 1)
            {
                MessageBox.Show("please input server userName");
                return;
            }

            if (userName.Contains("@_@"))
            {
                MessageBox.Show("userName can not Contains @_@");
                return;
            }

            var pwd = this.txt_linux_pwd.Text.Trim();
            if (pwd.Length < 1)
            {
                MessageBox.Show("please input server pwd");
                return;
            }

            if (pwd.Contains("@_@"))
            {
                MessageBox.Show("pwd can not Contains @_@");
                return;
            }

            var pwd2 = CodingHelper.AESEncrypt(pwd);
            var existServer = this.combo_linux_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBox.Show("input server host is exist!" + Environment.NewLine +
                                "if you want to modify,please remove and readd");
                return;
            }

            var newServer = serverHost + "@_@" + userName + (!string.IsNullOrWhiteSpace(nickName) ? "@_@" + nickName : "");
            this.combo_linux_server_list.Items.Add(newServer);
            DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList.Add(new LinuxServer
            {
                Host = serverHost,
                UserName = userName,
                Pwd = pwd2,
                NickName = nickName
            });
            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);
            this.combo_linux_server_list.SelectedItem = newServer;
            this.txt_linux_host.Text = string.Empty;
            this.txt_linux_username.Text = string.Empty;
            this.txt_linux_pwd.Text = string.Empty;
            this.txt_linux_server_nickname.Text = string.Empty;
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

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList
                .RemoveAt(this.combo_linux_server_list.SelectedIndex);
            this.combo_linux_server_list.Items.Remove(seletedServer);
            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);
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

            this.loading_linux_server_test.Visible = true;
            new Task(() =>
            {
                EnableForTestLinuxServer(false);
                try
                {
                    using (SSHClient sshClient =
                        new SSHClient(serverHost, userName, pwd, Console.WriteLine, Console.WriteLine))
                    {
                        var r = sshClient.Connect(true);
                        this.BeginInvokeLambda(() =>
                        {
                            if (r)
                            {
                                MessageBox.Show("Connect Success");
                            }
                            else
                            {
                                MessageBox.Show("Connect Fail");
                            }
                        });

                    }
                }
                catch (Exception)
                {
                    this.BeginInvokeLambda(() => { MessageBox.Show("Connect Fail"); });
                }
                finally
                {
                    EnableForTestLinuxServer(true);
                }
            }).Start();

        }

        private void EnableForTestLinuxServer(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                this.txt_linux_host.Enabled = flag;
                this.txt_linux_username.Enabled = flag;
                this.txt_linux_pwd.Enabled = flag;
                this.b_add_linux_server.Enabled = flag;
                this.b_linux_server_test.Enabled = flag;
                this.b_linux_server_remove.Enabled = flag;
                this.combo_linux_server_list.Enabled = flag;
                txt_linux_server_nickname.Enabled = flag;
                if (flag)
                {
                    this.loading_linux_server_test.Visible = false;
                }
            });

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
            this.txt_linux_server_nickname.Text = string.Empty;

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
                if (arr.Length == 3)
                {
                    this.txt_linux_server_nickname.Text = arr[2];
                }
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


        private bool CheckFireUri(ServerType serverType)
        {
            if (serverType.Equals(ServerType.IIS))
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (!box.Value.CheckFireUrl())
                        {
                            return true;
                        }
                    }
                }
            }
            if (serverType.Equals(ServerType.DOCKER))
            {
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (!box.Value.CheckFireUrl())
                        {
                            return true;
                        }
                    }
                }
            }

            if (serverType.Equals(ServerType.WINSERVICE))
            {
                //生成进度
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (!box.Value.CheckFireUrl())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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
                        box.Value.Dispose();
                        this.tabPage_progress.Controls.Remove(box.Value);
                    }
                    this.tabPage_progress.Tag = null;
                }

                var newBoxList = new Dictionary<string, ProgressBox>();

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
                    var nickName = serverList[i].NickName;
                    ProgressBox newBox =
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.IIS)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    newBoxList.Add(serverHost, newBox);
                    this.tabPage_progress.Controls.Add(newBox);
                }

                this.tabPage_progress.Tag = newBoxList;

                this.progress_iis_tip.SendToBack();
            }
            else
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_progress.Controls.Remove(box.Value);
                    }

                }
            }
        }


        private void b_iis_deploy_Click(object sender, EventArgs e)
        {

            //判断当前项目是否是web项目
            bool isWeb = _project.IsWebProejct || _project.IsNetcorePorject;

            if (!isWeb)
            {
                //检查工程文件里面是否含有 WebProjectProperties字样
                if (ProjectHelper.IsWebProject(ProjectPath))
                {
                    isWeb = true;
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

            //fire url
            if (CheckFireUri(ServerType.IIS))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }

            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;
            var backUpIgnoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).WindowsBackUpIgnoreList;

            var Port = this.txt_iis_port.Text.Trim();
            var PoolName = this.txt_pool_name.Text.Trim();
            var PhysicalPath = this.txt_iis_PhysicalPath.Text.Trim();

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = ShowInputMsgBox("Confirm Deploy!!",
                "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList);
            //var confirmResult = MessageBox.Show(
            //    "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList,
             //   "Confirm Deploy!!",
             //   MessageBoxButtons.YesNo);
            if (!confirmResult.Item1)
            {
                return;
            }


            combo_iis_env_SelectedIndexChanged(null, null);

            this.rich_iis_log.Text = "";
            DeployConfig.IIsConfig.WebSiteName = websiteName;
            new Task(async () =>
            {
                this.nlog_iis.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
                PrintCommonLog(this.nlog_iis);
                Enable(false); //第一台开始编译
                GitClient gitModel = null;
                var gitPath = string.Empty;
                var webFolderName = string.Empty;
                try
                {
                    var isNetcore = false;
                    var publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_iis", envName);
                    var path = publishPath + "\\";
                    if (DeployConfig.IIsConfig.SdkType.Equals("netcore"))
                    {
                        //执行 publish
                        var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                            $"publish \"{ProjectPath}\" -c Release{PluginConfig.GetNetCorePublishRuntimeArg()}", this.nlog_iis);

                        if (!isSuccess)
                        {
                            this.nlog_iis.Error("publish error,please check build log");
                            BuildError(this.tabPage_progress);
                            return;
                        }

                        isNetcore = true;
                    }
                    else
                    {
                        var isSuccess = CommandHelper.RunMsbuild(ProjectPath, path, this.nlog_iis, true);

                        if (!isSuccess)
                        {
                            this.nlog_iis.Error("publish error,please check build log");
                            BuildError(this.tabPage_progress);
                            return;
                        }

                    }

                    BuildEnd(this.tabPage_progress); //第一台结束编译
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent.LoggerName = "rich_iis_log";
                    this.nlog_iis.Log(publisEvent);



                    //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis

                    if (isNetcore)
                    {
                        var webConfig = Path.Combine(publishPath, "Web.Config");
                        if (!File.Exists(webConfig))
                        {
                            LogEventInfo theEvent = new LogEventInfo(LogLevel.Warn, "",
                                "publish sdkType:netcore ,but web.config file missing!");
                            theEvent.LoggerName = "rich_iis_log";
                            theEvent.Properties["ShowLink"] =
                                "https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis";
                            this.nlog_iis.Log(theEvent);
                        }
                    }


                    //执行 打包
                    this.nlog_iis.Info("-----------------Start package-----------------");

                    //查看是否开启了增量
                    if (this.PluginConfig.IISEnableIncrement)
                    {
                        this.nlog_iis.Info("Enable Increment Deploy:true");
                        if (string.IsNullOrEmpty(gitPath))
                        {
                            gitModel = new GitClient(publishPath, this.nlog_iis);
                        }
                        else
                        {
                            gitModel = new GitClient(gitPath, this.nlog_iis);
                        }

                        if (!gitModel.InitSuccess)
                        {
                            this.nlog_iis.Error("package fail,can not init git,please cancel Increment Deploy");
                            PackageError(this.tabPage_progress);
                            return;
                        }
                    }

                    byte[] zipBytes = null;
                    var gitChangeFileCount = 0;
                    if (gitModel != null)
                    {
                        var fileList = gitModel.GetChanges();
                        if (!string.IsNullOrEmpty(gitPath) && !string.IsNullOrEmpty(webFolderName))
                        {
                            var removeLength = (webFolderName + "/").Length;
                            fileList = fileList.Where(r => r.StartsWith(webFolderName + "/"))
                                .Select(r => r.Substring(removeLength)).ToList();
                        }

                        if (fileList == null || fileList.Count < 1)
                        {
                            PackageError(this.tabPage_progress);
                            return;
                        }
                        gitChangeFileCount = fileList.Count;
                        this.nlog_iis.Info("【git】Increment package file count:" + gitChangeFileCount);

                        if (this.PluginConfig.IISEnableSelectDeploy)
                        {
                            this.nlog_iis.Info("-----------------Select File Start-----------------");
                            this.BeginInvokeLambda(() =>
                            {
                                var slectFileForm = new SelectFile(fileList, publishPath);
                                slectFileForm.ShowDialog();
                                // ReSharper disable once AccessToDisposedClosure
                                DoSelectDeployIIS(slectFileForm.SelectedFileList, publishPath, serverList, backUpIgnoreList, Port, PoolName, PhysicalPath,gitModel,confirmResult.Item2);
                            });
                            return;
                        }

                        try
                        {
                            this.nlog_iis.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal,
                                true, ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_progress, null, progressValue); //打印打包记录
                                });
                        }
                        catch (Exception ex)
                        {
                            this.nlog_iis.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_progress);
                            return;
                        }
                    }
                    else if (this.PluginConfig.IISEnableSelectDeploy)
                    {
                        this.nlog_iis.Info("-----------------Select File Start-----------------");
                        this.BeginInvokeLambda(() =>
                        {
                            var slectFileForm = new SelectFile(publishPath);
                            slectFileForm.ShowDialog();
                            DoSelectDeployIIS(slectFileForm.SelectedFileList, publishPath, serverList, backUpIgnoreList, Port, PoolName, PhysicalPath,  null, confirmResult.Item2);
                        });


                        return;
                    }

                    try
                    {
                        if (zipBytes == null)
                        {
                            this.nlog_iis.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_progress, null, progressValue); //打印打包记录
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.nlog_iis.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_progress);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_iis.Error("package fail");
                        PackageError(this.tabPage_progress);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_iis.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    var dateTimeFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    //执行 上传
                    this.nlog_iis.Info("-----------------Deploy Start-----------------");
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        if (index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_progress, server.Host);
                            UpdatePackageProgress(this.tabPage_progress, server.Host, 100);
                        }

                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_iis.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_progress, server.Host);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        ProgressPercentage = 0;
                        ProgressCurrentHost = server.Host;
                        this.nlog_iis.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "iis");
                        httpRequestClient.SetFieldValue("isIncrement",
                            this.PluginConfig.IISEnableIncrement ? "true" : "");
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.IIsConfig.SdkType);
                        httpRequestClient.SetFieldValue("port", Port);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("poolName", PoolName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("remark", confirmResult.Item2);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_iis, HttpLogger);
                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");

                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged; });

                            if (ProgressPercentage == 0) UploadError(this.tabPage_progress, server.Host);
                            if (ProgressPercentage > 0 && ProgressPercentage < 100)
                                UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.IIsFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", $"Host:{getHostDisplayName(server)} Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.LoggerName = "rich_iis_log";
                                        publisEvent22.Properties["ShowLink"] = server.IIsFireUrl;
                                        this.nlog_iis.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.IIsFireUrl, this.nlog_iis);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                            this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            allSuccess = false;
                                            failCount++;
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            this.nlog_iis.Error($"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                        index++;
                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_iis.Info("Deploy Version：" + dateTimeFolderName);
                        if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                    }

                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.LoggerName = "rich_iis_log";
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    this.nlog_iis.Log(publisEvent2);
                    this.nlog_iis.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    zipBytes = null;

                }
                catch (Exception ex1)
                {
                    this.nlog_iis.Error(ex1);
                }
                finally
                {
                    if (!this.PluginConfig.IISEnableSelectDeploy)
                    {
                        ProgressPercentage = 0;
                        ProgressCurrentHost = null;
                        Enable(true);
                        gitModel?.Dispose();
                    }

                }


            }).Start();

        }

        private void DoSelectDeployIIS(List<string> fileList, string publishPath, List<Server> serverList, List<string> backUpIgnoreList, string Port, string PoolName, string PhysicalPath, GitClient gitModel,string remark)
        {
            new Task(async () =>
            {
                try
                {
                    if (fileList == null || !fileList.Any())
                    {
                        PackageError(this.tabPage_progress);
                        this.nlog_iis.Error("Please Select Files");
                        return;
                    }
                    this.nlog_iis.Info("Select Files count:"+ fileList.Count);
                    this.nlog_iis.Debug("ignore package ignoreList");
                    byte[] zipBytes = null;
                    List<string> ignoreList = new List<string>();
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, true,
                            ignoreList,
                            (progressValue) =>
                            {
                                UpdatePackageProgress(this.tabPage_progress, null, progressValue); //打印打包记录
                            }, true);
                    }
                    catch (Exception ex)
                    {
                        this.nlog_iis.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_progress);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_iis.Error("package fail");
                        PackageError(this.tabPage_progress);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_iis.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    var dateTimeFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    //执行 上传
                    this.nlog_iis.Info("-----------------Deploy Start-----------------");
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        if (index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_progress, server.Host);
                            UpdatePackageProgress(this.tabPage_progress, server.Host, 100);
                        }

                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_iis.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_progress, server.Host);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        ProgressPercentage = 0;
                        ProgressCurrentHost = server.Host;
                        this.nlog_iis.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "iis");
                        httpRequestClient.SetFieldValue("isIncrement", "true");
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.IIsConfig.SdkType);
                        httpRequestClient.SetFieldValue("port", Port);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("remark", remark);
                        httpRequestClient.SetFieldValue("poolName", PoolName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_iis, HttpLogger);
                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");

                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged; });

                            if (ProgressPercentage == 0) UploadError(this.tabPage_progress, server.Host);
                            if (ProgressPercentage > 0 && ProgressPercentage < 100)
                                UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.IIsFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", $"Host:{getHostDisplayName(server)} Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.LoggerName = "rich_iis_log";
                                        publisEvent22.Properties["ShowLink"] = server.IIsFireUrl;
                                        this.nlog_iis.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.IIsFireUrl, this.nlog_iis);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                            this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            allSuccess = false;
                                            failCount++;
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            this.nlog_iis.Error($"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                        index++;
                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_iis.Info("Deploy Version：" + dateTimeFolderName);
                        if (gitModel != null) gitModel.SubmitSelectedChanges(fileList,publishPath);
                    }

                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.LoggerName = "rich_iis_log";
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    this.nlog_iis.Log(publisEvent2);
                    this.nlog_iis.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    zipBytes = null;

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
                    gitModel?.Dispose();
                }


            }).Start();
        }

        private string getHostDisplayName(BaseServer server)
        {
            return $"{server.Host}{(!string.IsNullOrWhiteSpace(server.NickName) ? $"【{server.NickName}】" : "")}";
        }

        private void b_iis_rollback_Click(object sender, EventArgs e)
        {
            //判断当前项目是否是web项目
            bool isWeb = _project.IsNetcorePorject || _project.IsWebProejct;

            if (!isWeb)
            {
                //检查工程文件里面是否含有 WebProjectProperties字样
                if (ProjectHelper.IsWebProject(ProjectPath))
                {
                    isWeb = true;
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


            var envName = this.combo_iis_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no server set yet!");
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.IIS))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBox.Show(
                "Are you sure to rollback to Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_iis_env_SelectedIndexChanged(null, null);

            this.rich_iis_log.Text = "";
            DeployConfig.IIsConfig.WebSiteName = websiteName;
            this.tab_iis.SelectedIndex = 1;
            new Task(async () =>
            {
                var versionList = new List<string>();
                try
                {
                    Enable(false, true);
                    var firstServer = serverList.First();

                    if (string.IsNullOrEmpty(firstServer.Host))
                    {
                        this.nlog_iis.Error($"Server Host is Empty!");
                        return;
                    }

                    if (string.IsNullOrEmpty(firstServer.Token))
                    {
                        this.nlog_iis.Error($"Server Token is Empty!");
                        return;
                    }

                    this.nlog_iis.Info("Start get rollBack version list from first Server:" + firstServer.Host);

                    var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                        $"http://{firstServer.Host}/version", new
                        {
                            Token = firstServer.Token,
                            Type = "iis",
                            Name = DeployConfig.IIsConfig.WebSiteName,
                            WithArgs = true
                        }, nlog_iis);

                    if (getVersionResult == null)
                    {
                        this.nlog_iis.Error($"get rollBack version list fail");
                        return;
                    }

                    if (!string.IsNullOrEmpty(getVersionResult.Msg))
                    {
                        this.nlog_iis.Error($"get rollBack version list fail：" + getVersionResult.Msg);
                        return;
                    }

                    versionList = getVersionResult.Data;
                }
                catch (Exception ex1)
                {
                    this.nlog_iis.Error(ex1);
                    return;
                }
                finally
                {
                    Enable(true);
                }

                if (versionList == null || versionList.Count < 1)
                {
                    this.nlog_iis.Error($"get rollBack version list count = 0");
                    return;
                }

                this.BeginInvokeLambda(() =>
                {
                    RollBack rolleback = new RollBack(versionList);
                    var r = rolleback.ShowDialog();
                    if (r == DialogResult.Cancel)
                    {
                        this.nlog_iis.Info($"rollback canceled!");
                        return;
                    }
                    else
                    {
                        PrintCommonLog(this.nlog_iis);
                        this.nlog_iis.Info("Start rollBack from version:" + rolleback.SelectRollBackVersion);
                        this.tab_iis.SelectedIndex = 0;
                        DoIIsRollback(serverList, rolleback.SelectRollBackVersion);
                    }
                });

            }).Start();

        }


        private void DoIIsRollback(List<Server> serverList, string dateTimeFolderName)
        {
            new Task(async () =>
            {
                Enable(false, true);
                try
                {

                    var loggerId = Guid.NewGuid().ToString("N");

                    this.nlog_iis.Info($"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        BuildEnd(this.tabPage_progress, server.Host);
                        UpdatePackageProgress(this.tabPage_progress, server.Host, 100);
                        UpdateUploadProgress(this.tabPage_progress, server.Host, 100);


                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_iis.Warn($"{server.Host} Rollback skip,Token is null or empty!");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "iis_rollback");
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_iis, HttpLogger);

                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/rollback",
                                (client) => { });
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Rollback Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");
                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.IIsFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", $"Host:{getHostDisplayName(server)} Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.LoggerName = "rich_iis_log";
                                        publisEvent22.Properties["ShowLink"] = server.IIsFireUrl;
                                        this.nlog_iis.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.IIsFireUrl, this.nlog_iis);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                            this.nlog_iis.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            allSuccess = false;
                                            failCount++;
                                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_progress, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_iis.Error(
                                        $"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            this.nlog_iis.Error($"Fail Rollback,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                    }

                    if (allSuccess)
                    {
                        this.nlog_iis.Info($"Rollback Version：" + dateTimeFolderName);
                    }

                    this.nlog_iis.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                }
                catch (Exception ex1)
                {
                    this.nlog_iis.Error(ex1);
                }
                finally
                {
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
                if (!string.IsNullOrEmpty(ProgressCurrentHost))
                    UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, showValue);
                this.nlog_iis.Info($"Upload {showValue} % complete...");
            }
        }

        private void Enable(bool flag, bool ignore = false)
        {
            this.BeginInvokeLambda(() =>
            {

                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList1)
                {
                    foreach (var box in progressBoxList1)
                    {
                        box.Value.Enable(flag);
                        break;
                    }
                }

                this.txt_iis_PhysicalPath.Enabled = flag;
                this.b_iis_rollback.Enabled = flag;
                this.b_iis_deploy.Enabled = flag;
                this.checkBox_Increment_iis.Enabled = flag;
                this.txt_iis_web_site_name.Enabled = flag;
                this.txt_iis_port.Enabled = flag;
                this.txt_pool_name.Enabled = flag;
                this.combo_iis_env.Enabled = flag;
                this.combo_iis_sdk_type.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                this.checkBox_select_deploy_iis.Enabled = flag;
                if (flag)
                {
                    this.rich_windowservice_log.Text = "";
                }
                else
                {
                    tabcontrol.Tag = "0";
                    if (ignore) return;
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

        private void BuildError(TabPage tabPage, string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.BuildError();
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.BuildError();
                            break;
                        }
                    }
                }
            });
        }

        private void PackageError(TabPage tabPage, string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.PackageError();
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.PackageError();
                            break;
                        }
                    }
                }
            });
        }

        private void UploadError(TabPage tabPage, string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.UploadError();
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.UploadError();
                            break;
                        }
                    }
                }
            });
        }

        private void BuildEnd(TabPage tabPage, string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
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



        private void UpdatePackageProgress(TabPage tabPage, string host, int value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
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

        private void UpdateUploadProgress(TabPage tabPage, string host, int value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
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

        private void UpdateDeployProgress(TabPage tabPage, string host, bool value)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
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

        private void checkBox_Increment_iis_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.IISEnableIncrement = checkBox_Increment_iis.Checked;
            //if (PluginConfig.IISEnableIncrement)
            //{
            //    checkBox_select_deploy_iis.Checked = false;
            //    PluginConfig.IISEnableSelectDeploy = false;
            //}
        }
        private void checkBox_selectDeplot_iis_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.IISEnableSelectDeploy = checkBox_select_deploy_iis.Checked;
            //if (PluginConfig.IISEnableSelectDeploy)
            //{
            //    checkBox_Increment_iis.Checked = false;
            //    PluginConfig.IISEnableIncrement = false;
            //}
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
                //生成进度
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_windows_service.Controls.Remove(box.Value);
                    }
                    this.tabPage_windows_service.Tag = null;
                }

                var newBoxList = new Dictionary<string, ProgressBox>();

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
                    var nickName = serverList[i].NickName;
                    ProgressBox newBox =
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.WINSERVICE)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    newBoxList.Add(serverHost, newBox);
                    this.tabPage_windows_service.Controls.Add(newBox);
                }

                this.progress_window_service_tip.SendToBack();
                this.tabPage_windows_service.Tag = newBoxList;
            }
            else
            {
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_windows_service.Controls.Remove(box.Value);
                    }
                }
            }
        }


        private void ClientOnUploadProgressChanged2(object sender, UploadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > ProgressPercentageForWindowsService && e.ProgressPercentage != 100)
            {
                ProgressPercentageForWindowsService = e.ProgressPercentage;
                var showValue = (e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage);
                if (!string.IsNullOrEmpty(ProgressCurrentHostForWindowsService))
                    UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService, showValue);
                this.nlog_windowservice.Info($"Upload {showValue} % complete...");
            }
        }



        private void EnableForWindowsService(bool flag, bool ignore = false)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList1)
                {
                    foreach (var box in progressBoxList1)
                    {
                        box.Value.Enable(flag);
                        break;
                    }
                }
                this.b_windows_service_rollback.Enabled = flag;
                this.txt_windows_service_PhysicalPath.Enabled = flag;
                this.checkBox_Increment_window_service.Enabled = flag;
                this.b_windowservice_deploy.Enabled = flag;
                this.combo_windowservice_env.Enabled = flag;
                this.combo_windowservice_sdk_type.Enabled = flag;
                this.txt_windowservice_name.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_web_iis.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                checkBox_select_deploy_service.Enabled = flag;
                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_docker_log.Text = "";
                }
                else
                {
                    tabcontrol.Tag = "2";
                    if (ignore) return;
                    if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
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



        private void b_windowservice_deploy_Click(object sender, EventArgs e)
        {
            if (_project.IsWebProejct)
            {
                MessageBox.Show("current project is not windows service project!");
                return;
            }

            //检查工程文件里面是否含有 WebProjectProperties字样
            if (ProjectHelper.IsWebProject(ProjectPath))
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
                if (!_project.IsNetcorePorject)
                {
                    MessageBox.Show("current project is not netcore project!");
                    return;
                }


                //检查一下 如果是netcore的话 不允许runtime部署
                if (!string.IsNullOrEmpty(PluginConfig.NetCorePublishMode) &&
                    PluginConfig.NetCorePublishMode.Contains("runtime"))
                {
                    MessageBox.Show("netcore project can not use [FDD(runtime)] mode!");
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

            var PhysicalPath = this.txt_windows_service_PhysicalPath.Text.Trim();

            var envName = this.combo_windowservice_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.WINSERVICE))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }


            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;
            var backUpIgnoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).WindowsBackUpIgnoreList;

#if DEBUG
            var execFilePath = this.ProjectName + ".exe";
#else
            var execFilePath = _project.OutPutName;
            if (string.IsNullOrEmpty(execFilePath))
            {
                //MessageBox.Show("get current project property:outputfilename error");
                execFilePath = this.ProjectName + ".exe";
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

            var confirmResult = ShowInputMsgBox("Confirm Deploy!!",
                "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList);

            //var confirmResult = MessageBox.Show(
            //    "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList,
            //    "Confirm Deploy!!",
            //    MessageBoxButtons.YesNo);
            if (!confirmResult.Item1)
            {
                return;
            }

            combo_windowservice_env_SelectedIndexChanged(null, null);

            this.rich_windowservice_log.Text = "";
            this.nlog_windowservice.Info($"windows Service exe name:{execFilePath.Replace(".dll", ".exe")}");
            if (DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore"))
            {
                if (string.IsNullOrEmpty(_project.NetCoreSDKVersion))
                {
                    _project.NetCoreSDKVersion = ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
                }
                if (!string.IsNullOrEmpty(_project.NetCoreSDKVersion))
                {
                    this.nlog_windowservice.Info($"DotNetSDK.Version:{_project.NetCoreSDKVersion}");
                }
            }

            new Task(async () =>
            {
                this.nlog_windowservice.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
                PrintCommonLog(this.nlog_windowservice);
                EnableForWindowsService(false); //第一台开始编译
                GitClient gitModel = null;
                try
                {
                    var isNetcore = false;
                    var publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_winservice", envName);
                    var path = publishPath + "\\";
                    if (DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore"))
                    {
                        isNetcore = true;
                        var runtime = "";
                        if (string.IsNullOrEmpty(PluginConfig.NetCorePublishMode))
                        {
                            runtime = " --runtime win-x64";
                        }
                        else
                        {
                            runtime = PluginConfig.GetNetCorePublishRuntimeArg();
                        }
                        //执行 publish
                        var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                    $"publish \"{ProjectPath}\" -c Release{runtime}", nlog_windowservice);

                        if (!isSuccess)
                        {
                            this.nlog_windowservice.Error("publish error,please check build log");
                            BuildError(this.tabPage_windows_service);
                            return;
                        }
                    }
                    else
                    {
                        //执行 publish
                        var isSuccess = CommandHelper.RunMsbuild(ProjectPath, path, nlog_windowservice);

                        if (!isSuccess)
                        {
                            this.nlog_windowservice.Error("publish error,please check build log");
                            BuildError(this.tabPage_windows_service);
                            return;
                        }


                    }


                    if (string.IsNullOrEmpty(publishPath) || !Directory.Exists(publishPath))
                    {
                        this.nlog_windowservice.Error("can not find publishPath");
                        BuildError(this.tabPage_windows_service);
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
                            BuildError(this.tabPage_windows_service);
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
                            if (!serviceNameByFile.Equals(serviceName))
                            {
                                this.nlog_windowservice.Warn($"windowsService name is {serviceNameByFile} in file: {serviceFile} ,but input name is {serviceName} ,will install service by [{serviceNameByFile}] ! ");
                            }

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
                            BuildError(this.tabPage_windows_service);
                            this.nlog_windowservice.Error($"exe file can not find in publish folder: {serviceFile}");
                            return;
                        }
                    }


                    BuildEnd(this.tabPage_windows_service); //第一台结束编译
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent.LoggerName = "rich_windowservice_log";
                    this.nlog_windowservice.Log(publisEvent);



                    //执行 打包
                    this.nlog_windowservice.Info("-----------------Start package-----------------");


                    //查看是否开启了增量
                    if (this.PluginConfig.WindowsServiceEnableIncrement)
                    {
                        this.nlog_windowservice.Info("Enable Increment Deploy:true");
                        gitModel = new GitClient(publishPath, this.nlog_windowservice);
                        if (!gitModel.InitSuccess)
                        {
                            this.nlog_windowservice.Error(
                                "package fail,can not init git,please cancel Increment Deploy");
                            PackageError(this.tabPage_windows_service);
                            return;
                        }
                    }
                    var gitChangeFileCount = 0;
                    byte[] zipBytes = null;
                    if (gitModel != null)
                    {
                        var fileList = gitModel.GetChanges();
                        if (fileList == null || fileList.Count < 1)
                        {
                            PackageError(this.tabPage_windows_service);
                            return;
                        }
                        gitChangeFileCount = fileList.Count;
                        this.nlog_windowservice.Info("【git】Increment package file count:" + gitChangeFileCount);

                        if (PluginConfig.WindowsServiceEnableSelectDeploy)
                        {
                            this.nlog_windowservice.Info("-----------------Select File Start-----------------");
                            this.BeginInvokeLambda(() =>
                            {
                                var slectFileForm = new SelectFile(fileList, publishPath);
                                slectFileForm.ShowDialog();
                                // ReSharper disable once AccessToDisposedClosure
                                DoWindowsServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList, serviceName, isProjectInstallService, execFilePath, PhysicalPath, backUpIgnoreList, gitModel,confirmResult.Item2);
                            });
                            return;
                        }
                        

                        try
                        {
                            this.nlog_windowservice.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal,
                                true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_windows_service, null, progressValue); //打印打包记录
                                });
                        }
                        catch (Exception ex)
                        {
                            this.nlog_windowservice.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_windows_service);
                            return;
                        }



                    }
                    else if (PluginConfig.WindowsServiceEnableSelectDeploy)
                    {
                        this.nlog_windowservice.Info("-----------------Select File Start-----------------");
                        this.BeginInvokeLambda(() =>
                        {
                            var slectFileForm = new SelectFile(publishPath);
                            slectFileForm.ShowDialog();
                            DoWindowsServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList,serviceName,isProjectInstallService,execFilePath,PhysicalPath,backUpIgnoreList,null,confirmResult.Item2);
                        });
                        return;
                    }

                    try
                    {
                        if (zipBytes == null)
                        {
                            this.nlog_windowservice.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_windows_service, null, progressValue); //打印打包记录
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.nlog_windowservice.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_windows_service);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_windowservice.Error("package fail");
                        PackageError(this.tabPage_windows_service);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_windowservice.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_windowservice.Info("-----------------Deploy Start-----------------");
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    foreach (var server in serverList)
                    {
                        if (index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_windows_service, server.Host);
                            UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        }

                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        ProgressPercentageForWindowsService = 0;
                        ProgressCurrentHostForWindowsService = server.Host;
                        this.nlog_windowservice.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "windowservice");
                        httpRequestClient.SetFieldValue("isIncrement",
                            this.PluginConfig.WindowsServiceEnableIncrement ? "true" : "");
                        httpRequestClient.SetFieldValue("serviceName", serviceName);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.WindowsServiveConfig.SdkType);
                        httpRequestClient.SetFieldValue("isProjectInstallService",
                            isProjectInstallService ? "yes" : "no");
                        httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                        httpRequestClient.SetFieldValue("remark", confirmResult.Item2);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        }; ;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_windowservice, HttpLogger);
                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged2; });
                            if (ProgressPercentageForWindowsService == 0)
                                UploadError(this.tabPage_windows_service, server.Host);
                            if (ProgressPercentageForWindowsService > 0 && ProgressPercentageForWindowsService < 100)
                                UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService,
                                    100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.WindowsServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.WindowsServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_windowservice_log";
                                        this.nlog_windowservice.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.WindowsServiceFireUrl, this.nlog_windowservice);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                            this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_windowservice.Error(
                                        $"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            allSuccess = false;
                            this.nlog_windowservice.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                        index++;
                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_windowservice.Info("Deploy Version：" + dateTimeFolderName);
                        if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                    }

                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent2.LoggerName = "rich_windowservice_log";
                    this.nlog_windowservice.Log(publisEvent2);
                    this.nlog_windowservice.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                }
                catch (Exception ex1)
                {
                    this.nlog_windowservice.Error(ex1);
                }
                finally
                {
                    if (!PluginConfig.WindowsServiceEnableSelectDeploy)
                    {
                        ProgressPercentageForWindowsService = 0;
                        ProgressCurrentHostForWindowsService = null;
                        EnableForWindowsService(true);
                        gitModel?.Dispose();
                    }
                   
                }



            }).Start();
        }


        private void DoWindowsServiceSelectDeploy(List<string> fileList,string publishPath,List<Server> serverList,string serviceName,bool isProjectInstallService,string execFilePath,string PhysicalPath,List<string> backUpIgnoreList, GitClient gitModel,string remark)
        {
            new Task(async () =>
            {
                try
                {
                    byte[] zipBytes = null;
                    if (fileList == null || !fileList.Any())
                    {
                        PackageError(this.tabPage_windows_service);
                        this.nlog_windowservice.Error("Please Select Files");
                        return;
                    }
                    this.nlog_windowservice.Info("Select Files count:" + fileList.Count);
                    this.nlog_windowservice.Debug("ignore package ignoreList");
                    List<string> ignoreList = new List<string>();
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, true,
                            ignoreList,
                            (progressValue) =>
                            {
                                UpdatePackageProgress(this.tabPage_windows_service, null, progressValue); //打印打包记录
                            },true);
                    }
                    catch (Exception ex)
                    {
                        this.nlog_windowservice.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_windows_service);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_windowservice.Error("package fail");
                        PackageError(this.tabPage_windows_service);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_windowservice.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_windowservice.Info("-----------------Deploy Start-----------------");
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    foreach (var server in serverList)
                    {
                        if (index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_windows_service, server.Host);
                            UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        }

                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        ProgressPercentageForWindowsService = 0;
                        ProgressCurrentHostForWindowsService = server.Host;
                        this.nlog_windowservice.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "windowservice");
                        httpRequestClient.SetFieldValue("isIncrement", "true");
                        httpRequestClient.SetFieldValue("serviceName", serviceName);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.WindowsServiveConfig.SdkType);
                        httpRequestClient.SetFieldValue("isProjectInstallService",
                            isProjectInstallService ? "yes" : "no");
                        httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("remark", remark);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        }; ;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_windowservice, HttpLogger);
                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) => { client.UploadProgressChanged += ClientOnUploadProgressChanged2; });
                            if (ProgressPercentageForWindowsService == 0)
                                UploadError(this.tabPage_windows_service, server.Host);
                            if (ProgressPercentageForWindowsService > 0 && ProgressPercentageForWindowsService < 100)
                                UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService,
                                    100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.WindowsServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.WindowsServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_windowservice_log";
                                        this.nlog_windowservice.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.WindowsServiceFireUrl, this.nlog_windowservice);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                            this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_windowservice.Error(
                                        $"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            allSuccess = false;
                            this.nlog_windowservice.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                        index++;
                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_windowservice.Info("Deploy Version：" + dateTimeFolderName);
                        if (gitModel != null) gitModel.SubmitSelectedChanges(fileList,publishPath);
                    }

                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent2.LoggerName = "rich_windowservice_log";
                    this.nlog_windowservice.Log(publisEvent2);
                    this.nlog_windowservice.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                }
                catch (Exception ex1)
                {
                    this.nlog_windowservice.Error(ex1);
                }
                finally
                {
                    ProgressPercentageForWindowsService = 0;
                    ProgressCurrentHostForWindowsService = null;
                    EnableForWindowsService(true);
                    gitModel?.Dispose();
                }
            }).Start();
        }

        private void b_windows_service_rollback_Click(object sender, EventArgs e)
        {
            if (_project.IsWebProejct)
            {
                MessageBox.Show("current project is not windows service project!");
                return;
            }


            //检查工程文件里面是否含有 WebProjectProperties字样
            if (ProjectHelper.IsWebProject(ProjectPath))
            {
                MessageBox.Show("current project is not windows service project!");
                return;
            }


            var serviceName = this.txt_windowservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBox.Show("please input serviceName");
                return;
            }

            DeployConfig.WindowsServiveConfig.ServiceName = serviceName;



            var envName = this.combo_windowservice_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.WINSERVICE))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }

#if DEBUG
            var execFilePath = this.ProjectName + ".exe";
#else
            var execFilePath = _project.OutPutName;
            if (string.IsNullOrEmpty(execFilePath))
            {
                execFilePath = this.ProjectName + ".exe";// MessageBox.Show("get current project property:outputfilename error");
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

            var confirmResult = MessageBox.Show(
                "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_windowservice_env_SelectedIndexChanged(null, null);

            this.rich_windowservice_log.Text = "";
            this.nlog_windowservice.Info($"windows Service exe name:{execFilePath.Replace(".dll", ".exe")}");
            this.tabControl_window_service.SelectedIndex = 1;
            new Task(async () =>
            {
                var versionList = new List<string>();
                try
                {
                    EnableForWindowsService(false, true);
                    var firstServer = serverList.First();

                    if (string.IsNullOrEmpty(firstServer.Host))
                    {
                        this.nlog_windowservice.Error($"Server Host is Empty!");
                        return;
                    }

                    if (string.IsNullOrEmpty(firstServer.Token))
                    {
                        this.nlog_windowservice.Error($"Server Token is Empty!");
                        return;
                    }

                    this.nlog_windowservice.Info(
                        "Start get rollBack version list from first Server:" + firstServer.Host);
                    var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                        $"http://{firstServer.Host}/version", new
                        {
                            Token = firstServer.Token,
                            Type = "winservice",
                            Name = DeployConfig.WindowsServiveConfig.ServiceName,
                            WithArgs = true
                        }, nlog_windowservice);

                    if (getVersionResult == null)
                    {
                        this.nlog_windowservice.Error($"get rollBack version list fail");
                        return;
                    }

                    if (!string.IsNullOrEmpty(getVersionResult.Msg))
                    {
                        this.nlog_windowservice.Error($"get rollBack version list fail：" + getVersionResult.Msg);
                        return;
                    }

                    versionList = getVersionResult.Data;

                }
                catch (Exception ex1)
                {
                    this.nlog_windowservice.Error(ex1);
                    return;
                }
                finally
                {
                    EnableForWindowsService(true);
                }

                if (versionList == null || versionList.Count < 1)
                {
                    this.nlog_windowservice.Error($"get rollBack version list count = 0");
                    return;
                }

                this.BeginInvokeLambda(() =>
                {
                    RollBack rolleback = new RollBack(versionList);
                    var r = rolleback.ShowDialog();
                    if (r == DialogResult.Cancel)
                    {
                        this.nlog_windowservice.Info($"rollback canceled!");
                        return;
                    }
                    else
                    {
                        PrintCommonLog(this.nlog_windowservice);
                        this.nlog_windowservice.Info("Start rollBack from version:" + rolleback.SelectRollBackVersion);
                        this.tabControl_window_service.SelectedIndex = 0;
                        DoWindowsServiceRollback(serverList, rolleback.SelectRollBackVersion);
                    }
                });
            }).Start();
        }

        private void DoWindowsServiceRollback(List<Server> serverList, string dateTimeFolderName)
        {
            new Task(async () =>
            {
                EnableForWindowsService(false, true);
                try
                {

                    var loggerId = Guid.NewGuid().ToString("N");

                    this.nlog_windowservice.Info(
                        $"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        BuildEnd(this.tabPage_windows_service, server.Host);
                        UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        UpdateUploadProgress(this.tabPage_windows_service, server.Host, 100);


                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Warn($"{server.Host} Rollback skip,Token is null or empty!");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "windowservice_rollback");
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("serviceName", DeployConfig.WindowsServiveConfig.ServiceName);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_windowservice, HttpLogger);

                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/rollback",
                                (client) => { });
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Rollback Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");
                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.WindowsServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.WindowsServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_windowservice_log";
                                        this.nlog_windowservice.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.WindowsServiceFireUrl, this.nlog_windowservice);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                            this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_windowservice.Error(
                                        $"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            this.nlog_windowservice.Error(
                                $"Fail Rollback,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }

                        if (allSuccess)
                        {
                            this.nlog_windowservice.Info($"Rollback Version：" + dateTimeFolderName);
                        }

                        this.nlog_windowservice.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    }

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
            string pase = tabcontrol.Tag as string;
            if (string.IsNullOrEmpty(pase))
            {
                return;
            }

            int.TryParse(pase, out var excutePageIndex);
            if (excutePageIndex < 0)
            {
                return;
            }

            if (tabcontrol.SelectedIndex == 0)
            {
                if (excutePageIndex != tabcontrol.SelectedIndex)
                {
                    this.rich_iis_log.Text = "";
                }
            }
            else if (tabcontrol.SelectedIndex == 1)
            {
                if (excutePageIndex != tabcontrol.SelectedIndex)
                {
                    this.rich_docker_log.Text = "";
                }
            }
            else if (tabcontrol.SelectedIndex == 2)
            {
                if (excutePageIndex != tabcontrol.SelectedIndex)
                {
                    this.rich_windowservice_log.Text = "";
                }
            }
        }

        private void DeployConfigOnEnvChangeEvent(Env changeEnv, bool isServerChange)
        {

            var item1 = this.combo_iis_env.SelectedItem as string;
            var item2 = this.combo_windowservice_env.SelectedItem as string;
            var item3 = this.combo_docker_env.SelectedItem as string;
            if (isServerChange)
            {

                if (!string.IsNullOrEmpty(item1) && item1.Equals(changeEnv.Name))
                {
                    combo_iis_env_SelectedIndexChanged(null, null);
                }

                if (!string.IsNullOrEmpty(item2) && item2.Equals(changeEnv.Name))
                {
                    combo_windowservice_env_SelectedIndexChanged(null, null);
                }

                if (!string.IsNullOrEmpty(item3) && item3.Equals(changeEnv.Name))
                {
                    combo_docker_env_SelectedIndexChanged(null, null);
                }

                return;
            }

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

                //重新选中原来的
                if (!string.IsNullOrEmpty(item1))
                {
                    if (this.combo_iis_env.Items.Cast<string>().Contains(item1))
                    {
                        this.combo_iis_env.SelectedItem = item1;
                    }
                    else
                    {
                        combo_iis_env_SelectedIndexChanged(null, null);
                    }
                }

                if (!string.IsNullOrEmpty(item2))
                {
                    if (this.combo_windowservice_env.Items.Cast<string>().Contains(item2))
                    {
                        this.combo_windowservice_env.SelectedItem = item2;
                    }
                    else
                    {
                        combo_windowservice_env_SelectedIndexChanged(null, null);
                    }

                }

                if (!string.IsNullOrEmpty(item3))
                {
                    if (this.combo_docker_env.Items.Cast<string>().Contains(item3))
                    {
                        this.combo_docker_env.SelectedItem = item3;
                    }
                    else
                    {
                        combo_docker_env_SelectedIndexChanged(null, null);
                    }

                }


            }

        }

        private void ReadPorjectConfig(string projectPath)
        {
            if (File.Exists(projectPath))
            {
                var file = new FileInfo(projectPath);
                ProjectFolderPath = file.DirectoryName;


                ProjectConfigPath = Path.Combine(ProjectFolderPath, "AntDeploy.json");
                if (File.Exists(ProjectConfigPath))
                {
                    var config = File.ReadAllText(ProjectConfigPath, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(config))
                    {
                        DeployConfig = JsonConvert.DeserializeObject<DeployConfig>(config);
                        if (DeployConfig.Env == null)
                        {
                            DeployConfig.Env = new List<Env>();
                        }
                        else
                        {
                            foreach (var env in DeployConfig.Env)
                            {
                                if (env.IgnoreList == null) env.IgnoreList = new List<string>();
                                if (env.WindowsBackUpIgnoreList == null) env.WindowsBackUpIgnoreList = new List<string>();
                            }
                        }
                        if (DeployConfig.IIsConfig == null) DeployConfig.IIsConfig = new IIsConfig();
                    }
                }
            }
        }

        private void ReadPluginConfig(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                PluginConfig = new PluginConfig();
                return;
            }

            if (File.Exists(projectPath))
            {
                var config = File.ReadAllText(projectPath, Encoding.UTF8);
                if (!string.IsNullOrEmpty(config))
                {
                    try
                    {
                        PluginConfig = JsonConvert.DeserializeObject<PluginConfig>(config);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    if (PluginConfig == null) PluginConfig = new PluginConfig();
                }
                else
                {
                    if (PluginConfig == null) PluginConfig = new PluginConfig();
                }
            }
            else
            {
                PluginConfig = new PluginConfig();
            }
        }
        private void ReadConfig(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                GlobalConfig = new GlobalConfig() { ProjectPathList = new List<string>() };
                return;
            }

            if (File.Exists(projectPath))
            {
                var config = File.ReadAllText(projectPath, Encoding.UTF8);
                if (!string.IsNullOrEmpty(config))
                {
                    try
                    {
                        GlobalConfig = JsonConvert.DeserializeObject<GlobalConfig>(config);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    if (GlobalConfig == null) GlobalConfig = new GlobalConfig() { ProjectPathList = new List<string>() };
                }
                else
                {
                    if (GlobalConfig == null) new GlobalConfig() { ProjectPathList = new List<string>() };
                }
            }
            else
            {
                GlobalConfig = new GlobalConfig() { ProjectPathList = new List<string>() };
            }

        }

        private void BeginInvokeLambda(Action action)
        {
            BeginInvoke(action, null);
        }

        private void label_check_update_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo =
                new ProcessStartInfo("https://marketplace.visualstudio.com/items?itemName=nainaigu.AntDeploy");
            Process.Start(sInfo);
        }

        private void label_iis_demo_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/2");
            Process.Start(sInfo);
        }

        private void label_docker_demo_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/6");
            Process.Start(sInfo);
        }

        private void label_windows_serivce_demo_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/4");
            Process.Start(sInfo);
        }
        private void label_how_to_set_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/8");
            Process.Start(sInfo);
        }

        private void label32_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/10");
            Process.Start(sInfo);
        }

        private void checkBox_Increment_window_service_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.WindowsServiceEnableIncrement = checkBox_Increment_window_service.Checked;
            //if (PluginConfig.WindowsServiceEnableIncrement)
            //{
            //    checkBox_select_deploy_service.Checked = false;
            //    PluginConfig.WindowsServiceEnableSelectDeploy = false;
            //}
        }
        private void checkBox_SelectDeploy_window_service_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.WindowsServiceEnableSelectDeploy = checkBox_select_deploy_service.Checked;
            //if (PluginConfig.WindowsServiceEnableSelectDeploy)
            //{
            //    checkBox_Increment_window_service.Checked = false;
            //    PluginConfig.WindowsServiceEnableIncrement = false;
            //}
        }
        #endregion


        #region Docker

        private void b_docker_deploy_Click(object sender, EventArgs e)
        {
            var port = this.txt_docker_port.Text.Trim();
            if (!string.IsNullOrEmpty(port))
            {
                int.TryParse(port, out var dockerPort);
                if (dockerPort <= 0)
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

            var volume = this.txt_docker_volume.Text.Trim();
            if (volume.Length > 0)
            {
                volume = volume.Replace('；', ';');
                var arr = volume.Split(';');
                foreach (var item in arr)
                {
                    if (!item.Contains(':'))
                    {
                        MessageBox.Show("volume is not correct!");
                        return;
                    }
                }
                DeployConfig.DockerConfig.Volume = volume;
            }

            var envName = this.combo_docker_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.DOCKER))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }

            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;

            var removeDays = this.t_docker_delete_days.Text.Trim();
            if (!string.IsNullOrEmpty(removeDays))
            {
                int.TryParse(removeDays, out var _removeDays);
                if (_removeDays <= 0)
                {
                    MessageBox.Show("please input right days value");
                    return;
                }
                else
                {
                    DeployConfig.DockerConfig.RemoveDaysFromPublished = removeDays;
                }
            }
            else
            {
                DeployConfig.DockerConfig.RemoveDaysFromPublished = "";
            }

#if DEBUG
            var ENTRYPOINT = this.ProjectName + ".dll";
#else
            //必须是netcore应用
            var isNetcoreProject = _project.IsNetcorePorject;
            if (!isNetcoreProject)
            {
                MessageBox.Show("current project is not netcore");
                return;
            }

            var ENTRYPOINT = _project.OutPutName;
            if (string.IsNullOrEmpty(ENTRYPOINT))
            {
                //MessageBox.Show("get current project property:outputfilename error");
                ENTRYPOINT = this.ProjectName + ".dll";
            }
#endif
            var SDKVersion = !string.IsNullOrEmpty(_project.NetCoreSDKVersion) ? _project.NetCoreSDKVersion : ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            if (string.IsNullOrEmpty(SDKVersion))
            {
                MessageBox.Show("get current project skd version error");
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no linux server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = ShowInputMsgBox("Confirm Deploy!!",
                "Are you sure to deploy to Server: " + Environment.NewLine + serverHostList);

            //var confirmResult = MessageBox.Show(
            //    "Are you sure to deploy to Linux Server: " + Environment.NewLine + serverHostList,
            //    "Confirm Deploy!!",
            //    MessageBoxButtons.YesNo);
            if (!confirmResult.Item1)
            {
                return;
            }

            combo_docker_env_SelectedIndexChanged(null, null);

            this.rich_docker_log.Text = "";
            this.nlog_docker.Info($"The Porject ENTRYPOINT name:{ENTRYPOINT},DotNetSDK.Version:{_project.NetCoreSDKVersion}");
            var clientDateTimeFolderName = DateTime.Now.ToString("yyyyMMddHHmmss");
            new Task(() =>
           {
               this.nlog_docker.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
               PrintCommonLog(this.nlog_docker);
               EnableForDocker(false);

               try
               {
                   var publishPath = Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_docker", envName);
                   var path = publishPath + "\\";
                   //执行 publish
                   var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
              $"publish \"{ProjectPath}\" -c Release{PluginConfig.GetNetCorePublishRuntimeArg()}", nlog_docker);

                   if (!isSuccess)
                   {
                       this.nlog_docker.Error("publish error,please check build log");
                       BuildError(this.tabPage_docker);
                       return;
                   }

                   var serviceFile = Path.Combine(publishPath, ENTRYPOINT);
                   if (!File.Exists(serviceFile))
                   {
                       this.nlog_docker.Error($"ENTRYPOINT file can not find in publish folder: {serviceFile}");
                       BuildError(this.tabPage_docker);
                       return;
                   }

                   BuildEnd(this.tabPage_docker); //第一台结束编译

                   LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                   publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                   publisEvent.LoggerName = "rich_docker_log";
                   this.nlog_docker.Log(publisEvent);



                   //执行 打包
                   this.nlog_docker.Info("-----------------Start package-----------------");
                   MemoryStream zipBytes;
                   try
                   {
                       this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                       zipBytes = ZipHelper.DoCreateTarFromDirectory(publishPath,
                           ignoreList,
                           (progressValue) =>
                           {
                               UpdatePackageProgress(this.tabPage_docker, null, progressValue); //打印打包记录
                           });
                   }
                   catch (Exception ex)
                   {
                       this.nlog_docker.Error("package fail:" + ex.Message);
                       PackageError(this.tabPage_docker);
                       return;
                   }

                   if (zipBytes == null || zipBytes.Length < 1)
                   {
                       this.nlog_docker.Error("package fail");
                       PackageError(this.tabPage_docker);
                       return;
                   }

                   this.nlog_docker.Info($"package success,package size:{(zipBytes.Length / 1024 / 1024)}M");
                   //执行 上传
                   this.nlog_docker.Info("-----------------Deploy Start-----------------");
                   var index = 0;
                   var allSuccess = true;
                   var failCount = 0;
                   foreach (var server in serverList)
                   {
                       if (index != 0) //因为编译和打包只会占用第一台服务器的时间
                       {
                           BuildEnd(this.tabPage_docker, server.Host);
                           UpdatePackageProgress(this.tabPage_docker, server.Host, 100);
                       }

                       #region 参数Check

                       if (string.IsNullOrEmpty(server.Host))
                       {
                           this.nlog_docker.Error("Server Host is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.UserName))
                       {
                           this.nlog_docker.Error("Server UserName is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.Pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       var pwd = CodingHelper.AESDecrypt(server.Pwd);
                       if (string.IsNullOrEmpty(pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       #endregion

                       var hasError = false;

                       zipBytes.Seek(0, SeekOrigin.Begin);
                       using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, (str, logLevel) =>
                       {
                           if (logLevel == NLog.LogLevel.Error)
                           {
                               hasError = true;
                               allSuccess = false;
                               this.nlog_docker.Error("【Server】" + str);
                           }
                           else
                           {
                               this.nlog_docker.Info("【Server】" + str);
                           }
                       }, (uploadValue) => { UpdateUploadProgress(this.tabPage_docker, server.Host, uploadValue); })
                       {
                           NetCoreENTRYPOINT = ENTRYPOINT,
                           NetCoreVersion = SDKVersion,
                           NetCorePort = DeployConfig.DockerConfig.Prot,
                           NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                           ClientDateTimeFolderName = clientDateTimeFolderName,
                           RemoveDaysFromPublished = DeployConfig.DockerConfig.RemoveDaysFromPublished,
                           Volume = DeployConfig.DockerConfig.Volume,
                           Remark = confirmResult.Item2
                       })
                       {
                           var connectResult = sshClient.Connect();
                           if (!connectResult)
                           {
                               this.nlog_docker.Error($"Deploy Host:{getHostDisplayName(server)} Fail: connect fail");
                               UploadError(this.tabPage_docker);
                               allSuccess = false;
                               failCount++;
                               continue;
                           }

                           try
                           {
                               sshClient.PublishZip(zipBytes, "antdeploy", "publish.tar");
                               UpdateUploadProgress(this.tabPage_docker, server.Host, 100);

                               if (hasError)
                               {
                                   allSuccess = false;
                                   failCount++;
                                   sshClient.DeletePublishFolder("antdeploy");
                                   UpdateDeployProgress(this.tabPage_docker, server.Host, !hasError);
                               }
                               else
                               {
                                   //fire the website
                                   if (!string.IsNullOrEmpty(server.DockerFireUrl))
                                   {
                                       LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                       publisEvent22.Properties["ShowLink"] = server.DockerFireUrl;
                                       publisEvent22.LoggerName = "rich_docker_log";
                                       this.nlog_docker.Log(publisEvent22);

                                       var fireRt = WebUtil.IsHttpGetOk(server.DockerFireUrl, this.nlog_docker);
                                       if (fireRt)
                                       {
                                           UpdateDeployProgress(this.tabPage_docker, server.Host, true);
                                           this.nlog_docker.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                       }
                                       else
                                       {
                                           UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                                           allSuccess = false;
                                           failCount++;
                                       }
                                   }
                                   else
                                   {
                                       UpdateDeployProgress(this.tabPage_docker, server.Host, true);
                                   }

                               }

                               this.nlog_docker.Info($"publish Host: {getHostDisplayName(server)} End");
                           }
                           catch (Exception ex)
                           {
                               allSuccess = false;
                               failCount++;
                               this.nlog_docker.Error($"Deploy Host:{getHostDisplayName(server)} Fail:" + ex.Message);
                               UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                           }
                       }

                       index++;
                   }

                   zipBytes.Dispose();
                   if (allSuccess)
                   {
                       this.nlog_docker.Info("Deploy Version：" + clientDateTimeFolderName);
                   }

                   LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                   publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                   publisEvent2.LoggerName = "rich_docker_log";
                   this.nlog_docker.Log(publisEvent2);
                   this.nlog_docker.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
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

        private void btn_docker_rollback_Click(object sender, EventArgs e)
        {
            var envName = this.combo_docker_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBox.Show("please select env");
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.DOCKER))
            {
                MessageBox.Show("fire url is not correct!");
                return;
            }

#if DEBUG
            var ENTRYPOINT = this.ProjectName + ".dll";
#else
            //必须是netcore应用
            var isNetcoreProject = _project.IsNetcorePorject;
            if (!isNetcoreProject)
            {
                MessageBox.Show("current project is not netcore");
                return;
            }

            var ENTRYPOINT = _project.OutPutName;
            if (string.IsNullOrEmpty(ENTRYPOINT))
            {
                ENTRYPOINT = this.ProjectName + ".dll";//MessageBox.Show("get current project property:outputfilename error");
            }
#endif
            var SDKVersion = !string.IsNullOrEmpty(_project.NetCoreSDKVersion) ? _project.NetCoreSDKVersion : ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            if (string.IsNullOrEmpty(SDKVersion))
            {
                MessageBox.Show("get current project skd version error");
                return;
            }

            combo_docker_env_SelectedIndexChanged(null, null);


            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("selected env have no linux server set yet!");
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBox.Show(
                "Are you sure to rollBack to Linux Server: " + Environment.NewLine + serverHostList,
                "Confirm Deploy!!",
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            this.rich_docker_log.Text = "";
            this.nlog_docker.Info($"The Porject ENTRYPOINT name:{ENTRYPOINT}");
            this.tabControl_docker.SelectedIndex = 1;
            new Task(() =>
           {


               var versionList = new List<Tuple<string,string>>();

               try
               {

                   EnableForDocker(false, true);

                   var firstServer = serverList.First();

                   #region 参数Check

                   if (string.IsNullOrEmpty(firstServer.Host))
                   {
                       this.nlog_docker.Error("Server Host is Empty");
                       return;
                   }

                   if (string.IsNullOrEmpty(firstServer.UserName))
                   {
                       this.nlog_docker.Error("Server UserName is Empty");
                       return;
                   }

                   if (string.IsNullOrEmpty(firstServer.Pwd))
                   {
                       this.nlog_docker.Error("Server Pwd is Empty");
                       return;
                   }

                   var pwd1 = CodingHelper.AESDecrypt(firstServer.Pwd);
                   if (string.IsNullOrEmpty(pwd1))
                   {
                       this.nlog_docker.Error("Server Pwd is Empty");
                       return;
                   }

                   #endregion

                   this.nlog_docker.Info("Start get rollBack version list from first Server:" + firstServer.Host);

                   using (SSHClient sshClient = new SSHClient(firstServer.Host, firstServer.UserName, pwd1,
                       (str, logLevel) =>
                       {
                           if (logLevel == NLog.LogLevel.Error)
                           {
                               this.nlog_docker.Error("【Server】" + str);
                           }
                           else
                           {
                               this.nlog_docker.Info("【Server】" + str);
                           }
                       }, (uploadValue) => { })
                   {
                       NetCoreENTRYPOINT = ENTRYPOINT,
                       NetCoreVersion = SDKVersion,
                       NetCorePort = DeployConfig.DockerConfig.Prot,
                       NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                   })
                   {
                       var connectResult = sshClient.Connect();
                       if (!connectResult)
                       {
                           this.nlog_docker.Error($"connect rollBack Host:{getHostDisplayName(firstServer)} Fail");
                           return;
                       }

                       versionList = sshClient.GetDeployHistory("antdeploy", 11);
                   }
               }
               catch (Exception ex1)
               {
                   this.nlog_docker.Error(ex1);
                   return;
               }
               finally
               {
                   EnableForDocker(true);
               }

               if (versionList.Count <= 1)
               {
                   this.nlog_docker.Error($"get rollBack version list count = 0");
                   return;
               }

               this.BeginInvokeLambda(() =>
               {
                   RollBack rolleback = new RollBack(versionList);
                   var r = rolleback.ShowDialog();
                   if (r == DialogResult.Cancel)
                   {
                       this.nlog_docker.Info($"rollback canceled!");
                       return;
                   }
                   else
                   {
                       PrintCommonLog(this.nlog_docker);
                       this.nlog_docker.Info("Start rollBack from version:" + rolleback.SelectRollBackVersion);
                       this.tabControl_docker.SelectedIndex = 0;
                       DoRollBack(serverList, ENTRYPOINT, SDKVersion, rolleback.SelectRollBackVersion);
                   }
               });


           }).Start();




        }


        private void DoRollBack(List<LinuxServer> serverList, string ENTRYPOINT, string SDKVersion,
            string rollbackVersion)
        {


            new Task(() =>
           {

               this.nlog_docker.Info($"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");
               try
               {
                   EnableForDocker(false, true);
                   var allSuccess = true;
                   var failCount = 0;
                   foreach (var server in serverList)
                   {
                       this.nlog_docker.Info("ignore build...");
                       BuildEnd(this.tabPage_docker, server.Host);
                       this.nlog_docker.Info("ignore package...");
                       UpdatePackageProgress(this.tabPage_docker, server.Host, 100);

                       #region 参数Check

                       if (string.IsNullOrEmpty(server.Host))
                       {
                           this.nlog_docker.Error("Server Host is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.UserName))
                       {
                           this.nlog_docker.Error("Server UserName is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.Pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       var pwd = CodingHelper.AESDecrypt(server.Pwd);
                       if (string.IsNullOrEmpty(pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker);
                           allSuccess = false;
                           failCount++;
                           continue;
                       }

                       this.nlog_docker.Info("ignore upload...");
                       UpdateUploadProgress(this.tabPage_docker, server.Host, 100);

                       #endregion

                       var hasError = false;
                       using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, (str, logLevel) =>
                       {
                           if (logLevel == NLog.LogLevel.Error)
                           {
                               hasError = true;
                               allSuccess = false;
                               this.nlog_docker.Error("【Server】" + str);
                           }
                           else
                           {
                               this.nlog_docker.Info("【Server】" + str);
                           }
                       }, (uploadValue) => { UpdateUploadProgress(this.tabPage_docker, server.Host, uploadValue); })
                       {
                           NetCoreENTRYPOINT = ENTRYPOINT,
                           NetCoreVersion = SDKVersion,
                           NetCorePort = DeployConfig.DockerConfig.Prot,
                           NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                       })
                       {
                           var connectResult = sshClient.Connect();
                           if (!connectResult)
                           {
                               this.nlog_docker.Error($"RollBack Host:{getHostDisplayName(server)} Fail: connect fail");
                               UploadError(this.tabPage_docker);
                               allSuccess = false;
                               failCount++;
                               continue;
                           }

                           try
                           {
                               sshClient.RollBack(rollbackVersion);
                               if (hasError)
                               {
                                   allSuccess = false;
                                   failCount++;
                                   UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                               }
                               else
                               {
                                   //fire the website
                                   if (!string.IsNullOrEmpty(server.DockerFireUrl))
                                   {
                                       LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                       publisEvent22.Properties["ShowLink"] = server.DockerFireUrl;
                                       publisEvent22.LoggerName = "rich_docker_log";
                                       this.nlog_docker.Log(publisEvent22);

                                       var fireRt = WebUtil.IsHttpGetOk(server.DockerFireUrl, this.nlog_docker);
                                       if (fireRt)
                                       {
                                           UpdateDeployProgress(this.tabPage_docker, server.Host, true);
                                           this.nlog_docker.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                       }
                                       else
                                       {
                                           UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                                           allSuccess = false;
                                           failCount++;
                                       }
                                   }
                                   else
                                   {
                                       UpdateDeployProgress(this.tabPage_docker, server.Host, true);
                                   }
                               }


                               this.nlog_docker.Info($"RollBack Host: {getHostDisplayName(server)} End");
                           }
                           catch (Exception ex)
                           {
                               allSuccess = false;
                               failCount++;
                               this.nlog_docker.Error($"RollBack Host:{getHostDisplayName(server)} Fail:" + ex.Message);
                               UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                           }
                       }

                   }

                   if (allSuccess)
                   {
                       this.nlog_docker.Info("RollBack Version：" + rollbackVersion);
                   }

                   this.nlog_docker.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------"); ;
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

        private void EnableForDocker(bool flag, bool ignore = false)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList1)
                {
                    foreach (var box in progressBoxList1)
                    {
                        box.Value.Enable(flag);
                        break;
                    }
                }
                this.t_docker_delete_days.Enabled = flag;
                this.txt_docker_volume.Enabled = flag;
                this.b_docker_rollback.Enabled = flag;
                this.b_docker_deploy.Enabled = flag;
                this.combo_docker_env.Enabled = flag;
                this.txt_docker_port.Enabled = flag;
                this.txt_docker_envname.Enabled = flag;

                this.page_set.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.page_web_iis.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_windowservice_log.Text = "";
                }
                else
                {

                    tabcontrol.Tag = "1";
                    if (ignore) return;
                    if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
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

                //生成进度
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_docker.Controls.Remove(box.Value);
                    }
                    this.tabPage_docker.Tag = null;
                }

                var newBoxList = new Dictionary<string, ProgressBox>();

                var serverList = DeployConfig.Env.Where(r => r.Name.Equals(selectName)).Select(r => r.LinuxServerList)
                    .FirstOrDefault();

                if (serverList == null || !serverList.Any())
                {
                    return;
                }

                var serverHostList = serverList.Select(r => r.Host).ToList();

                for (int i = 0; i < serverHostList.Count; i++)
                {
                    var serverHost = serverHostList[i];
                    var nickName = serverList[i].NickName;
                    ProgressBox newBox =
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.DOCKER)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    newBoxList.Add(serverHost, newBox);
                    this.tabPage_docker.Controls.Add(newBox);
                }

                this.progress_docker_tip.SendToBack();
                this.tabPage_docker.Tag = newBoxList;
            }
            else
            {
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_docker.Controls.Remove(box.Value);
                    }
                }
            }
        }

        private void PrintCommonLog(Logger log)
        {
            var vsVersion = _project.VsVersion;
            if (!string.IsNullOrEmpty(vsVersion))
            {
                log.Info("Visual Studio Version : " + vsVersion);
            }


        }









        #endregion

        private void Deploy_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            About about = new About();
            about.ShowDialog();
        }

        private void b_copy_pack_ignore_Click(object sender, EventArgs e)
        {
            var current = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(current)) return;


            var envList = this.DeployConfig.Env.Where(r => !r.Name.Equals(current) && r.IgnoreList.Any()).Select(r => r.Name).ToList();
            if (envList.Count < 1)
            {
                MessageBox.Show("no env have ignore value!");
                return;
            }
            RollBack rollBack = new RollBack(envList,true) { Text = "Select Env Name" };
            rollBack.SetButtonName("Copy");
            var r1 = rollBack.ShowDialog();
            if (r1 == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                var target = this.DeployConfig.Env.FirstOrDefault(r => r.Name.Equals(rollBack.SelectRollBackVersion));
                if (target != null)
                {
                    var existItem = list_env_ignore.Items.Cast<string>().ToList();
                    foreach (var item in target.IgnoreList)
                    {
                        if (!existItem.Contains(item))
                        {
                            this.DeployConfig.Env[combo_env_list.SelectedIndex].IgnoreList.Add(item);
                            list_env_ignore.Items.Add(item);
                        }
                    }

                    if (this.list_env_ignore.Items.Count > 0)
                    {
                        this.list_env_ignore.Tag = "active";
                        this.list_env_ignore.SelectedIndex = 0;
                    }
                }
            }

        }

        private void b_copy_backup_ignore_Click(object sender, EventArgs e)
        {
            var current = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(current)) return;


            var envList = this.DeployConfig.Env.Where(r => !r.Name.Equals(current) && r.WindowsBackUpIgnoreList.Any()).Select(r => r.Name).ToList();

            if (envList.Count < 1)
            {
                MessageBox.Show("no env have ignore value!");
                return;
            }
            RollBack rollBack = new RollBack(envList,true) { Text = "Select Env Name" };
            rollBack.SetButtonName("Copy");
            var r1 = rollBack.ShowDialog();
            if (r1 == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                var target = this.DeployConfig.Env.FirstOrDefault(r => r.Name.Equals(rollBack.SelectRollBackVersion));
                if (target != null)
                {
                    var existItem = list_backUp_ignore.Items.Cast<string>().ToList();
                    foreach (var item in target.WindowsBackUpIgnoreList)
                    {
                        this.DeployConfig.Env[combo_env_list.SelectedIndex].WindowsBackUpIgnoreList.Add(item);
                        if (!existItem.Contains(item)) list_backUp_ignore.Items.Add(item);
                    }

                    if (this.list_backUp_ignore.Items.Count > 0)
                    {
                        this.list_backUp_ignore.Tag = "active";
                        this.list_backUp_ignore.SelectedIndex = 0;
                    }
                }
            }
        }

        private void checkBox_Chinese_Click(object sender, EventArgs e)
        {
            GlobalConfig.IsChinease = checkBox_Chinese.Checked;
            MessageBox.Show("change success please reload antdeploy!");
        }

        private void btn_choose_msbuild_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Choose MSBuild.exe";
            fdlg.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
            fdlg.FilterIndex = 1;
            if (!string.IsNullOrEmpty(this.txt_msbuild_path.Text))
            {
                try
                {
                    fdlg.InitialDirectory = new FileInfo(this.txt_msbuild_path.Text).DirectoryName;
                }
                catch (Exception)
                {

                }
            }
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                if (!fdlg.FileName.ToLower().EndsWith("msbuild.exe"))
                {
                    MessageBox.Show("choose file error！");
                    return;
                }
                this.txt_msbuild_path.Text = fdlg.FileName;
                CommandHelper.MsBuildPath = GlobalConfig.MsBuildPath = this.txt_msbuild_path.Text;
            }
        }

        private void label_without_vs_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/18");
            Process.Start(sInfo);
        }

        private void combo_netcore_publish_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_netcore_publish_mode.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                PluginConfig.NetCorePublishMode = selectName;
            }
        }
        private void label39_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://docs.microsoft.com/en-us/dotnet/core/deploying/");
            Process.Start(sInfo);
        }

        private void label12_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/yuzd/AntDeployAgent/issues/16");
            Process.Start(sInfo);
        }

        private Tuple<bool,string> ShowInputMsgBox(string title,string message,string defaultValue = null)
        {
            InputDialog dialog = new InputDialog(message,title,defaultValue);
            dialog.SetInputLength(0,200);
            var result = dialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return new Tuple<bool, string>(false,string.Empty);
            }
            return new Tuple<bool, string>(true, dialog.Input);
        }
    }
}
