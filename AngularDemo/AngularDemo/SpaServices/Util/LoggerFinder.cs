namespace AngularDemo.SpaServices.Util
{
    internal static class LoggerFinder
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //public static ILogger GetOrCreateLogger(
        //    IAppBuilder appBuilder,
        //    string logCategoryName)
        //{
        //    ILoggerFactory service = appBuilder.ApplicationServices.GetService<ILoggerFactory>();
        //    if (service == null)
        //        return (ILogger)new ConsoleLogger(logCategoryName, (Func<string, LogLevel, bool>)null, false);
        //    return service.CreateLogger(logCategoryName);
        //}
    }
}