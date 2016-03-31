using System.Reflection;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyTitle("Debug build")]
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyTitle("Lightweight OPDS server.")]
#endif

[assembly: AssemblyProduct("DotOPDS")]
[assembly: AssemblyCopyright("(c) 2016 Denis Shemanaev")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0")]
