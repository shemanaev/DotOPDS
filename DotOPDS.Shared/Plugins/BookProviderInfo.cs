using System;

namespace DotOPDS.Shared.Plugins;

public class BookProviderInfo
{
    public string Name { get; private set; }
    public Version Version { get; private set; }
    public string Command { get; private set; }
    public string Help { get; private set; }

    public BookProviderInfo(string name, Version version, string command, string help)
    {
        Name = name;
        Version = version;
        Command = command;
        Help = help;
    }
}
