using System;
using System.Windows.Forms;

namespace AntDeployWinform.Models
{
    public enum ServerType
    {
        IIS,
        DOCKER,
        WINSERVICE
    }
    public class ProgressBox : GroupBox
    {
        public CircularProgressBar.CircularProgressBar progress_iis_build;
        public CircularProgressBar.CircularProgressBar progress_iis_package;
        public CircularProgressBar.CircularProgressBar progress_iis_upload;
        public CircularProgressBar.CircularProgressBar progress_iis_deploy;
        private System.Windows.Forms.Button b_build_end;
        private System.Windows.Forms.Button b_package_end;
        private System.Windows.Forms.Button b_upload_end;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox FireUrlText;

        public ServerType ServerType { get; set; }
        public BaseServer Server { get; set; }
        public ProgressBox(System.Drawing.Point location, BaseServer server, ServerType serverType)
        {

            this.ServerType = serverType;
            this.Server = server;
            this.Location = location;
            var Progressheight = 36;
            var buttonHeight = Progressheight + 31;
            this.Size = new System.Drawing.Size(546, 130);
            this.TabStop = false;
            this.SuspendLayout();

            this.label29 = new System.Windows.Forms.Label();
            this.progress_iis_build = new CircularProgressBar.CircularProgressBar();
            this.progress_iis_package = new CircularProgressBar.CircularProgressBar();
            this.progress_iis_upload = new CircularProgressBar.CircularProgressBar();
            this.progress_iis_deploy = new CircularProgressBar.CircularProgressBar();
            this.b_build_end = new System.Windows.Forms.Button();
            this.b_package_end = new System.Windows.Forms.Button();
            this.b_upload_end = new System.Windows.Forms.Button();
            this.FireUrlText = new System.Windows.Forms.TextBox();
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(42, Progressheight - 15);
            this.label29.Size = new System.Drawing.Size(107, 12);
            this.label29.Text = "Fire Url:";

            // 
            // txt_iis_website_url
            // 
            this.FireUrlText.Location = new System.Drawing.Point(120, Progressheight - 20);
            this.FireUrlText.Size = new System.Drawing.Size(322, 21);

            if (serverType.Equals(ServerType.IIS))
            {
                this.FireUrlText.Text = server.IIsFireUrl;
            }
            if (serverType.Equals(ServerType.DOCKER))
            {
                this.FireUrlText.Text = server.DockerFireUrl;
            }
            if (serverType.Equals(ServerType.WINSERVICE))
            {
                this.FireUrlText.Text = server.WindowsServiceFireUrl;
            }
            // 
            // progress_iis_build
            // 
            this.progress_iis_build.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progress_iis_build.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progress_iis_build.AnimationSpeed = 500;
            this.progress_iis_build.BackColor = System.Drawing.Color.Transparent;
            this.progress_iis_build.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.progress_iis_build.ForeColor = System.Drawing.Color.Red;
            this.progress_iis_build.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.progress_iis_build.InnerMargin = 0;
            this.progress_iis_build.InnerWidth = -1;
            this.progress_iis_build.Location = new System.Drawing.Point(42, Progressheight);
            this.progress_iis_build.MarqueeAnimationSpeed = 2000;
            this.progress_iis_build.OuterColor = System.Drawing.Color.Gray;
            this.progress_iis_build.OuterMargin = -25;
            this.progress_iis_build.OuterWidth = 22;
            this.progress_iis_build.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_build.ProgressWidth = 15;
            this.progress_iis_build.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.progress_iis_build.Size = new System.Drawing.Size(84, 77);
            this.progress_iis_build.StartAngle = 270;
            this.progress_iis_build.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_build.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.progress_iis_build.SubscriptText = "";
            this.progress_iis_build.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_build.SuperscriptMargin = new System.Windows.Forms.Padding(-23, 0, 0, 0);
            this.progress_iis_build.SuperscriptText = "Wait";
            this.progress_iis_build.TabIndex = 9;
            this.progress_iis_build.Text = "Build";
            this.progress_iis_build.TextMargin = new System.Windows.Forms.Padding(10, 8, 0, 0);
            this.progress_iis_build.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress_iis_build.Value = 0;
            // 
            // progress_iis_package
            // 
            this.progress_iis_package.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progress_iis_package.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progress_iis_package.AnimationSpeed = 500;
            this.progress_iis_package.BackColor = System.Drawing.Color.Transparent;
            this.progress_iis_package.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.progress_iis_package.ForeColor = System.Drawing.Color.Red;
            this.progress_iis_package.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.progress_iis_package.InnerMargin = 0;
            this.progress_iis_package.InnerWidth = -1;
            this.progress_iis_package.Location = new System.Drawing.Point(162, Progressheight);
            this.progress_iis_package.MarqueeAnimationSpeed = 2000;
            this.progress_iis_package.OuterColor = System.Drawing.Color.Gray;
            this.progress_iis_package.OuterMargin = -25;
            this.progress_iis_package.OuterWidth = 22;
            this.progress_iis_package.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_package.ProgressWidth = 15;
            this.progress_iis_package.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.progress_iis_package.Size = new System.Drawing.Size(84, 77);
            this.progress_iis_package.StartAngle = 270;
            this.progress_iis_package.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_package.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.progress_iis_package.SubscriptText = "";
            this.progress_iis_package.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_package.SuperscriptMargin = new System.Windows.Forms.Padding(-26, 0, 0, 0);
            this.progress_iis_package.SuperscriptText = "0%";
            this.progress_iis_package.TabIndex = 10;
            this.progress_iis_package.Text = "Package";
            this.progress_iis_package.TextMargin = new System.Windows.Forms.Padding(10, 8, 0, 0);
            this.progress_iis_package.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress_iis_package.Value = 0;
            // 
            // progress_iis_upload
            // 
            this.progress_iis_upload.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progress_iis_upload.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progress_iis_upload.AnimationSpeed = 500;
            this.progress_iis_upload.BackColor = System.Drawing.Color.Transparent;
            this.progress_iis_upload.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.progress_iis_upload.ForeColor = System.Drawing.Color.Red;
            this.progress_iis_upload.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.progress_iis_upload.InnerMargin = 0;
            this.progress_iis_upload.InnerWidth = -1;
            this.progress_iis_upload.Location = new System.Drawing.Point(282, Progressheight);
            this.progress_iis_upload.MarqueeAnimationSpeed = 2000;
            this.progress_iis_upload.OuterColor = System.Drawing.Color.Gray;
            this.progress_iis_upload.OuterMargin = -25;
            this.progress_iis_upload.OuterWidth = 22;
            this.progress_iis_upload.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_upload.ProgressWidth = 15;
            this.progress_iis_upload.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.progress_iis_upload.Size = new System.Drawing.Size(84, 77);
            this.progress_iis_upload.StartAngle = 270;
            this.progress_iis_upload.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_upload.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.progress_iis_upload.SubscriptText = "";
            this.progress_iis_upload.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_upload.SuperscriptMargin = new System.Windows.Forms.Padding(-26, 0, 0, 0);
            this.progress_iis_upload.SuperscriptText = "0%";
            this.progress_iis_upload.TabIndex = 11;
            this.progress_iis_upload.Text = "Upload";
            this.progress_iis_upload.TextMargin = new System.Windows.Forms.Padding(10, 8, 0, 0);
            this.progress_iis_upload.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress_iis_upload.Value = 0;
            // 
            // progress_iis_deploy
            // 
            this.progress_iis_deploy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.progress_iis_deploy.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.progress_iis_deploy.AnimationSpeed = 500;
            this.progress_iis_deploy.BackColor = System.Drawing.Color.Transparent;
            this.progress_iis_deploy.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.progress_iis_deploy.ForeColor = System.Drawing.Color.Red;
            this.progress_iis_deploy.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.progress_iis_deploy.InnerMargin = 0;
            this.progress_iis_deploy.InnerWidth = -1;
            this.progress_iis_deploy.Location = new System.Drawing.Point(402, Progressheight);
            this.progress_iis_deploy.MarqueeAnimationSpeed = 2000;
            this.progress_iis_deploy.OuterColor = System.Drawing.Color.Gray;
            this.progress_iis_deploy.OuterMargin = -25;
            this.progress_iis_deploy.OuterWidth = 22;
            this.progress_iis_deploy.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_deploy.ProgressWidth = 15;
            this.progress_iis_deploy.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.progress_iis_deploy.Size = new System.Drawing.Size(92, 77);
            this.progress_iis_deploy.StartAngle = 270;
            this.progress_iis_deploy.SubscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_deploy.SubscriptMargin = new System.Windows.Forms.Padding(0);
            this.progress_iis_deploy.SubscriptText = "";
            this.progress_iis_deploy.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            this.progress_iis_deploy.SuperscriptMargin = new System.Windows.Forms.Padding(-28, 0, 0, 0);
            this.progress_iis_deploy.SuperscriptText = "Wait";
            this.progress_iis_deploy.TabIndex = 12;
            this.progress_iis_deploy.Text = "Deploy";
            this.progress_iis_deploy.TextMargin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this.progress_iis_deploy.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progress_iis_deploy.Value = 0;
            // 
            // b_build_end
            // 
            this.b_build_end.Location = new System.Drawing.Point(113, buttonHeight);
            this.b_build_end.BackColor = System.Drawing.Color.LightGray;
            this.b_build_end.Size = new System.Drawing.Size(76, 19);
            this.b_build_end.TabIndex = 17;
            this.b_build_end.UseVisualStyleBackColor = true;

            // 
            // b_package_end
            // 
            this.b_package_end.Location = new System.Drawing.Point(234, buttonHeight);
            this.b_package_end.BackColor = System.Drawing.Color.LightGray;
            this.b_package_end.Size = new System.Drawing.Size(76, 19);
            this.b_package_end.TabIndex = 18;
            this.b_package_end.UseVisualStyleBackColor = true;
            // 
            // b_upload_end
            // 
            this.b_upload_end.Location = new System.Drawing.Point(358, buttonHeight);
            this.b_upload_end.BackColor = System.Drawing.Color.LightGray;
            this.b_upload_end.Size = new System.Drawing.Size(76, 19);
            this.b_upload_end.TabIndex = 19;
            this.b_upload_end.UseVisualStyleBackColor = true;


            // 
            // groupBox_iis_progress
            // 
            this.Controls.Add(this.label29);
            this.Controls.Add(this.FireUrlText);
            this.Controls.Add(this.progress_iis_build);
            this.Controls.Add(this.b_build_end);

            this.Controls.Add(this.progress_iis_package);
            this.Controls.Add(this.b_package_end);

            this.Controls.Add(this.progress_iis_upload);
            this.Controls.Add(this.b_upload_end);

            this.Controls.Add(this.progress_iis_deploy);

            this.b_build_end.SendToBack();
            this.b_package_end.SendToBack();
            this.b_upload_end.SendToBack();

        }

        protected override void Dispose(bool disposing)
        {
            Action action = () =>
            {
                 
                try
                {
                    if (ServerType.Equals(ServerType.IIS))
                    {
                        Server.IIsFireUrl = this.FireUrlText.Text;
                    }
                    if (ServerType.Equals(ServerType.DOCKER))
                    {
                        Server.DockerFireUrl = this.FireUrlText.Text;
                    }
                    if (ServerType.Equals(ServerType.WINSERVICE))
                    {
                        Server.WindowsServiceFireUrl = this.FireUrlText.Text;
                    }

                    progress_iis_build.Dispose();
                    progress_iis_package.Dispose();
                    progress_iis_upload.Dispose();
                    progress_iis_deploy.Dispose();
                    base.Dispose(disposing);
                }
                catch (Exception)
                {

                }
              };

            if (this.InvokeRequired)
            {

                this.BeginInvoke(action, null);
            }
            else
            {
                action();
            }

        }

        public void StartBuild()
        {
            progress_iis_build.Value = 20;
        }

        public void UpdateBuildProgress(int value)
        {
            if (value > 0 && progress_iis_build.Value == 0)
            {
                progress_iis_build.Value = 20;
            }

            if (value >= 100)
            {
                progress_iis_build.Value = 100;
                BuildEnd();
            }
        }
        public void UpdatePackageProgress(int value)
        {
            if (value > 0 && progress_iis_package.Value == 0)
            {
                progress_iis_package.Value = 20;
            }
            this.progress_iis_package.SuperscriptText = value + "%";
            if (value >= 100)
            {
                progress_iis_package.Value = 100;
                PackageEnd();
                progress_iis_upload.Value = 20;
            }
        }

        public void UpdateUploadProgress(int value)
        {
            if (value > 0 && progress_iis_upload.Value == 0)
            {
                progress_iis_upload.Value = 20;
            }
            this.progress_iis_upload.SuperscriptText = value + "%";
            if (value >= 100)
            {
                progress_iis_upload.Value = 100;
                UploadEnd();
                StartDeploy();
            }
        }
     
        public void StartDeploy()
        {
            progress_iis_deploy.Value = 20;
        }
        public void UpdateDeployProgress(bool value)
        {
            if (value)
            {
                this.progress_iis_deploy.SuperscriptText = "Success";
                this.progress_iis_deploy.SuperscriptMargin = new System.Windows.Forms.Padding(-25, 0, 0, 0);
            }
            else
            {
                this.progress_iis_deploy.SuperscriptText = "Fail";
                this.progress_iis_deploy.ProgressColor = System.Drawing.Color.Red;
            }

            this.progress_iis_deploy.Value = 100;
        }

        public void BuildEnd()
        {
            progress_iis_build.Value = 100;
            progress_iis_build.SuperscriptText = "√";
            b_build_end.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
            progress_iis_package.Value = 20;
        }

        public void BuildError()
        {
            progress_iis_build.Value = 100;
            progress_iis_build.SuperscriptText = "Error";
            this.progress_iis_build.ProgressColor = System.Drawing.Color.Red;
        }

        public void PackageError()
        {
            progress_iis_package.Value = 100;
            progress_iis_package.SuperscriptText = "Error";
            this.progress_iis_package.ProgressColor = System.Drawing.Color.Red;
        }

        public void UploadError()
        {
            progress_iis_upload.Value = 100;
            progress_iis_upload.SuperscriptText = "Error";
            this.progress_iis_upload.ProgressColor = System.Drawing.Color.Red;
        }
        public void PackageEnd()
        {
            b_package_end.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
        }

        public void UploadEnd()
        {
            b_upload_end.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(180)))));
        }


        public void Enable(bool flag)
        {
            this.FireUrlText.Enabled = flag;
        }

        public bool CheckFireUrl()
        {
            var url = this.FireUrlText.Text.ToLower();
            if (!string.IsNullOrEmpty(url) && !url.StartsWith("http"))
            {
                return false;
            }

            if (ServerType.Equals(ServerType.IIS))
            {
                Server.IIsFireUrl = this.FireUrlText.Text;
            }
            if (ServerType.Equals(ServerType.DOCKER))
            {
                Server.DockerFireUrl = this.FireUrlText.Text;
            }
            if (ServerType.Equals(ServerType.WINSERVICE))
            {
                Server.WindowsServiceFireUrl = this.FireUrlText.Text;
            }

            return true;
        }
    }
}
