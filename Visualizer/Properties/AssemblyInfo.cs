using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Moq;
using Moq.Visualizer;

[assembly: AssemblyTitle("Moq.DebuggerVisualizer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Moq.DebuggerVisualizer")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("d1379ca2-5f85-4017-96fe-cd61fa448ff6")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: DebuggerVisualizer(
    typeof(MockVisualizer),
	typeof(MockVisualizerObjectSource),
    Target = typeof(Mock),
    Description = "MOQ Debugger Visualizer")]