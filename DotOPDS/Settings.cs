using DotOPDS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotOPDS
{
    public class Settings
    {
        public int Port { get; set; }
        public string Title { get; set; }
        public string Database { get; set; }
        public string Language { get; set; } = "en";
        public string Web { get; set; } = "";
        public bool LazyInfoExtract { get; set; } = false;
        public SettingsLog Log { get; set; }
        public SettingsAuthentication Authentication { get; set; } = new SettingsAuthentication();
        public int Pagination { get; set; }
        public List<SettingsConverter> Converters { get; set; }
        public Dictionary<Guid, SettingsLibrary> Libraries { get; set; } = new Dictionary<Guid, SettingsLibrary>();

        #region Static routines
        public static string FileName { get; private set; }
        private static Settings instance;
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string build = ((AssemblyInformationalVersionAttribute)Assembly
                                      .GetAssembly(typeof(Settings))
                                      .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)[0])
                                      .InformationalVersion;

        public static Settings Instance
        {
            get
            {
                if (instance == null) throw new Exception("Create f*ckin Settings first!"); // shouldn't be ever really
                return instance;
            }
        }

        public static void Load()
        {
            Load(FileName);
        }

        public static void Load(string filename, bool console = true)
        {
            FileName = Util.Normalize(filename);
            if (!File.Exists(FileName))
            {
                var resource = string.Format("default.{0}.json", Util.IsLinux ? "nix" : "win");
                Resource.SaveToFile(resource, FileName);
            }
            using (var reader = File.OpenText(FileName))
            {
                instance = JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd(), jsonSettings);
            }
            InitLog(console);
            T.ChangeLanguage(instance.Language);
        }

        public static void Save()
        {
            using (var writer = File.CreateText(FileName))
            {
                var s = JsonConvert.SerializeObject(instance, Formatting.Indented, jsonSettings);
                writer.Write(s);
            }
        }

        private static void InitLog(bool console)
        {
            var config = new LoggingConfiguration();

            var debuggerTarget = new DebuggerTarget();
            config.AddTarget("debugger", debuggerTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, debuggerTarget));

            if (console)
            {
                var consoleTarget = new ColoredConsoleTarget();
                consoleTarget.Layout = "${time}|${level:uppercase=true}|${message}";
                config.AddTarget("console", consoleTarget);
#if DEBUG
                var level = LogLevel.Trace;
#else
                var level = LogLevel.Info;
#endif
                config.LoggingRules.Add(new LoggingRule("*", level, consoleTarget));
            }

            if (instance.Log.Enabled)
            {
                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                fileTarget.FileName = Path.Combine(Util.Normalize(instance.Log.Path), "${shortdate}.log");
                // fileTarget.Layout = "${message}";
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.FromString(instance.Log.Level), fileTarget));
            }

            LogManager.Configuration = config;

            logger.Info("DotOPDS v{0}", build);
            logger.Info("Loaded configuration from {0}", FileName);
        }
        #endregion
    }

    public class SettingsConverter
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Command { get; set; }
    }

    public class SettingsLog
    {
        public bool Enabled { get; set; } = false;
        public string Path { get; set; }
        public string Level { get; set; } = "inforamtion";
    }

    public class SettingsAuthentication
    {
        public bool Enabled { get; set; } = false;
        public int Attempts { get; set; } = 10;
        public Dictionary<string, string> Users { get; set; } = new Dictionary<string, string>();
        public List<string> Banned { get; set; } = new List<string>();
    }

    public class SettingsLibrary
    {
        public string Path { get; set; }
        public string Covers { get; set; }
    }
}
