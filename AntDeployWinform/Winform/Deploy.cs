using AntDeployWinform.Models;
using AntDeployWinform.Util;
using CCWin;
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
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToastHelper;
using Exception = System.Exception;
using Process = System.Diagnostics.Process;
using MessageBoxEx = AntDeployWinform.Models.MessageBoxEx;

namespace AntDeployWinform.Winform
{
    [Serializable]
    public partial class Deploy : CCSkinMain
    {
        private AutoResetEvent Condition { get; set; }
        private Tuple<string,string> ProjectConfigPath;
        private string ProjectFolderPath;
        private string ProjectName;
        private string ProjectPath;
        private string PluginConfigPath;//这个是按照项目来的
        private string ConfigPath;//这个是全局配置
        private ProjectParam _project;
        private FirstCreateParam _CreateParam = new FirstCreateParam();
        private RollBackVersion _rollBackVersion = new RollBackVersion();
        private NLog.Logger nlog_iis;
        private NLog.Logger nlog_windowservice;
        private NLog.Logger nlog_docker;
        private NLog.Logger nlog_config;
        private NLog.Logger nlog_image;
        private NLog.Logger nlog_linux;

        private int ProgressPercentage = 0;
        private string ProgressCurrentHost = null;
        private string ProgressCurrentHostForWindowsService = null;
        private string ProgressCurrentHostForLinuxService = null;
        private int ProgressPercentageForWindowsService = 0;
        private int ProgressPercentageForLinuxService = 0;
        private int ProgressBoxLocationLeft = 30;

        private volatile bool stop_iis_cancel_token;
        private volatile bool stop_windows_cancel_token;
        private volatile bool stop_linux_cancel_token;
        private volatile bool stop_docker_cancel_token;
        private volatile bool stop_docker_image_cancel_token;

        private string _webSiteName = string.Empty;
        private string _windowsServiceName = string.Empty;
        private string _linuxServiceName = string.Empty;
        private string _linuxEnvName = string.Empty;
        private string _dockerPort = string.Empty;
        private string _dockerEnvName = string.Empty;
        private string _dockerVolume = string.Empty;
        private string _dockerOther = string.Empty;
        private string iconPath = string.Empty;

        ToastHelper.NotificationService notificationService = new NotificationService();
        private string _formText = "";
        private SystemMenu systemMemu;
        public Deploy(string projectPath = null, ProjectParam project = null)
        {            
            this.Deploy_InitLoad(projectPath, project);
        }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="project"></param>
        /// <param name="isFirstLoad">首次加载</param>
        public void Deploy_InitLoad(string projectPath, ProjectParam project, bool isFirstLoad = true)
        {            
            ConfigPath = ProjectHelper.GetPluginConfigPath();
            ReadConfig(ConfigPath);

            if (isFirstLoad)
            {
                LoadLanguage();
                InitializeComponent();
                this._formText = this.Text;
                Assembly assembly = typeof(Deploy).Assembly;
                using (Stream stream = assembly.GetManifestResourceStream("AntDeployWinform.Resources.Logo12.ico"))
                {
                    if (stream != null)
                    {
                        this.Icon = new Icon(stream);
                        iconPath = Path.Combine(new FileInfo(ConfigPath).Directory.FullName, "antdeploy.ico");
                        try
                        {
                            using (var stream2 = new System.IO.FileStream(iconPath, System.IO.FileMode.Create))
                            {

                                this.Icon.Save(stream2);
                            }
                        }
                        catch (Exception)
                        {
                            iconPath = null;
                        }

                    }
                }
            }
            if (project != null && project.IsFirst)
            {
                this.Activated -= Deploy_Activated;
                Init(projectPath, null, true);
            }
            else
            {
                Init(projectPath, project, isFirstLoad);
            }

            if (isFirstLoad)
            {
                NlogConfig();

                Condition = new AutoResetEvent(false);

                this.txt_iis_web_site_name.DataBindings.Add("Text", this, "BindWebSiteName", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_windowservice_name.DataBindings.Add("Text", this, "BindWindowsServiceName", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_docker_port.DataBindings.Add("Text", this, "BindDockerPort", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_docker_envname.DataBindings.Add("Text", this, "BindDockerEnvName", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_docker_volume.DataBindings.Add("Text", this, "BindDockerVolume", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_docker_other.DataBindings.Add("Text", this, "BindDockerOther", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_linuxservice_name.DataBindings.Add("Text", this, "BindLinuxServiceName", false, DataSourceUpdateMode.OnPropertyChanged);
                this.txt_linux_service_env.DataBindings.Add("Text", this, "BindLinuxEnvName", false, DataSourceUpdateMode.OnPropertyChanged);

                try
                {
#if !DEBUG
                    notificationService.Init("AntDeploy");
#endif
                }
                catch (Exception)
                {
                    //ignore
                }

                try
                {
                    systemMemu = new SystemMenu(this.Handle);
                }
                catch (Exception)
                {
                    //有些系统不支持
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowsMessageHelper.JumplistHelpArgs)
            {
                Process.Start("https://github.com/yuzd/AntDeploy");
            }
            else
            {
                try
                {
                    base.WndProc(ref m);
                }
                catch (Exception)
                {
                }
            }
        }

        public string BindDockerVolume
        {
            get { return _dockerVolume; }
            set
            {
                _dockerVolume = value;

                var selectName = this.combo_docker_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.DockerConfig != null && DeployConfig.DockerConfig.EnvPairList != null)
                {
                    var first = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.DockerVolume = _dockerVolume;
                    }
                    else
                    {
                        DeployConfig.DockerConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            DockerVolume = _dockerVolume
                        });
                    }
                }
            }
        }

        public string BindDockerOther
        {
            get { return _dockerOther; }
            set
            {
                _dockerOther = value;

                var selectName = this.combo_docker_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.DockerConfig != null && DeployConfig.DockerConfig.EnvPairList != null)
                {
                    var first = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.DockerOther = _dockerOther;
                    }
                    else
                    {
                        DeployConfig.DockerConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            DockerOther = _dockerOther
                        });
                    }
                }
            }
        }
        public string BindDockerEnvName
        {
            get { return _dockerEnvName; }
            set
            {
                _dockerEnvName = value;

                var selectName = this.combo_docker_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.DockerConfig != null && DeployConfig.DockerConfig.EnvPairList != null)
                {
                    var first = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.DockerEnvName = _dockerEnvName;
                    }
                    else
                    {
                        DeployConfig.DockerConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            DockerEnvName = _dockerEnvName
                        });
                    }
                }
            }
        }
        public string BindDockerPort
        {
            get { return _dockerPort; }
            set
            {
                _dockerPort = value;

                var selectName = this.combo_docker_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.DockerConfig != null && DeployConfig.DockerConfig.EnvPairList != null)
                {
                    var first = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.DockerPort = _dockerPort;
                    }
                    else
                    {
                        DeployConfig.DockerConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            DockerPort = _dockerPort
                        });
                    }
                }
            }
        }
        public string BindWebSiteName
        {
            get { return _webSiteName; }
            set
            {
                _webSiteName = value;

                var selectName = this.combo_iis_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.IIsConfig != null && DeployConfig.IIsConfig.EnvPairList != null)
                {
                    var first = DeployConfig.IIsConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.ConfigName = _webSiteName;
                    }
                    else
                    {
                        DeployConfig.IIsConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            ConfigName = _webSiteName
                        });
                    }
                }
            }
        }
        public string BindWindowsServiceName
        {
            get { return _windowsServiceName; }
            set
            {
                _windowsServiceName = value;
                var selectName = this.combo_windowservice_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.WindowsServiveConfig != null && DeployConfig.WindowsServiveConfig.EnvPairList != null)
                {
                    var first = DeployConfig.WindowsServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.ConfigName = _windowsServiceName;
                    }
                    else
                    {
                        DeployConfig.WindowsServiveConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            ConfigName = _windowsServiceName
                        });
                    }
                }
            }
        }

        public string BindLinuxServiceName
        {
            get { return _linuxServiceName; }
            set
            {
                _linuxServiceName = value;
                var selectName = this.combo_linux_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.LinuxServiveConfig != null && DeployConfig.LinuxServiveConfig.EnvPairList != null)
                {
                    var first = DeployConfig.LinuxServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.ConfigName = _linuxServiceName;
                    }
                    else
                    {
                        DeployConfig.LinuxServiveConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            ConfigName = _linuxServiceName
                        });
                    }
                }
            }
        }

        public string BindLinuxEnvName
        {
            get { return _linuxEnvName; }
            set
            {
                _linuxEnvName = value;
                var selectName = this.combo_linux_env.SelectedItem as string;
                if (string.IsNullOrEmpty(selectName)) return;
                if (DeployConfig.LinuxServiveConfig != null && DeployConfig.LinuxServiveConfig.EnvPairList != null)
                {
                    var first = DeployConfig.LinuxServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (first != null)
                    {
                        first.LinuxEnvParam = _linuxEnvName;
                    }
                    else
                    {
                        DeployConfig.LinuxServiveConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = selectName,
                            LinuxEnvParam = _linuxEnvName
                        });
                    }
                }
            }
        }

        public DeployConfig DeployConfig { get; set; } = new DeployConfig();
        public PluginConfig PluginConfig { get; set; } = new PluginConfig();
        public GlobalConfig GlobalConfig { get; set; } = new GlobalConfig();

        #region Form

        private void Init(string projectPath, ProjectParam project = null, bool isFirst = true)
        {
            if (string.IsNullOrEmpty(projectPath) || (project!=null && project.OpenNewWindow))
            {
                if (isFirst)
                {
                    return;
                }

                this.page_set.Enabled = false;
                this.page_docker.Enabled = false;
                this.page_window_service.Enabled = false;
                this.page_web_iis.Enabled = false;
                this.page_linux_service.Enabled = false;
                this.pag_advance_setting.Enabled = false;
                this.page_docker_img.Enabled = false;
                SelectProject selectProject = new SelectProject(GlobalConfig.ProjectPathList);
                var r = selectProject.ShowDialog();
                if (r == DialogResult.Cancel && string.IsNullOrEmpty(projectPath))
                {
                    this.Close();
                    return;
                }
                else if(r == DialogResult.OK)
                {
                    projectPath = selectProject.SelectProjectPath;
                }
                //保存记录
                this.page_set.Enabled = true;
                this.page_docker.Enabled = true;
                this.page_docker_img.Enabled = true;
                this.page_window_service.Enabled = true;
                this.page_linux_service.Enabled = true;
                this.page_web_iis.Enabled = true;
                this.pag_advance_setting.Enabled = true;

                this.BringToFront();
            }
            else
            {
                if (!isFirst) return;
            }
            //ProjectName = Path.GetFileNameWithoutExtension(projectPath);

            if (File.Exists(projectPath))
            {
                ProjectPath = projectPath;
                if (project == null || project.OpenNewWindow)
                {
                    //读配置
                    project = ProjectHelper.GetNetCoreParamInCsprojectFile(projectPath);
                }
            }
            ProjectName = "";
            if (Directory.Exists(projectPath))
            {
                int index = projectPath.TrimEnd('\\').LastIndexOf("\\"); //是文件夹，文件夹名称可能也会有点
                ProjectName = projectPath.TrimEnd('\\').Substring(index + 1);

                ProjectFolderPath = projectPath;
                project = new ProjectParam
                {
                    IsNetcorePorject = true,
                    IsWebProejct = true
                };

                this.btn_choose_folder.Visible = false;
                this.btn_folder_clear.Visible = false;

                this.Text = $"{this._formText}(Version:{Vsix.VERSION})[FolderDeploy:{ProjectName}]";
            }
            else
            {
                ProjectName = Path.GetFileNameWithoutExtension(projectPath); //是文件，取文件名
                this.Text = $"{this._formText}(Version:{Vsix.VERSION})[{ProjectName}]";
            }

            //this.Text = Vsix.FORM_NAME;

            if (string.IsNullOrEmpty(project.DomainPath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var codeBase = assembly.Location;
                project.DomainPath = Path.GetDirectoryName(codeBase);
            }

            _project = project;
            CommandHelper.MsBuildPath = _project?.MsBuildPath;
            ReadPorjectConfig(projectPath);
            PluginConfigPath = ProjectHelper.GetPluginConfigPath(projectPath);
            ReadPluginConfig(PluginConfigPath);
            if (Directory.Exists(projectPath))
            {
                PluginConfig.DeployFolderPath = projectPath;
            }
            if (GlobalConfig.ProjectPathList == null) GlobalConfig.ProjectPathList = new List<string>();
            GlobalConfig.ProjectPathList.Insert(0, projectPath);
            GlobalConfig.ProjectPathList = GlobalConfig.ProjectPathList.Distinct().ToList();
            if (!isFirst)
            {
                Reload();
            }
        }

        private void NlogConfig()
        {

            #region Nlog
            
            /*
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
                UseDefaultRowColoringRules = true,                
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


            var richTarget22 = new RichTextBoxTarget
            {
                Name = "rich_linuxservice_log",
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_linuxservice_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_linuxservice_log", richTarget22);
            LoggingRule rule22 = new LoggingRule("*", LogLevel.Debug, richTarget22);
            config.LoggingRules.Add(rule22);

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


            var richTarget4 = new RichTextBoxTarget
            {
                Name = "rich_config_log",
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_config_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_config_log", richTarget4);
            LoggingRule rule4 = new LoggingRule("*", LogLevel.Debug, richTarget4);
            config.LoggingRules.Add(rule4);


            var richTarget5 = new RichTextBoxTarget
            {
                Name = "rich_docker_image_log",
                Layout =
                    "${date:format=HH\\:mm\\:ss}|${uppercase:${level}}|${message} ${exception:format=tostring} ${rtb-link:inner=${event-properties:item=ShowLink}}",
                FormName = "Deploy",
                ControlName = "rich_docker_image_log",
                AutoScroll = true,
                MaxLines = 0,
                AllowAccessoryFormCreation = false,
                SupportLinks = true,
                UseDefaultRowColoringRules = true

            };
            config.AddTarget("rich_docker_image_log", richTarget5);
            LoggingRule rule5 = new LoggingRule("*", LogLevel.Debug, richTarget5);
            config.LoggingRules.Add(rule5);

            LogManager.Configuration = config;
            */
            LogManager.Setup().SetupExtensions(ext => ext.RegisterAssembly(typeof(RichTextBoxTarget).Assembly));
            nlog_iis = NLog.LogManager.GetLogger("rich_iis_log");
            nlog_windowservice = NLog.LogManager.GetLogger("rich_windowservice_log");
            nlog_linux = NLog.LogManager.GetLogger("rich_linuxservice_log");
            nlog_docker = NLog.LogManager.GetLogger("rich_docker_log");
            nlog_config = NLog.LogManager.GetLogger("rich_config_log");
            nlog_image = NLog.LogManager.GetLogger("rich_docker_image_log");

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
            if (systemMemu != null)
            {
                //把自己给排除掉
                systemMemu.AddToRecentList(GlobalConfig.ProjectPathList);
            }
        }
        private void Deploy_Activated(object sender, EventArgs e)
        {
            this.Activated -= Deploy_Activated;
            Init(ProjectPath, _project, false);
        }
        private void Reload()
        {
            this.checkBox_Chinese.Checked = GlobalConfig.IsChinease;
            ProgressBox.IsEnableGroup = GlobalConfig.EnableEnvGroup;
            this.chk_global_useCheckBox.Checked = GlobalConfig.EnableEnvGroup;
            this.chk_global_saveconfig_in_projectFolder.Checked = GlobalConfig.EnableAntDeployJson;
            this.chk_use_AsiaShanghai_timezone.Checked = GlobalConfig.UseAsiaShanghai;
            this.checkBox_save_deploy_log.Checked = GlobalConfig.SaveLogs;
            this.checkBox_multi_deploy.Checked = GlobalConfig.MultiInstance;

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
            this.combo_env_list.Items.Clear(); //添加前先清空旧数据
            this.combo_iis_env.Items.Clear();
            this.combo_windowservice_env.Items.Clear();
            this.combo_linux_env.Items.Clear();
            this.combo_docker_env.Items.Clear();
            if (DeployConfig.Env != null && DeployConfig.Env.Any())
            {
                foreach (var env in DeployConfig.Env)
                {
                    this.combo_env_list.Items.Add(env.Name);
                    this.combo_iis_env.Items.Add(env.Name);
                    this.combo_windowservice_env.Items.Add(env.Name);
                    this.combo_linux_env.Items.Add(env.Name);
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
                    _webSiteName = this.txt_iis_web_site_name.Text = DeployConfig.IIsConfig.WebSiteName;
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
                    _windowsServiceName = this.txt_windowservice_name.Text = DeployConfig.WindowsServiveConfig.ServiceName;
                }


            }

            if (DeployConfig.LinuxServiveConfig != null)
            {
                if (this.combo_linux_env.Items.Count > 0 &&
                    !string.IsNullOrEmpty(DeployConfig.LinuxServiveConfig.LastEnvName)
                    && this.combo_linux_env.Items.Cast<string>()
                        .Contains(DeployConfig.LinuxServiveConfig.LastEnvName))
                {
                    this.combo_linux_env.SelectedItem = DeployConfig.LinuxServiveConfig.LastEnvName;
                }

                if (!string.IsNullOrEmpty(DeployConfig.LinuxServiveConfig.ServiceName))
                {
                    _linuxServiceName = this.txt_linuxservice_name.Text = DeployConfig.LinuxServiveConfig.ServiceName;
                }
                if (!string.IsNullOrEmpty(DeployConfig.LinuxServiveConfig.EnvParam))
                {
                    _linuxEnvName = this.txt_linux_service_env.Text = DeployConfig.LinuxServiveConfig.EnvParam;
                }


            }

            if (DeployConfig.DockerImageConfig != null)
            {
                this.txt_BaseImage.Text = DeployConfig.DockerImageConfig.BaseImage;
                this.txt_BaseImage_username.Text = DeployConfig.DockerImageConfig.BaseImageCredential?.UserName ?? "";
                this.txt_BaseImage_pwd.Text = DeployConfig.DockerImageConfig.BaseImageCredential?.Password ?? "";
                this.txt_HttpProxy.Text = DeployConfig.DockerImageConfig.BaseHttpProxy;
                this.txt_TargetImage.Text = DeployConfig.DockerImageConfig.TargetImage;
                this.txt_TargetImage_username.Text = DeployConfig.DockerImageConfig.TargetImageCredential?.UserName ?? "";
                this.txt_TargetImage_pwd.Text = DeployConfig.DockerImageConfig.TargetImageCredential?.Password ?? "";
                this.txt_TargetImage_tag.Text = DeployConfig.DockerImageConfig.TargetTags != null && DeployConfig.DockerImageConfig.TargetTags.Any() ? DeployConfig.DockerImageConfig.TargetTags[0] : "";
                this.txt_Entrypoint.Text = DeployConfig.DockerImageConfig.Entrypoint != null && DeployConfig.DockerImageConfig.Entrypoint.Any() ? string.Join("->", DeployConfig.DockerImageConfig.Entrypoint) : "";
                this.txt_Cmd.Text = DeployConfig.DockerImageConfig.Cmd != null && DeployConfig.DockerImageConfig.Cmd.Any() ? string.Join("->", DeployConfig.DockerImageConfig.Cmd) : "";
                this.txt_TargetHttpProxy.Text = DeployConfig.DockerImageConfig.TargetHttpProxy;
                this.cmbo_ImageFormat.SelectedItem = DeployConfig.DockerImageConfig.ImageFormat;
                if (DeployConfig.DockerImageConfig.IgnoreList != null)
                {
                    foreach (var item in DeployConfig.DockerImageConfig.IgnoreList)
                    {
                        this.list_dockerImage_ignore.Items.Add(item);
                    }
                }

            }

            if (this.cmbo_ImageFormat.SelectedItem == null) this.cmbo_ImageFormat.SelectedItem = "Docker";

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
                    _dockerPort = this.txt_docker_port.Text = DeployConfig.DockerConfig.Prot;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.AspNetCoreEnv))
                {
                    _dockerEnvName = this.txt_docker_envname.Text = DeployConfig.DockerConfig.AspNetCoreEnv;
                }




                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.RemoveDaysFromPublished))
                {
                    this.t_docker_delete_days.Text = DeployConfig.DockerConfig.RemoveDaysFromPublished;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.Volume))
                {
                    _dockerVolume = this.txt_docker_volume.Text = DeployConfig.DockerConfig.Volume;
                }

                if (!string.IsNullOrEmpty(DeployConfig.DockerConfig.Other))
                {
                    _dockerOther = this.txt_docker_other.Text = DeployConfig.DockerConfig.Other;
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

            this.checkBox_iis_restart_site.Checked = PluginConfig.IISEnableNotStopSiteDeploy;
            this.checkBox_iis_use_offlinehtm.Checked = PluginConfig.IISEnableUseOfflineHtm;
            this.checkBox_Increment_iis.Checked = PluginConfig.IISEnableIncrement;
            this.checkBox_Increment_docker.Checked = PluginConfig.DockerEnableIncrement;
            this.checkBox_sudo_docker.Checked = PluginConfig.DockerEnableSudo;
            this.checkBox_select_deploy_docker.Checked = PluginConfig.DockerServiceEnableSelectDeploy;
            this.checkBox_Increment_window_service.Checked = PluginConfig.WindowsServiceEnableIncrement;
            this.checkBox_Increment_linux_service.Checked = PluginConfig.LinuxServiceEnableIncrement;
            this.checkBox_select_deploy_service.Checked = PluginConfig.WindowsServiceEnableSelectDeploy;
            this.checkBox_select_deploy_linuxservice.Checked = PluginConfig.LinuxServiceEnableSelectDeploy;
            this.checkBox_select_type_linuxservice.Checked = PluginConfig.LinuxServiceNotifySystemd;
            this.checkBox_select_deploy_iis.Checked = PluginConfig.IISEnableSelectDeploy;
            this.txt_folder_deploy.Text = PluginConfig.DeployFolderPath;
            this.txt_http_proxy.Text = PluginConfig.DeployHttpProxy;

            this.checkBoxdocker_rep_enable.Checked = PluginConfig.DockerServiceEnableUpload;
            this.checkBoxdocker_rep_uploadOnly.Checked = PluginConfig.DockerServiceBuildImageOnly;
            this.txt_docker_rep_domain.Text = PluginConfig.RepositoryUrl;
            this.txt_docker_rep_name.Text = PluginConfig.RepositoryUserName;
            this.txt_docker_rep_pwd.Text = PluginConfig.RepositoryUserPwd;
            this.txt_docker_rep_namespace.Text = PluginConfig.RepositoryNameSpace;
            this.txt_docker_rep_image.Text = PluginConfig.RepositoryImageName;

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
            RichTextBoxTarget.GetTargetByControl(rich_config_log).LinkClicked += LinkClicked;
            RichTextBoxTarget.GetTargetByControl(rich_linuxservice_log).LinkClicked += LinkClicked;
            RichTextBoxTarget.GetTargetByControl(rich_docker_image_log).LinkClicked += LinkClicked;

        }


        private void LinkClicked(RichTextBoxTarget sender, string linktext, LogEventInfo logevent)
        {
            BeginInvokeLambda(() =>
                {
                    try
                    {
                        if (linktext.StartsWith("file://removeWinServer_"))
                        {
                            b_env_server_remove_Click(linktext.Split('_')[1]);
                        }
                        else if (linktext.StartsWith("file://removeLinuxServer_"))
                        {
                            b_linux_server_remove_Click(linktext.Split('_')[1]);
                        }
                        else if (linktext.StartsWith("http") || linktext.StartsWith("file:"))
                        {
                            if (linktext.StartsWith("file://%LOCALAPPDATA%"))
                            {
                                ProcessStartInfo sInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(linktext.Replace("file://", "").Replace("/", "\\")));
                                Process.Start(sInfo);

                                return;
                            }
                            ProcessStartInfo sInfo2 = new ProcessStartInfo(linktext);
                            Process.Start(sInfo2);
                        }
                        else
                        {
                            System.Windows.Forms.Clipboard.SetText(linktext);
                            MessageBoxEx.Show(this, "copy to Clipboard success", sender.Name);
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
            RichTextBoxTarget.GetTargetByControl(rich_linuxservice_log)?.Dispose();


            this.b_iis_rollback.Dispose();
            this.b_docker_rollback.Dispose();
            this.b_windows_service_rollback.Dispose();
            this.b_linux_service_rollback.Dispose();

            this.txt_docker_volume.Dispose();

            this.loading_win_server_test.Dispose();
            this.loading_linux_server_test.Dispose();

            this.rich_docker_log.Dispose();
            this.rich_iis_log.Dispose();
            this.rich_windowservice_log.Dispose();
            this.rich_linuxservice_log.Dispose();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="dispose"></param>
        private void Unload(bool dispose=true)
        {
            try
            {
                if (dispose)
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
                    if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList4)
                    {
                        foreach (var box in progressBoxList4)
                        {
                            box.Value.Dispose();
                            //this.tabPage_windows_service.Controls.Remove(box.Value);
                        }
                    }
                }

                dockerImageParams();
                GlobalConfig.MsBuildPath = this.txt_msbuild_path.Text.Trim();
                GlobalConfig.ProjectPathList = GlobalConfig.ProjectPathList.Take(15).ToList();
                PluginConfig.LastTabIndex = this.tabcontrol.SelectedIndex;
                PluginConfig.IISEnableIncrement = this.checkBox_Increment_iis.Checked;
                PluginConfig.WindowsServiceEnableIncrement = this.checkBox_Increment_window_service.Checked;
                PluginConfig.LinuxServiceEnableIncrement = this.checkBox_Increment_linux_service.Checked;
                PluginConfig.IISEnableSelectDeploy = this.checkBox_select_deploy_iis.Checked;
                PluginConfig.WindowsServiceEnableSelectDeploy = this.checkBox_select_deploy_service.Checked;
                PluginConfig.LinuxServiceEnableSelectDeploy = this.checkBox_select_deploy_linuxservice.Checked;
                PluginConfig.LinuxServiceNotifySystemd = this.checkBox_select_type_linuxservice.Checked;
                PluginConfig.DeployFolderPath = this.txt_folder_deploy.Text.Trim();
                PluginConfig.DeployHttpProxy = this.txt_http_proxy.Text.Trim();

                this.BindWebSiteName = DeployConfig.IIsConfig.WebSiteName = this.txt_iis_web_site_name.Text.Trim();

                this.BindWindowsServiceName = DeployConfig.WindowsServiveConfig.ServiceName = this.txt_windowservice_name.Text.Trim();
                this.BindLinuxServiceName = DeployConfig.LinuxServiveConfig.ServiceName = this.txt_linuxservice_name.Text.Trim();
                this.BindLinuxEnvName = DeployConfig.LinuxServiveConfig.EnvParam = this.txt_linux_service_env.Text.Trim();

                this.BindDockerPort = DeployConfig.DockerConfig.Prot = this.txt_docker_port.Text.Trim();
                this.BindDockerEnvName = DeployConfig.DockerConfig.AspNetCoreEnv = this.txt_docker_envname.Text.Trim();
                DeployConfig.DockerConfig.RemoveDaysFromPublished = this.t_docker_delete_days.Text.Trim();
                this.BindDockerVolume = DeployConfig.DockerConfig.Volume = this.txt_docker_volume.Text.Trim();
                this.BindDockerOther = DeployConfig.DockerConfig.Other = this.txt_docker_other.Text.Trim();


                PluginConfig.DockerServiceEnableUpload = this.checkBoxdocker_rep_enable.Checked;
                PluginConfig.DockerServiceBuildImageOnly = this.checkBoxdocker_rep_uploadOnly.Checked;
                PluginConfig.RepositoryUrl = this.txt_docker_rep_domain.Text.Trim();
                PluginConfig.RepositoryUserName = this.txt_docker_rep_name.Text.Trim();
                PluginConfig.RepositoryUserPwd = this.txt_docker_rep_pwd.Text.Trim();
                PluginConfig.RepositoryNameSpace = this.txt_docker_rep_namespace.Text.Trim();
                PluginConfig.RepositoryImageName = this.txt_docker_rep_image.Text.Trim();


                saveAntDeployJson();

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
                MessageBoxEx.Show(this, "please input env name first");
                return;
            }

            //检查特殊字符
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (env_name.Contains(c))
                {
                    MessageBoxEx.Show(this, "env name contains invalid char：" + c);
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

        private void b_env_server_remove_Click(string server)
        {
            //先找到 server 对应的 index
            var existServer = this.combo_env_server_list.Items.Cast<string>().Select((r, index) => new
            {
                Server = r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0],
                Index = index
            })
                .FirstOrDefault(r => r.Server.Equals(server));
            if (existServer == null) return;

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].ServerList.RemoveAt(existServer.Index);
            this.combo_env_server_list.Items.RemoveAt(existServer.Index);
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
                MessageBoxEx.Show(this, "nickName can not Contains @_@");
                return;
            }

            var serverHost = this.txt_env_server_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server host");
                return;
            }

            if (serverHost.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "server host can not Contains @_@");
                return;
            }

            var serverTolen = this.txt_env_server_token.Text.Trim();
            if (serverTolen.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server Token");
                return;
            }

            if (serverTolen.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "server Token can not Contains @_@");
                return;
            }

            var existServer = this.combo_env_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBoxEx.Show(this, "input server host is exist!" + Environment.NewLine +
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
        /// window server 链接测试全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_win_server_testAll_Click(object sender, EventArgs e)
        {
            if (this.combo_env_server_list.Items.Count < 1)
            {
                MessageBoxEx.Show(this, "Please add the server first");
                return;
            }

            List<Tuple<string, string, string>> allServerList = new List<Tuple<string, string, string>>();
            var existServerList = this.combo_env_server_list.Items.Cast<string>();
            foreach (var seletedServer in existServerList)
            {
                var arr = seletedServer.Split(new string[] { "@_@" }, StringSplitOptions.None);
                if (arr.Length >= 2)
                {
                    if (arr.Length == 3)
                    {
                        allServerList.Add(new Tuple<string, string, string>(arr[0], arr[1], arr[2]));
                    }
                    else
                    {
                        allServerList.Add(new Tuple<string, string, string>(arr[0], arr[1], ""));
                    }
                }
            }
            if (allServerList.Count < 1)
            {
                MessageBoxEx.Show(this, "Please add the server first");
                return;
            }

            new Task(() =>
            {
                EnableForTestWinServerAll(false);
                this.nlog_config.Info($"Connect Test All Start");
                WebClient client = new WebClient();
                if (!string.IsNullOrEmpty(this.PluginConfig.DeployHttpProxy))
                {
                    var arr = this.PluginConfig.DeployHttpProxy.Split(':');
                    if (arr.Length == 2)
                    {
                        this.nlog_config.Info($"Use Proxy：【{this.PluginConfig.DeployHttpProxy}】");
                        client.Proxy = new WebProxy(this.PluginConfig.DeployHttpProxy);
                    }
                    else
                    {
                        this.nlog_config.Warn($"Invaild Proxy：【{this.PluginConfig.DeployHttpProxy}】");
                    }
                }
                else
                {
                    client.Proxy = null;
                }

                try
                {
                    var index = 0;
                    foreach (var server in allServerList)
                    {
                        try
                        {
                            //检查是否启动了系统代理
                            var systemProxy = WebRequest.GetSystemWebProxy();
                            if (systemProxy != null)
                            {
                                var destination = new Uri($"http://{server.Item1}");
                                var proxyServer = systemProxy.GetProxy(destination);
                                if (proxyServer != null)
                                {
                                    this.nlog_config.Warn($"[pay attention] find system proxy:[{proxyServer}]");
                                }
                            }

                            this.nlog_config.Info($"Connect Start -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");
                            var result = client.DownloadString($"http://{server.Item1}/publish?Token={WebUtility.UrlEncode(server.Item2)}");
                            if (result.Equals("success"))
                            {
                                this.nlog_config.Info($"Connect Success -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】,response:{result}");
                            }
                            else
                            {
                                this.nlog_config.Error($"Connect Fail -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】,response:{result}");

                                LogEventInfo publisEvent = new LogEventInfo(LogLevel.Warn, "", $"Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】 -> click to remove -->");
                                publisEvent.Properties["ShowLink"] = $"file://removeWinServer_" + server.Item1;
                                publisEvent.LoggerName = "rich_config_log";
                                this.nlog_config.Log(publisEvent);

                            }
                        }
                        catch (Exception exception)
                        {
                            this.nlog_config.Error($"Connect Fail -> Host:【{server.Item1}】,err:{exception.Message}");
                            LogEventInfo publisEvent = new LogEventInfo(LogLevel.Warn, "", $"Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】 -> click to remove -->");
                            publisEvent.Properties["ShowLink"] = $"file://removeWinServer_" + server.Item1;
                            publisEvent.LoggerName = "rich_config_log";
                            this.nlog_config.Log(publisEvent);
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    this.nlog_config.Error($"Fail ex:{ex.Message},Skip to Next");
                }
                finally
                {
                    EnableForTestWinServer(true);
                    client.Dispose();
                    this.nlog_config.Info($"Connect Test All End");
                }

            }).Start();

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
                MessageBoxEx.Show(this, "please input server host");
                return;
            }

            var serverTolen = this.txt_env_server_token.Text.Trim();
            if (serverTolen.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server Token");
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
                    var result = client.DownloadString($"http://{serverHost}/publish?Token={WebUtility.UrlEncode(serverTolen)}");

                    this.BeginInvokeLambda(() =>
                    {
                        if (result.Equals("success"))
                        {
                            MessageBoxEx.Show(this, "Connect Success", "AntDeploy", MessageBoxButtons.OK, MessageBoxIcon.None);
                        }
                        else
                        {
                            MessageBoxEx.Show(this, "Connect Fail");
                        }
                    });
                }
                catch (Exception)
                {
                    this.BeginInvokeLambda(() => { MessageBoxEx.Show(this, "Connect Fail"); });
                }
                finally
                {
                    EnableForTestWinServer(true);
                    client.Dispose();
                }

            }).Start();

        }

        private void EnableForTestWinServerAll(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {
                if (!flag)
                {
                    this.rich_config_log.Text = "";
                    this.panel_rich_config_log.Visible = true;
                    this.btn_rich_config_log_close.Visible = true;
                }
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
                MessageBoxEx.Show(this, "please input ignore rule");
                return;
            }

            if (ignoreTxt.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "can not contains @_@");
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
                MessageBoxEx.Show(this, "please input backUp ignore rule");
                return;
            }
            if (ignoreTxt.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "can not contains @_@");
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
                MessageBoxEx.Show(this, "nickName can not Contains @_@");
                return;
            }
            var serverHost = this.txt_linux_host.Text.Trim();
            if (serverHost.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server host");
                return;
            }

            if (serverHost.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "server host can not Contains @_@");
                return;
            }

            var userName = this.txt_linux_username.Text.Trim();
            if (userName.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server userName");
                return;
            }

            if (userName.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "userName can not Contains @_@");
                return;
            }

            var pwd = this.txt_linux_pwd.Text.Trim();
            if (pwd.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server pwd");
                return;
            }

            if (pwd.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "pwd can not Contains @_@");
                return;
            }

            var pwd2 = CodingHelper.AESEncrypt(pwd);
            var existServer = this.combo_linux_server_list.Items.Cast<string>()
                .Select(r => r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0])
                .FirstOrDefault(r => r.Equals(serverHost));

            if (!string.IsNullOrEmpty(existServer))
            {
                MessageBoxEx.Show(this, "input server host is exist!" + Environment.NewLine +
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

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList.RemoveAt(this.combo_linux_server_list.SelectedIndex);
            this.combo_linux_server_list.Items.Remove(seletedServer);
            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);
        }
        private void b_linux_server_remove_Click(string server)
        {
            var existServer = this.combo_linux_server_list.Items.Cast<string>().Select((r, index) => new
            {
                Server = r.Split(new string[] { "@_@" }, StringSplitOptions.None)[0],
                Index = index
            })
                .FirstOrDefault(r => r.Server.Equals(server));
            if (existServer == null) return;

            this.DeployConfig.Env[this.combo_env_list.SelectedIndex].LinuxServerList.RemoveAt(existServer.Index);
            this.combo_linux_server_list.Items.RemoveAt(existServer.Index);
            DeployConfig.EnvServerChange(DeployConfig.Env[this.combo_env_list.SelectedIndex]);
        }

        /// <summary>
        /// linux server 测试全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_linux_server_testAll_Click(object sender, EventArgs e)
        {
            if (this.combo_linux_server_list.Items.Count < 1)
            {
                MessageBoxEx.Show(this, "Please add the server first");
                return;
            }

            var envName = this.combo_docker_env.SelectedItem as string;

            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, "Please select env first");
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList).FirstOrDefault();
            if (serverList == null || serverList.Count < 1)
            {
                MessageBoxEx.Show(this, "Please add the server first");
                return;
            }


            List<Tuple<string, string, string>> allServerList = new List<Tuple<string, string, string>>();
            var existServerList = this.combo_linux_server_list.Items.Cast<string>();
            foreach (var seletedServer in existServerList)
            {
                var arr = seletedServer.Split(new string[] { "@_@" }, StringSplitOptions.None);
                if (arr.Length >= 2)
                {
                    if (arr.Length == 3)
                    {
                        allServerList.Add(new Tuple<string, string, string>(arr[0], arr[1], arr[2]));
                    }
                    else
                    {
                        allServerList.Add(new Tuple<string, string, string>(arr[0], arr[1], ""));
                    }
                }
            }
            if (allServerList.Count < 1)
            {
                MessageBoxEx.Show(this, "Please add the server first");
                return;
            }

            new Task(() =>
            {
                EnableForTestLinuxServerAll(false);
                this.nlog_config.Info($"Connect Test All Start");
                if (!string.IsNullOrEmpty(this.PluginConfig.DeployHttpProxy))
                {
                    var arr = this.PluginConfig.DeployHttpProxy.Split(':');
                    if (arr.Length == 2)
                    {
                        this.nlog_config.Info($"Use Proxy：【{this.PluginConfig.DeployHttpProxy}】");
                    }
                    else
                    {
                        this.nlog_config.Warn($"Invaild Proxy：【{this.PluginConfig.DeployHttpProxy}】");
                    }
                }

                try
                {


                    var index = 0;
                    foreach (var server in allServerList)
                    {
                        try
                        {
                            this.nlog_config.Info($"Connect Start -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");

                            //获取pwd
                            var pwd = serverList.Where(r => r.Host.Equals(server.Item1)).FirstOrDefault();

                            if (pwd == null || string.IsNullOrEmpty(pwd.Pwd))
                            {
                                this.nlog_config.Error($"Get pwd From Config File Fail -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");
                                index++;
                                continue;
                            }

                            var pwd1 = CodingHelper.AESDecrypt(pwd.Pwd);
                            if (string.IsNullOrEmpty(pwd1))
                            {
                                this.nlog_config.Error($"Get pwd From Config File Fail -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");
                                index++;
                                continue;
                            }

                            using (SSHClient sshClient = new SSHClient(pwd.Host, pwd.UserName, pwd1, this.PluginConfig.DeployHttpProxy))
                            {
                                var r = sshClient.Connect(true);
                                this.BeginInvokeLambda(() =>
                                {
                                    if (r)
                                    {
                                        this.nlog_config.Info($"Connect Success -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");
                                    }
                                    else
                                    {
                                        this.nlog_config.Error($"Connect Fail -> Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】");

                                        LogEventInfo publisEvent = new LogEventInfo(LogLevel.Warn, "", $"Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】 -> click to remove -->");
                                        publisEvent.Properties["ShowLink"] = $"file://removeLinuxServer_" + server.Item1;
                                        publisEvent.LoggerName = "rich_config_log";
                                        this.nlog_config.Log(publisEvent);
                                    }
                                });
                            }
                        }
                        catch (Exception exception)
                        {
                            this.nlog_config.Error($"Connect Fail -> Host:【{server.Item1}】,err:{exception.Message}");
                            LogEventInfo publisEvent = new LogEventInfo(LogLevel.Warn, "", $"Host:【{server.Item1 + (!string.IsNullOrEmpty(server.Item3) ? $"[{server.Item3}]" : "")}】 -> click to remove -->");
                            publisEvent.Properties["ShowLink"] = $"file://removeLinuxServer_" + server.Item1;
                            publisEvent.LoggerName = "rich_config_log";
                            this.nlog_config.Log(publisEvent);
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    this.nlog_config.Error($"Fail ex:{ex.Message},Skip to Next");
                }
                finally
                {
                    EnableForTestLinuxServer(true);
                    this.nlog_config.Info($"Connect Test All End");
                }

            }).Start();
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
                MessageBoxEx.Show(this, "please input server host");
                return;
            }

            var userName = this.txt_linux_username.Text.Trim();
            if (userName.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server userName");
                return;
            }

            var pwd = this.txt_linux_pwd.Text.Trim();
            if (pwd.Length < 1)
            {
                MessageBoxEx.Show(this, "please input server pwd");
                return;
            }

            this.loading_linux_server_test.Visible = true;
            new Task(() =>
            {
                EnableForTestLinuxServer(false);
                try
                {
                    using (SSHClient sshClient =
                        new SSHClient(serverHost, userName, pwd, this.PluginConfig.DeployHttpProxy))
                    {
                        var r = sshClient.Connect(true);
                        this.BeginInvokeLambda(() =>
                        {
                            if (r)
                            {
                                MessageBoxEx.Show(this, "Connect Success", "AntDeploy", MessageBoxButtons.OK, MessageBoxIcon.None);
                            }
                            else
                            {
                                MessageBoxEx.Show(this, "Connect Fail");
                            }
                        });

                    }
                }
                catch (Exception)
                {
                    this.BeginInvokeLambda(() => { MessageBoxEx.Show(this, "Connect Fail"); });
                }
                finally
                {
                    EnableForTestLinuxServer(true);
                }
            }).Start();

        }
        private void EnableForTestLinuxServerAll(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {
                if (!flag)
                {
                    this.panel_rich_config_log.Visible = true;
                    this.rich_config_log.Text = "";
                    this.btn_rich_config_log_close.Visible = true;
                }
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

            if (serverType.Equals(ServerType.LINUXSERVICE))
            {
                //生成进度
                if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
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

        private List<BaseServer> getSelectedBaseServers(ServerType serverType)
        {
            var result = new List<BaseServer>();
            if (serverType.Equals(ServerType.IIS))
            {
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        var s = box.Value.GetServer();
                        if (s == null) continue;
                        result.Add(s);
                    }
                }
            }
            else if (serverType.Equals(ServerType.DOCKER))
            {
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        var s = box.Value.GetServer();
                        if (s == null) continue;
                        result.Add(s);
                    }
                }
            }
            else if (serverType.Equals(ServerType.WINSERVICE))
            {
                //生成进度
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        var s = box.Value.GetServer();
                        if (s == null) continue;
                        result.Add(s);
                    }
                }
            }
            else if (serverType.Equals(ServerType.LINUXSERVICE))
            {
                //生成进度
                if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        var s = box.Value.GetServer();
                        if (s == null) continue;
                        result.Add(s);
                    }
                }
            }
            return result;
        }

        private void combo_iis_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_iis_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.IIsConfig.LastEnvName = selectName;

                //设置对应的websitename
                if (DeployConfig.IIsConfig.EnvPairList != null && DeployConfig.IIsConfig.EnvPairList.Any())
                {
                    var target = DeployConfig.IIsConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (target != null && !string.IsNullOrEmpty(target.ConfigName))
                    {
                        this.txt_iis_web_site_name.Text = target.ConfigName;
                    }
                }
                var dic = new Dictionary<string, bool>();
                //生成进度
                if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        dic.Add(box.Value.Server.Host, box.Value.CheckBox.Checked);
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
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.IIS, HostoryButtonSearch)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    if (dic.TryGetValue(serverHost, out var chec))
                    {
                        newBox.CheckBox.Checked = chec;
                    }


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
            Unload(false);
            stop_iis_cancel_token = false;
            Condition = new AutoResetEvent(false);
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
                MessageBoxEx.Show(this, Strings.NotWebProject);
                return;
            }

            var websiteName = this.txt_iis_web_site_name.Text.Trim();
            if (websiteName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.WebSiteNameRequired);
                return;
            }

            var sdkTypeName = this.combo_iis_sdk_type.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(sdkTypeName))
            {
                MessageBoxEx.Show(this, Strings.SelectSdkType);
                return;
            }

            var envName = this.combo_iis_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.IIS))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;
            var backUpIgnoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).WindowsBackUpIgnoreList;

            var Port = "";
            var PoolName = "";
            var PhysicalPath = "";
            bool PoolAlwaysRunning = false;
            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                return;
            }


            if (ProgressBox.IsEnableGroup)
            {
                //获取所有的选择了的server
                var selectedList = getSelectedBaseServers(ServerType.IIS);
                if (!selectedList.Any())
                {
                    MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                    return;
                }

                //找到选择了的
                serverList = serverList.Where(r => selectedList.Any(y => y.Host.Equals(r.Host))).ToList();
                if (!serverList.Any())
                {
                    MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                    return;
                }
            }


            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());
            var confirmResult = ShowInputMsgBox(Strings.ConfirmDeploy,
                Strings.DeployServerConfim + Environment.NewLine + serverHostList);
            //var confirmResult = MessageBoxEx.Show(this,
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
            _CreateParam = new FirstCreateParam();
            new Task(async () =>
            {


                this.nlog_iis.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
                if (PluginConfig.IISEnableUseOfflineHtm)
                {
                    nlog_iis.Info("Do not stop webSite during deploy use app_offline.htm!");
                }
                else if (PluginConfig.IISEnableNotStopSiteDeploy)
                {
                    nlog_iis.Info("Do not stop webSite during deploy!");
                }

                PrintCommonLog(this.nlog_iis);
                Enable(false); //第一台开始编译
                GitClient gitModel = null;
                var gitPath = string.Empty;
                var webFolderName = string.Empty;
                var isRuningSelectDeploy = false;
                var isNetcore = DeployConfig.IIsConfig.SdkType.Equals("netcore");

                var publishPath = !string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? PluginConfig.DeployFolderPath : Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_iis", envName);

                var dateTimeFolderNameParent = string.Empty;
                try
                {

                    if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                    {
                        var path = publishPath + "\\";
                        if (isNetcore)
                        {
                            //执行 publish
                            var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                                $"publish \"{ProjectPath}\" -c Release{PluginConfig.GetNetCorePublishRuntimeArg()}", this.nlog_iis, () => stop_iis_cancel_token);

                            if (!isSuccess)
                            {
                                this.nlog_iis.Error("publish error,please check build log");
                                BuildError(this.tabPage_progress, serverList.First().Host);
                                return;
                            }
                        }
                        else
                        {
                            var isSuccess = CommandHelper.RunMsbuild(ProjectPath, path, this.nlog_iis, true, () => stop_iis_cancel_token);

                            if (!isSuccess)
                            {
                                this.nlog_iis.Error("publish error,please check build log");
                                BuildError(this.tabPage_progress, serverList.First().Host);
                                return;
                            }

                        }
                    }

                    BuildEnd(this.tabPage_progress, serverList.First().Host); //第一台结束编译
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

                    if (stop_iis_cancel_token)
                    {
                        this.nlog_iis.Warn($"deploy task was canceled!");
                        PackageError(this.tabPage_progress, serverList.First().Host);
                        return;
                    }

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
                            PackageError(this.tabPage_progress, serverList.First().Host);
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
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }
                        gitChangeFileCount = fileList.Count;
                        this.nlog_iis.Info("【git】Increment package file count:" + gitChangeFileCount);

                        if (this.PluginConfig.IISEnableSelectDeploy)
                        {
                            isRuningSelectDeploy = true;
                            this.nlog_iis.Info("-----------------Select File Start-----------------");
                            this.BeginInvokeLambda(() =>
                            {
                                var slectFileForm = new SelectFile(fileList, publishPath, ignoreList);
                                slectFileForm.ShowDialog();
                                // ReSharper disable once AccessToDisposedClosure
                                DoSelectDeployIIS(slectFileForm.SelectedFileList, publishPath, serverList, backUpIgnoreList, Port, PoolName, PhysicalPath, PoolAlwaysRunning, gitModel, confirmResult.Item2, ignoreList);
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
                                    UpdatePackageProgress(this.tabPage_progress, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_iis_cancel_token;
                                });
                        }
                        catch (Exception ex)
                        {
                            this.nlog_iis.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }
                    }
                    else if (this.PluginConfig.IISEnableSelectDeploy)
                    {
                        isRuningSelectDeploy = true;
                        this.nlog_iis.Info("-----------------Select File Start-----------------");
                        this.BeginInvokeLambda(() =>
                        {
                            var slectFileForm = new SelectFile(publishPath, ignoreList);
                            slectFileForm.ShowDialog();
                            DoSelectDeployIIS(slectFileForm.SelectedFileList, publishPath, serverList, backUpIgnoreList, Port, PoolName, PhysicalPath, PoolAlwaysRunning, null, confirmResult.Item2, ignoreList);
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
                                    UpdatePackageProgress(this.tabPage_progress, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_iis_cancel_token;
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.nlog_iis.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_progress, serverList.First().Host);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_iis.Error("package fail");
                        PackageError(this.tabPage_progress, serverList.First().Host);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_iis.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_iis.Info("-----------------Deploy Start-----------------");
                    dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var allfailServerList = new List<Server>();
                    var retryTimes = 0;
RETRY_IIS:
                    var failServerList = new List<Server>();
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = string.Empty;
                    var isRetry = allfailServerList.Count > 0;
                    if (isRetry)
                    {
                        dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                    }
                    else
                    {
                        dateTimeFolderName = dateTimeFolderNameParent;
                    }
                    //重试了 但是没有发现错误的Server List
                    if (retryTimes > 0 && allfailServerList.Count == 0) return;
                    foreach (var server in isRetry ? allfailServerList : serverList)
                    {
                        if (isRetry) UploadReset(this.tabPage_progress, server.Host);

                        if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_progress, server.Host);
                            UpdatePackageProgress(this.tabPage_progress, server.Host, 100);
                        }
                        if (stop_iis_cancel_token)
                        {
                            this.nlog_iis.Warn($"deploy task was canceled!");
                            UploadError(this.tabPage_progress, server.Host);
                            return;
                        }
                        index++;
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_iis.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_progress, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }



                        this.nlog_iis.Info("Start Check Website IsExist In Remote IIS:" + server.Host);
                        var checkIisResult = await WebUtil.HttpPostAsync<IIsSiteCheck>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "checkiis",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = DeployConfig.IIsConfig.WebSiteName
                            }, nlog_iis);

                        if (checkIisResult == null || checkIisResult.Data == null)
                        {
                            this.nlog_iis.Error($"Check Website IsExist In Remote IIS Fail!");
                            UploadError(this.tabPage_progress, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(checkIisResult.Msg))
                        {
                            this.nlog_iis.Error($"Check Website IsExist In Remote IIS Fail：" + checkIisResult.Msg);
                            UploadError(this.tabPage_progress, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }

                        if (checkIisResult.Data.Success)
                        {
                            this.nlog_iis.Info($"Website Is Exist In Remote IIS:" + server.Host);
                        }
                        else if (!checkIisResult.Data.Level1Exist)
                        {
                            if (this.PluginConfig.IISEnableIncrement)
                            {
                                //网站还不存在不能选择指定的文件发布 
                                this.nlog_iis.Error($"Website Is Not Exist In Remote IIS,Can not use [Increment deplpoy]");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            if (this.PluginConfig.IISEnableNotStopSiteDeploy)
                            {
                                //网站还不存在不能选择不关闭站点
                                this.nlog_iis.Error($"Website Is Not Exist In Remote IIS,Can not use [Do Not Stop Site]");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            if (this.PluginConfig.IISEnableUseOfflineHtm)
                            {
                                //网站还不存在不能选择不关闭站点
                                this.nlog_iis.Error($"Website Is Not Exist In Remote IIS,Can not use [Use app_offline.htm]");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            this.BeginInvokeLambda(() =>
                            {
                                //级别一不存在
                                FirstCreate creatFrom = new FirstCreate(true);
                                var data = creatFrom.ShowDialog();
                                if (data == DialogResult.Cancel)
                                {
                                    _CreateParam = null;
                                }
                                else
                                {
                                    _CreateParam = creatFrom.IsCreateParam;
                                }
                                Condition.Set();
                            });
                            Condition.WaitOne();
                            if (_CreateParam == null)
                            {
                                this.nlog_iis.Error($"Create Website Param Required!");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            else
                            {
                                Port = _CreateParam.Port;
                                PhysicalPath = _CreateParam.PhysicalPath;
                                PoolName = _CreateParam.PoolName;
                                PoolAlwaysRunning = _CreateParam.PoolAlwaysRunning;
                                this.nlog_iis.Info($"Website Create Port:{Port},PoolName:{PoolName},PhysicalPath:{PhysicalPath},PoolAlwaysRunning:{PoolAlwaysRunning}");
                            }
                        }
                        else if (!checkIisResult.Data.Level2Exist)
                        {
                            if (this.PluginConfig.IISEnableIncrement)
                            {
                                //网站还不存在不能选择指定的文件发布 
                                this.nlog_iis.Error($"Website Is Not Exist In Remote IIS,Can not use [Increment deplpoy]");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            this.BeginInvokeLambda(() =>
                            {
                                //级别二不存在
                                FirstCreate creatFrom = new FirstCreate(false);
                                var data = creatFrom.ShowDialog();
                                if (data == DialogResult.Cancel)
                                {
                                    _CreateParam = null;
                                }
                                else
                                {
                                    _CreateParam = creatFrom.IsCreateParam;
                                }
                                Condition.Set();
                            });
                            Condition.WaitOne();
                            if (_CreateParam == null)
                            {
                                this.nlog_iis.Error($"Create Website Param Required!");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            else
                            {
                                Port = _CreateParam.Port;
                                PhysicalPath = _CreateParam.PhysicalPath;
                                PoolName = _CreateParam.PoolName;
                                PoolAlwaysRunning = _CreateParam.PoolAlwaysRunning;
                                this.nlog_iis.Info($"Website Create Port:{Port},PoolName:{PoolName},PhysicalPath:{PhysicalPath},PoolAlwaysRunning:{PoolAlwaysRunning}");
                            }
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
                        httpRequestClient.SetFieldValue("poolAlwaysRunning", PoolAlwaysRunning ? "true" : "");
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("remark", confirmResult.Item2);
                        httpRequestClient.SetFieldValue("useTempPhysicalPath", PluginConfig.IISEnableNotStopSiteDeploy ? "true" : "");
                        httpRequestClient.SetFieldValue("useOfflineHtm", PluginConfig.IISEnableUseOfflineHtm ? "true" : "");
                        httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                        httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                        httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        IDisposable _subcribe = null;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_iis, HttpLogger);
                        var haveError = false;
                        try
                        {
                            if (stop_iis_cancel_token)
                            {
                                this.nlog_iis.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_progress, server.Host);
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                return;
                            }

                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");

                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_iis);
                                    _subcribe = System.Reactive.Linq.Observable
                                        .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                        .Sample(TimeSpan.FromMilliseconds(100))
                                        .Subscribe(arg => { ClientOnUploadProgressChanged(arg.Sender, arg.EventArgs); });
                                    //client.UploadProgressChanged += ClientOnUploadProgressChanged;
                                });

                            if (ProgressPercentage == 0 && !uploadResult.Item1) UploadError(this.tabPage_progress, server.Host);
                            if ((ProgressPercentage > 0 && ProgressPercentage < 100))
                                UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UploadError(this.tabPage_progress, server.Host);
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

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
                                            failServerList.Add(server);
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
                                    failServerList.Add(server);
                                    this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UploadError(this.tabPage_progress, server.Host);
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            this.nlog_iis.Error($"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            if (stop_iis_cancel_token)
                            {
                                this.nlog_iis.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_progress, server.Host);
                                return;
                            }
                        }
                        finally
                        {
                            await webSocket?.Dispose();

                            _subcribe?.Dispose();
                        }

                    }



                    //交互
                    if (allSuccess)
                    {
                        this.nlog_iis.Info("Deploy Version：" + dateTimeFolderNameParent);
                        if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                        allfailServerList = new List<Server>();

                        Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                    }
                    else
                    {
                        Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");

                        if (!stop_iis_cancel_token)
                        {
                            allfailServerList = new List<Server>();
                            allfailServerList.AddRange(failServerList);
                            EnableIIsRetry(true);
                            //看是否要重试
                            Condition.WaitOne();
                            if (!stop_iis_cancel_token)
                            {
                                retryTimes++;
                                goto RETRY_IIS;
                            }
                        }
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
                    if (!isRuningSelectDeploy)
                    {
                        //记录发布日志
                        SaveLog(publishPath, dateTimeFolderNameParent, nlog_iis);

                        ProgressPercentage = 0;
                        ProgressCurrentHost = null;
                        Enable(true);
                        gitModel?.Dispose();
                    }

                }


            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();

        }

        private void SaveLog(string publishPath, string dateTimeFolderNameParent, Logger nlog)
        {
            try
            {
                if (!GlobalConfig.SaveLogs) return;

                if (string.IsNullOrEmpty(publishPath) || string.IsNullOrEmpty(dateTimeFolderNameParent) || !Directory.Exists(publishPath))
                {
                    return;
                }

                var folder = new DirectoryInfo(publishPath);

                var logFolder = Path.Combine(folder.Parent.FullName, folder.Name + "_deploy_logs");

                if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
                LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "【Deploy log】 ==> ");
                var currentLogPath = Path.Combine(logFolder, dateTimeFolderNameParent + ".log");
                if (nlog == nlog_iis)
                {
                    this.BeginInvokeLambda(() =>
                    {
                        rich_iis_log.SaveFile(currentLogPath, RichTextBoxStreamType.PlainText);
                    });
                    publisEvent2.LoggerName = "rich_iis_log";
                }
                else if (nlog == nlog_image)
                {
                    this.BeginInvokeLambda(() =>
                    {
                        rich_docker_image_log.SaveFile(currentLogPath, RichTextBoxStreamType.PlainText);
                    });
                    publisEvent2.LoggerName = "rich_docker_image_log";
                }
                else if (nlog == nlog_docker)
                {
                    this.BeginInvokeLambda(() =>
                    {
                        rich_docker_log.SaveFile(currentLogPath, RichTextBoxStreamType.PlainText);
                    });
                    publisEvent2.LoggerName = "rich_docker_log";
                }
                else if (nlog == nlog_windowservice)
                {
                    this.BeginInvokeLambda(() =>
                    {
                        rich_windowservice_log.SaveFile(currentLogPath, RichTextBoxStreamType.PlainText);
                    });
                    publisEvent2.LoggerName = "rich_windowservice_log";
                }
                else if (nlog == nlog_linux)
                {
                    this.BeginInvokeLambda(() =>
                    {
                        rich_linuxservice_log.SaveFile(currentLogPath, RichTextBoxStreamType.PlainText);
                    });
                    publisEvent2.LoggerName = "rich_linuxservice_log";
                }
                publisEvent2.Properties["ShowLink"] = "file://" + currentLogPath.Replace("\\", "\\\\");
                nlog.Log(publisEvent2);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void DoSelectDeployIIS(List<string> fileList, string publishPath, List<Server> serverList, List<string> backUpIgnoreList, 
            string Port, string PoolName, string PhysicalPath, bool alwaysRun, GitClient gitModel, string remark, List<string> ignoreList)
        {
            try
            {
                new Task(async () =>
                {
                    var dateTimeFolderNameParent = string.Empty;
                    try
                    {
                        if (stop_iis_cancel_token)
                        {
                            this.nlog_iis.Warn($"deploy task was canceled!");
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }
                        if (fileList == null || !fileList.Any())
                        {
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            this.nlog_iis.Error("Please Select Files");
                            return;
                        }
                        this.nlog_iis.Info("Select Files count:" + fileList.Count);
                        //this.nlog_iis.Debug("ignore package ignoreList");
                        this.nlog_iis.Info($"package ignoreList Count:{ignoreList.Count}, backUp IgnoreList Count:{backUpIgnoreList.Count}");
                        byte[] zipBytes = null;
                        //List<string> ignoreList = new List<string>();
                        try
                        {
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_progress, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_iis_cancel_token;
                                }, true);
                        }
                        catch (Exception ex)
                        {
                            this.nlog_iis.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }

                        if (zipBytes == null || zipBytes.Length < 1)
                        {
                            this.nlog_iis.Error("package fail");
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }
                        var packageSize = (zipBytes.Length / 1024 / 1024);
                        this.nlog_iis.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                        var loggerId = Guid.NewGuid().ToString("N");
                        //执行 上传
                        this.nlog_iis.Info("-----------------Deploy Start-----------------");
                        dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var allfailServerList = new List<Server>();
                        var retryTimes = 0;
RETRY_IIS2:
                        if (stop_iis_cancel_token)
                        {
                            this.nlog_iis.Warn($"deploy task was canceled!");
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }
                        var failServerList = new List<Server>();
                        var index = 0;
                        var allSuccess = true;
                        var failCount = 0;
                        var dateTimeFolderName = string.Empty;
                        var isRetry = allfailServerList.Count > 0;
                        if (isRetry)
                        {
                            dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                        }
                        else
                        {
                            dateTimeFolderName = dateTimeFolderNameParent;
                        }
                        //重试了 但是没有发现错误的Server List
                        if (retryTimes > 0 && allfailServerList.Count == 0) return;
                        foreach (var server in isRetry ? allfailServerList : serverList)
                        {
                            if (isRetry) UploadReset(this.tabPage_progress, server.Host);

                            if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                            {
                                BuildEnd(this.tabPage_progress, server.Host);
                                UpdatePackageProgress(this.tabPage_progress, server.Host, 100);
                            }
                            if (stop_iis_cancel_token)
                            {
                                this.nlog_iis.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_progress, server.Host);
                                return;
                            }
                            index++;
                            if (string.IsNullOrEmpty(server.Token))
                            {
                                this.nlog_iis.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }



                            this.nlog_iis.Info("Start Check Website IsExist In Remote IIS:" + server.Host);
                            var checkIisResult = await WebUtil.HttpPostAsync<IIsSiteCheck>(
                                $"http://{server.Host}/version", new
                                {
                                    Token = server.Token,
                                    Type = "checkiis",
                                    Mac = CodingHelper.GetMacAddress(),
                                    Name = DeployConfig.IIsConfig.WebSiteName
                                }, nlog_iis);

                            if (checkIisResult == null || checkIisResult.Data == null)
                            {
                                this.nlog_iis.Error($"Check Website IsExist In Remote IIS Fail!");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            if (!string.IsNullOrEmpty(checkIisResult.Msg))
                            {
                                this.nlog_iis.Error($"Check Website IsExist In Remote IIS Fail：" + checkIisResult.Msg);
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            if (checkIisResult.Data.Success)
                            {
                                this.nlog_iis.Info($"Website Is Exist In Remote IIS:" + server.Host);
                            }
                            else if (!checkIisResult.Data.Level1Exist || !checkIisResult.Data.Level2Exist)
                            {
                                //网站还不存在不能选择指定的文件发布 
                                this.nlog_iis.Error($"Website Is Not Exist In Remote IIS,Can not use [select file deplpoy]");
                                UploadError(this.tabPage_progress, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
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
                            httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                            httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                            httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                            httpRequestClient.SetFieldValue("poolName", PoolName);
                            httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                            httpRequestClient.SetFieldValue("poolAlwaysRunning", alwaysRun ? "true" : "");
                            httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                            httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                            httpRequestClient.SetFieldValue("useTempPhysicalPath", PluginConfig.IISEnableNotStopSiteDeploy ? "true" : "");
                            httpRequestClient.SetFieldValue("useOfflineHtm", PluginConfig.IISEnableUseOfflineHtm ? "true" : "");
                            httpRequestClient.SetFieldValue("Token", server.Token);
                            httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                            httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                            HttpLogger HttpLogger = new HttpLogger
                            {
                                Key = loggerId,
                                Url = $"http://{server.Host}/logger?key=" + loggerId
                            };
                            IDisposable _subcribe = null;
                            WebSocketClient webSocket = new WebSocketClient(this.nlog_iis, HttpLogger);
                            var haveError = false;
                            try
                            {
                                if (stop_iis_cancel_token)
                                {
                                    this.nlog_iis.Warn($"deploy task was canceled!");
                                    UploadError(this.tabPage_progress, server.Host);
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                    return;
                                }

                                var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");

                                httpRequestClient.SetFieldValue("wsKey", hostKey);

                                var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                    (client) =>
                                    {
                                        client.Proxy = GetProxy(this.nlog_iis);
                                        _subcribe = System.Reactive.Linq.Observable
                                            .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                            .Sample(TimeSpan.FromMilliseconds(100))
                                            .Subscribe(arg => { ClientOnUploadProgressChanged(arg.Sender, arg.EventArgs); });
                                        //client.UploadProgressChanged += ClientOnUploadProgressChanged;
                                    });

                                if (ProgressPercentage == 0 && !uploadResult.Item1) UploadError(this.tabPage_progress, server.Host);
                                if ((ProgressPercentage > 0 && ProgressPercentage < 100))
                                    UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

                                webSocket.ReceiveHttpAction(true);
                                haveError = webSocket.HasError;
                                if (haveError)
                                {
                                    allSuccess = false;
                                    failCount++;
                                    failServerList.Add(server);
                                    this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                    UploadError(this.tabPage_progress, server.Host);
                                    UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                }
                                else
                                {
                                    if (uploadResult.Item1)
                                    {
                                        UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, 100); //结束上传

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
                                                failServerList.Add(server);
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
                                        failServerList.Add(server);
                                        this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                        UploadError(this.tabPage_progress, server.Host);
                                        UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_iis.Error($"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                                UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            }
                            finally
                            {
                                await webSocket?.Dispose();
                                _subcribe?.Dispose();
                            }

                        }

                        if (stop_iis_cancel_token)
                        {
                            this.nlog_iis.Warn($"deploy task was canceled!");
                            PackageError(this.tabPage_progress, serverList.First().Host);
                            return;
                        }

                        //交互
                        if (allSuccess)
                        {
                            this.nlog_iis.Info("Deploy Version：" + dateTimeFolderNameParent);
                            if (gitModel != null) gitModel.SubmitSelectedChanges(fileList, publishPath);
                            allfailServerList = new List<Server>();
                            Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                        }
                        else
                        {
                            Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                            if (!stop_iis_cancel_token)
                            {
                                allfailServerList = new List<Server>();
                                allfailServerList.AddRange(failServerList);
                                EnableIIsRetry(true);
                                //看是否要重试
                                Condition.WaitOne();
                                if (!stop_iis_cancel_token)
                                {
                                    retryTimes++;
                                    goto RETRY_IIS2;
                                }
                            }
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
                        //记录发布日志
                        SaveLog(publishPath, dateTimeFolderNameParent, nlog_iis);
                        ProgressPercentage = 0;
                        ProgressCurrentHost = null;
                        Enable(true);
                        gitModel?.Dispose();
                    }


                }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
            }
            catch (Exception)
            {
                ProgressPercentage = 0;
                ProgressCurrentHost = null;
                Enable(true);
                gitModel?.Dispose();
            }

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
                MessageBoxEx.Show(this, Strings.NotWebProject);
                return;
            }

            var websiteName = this.txt_iis_web_site_name.Text.Trim();
            if (websiteName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.WebSiteNameRequired);
                return;
            }


            var envName = this.combo_iis_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.IIS))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBoxEx.Show(this,
                Strings.DeployRollBackConfirm + Environment.NewLine + serverHostList,
                Strings.RollBackConfirm,
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_iis_env_SelectedIndexChanged(null, null);

            this.rich_iis_log.Text = "";
            DeployConfig.IIsConfig.WebSiteName = websiteName;
            //this.tab_iis.SelectedIndex = 1;

            PrintCommonLog(this.nlog_iis);

            new Task(async () =>
            {
                try
                {
                    Enable(false, true);


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
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} Rollback skip,Token is null or empty!");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        if (string.IsNullOrEmpty(server.Host))
                        {
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} Rollback fail,Server Host is Empty!");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        this.nlog_iis.Info($"Host:{getHostDisplayName(server)} Start get rollBack version list");

                        var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "iis",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = DeployConfig.IIsConfig.WebSiteName,
                                WithArgs = true
                            }, nlog_iis);

                        if (getVersionResult == null)
                        {
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(getVersionResult.Msg))
                        {
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail：" + getVersionResult.Msg);
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        var versionList = getVersionResult.Data;
                        if (versionList == null || versionList.Count <= 1)
                        {
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} get rollBack version list count:0");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        this.nlog_iis.Info($"Host:{getHostDisplayName(server)} get rollBack version list count:{versionList.Count}");

                        this.BeginInvokeLambda(() =>
                        {
                            RollBack rolleback = new RollBack(versionList.ToList());
                            rolleback.SetTitle($"Current Server:{getHostDisplayName(server)}");
                            var r = rolleback.ShowDialog();
                            if (r == DialogResult.Cancel)
                            {
                                _rollBackVersion = null;
                            }
                            else
                            {
                                _rollBackVersion = new RollBackVersion
                                {
                                    Version = rolleback.SelectRollBackVersion
                                };
                            }
                            Condition.Set();
                        });

                        Condition.WaitOne();

                        if (_rollBackVersion == null || string.IsNullOrEmpty(_rollBackVersion.Version))
                        {
                            this.nlog_iis.Error($"Host:{getHostDisplayName(server)} Rollback canceled!");
                            UpdateDeployProgress(this.tabPage_progress, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        this.nlog_iis.Info($"Host:{getHostDisplayName(server)} Start rollBack from version:" + _rollBackVersion.Version);

                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "iis_rollback");
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                        httpRequestClient.SetFieldValue("deployFolderName", _rollBackVersion.Version);
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
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_iis);
                                });
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
                                    this.nlog_iis.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
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

                    this.nlog_iis.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    Notice("Rollback End", $"[Total]:{serverList.Count},[Fail]:{failCount}");
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



            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();

        }




        private void ClientOnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            var showValue = 0;
            if (e.ProgressPercentage > ProgressPercentage && e.ProgressPercentage != 100)
            {
                ProgressPercentage = e.ProgressPercentage;
                showValue = (e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage);
                if (!string.IsNullOrEmpty(ProgressCurrentHost))
                {
                    UpdateUploadProgress(this.tabPage_progress, ProgressCurrentHost, showValue);
                }
                this.nlog_iis.Info($"Upload {showValue} % complete...");

            }


            if (showValue > 95)
            {
                //如果上传进度已经到了100 了 在取消已经没有意义了
                return;
            }

            if (stop_iis_cancel_token)
            {
                try
                {
                    var client = sender as WebClient;
                    if (client != null)
                    {
                        client.CancelAsync();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

        }
        private void EnableDockerRetry(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                btn_docker_retry.Visible = flag;//隐藏发布按钮

            });
        }
        private void EnableIIsRetry(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                btn_iis_retry.Visible = flag;//隐藏发布按钮

            });
        }
        private void EnableWindowsServiceRetry(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                btn_windows_service_retry.Visible = flag;//隐藏发布按钮

            });
        }
        private void EnableLinuxServiceRetry(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {

                btn_linux_service_retry.Visible = flag;//隐藏发布按钮

            });
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
                    }
                }

                this.b_iis_rollback.Enabled = flag;
                this.b_iis_deploy.Enabled = flag;
                if (!ignore)
                {
                    b_iis_deploy.Visible = flag;//隐藏发布按钮
                    btn_iis_stop.Visible = !flag;//是否展示停止按钮
                }

                this.checkBox_Increment_iis.Enabled = flag;
                this.checkBox_iis_use_offlinehtm.Enabled = flag;
                this.txt_iis_web_site_name.Enabled = flag;
                this.checkBox_iis_restart_site.Enabled = flag;
                this.combo_iis_env.Enabled = flag;
                this.combo_iis_sdk_type.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_linux_service.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                this.page_docker_img.Enabled = flag;
                this.checkBox_select_deploy_iis.Enabled = flag;
                if (flag)
                {
                    this.rich_windowservice_log.Text = "";
                    this.rich_linuxservice_log.Text = "";
                    this.rich_docker_log.Text = "";
                    btn_iis_stop.Enabled = true;
                    btn_iis_stop.Text = "Stop";
                }
                else
                {
                    tabcontrol.Tag = "0";
                    if (ignore) return;
                    if (this.tabPage_progress.Tag is Dictionary<string, ProgressBox> progressBoxList)
                    {
                        if (ProgressBox.IsEnableGroup)
                        {
                            foreach (var box in progressBoxList)
                            {
                                if (box.Value.CheckBox.Visible && box.Value.CheckBox.Checked)
                                {
                                    box.Value.StartBuild();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var box in progressBoxList)
                            {
                                box.Value.StartBuild();
                                break;
                            }
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

        private void UploadReset(TabPage tabPage, string host = null)
        {
            this.BeginInvokeLambda(() =>
            {
                if (tabPage.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        if (host == null)
                        {
                            box.Value.UploadReset();
                            break;
                        }

                        if (box.Key.Equals(host))
                        {
                            box.Value.UploadReset();
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
                if (tabPage != tabPage_docker && value >= 100)
                {
                    //针对iis 和 windows服务发布 上传超过100的话  隐藏 Stop
                    this.btn_iis_stop.Visible = false;
                    this.btn_iis_stop.Tag = "false";
                    this.btn_windows_serivce_stop.Visible = false;
                    this.btn_windows_serivce_stop.Tag = "false";
                    this.btn_linux_serivce_stop.Visible = false;
                    this.btn_linux_serivce_stop.Tag = "false";
                    stop_iis_cancel_token = false;
                    stop_windows_cancel_token = false;
                    stop_linux_cancel_token = false;
                }

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


                    if (tabPage == this.tabPage_progress)
                    {
                        if (progressBoxList.Count == 1)
                        {
                            if (!this.btn_iis_stop.Visible) this.btn_iis_stop.Visible = true;
                            return;
                        }
                        //针对iis 和 windows服务发布 上传超过100的话  会隐藏 Stop
                        //在结束第四个圈圈的时候在释放
                        var flag = this.btn_iis_stop.Tag as string;
                        if (!string.IsNullOrEmpty(flag) && flag == "false")
                        {
                            this.btn_iis_stop.Tag = null;
                            this.btn_iis_stop.Visible = true;
                        }
                    }
                    else if (tabPage == this.tabPage_windows_service)
                    {
                        if (progressBoxList.Count == 1)
                        {

                            if (!this.btn_windows_serivce_stop.Visible) this.btn_windows_serivce_stop.Visible = true;
                            return;
                        }
                        var flag = this.btn_windows_serivce_stop.Tag as string;
                        if (!string.IsNullOrEmpty(flag) && flag == "false")
                        {
                            this.btn_windows_serivce_stop.Tag = null;
                            this.btn_windows_serivce_stop.Visible = true;
                        }
                    }
                    else if (tabPage == this.tabPage_linux_service)
                    {
                        if (progressBoxList.Count == 1)
                        {

                            if (!this.btn_linux_serivce_stop.Visible) this.btn_linux_serivce_stop.Visible = true;
                            return;
                        }
                        var flag = this.btn_linux_serivce_stop.Tag as string;
                        if (!string.IsNullOrEmpty(flag) && flag == "false")
                        {
                            this.btn_linux_serivce_stop.Tag = null;
                            this.btn_linux_serivce_stop.Visible = true;
                        }
                    }
                }
            });
        }
        private void checkBox_iis_restart_site_Click(object sender, EventArgs e)
        {
            PluginConfig.IISEnableNotStopSiteDeploy = this.checkBox_iis_restart_site.Checked;
        }
        private void checkBox_Increment_iis_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.IISEnableIncrement = checkBox_Increment_iis.Checked;
        }
        private void checkBox_selectDeplot_iis_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.IISEnableSelectDeploy = checkBox_select_deploy_iis.Checked;
        }
        private void checkBox_iis_use_offlinehtm_Click(object sender, EventArgs e)
        {
            PluginConfig.IISEnableUseOfflineHtm = checkBox_iis_use_offlinehtm.Checked;
        }
        private void checkBox_Increment_docker_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.DockerEnableIncrement = checkBox_Increment_docker.Checked;
        }

        private void checkBox_sudo_docker_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.DockerEnableSudo = checkBox_sudo_docker.Checked;
        }
        private void checkBox_selectDeplot_docker_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.DockerServiceEnableSelectDeploy = checkBox_select_deploy_docker.Checked;
        }

        private void checkBoxdocker_rep_enable_Click(object sender, EventArgs e)
        {
            PluginConfig.DockerServiceEnableUpload = checkBoxdocker_rep_enable.Checked;
        }
        private void checkBoxdocker_rep_uploadOnly_Click(object sender, EventArgs e)
        {
            PluginConfig.DockerServiceBuildImageOnly = checkBoxdocker_rep_uploadOnly.Checked;
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

        private void combo_linux_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_linux_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.LinuxServiveConfig.LastEnvName = selectName;

                //设置对应的websitename
                if (DeployConfig.LinuxServiveConfig.EnvPairList != null && DeployConfig.LinuxServiveConfig.EnvPairList.Any())
                {
                    var target = DeployConfig.LinuxServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (target != null && !string.IsNullOrEmpty(target.ConfigName))
                    {
                        this.txt_linuxservice_name.Text = target.ConfigName;
                    }

                    if (target != null && !string.IsNullOrEmpty(target.LinuxEnvParam))
                    {
                        this.txt_linux_service_env.Text = target.LinuxEnvParam;
                    }
                }
                var dic = new Dictionary<string, bool>();
                //生成进度
                if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        dic.Add(box.Value.Server.Host, box.Value.CheckBox.Checked);
                        box.Value.Dispose();
                        this.tabPage_linux_service.Controls.Remove(box.Value);
                    }
                    this.tabPage_linux_service.Tag = null;
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
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.LINUXSERVICE, HostoryButtonSearch)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    if (dic.TryGetValue(serverHost, out var chec))
                    {
                        newBox.CheckBox.Checked = chec;
                    }

                    newBoxList.Add(serverHost, newBox);
                    this.tabPage_linux_service.Controls.Add(newBox);
                }

                this.progress_linux_service_tip.SendToBack();
                this.tabPage_linux_service.Tag = newBoxList;
            }
            else
            {
                if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        box.Value.Dispose();
                        this.tabPage_linux_service.Controls.Remove(box.Value);
                    }
                }
            }
        }
        private void combo_windowservice_env_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectName = this.combo_windowservice_env.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectName))
            {
                DeployConfig.WindowsServiveConfig.LastEnvName = selectName;

                //设置对应的websitename
                if (DeployConfig.WindowsServiveConfig.EnvPairList != null && DeployConfig.WindowsServiveConfig.EnvPairList.Any())
                {
                    var target = DeployConfig.WindowsServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (target != null && !string.IsNullOrEmpty(target.ConfigName))
                    {
                        this.txt_windowservice_name.Text = target.ConfigName;
                    }
                }
                var dic = new Dictionary<string, bool>();
                //生成进度
                if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        dic.Add(box.Value.Server.Host, box.Value.CheckBox.Checked);
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
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.WINSERVICE, HostoryButtonSearch)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    if (dic.TryGetValue(serverHost, out var chec))
                    {
                        newBox.CheckBox.Checked = chec;
                    }

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

            if (ProgressPercentageForWindowsService > 95)
            {
                return;
            }

            if (stop_windows_cancel_token)
            {
                try
                {
                    var client = sender as WebClient;
                    if (client != null)
                    {

                        client.CancelAsync();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }
        }

        private void ClientOnUploadProgressChanged3(object sender, UploadProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage > ProgressPercentageForLinuxService && e.ProgressPercentage != 100)
            {
                ProgressPercentageForLinuxService = e.ProgressPercentage;
                var showValue = (e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage);
                if (!string.IsNullOrEmpty(ProgressCurrentHostForLinuxService))
                    UpdateUploadProgress(this.tabPage_linux_service, ProgressCurrentHostForLinuxService, showValue);
                this.nlog_linux.Info($"Upload {showValue} % complete...");
            }

            if (ProgressPercentageForLinuxService > 95)
            {
                return;
            }

            if (stop_linux_cancel_token)
            {
                try
                {
                    var client = sender as WebClient;
                    if (client != null)
                    {

                        client.CancelAsync();
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
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
                    }
                }
                this.b_windows_service_rollback.Enabled = flag;
                this.checkBox_Increment_window_service.Enabled = flag;
                this.b_windowservice_deploy.Enabled = flag;
                if (!ignore)
                {
                    this.b_windowservice_deploy.Visible = flag;
                    btn_windows_serivce_stop.Visible = !flag;//是否展示停止按钮
                }
                this.combo_windowservice_env.Enabled = flag;
                this.combo_windowservice_sdk_type.Enabled = flag;
                this.txt_windowservice_name.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_linux_service.Enabled = flag;
                this.page_web_iis.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                this.page_docker_img.Enabled = flag;
                checkBox_select_deploy_service.Enabled = flag;
                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_docker_log.Text = "";
                    this.rich_linuxservice_log.Text = "";
                    btn_windows_serivce_stop.Enabled = true;
                    btn_windows_serivce_stop.Text = "Stop";
                }
                else
                {
                    tabcontrol.Tag = "2";
                    if (ignore) return;
                    if (this.tabPage_windows_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                    {
                        if (ProgressBox.IsEnableGroup)
                        {
                            foreach (var box in progressBoxList)
                            {
                                if (box.Value.CheckBox.Visible && box.Value.CheckBox.Checked)
                                {
                                    box.Value.StartBuild();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var box in progressBoxList)
                            {
                                box.Value.StartBuild();
                                break;
                            }
                        }
                    }
                }
            });

        }


        private void EnableForLinuxService(bool flag, bool ignore = false)
        {
            this.BeginInvokeLambda(() =>
            {
                if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList1)
                {
                    foreach (var box in progressBoxList1)
                    {
                        box.Value.Enable(flag);
                    }
                }
                this.b_linux_service_rollback.Enabled = flag;
                this.checkBox_Increment_linux_service.Enabled = flag;
                this.b_linuxservice_deploy.Enabled = flag;
                if (!ignore)
                {
                    this.b_linuxservice_deploy.Visible = flag;
                    btn_linux_serivce_stop.Visible = !flag;//是否展示停止按钮
                }
                this.combo_linux_env.Enabled = flag;
                this.txt_linuxservice_name.Enabled = flag;
                this.txt_linux_service_env.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_web_iis.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;
                this.page_docker_img.Enabled = flag;
                checkBox_select_deploy_linuxservice.Enabled = flag;
                checkBox_select_type_linuxservice.Enabled = flag;
                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_docker_log.Text = "";
                    btn_linux_serivce_stop.Enabled = true;
                    btn_linux_serivce_stop.Text = "Stop";
                }
                else
                {
                    tabcontrol.Tag = "3";
                    if (ignore) return;
                    if (this.tabPage_linux_service.Tag is Dictionary<string, ProgressBox> progressBoxList)
                    {
                        if (ProgressBox.IsEnableGroup)
                        {
                            foreach (var box in progressBoxList)
                            {
                                if (box.Value.CheckBox.Visible && box.Value.CheckBox.Checked)
                                {
                                    box.Value.StartBuild();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var box in progressBoxList)
                            {
                                box.Value.StartBuild();
                                break;
                            }
                        }
                    }
                }
            });

        }
        private void b_windowservice_deploy_Click(object sender, EventArgs e)
        {
            stop_windows_cancel_token = false;
            Condition = new AutoResetEvent(false);
            

            var sdkTypeName = this.combo_windowservice_sdk_type.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(sdkTypeName))
            {
                MessageBoxEx.Show(this, Strings.SelectSdkType);
                return;
            }

            if (sdkTypeName.Equals("netcore"))
            {
                // 针对netcore类型的 不管是workerservice模板还是webapi模板只要引用服务组件都可以部署成windows服务
                if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && !_project.IsNetcorePorject)
                {
                    MessageBoxEx.Show(this, Strings.NowNetcoreProject);
                    return;
                }

                //检查一下 如果是netcore的话 确认是否一定要runtime部署
                if (!string.IsNullOrEmpty(PluginConfig.NetCorePublishMode) && PluginConfig.NetCorePublishMode.Contains("runtime"))
                {
                    var result = MessageBoxEx.Show(this, Strings.NetcoreProjectPublishModeConfirm, Strings.ConfirmContinue, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }
            else
            {
                // 针对netframework类型的限制
                if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && _project.IsWebProejct)
                {
                    MessageBoxEx.Show(this, Strings.NotServiceProject);
                    return;
                }

                //检查工程文件里面是否含有 WebProjectProperties字样
                if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && ProjectHelper.IsWebProject(ProjectPath))
                {
                    MessageBoxEx.Show(this, Strings.NotServiceProject);
                    return;
                }
            }

            var serviceName = this.txt_windowservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.ServiceNameRequired);
                return;
            }

            DeployConfig.WindowsServiveConfig.ServiceName = serviceName;
            var isAgentUpdate = DeployConfig.WindowsServiveConfig.ServiceName.ToLower().Equals("antdeployagentwindowsservice");
            var PhysicalPath = "";
            var ServiceStartType = "";
            var ServiceDescription = "";

            var envName = this.combo_windowservice_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.WINSERVICE))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }


            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;
            var backUpIgnoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).WindowsBackUpIgnoreList;

#if DEBUG

            var execFilePath = (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? serviceName : this.ProjectName) + ".exe";
#else
            //如果是特定文件夹发布 得选择一个exe
            if (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && string.IsNullOrEmpty(_project.OutPutName))
            {
                var exePath = string.Empty;
                if (isAgentUpdate)
                {
                    exePath = Path.Combine(PluginConfig.DeployFolderPath, "AntDeployAgentWindowsService.exe");
                    if (!File.Exists(exePath))
                    {
                        MessageBoxEx.Show(this,"AntDeployAgentWindowsService.exe not exist");
                        return;
                    }
                }
                else
                {
                    //找当前根目录下 有没有 .deps.json 文件
                    var depsJsonFile = CodingHelper.FindDepsJsonFile(PluginConfig.DeployFolderPath);
                    if (!string.IsNullOrEmpty(depsJsonFile))
                    {
                        var exeTempName = depsJsonFile.Replace(".deps.json", ".exe");
                        if (File.Exists(exeTempName))
                        {
                            exePath = exeTempName;
                        }
                    }

                    if (string.IsNullOrEmpty(exePath))
                    {
                        exePath = CodingHelper.GetWindowsServiceExe(PluginConfig.DeployFolderPath);
                        if (string.IsNullOrEmpty(exePath))
                        {
                            MessageBoxEx.Show(this,"please select exe path");
                            return;
                        }
                    }
                }

                _project.OutPutName = new FileInfo(exePath).Name;
            }
            var execFilePath = _project.OutPutName;
            if (string.IsNullOrEmpty(execFilePath))
            {
                //MessageBoxEx.Show(this,"get current project property:outputfilename error");
                execFilePath = this.ProjectName + ".exe";
            }

            if (!DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore") && !execFilePath.Trim().ToLower().EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                MessageBoxEx.Show(this,"current project out file name is not exe!");
                return;
            }


#endif
            var agentNewVersion = string.Empty;
            if (isAgentUpdate)
            {
                var agentDll = Path.Combine(PluginConfig.DeployFolderPath, "AntDeployAgent.dll");
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(agentDll);
                agentNewVersion = myFileVersionInfo.FileVersion;
            }



            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                return;
            }

            if (ProgressBox.IsEnableGroup)
            {
                //获取所有的选择了的server
                var selectedList = getSelectedBaseServers(ServerType.WINSERVICE);
                if (!selectedList.Any())
                {
                    MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                    return;
                }

                //找到选择了的
                serverList = serverList.Where(r => selectedList.Any(y => y.Host.Equals(r.Host))).ToList();
                if (!serverList.Any())
                {
                    MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                    return;
                }
            }
            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = ShowInputMsgBox(Strings.ConfirmDeploy,
                Strings.DeployServerConfim + Environment.NewLine + serverHostList);

            //var confirmResult = MessageBoxEx.Show(this,
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
            _CreateParam = new FirstCreateParam();
            new Task(async () =>
            {
                this.nlog_windowservice.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
                if (!string.IsNullOrEmpty(agentNewVersion))
                {
                    this.nlog_windowservice.Info($"-----------------Update Agent To [Ver:{agentNewVersion}]-----------------");
                }

                PrintCommonLog(this.nlog_windowservice);
                EnableForWindowsService(false); //第一台开始编译
                GitClient gitModel = null;
                var isRuningSelectDeploy = false;
                var isNetcore = DeployConfig.WindowsServiveConfig.SdkType.Equals("netcore");
                var publishPath = !string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? PluginConfig.DeployFolderPath : Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_winservice", envName);
                var dateTimeFolderNameParent = string.Empty;
                try
                {

                    if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                    {
                        var path = publishPath + "\\";
                        if (isNetcore)
                        {
                            var runtime = "";
                            if (string.IsNullOrEmpty(PluginConfig.NetCorePublishMode) || PluginConfig.NetCorePublishMode=="Default")
                            {
                                runtime = " --runtime win-x64";
                            }
                            else
                            {
                                runtime = PluginConfig.GetNetCorePublishRuntimeArg();
                            }
                            //执行 publish
                            var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                                $"publish \"{ProjectPath}\" -c Release{runtime}", nlog_windowservice, () => stop_windows_cancel_token);

                            if (!isSuccess)
                            {
                                this.nlog_windowservice.Error("publish error,please check build log");
                                BuildError(this.tabPage_windows_service, serverList.First().Host);
                                return;
                            }
                        }
                        else
                        {
                            //执行 publish
                            var isSuccess = CommandHelper.RunMsbuild(ProjectPath, path, nlog_windowservice, false, () => stop_windows_cancel_token);

                            if (!isSuccess)
                            {
                                this.nlog_windowservice.Error("publish error,please check build log");
                                BuildError(this.tabPage_windows_service, serverList.First().Host);
                                return;
                            }
                        }

                    }


                    if (string.IsNullOrEmpty(publishPath) || !Directory.Exists(publishPath))
                    {
                        this.nlog_windowservice.Error("can not find publishPath");
                        BuildError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }

                    var isProjectInstallService = false;
                    if (!isNetcore && string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                    {
                        //判断是否是windows PorjectInstall的服务
                        var serviceFile = Path.Combine(publishPath, execFilePath);
                        if (!File.Exists(serviceFile))
                        {
                            this.nlog_windowservice.Error($"exe file can not find in publish folder: {serviceFile}");
                            BuildError(this.tabPage_windows_service, serverList.First().Host);
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
                                this.nlog_windowservice.Warn($"windowsService name is {serviceNameByFile} in file: {serviceFile} ,but input name is {serviceName} !");
                                //BuildError(this.tabPage_windows_service);
                                //return;
                            }
                            isProjectInstallService = true;
                        }

                    }
                    else
                    {
                        execFilePath = execFilePath.Replace(".dll", ".exe");
                        var serviceFile = Path.Combine(publishPath, execFilePath);
                        if (!File.Exists(serviceFile))
                        {
                            BuildError(this.tabPage_windows_service, serverList.First().Host);
                            this.nlog_windowservice.Error($"exe file can not find in publish folder: {serviceFile}");
                            return;
                        }
                    }


                    BuildEnd(this.tabPage_windows_service, serverList.First().Host); //第一台结束编译
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent.LoggerName = "rich_windowservice_log";
                    this.nlog_windowservice.Log(publisEvent);



                    //执行 打包
                    this.nlog_windowservice.Info("-----------------Start package-----------------");

                    if (stop_windows_cancel_token)
                    {
                        this.nlog_windowservice.Warn($"deploy task was canceled!");
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }

                    //查看是否开启了增量
                    if (this.PluginConfig.WindowsServiceEnableIncrement && !isAgentUpdate)
                    {
                        this.nlog_windowservice.Info("Enable Increment Deploy:true");
                        gitModel = new GitClient(publishPath, this.nlog_windowservice);
                        if (!gitModel.InitSuccess)
                        {
                            this.nlog_windowservice.Error(
                                "package fail,can not init git,please cancel Increment Deploy");
                            PackageError(this.tabPage_windows_service, serverList.First().Host);
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
                            PackageError(this.tabPage_windows_service, serverList.First().Host);
                            return;
                        }
                        gitChangeFileCount = fileList.Count;
                        this.nlog_windowservice.Info("【git】Increment package file count:" + gitChangeFileCount);

                        if (PluginConfig.WindowsServiceEnableSelectDeploy)
                        {
                            isRuningSelectDeploy = true;
                            this.nlog_windowservice.Info("-----------------Select File Start-----------------");
                            this.BeginInvokeLambda(() =>
                            {
                                var slectFileForm = new SelectFile(fileList, publishPath, ignoreList);
                                slectFileForm.ShowDialog();
                                // ReSharper disable once AccessToDisposedClosure
                                DoWindowsServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList, serviceName, isProjectInstallService, execFilePath, PhysicalPath, backUpIgnoreList, gitModel, confirmResult.Item2, ignoreList);
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
                                    UpdatePackageProgress(this.tabPage_windows_service, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_windows_cancel_token;
                                });
                        }
                        catch (Exception ex)
                        {
                            this.nlog_windowservice.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_windows_service, serverList.First().Host);
                            return;
                        }



                    }
                    else if (PluginConfig.WindowsServiceEnableSelectDeploy && !isAgentUpdate)
                    {
                        isRuningSelectDeploy = true;
                        this.nlog_windowservice.Info("-----------------Select File Start-----------------");
                        this.BeginInvokeLambda(() =>
                        {
                            var slectFileForm = new SelectFile(publishPath, ignoreList);
                            slectFileForm.ShowDialog();
                            DoWindowsServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList, serviceName, isProjectInstallService, execFilePath, PhysicalPath, backUpIgnoreList, null, confirmResult.Item2, ignoreList);
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
                                    UpdatePackageProgress(this.tabPage_windows_service, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_windows_cancel_token;
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.nlog_windowservice.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_windowservice.Error("package fail");
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_windowservice.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_windowservice.Info("-----------------Deploy Start-----------------");
                    dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var retryTimes = 0;
                    var allfailServerList = new List<Server>();
RETRY_WINDOWSSERVICE:
                    var failServerList = new List<Server>();
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = string.Empty;
                    var isRetry = allfailServerList.Count > 0;
                    if (isRetry)
                    {
                        dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                    }
                    else
                    {
                        dateTimeFolderName = dateTimeFolderNameParent;
                    }

                    //重试了 但是没有发现错误的Server List
                    if (retryTimes > 0 && allfailServerList.Count == 0) return;
                    foreach (var server in isRetry ? allfailServerList : serverList)
                    {
                        if (isRetry) UploadReset(this.tabPage_windows_service, server.Host);
                        if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_windows_service, server.Host);
                            UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        }
                        if (stop_windows_cancel_token)
                        {
                            this.nlog_windowservice.Warn($"deploy task was canceled!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            return;
                        }
                        index++;
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }
                        var isOldAgent = false;
                        this.nlog_windowservice.Info("Start Check Windows Service IsExist In Remote Server:" + server.Host);
                        var checkResult = await WebUtil.HttpPostAsync<IIsSiteCheck>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "checkwinservice",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = serviceName
                            }, nlog_windowservice, true);


                        if (checkResult == null || checkResult.Data == null)
                        {
                            if (isAgentUpdate)
                            {
                                isOldAgent = true;
                            }
                            else
                            {
                                this.nlog_windowservice.Error($"Check IsExist In Remote Server Fail!");
                                UploadError(this.tabPage_windows_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                        }


                        if (!isOldAgent && !string.IsNullOrEmpty(checkResult.Msg))
                        {
                            this.nlog_windowservice.Error($"Check IsExist In Remote Server Fail：" + checkResult.Msg);
                            UploadError(this.tabPage_windows_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }

                        if (isOldAgent)
                        {
                            //兼容老的Agent版本
                            this.nlog_windowservice.Warn($"【Server】Agent version is old,please update agent version to:{Vsix.AGENTVERSION}！");
                        }
                        else if (checkResult.Data.Success)
                        {
                            this.nlog_windowservice.Info($"Windows Service Is Exist In Remote Server:" + server.Host);
                        }
                        else
                        {
                            if (this.PluginConfig.WindowsServiceEnableIncrement)
                            {
                                this.nlog_windowservice.Error($"Windows Service Is Not Exist In Remote Server,Can not use [Increment deplpoy]");
                                UploadError(this.tabPage_windows_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            else if (isAgentUpdate)
                            {
                                this.nlog_windowservice.Error($"Agent Service Is Not Exist In Remote Server,Can not update！");
                                UploadError(this.tabPage_windows_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            this.BeginInvokeLambda(() =>
                            {
                                //级别一不存在
                                FirstService creatFrom = new FirstService();
                                var data = creatFrom.ShowDialog();
                                if (data == DialogResult.Cancel)
                                {
                                    _CreateParam = null;
                                }
                                else
                                {
                                    _CreateParam = creatFrom.WindowsServiceCreateParam;
                                }
                                Condition.Set();
                            });
                            Condition.WaitOne();

                            if (_CreateParam == null)
                            {
                                this.nlog_windowservice.Error($"Create Windows Service Param Required!");
                                UploadError(this.tabPage_windows_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            else
                            {
                                ServiceStartType = _CreateParam.StartUp;
                                PhysicalPath = _CreateParam.PhysicalPath;
                                ServiceDescription = _CreateParam.Desc;
                                this.nlog_windowservice.Info($"WindowsService Create Description:{_CreateParam.Desc},StartType:{_CreateParam.StartUp},PhysicalPath:{PhysicalPath}");
                            }
                        }


                        ProgressPercentageForWindowsService = 0;
                        ProgressCurrentHostForWindowsService = server.Host;
                        this.nlog_windowservice.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "windowservice");
                        httpRequestClient.SetFieldValue("isIncrement", this.PluginConfig.WindowsServiceEnableIncrement && !isAgentUpdate ? "true" : "");
                        httpRequestClient.SetFieldValue("serviceName", serviceName);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("sdkType", DeployConfig.WindowsServiveConfig.SdkType);
                        httpRequestClient.SetFieldValue("isProjectInstallService",
                            isProjectInstallService ? "yes" : "no");
                        httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                        httpRequestClient.SetFieldValue("remark", confirmResult.Item2);
                        httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                        httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                        httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("startType", ServiceStartType);
                        httpRequestClient.SetFieldValue("desc", ServiceDescription);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("useNssm", _CreateParam != null ? _CreateParam.useNssm : "");
                        httpRequestClient.SetFieldValue("param", _CreateParam != null ? _CreateParam.Param : "");
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        IDisposable _subcribe = null;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_windowservice, HttpLogger);
                        var haveError = false;
                        try
                        {
                            if (stop_windows_cancel_token)
                            {
                                this.nlog_windowservice.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_windows_service, server.Host);
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                return;
                            }

                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_windowservice);
                                    _subcribe = System.Reactive.Linq.Observable
                                        .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                        .Sample(TimeSpan.FromMilliseconds(100))
                                        .Subscribe(arg => { ClientOnUploadProgressChanged2(arg.Sender, arg.EventArgs); });
                                    //client.UploadProgressChanged += ClientOnUploadProgressChanged2;
                                });
                            if (ProgressPercentageForWindowsService == 0 && !uploadResult.Item1) UploadError(this.tabPage_windows_service, server.Host);
                            if ((ProgressPercentageForWindowsService > 0 && ProgressPercentageForWindowsService < 100))
                                UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService, 100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError && !isAgentUpdate)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UploadError(this.tabPage_windows_service, server.Host);
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            }
                            else
                            {
                                if (isAgentUpdate)
                                {
                                    var checkAgentUrl = $"http://{server.Host}/version";
                                    LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Check AgetVersion,TimeOut：10senconds  ==> ");
                                    publisEvent22.Properties["ShowLink"] = checkAgentUrl;
                                    publisEvent22.LoggerName = "rich_windowservice_log";
                                    this.nlog_windowservice.Log(publisEvent22);

                                    var fireRt = WebUtil.IsHttpGetOk(checkAgentUrl, this.nlog_windowservice, agentNewVersion);
                                    if (fireRt)
                                    {
                                        UpdateDeployProgress(this.tabPage_windows_service, server.Host, true);
                                        this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)},Agent Update Success");
                                    }
                                    else
                                    {
                                        failCount++;
                                        failServerList.Add(server);
                                        allSuccess = false;
                                        UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                        this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Agent Update Error");
                                    }
                                }
                                else if (uploadResult.Item1)
                                {
                                    UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService, 100); //结束上传
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
                                            failServerList.Add(server);
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
                                    failServerList.Add(server);
                                    this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UploadError(this.tabPage_windows_service, server.Host);
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            failServerList.Add(server);
                            allSuccess = false;
                            this.nlog_windowservice.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            if (stop_windows_cancel_token)
                            {
                                this.nlog_windowservice.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_windows_service, server.Host);
                                return;
                            }
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                            _subcribe?.Dispose();
                        }

                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_windowservice.Info("Deploy Version：" + dateTimeFolderNameParent);
                        if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                        allfailServerList = new List<Server>();
                        Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");

                    }
                    else
                    {
                        Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                        if (!stop_windows_cancel_token)
                        {
                            allfailServerList = new List<Server>();
                            allfailServerList.AddRange(failServerList);
                            EnableWindowsServiceRetry(true);
                            //看是否要重试
                            Condition.WaitOne();
                            if (!stop_windows_cancel_token)
                            {
                                retryTimes++;
                                goto RETRY_WINDOWSSERVICE;
                            }
                        }

                    }

                    zipBytes = null;
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
                    //记录发布日志
                    SaveLog(publishPath, dateTimeFolderNameParent, nlog_windowservice);

                    if (!isRuningSelectDeploy)
                    {
                        ProgressPercentageForWindowsService = 0;
                        ProgressCurrentHostForWindowsService = null;
                        EnableForWindowsService(true);
                        gitModel?.Dispose();
                    }

                }



            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }


        private void DoWindowsServiceSelectDeploy(List<string> fileList, string publishPath, List<Server> serverList, string serviceName, bool isProjectInstallService, 
            string execFilePath, string PhysicalPath, List<string> backUpIgnoreList, GitClient gitModel, string remark, List<string> ignoreList)
        {
            new Task(async () =>
            {
                var dateTimeFolderNameParent = string.Empty;
                try
                {
                    if (stop_windows_cancel_token)
                    {
                        this.nlog_windowservice.Warn($"deploy task was canceled!");
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }

                    byte[] zipBytes = null;
                    if (fileList == null || !fileList.Any())
                    {
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        this.nlog_windowservice.Error("Please Select Files");
                        return;
                    }
                    this.nlog_windowservice.Info("Select Files count:" + fileList.Count);
                    //this.nlog_windowservice.Debug("ignore package ignoreList");
                    this.nlog_windowservice.Info($"package ignoreList Count:{ignoreList.Count}, backUp IgnoreList Count:{backUpIgnoreList.Count}");
                    //List<string> ignoreList = new List<string>();
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, true,
                            ignoreList,
                            (progressValue) =>
                            {
                                UpdatePackageProgress(this.tabPage_windows_service, serverList.First().Host, progressValue); //打印打包记录
                                return stop_windows_cancel_token;
                            }, true);
                    }
                    catch (Exception ex)
                    {
                        this.nlog_windowservice.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_windowservice.Error("package fail");
                        PackageError(this.tabPage_windows_service, serverList.First().Host);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_windowservice.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_windowservice.Info("-----------------Deploy Start-----------------");
                    dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var retryTimes = 0;
                    var allfailServerList = new List<Server>();
RETRY_WINDOWSSERVICE2:
                    var failServerList = new List<Server>();
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = string.Empty;
                    var isRetry = allfailServerList.Count > 0;
                    if (isRetry)
                    {
                        dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                    }
                    else
                    {
                        dateTimeFolderName = dateTimeFolderNameParent;
                    }
                    //重试了 但是没有发现错误的Server List
                    if (retryTimes > 0 && allfailServerList.Count == 0) return;
                    foreach (var server in isRetry ? allfailServerList : serverList)
                    {
                        if (isRetry) UploadReset(this.tabPage_windows_service, server.Host);
                        if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_windows_service, server.Host);
                            UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        }
                        if (stop_windows_cancel_token)
                        {
                            this.nlog_windowservice.Warn($"deploy task was canceled!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            return;
                        }
                        index++;
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_windows_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
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
                        httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                        httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                        httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        IDisposable _subcribe = null;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_windowservice, HttpLogger);
                        var haveError = false;
                        try
                        {
                            if (stop_windows_cancel_token)
                            {
                                this.nlog_windowservice.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_windows_service, server.Host);
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                return;
                            }
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_windowservice);
                                    _subcribe = System.Reactive.Linq.Observable
                                        .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                        .Sample(TimeSpan.FromMilliseconds(100))
                                        .Subscribe(arg => { ClientOnUploadProgressChanged2(arg.Sender, arg.EventArgs); });
                                    //client.UploadProgressChanged += ClientOnUploadProgressChanged2;
                                });
                            if (ProgressPercentageForWindowsService == 0 && !uploadResult.Item1) UploadError(this.tabPage_windows_service, server.Host);
                            if ((ProgressPercentageForWindowsService > 0 && ProgressPercentageForWindowsService < 100))
                                UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService, 100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UploadError(this.tabPage_windows_service, server.Host);
                                UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    UpdateUploadProgress(this.tabPage_windows_service, ProgressCurrentHostForWindowsService, 100); //结束上传

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
                                            failServerList.Add(server);
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
                                    failServerList.Add(server);
                                    this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UploadError(this.tabPage_windows_service, server.Host);
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            failServerList.Add(server);
                            allSuccess = false;
                            this.nlog_windowservice.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                            _subcribe?.Dispose();
                        }

                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_windowservice.Info("Deploy Version：" + dateTimeFolderNameParent);
                        if (gitModel != null) gitModel.SubmitSelectedChanges(fileList, publishPath);
                        allfailServerList = new List<Server>();
                        Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");

                    }
                    else
                    {
                        Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                        if (!stop_windows_cancel_token)
                        {
                            allfailServerList = new List<Server>();
                            allfailServerList.AddRange(failServerList);
                            EnableWindowsServiceRetry(true);
                            //看是否要重试
                            Condition.WaitOne();
                            if (!stop_windows_cancel_token)
                            {
                                retryTimes++;
                                goto RETRY_WINDOWSSERVICE2;
                            }
                        }

                    }

                    zipBytes = null;
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
                    //记录发布日志
                    SaveLog(publishPath, dateTimeFolderNameParent, nlog_windowservice);
                    ProgressPercentageForWindowsService = 0;
                    ProgressCurrentHostForWindowsService = null;
                    EnableForWindowsService(true);
                    gitModel?.Dispose();
                }
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }

        private void b_windows_service_rollback_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && _project.IsWebProejct)
            {
                MessageBoxEx.Show(this, Strings.NotServiceProject);
                return;
            }


            //检查工程文件里面是否含有 WebProjectProperties字样
            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && ProjectHelper.IsWebProject(ProjectPath))
            {
                MessageBoxEx.Show(this, Strings.NotServiceProject);
                return;
            }


            var serviceName = this.txt_windowservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.ServiceNameRequired);
                return;
            }

            DeployConfig.WindowsServiveConfig.ServiceName = serviceName;



            var envName = this.combo_windowservice_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.WINSERVICE))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBoxEx.Show(this,
                Strings.DeployRollBackConfirm + Environment.NewLine + serverHostList,
                Strings.RollBackConfirm,
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_windowservice_env_SelectedIndexChanged(null, null);

            this.rich_windowservice_log.Text = "";
            this.nlog_windowservice.Info($"windows Service name:{DeployConfig.WindowsServiveConfig.ServiceName}");
            //this.tabControl_window_service.SelectedIndex = 1;
            PrintCommonLog(this.nlog_windowservice);

            new Task(async () =>
            {
                try
                {
                    EnableForWindowsService(false, true);

                    var loggerId = Guid.NewGuid().ToString("N");

                    this.nlog_windowservice.Info($"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        BuildEnd(this.tabPage_windows_service, server.Host);
                        UpdatePackageProgress(this.tabPage_windows_service, server.Host, 100);
                        UpdateUploadProgress(this.tabPage_windows_service, server.Host, 100);


                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} Rollback skip,Token is null or empty!");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(server.Host))
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} Rollback fail,Server Host is Empty!");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)} Start get rollBack version list");


                        var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "winservice",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = DeployConfig.WindowsServiveConfig.ServiceName,
                                WithArgs = true
                            }, nlog_windowservice);

                        if (getVersionResult == null)
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(getVersionResult.Msg))
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail：" + getVersionResult.Msg);
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        var versionList = getVersionResult.Data;

                        if (versionList == null || versionList.Count <= 1)
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} get rollBack version list count:0");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }
                        this.nlog_windowservice.Info($"Host:{getHostDisplayName(server)} get rollBack version list count:{versionList.Count}");
                        this.BeginInvokeLambda(() =>
                        {
                            RollBack rolleback = new RollBack(versionList.ToList());
                            rolleback.SetTitle($"Current Server:{getHostDisplayName(server)}");
                            var r = rolleback.ShowDialog();
                            if (r == DialogResult.Cancel)
                            {
                                _rollBackVersion = null;
                            }
                            else
                            {
                                _rollBackVersion = new RollBackVersion
                                {
                                    Version = rolleback.SelectRollBackVersion
                                };
                            }
                            Condition.Set();
                        });
                        Condition.WaitOne();
                        if (_rollBackVersion == null || string.IsNullOrEmpty(_rollBackVersion.Version))
                        {
                            this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)} Rollback canceled!");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "windowservice_rollback");
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("serviceName", DeployConfig.WindowsServiveConfig.ServiceName);
                        httpRequestClient.SetFieldValue("deployFolderName", _rollBackVersion.Version);
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
                                (client) => { client.Proxy = GetProxy(this.nlog_windowservice); });
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
                                    this.nlog_windowservice.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            this.nlog_windowservice.Error($"Fail Rollback,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_windows_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }
                    }

                    this.nlog_windowservice.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    Notice("Rollback End", $"[Total]:{serverList.Count},[Fail]:{failCount}");
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

            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
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
            else if (tabcontrol.SelectedIndex == 3)
            {
                if (excutePageIndex != tabcontrol.SelectedIndex)
                {
                    this.rich_linuxservice_log.Text = "";
                }
            }
        }

        private void DeployConfigOnEnvChangeEvent(Env changeEnv, bool isServerChange, bool isRemove)
        {

            var item1 = this.combo_iis_env.SelectedItem as string;
            var item2 = this.combo_windowservice_env.SelectedItem as string;
            var item3 = this.combo_docker_env.SelectedItem as string;
            var item4 = this.combo_linux_env.SelectedItem as string;
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

                if (!string.IsNullOrEmpty(item4) && item4.Equals(changeEnv.Name))
                {
                    combo_linux_env_SelectedIndexChanged(null, null);
                }

                return;
            }
            else
            {
                #region 说明环境有新增或者减少

                if (isRemove)
                {
                    if (DeployConfig.IIsConfig != null && DeployConfig.IIsConfig.EnvPairList != null)
                    {
                        var toRemove = DeployConfig.IIsConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(changeEnv.Name));
                        if (toRemove != null) DeployConfig.IIsConfig.EnvPairList.Remove(toRemove);
                    }

                    if (DeployConfig.WindowsServiveConfig != null && DeployConfig.WindowsServiveConfig.EnvPairList != null)
                    {
                        var toRemove = DeployConfig.WindowsServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(changeEnv.Name));
                        if (toRemove != null) DeployConfig.WindowsServiveConfig.EnvPairList.Remove(toRemove);
                    }

                    if (DeployConfig.DockerConfig != null && DeployConfig.DockerConfig.EnvPairList != null)
                    {
                        var toRemove = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(changeEnv.Name));
                        if (toRemove != null) DeployConfig.DockerConfig.EnvPairList.Remove(toRemove);
                    }

                    if (DeployConfig.LinuxServiveConfig != null && DeployConfig.LinuxServiveConfig.EnvPairList != null)
                    {
                        var toRemove = DeployConfig.LinuxServiveConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(changeEnv.Name));
                        if (toRemove != null) DeployConfig.LinuxServiveConfig.EnvPairList.Remove(toRemove);
                    }
                }
                else
                {
                    if (DeployConfig.IIsConfig == null)
                    {
                        DeployConfig.IIsConfig = new IIsConfig
                        {
                            EnvPairList = new List<EnvPairConfig>
                            {
                                new EnvPairConfig
                                {
                                    EnvName = changeEnv.Name,
                                    ConfigName = this.txt_iis_web_site_name.Text
                                }
                            }
                        };
                    }
                    else if (DeployConfig.IIsConfig.EnvPairList == null)
                    {
                        DeployConfig.IIsConfig.EnvPairList = new List<EnvPairConfig>
                        {
                            new EnvPairConfig
                            {
                                EnvName = changeEnv.Name,
                                ConfigName = this.txt_iis_web_site_name.Text
                            }
                        };
                    }
                    else if (!DeployConfig.IIsConfig.EnvPairList.Exists(r => r.EnvName.Equals(changeEnv.Name)))
                    {
                        DeployConfig.IIsConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = changeEnv.Name,
                            ConfigName = this.txt_iis_web_site_name.Text
                        });
                    }

                    if (DeployConfig.WindowsServiveConfig == null)
                    {
                        DeployConfig.WindowsServiveConfig = new WindowsServiveConfig
                        {
                            EnvPairList = new List<EnvPairConfig>
                            {
                                new EnvPairConfig
                                {
                                    EnvName = changeEnv.Name,
                                    ConfigName = this.txt_windowservice_name.Text
                                }
                            }
                        };
                    }
                    else if (DeployConfig.WindowsServiveConfig.EnvPairList == null)
                    {
                        DeployConfig.WindowsServiveConfig.EnvPairList = new List<EnvPairConfig>
                        {
                            new EnvPairConfig
                            {
                                EnvName = changeEnv.Name,
                                ConfigName = this.txt_windowservice_name.Text
                            }
                        };
                    }
                    else if (!DeployConfig.WindowsServiveConfig.EnvPairList.Exists(r => r.EnvName.Equals(changeEnv.Name)))
                    {
                        DeployConfig.WindowsServiveConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = changeEnv.Name,
                            ConfigName = this.txt_windowservice_name.Text
                        });
                    }


                    if (DeployConfig.LinuxServiveConfig == null)
                    {
                        DeployConfig.LinuxServiveConfig = new LinuxServiveConfig
                        {
                            EnvPairList = new List<EnvPairConfig>
                            {
                                new EnvPairConfig
                                {
                                    EnvName = changeEnv.Name,
                                    ConfigName = this.txt_linuxservice_name.Text
                                }
                            }
                        };
                    }
                    else if (DeployConfig.LinuxServiveConfig.EnvPairList == null)
                    {
                        DeployConfig.LinuxServiveConfig.EnvPairList = new List<EnvPairConfig>
                        {
                            new EnvPairConfig
                            {
                                EnvName = changeEnv.Name,
                                ConfigName = this.txt_linuxservice_name.Text
                            }
                        };
                    }
                    else if (!DeployConfig.LinuxServiveConfig.EnvPairList.Exists(r => r.EnvName.Equals(changeEnv.Name)))
                    {
                        DeployConfig.LinuxServiveConfig.EnvPairList.Add(new EnvPairConfig
                        {
                            EnvName = changeEnv.Name,
                            ConfigName = this.txt_linuxservice_name.Text
                        });
                    }
                }

                #endregion
            }

            this.combo_iis_env.Items.Clear();
            this.combo_windowservice_env.Items.Clear();
            this.combo_docker_env.Items.Clear();
            this.combo_linux_env.Items.Clear();
            if (DeployConfig.Env.Any())
            {
                foreach (var env in DeployConfig.Env)
                {
                    this.combo_iis_env.Items.Add(env.Name);
                    this.combo_windowservice_env.Items.Add(env.Name);
                    this.combo_docker_env.Items.Add(env.Name);
                    this.combo_linux_env.Items.Add(env.Name);
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

                if (!string.IsNullOrEmpty(item4))
                {
                    if (this.combo_linux_env.Items.Cast<string>().Contains(item3))
                    {
                        this.combo_linux_env.SelectedItem = item3;
                    }
                    else
                    {
                        combo_linux_env_SelectedIndexChanged(null, null);
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

            }

            if (string.IsNullOrEmpty(ProjectFolderPath))
            {
                return;
            }

            var old_ProjectConfigPath = Path.Combine(ProjectFolderPath, "AntDeploy.json");
            var antdeployJsonInProjectPath = old_ProjectConfigPath;
            string rootPath = Path.GetDirectoryName(ProjectFolderPath.TrimEnd('\\'));
            string dirName = ProjectFolderPath.Substring(rootPath.Length).Trim('\\');
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //其他文件不要写入发布文件夹
            string newDir = Path.Combine(appDataPath, "AntDeploy", $"{dirName}_config");
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }
            var new_ProjectConfigPath = Path.Combine(newDir, "AntDeploy.json");
            //AntDeploy.json发布配置文件与发布文件隔离开 兼容老的配置文件第一次默认转移
            if (File.Exists(old_ProjectConfigPath) && !File.Exists(new_ProjectConfigPath))
            {
                File.Copy(old_ProjectConfigPath, new_ProjectConfigPath, false);
            }

            if (File.Exists(new_ProjectConfigPath))
            {
                var useAntJsonInProjectPath = GlobalConfig.EnableAntDeployJson && File.Exists(old_ProjectConfigPath);
                var config = useAntJsonInProjectPath ? File.ReadAllText(old_ProjectConfigPath, Encoding.UTF8): File.ReadAllText(new_ProjectConfigPath, Encoding.UTF8);
                if (useAntJsonInProjectPath)
                {
                    // 强制使用项目文件夹下的AntDeploy.json
                    File.Copy(old_ProjectConfigPath, new_ProjectConfigPath, true);
                }
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

            ProjectConfigPath = new Tuple<string, string>(new_ProjectConfigPath, antdeployJsonInProjectPath);
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
            else if (Directory.Exists(projectPath))
            {
                PluginConfig = new PluginConfig
                {
                    DeployFolderPath = projectPath
                };
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
                GlobalConfig = new GlobalConfig() { ProjectPathList = new List<string>(), IsChinease = CodingHelper.IsChineseSystem() };
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
                GlobalConfig = new GlobalConfig() { ProjectPathList = new List<string>(), IsChinease = CodingHelper.IsChineseSystem() };
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
            stop_docker_cancel_token = false;
            Condition = new AutoResetEvent(false);
            var port = this.txt_docker_port.Text.Trim();
            if (!string.IsNullOrEmpty(port))
            {
                if (!port.Contains(":"))
                {
                    int.TryParse(port, out var dockerPort);
                    if (dockerPort < 0)
                    {
                        MessageBoxEx.Show(this, Strings.PortValueInvaid);
                        return;
                    }
                    else
                    {
                        DeployConfig.DockerConfig.Prot = port;
                    }
                }
                else
                {
                    var arr = port.Split(':');
                    if (arr.Length != 2)
                    {
                        MessageBoxEx.Show(this, Strings.PortValueInvaid);
                        return;
                    }

                    foreach (var aPort in arr)
                    {
                        int.TryParse(aPort, out var dockerPort);
                        if (dockerPort < 0)
                        {
                            MessageBoxEx.Show(this, Strings.PortValueInvaid);
                            return;
                        }
                    }

                    DeployConfig.DockerConfig.Prot = port;
                }

            }
            else
            {
                DeployConfig.DockerConfig.Prot = "";
            }

            PluginConfig.RepositoryUrl = this.txt_docker_rep_domain.Text.Trim();
            PluginConfig.RepositoryUserName = this.txt_docker_rep_name.Text.Trim();
            PluginConfig.RepositoryUserPwd = this.txt_docker_rep_pwd.Text.Trim();
            PluginConfig.RepositoryNameSpace = this.txt_docker_rep_namespace.Text.Trim();
            PluginConfig.RepositoryImageName = this.txt_docker_rep_image.Text.Trim();

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
                        MessageBoxEx.Show(this, Strings.VolumeInvaid);
                        return;
                    }
                }
                DeployConfig.DockerConfig.Volume = volume;
            }

            DeployConfig.DockerConfig.Other = this.txt_docker_other.Text;

            var envName = this.combo_docker_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.DOCKER))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;

            var removeDays = this.t_docker_delete_days.Text.Trim();
            if (!string.IsNullOrEmpty(removeDays))
            {
                int.TryParse(removeDays, out var _removeDays);
                if (_removeDays <= 0)
                {
                    MessageBoxEx.Show(this, Strings.DaysInvaild);
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


            //必须是netcore应用
            var isNetcoreProject = _project.IsNetcorePorject;
            if (!isNetcoreProject)
            {
                MessageBoxEx.Show(this, Strings.NowNetcoreProject);
                return;
            }

            if (PluginConfig.DockerServiceEnableUpload)
            {
                if (
                    string.IsNullOrEmpty(PluginConfig.RepositoryUserName) ||
                    string.IsNullOrEmpty(PluginConfig.RepositoryUserPwd) ||
                    string.IsNullOrEmpty(PluginConfig.RepositoryNameSpace) ||
                    string.IsNullOrEmpty(PluginConfig.RepositoryUserPwd))
                {
                    MessageBoxEx.Show(this, Strings.DockerRepositoryRequired);
                    return;
                }
            }

            //如果是特定文件夹发布 得选择一个入口dll
            CheckIsDockerSpecialFolderDeploy();

            var ENTRYPOINT = _project.OutPutName;
            if (string.IsNullOrEmpty(ENTRYPOINT))
            {
                //MessageBoxEx.Show(this,"get current project property:outputfilename error");
                ENTRYPOINT = this.ProjectName + ".dll";
            }

            if (string.IsNullOrEmpty(_project.NetCoreSDKVersion))
            {
                _project.NetCoreSDKVersion = ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            }

            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && !ProjectHelper.CheckDockerFileIsSetCopy(ProjectPath))
            {
                var confirmDockerfile = ShowInputMsgBox(Strings.DockerFileWarn,
                    Strings.DockerFileNotSetCopy,"hide" );
                if (!confirmDockerfile.Item1)
                {
                    return;
                }
            }

            var SDKVersion = _project.NetCoreSDKVersion;
            if (string.IsNullOrEmpty(SDKVersion))
            {
                MessageBoxEx.Show(this, Strings.GetNetCoreSdkVersionError);
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.NoLinuxServer);
                return;
            }

            if (ProgressBox.IsEnableGroup)
            {
                //获取所有的选择了的server
                var selectedList = getSelectedBaseServers(ServerType.DOCKER);
                if (!selectedList.Any())
                {
                    MessageBoxEx.Show(this, Strings.NoLinuxServer);
                    return;
                }

                //找到选择了的
                serverList = serverList.Where(r => selectedList.Any(y => y.Host.Equals(r.Host))).ToList();
                if (!serverList.Any())
                {
                    MessageBoxEx.Show(this, Strings.NoLinuxServer);
                    return;
                }
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = ShowInputMsgBox(Strings.ConfirmDeploy,
                Strings.DeployServerConfim + Environment.NewLine + serverHostList);


            if (!confirmResult.Item1)
            {
                return;
            }

            combo_docker_env_SelectedIndexChanged(null, null);

            this.rich_docker_log.Text = "";
            this.nlog_docker.Info($"The Porject ENTRYPOINT name:{ENTRYPOINT},DotNetSDK.Version:{_project.NetCoreSDKVersion}");

            new Task(() =>
           {
               this.nlog_docker.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
               PrintCommonLog(this.nlog_docker);
               EnableForDocker(false);
               GitClient gitModel = null;
               var publishPath = !string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? PluginConfig.DeployFolderPath : Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_docker", envName);
               var clientDateTimeFolderNameParent = string.Empty;
               try
               {

                   if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                   {
                       var path = publishPath + "\\";
                       //执行 publish
                       var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                           $"publish \"{ProjectPath}\" -c Release{PluginConfig.GetNetCorePublishRuntimeArg()}", nlog_docker, () => stop_docker_cancel_token);

                       if (!isSuccess)
                       {
                           this.nlog_docker.Error("publish error,please check build log");
                           BuildError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                   }


                   var serviceFile = Path.Combine(publishPath, ENTRYPOINT);
                   if (!File.Exists(serviceFile))
                   {
                       this.nlog_docker.Error($"ENTRYPOINT file can not find in publish folder: {serviceFile}");
                       BuildError(this.tabPage_docker, serverList.First().Host);
                       return;
                   }

                   BuildEnd(this.tabPage_docker, serverList.First().Host); //第一台结束编译

                   LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                   publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                   publisEvent.LoggerName = "rich_docker_log";
                   this.nlog_docker.Log(publisEvent);



                   //执行 打包
                   this.nlog_docker.Info("-----------------Start package-----------------");

                   if (stop_docker_cancel_token)
                   {
                       this.nlog_docker.Warn($"deploy task was canceled!");
                       PackageError(this.tabPage_docker, serverList.First().Host);
                       return;
                   }

                   //查看是否开启了增量
                   if (this.PluginConfig.DockerEnableIncrement)
                   {
                       this.nlog_docker.Info("Enable Increment Deploy:true");
                       gitModel = new GitClient(publishPath, this.nlog_docker);
                       if (!gitModel.InitSuccess)
                       {
                           this.nlog_docker.Error("package fail,can not init git,please cancel Increment Deploy");
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }

                   }

                   MemoryStream zipBytes = null;
                   //含有中文的文件名映射表
                   var chineseFileList = new Dictionary<string, Tuple<string, bool>>();
                   var gitChangeFileCount = 0;
                   if (gitModel != null)
                   {
                       var fileList = gitModel.GetChanges();
                       gitChangeFileCount = fileList.Count;
                       if (gitChangeFileCount < 1)
                       {
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }

                       this.nlog_docker.Info("【git】Increment package file count:" + gitChangeFileCount);
                       if (this.PluginConfig.DockerServiceEnableSelectDeploy)
                       {
                           this.nlog_docker.Info("-----------------Select File Start-----------------");
                           this.BeginInvokeLambda(() =>
                           {
                               var slectFileForm = new SelectFile(fileList, publishPath, ignoreList);
                               slectFileForm.ShowDialog();

                               if (slectFileForm.SelectedFileList == null || !slectFileForm.SelectedFileList.Any())
                               {
                                   PackageError(this.tabPage_docker, serverList.First().Host);
                                   this.nlog_docker.Error("Please Select Files");
                                   return;
                               }

                               fileList = slectFileForm.SelectedFileList;
                               this.nlog_docker.Info("Select Files count:" + fileList.Count);
                               //this.nlog_docker.Debug("ignore package ignoreList");
                               this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                               Condition.Set();
                           });

                           Condition.WaitOne();
                       }

                       if (stop_docker_cancel_token)
                       {
                           this.nlog_docker.Warn($"deploy task was canceled!");
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                       try
                       {
                           this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                           //这里要记录下fileList 里面有哪些是包含中文的文件名称
                           var b_zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, false,
                               ignoreList,
                               (progressValue) =>
                               {
                                   UpdatePackageProgress(this.tabPage_docker, serverList.First().Host, progressValue); //打印打包记录
                                   return stop_docker_cancel_token;
                               }, this.PluginConfig.DockerServiceEnableSelectDeploy, nlog_docker, chineseFileList);
                           if (b_zipBytes.Length < 1)
                           {
                               this.nlog_docker.Error("package fail");
                               PackageError(this.tabPage_docker, serverList.First().Host);
                               return;
                           }
                           zipBytes = new MemoryStream(b_zipBytes);
                       }
                       catch (Exception ex)
                       {
                           this.nlog_docker.Error("package fail:" + ex.Message);
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                   }
                   else if (this.PluginConfig.DockerServiceEnableSelectDeploy)
                   {
                       var fileList = new List<string>();
                       this.nlog_docker.Info("-----------------Select File Start-----------------");
                       this.BeginInvokeLambda(() =>
                       {
                           var slectFileForm = new SelectFile(publishPath, ignoreList);
                           slectFileForm.ShowDialog();
                           if (slectFileForm.SelectedFileList == null || !slectFileForm.SelectedFileList.Any())
                           {
                               PackageError(this.tabPage_docker, serverList.First().Host);
                               this.nlog_docker.Error("Please Select Files");
                               return;
                           }
                           fileList = slectFileForm.SelectedFileList;
                           this.nlog_docker.Info("Select Files count:" + fileList.Count);
                           //this.nlog_docker.Debug("ignore package ignoreList");
                           this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                           Condition.Set();
                       });

                       Condition.WaitOne();

                       if (stop_docker_cancel_token)
                       {
                           this.nlog_docker.Warn($"deploy task was canceled!");
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                       try
                       {
                           this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                           //这里要记录下fileList 里面有哪些是包含中文的文件名称
                           var b_zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, false,
                               ignoreList,
                               (progressValue) =>
                               {
                                   UpdatePackageProgress(this.tabPage_docker, serverList.First().Host, progressValue); //打印打包记录
                                   return stop_docker_cancel_token;
                               }, this.PluginConfig.DockerServiceEnableSelectDeploy, nlog_docker, chineseFileList);
                           if (b_zipBytes.Length < 1)
                           {
                               this.nlog_docker.Error("package fail");
                               PackageError(this.tabPage_docker, serverList.First().Host);
                               return;
                           }
                           zipBytes = new MemoryStream(b_zipBytes);
                       }
                       catch (Exception ex)
                       {
                           this.nlog_docker.Error("package fail:" + ex.Message);
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                   }

                   if (zipBytes == null)
                   {
                       try
                       {
                           this.nlog_docker.Info($"package ignoreList Count:{ignoreList.Count}");
                           //这里也得记录出有包含中文的文件名称
                           var b_zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, false,
                               ignoreList,
                               (progressValue) =>
                               {
                                   UpdatePackageProgress(this.tabPage_docker, serverList.First().Host, progressValue); //打印打包记录
                                   return stop_docker_cancel_token;
                               }, nlog_docker, chineseFileList);
                           if (b_zipBytes.Length < 1)
                           {
                               this.nlog_docker.Error("package fail");
                               PackageError(this.tabPage_docker, serverList.First().Host);
                               return;
                           }
                           zipBytes = new MemoryStream(b_zipBytes);
                       }
                       catch (Exception ex)
                       {
                           this.nlog_docker.Error("package fail:" + ex.Message);
                           PackageError(this.tabPage_docker, serverList.First().Host);
                           return;
                       }
                   }


                   if (zipBytes == null || zipBytes.Length < 1)
                   {
                       this.nlog_docker.Error("package fail");
                       PackageError(this.tabPage_docker, serverList.First().Host);
                       return;
                   }
#if DEBUG
                   using (FileStream file = new FileStream("package.zip", FileMode.Create, System.IO.FileAccess.Write))
                       zipBytes.CopyTo(file);
#endif
                   var packageSize = (zipBytes.Length / 1024 / 1024);
                   this.nlog_docker.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                   //执行 上传
                   this.nlog_docker.Info("-----------------Deploy Start-----------------");
                   clientDateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                   var clientDateTimeFolderName = string.Empty;
                   var retryTimes = 0;
                   var allfailServerList = new List<LinuxServer>();
RETRY_DOCKER:
                   var failServerList = new List<LinuxServer>();
                   var index = 0;
                   var allSuccess = true;
                   var failCount = 0;
                   var isRetry = allfailServerList.Count > 0;
                   if (isRetry)
                   {
                       clientDateTimeFolderName = clientDateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                   }
                   else
                   {
                       clientDateTimeFolderName = clientDateTimeFolderNameParent;
                   }
                   //重试了 但是没有发现错误的Server List
                   if (retryTimes > 0 && allfailServerList.Count == 0) return;
                   foreach (var server in isRetry ? allfailServerList : serverList)
                   {
                       if (isRetry) UploadReset(this.tabPage_docker, server.Host);
                       if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                       {
                           BuildEnd(this.tabPage_docker, server.Host);
                           UpdatePackageProgress(this.tabPage_docker, server.Host, 100);
                       }
                       if (stop_docker_cancel_token)
                       {
                           this.nlog_docker.Warn($"deploy task was canceled!");
                           UploadError(this.tabPage_docker, server.Host);
                           return;
                       }
                       index++;
                       #region 参数Check

                       if (string.IsNullOrEmpty(server.Host))
                       {
                           this.nlog_docker.Error("Server Host is Empty");
                           UploadError(this.tabPage_docker, serverList.First().Host);
                           allSuccess = false;
                           failCount++;
                           failServerList.Add(server);
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.UserName))
                       {
                           this.nlog_docker.Error("Server UserName is Empty");
                           UploadError(this.tabPage_docker, serverList.First().Host);
                           allSuccess = false;
                           failCount++;
                           failServerList.Add(server);
                           continue;
                       }

                       if (string.IsNullOrEmpty(server.Pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker, serverList.First().Host);
                           allSuccess = false;
                           failCount++;
                           failServerList.Add(server);
                           continue;
                       }

                       var pwd = CodingHelper.AESDecrypt(server.Pwd);
                       if (string.IsNullOrEmpty(pwd))
                       {
                           this.nlog_docker.Error("Server Pwd is Empty");
                           UploadError(this.tabPage_docker, serverList.First().Host);
                           allSuccess = false;
                           failCount++;
                           failServerList.Add(server);
                           continue;
                       }

                       #endregion

                       var hasError = false;

                       zipBytes.Seek(0, SeekOrigin.Begin);
                       using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, PluginConfig.DeployHttpProxy, (str, logLevel) =>
                        {

                            if (logLevel == NLog.LogLevel.Error)
                            {
                                hasError = true;
                                allSuccess = false;
                                this.nlog_docker.Error("【Server】" + str);
                            }
                            else if (logLevel == NLog.LogLevel.Warn)
                            {
                                this.nlog_docker.Warn("【Server】" + str);
                            }
                            else
                            {
                                this.nlog_docker.Info("【Server】" + str);
                            }

                            return stop_docker_cancel_token;
                        }, (uploadValue) => { UpdateUploadProgress(this.tabPage_docker, server.Host, uploadValue); })
                       {
                           NetCoreENTRYPOINT = ENTRYPOINT,
                           NetCoreVersion = SDKVersion,
                           NetCorePort = DeployConfig.DockerConfig.Prot,
                           NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                           ClientDateTimeFolderName = clientDateTimeFolderName,
                           RemoveDaysFromPublished = DeployConfig.DockerConfig.RemoveDaysFromPublished,
                           Volume = DeployConfig.DockerConfig.Volume,
                           Other = DeployConfig.DockerConfig.Other,
                           Remark = confirmResult.Item2,
                           UseAsiaShanghai = GlobalConfig.UseAsiaShanghai,
                           Increment = this.PluginConfig.DockerEnableIncrement || this.PluginConfig.DockerServiceEnableSelectDeploy,
                           Sudo = this.PluginConfig.DockerEnableSudo ? "sudo" : "",
                           IsSelect = this.PluginConfig.DockerServiceEnableSelectDeploy,
                           DockerServiceEnableUpload = this.PluginConfig.DockerServiceEnableUpload,
                           DockerServiceBuildImageOnly = this.PluginConfig.DockerServiceBuildImageOnly,
                           RepositoryUrl = this.PluginConfig.RepositoryUrl,
                           RepositoryUserName = this.PluginConfig.RepositoryUserName,
                           RepositoryUserPwd = this.PluginConfig.RepositoryUserPwd,
                           RepositoryNameSpace = this.PluginConfig.RepositoryNameSpace,
                           RepositoryImageName = this.PluginConfig.RepositoryImageName
                       })
                       {
                           var connectResult = sshClient.Connect();
                           if (!connectResult)
                           {
                               this.nlog_docker.Error($"Deploy Host:{getHostDisplayName(server)} Fail: connect fail");
                               UploadError(this.tabPage_docker, server.Host);
                               allSuccess = false;
                               failCount++;
                               failServerList.Add(server);
                               continue;
                           }

                           try
                           {
                               sshClient.PublishZip(zipBytes, "antdeploy", "publish.zip", () => !stop_docker_cancel_token, chineseFileList);
                               UpdateUploadProgress(this.tabPage_docker, server.Host, 100);

                               if (stop_docker_cancel_token)
                               {
                                   this.nlog_docker.Warn($"deploy task was canceled!");
                                   UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                                   return;
                               }
                               if (hasError)
                               {
                                   allSuccess = false;
                                   failCount++;
                                   failServerList.Add(server);
                                   //sshClient.DeletePublishFolder("antdeploy");
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
                                           failServerList.Add(server);
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
                               failServerList.Add(server);
                               this.nlog_docker.Error($"Deploy Host:{getHostDisplayName(server)} Fail:" + ex.Message);
                               UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                           }
                       }

                   }


                   if (allSuccess)
                   {
                       this.nlog_docker.Info("Deploy Version：" + clientDateTimeFolderNameParent);
                       if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                       allfailServerList = new List<LinuxServer>();
                       Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                   }
                   else
                   {
                       Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                       if (!stop_docker_cancel_token)
                       {
                           allfailServerList = new List<LinuxServer>();
                           allfailServerList.AddRange(failServerList);
                           EnableDockerRetry(true);
                           //看是否要重试
                           Condition.WaitOne();
                           if (!stop_docker_cancel_token)
                           {
                               retryTimes++;
                               goto RETRY_DOCKER;
                           }
                       }
                   }
                   zipBytes.Dispose();
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
                   //记录发布日志
                   SaveLog(publishPath, clientDateTimeFolderNameParent, nlog_docker);
                   EnableForDocker(true);
               }



           }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }

        private void btn_docker_rollback_Click(object sender, EventArgs e)
        {
            var envName = this.combo_docker_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.DOCKER))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

#if DEBUG
            var ENTRYPOINT = this.ProjectName + ".dll";
#else
            //必须是netcore应用
            var isNetcoreProject = _project.IsNetcorePorject;
            if (!isNetcoreProject)
            {
                MessageBoxEx.Show(this,Strings.NowNetcoreProject);
                return;
            }

            //如果是特定文件夹发布 得选择一个dll
            CheckIsDockerSpecialFolderDeploy();

            var ENTRYPOINT = _project.OutPutName;
            if (string.IsNullOrEmpty(ENTRYPOINT))
            {
                ENTRYPOINT = this.ProjectName + ".dll";//MessageBoxEx.Show(this,"get current project property:outputfilename error");
            }
#endif


            if (string.IsNullOrEmpty(_project.NetCoreSDKVersion))
            {
                _project.NetCoreSDKVersion = ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            }
            var SDKVersion = _project.NetCoreSDKVersion;
            //if (string.IsNullOrEmpty(SDKVersion))
            //{
            //    MessageBoxEx.Show(this,"get current project skd version error");
            //    return;
            //}

            combo_docker_env_SelectedIndexChanged(null, null);


            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.LinuxServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.NoLinuxServer);
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBoxEx.Show(this,
                Strings.DeployRollBackConfirm + Environment.NewLine + serverHostList,
                Strings.RollBackConfirm,
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            this.rich_docker_log.Text = "";
            this.nlog_docker.Info($"The Porject ENTRYPOINT name:{ENTRYPOINT},DotNetSDK.Version:{_project.NetCoreSDKVersion}");
            //this.tabControl_docker.SelectedIndex = 1;
            PrintCommonLog(this.nlog_docker);
            new Task(() =>
            {

                try
                {

                    EnableForDocker(false, true);

                    this.nlog_docker.Info($"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");


                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        BuildEnd(this.tabPage_docker, server.Host);
                        UpdatePackageProgress(this.tabPage_docker, server.Host, 100);
                        UpdateUploadProgress(this.tabPage_docker, server.Host, 100);
                        #region 参数Check

                        if (string.IsNullOrEmpty(server.Host))
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} Rollback skip,Server Host is Empty");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(server.UserName))
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} Rollback skip,Server UserName is Empty");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(server.Pwd))
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} Rollback skip,Server Pwd is Empty");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        var pwd = CodingHelper.AESDecrypt(server.Pwd);
                        if (string.IsNullOrEmpty(pwd))
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} Rollback skip,Server Pwd is Empty");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }


                        #endregion

                        this.nlog_docker.Info($"Host:{getHostDisplayName(server)} Start get rollBack version list");
                        Tuple<string, List<Tuple<string, string>>> versionList = null;
                        using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, PluginConfig.DeployHttpProxy,
                            (str, logLevel) =>
                            {
                                if (logLevel == NLog.LogLevel.Error)
                                {
                                    this.nlog_docker.Error("【Server】" + str);
                                }
                                else if (logLevel == NLog.LogLevel.Warn)
                                {
                                    this.nlog_docker.Warn("【Server】" + str);
                                }
                                else
                                {
                                    this.nlog_docker.Info("【Server】" + str);
                                }

                                return stop_docker_cancel_token;
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
                                this.nlog_docker.Error($"connect rollBack Host:{getHostDisplayName(server)} Fail");
                                UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                                allSuccess = false;
                                failCount++;
                                continue;
                            }

                            versionList = sshClient.GetDeployHistory("antdeploy", 10);
                        }

                        if (versionList == null || versionList.Item2.Count <= 1)
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} get rollBack version list count:0");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        this.nlog_docker.Info($"Host:{getHostDisplayName(server)} get rollBack version list count:{versionList.Item2.Count}");
                        this.BeginInvokeLambda(() =>
                       {
                           RollBack rolleback = new RollBack(versionList);
                           rolleback.SetTitle($"Current Server:{getHostDisplayName(server)}");
                           var r = rolleback.ShowDialog();
                           if (r == DialogResult.Cancel)
                           {
                               _rollBackVersion = null;
                           }
                           else
                           {
                               _rollBackVersion = new RollBackVersion
                               {
                                   Version = rolleback.SelectRollBackVersion
                               };
                           }
                           Condition.Set();
                       });
                        Condition.WaitOne();
                        if (_rollBackVersion == null || string.IsNullOrEmpty(_rollBackVersion.Version))
                        {
                            this.nlog_docker.Error($"Host:{getHostDisplayName(server)} Rollback canceled!");
                            UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }
                        var hasError = false;
                        using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, PluginConfig.DeployHttpProxy, (str, logLevel) =>
                        {
                            if (logLevel == NLog.LogLevel.Error)
                            {
                                hasError = true;
                                allSuccess = false;
                                this.nlog_docker.Error("【Server】" + str);
                            }
                            else if (logLevel == NLog.LogLevel.Warn)
                            {
                                this.nlog_docker.Warn("【Server】" + str);
                            }
                            else
                            {
                                this.nlog_docker.Info("【Server】" + str);
                            }

                            return stop_docker_cancel_token;
                        }, (uploadValue) => { UpdateUploadProgress(this.tabPage_docker, server.Host, uploadValue); })
                        {
                            NetCoreENTRYPOINT = ENTRYPOINT,
                            NetCoreVersion = SDKVersion,
                            NetCorePort = DeployConfig.DockerConfig.Prot,
                            NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                            Sudo = this.PluginConfig.DockerEnableSudo ? "sudo" : ""
                        })
                        {
                            var connectResult = sshClient.Connect();
                            if (!connectResult)
                            {
                                this.nlog_docker.Error($"RollBack Host:{getHostDisplayName(server)} Fail: connect fail");
                                UpdateDeployProgress(this.tabPage_docker, server.Host, false);
                                allSuccess = false;
                                failCount++;
                                continue;
                            }

                            try
                            {
                                sshClient.RollBack(_rollBackVersion.Version);
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
                    }

                    this.nlog_docker.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    Notice("Rollback End", $"[Total]:{serverList.Count},[Fail]:{failCount}");
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

            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();

        }

        private void EnableForDockerImage(bool flag, bool ignore = false)
        {
            this.BeginInvokeLambda(() =>
            {
                this.txt_BaseImage.Enabled = flag;
                this.txt_BaseImage_username.Enabled = flag;
                this.txt_BaseImage_pwd.Enabled = flag;
                this.txt_TargetImage.Enabled = flag;
                this.txt_TargetImage_username.Enabled = flag;
                this.altoButton1.Enabled = flag;
                if (!ignore)
                {
                    this.altoButton1.Visible = flag;
                    btn_docker_image_stop.Visible = !flag;
                }

                this.txt_TargetImage_pwd.Enabled = flag;
                this.txt_TargetHttpProxy.Enabled = flag;
                this.txt_HttpProxy.Enabled = flag;
                this.txt_TargetImage_tag.Enabled = flag;
                this.txt_Cmd.Enabled = flag;
                this.txt_Entrypoint.Enabled = flag;
                this.cmbo_ImageFormat.Enabled = flag;

                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_linux_service.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;

            });

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
                    }
                }

                this.checkBoxdocker_rep_uploadOnly.Enabled = flag;
                this.t_docker_delete_days.Enabled = flag;
                this.txt_docker_volume.Enabled = flag;
                this.txt_docker_other.Enabled = flag;
                this.b_docker_rollback.Enabled = flag;
                this.b_docker_deploy.Enabled = flag;
                if (!ignore)
                {
                    this.b_docker_deploy.Visible = flag;
                    btn_docker_stop.Visible = !flag;
                }

                this.combo_docker_env.Enabled = flag;
                this.txt_docker_port.Enabled = flag;
                this.txt_docker_envname.Enabled = flag;
                this.txt_docker_rep_domain.Enabled = flag;
                this.checkBoxdocker_rep_enable.Enabled = flag;
                this.txt_docker_rep_name.Enabled = flag;
                this.txt_docker_rep_pwd.Enabled = flag;
                this.txt_docker_rep_namespace.Enabled = flag;
                this.txt_docker_rep_image.Enabled = flag;

                this.page_set.Enabled = flag;
                this.page_window_service.Enabled = flag;
                this.page_web_iis.Enabled = flag;
                this.page_linux_service.Enabled = flag;
                this.pag_advance_setting.Enabled = flag;

                this.checkBox_Increment_docker.Enabled = flag;
                this.checkBox_sudo_docker.Enabled = flag;
                this.checkBox_select_deploy_docker.Enabled = flag;
                if (flag)
                {
                    this.rich_iis_log.Text = "";
                    this.rich_windowservice_log.Text = "";
                    this.rich_linuxservice_log.Text = "";
                    btn_docker_stop.Enabled = true;
                    btn_docker_stop.Text = "Stop";
                }
                else
                {
                    tabcontrol.Tag = "1";
                    if (ignore) return;
                    if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                    {
                        if (ProgressBox.IsEnableGroup)
                        {
                            foreach (var box in progressBoxList)
                            {
                                if (box.Value.CheckBox.Visible && box.Value.CheckBox.Checked)
                                {
                                    box.Value.StartBuild();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var box in progressBoxList)
                            {
                                box.Value.StartBuild();
                                break;
                            }
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

                //设置对应的websitename
                if (DeployConfig.DockerConfig.EnvPairList != null && DeployConfig.DockerConfig.EnvPairList.Any())
                {
                    var target = DeployConfig.DockerConfig.EnvPairList.FirstOrDefault(r => r.EnvName.Equals(selectName));
                    if (target != null)
                    {
                        if (!string.IsNullOrEmpty(target.DockerPort)) this.txt_docker_port.Text = target.DockerPort;
                        if (!string.IsNullOrEmpty(target.DockerEnvName)) this.txt_docker_envname.Text = target.DockerEnvName;
                        if (!string.IsNullOrEmpty(target.DockerVolume)) this.txt_docker_volume.Text = target.DockerVolume;
                        if (!string.IsNullOrEmpty(target.DockerOther)) this.txt_docker_other.Text = target.DockerOther;
                    }
                }

                var dic = new Dictionary<string, bool>();
                //生成进度
                if (this.tabPage_docker.Tag is Dictionary<string, ProgressBox> progressBoxList)
                {
                    foreach (var box in progressBoxList)
                    {
                        dic.Add(box.Value.Server.Host, box.Value.CheckBox.Checked);
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
                        new ProgressBox(new System.Drawing.Point(ProgressBoxLocationLeft, 15 + (i * 140)), serverList[i], ServerType.DOCKER, HostoryButtonSearch)
                        {
                            Text = serverHost + (!string.IsNullOrWhiteSpace(nickName) ? $"【{nickName}】" : ""),
                        };

                    if (dic.TryGetValue(serverHost, out var chec))
                    {
                        newBox.CheckBox.Checked = chec;
                    }

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
            if (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
            {
                log.Info("Special Folder Publish : " + PluginConfig.DeployFolderPath);
                log.Info("ignore [Build] ");
            }


            var vsVersion = _project.VsVersion;
            if (!string.IsNullOrEmpty(vsVersion))
            {
                log.Info("Visual Studio Version : " + vsVersion);
            }

            if (ProjectConfigPath!=null)
            {
                var fileInfo = new FileInfo(GlobalConfig.EnableAntDeployJson ? ProjectConfigPath.Item2 : ProjectConfigPath.Item1);
                if (fileInfo.Exists && !string.IsNullOrEmpty(fileInfo.FullName))
                {
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "【AntDeploy.json】 ");
                    publisEvent.Properties["ShowLink"] = "file://" + fileInfo.FullName.Replace("\\", "\\\\") ;
                    publisEvent.LoggerName = log.Name;
                    log.Log(publisEvent);
                }
            }
            if (!string.IsNullOrEmpty(ProjectPath))
            {
                var fileInfo = new FileInfo(ProjectPath);
                if (fileInfo.Exists && !string.IsNullOrEmpty(fileInfo.DirectoryName))
                {
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "【CurrentProjectFolder】");
                    publisEvent.Properties["ShowLink"] = "file://" + fileInfo.DirectoryName.Replace("\\", "\\\\");
                    publisEvent.LoggerName = log.Name;
                    log.Log(publisEvent);
                }
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
                MessageBoxEx.Show(this, "no env have ignore value!");
                return;
            }
            RollBack rollBack = new RollBack(envList, true) { Text = "Select Env Name" };
            rollBack.SetButtonName("Copy");
            rollBack.SetTitle($"Current Env:{current}");
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
                MessageBoxEx.Show(this, "no env have ignore value!");
                return;
            }
            RollBack rollBack = new RollBack(envList, true) { Text = "Select Env Name" };
            rollBack.SetButtonName("Copy");
            rollBack.SetTitle($"Current Env:{current}");
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
            MessageBoxEx.ShowOk(this, "change success please reload antdeploy!");
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
                    MessageBoxEx.Show(this, "choose file error！");
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

        private Tuple<bool, string> ShowInputMsgBox(string title, string message, string defaultValue = null)
        {
            InputDialog dialog = new InputDialog(message, title, defaultValue);
            dialog.SetInputLength(0, 200);
            var result = dialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return new Tuple<bool, string>(false, string.Empty);
            }
            return new Tuple<bool, string>(true, dialog.Input);
        }

        private void btn_iis_stop_Click(object sender, EventArgs e)
        {
            //iis的停止按钮点击了
            btn_iis_stop.Text = "Stoping";
            btn_iis_stop.Enabled = false;
            stop_iis_cancel_token = true;
            btn_iis_retry.Visible = false;
            Condition.Set();
        }

        private void btn_windows_serivce_stop_Click(object sender, EventArgs e)
        {
            btn_windows_serivce_stop.Text = "Stoping";
            btn_windows_serivce_stop.Enabled = false;
            stop_windows_cancel_token = true;
            btn_windows_service_retry.Visible = false;
            Condition.Set();
        }

        private void btn_docker_stop_Click(object sender, EventArgs e)
        {
            btn_docker_stop.Text = "Stoping";
            btn_docker_stop.Enabled = false;
            stop_docker_cancel_token = true;
            btn_docker_retry.Visible = false;
            Condition.Set();
        }

        private void btn_iis_retry_Click(object sender, EventArgs e)
        {
            btn_iis_retry.Visible = false;
            Condition.Set();
        }

        private void btn_docker_retry_Click(object sender, EventArgs e)
        {
            btn_docker_retry.Visible = false;
            Condition.Set();
        }

        private void btn_windows_service_retry_Click(object sender, EventArgs e)
        {
            btn_windows_service_retry.Visible = false;
            Condition.Set();
        }

        private void btn_choose_folder_Click(object sender, EventArgs e)
        {
            using (var fsd = new FolderSelectDialog())
            {

                fsd.Title = GlobalConfig.IsChinease ? "选择指定的文件夹发布" : "Select Folder To Deploy";
                if (fsd.ShowDialog(this.Handle))
                {
                    var folder = fsd.FileName;
                    if (!string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
                    {
                        this.txt_folder_deploy.Text = folder;
                        PluginConfig.DeployFolderPath = folder;
                    }
                }
            }



            //using (var fbd = new FolderBrowserDialog())
            //{
            //    DialogResult result = fbd.ShowDialog();

            //    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            //    {
            //        var folder = fbd.SelectedPath;
            //        this.txt_folder_deploy.Text = folder;
            //        PluginConfig.DeployFolderPath = folder;
            //    }
            //}
        }

        private void btn_folder_clear_Click(object sender, EventArgs e)
        {
            this.txt_folder_deploy.Text = string.Empty;
            PluginConfig.DeployFolderPath = string.Empty;
        }

        private void btn_shang_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void CheckIsDockerSpecialFolderDeploy()
        {
            if (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && string.IsNullOrEmpty(_project.OutPutName))
            {
                var exePath = string.Empty;
                //找当前根目录下 有没有 .deps.json 文件
                var depsJsonFile = CodingHelper.FindDepsJsonFile(PluginConfig.DeployFolderPath);
                if (!string.IsNullOrEmpty(depsJsonFile))
                {
                    var exeTempName = depsJsonFile.Replace(".deps.json", ".dll");
                    if (File.Exists(exeTempName))
                    {
                        exePath = exeTempName;
                        _project.NetCoreSDKVersion = CodingHelper.GetSdkInDepsJson(depsJsonFile);
                    }
                }

                if (string.IsNullOrEmpty(exePath))
                {
                    exePath = CodingHelper.GetDockerServiceExe(PluginConfig.DeployFolderPath);
                    if (string.IsNullOrEmpty(exePath))
                    {
                        MessageBoxEx.Show(this, "please select dll path");
                        return;
                    }
                }

                var dllName = new FileInfo(exePath).Name;
                if (string.IsNullOrEmpty(_project.NetCoreSDKVersion))
                {
                    var devopsJsonPath = exePath.Replace(".dll", ".deps.json");
                    if (!File.Exists(devopsJsonPath))
                    {
                        MessageBoxEx.Show(this, dllName.Replace(".dll", ".deps.json") + " not found!");
                        return;
                    }
                    _project.NetCoreSDKVersion = CodingHelper.GetSdkInDepsJson(devopsJsonPath);
                }

                _project.OutPutName = dllName;
            }
        }

        /// <summary>
        /// 查看发布历史
        /// </summary>
        private void HostoryButtonSearch(ServerType ServerType, BaseServer Server)
        {
            if (ServerType == ServerType.DOCKER)
            {
                var server = Server as LinuxServer;
                if (server == null)
                {
                    MessageBoxEx.Show(this, "Server is not correct!");
                    return;
                }

                #region 参数Check

                if (string.IsNullOrEmpty(server.Host))
                {
                    MessageBoxEx.Show(this, "Server Host is Empty!");
                    return;
                }

                if (string.IsNullOrEmpty(server.UserName))
                {
                    MessageBoxEx.Show(this, "Server UserName is Empty!");
                    return;
                }

                if (string.IsNullOrEmpty(server.Pwd))
                {
                    MessageBoxEx.Show(this, "Server Pwd is Empty!");
                    return;
                }

                var pwd = CodingHelper.AESDecrypt(server.Pwd);
                if (string.IsNullOrEmpty(pwd))
                {
                    MessageBoxEx.Show(this, "Server Pwd is Empty!");
                    return;
                }


                #endregion

                //如果是特定文件夹发布 得选择一个dll

                CheckIsDockerSpecialFolderDeploy();
                var ENTRYPOINT = _project.OutPutName;
                if (string.IsNullOrEmpty(ENTRYPOINT))
                {
                    ENTRYPOINT = this.ProjectName + ".dll";//MessageBoxEx.Show(this,"get current project property:outputfilename error");
                }

                new Task(() =>
               {
                   try
                   {
                       EnableForDocker(false, true);
                       Tuple<string, List<Tuple<string, string>>> versionList = null;
                       using (SSHClient sshClient = new SSHClient(server.Host, server.UserName, pwd, PluginConfig.DeployHttpProxy,
                           (str, logLevel) => { return false; }, (uploadValue) => { })
                       {
                           NetCoreENTRYPOINT = ENTRYPOINT,
                           NetCorePort = DeployConfig.DockerConfig.Prot,
                           NetCoreEnvironment = DeployConfig.DockerConfig.AspNetCoreEnv,
                       })
                       {
                           var connectResult = sshClient.Connect();
                           if (!connectResult)
                           {
                               ShowThreadMessageBox("get history list fail");
                               return;
                           }
                           versionList = sshClient.GetDeployHistory("antdeploy", 10);
                       }

                       if (versionList == null || versionList.Item2.Count < 1)
                       {
                           ShowThreadMessageBox("get history list count:0");
                           return;
                       }

                       this.BeginInvokeLambda(() =>
                       {
                           RollBack rolleback = new RollBack(versionList);
                           rolleback.ShowAsHistory(!string.IsNullOrEmpty(server.NickName)
                               ? server.NickName
                               : server.Host);
                       });
                   }
                   catch (Exception e)
                   {
                       ShowThreadMessageBox(e.Message);
                   }
                   finally
                   {
                       EnableForDocker(true);
                   }
               }).Start();
            }
            else
            {
                if (ServerType == ServerType.IIS && string.IsNullOrEmpty(DeployConfig.IIsConfig.WebSiteName))
                {
                    MessageBoxEx.Show(this, "WebSiteName is not correct!");
                    return;
                }

                if (ServerType == ServerType.WINSERVICE && string.IsNullOrEmpty(DeployConfig.WindowsServiveConfig.ServiceName))
                {
                    MessageBoxEx.Show(this, "ServiceName is not correct!");
                    return;
                }

                if (ServerType == ServerType.LINUXSERVICE && string.IsNullOrEmpty(DeployConfig.LinuxServiveConfig.ServiceName))
                {
                    MessageBoxEx.Show(this, "ServiceName is not correct!");
                    return;
                }

                var server = Server as Server;
                if (server == null)
                {
                    MessageBoxEx.Show(this, "Server is not correct!");
                    return;
                }


                if (string.IsNullOrEmpty(server.Token))
                {
                    MessageBoxEx.Show(this, "Server Token is not correct!");
                    return;
                }

                if (string.IsNullOrEmpty(server.Host))
                {
                    MessageBoxEx.Show(this, "Server Host is not correct!");
                    return;
                }

                new Task(async () =>
                {
                    try
                    {

                        if (ServerType == ServerType.IIS)
                        {
                            Enable(false, true);
                        }
                        else if (ServerType == ServerType.LINUXSERVICE)
                        {
                            EnableForLinuxService(false, true);
                        }
                        else
                        {
                            EnableForWindowsService(false, true);
                        }
                        var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Mac = CodingHelper.GetMacAddress(),
                                Type = ServerType == ServerType.LINUXSERVICE ? "linux" : ServerType == ServerType.IIS ? "iis" : "winservice",
                                Name = ServerType == ServerType.LINUXSERVICE ? DeployConfig.LinuxServiveConfig.ServiceName : ServerType == ServerType.IIS ? DeployConfig.IIsConfig.WebSiteName : DeployConfig.WindowsServiveConfig.ServiceName,
                                WithArgs = true
                            }, ServerType == ServerType.LINUXSERVICE ? nlog_linux : ServerType == ServerType.IIS ? nlog_iis : nlog_windowservice);

                        if (getVersionResult == null)
                        {
                            ShowThreadMessageBox("get history list fail");
                            return;
                        }

                        var versionList = getVersionResult.Data;
                        if (versionList == null || versionList.Count < 1)
                        {
                            ShowThreadMessageBox("get history list count:0");
                            return;
                        }


                        this.BeginInvokeLambda(() =>
                        {
                            RollBack rolleback = new RollBack(versionList);
                            rolleback.ShowAsHistory(!string.IsNullOrEmpty(server.NickName) ? server.NickName : server.Host);
                        });
                    }
                    catch (Exception e)
                    {
                        ShowThreadMessageBox(e.Message);
                    }
                    finally
                    {
                        if (ServerType == ServerType.IIS)
                        {
                            Enable(true);
                        }
                        else if (ServerType == ServerType.LINUXSERVICE)
                        {
                            EnableForLinuxService(true);
                        }
                        else
                        {
                            EnableForWindowsService(true);
                        }

                    }

                }).Start();

            }


        }


        private void ShowThreadMessageBox(string message)
        {
            this.BeginInvokeLambda(() =>
            {
                MessageBoxEx.Show(this, message);
            });
        }

        private IWebProxy GetProxy(Logger logger)
        {
            try
            {
                if (string.IsNullOrEmpty(PluginConfig.DeployHttpProxy)) return null;
                var proxy = new WebProxy(PluginConfig.DeployHttpProxy);
                logger.Warn($"UseProxy:【{PluginConfig.DeployHttpProxy}】");
                return proxy;
            }
            catch (Exception e)
            {
                logger.Warn("UseProxy Fail:" + PluginConfig.DeployHttpProxy + ",Err:" + e.Message);
                return null;
            }
        }

        private void txt_http_proxy_TextChanged(object sender, EventArgs e)
        {
            PluginConfig.DeployHttpProxy = this.txt_http_proxy.Text.Trim();
        }

        private void checkBox_save_deploy_log_Click(object sender, EventArgs e)
        {
            GlobalConfig.SaveLogs = checkBox_save_deploy_log.Checked;
        }

        private void checkBox_multi_deploy_Click(object sender, EventArgs e)
        {
            GlobalConfig.MultiInstance = checkBox_multi_deploy.Checked;
        }

        private void Notice(string title, string message)
        {
            BeginInvokeLambda(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.iconPath))
                    {
                        notificationService.Notify(this.iconPath, title, message);
                    }
                    else
                    {
                        notificationService.Notify(title, message);
                    }

                }
                catch (Exception)
                {
                }

            });
        }

        private void btn_rich_config_log_close_Click(object sender, EventArgs e)
        {
            this.panel_rich_config_log.Visible = false;
            this.rich_config_log.Text = "";
        }

        private void chk_global_useCheckBox_Click(object sender, EventArgs e)
        {
            GlobalConfig.EnableEnvGroup = this.chk_global_useCheckBox.Checked;
            MessageBoxEx.ShowOk(this, "please reload antdeploy!");
        }

        private void checkBox_Increment_linux_service_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.LinuxServiceEnableIncrement = checkBox_Increment_linux_service.Checked;
        }

        private void checkBox_select_deploy_linuxservice_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.LinuxServiceEnableSelectDeploy = checkBox_select_deploy_linuxservice.Checked;
        }
        private void checkBox_select_type_linuxservice_CheckedChanged(object sender, EventArgs e)
        {
            PluginConfig.LinuxServiceNotifySystemd = checkBox_select_type_linuxservice.Checked;
        }
        private void btn_linux_service_retry_Click(object sender, EventArgs e)
        {
            btn_linux_service_retry.Visible = false;
            Condition.Set();
        }

        private void btn_linux_serivce_stop_Click(object sender, EventArgs e)
        {
            btn_linux_serivce_stop.Text = "Stoping";
            btn_linux_serivce_stop.Enabled = false;
            stop_linux_cancel_token = true;
            btn_linux_service_retry.Visible = false;
            Condition.Set();
        }

        private void b_linux_service_rollback_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && !_project.IsNetcorePorject)
            {
                MessageBoxEx.Show(this, Strings.NowNetcoreProject);
                return;
            }

            var serviceName = this.txt_linuxservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.ServiceNameRequired);
                return;
            }
            if (!CodingHelper.IsNatural_Number(serviceName))
            {
                MessageBoxEx.Show(this, Strings.ServiceNameMustBeNature);
                return;
            }

            DeployConfig.LinuxServiveConfig.ServiceName = serviceName;


            var envName = this.combo_linux_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.LINUXSERVICE))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.EnvHaveNoServer);
                return;
            }

            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = MessageBoxEx.Show(this,
                Strings.DeployRollBackConfirm + Environment.NewLine + serverHostList,
                Strings.RollBackConfirm,
                MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            combo_linux_env_SelectedIndexChanged(null, null);

            this.rich_linuxservice_log.Text = "";
            this.nlog_linux.Info($"linux Service name:{DeployConfig.LinuxServiveConfig.ServiceName}");
            //this.tabControl_window_service.SelectedIndex = 1;
            PrintCommonLog(this.nlog_linux);

            new Task(async () =>
            {
                try
                {
                    EnableForLinuxService(false, true);

                    var loggerId = Guid.NewGuid().ToString("N");

                    this.nlog_linux.Info($"-----------------Rollback Start[Ver:{Vsix.VERSION}]-----------------");
                    var allSuccess = true;
                    var failCount = 0;
                    foreach (var server in serverList)
                    {
                        BuildEnd(this.tabPage_linux_service, server.Host);
                        UpdatePackageProgress(this.tabPage_linux_service, server.Host, 100);
                        UpdateUploadProgress(this.tabPage_linux_service, server.Host, 100);


                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} Rollback skip,Token is null or empty!");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(server.Host))
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} Rollback fail,Server Host is Empty!");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        this.nlog_linux.Info($"Host:{getHostDisplayName(server)} Start get rollBack version list");


                        var getVersionResult = await WebUtil.HttpPostAsync<GetVersionResult>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "linux",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = DeployConfig.LinuxServiveConfig.ServiceName,
                                WithArgs = true
                            }, nlog_linux);

                        if (getVersionResult == null)
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        if (!string.IsNullOrEmpty(getVersionResult.Msg))
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} get rollBack version list fail：" + getVersionResult.Msg);
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }

                        var versionList = getVersionResult.Data;

                        if (versionList == null || versionList.Count <= 1)
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} get rollBack version list count:0");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }
                        this.nlog_linux.Info($"Host:{getHostDisplayName(server)} get rollBack version list count:{versionList.Count}");
                        this.BeginInvokeLambda(() =>
                        {
                            RollBack rolleback = new RollBack(versionList.ToList());
                            rolleback.SetTitle($"Current Server:{getHostDisplayName(server)}");
                            var r = rolleback.ShowDialog();
                            if (r == DialogResult.Cancel)
                            {
                                _rollBackVersion = null;
                            }
                            else
                            {
                                _rollBackVersion = new RollBackVersion
                                {
                                    Version = rolleback.SelectRollBackVersion
                                };
                            }
                            Condition.Set();
                        });
                        Condition.WaitOne();
                        if (_rollBackVersion == null || string.IsNullOrEmpty(_rollBackVersion.Version))
                        {
                            this.nlog_linux.Error($"Host:{getHostDisplayName(server)} Rollback canceled!");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            allSuccess = false;
                            failCount++;
                            continue;
                        }
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "linux_rollback");
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("serviceName", DeployConfig.LinuxServiveConfig.ServiceName);
                        httpRequestClient.SetFieldValue("deployFolderName", _rollBackVersion.Version);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_linux, HttpLogger);

                        var haveError = false;
                        try
                        {
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/rollback",
                                (client) => { client.Proxy = GetProxy(this.nlog_linux); });
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Rollback Fail,Skip to Next");
                                UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");
                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.LinuxServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.LinuxServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_linuxservice_log";
                                        this.nlog_linux.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.LinuxServiceFireUrl, this.nlog_linux);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                            this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            allSuccess = false;
                            failCount++;
                            this.nlog_linux.Error($"Fail Rollback,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                        }
                    }

                    this.nlog_linux.Info($"-----------------Rollback End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                    Notice("Rollback End", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                }
                catch (Exception ex1)
                {
                    this.nlog_linux.Error(ex1);
                    return;
                }
                finally
                {
                    EnableForLinuxService(true);
                }

            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }

        private void b_linuxservice_deploy_Click(object sender, EventArgs e)
        {
            stop_linux_cancel_token = false;
            Condition = new AutoResetEvent(false);

            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && !_project.IsNetcorePorject)
            {
                MessageBoxEx.Show(this, Strings.NowNetcoreProject);
                return;
            }

            var serviceName = this.txt_linuxservice_name.Text.Trim();
            if (serviceName.Length < 1)
            {
                MessageBoxEx.Show(this, Strings.ServiceNameRequired);
                return;
            }
            if (!CodingHelper.IsNatural_Number(serviceName))
            {
                MessageBoxEx.Show(this, Strings.ServiceNameMustBeNature);
                return;
            }

            DeployConfig.LinuxServiveConfig.ServiceName = serviceName;

            var linuxEnvParam = this.txt_linux_service_env.Text.Trim();
            if (linuxEnvParam.Length > 0)
            {
                DeployConfig.LinuxServiveConfig.EnvParam = linuxEnvParam;
            }
            var PhysicalPath = "";
            var ServiceStartType = "";
            var ServiceDescription = "";

            var envName = this.combo_linux_env.SelectedItem as string;
            if (string.IsNullOrEmpty(envName))
            {
                MessageBoxEx.Show(this, Strings.SelectEnv);
                return;
            }

            //fire url
            if (CheckFireUri(ServerType.LINUXSERVICE))
            {
                MessageBoxEx.Show(this, Strings.FireUrlInvaid);
                return;
            }


            var ignoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).IgnoreList;
            var backUpIgnoreList = DeployConfig.Env.First(r => r.Name.Equals(envName)).WindowsBackUpIgnoreList;

#if DEBUG

            var execFilePath = (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? serviceName : this.ProjectName);
#else
            //如果是特定文件夹发布 得选择一个exe
            if (!string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && string.IsNullOrEmpty(_project.OutPutName))
            {
                var exePath = string.Empty;
                //找当前根目录下 有没有 .deps.json 文件
                var depsJsonFile = CodingHelper.FindDepsJsonFile(PluginConfig.DeployFolderPath);
                if (!string.IsNullOrEmpty(depsJsonFile))
                {
                    var exeTempName = depsJsonFile.Replace(".deps.json", ".exe");
                    if (File.Exists(exeTempName))
                    {
                        exePath = exeTempName;
                    }
                }

                if (!string.IsNullOrEmpty(exePath))
                {
                    _project.OutPutName = new FileInfo(exePath).Name;
                }
               
            }
            var execFilePath = _project.OutPutName;
            if (string.IsNullOrEmpty(execFilePath))
            {
                execFilePath = this.ProjectName;
            }

#endif
            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBoxEx.Show(this, Strings.NoLinuxServer);
                return;
            }

            if (ProgressBox.IsEnableGroup)
            {
                //获取所有的选择了的server
                var selectedList = getSelectedBaseServers(ServerType.LINUXSERVICE);
                if (!selectedList.Any())
                {
                    MessageBoxEx.Show(this, Strings.NoLinuxServer);
                    return;
                }

                //找到选择了的
                serverList = serverList.Where(r => selectedList.Any(y => y.Host.Equals(r.Host))).ToList();
                if (!serverList.Any())
                {
                    MessageBoxEx.Show(this, Strings.NoLinuxServer);
                    return;
                }
            }
            var serverHostList = string.Join(Environment.NewLine, serverList.Select(r => r.Host).ToList());

            var confirmResult = ShowInputMsgBox(Strings.ConfirmDeploy,
                Strings.DeployServerConfim + Environment.NewLine + serverHostList);

            if (!confirmResult.Item1)
            {
                return;
            }

            combo_linux_env_SelectedIndexChanged(null, null);
            execFilePath = execFilePath.Replace(".dll", "");
            this.rich_linuxservice_log.Text = "";
            this.nlog_linux.Info($"linux Service excute name:{execFilePath}");

            if (string.IsNullOrEmpty(_project.NetCoreSDKVersion))
            {
                _project.NetCoreSDKVersion = ProjectHelper.GetProjectSkdInNetCoreProject(ProjectPath);
            }
            if (!string.IsNullOrEmpty(_project.NetCoreSDKVersion))
            {
                this.nlog_linux.Info($"DotNetSDK.Version:{_project.NetCoreSDKVersion}");
            }

            _CreateParam = new FirstCreateParam();
            new Task(async () =>
            {
                this.nlog_linux.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");


                PrintCommonLog(this.nlog_linux);
                EnableForLinuxService(false); //第一台开始编译
                GitClient gitModel = null;
                var isRuningSelectDeploy = false;
                var publishPath = !string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? PluginConfig.DeployFolderPath : Path.Combine(ProjectFolderPath, "bin", "Release", "deploy_linux", envName);
                var dateTimeFolderNameParent = string.Empty;
                try
                {
                    var runtime = "";
                    if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                    {
                        var path = publishPath + "\\";
                        
                        if (string.IsNullOrEmpty(PluginConfig.NetCorePublishMode) || PluginConfig.NetCorePublishMode == "Default")
                        {
                            runtime = " --runtime linux-x64";
                        }
                        else
                        {
                            runtime = PluginConfig.GetNetCorePublishRuntimeArg();
                        }

                        //如果runtime 为空的话 代表服务端需要用dotnet 来运行了

                        //执行 publish
                        var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                            $"publish \"{ProjectPath}\" -c Release{runtime}", nlog_linux, () => stop_linux_cancel_token);

                        if (!isSuccess)
                        {
                            this.nlog_linux.Error("publish error,please check build log");
                            BuildError(this.tabPage_linux_service, serverList.First().Host);
                            return;
                        }
                    }


                    if (string.IsNullOrEmpty(publishPath) || !Directory.Exists(publishPath))
                    {
                        this.nlog_linux.Error("can not find publishPath");
                        BuildError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }

                    var useDotnet = !string.IsNullOrEmpty(runtime) && runtime.Contains("linux");
                    execFilePath = useDotnet
                        ? execFilePath
                        : execFilePath + ".dll";

                    var serviceFile = Path.Combine(publishPath, execFilePath);
                    if (!File.Exists(serviceFile))
                    {
                        BuildError(this.tabPage_linux_service, serverList.First().Host);
                        this.nlog_linux.Error($"exe file can not find in publish folder: {serviceFile}");
                        return;
                    }


                    BuildEnd(this.tabPage_linux_service, serverList.First().Host); //第一台结束编译
                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent.LoggerName = "rich_linuxservice_log";
                    this.nlog_linux.Log(publisEvent);



                    //执行 打包
                    this.nlog_linux.Info("-----------------Start package-----------------");

                    if (stop_linux_cancel_token)
                    {
                        this.nlog_linux.Warn($"deploy task was canceled!");
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }

                    //查看是否开启了增量
                    if (this.PluginConfig.LinuxServiceEnableIncrement)
                    {
                        this.nlog_linux.Info("Enable Increment Deploy:true");
                        gitModel = new GitClient(publishPath, this.nlog_linux);
                        if (!gitModel.InitSuccess)
                        {
                            this.nlog_linux.Error(
                                "package fail,can not init git,please cancel Increment Deploy");
                            PackageError(this.tabPage_linux_service, serverList.First().Host);
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
                            PackageError(this.tabPage_linux_service, serverList.First().Host);
                            return;
                        }
                        gitChangeFileCount = fileList.Count;
                        this.nlog_linux.Info("【git】Increment package file count:" + gitChangeFileCount);

                        if (PluginConfig.LinuxServiceEnableSelectDeploy)
                        {
                            isRuningSelectDeploy = true;
                            this.nlog_linux.Info("-----------------Select File Start-----------------");
                            this.BeginInvokeLambda(() =>
                            {
                                var slectFileForm = new SelectFile(fileList, publishPath, ignoreList);
                                slectFileForm.ShowDialog();

                                //增量 选择特定文件发布
                                DoLinuxServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList, serviceName, DeployConfig.LinuxServiveConfig.EnvParam, useDotnet, execFilePath, PhysicalPath, backUpIgnoreList, gitModel, confirmResult.Item2, ignoreList);
                            });
                            return;
                        }

                        try
                        {
                            this.nlog_linux.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal,
                                true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_linux_service, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_linux_cancel_token;
                                });
                        }
                        catch (Exception ex)
                        {
                            this.nlog_linux.Error("package fail:" + ex.Message);
                            PackageError(this.tabPage_linux_service, serverList.First().Host);
                            return;
                        }
                    }
                    else if (PluginConfig.LinuxServiceEnableSelectDeploy)
                    {
                        isRuningSelectDeploy = true;
                        this.nlog_linux.Info("-----------------Select File Start-----------------");
                        this.BeginInvokeLambda(() =>
                        {
                            var slectFileForm = new SelectFile(publishPath, ignoreList);
                            slectFileForm.ShowDialog();

                            //选择特定文件发布
                            DoLinuxServiceSelectDeploy(slectFileForm.SelectedFileList, publishPath, serverList, serviceName, DeployConfig.LinuxServiveConfig.EnvParam, useDotnet, execFilePath, PhysicalPath, backUpIgnoreList, null, confirmResult.Item2, ignoreList);
                        });
                        return;
                    }

                    try
                    {
                        if (zipBytes == null)
                        {
                            this.nlog_linux.Info($"package ignoreList Count:{ignoreList.Count},backUp IgnoreList Count:{backUpIgnoreList.Count}");
                            zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true,
                                ignoreList,
                                (progressValue) =>
                                {
                                    UpdatePackageProgress(this.tabPage_linux_service, serverList.First().Host, progressValue); //打印打包记录
                                    return stop_linux_cancel_token;
                                });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.nlog_linux.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_linux.Error("package fail");
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_linux.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_linux.Info("-----------------Deploy Start-----------------");
                    dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var retryTimes = 0;
                    var allfailServerList = new List<Server>();
RETRY_WINDOWSSERVICE:
                    var failServerList = new List<Server>();
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = string.Empty;
                    var isRetry = allfailServerList.Count > 0;
                    if (isRetry)
                    {
                        dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                    }
                    else
                    {
                        dateTimeFolderName = dateTimeFolderNameParent;
                    }

                    //重试了 但是没有发现错误的Server List
                    if (retryTimes > 0 && allfailServerList.Count == 0) return;
                    foreach (var server in isRetry ? allfailServerList : serverList)
                    {
                        if (isRetry) UploadReset(this.tabPage_linux_service, server.Host);
                        if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_linux_service, server.Host);
                            UpdatePackageProgress(this.tabPage_linux_service, server.Host, 100);
                        }
                        if (stop_linux_cancel_token)
                        {
                            this.nlog_linux.Warn($"deploy task was canceled!");
                            UploadError(this.tabPage_linux_service, server.Host);
                            return;
                        }
                        index++;
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_linux.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_linux_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }
                        this.nlog_linux.Info("Start Check Linux Service IsExist In Remote Server:" + server.Host);
                        var checkResult = await WebUtil.HttpPostAsync<IIsSiteCheck>(
                            $"http://{server.Host}/version", new
                            {
                                Token = server.Token,
                                Type = "checklinux",
                                Mac = CodingHelper.GetMacAddress(),
                                Name = serviceName
                            }, nlog_linux, true);


                        if (checkResult == null || checkResult.Data == null)
                        {
                            this.nlog_linux.Error($"Check IsExist In Remote Server Fail!");
                            UploadError(this.tabPage_linux_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }

                        if (!string.IsNullOrEmpty(checkResult.Msg))
                        {
                            this.nlog_linux.Error($"Check IsExist In Remote Server Fail：" + checkResult.Msg);
                            UploadError(this.tabPage_linux_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }

                        if (checkResult.Data.Success)
                        {
                            this.nlog_linux.Info($"Linux Service Is Exist In Remote Server:" + server.Host);
                        }
                        else
                        {
                            if (this.PluginConfig.LinuxServiceEnableIncrement)
                            {
                                this.nlog_linux.Error($"Linux Service Is Not Exist In Remote Server,Can not use [Increment deplpoy]");
                                UploadError(this.tabPage_linux_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }

                            this.BeginInvokeLambda(() =>
                            {
                                //级别一不存在
                                FirstService creatFrom = new FirstService();
                                var data = creatFrom.ShowDialog();
                                if (data == DialogResult.Cancel)
                                {
                                    _CreateParam = null;
                                }
                                else
                                {
                                    _CreateParam = creatFrom.WindowsServiceCreateParam;
                                }
                                Condition.Set();
                            });
                            Condition.WaitOne();

                            if (_CreateParam == null)
                            {
                                this.nlog_linux.Error($"Create Linux Service Param Required!");
                                UploadError(this.tabPage_linux_service, server.Host);
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                continue;
                            }
                            else
                            {
                                ServiceStartType = _CreateParam.StartUp;
                                PhysicalPath = _CreateParam.PhysicalPath;
                                ServiceDescription = _CreateParam.Desc;
                                this.nlog_linux.Info($"LinuxService Create Description:{_CreateParam.Desc},StartType:{_CreateParam.StartUp},PhysicalPath:{PhysicalPath}");
                            }
                        }


                        ProgressPercentageForLinuxService = 0;
                        ProgressCurrentHostForLinuxService = server.Host;
                        this.nlog_linux.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "linux");
                        httpRequestClient.SetFieldValue("isIncrement", this.PluginConfig.LinuxServiceEnableIncrement ? "true" : "");
                        httpRequestClient.SetFieldValue("serviceName", serviceName);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                        httpRequestClient.SetFieldValue("remark", confirmResult.Item2);
                        httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                        httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                        httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("notify", this.PluginConfig.LinuxServiceNotifySystemd ? "true" : "");
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("env", DeployConfig.LinuxServiveConfig.EnvParam);
                        httpRequestClient.SetFieldValue("useDotnet", !useDotnet ? "true" : "");//true 代表需要 服务器上用dotnet xxx.dll的方式启动服务
                        httpRequestClient.SetFieldValue("startType", ServiceStartType);
                        httpRequestClient.SetFieldValue("desc", ServiceDescription);
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        IDisposable _subcribe = null;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_linux, HttpLogger);
                        var haveError = false;
                        try
                        {
                            if (stop_linux_cancel_token)
                            {
                                this.nlog_linux.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_linux_service, server.Host);
                                UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                return;
                            }

                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_linux);
                                    _subcribe = System.Reactive.Linq.Observable
                                        .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                        .Sample(TimeSpan.FromMilliseconds(100))
                                        .Subscribe(arg => { ClientOnUploadProgressChanged3(arg.Sender, arg.EventArgs); });
                                    //client.UploadProgressChanged += ClientOnUploadProgressChanged2;
                                });
                            if (ProgressPercentageForLinuxService == 0 && !uploadResult.Item1) UploadError(this.tabPage_linux_service, server.Host);
                            if ((ProgressPercentageForLinuxService > 0 && ProgressPercentageForLinuxService < 100))
                                UpdateUploadProgress(this.tabPage_linux_service, ProgressCurrentHostForLinuxService, 100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UploadError(this.tabPage_linux_service, server.Host);
                                UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    UpdateUploadProgress(this.tabPage_linux_service, ProgressCurrentHostForLinuxService, 100); //结束上传
                                    this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.LinuxServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.LinuxServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_linuxservice_log";
                                        this.nlog_linux.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.LinuxServiceFireUrl, this.nlog_linux);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                            this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            failServerList.Add(server);
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    failServerList.Add(server);
                                    this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UploadError(this.tabPage_linux_service, server.Host);
                                    UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            failServerList.Add(server);
                            allSuccess = false;
                            this.nlog_linux.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            if (stop_linux_cancel_token)
                            {
                                this.nlog_linux.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_linux_service, server.Host);
                                return;
                            }
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                            _subcribe?.Dispose();
                        }

                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_linux.Info("Deploy Version：" + dateTimeFolderNameParent);
                        if (gitModel != null) gitModel.SubmitChanges(gitChangeFileCount);
                        allfailServerList = new List<Server>();
                        Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");

                    }
                    else
                    {
                        Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                        if (!stop_linux_cancel_token)
                        {
                            allfailServerList = new List<Server>();
                            allfailServerList.AddRange(failServerList);
                            EnableLinuxServiceRetry(true);
                            //看是否要重试
                            Condition.WaitOne();
                            if (!stop_linux_cancel_token)
                            {
                                retryTimes++;
                                goto RETRY_WINDOWSSERVICE;
                            }
                        }

                    }

                    zipBytes = null;
                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent2.LoggerName = "rich_linuxservice_log";
                    this.nlog_linux.Log(publisEvent2);
                    this.nlog_linux.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");
                }
                catch (Exception ex1)
                {
                    this.nlog_linux.Error(ex1);
                }
                finally
                {
                    //记录发布日志
                    SaveLog(publishPath, dateTimeFolderNameParent, nlog_linux);

                    if (!isRuningSelectDeploy)
                    {
                        ProgressPercentageForLinuxService = 0;
                        ProgressCurrentHostForLinuxService = null;
                        EnableForLinuxService(true);
                        gitModel?.Dispose();
                    }

                }



            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }


        private void DoLinuxServiceSelectDeploy(List<string> fileList, string publishPath, List<Server> serverList, string serviceName, string envParam, bool useDotnet, 
            string execFilePath, string PhysicalPath, List<string> backUpIgnoreList, GitClient gitModel, string remark, List<string> ignoreList)
        {
            new Task(async () =>
            {
                var dateTimeFolderNameParent = string.Empty;
                try
                {
                    if (stop_linux_cancel_token)
                    {
                        this.nlog_linux.Warn($"deploy task was canceled!");
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }

                    byte[] zipBytes = null;
                    if (fileList == null || !fileList.Any())
                    {
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        this.nlog_linux.Error("Please Select Files");
                        return;
                    }
                    this.nlog_linux.Info("Select Files count:" + fileList.Count);
                    //this.nlog_linux.Debug("ignore package ignoreList");
                    this.nlog_linux.Info($"package ignoreList Count:{ignoreList.Count}, backUp IgnoreList Count:{backUpIgnoreList.Count}");
                    //List<string> ignoreList = new List<string>();
                    try
                    {
                        zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, fileList, CompressionLevel.Optimal, true,
                            ignoreList,
                            (progressValue) =>
                            {
                                UpdatePackageProgress(this.tabPage_linux_service, serverList.First().Host, progressValue); //打印打包记录
                                return stop_linux_cancel_token;
                            }, true);
                    }
                    catch (Exception ex)
                    {
                        this.nlog_linux.Error("package fail:" + ex.Message);
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }

                    if (zipBytes == null || zipBytes.Length < 1)
                    {
                        this.nlog_linux.Error("package fail");
                        PackageError(this.tabPage_linux_service, serverList.First().Host);
                        return;
                    }
                    var packageSize = (zipBytes.Length / 1024 / 1024);
                    this.nlog_linux.Info($"package success,package size:{(packageSize > 0 ? (packageSize + "") : "<1")}M");
                    var loggerId = Guid.NewGuid().ToString("N");
                    //执行 上传
                    this.nlog_linux.Info("-----------------Deploy Start-----------------");
                    dateTimeFolderNameParent = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var retryTimes = 0;
                    var allfailServerList = new List<Server>();
RETRY_WINDOWSSERVICE2:
                    var failServerList = new List<Server>();
                    var index = 0;
                    var allSuccess = true;
                    var failCount = 0;
                    var dateTimeFolderName = string.Empty;
                    var isRetry = allfailServerList.Count > 0;
                    if (isRetry)
                    {
                        dateTimeFolderName = dateTimeFolderNameParent + (retryTimes.AppendRetryStrings());
                    }
                    else
                    {
                        dateTimeFolderName = dateTimeFolderNameParent;
                    }
                    //重试了 但是没有发现错误的Server List
                    if (retryTimes > 0 && allfailServerList.Count == 0) return;
                    foreach (var server in isRetry ? allfailServerList : serverList)
                    {
                        if (isRetry) UploadReset(this.tabPage_linux_service, server.Host);
                        if (!isRetry && index != 0) //因为编译和打包只会占用第一台服务器的时间
                        {
                            BuildEnd(this.tabPage_linux_service, server.Host);
                            UpdatePackageProgress(this.tabPage_linux_service, server.Host, 100);
                        }
                        if (stop_linux_cancel_token)
                        {
                            this.nlog_linux.Warn($"deploy task was canceled!");
                            UploadError(this.tabPage_linux_service, server.Host);
                            return;
                        }
                        index++;
                        if (string.IsNullOrEmpty(server.Token))
                        {
                            this.nlog_linux.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                            UploadError(this.tabPage_linux_service, server.Host);
                            allSuccess = false;
                            failCount++;
                            failServerList.Add(server);
                            continue;
                        }


                        ProgressPercentageForLinuxService = 0;
                        ProgressCurrentHostForLinuxService = server.Host;
                        this.nlog_linux.Info($"Start Uppload,Host:{getHostDisplayName(server)}");
                        HttpRequestClient httpRequestClient = new HttpRequestClient();
                        httpRequestClient.SetFieldValue("publishType", "linux");
                        httpRequestClient.SetFieldValue("isIncrement", "true");
                        httpRequestClient.SetFieldValue("serviceName", serviceName);
                        httpRequestClient.SetFieldValue("id", loggerId);
                        httpRequestClient.SetFieldValue("execFilePath", execFilePath);
                        httpRequestClient.SetFieldValue("remark", remark);
                        httpRequestClient.SetFieldValue("notify", this.PluginConfig.LinuxServiceNotifySystemd ? "true" : "");
                        httpRequestClient.SetFieldValue("mac", CodingHelper.GetMacAddress());
                        httpRequestClient.SetFieldValue("pc", System.Environment.MachineName);
                        httpRequestClient.SetFieldValue("localIp", CodingHelper.GetLocalIPAddress());
                        httpRequestClient.SetFieldValue("deployFolderName", dateTimeFolderName);
                        httpRequestClient.SetFieldValue("physicalPath", PhysicalPath);
                        httpRequestClient.SetFieldValue("env", envParam);
                        httpRequestClient.SetFieldValue("useDotnet", !useDotnet ? "true" : "");//true 代表需要 服务器上用dotnet xxx.dll的方式启动服务
                        httpRequestClient.SetFieldValue("Token", server.Token);
                        httpRequestClient.SetFieldValue("backUpIgnore", (backUpIgnoreList != null && backUpIgnoreList.Any()) ? string.Join("@_@", backUpIgnoreList) : "");
                        httpRequestClient.SetFieldValue("publish", "publish.zip", "application/octet-stream", zipBytes);
                        HttpLogger HttpLogger = new HttpLogger
                        {
                            Key = loggerId,
                            Url = $"http://{server.Host}/logger?key=" + loggerId
                        };
                        IDisposable _subcribe = null;
                        WebSocketClient webSocket = new WebSocketClient(this.nlog_linux, HttpLogger);
                        var haveError = false;
                        try
                        {
                            if (stop_linux_cancel_token)
                            {
                                this.nlog_linux.Warn($"deploy task was canceled!");
                                UploadError(this.tabPage_linux_service, server.Host);
                                UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                return;
                            }
                            var hostKey = await webSocket.Connect($"ws://{server.Host}/socket");
                            httpRequestClient.SetFieldValue("wsKey", hostKey);

                            var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish",
                                (client) =>
                                {
                                    client.Proxy = GetProxy(this.nlog_linux);
                                    _subcribe = System.Reactive.Linq.Observable
                                        .FromEventPattern<UploadProgressChangedEventArgs>(client, "UploadProgressChanged")
                                        .Sample(TimeSpan.FromMilliseconds(100))
                                        .Subscribe(arg => { ClientOnUploadProgressChanged3(arg.Sender, arg.EventArgs); });
                                    //client.UploadProgressChanged += ClientOnUploadProgressChanged2;
                                });
                            if (ProgressPercentageForLinuxService == 0 && !uploadResult.Item1) UploadError(this.tabPage_linux_service, server.Host);
                            if ((ProgressPercentageForLinuxService > 0 && ProgressPercentageForLinuxService < 100))
                                UpdateUploadProgress(this.tabPage_linux_service, ProgressCurrentHostForLinuxService, 100); //结束上传
                            webSocket.ReceiveHttpAction(true);
                            haveError = webSocket.HasError;
                            if (haveError)
                            {
                                allSuccess = false;
                                failCount++;
                                failServerList.Add(server);
                                this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Deploy Fail,Skip to Next");
                                UploadError(this.tabPage_linux_service, server.Host);
                                UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                            }
                            else
                            {
                                if (uploadResult.Item1)
                                {
                                    UpdateUploadProgress(this.tabPage_linux_service, ProgressCurrentHostForLinuxService, 100); //结束上传

                                    this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2}");

                                    //fire the website
                                    if (!string.IsNullOrEmpty(server.LinuxServiceFireUrl))
                                    {
                                        LogEventInfo publisEvent22 = new LogEventInfo(LogLevel.Info, "", "Start to Fire Url,TimeOut：10senconds  ==> ");
                                        publisEvent22.Properties["ShowLink"] = server.LinuxServiceFireUrl;
                                        publisEvent22.LoggerName = "rich_linuxservice_log";
                                        this.nlog_linux.Log(publisEvent22);

                                        var fireRt = WebUtil.IsHttpGetOk(server.LinuxServiceFireUrl, this.nlog_linux);
                                        if (fireRt)
                                        {
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                            this.nlog_linux.Info($"Host:{getHostDisplayName(server)},Success Fire Url");
                                        }
                                        else
                                        {
                                            failCount++;
                                            failServerList.Add(server);
                                            allSuccess = false;
                                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                        }
                                    }
                                    else
                                    {
                                        UpdateDeployProgress(this.tabPage_linux_service, server.Host, true);
                                    }
                                }
                                else
                                {
                                    allSuccess = false;
                                    failCount++;
                                    failServerList.Add(server);
                                    this.nlog_linux.Error($"Host:{getHostDisplayName(server)},Response:{uploadResult.Item2},Skip to Next");
                                    UploadError(this.tabPage_linux_service, server.Host);
                                    UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            failCount++;
                            failServerList.Add(server);
                            allSuccess = false;
                            this.nlog_linux.Error(
                                $"Fail Deploy,Host:{getHostDisplayName(server)},Response:{ex.Message},Skip to Next");
                            UpdateDeployProgress(this.tabPage_linux_service, server.Host, false);
                        }
                        finally
                        {
                            await webSocket?.Dispose();
                            _subcribe?.Dispose();
                        }

                    }

                    //交互
                    if (allSuccess)
                    {
                        this.nlog_linux.Info("Deploy Version：" + dateTimeFolderNameParent);
                        if (gitModel != null) gitModel.SubmitSelectedChanges(fileList, publishPath);
                        allfailServerList = new List<Server>();
                        Notice("Deploy Success", $"[Total]:{serverList.Count},[Fail]:{failCount}");

                    }
                    else
                    {
                        Notice("Deploy End With Error", $"[Total]:{serverList.Count},[Fail]:{failCount}");
                        if (!stop_linux_cancel_token)
                        {
                            allfailServerList = new List<Server>();
                            allfailServerList.AddRange(failServerList);
                            EnableLinuxServiceRetry(true);
                            //看是否要重试
                            Condition.WaitOne();
                            if (!stop_linux_cancel_token)
                            {
                                retryTimes++;
                                goto RETRY_WINDOWSSERVICE2;
                            }
                        }

                    }

                    zipBytes = null;
                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "local publish folder  ==> ");
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent2.LoggerName = "rich_linuxservice_log";
                    this.nlog_linux.Log(publisEvent2);
                    this.nlog_linux.Info($"-----------------Deploy End,[Total]:{serverList.Count},[Fail]:{failCount}-----------------");

                }
                catch (Exception ex1)
                {
                    this.nlog_linux.Error(ex1);
                }
                finally
                {
                    //记录发布日志
                    SaveLog(publishPath, dateTimeFolderNameParent, nlog_linux);
                    ProgressPercentageForLinuxService = 0;
                    ProgressCurrentHostForLinuxService = null;
                    EnableForLinuxService(true);
                    gitModel?.Dispose();
                }
            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }

        private void dockerImageParams()
        {
            DeployConfig.DockerImageConfig.BaseImage = this.txt_BaseImage.Text;
            DeployConfig.DockerImageConfig.BaseImageCredential = new ImageCredential
            {
                UserName = this.txt_BaseImage_username.Text,
                Password = this.txt_BaseImage_pwd.Text
            };
            DeployConfig.DockerImageConfig.BaseHttpProxy = this.txt_HttpProxy.Text;
            DeployConfig.DockerImageConfig.TargetImage = this.txt_TargetImage.Text;
            DeployConfig.DockerImageConfig.TargetImageCredential = new ImageCredential
            {
                UserName = this.txt_TargetImage_username.Text,
                Password = this.txt_TargetImage_pwd.Text
            };
            DeployConfig.DockerImageConfig.TargetTags = new string[] { this.txt_TargetImage_tag.Text };
            DeployConfig.DockerImageConfig.ImageFormat = this.cmbo_ImageFormat.Text;
            DeployConfig.DockerImageConfig.TargetHttpProxy = this.txt_TargetHttpProxy.Text;
            DeployConfig.DockerImageConfig.Entrypoint = (this.txt_Entrypoint.Text ?? string.Empty).Split(new string[] { "->" }, StringSplitOptions.None).ToArray();
            DeployConfig.DockerImageConfig.Cmd = (this.txt_Cmd.Text ?? string.Empty).Split(new string[] { "->" }, StringSplitOptions.None).ToArray();



        }
        /// <summary>
        /// 镜像发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void altoButton1_Click(object sender, EventArgs e)
        {
            stop_docker_image_cancel_token = false;
            Condition = new AutoResetEvent(false);
            dockerImageParams();


            if (string.IsNullOrEmpty(DeployConfig.DockerImageConfig.TargetImage))
            {
                MessageBoxEx.Show(this, "TargetImage invaild");
                return;
            }
            if (string.IsNullOrEmpty(DeployConfig.DockerImageConfig.BaseImage))
            {
                MessageBoxEx.Show(this, "BaseImage invaild");
                return;
            }
            if (string.IsNullOrEmpty(this.txt_TargetImage_tag.Text))
            {
                MessageBoxEx.Show(this, "TargetTag invaild");
                return;
            }
            if (string.IsNullOrEmpty(DeployConfig.DockerImageConfig.ImageFormat))
            {
                DeployConfig.DockerImageConfig.ImageFormat = "Docker";
            }
            if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath) && !ProjectHelper.CheckDockerFileIsSetCopy(ProjectPath))
            {
                var confirmDockerfile = ShowInputMsgBox(Strings.DockerFileWarn,
                    Strings.DockerFileNotSetCopy, "hide");
                if (!confirmDockerfile.Item1)
                {
                    return;
                }
            }
            this.rich_docker_image_log.Text = "";


            new Task(() =>
            {

                this.nlog_image.Info($"-----------------Start publish[Ver:{Vsix.VERSION}]-----------------");
                PrintCommonLog(this.nlog_image);
                EnableForDockerImage(false);
                var publishPath = !string.IsNullOrEmpty(PluginConfig.DeployFolderPath) ? PluginConfig.DeployFolderPath : Path.Combine(ProjectFolderPath, "bin", "Release", "dockerImage", "publish");
                try
                {

                    if (string.IsNullOrEmpty(PluginConfig.DeployFolderPath))
                    {
                        var path = publishPath + "\\";
                        //执行 publish
                        var isSuccess = CommandHelper.RunDotnetExe(ProjectPath, ProjectFolderPath, path.Replace("\\\\", "\\"),
                            $"publish \"{ProjectPath}\" -c Release{PluginConfig.GetNetCorePublishRuntimeArg()}", nlog_image, () => stop_docker_image_cancel_token);

                        if (!isSuccess)
                        {
                            this.nlog_image.Error("publish error,please check build log");
                            return;
                        }
                    }



                    LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish target  ==> ");
                    publisEvent.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent.LoggerName = "rich_docker_image_log";
                    this.nlog_image.Log(publisEvent);


                    this.nlog_image.Info("-----------------Start publish docker image-----------------");

                    if (stop_docker_image_cancel_token)
                    {
                        this.nlog_image.Warn($"deploy task was canceled!");
                        return;
                    }



                    //执行 上传
                    this.nlog_image.Info("-----------------Deploy Start-----------------");


                    var publishSuccess = CommandHelper.RunJibExe(publishPath, _project.DomainPath, DeployConfig.DockerImageConfig,
                        nlog_image, () => stop_docker_image_cancel_token);

                    if (!publishSuccess)
                    {
                        Notice("Error", "Push Image Error");
                        this.nlog_image.Error("publish error,please check publish log");
                        return;
                    }

                    Notice("Success", "Push Image Success");
                    LogEventInfo publisEvent2 = new LogEventInfo(LogLevel.Info, "", "publish target  ==> ");
                    publisEvent2.Properties["ShowLink"] = "file://" + publishPath.Replace("\\", "\\\\");
                    publisEvent2.LoggerName = "rich_docker_image_log";
                    this.nlog_image.Log(publisEvent2);
                    this.nlog_image.Info($"-----------------Deploy End-----------------");

                }
                catch (Exception ex1)
                {
                    Notice("Error", "Push Image Error");
                    this.nlog_image.Error(ex1);
                }
                finally
                {
                    //记录发布日志
                    SaveLog(publishPath, DateTime.Now.ToString("yyyyMMddHHmmss"), nlog_image);
                    EnableForDockerImage(true);
                }



            }, System.Threading.Tasks.TaskCreationOptions.LongRunning).Start();
        }

        /// <summary>
        /// 停止镜像发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_docker_image_stop_Click(object sender, EventArgs e)
        {
            btn_docker_image_stop.Text = "Stoping";
            btn_docker_image_stop.Enabled = false;
            stop_docker_image_cancel_token = true;
            Condition.Set();
        }

        private void btn_dockerImage_ignore_add_Click(object sender, EventArgs e)
        {
            var ignoreTxt = this.txt_dockerImage_ignore.Text.Trim();
            if (ignoreTxt.Length < 1)
            {
                MessageBoxEx.Show(this, "please input ignore rule");
                return;
            }

            if (ignoreTxt.Contains("@_@"))
            {
                MessageBoxEx.Show(this, "can not contains @_@");
                return;
            }

            var existIgnore = this.list_dockerImage_ignore.Items.Cast<string>().FirstOrDefault(r => r.Equals(ignoreTxt));
            if (!string.IsNullOrEmpty(existIgnore))
            {
                this.list_dockerImage_ignore.SelectedItem = existIgnore;
            }
            else
            {
                if (this.DeployConfig.DockerImageConfig.IgnoreList == null)
                    this.DeployConfig.DockerImageConfig.IgnoreList = new List<string>();
                this.DeployConfig.DockerImageConfig.IgnoreList.Add(ignoreTxt);
                this.list_dockerImage_ignore.Items.Add(ignoreTxt);
                this.txt_dockerImage_ignore.Text = string.Empty;
                this.list_dockerImage_ignore.SelectedItem = ignoreTxt;
            }
        }

        private void btn_dockerImage_ignore_remove_Click(object sender, EventArgs e)
        {
            if (this.list_dockerImage_ignore.SelectedIndex < 0) return;
            this.DeployConfig.DockerImageConfig.IgnoreList.RemoveAt(this.list_dockerImage_ignore.SelectedIndex);
            this.list_dockerImage_ignore.Items.RemoveAt(this.list_dockerImage_ignore.SelectedIndex);
            if (this.list_dockerImage_ignore.Items.Count >= 0)
                this.list_dockerImage_ignore.SelectedIndex = this.list_dockerImage_ignore.Items.Count - 1;
        }

        private void chk_use_AsiaShanghai_timezone_Click(object sender, EventArgs e)
        {
            GlobalConfig.UseAsiaShanghai = this.chk_use_AsiaShanghai_timezone.Checked;
        }

        private void btn_auto_find_msbuild_Click(object sender, EventArgs e)
        {
            var msbuildPath = CommandHelper.GetMsBuildPath();
            if (!string.IsNullOrEmpty(msbuildPath))
            {
                this.txt_msbuild_path.Text = msbuildPath;
                CommandHelper.MsBuildPath = GlobalConfig.MsBuildPath = msbuildPath;
            }
            else
            {
                MessageBoxEx.Show(this, "Find Msbuild fail");
            }
        }

        private void label_how_to_linuxservice_Click_1(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://www.cnblogs.com/yudongdong/p/14017569.html");
            Process.Start(sInfo);
        }

        private void label_how_to_dockerimage_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://mp.weixin.qq.com/s/ga3nCq-DNkqVER8eYCooXA");
            Process.Start(sInfo);
        }


        private void Deploy_SysBottomClick(object sender, CCWin.SkinControl.SysButtonEventArgs e)
        {
            if (e.SysButton.Name == "btn_question")
            {
                About about = new About();
                about.ShowDialog();
            }
            else if (e.SysButton.Name == "btn_open_new")
            {
                this.Deploy_InitLoad(this.ProjectPath, new ProjectParam { OpenNewWindow = true}, false);
                this.combo_iis_env_SelectedIndexChanged(null, null);
            }
        }

        private void chk_global_saveconfig_in_projectFolder_Click(object sender, EventArgs e)
        {
            GlobalConfig.EnableAntDeployJson = this.chk_global_saveconfig_in_projectFolder.Checked;
            saveAntDeployJson();
        }

        private void saveAntDeployJson()
        {
            if (!string.IsNullOrEmpty(ProjectConfigPath.Item1))
            {
                var configJson = JsonConvert.SerializeObject(DeployConfig, Formatting.Indented);
                File.WriteAllText(ProjectConfigPath.Item1, configJson, Encoding.UTF8);
                if (GlobalConfig.EnableAntDeployJson && !string.IsNullOrEmpty(ProjectConfigPath.Item2)) File.WriteAllText(ProjectConfigPath.Item2, configJson, Encoding.UTF8);
            }
        }
    }
}
