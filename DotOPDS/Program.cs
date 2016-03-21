using CommandLine;
using DotOPDS.Commands;
using NLog;
using System;
using System.Threading;

namespace DotOPDS
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static ManualResetEvent exitEvent = new ManualResetEvent(false);
        public static ManualResetEvent Exit { get { return exitEvent; } }

        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.CancelKeyPress += Console_CancelKeyPress;

            var result = Parser.Default.ParseArguments<
                ImportOptions,
                ServeOptions,
                LsOptions,
                MvOptions,
                RmOptions,
                RpcOptions,
                FixtureOptions
                >(args).MapResult(
                    (ImportOptions opts) => RunCommand(typeof(ImportCommand), opts),
                    (ServeOptions opts) => RunCommand(typeof(ServeCommand), opts),
                    (LsOptions opts) => RunCommand(typeof(LsCommand), opts),
                    (MvOptions opts) => RunCommand(typeof(MvCommand), opts),
                    (RmOptions opts) => RunCommand(typeof(RmCommand), opts),
                    (RpcOptions opts) => RunCommand(typeof(RpcCommand), opts),
                    (FixtureOptions opts) => RunCommand(typeof(FixtureCommand), opts),
                    errs => 1);

            return result;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            exitEvent.Set();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error((Exception)e.ExceptionObject, "Unhandled exception");
            exitEvent.Set();
        }

        private static int RunCommand(Type command, BaseOptions options)
        {
            var cmd = (ICommand)Activator.CreateInstance(command);
            return cmd.Run(options);
        }
    }
}
