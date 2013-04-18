using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Utils;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace Xpand.ExpressApp.Workflow {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    [ToolboxBitmap(typeof(WorkflowModule), "Resources.Toolbox_Module_Workflow.ico")]
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