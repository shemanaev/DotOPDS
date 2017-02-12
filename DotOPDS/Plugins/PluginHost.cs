using DotOPDS.Utils;

namespace DotOPDS.Plugins
{
    internal class PluginHost : IPluginHost
    {
        private string _name;
        private ITranslator translator;

        public PluginHost(string name)
        {
            _name = name;
        }

        public ILogger GetLogger(string name)
        {
            return new PluginLogger($"{_name}.{name}");
        }

        public ITranslator GetTranslator()
        {
            if (translator == null)
                translator = new Translator(_name, Settings.Instance.Language);
            return translator;
        }

        public string NormalizePath(string path)
        {
            return Util.Normalize(path);
        }
    }
}
