using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotOPDS.Plugins
{
    internal class PluginProvider : IPluginHost
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<PluginProvider> lazy = new Lazy<PluginProvider>(() => new PluginProvider());
        public static PluginProvider Instance { get { return lazy.Value; } }

        private Dictionary<string, IFileFormat> fileFormatReaders = new Dictionary<string, IFileFormat>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, IImporter> importers = new Dictionary<string, IImporter>(StringComparer.InvariantCultureIgnoreCase);

        public List<IImporter> Importers { get { return importers.Values.ToList(); } }

        private PluginProvider()
        {
        }

        ~PluginProvider()
        {
            foreach (var p in importers.Values)
            {
                p.Terminate();
            }
            foreach (var p in fileFormatReaders.Values)
            {
                p.Terminate();
            }
        }

        public void Initialize()
        {
            var plugins = from file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"plugins\"), "*.dll")
                          from type in Assembly.LoadFrom(file).GetTypes()
                          where type.GetInterfaces().Contains(typeof(IPlugin))
                          select type;

            (from i in plugins
             where i.GetInterfaces().Contains(typeof(IImporter))
             select (IImporter)Activator.CreateInstance(i)).ToList()
            .ForEach(i =>
            {
                if (i.Initialize(this))
                {
                    logger.Info("Loaded import plugin: {0} {1}.", i.Name, i.Version);
                    importers.Add(i.Command, i);
                }
            });

            (from i in plugins
             where i.GetInterfaces().Contains(typeof(IFileFormat))
             select (IFileFormat)Activator.CreateInstance(i)).ToList()
            .ForEach(i =>
            {
                if (i.Initialize(this))
                {
                    logger.Info("Loaded file format plugin: {0} {1}.", i.Name, i.Version);
                    fileFormatReaders.Add(i.Extension, i);
                }
            });
        }

        public IImporter GetImporter(string command)
        {
            importers.TryGetValue(command, out IImporter ret);
            return ret;
        }

        public IFileFormat GetFileFormatReader(string ext)
        {
            fileFormatReaders.TryGetValue(ext, out IFileFormat ret);
            return ret;
        }

        public ILogger GetLogger(string name)
        {
            return new PluginLogger(name);
        }
    }
}
