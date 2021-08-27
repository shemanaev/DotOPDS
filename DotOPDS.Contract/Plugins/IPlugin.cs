using System;

namespace DotOPDS.Contract.Plugins;

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
}
