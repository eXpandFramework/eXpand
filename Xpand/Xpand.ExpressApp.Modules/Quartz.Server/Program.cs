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
            var application = XafApplicationFactory.GetApplication("../FeatureCenter/FeatureCenter.Win/bin/debug/FeatureCenter.Win.exe");
            RunConfiguration cfg = RunnerConfigurator.New(x => {
                x.ConfigureService<QuartzServer>(s => {
                    s.Named("quartz.server");
                    s.HowToBuildService(builder => {
                                                var server = new QuartzServer(application);
                                                server.Initialize();
                                                return server;
                                            });
                    s.WhenStarted(server => server.Start());
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
