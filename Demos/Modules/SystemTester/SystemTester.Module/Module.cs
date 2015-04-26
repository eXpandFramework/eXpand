using System;
using System.Configuration;
using System.IO;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : EasyTestModule {
        private bool _isUsed;

        public SystemTesterModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
#if EASYTEST
            application.LoggingOn+=ApplicationOnLoggingOn;
#endif
        }

        private void ApplicationOnLoggingOn(object sender, LogonEventArgs logonEventArgs){
            var customLogonParameters = (logonEventArgs.LogonParameters as CustomLogonParameters);
            if (customLogonParameters != null){
                var dbServerType = customLogonParameters.DbServerType;
                if (dbServerType!=DbServerType.Default){
                    Application.ConnectionString = ConfigurationManager.ConnectionStrings[dbServerType.ToString()].ConnectionString;
                    Application.SetFieldValue("objectSpaceProviders",new List<IObjectSpaceProvider>{new XPObjectSpaceProvider(Application.ConnectionString,null)});
                    if (dbServerType == DbServerType.SqlLite && !_isUsed && File.Exists("SystemTesterEasyTest.db")) {
                        File.Delete("SystemTesterEasyTest.db");
                        _isUsed = true;
                    }
                }
            }
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
