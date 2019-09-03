using System.IO;
using System.Web.Hosting;
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

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = Path.GetFullPath(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "ClientApp"));

                if (HostingEnvironment.IsDevelopmentEnvironment)
                {
                    spa.UseAngularCliServer("start");
                }
            });
        }
    }
}
