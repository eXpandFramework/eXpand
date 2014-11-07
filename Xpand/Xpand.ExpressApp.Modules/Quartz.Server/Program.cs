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
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            var cfg = HostFactory.New(x => {
                x.Service<QuartzServer>(configurator => {
                    configurator.ConstructUsing(builder => new QuartzServer());
                    configurator.WhenStarted(quartzServer => {
                        var modulePath = ConfigurationManager.AppSettings["xafApplicationPath"];
                        var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
#if EasyTest
                        connectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
#endif
                        var xafApplication = XafApplicationFactory.GetApplication(modulePath, connectionString);
                        quartzServer.Initialize(xafApplication);
                        quartzServer.Start();
                    });
                    configurator.WhenPaused(server => server.Pause());
                    configurator.WhenContinued(server => server.Resume());
                    configurator.WhenStopped(server => server.Stop());

                });
                x.RunAsLocalSystem();

                x.SetDescription(Configuration.ServiceDescription);
                x.SetDisplayName(Configuration.ServiceDisplayName);
                x.SetServiceName(Configuration.ServiceName);

                x.RunAsLocalService();
                x.StartAutomaticallyDelayed();
            });


            cfg.Run();

        }

    }
}
