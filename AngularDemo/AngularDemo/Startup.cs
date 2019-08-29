using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartupAttribute(typeof(AngularDemo.Startup))]
namespace AngularDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var pathToData = Path.GetFullPath(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientApp"));

            //app.Use((Func<HttpContext, Func<Task>, Task>)((context, next) =>
            //{
            //    context.RewritePath("/index.html");
            //    return next();
            //}));
            //app.UseHandler((request, response, next) =>
            //{
            //    request.Path = "/index.html";
            //    return next();
            //});

            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(@".\ClientApp\dist")
            });

            //app.UseHandler((request, response, next) =>
            //{
            //    string message = "The SPA default page middleware could not return the default page " +
            //                     string.Format(
            //                         "'/index.html' because it was not found, and no other middleware handled the request.\n");

            //    message +=
            //        "Your application is running in Production mode, so make sure it has been published, or that you have built your SPA manually. Alternatively you may wish to switch to the Development environment.\n";
            //    throw new InvalidOperationException(message);
            //});

            
            ISpaBuilder spa = new DefaultSpaBuilder(app, new SpaOptions
            {
                //SourcePath = "ClientApp"
                SourcePath = pathToData
            });

            spa.UseAngularCliServer("start");
        }
    }
}
