namespace DotOPDS.Plugins
{
    public interface IPluginHost
    {
        ILogger GetLogger(string name);
        ITranslator GetTranslator();
        string NormalizePath(string path);
    }
}
