using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Moq;
using Moq.Sdk;

static class WorkspaceServices
{
    static WorkspaceServices()
    {
        if (Environment.GetEnvironmentVariable("DEBUG_MOQ") == "1")
            Debugger.Break();

        HostServices = MefHostServices.Create(
            MefHostServices.DefaultAssemblies.Concat(new[]
            {
                    // Moq.Sdk.dll
                    typeof(IMock).Assembly,
                    // Moq.CodeFixes.dll
                    typeof(MockValidateTypesAnalyzer).Assembly,
                    // Moq.dll
                    typeof(IMoq).Assembly,
            }));
    }

    public static HostServices HostServices { get; }
}
