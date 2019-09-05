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

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\")
            });

            app.UseStaticFiles("/Scripts");
            app.UseStaticFiles("/Content");

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = new PathString("/scripts"),
            //    FileSystem = new PhysicalFileSystem(@".\Scripts")
            //});


            app.UseFileServer(new FileServerOptions
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\ClientApp\dist")
            });

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = new PathString("/content"),
            //    FileSystem = new PhysicalFileSystem(@".\Content")
            //});

            //app.Use((context, next) =>
            //{
            //   var currentRequest = HttpContext.Current.Request.RequestContext;

            //   var controllerId = currentRequest.RouteData.Values.Count > 0 ? currentRequest.RouteData.GetRequiredString("controller") : null;
            //   if (controllerId == null) return next();

            //   IController controller = null;
            //   IControllerFactory factory = null;
            //   try
            //   {
            //       factory = ControllerBuilder.Current.GetControllerFactory();
            //       controller = factory.CreateController(currentRequest, controllerId);
            //       controller?.Execute(currentRequest);
            //   }
            //   finally
            //   {
            //       factory.ReleaseController(controller);
            //   }

            //   return next();
            //});

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = Path.GetFullPath(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientApp"));

            //    //if (HostingEnvironment.IsDevelopmentEnvironment)
            //    //{
            //    //    spa.UseAngularCliServer("start");
            //    //}
            //});
        }
    }
}
