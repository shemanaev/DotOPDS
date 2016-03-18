using DotOPDS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotOPDS
{
    public class Settings
    {
        public string Listen { get; set; }
        public string Database { get; set; }
        public SettingsLog Log { get; set; }
        public SettingsAuthentication Authentication { get; set; }
        public int Pagination { get; set; }
        public List<string> Modules { get; set; }
        public Dictionary<string, string> Converters { get; set; }
        public Dictionary<Guid, SettingsLibrary> Libraries { get; set; }

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

        public static void Load(string filename)
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
            if (instance.Log != null)
            {
                if (instance.Log.Path != null)
                    instance.Log.Path = PathUtil.Normalize(instance.Log.Path);
            }
            if (instance.Modules == null)
            {
                instance.Modules = new List<string>();
            }
            if (instance.Libraries == null)
            {
                instance.Libraries = new Dictionary<Guid, SettingsLibrary>();
            }
        }
        #endregion
    }

    public class SettingsLog
    {
        public bool Enabled { get; set; }
        public string Path { get; set; }
        public string Level { get; set; }
    }

    public class SettingsAuthentication
    {
        public bool Enabled { get; set; }
        public int Attempts { get; set; }
        public Dictionary<string, string> Users { get; set; }
        public List<string> Banned { get; set; }
    }

    public class SettingsLibrary
    {
        public string Path { get; set; }
        public string Covers { get; set; }
    }
}
