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
        public const string Web_PROJECT_KIND = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string Web_PROJECT_KIND_MVC1 = "{603C0E0B-DB56-11DC-BE95-000D561079B0}";
        public const string Web_PROJECT_KIND_MVC2 = "{F85E285D-A4E0-4152-9332-AB1D724D3325}";
        public const string Web_PROJECT_KIND_MVC3 = "{E53F8FEA-EAE0-44A6-8774-FFD645390401}";
        public const string Web_PROJECT_KIND_MVC4 = "{E3E379DF-F4C6-4180-9B81-6769533ABE47}";
        public const string Web_Application = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string Web_ASPNET5 = "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}";
    }
}