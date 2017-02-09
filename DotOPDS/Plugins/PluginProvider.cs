using DotOPDS.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotOPDS.Plugins
{
    internal class PluginProvider
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<PluginProvider> lazy = new Lazy<PluginProvider>(() => new PluginProvider());
        public static PluginProvider Instance { get { return lazy.Value; } }

        private Dictionary<string, IFileFormat> fileFormatReaders = new Dictionary<string, IFileFormat>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string, IBookProvider> bookProviders = new Dictionary<string, IBookProvider>(StringComparer.InvariantCultureIgnoreCase);

        public List<IBookProvider> Importers { get { return bookProviders.Values.ToList(); } }
        public IEnumerable<IndexField> IndexFields
        {
            get
            {
                return bookProviders.Values.Where(p => p.IndexFields != null).SelectMany(p => p.IndexFields);
            }
        }

        private PluginProvider()
        {
        }

        ~PluginProvider()
        {
            foreach (var p in bookProviders.Values)
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
            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins/");
            var plugins = from file in Directory.GetFiles(pluginsPath, "*.dll")
                          from type in Assembly.LoadFrom(file).GetTypes()
                          where type.GetInterfaces().Contains(typeof(IPlugin))
                          select type;

            (from i in plugins
             where i.GetInterfaces().Contains(typeof(IBookProvider))
             select (IBookProvider)Activator.CreateInstance(i)).ToList()
            .ForEach(i =>
            {
                var host = new PluginHost(i.GetType().Assembly.GetName().Name);
                if (i.Initialize(host))
                {
                    logger.Info("Loaded import plugin: {0} {1}.", i.Name, i.Version);
                    bookProviders.Add(i.Command, i);
                }
            });

            (from i in plugins
             where i.GetInterfaces().Contains(typeof(IFileFormat))
             select (IFileFormat)Activator.CreateInstance(i)).ToList()
            .ForEach(i =>
            {
                var host = new PluginHost(i.GetType().Assembly.GetName().Name);
                if (i.Initialize(host))
                {
                    logger.Info("Loaded file format plugin: {0} {1}.", i.Name, i.Version);
                    fileFormatReaders.Add(i.Extension, i);
                }
            });
        }

        public IBookProvider GetBookProvider(string command)
        {
            IBookProvider ret;
            bookProviders.TryGetValue(command, out ret);
            return ret;
        }

        public IFileFormat GetFileFormatReader(string ext)
        {
            IFileFormat ret;
            fileFormatReaders.TryGetValue(ext, out ret);
            return ret;
        }
    }
}
