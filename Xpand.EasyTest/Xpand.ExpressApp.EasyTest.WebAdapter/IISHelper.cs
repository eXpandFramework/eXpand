using System.IO;
using System.Linq;
using System.Threading;
using DevExpress.EasyTest.Framework;
using Microsoft.Web.Administration;
using Xpand.EasyTest;

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class IISHelper{

        public static void Configure(TestApplication testApplication){
            using (var server = new ServerManager()){
                var applicationName = GetApplicationName(testApplication);
                var applicationPool = server.ApplicationPools.FirstOrDefault(pool => pool.Name == applicationName) ?? server.ApplicationPools.Add(applicationName);
                var webSite = server.Sites.First(site => site.Name == "Default Web Site");
                var url = GetUrl(webSite,applicationName);
                ConfigureTestApplication(testApplication, url);
                ConfigureSiteApplication(testApplication, webSite, applicationPool,applicationName);
                server.CommitChanges();
                if (applicationPool.State == ObjectState.Started)
                    applicationPool.Stop();
                while (applicationPool.State != ObjectState.Stopped) {
                    Thread.Sleep(300);
                }
                applicationPool.Start();
                while (applicationPool.State != ObjectState.Started) {
                    Thread.Sleep(300);
                }
            }
        }

        private static void ConfigureSiteApplication(TestApplication testApplication, Site webSite, ApplicationPool applicationPool, string applicationName){
            var application =webSite.Applications.FirstOrDefault(application1 => application1.Path.EndsWith(applicationName));
            if (application == null){
                application = webSite.Applications.CreateElement();
                application["path"] = "/" + applicationName;
                var physicalPath = Path.GetFullPath(testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath));
                application.VirtualDirectories.Add("/", physicalPath);
                webSite.Applications.Add(application);
                application.ApplicationPoolName = applicationPool.Name;
            }
            else{
                application.VirtualDirectories.Clear();
                var physicalPath = Path.GetFullPath(testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath));
                application.VirtualDirectories.Add("/", physicalPath);
            }
            
        }

        private static string GetApplicationName(TestApplication testApplication){
            var userName = testApplication.ParameterValue<string>(ApplicationParams.UserName);
            return userName ?? testApplication.Name;
        }

        private static string GetUrl(Site webSite, string applicationName){
            var binding = webSite.Bindings.First();
            var host = binding.Host;
            if (string.IsNullOrEmpty(host))
                host = "localhost";
            var url = binding.Protocol + "://" + host + ":" + binding.EndPoint.Port + "/" + applicationName;
            return url;
        }

        private static void ConfigureTestApplication(TestApplication testApplication, string url){
            testApplication.SetParameterValue(ApplicationParams.DontRunWebDev, "True");
            testApplication.SetParameterValue(ApplicationParams.DontRunIISExpress, "True");
            testApplication.SetParameterValue(ApplicationParams.DontRestartIIS, "True");
            testApplication.SetParameterValue(ApplicationParams.Url, url);
        }

        public static void StopAplicationPool(TestApplication testApplication){
            var applicationName = GetApplicationName(testApplication);
            using (var serverManager = new ServerManager()){
                var applicationPool = serverManager.ApplicationPools.First(pool => pool.Name==applicationName);
                applicationPool.Stop();
            }
        }
    }
}