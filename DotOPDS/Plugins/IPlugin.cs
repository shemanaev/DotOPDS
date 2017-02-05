using System;

namespace DotOPDS.Plugins
{
    public interface IPlugin
    {
        Version Version { get; }
        string Name { get; }
        bool Initialize(IPluginHost host);
        void Terminate();
    }
}
