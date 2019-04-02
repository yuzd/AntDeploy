// Guids.cs
// MUST match guids.h
using System;

namespace yuzd.AntDeploy
{
    static class GuidList
    {
        public const string guidAntDeployPkgString = "2f0edaeb-127f-4467-acd7-918195a1a583";
        public const string guidAntDeployCmdSetString = "ebc2240c-7db6-400f-8ccc-d10d958a4dd9";

        public static readonly Guid guidAntDeployCmdSet = new Guid(guidAntDeployCmdSetString);
    };
}