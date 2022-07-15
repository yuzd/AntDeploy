// Guids.cs
// MUST match guids.h
using System;

namespace yuzd.AntDeploy
{
    static class GuidList
    {
        public const string guidAntDeployPkgString = "2f0edaeb-127f-4467-acd7-918195a1a583";
        public const string guidAntDeployCmdSetString = "b17151eb-1835-477e-a501-5f5cb7d1db91";

        public static readonly Guid guidAntDeployCmdSet = new Guid(guidAntDeployCmdSetString);
    };
}