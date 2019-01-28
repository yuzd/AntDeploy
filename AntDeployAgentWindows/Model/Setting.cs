using System.IO;

namespace AntDeployAgentWindows.Model
{
    public class Setting
    {

        public static string WebRootPath = "";

        public static string PublishPathFolder = "";

        public static string PublishIIsPathFolder = "";
        public static string BackUpIIsPathFolder = "";


        public static string PublishWindowServicePathFolder = "";
        public static string BackUpWindowServicePathFolder = "";

        public static void InitWebRoot(string rootPath)
        {

            if (string.IsNullOrEmpty(rootPath))
            {
                return;
            }

            WebRootPath = rootPath;

            PublishPathFolder = Path.Combine(WebRootPath, "publisher");

            if (!Directory.Exists(PublishPathFolder))
            {
                Directory.CreateDirectory(PublishPathFolder);
            }


            PublishIIsPathFolder = Path.Combine(PublishPathFolder, "iis");

            if (!Directory.Exists(PublishIIsPathFolder))
            {
                Directory.CreateDirectory(PublishIIsPathFolder);
            }

            BackUpIIsPathFolder = Path.Combine(PublishPathFolder, "iis_backup");

            if (!Directory.Exists(BackUpIIsPathFolder))
            {
                Directory.CreateDirectory(BackUpIIsPathFolder);
            }

            PublishWindowServicePathFolder = Path.Combine(PublishPathFolder, "window_service");

            if (!Directory.Exists(PublishWindowServicePathFolder))
            {
                Directory.CreateDirectory(PublishWindowServicePathFolder);
            }

            BackUpWindowServicePathFolder = Path.Combine(PublishPathFolder, "window_service_backup");

            if (!Directory.Exists(BackUpWindowServicePathFolder))
            {
                Directory.CreateDirectory(BackUpWindowServicePathFolder);
            }
        }
    }
}
