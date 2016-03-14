using System.IO;
using System.Reflection;

namespace DotOPDS.Tests
{
    public class Utils
    {
        private static string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string GetPath(string path)
        {
            return Path.Combine(dir, path);
        }
    }
}
