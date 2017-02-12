using NLog;

namespace DotOPDS.Plugins
{
    internal class PluginLogger : ILogger
    {
        private Logger logger;

        public PluginLogger(string name)
        {
            logger = LogManager.GetLogger(name);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Trace(string message)
        {
            logger.Trace(message);
        }

        public void Warn(string message)
        {
            logger.Warn(message);
        }
    }
}
