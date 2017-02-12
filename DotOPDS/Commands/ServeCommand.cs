using CommandLine;
using DotOPDS.Plugins;
using DotOPDS.Tasks;
using NLog;

namespace DotOPDS.Commands
{
    [Verb("serve",
    HelpText = "Start web server.")]
    class ServeOptions : BaseOptions
    {
    }

    class ServeCommand : ICommand
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public int Run(BaseOptions options)
        {
            var opts = (ServeOptions)options;
            Settings.Load(opts.Config);
            PluginProvider.Instance.Initialize();

            logger.Info("Hit Ctrl+C to stop");
            using (var task = new ServeTask())
            {
                task.Run(new ServeTaskArgs { Port = Settings.Instance.Port });
            }

            return 0;
        }
    }
}
