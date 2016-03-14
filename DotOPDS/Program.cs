using CommandLine;
using DotOPDS.Commands;
using Serilog;
using System;

namespace DotOPDS
{
    class Program
    {

        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration() // FIXME: don't create logger in rpc mode
                .WriteTo.ColoredConsole()
                .MinimumLevel.Debug()
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var result = Parser.Default.ParseArguments<
                ImportOptions,
                ServeOptions,
                LsOptions
#if DEBUG
                , FixtureOptions
#endif
                >(args).MapResult(
                    (ImportOptions opts) => RunCommand(typeof(ImportCommand), opts),
                    (ServeOptions opts) => RunCommand(typeof(ServeCommand), opts),
                    (LsOptions opts) => RunCommand(typeof(LsCommand), opts),
#if DEBUG
                    (FixtureOptions opts) => RunCommand(typeof(FixtureCommand), opts),
#endif
                    errs => 1);

            return result;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine(e.ExceptionObject);
            // FIXME
        }

        static int RunCommand(Type command, SharedOptions options)
        {
            int result = 1;
#if !DEBUG
            try
            {
#endif
            using (var cmd = (ICommand)Activator.CreateInstance(command))
            {
                result = cmd.Run(options);
            }
#if !DEBUG
        }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: {0}", e.Message);
                // TODO: save trace to file
            }
#endif
            return result;
        }
    }
}
