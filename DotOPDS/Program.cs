using CommandLine;
using DotOPDS.Commands;
using DotOPDS.Utils;
using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Console.CancelKeyPress += Console_CancelKeyPress;

            var result = Parser.Default.ParseArguments<
                ImportOptions,
                ServeOptions,
                LsOptions,
                MvOptions,
                RmOptions,
                InitOptions
                #if DEBUG_FIXTURE
                ,FixtureOptions
                #endif
                >(args).MapResult(
                    (ImportOptions opts) => RunCommand(typeof(ImportCommand), opts),
                    (ServeOptions opts) => RunCommand(typeof(ServeCommand), opts),
                    (LsOptions opts) => RunCommand(typeof(LsCommand), opts),
                    (MvOptions opts) => RunCommand(typeof(MvCommand), opts),
                    (RmOptions opts) => RunCommand(typeof(RmCommand), opts),
                    (InitOptions opts) => RunCommand(typeof(InitCommand), opts),
                    #if DEBUG_FIXTURE
                    (FixtureOptions opts) => RunCommand(typeof(FixtureCommand), opts),
                    #endif
                    errs => 1);

            return result;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            exitEvent.Set();
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        public static void HandleException(Exception e)
        {
            Console.WriteLine();

            if (Util.IsLinux)
            {
                logger.Error(e, "Unhandled exception");
            }
            else
            {
                var dump = Path.Combine(Path.GetTempPath(), string.Format("DotOPDS_{0}.mdmp", Guid.NewGuid()));
                using (var fs = new FileStream(dump, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
                {
#if DEBUG
                    var flags = MiniDump.Option.WithFullMemory
                              | MiniDump.Option.WithFullMemoryInfo
                              | MiniDump.Option.WithThreadInfo;
#else
                    var flags = MiniDump.Option.WithThreadInfo
                              | MiniDump.Option.WithProcessThreadData
                              | MiniDump.Option.WithHandleData;
#endif
                    MiniDump.Write(fs.SafeFileHandle, flags);
                }
                logger.Error("Something bad happened... Dump written to {0}", dump);
            }

            Environment.Exit(1);
        }

        private static int RunCommand(Type command, BaseOptions options)
        {
            var cmd = (ICommand)Activator.CreateInstance(command);
            return cmd.Run(options);
        }
    }
}
