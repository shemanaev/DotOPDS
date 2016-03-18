using CommandLine;
using DotOPDS.Tasks;
using Serilog;

namespace DotOPDS.Commands
{
    [Verb("serve",
    HelpText = "Start web server.")]
    class ServeOptions : BaseOptions
    {
    }

    class ServeCommand : ICommand
    {
        public int Run(BaseOptions options)
        {
            var opts = (ServeOptions)options;
            Settings.Load(opts.Config);

            Log.Information("Hit Ctrl+C to stop");
            using (var task = new ServeTask())
            {
                task.Run();
            }

            return 0;
        }
    }
}
