using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Moq;
using Moq.Visualizer;

[assembly: AssemblyTitle("Moq.Visualizer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Clarius Consulting")]
[assembly: AssemblyProduct("Moq.Visualizer")]
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

[assembly: InternalsVisibleTo("Moq.Visualizer.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001009f7a95086500f8f66d892174803850fed9c22225c2ccfff21f39c8af8abfa5415b1664efd0d8e0a6f7f2513b1c11659bd84723dc7900c3d481b833a73a2bcf1ed94c16c4be64d54352c86956c89930444e9ac15124d3693e3f029818e8410f167399d6b995324b635e95353ba97bfab856abbaeb9b40c9b160070c6325e22ddc")]