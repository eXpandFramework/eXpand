using System;
using System.Collections.Generic;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using SystemTester.Module.FunctionalTests.SequenceGenerator;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using Xpand.Persistent.Base.General;
using Updater = SystemTester.Module.DatabaseUpdate.Updater;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : EasyTestModule {
        public SystemTesterModule() {
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.RegisterEntity("SequenceGeneratorDCObject",typeof(ISequenceGeneratorObject));
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }
}
