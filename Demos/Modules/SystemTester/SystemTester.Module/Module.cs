using System;
using System.Reflection;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using Fasterflect;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module {
    [Obsolete("T252521")]
    public class BC151{
        public static void RegisterModule<TModule>() where TModule : ModuleBase {
            var assemblyStore = (Dictionary<Assembly, DcAssemblyInfo>)XafTypesInfo.Instance.GetFieldValue("assemblyStore");
            var assembly = typeof(TModule).Assembly;
            var assemblyInfo = new DcAssemblyInfo(assembly, (TypesInfo)XafTypesInfo.Instance);
            assemblyInfo.SetAttributes(assembly.GetCustomAttributes(false), assembly.GetCustomAttributes(true));
            assemblyStore.Add(assembly, assemblyInfo);
        }
    }
    public sealed partial class SystemTesterModule : EasyTestModule {
        static SystemTesterModule(){
            BC151.RegisterModule<SystemTesterModule>();
        }

        public SystemTesterModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.CreateCustomLogonWindowControllers+=ApplicationOnCreateCustomLogonWindowControllers;
        }

        private void ApplicationOnCreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(new ChooseDatabaseAtLogonController());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
