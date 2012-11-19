using System;
using System.Configuration;
using DevExpress.ExpressApp;
using Topshelf;

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
            var cfg = HostFactory.New(x =>
            {
                x.Service<QuartzServer>(configurator =>
                {
                    configurator.ConstructUsing(builder => new QuartzServer());
                    configurator.WhenStarted(server =>
                    {
                        XafApplication xafApplication = XafApplicationFactory.GetApplication(ConfigurationManager.AppSettings["xafApplicationPath"]);
                        server.Initialize(xafApplication);
                        server.Start();
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
