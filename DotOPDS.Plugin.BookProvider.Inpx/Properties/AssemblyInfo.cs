using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("DotOPDS.Plugin.BookProvider.Inpx")]
[assembly: AssemblyDescription("DotOPDS inpx import support")]
[assembly: AssemblyProduct("DotOPDS")]
[assembly: AssemblyCopyright("(c) 2017 Denis Shemanaev")]
[assembly: ComVisible(false)]
[assembly: Guid("bf1aac79-02dd-4075-b675-1ea19439b810")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0")]
