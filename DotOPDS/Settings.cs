using DotOPDS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotOPDS
{
    public class Settings
    {
        public int Port { get; set; }
        public string Title { get; set; }
        public string Database { get; set; }
        public SettingsLog Log { get; set; }
        public SettingsAuthentication Authentication { get; set; } = new SettingsAuthentication();
        public int Pagination { get; set; }
        public Dictionary<string, string> Converters { get; set; }
        public Dictionary<Guid, SettingsLibrary> Libraries { get; set; } = new Dictionary<Guid, SettingsLibrary>();

        #region Static routines
        public static string FileName { get; private set; }
        private static Settings instance;
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

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
            FileName = PathUtil.Normalize(filename);
            if (!File.Exists(FileName))
            {
                Resource.SaveToFile("default.win.json", FileName);
            }
            using (var reader = File.OpenText(FileName))
            {
                instance = JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd(), jsonSettings);
            }
            InitLog(console);
            Normalize();
        }

        public static void Save()
        {
            using (var writer = File.CreateText(FileName))
            {
                var s = JsonConvert.SerializeObject(instance, Formatting.Indented, jsonSettings);
                writer.Write(s);
            }
        }

        private static void Normalize()
        {
            instance.Database = PathUtil.Normalize(instance.Database);

            //T.ChangeLanguage(instance.Language); // FIXME
        }

        private static void InitLog(bool console)
        {
            if (instance.Log != null)
            {
                if (instance.Log.Path != null)
                    instance.Log.Path = PathUtil.Normalize(instance.Log.Path);
            }

            var config = new LoggingConfiguration();

            var debuggerTarget = new DebuggerTarget();
            config.AddTarget("debugger", debuggerTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, debuggerTarget));

            if (console)
            {
                var consoleTarget = new ColoredConsoleTarget();
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

                fileTarget.FileName = instance.Log.Path + "/${shortdate}.log";
                // fileTarget.Layout = "${message}";
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.FromString(instance.Log.Level), fileTarget));
            }

            LogManager.Configuration = config;
        }
        #endregion
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
