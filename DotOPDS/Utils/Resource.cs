using System.IO;
using System.Reflection;

namespace DotOPDS.Utils
{
    class Resource
    {
        public static Stream AsStream(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("{0}.{1}", assembly.GetName().Name, name);
            try
            {
                return assembly.GetManifestResourceStream(resourceName);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public static void SaveToFile(string name, string output)
        {
            output = Util.Normalize(output);
            var dir = Path.GetDirectoryName(output);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var stream = AsStream(name))
            using (var writer = new FileStream(output, FileMode.Create))
            {
                byte[] buffer = new byte[8192];

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
