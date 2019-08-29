using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Logging;
using Owin;

namespace App.Demo.Util
{
    internal static class LoggerFinder
    {
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