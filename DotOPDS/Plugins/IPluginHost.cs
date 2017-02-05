namespace DotOPDS.Plugins
{
    public interface IPluginHost
    {
        ILogger GetLogger(string name);
        string NormalizePath(string path);
    }
}
