using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Workflow;
using System.Linq;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;

namespace Xpand.ExpressApp.Workflow {
    [ToolboxItem(true)]
    public sealed partial class XpandWorkFlowModule : XpandModuleBase {


        public XpandWorkFlowModule() {
            InitializeComponent();
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(GetType().Assembly));
        }


    }
}