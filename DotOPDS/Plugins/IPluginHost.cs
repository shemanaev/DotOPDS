namespace DotOPDS.Plugins
{
    /// <summary>
    /// Provides access to host services.
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// Access to logging.
        /// </summary>
        /// <param name="name">Logger name. Appends to assembly name.</param>
        /// <returns>Logger interface with provided name.</returns>
        ILogger GetLogger(string name);

        /// <summary>
        /// Access to translator interface with current locale.
        /// </summary>
        ITranslator GetTranslator();

        /// <summary>
        /// Expand env variables and make full path relative to host executable.
        /// </summary>
        /// <param name="path">Path to normalize.</param>
        /// <returns>Normalized path.</returns>
        string NormalizePath(string path);
    }
}
