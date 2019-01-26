using AntDeploy.Models;
using AntDeploy.Util;
using Newtonsoft.Json;
using NLog.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace AntDeploy.Winform
{
    public partial class Deploy : Form
    {
        private string ProjectConfigPath;
        private string ProjectFolderPath;
        private NLog.Logger Logger;
        public Deploy(string projectPath)
        {
            InitializeComponent();

            ReadPorjectConfig(projectPath);

            Logger = NLog.LogManager.GetCurrentClassLogger();
            RichTextBoxTarget.ReInitializeAllTextboxes(this);
        }





        public DeployConfig DeployConfig { get; set; }


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




            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;

            RichTextBoxTarget.GetTargetByControl(rich_iis_log).LinkClicked += LinkClicked;
        }

        private void LinkClicked(RichTextBoxTarget sender, string linktext, LogEventInfo logevent)
        {
            BeginInvokeLambda(() =>
                {
                    try
                    {
                        if (linktext.StartsWith("http")||linktext.StartsWith("file:"))
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

        private void DeployConfigOnEnvChangeEvent(Env changeEnv, bool isremove)
        {
            this.combo_iis_env.Items.Clear();
            if (DeployConfig.Env.Any())
            {
                foreach (var env in DeployConfig.Env)
                {
                    this.combo_iis_env.Items.Add(env.Name);
                }
            }

        }

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
                this.txt_env_name.Text = string.Empty;
            }
        }

        private void combo_env_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.combo_env_server_list.Items.Clear();
            var selectedEnv = this.combo_env_list.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedEnv))
            {
                this.b_env_server_add.Enabled = false;
                this.b_env_server_remove.Enabled = false;
                this.b_env_server_test.Enabled = false;
                this.txt_env_server_host.Text = string.Empty;
                this.txt_env_server_token.Text = string.Empty;
                return;
            }

            this.b_env_server_add.Enabled = true;
            this.b_env_server_remove.Enabled = true;
            this.b_env_server_test.Enabled = true;

            var env = this.DeployConfig.Env.FirstOrDefault(r => r.Name.Equals(selectedEnv));

            if (env != null)
            {
                // ReSharper disable once CoVariantArrayConversion
                this.combo_env_server_list.Items.AddRange(items: env.ServerList.Select(r => r.Host + "@_@" + r.Token).ToArray());
            }
            if (this.combo_env_server_list.Items.Count > 0) this.combo_env_server_list.SelectedIndex = 0;

        }

        private void combo_env_server_list_SelectedIndexChanged(object sender, EventArgs e)
        {

            this.txt_env_server_host.Text = string.Empty;
            this.txt_env_server_token.Text = string.Empty;
            var seletedServer = this.combo_env_server_list.SelectedItem as string;
            if (string.IsNullOrEmpty(seletedServer))
            {
                return;
            }

            var arr = seletedServer.Split(new string[] { "@_@" }, StringSplitOptions.None);
            if (arr.Length == 2)
            {
                this.txt_env_server_host.Text = arr[0];
                this.txt_env_server_token.Text = arr[1];
            }

        }

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
                MessageBox.Show("input server host is exist!");
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

        private void b_env_ignore_remove_Click(object sender, EventArgs e)
        {
            if (this.list_env_ignore.SelectedIndex < 0) return;
            DeployConfig.IgnoreList.RemoveAt(this.list_env_ignore.SelectedIndex);
            this.list_env_ignore.Items.RemoveAt(this.list_env_ignore.SelectedIndex);
            if (this.list_env_ignore.Items.Count >= 0)
                this.list_env_ignore.SelectedIndex = this.list_env_ignore.Items.Count - 1;
        }

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
            }
        }

        private void Deploy_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeployConfig.IIsConfig.WebSiteName = this.txt_iis_web_site_name.Text;
            var configJson = JsonConvert.SerializeObject(DeployConfig, Formatting.Indented);
            File.WriteAllText(ProjectConfigPath, configJson);
        }

        private void ReadPorjectConfig(string projectPath)
        {
            if (File.Exists(projectPath))
            {
                ProjectFolderPath = new FileInfo(projectPath).DirectoryName;
                ProjectConfigPath = Path.Combine(ProjectFolderPath, "AntDeploy.json");
                if (File.Exists(ProjectConfigPath))
                {
                    var config = File.ReadAllText(ProjectConfigPath);
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

        private void b_iis_deploy_Click(object sender, EventArgs e)
        {
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

            var serverList = DeployConfig.Env.Where(r => r.Name.Equals(envName)).Select(r => r.ServerList)
                .FirstOrDefault();

            if (serverList == null || !serverList.Any())
            {
                MessageBox.Show("select env have no server set yet!");
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

            this.rich_iis_log.Text = "";
            DeployConfig.IIsConfig.WebSiteName = this.txt_iis_web_site_name.Text;
            new Task(async () =>
            {
                this.Logger.Info("Start publish");
                Enable(false);
                var publishLog = new List<string>();
                //执行 publish
                var isSuccess = CommandHelper.RunDotnetExternalExe(ProjectFolderPath,
                    "publish -c Release",
                    (r) =>
                    {
                        this.Logger.Info(r);
                        publishLog.Add(r);
                    },
                    (er) => this.Logger.Error(er));

                if (!isSuccess)
                {
                    this.Logger.Error("publish error");
                    return;
                }

                var publishPathLine = publishLog
                    .FirstOrDefault(r => !string.IsNullOrEmpty(r) && r.EndsWith("\\publish\\"));

                if (string.IsNullOrEmpty(publishPathLine))
                {
                    this.Logger.Error("can not find publishPath in log");
                    return;
                }

                var publishPathArr = publishPathLine.Split(new string[] { " ->" }, StringSplitOptions.None);
                if (publishPathArr.Length != 2)
                {
                    this.Logger.Error("can not find publishPath in log");
                    return;
                }

                var publishPath = publishPathArr[1].Trim();

                LogEventInfo publisEvent = new LogEventInfo(LogLevel.Info, "", "publish success,  ==> ");
                publisEvent.Properties["ShowLink"] = "file://"+publishPath.Replace("\\","\\\\");
                this.Logger.Log(publisEvent);



                //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis

                if (DeployConfig.IIsConfig.SdkType.Equals("netcore"))
                {
                    var webConfig = Path.Combine(publishPath, "Web.Config");
                    if (!File.Exists(webConfig))
                    {
                        LogEventInfo theEvent = new LogEventInfo(LogLevel.Warn, "", "publish sdkType:netcore ,but web.config file missing!");
                        theEvent.Properties["ShowLink"] = "https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis";
                        this.Logger.Log(theEvent);
                    }
                }


                //执行 打包
                this.Logger.Info("Start package");
                byte[] zipBytes;
                try
                {
                    zipBytes = ZipHelper.DoCreateFromDirectory(publishPath, CompressionLevel.Optimal, true,DeployConfig.IgnoreList);
                }
                catch (Exception ex)
                {
                    this.Logger.Error("package fail:" + ex.Message);
                    return;
                }

                if (zipBytes == null || zipBytes.Length < 1)
                {
                    this.Logger.Error("package fail");
                    return;
                }
                this.Logger.Info("package success");

                //执行 上传
                this.Logger.Info("Deploy Start");
                foreach (var server in serverList)
                {
                    if (string.IsNullOrEmpty(server.Token))
                    {
                        this.Logger.Warn($"{server.Host} Deploy skip,Token is null or empty!");
                        continue;
                    }

                    ProgressPercentage = 0;
                    this.Logger.Info($"Start Uppload,Host:{server.Host}");
                    HttpRequestClient httpRequestClient = new HttpRequestClient();
                    httpRequestClient.SetFieldValue("publishType", "iis");
                    httpRequestClient.SetFieldValue("sdkType", DeployConfig.IIsConfig.SdkType);
                    httpRequestClient.SetFieldValue("webSiteName", DeployConfig.IIsConfig.WebSiteName);
                    httpRequestClient.SetFieldValue("Token", server.Token);
                    httpRequestClient.SetFieldValue("publish","publish.zip", "application/octet-stream", zipBytes);
                    try
                    {
                        var uploadResult = await httpRequestClient.Upload($"http://{server.Host}/publish", (client) =>
                        {
                           client.UploadProgressChanged += ClientOnUploadProgressChanged;
                        });

                        if (uploadResult.Item1)
                        {
                            this.Logger.Info($"End Uppload,Host:{server.Host},Response:{uploadResult.Item2}");
                        }
                        else
                        {
                            this.Logger.Error($"Fail Uppload,Host:{server.Host},Response:{uploadResult.Item2},Skip to Next");
                        }
                    }
                    catch (Exception ex)
                    {

                        this.Logger.Error($"Fail Uppload,Host:{server.Host},Response:{ex.Message},Skip to Next");
                    }
                    
                }
                this.Logger.Info("Deploy End");
                //交互

                Enable(true);

            }).Start();

        }

        private int ProgressPercentage = 0;
        private void ClientOnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > ProgressPercentage)
            {
                ProgressPercentage = e.ProgressPercentage;
                this.Logger.Info($"Upload {(e.ProgressPercentage != 100 ? e.ProgressPercentage * 2 : e.ProgressPercentage)} % complete...");
            }
        }


        private void Enable(bool flag)
        {
            this.BeginInvokeLambda(() =>
            {
                this.b_iis_deploy.Enabled = flag;
                this.txt_iis_web_site_name.Enabled = flag;
                this.combo_iis_env.Enabled = flag;
                this.combo_iis_sdk_type.Enabled = flag;
                this.page_set.Enabled = flag;
                this.page_docker.Enabled = flag;
                this.page_window_service.Enabled = flag;
            });

        }

        private void BeginInvokeLambda(Action action)
        {
            BeginInvoke(action, null);
        }

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
    }
}
