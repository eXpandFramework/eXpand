using System.Configuration;
using System.IO;
using Topshelf;
using Xpand.Persistent.Base.General;

namespace Xpand.Quartz.Server {
    /// <summary>
    /// The server's main entry point.
    /// </summary>
    public static class Program {
        /// <summary>
        /// Entry point.  Initializes and runs the service using Topshelf <see cref="topshelf-project.com" />
        /// </summary>
        /// <remarks>
        /// <para>
        /// Uses the Topshelf Service configuration library to configure and run the service.
        /// <see cref="http://docs.topshelf-project.com/en/latest/"/>
        /// </para>
        /// <para>
        /// To understand the below you need to have an understanding of Topshelf.
        /// <see cref="http://docs.topshelf-project.com/en/latest/configuration/quickstart.html"/>
        /// </para>
        /// <para>
        /// Generally there should be no need to create your own implementation of the server, but if this is required
        /// this can be used as a template.
        /// </para>
        /// <para>
        /// For quartz configuration options <see cref="http://quartz-scheduler.org/documentation/quartz-2.x/configuration/"/>
        /// as quartz.net is a port of the java based quartz scheduler and uses the same options.
        /// </para>
        /// </remarks>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            HostFactory.Run(x => {
                x.Service<QuartzServer>(configurator => {
                    configurator.ConstructUsing(builder => new QuartzServer());

                    configurator.WhenStarted(quartzServer => {
                        string modulePath = ConfigurationManager.AppSettings["xafApplicationPath"];
                        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
#if EasyTest
                        connectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
#endif

                        var xafApplication = XafApplicationFactory.GetApplication(modulePath, connectionString);
                        
                        quartzServer.Initialize(xafApplication);
                        
                        quartzServer.Start();
                    });

                    x.StartAutomaticallyDelayed();

                    configurator.WhenPaused(server => server.Pause());
                    configurator.WhenContinued(server => server.Resume());
                    configurator.WhenStopped(server => server.Stop());

                });
            });
        }

    }
}
