using Serilog;
using System;
using Unosquare.Labs.EmbedIO.Log;

namespace DotOPDS.Utils
{
    class EmbedioLog : ILog
    {
        public void DebugFormat(string format, params object[] args)
        {
            Log.Debug(format, args);
        }

        public void Error(object message)
        {
            Log.Error((string)message);
        }

        public void Error(object message, Exception exception)
        {
            Log.Error(exception, (string)message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log.Error(format, args);
        }

        public void Info(object message)
        {
            Log.Information((string)message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log.Information(format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Log.Warning(format, args);
        }
    }
}
