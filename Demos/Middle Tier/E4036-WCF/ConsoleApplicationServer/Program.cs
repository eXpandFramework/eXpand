using System;
using DevExpress.ExpressApp.Security.ClientServer;
using System.Configuration;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Xpo;
using System.ServiceModel;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;
using Xpand.ExpressApp.Security.Permissions;

namespace ConsoleApplicationServer {
    class Program {
        static void Main() {
            try {
//                WcfDataServerHelper.AddKnownType(typeof(MyDetailsOperationRequest));
//                WcfDataServerHelper.AddKnownType(typeof(IsAdministratorPermissionRequest));
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                ValueManager.ValueManagerType = typeof(MultiThreadValueManager<>).GetGenericTypeDefinition();

                Console.WriteLine("Starting...");
                var securityStrategyComplex = new SecurityStrategyComplex(typeof(SecuritySystemUser), typeof(SecuritySystemRole), new AuthenticationStandard());
                var serverApplication = new ConsoleApplicationServerServerApplication(securityStrategyComplex,true) {
                    ConnectionString = connectionString
                };
                Console.WriteLine("Setup...");
                serverApplication.Setup();
                Console.WriteLine("CheckCompatibility...");
                serverApplication.CheckCompatibility();
                serverApplication.Dispose();

                Console.WriteLine("Starting server...");
                QueryRequestSecurityStrategyHandler securityProviderHandler =() => securityStrategyComplex;

                var dataServer =new SecuredDataServer(connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, securityProviderHandler);

                var serviceHost = new ServiceHost(new WcfSecuredDataServer(dataServer));
                WSHttpBinding defaultBinding = (WSHttpBinding)WcfDataServerHelper.CreateDefaultBinding();
                defaultBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
                serviceHost.AddServiceEndpoint(typeof(IWcfSecuredDataServer), defaultBinding, "http://localhost:1451/DataServer");
                serviceHost.Open();

                Console.WriteLine("Server is started. Press Enter to stop.");
                Console.ReadLine();
                Console.WriteLine("Stopping...");
                serviceHost.Close();
                Console.WriteLine("Server is stopped.");
            } catch (Exception e) {
                Console.WriteLine("Exception occurs: " + e.Message);
                Console.WriteLine("Press Enter to close.");
                Console.ReadLine();
            }
        }
    }
}
