using SecurityDemo.Module;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using System;

namespace SecurityDemo.Win {
    public class ApplicationServerStarter : ApplicationServerStarterBase {
        protected override IList<ModuleBase> GetModules() {
            IList<ModuleBase> result = base.GetModules();
            result.Add(new SecurityDemo.Module.Win.SecurityDemoWindowsFormsModule());
            return result;
        }
        protected override AppDomain CreateDomain() {
            var appDomain = AppDomain.CreateDomain("ServerDomain");
            appDomain.UnhandledException+=AppDomainOnUnhandledException;
            return appDomain;
        }

        void AppDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs) {
            
        }

        protected override ApplicationServerStarterBase CreateServerStarter(AppDomain domain) {
            return (ApplicationServerStarterBase)domain.CreateInstanceAndUnwrap(
                    GetType().Assembly.FullName, GetType().FullName);
        }
    }
}
