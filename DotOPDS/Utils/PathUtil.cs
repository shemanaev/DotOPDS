using System;

namespace DotOPDS.Utils
{
    class PathUtil
    {
        public static string Normalize(string path)
        {
            // TODO: replace \\ to / for UNIX
            return Environment.ExpandEnvironmentVariables(path).Replace('/', '\\');
        }
    }
}
