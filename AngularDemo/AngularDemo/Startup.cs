using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using AngularDemo.SpaServices;
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

            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new PhysicalFileSystem(@".\ClientApp\dist")
            });

            app.Use((context, next) =>
            {
                var contains = RouteTable.Routes.Contains(HttpContext.Current.Request.RequestContext.RouteData.Route);
                var test = RouteTable.Routes.RouteExistingFiles;

                if (!contains)
                {
                    return next();
                }

                return null;
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = Path.GetFullPath(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientApp"));

                //if (HostingEnvironment.IsDevelopmentEnvironment)
                //{
                //    spa.UseAngularCliServer("start");
                //}
            });
        }
    }
}
