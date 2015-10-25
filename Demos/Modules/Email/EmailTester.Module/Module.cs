using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using EmailTester.Module.FunctionalTests;
using Xpand.Persistent.Base.General;
using Updater = EmailTester.Module.DatabaseUpdate.Updater;

namespace EmailTester.Module {
    public sealed partial class EmailTesterModule : EasyTestModule {
        public EmailTesterModule() {
            InitializeComponent();
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (Application != null)
                Application.CreateCustomLogonWindowControllers += ApplicationOnCreateCustomLogonWindowControllers;
        }

        private void ApplicationOnCreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(new EmailObjectController());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[]{updater};
        }
    }
}
