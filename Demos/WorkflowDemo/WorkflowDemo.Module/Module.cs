using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Objects;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Persistent.BaseImpl;
using WorkflowDemo.Module.Objects;
using Task = WorkflowDemo.Module.Objects.Task;

namespace WorkflowDemo.Module {
    public sealed partial class WorkflowDemoModule : ModuleBase {
        public WorkflowDemoModule() {
            InitializeComponent();
            AdditionalExportedTypes.Add(typeof (Issue));
            AdditionalExportedTypes.Add(typeof (Task));
            AdditionalExportedTypes.Add(typeof (User));
            AdditionalExportedTypes.Add(typeof (Role));
            RequiredModuleTypes.Add(typeof (WorkflowModule));
            RequiredModuleTypes.Add(typeof (SecurityModule));
            RequiredModuleTypes.Add(typeof (BusinessClassLibraryCustomizationModule));
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[]{updater, new XpandUpdater(objectSpace, Version)};
        }
    }
}