using System.IO;

namespace AntDeployAgentWindows.Model
{
    public class Setting
    {

        public static string WebRootPath = "";

        public static string PublishPathFolder = "";

        public static string PublishIIsPathFolder = "";


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
        }
    }
}
