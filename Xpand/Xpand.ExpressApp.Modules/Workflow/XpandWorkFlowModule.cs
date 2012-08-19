using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Workflow;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace Xpand.ExpressApp.Workflow {
    [ToolboxItem(true)]
    public sealed class XpandWorkFlowModule : XpandModuleBase {
        public XpandWorkFlowModule() {
            RequiredModuleTypes.Add(typeof(WorkflowModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(GetType().Assembly, IsExportedType));
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            List<Type> declaredExportedTypes = base.GetDeclaredExportedTypes().ToList();
            declaredExportedTypes.Add(typeof(ObjectChangedWorkflow));
            declaredExportedTypes.Add(typeof(ScheduledWorkflow));
            return declaredExportedTypes;
        }

    }
}