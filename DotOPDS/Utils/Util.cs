using System;

namespace DotOPDS.Utils
{
    class Util
    {
        public static string Normalize(string path)
        {
            var from = IsLinux ? '\\' : '/';
            var to = IsLinux ? '/' : '\\';
            return Environment.ExpandEnvironmentVariables(path).Replace(from, to);
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}
