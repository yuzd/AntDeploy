namespace AntDeploy.Models
{
    // Looks like this is out to date since 15.8, refer to https://developercommunity.visualstudio.com/content/problem/312523/envdteprojectkind-no-longer-differentiates-between.html

    // https://stackoverflow.com/questions/45795759/detect-a-dotnet-core-project-from-envdte-project-api
    // https://www.codeproject.com/reference/720512/list-of-visual-studio-project-type-guids
    internal static class ProjectKinds
    {
        public const string CS_CORE_PROJECT_KIND = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
        public const string FS_CORE_PROJECT_KIND = "{6EC3EE1D-3C4E-46DD-8F32-0CC8E7565705}";
        public const string VB_CORE_PROJECT_KIND = "{778DAE3C-4631-46EA-AA77-85C1314464D9}";
    }
}