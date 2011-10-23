using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using System.Linq;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;

namespace Xpand.ExpressApp.Workflow {
    [ToolboxItem(true)]
    public sealed partial class XpandWorkFlowModule : XpandModuleBase {


        public XpandWorkFlowModule() {
            InitializeComponent();
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(GetType().Assembly));
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            var declaredExportedTypes = base.GetDeclaredExportedTypes().ToList();
            declaredExportedTypes.Add(typeof(ObjectChangedWorkflow));
            declaredExportedTypes.Add(typeof(ScheduledWorkflows.ScheduledWorkflow));
            return declaredExportedTypes;
        }
    }
}