namespace DotOPDS.Plugins
{
    public interface ILogger
    {
        void Debug(string message);
        void Error(string message);
        void Fatal(string message);
        void Info(string message);
        void Trace(string message);
        void Warn(string message);
    }
}
