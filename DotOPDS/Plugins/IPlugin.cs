using System;

namespace DotOPDS.Plugins
{
    /// <summary>
    /// Base interface for all plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Plugin version.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Plugin display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called on plugin loading.
        /// </summary>
        /// <param name="host">Interface to access host services.</param>
        /// <returns>True if plugin initialized, false if there was error and it should be unloaded.</returns>
        bool Initialize(IPluginHost host);

        /// <summary>
        /// Called on plugin unloading.
        /// </summary>
        void Terminate();
    }
}
