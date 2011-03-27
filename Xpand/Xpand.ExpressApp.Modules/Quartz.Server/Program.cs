using System.Configuration;
using DevExpress.ExpressApp;
using Topshelf;
using Topshelf.Configuration;
using Topshelf.Configuration.Dsl;

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
            RunConfiguration cfg = RunnerConfigurator.New(x => {
                x.ConfigureService<QuartzServer>(s => {
                    s.Named(Configuration.ServiceName);
                    s.HowToBuildService(builder => new QuartzServer());
                    s.WhenStarted(server => {
                        XafApplication xafApplication = XafApplicationFactory.GetApplication(ConfigurationManager.AppSettings["xafApplicationPath"]);
                        server.Initialize(xafApplication);
                        server.Start();
                    });
                    s.WhenPaused(server => server.Pause());
                    s.WhenContinued(server => server.Resume());
                    s.WhenStopped(server => server.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription(Configuration.ServiceDescription);
                x.SetDisplayName(Configuration.ServiceDisplayName);
                x.SetServiceName(Configuration.ServiceName);
            });

            Runner.Host(cfg, args);
        }

    }
}
