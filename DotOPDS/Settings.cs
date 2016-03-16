using DotOPDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotOPDS
{
    public class Settings
    {
        public int Port { get; set; }
        public string Database { get; set; }
        public SettingsLog Log { get; set; }
        public SettingsAuthentication Authentication { get; set; }
        public int Pagination { get; set; }
        public List<SettingsConverter> Converters { get; set; }
        public Dictionary<Guid, string> Libraries { get; set; }

        #region Static routines
        public static string FileName { get; private set; }
        private static Settings instance;

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
                Resource.SaveToFile("default.conf.example", FileName);
            }
            using (var reader = File.OpenText(FileName))
            {
                var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
                instance = deserializer.Deserialize<Settings>(reader);
            }
            Normalize();
        }

        public static void Save()
        {
            using (var reader = File.CreateText(FileName))
            {
                var serializer = new Serializer(namingConvention: new CamelCaseNamingConvention());
                serializer.Serialize(reader, instance);
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
            if (instance.Libraries == null)
            {
                instance.Libraries = new Dictionary<Guid, string>();
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
        public List<SettingsAuthenticationUser> Users { get; set; }
        public List<string> Banned { get; set; }
    }

    public class SettingsAuthenticationUser
    {
        public string Login { get; set; }
        public string Pass { get; set; }
    }

    public class SettingsConverter
    {
        public string Ext { get; set; }
        public string Command { get; set; }
    }
}
