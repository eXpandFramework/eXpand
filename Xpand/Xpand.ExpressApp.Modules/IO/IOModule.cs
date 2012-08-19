using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.TreeListEditors;
using Xpand.ExpressApp.IO.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.IO {
    [ToolboxItem(false)]
    public sealed class IOModule : XpandModuleBase {
        public IOModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ImportExport");
                Core.TypesInfo.Instance.RegisterTypes(GetAdditionalClasses(moduleManager));
            }
        }

        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            var requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.AddRange(new[] { typeof(TreeListEditorsModuleBase), typeof(ModelArtifactStateModule) });
            return requiredModuleTypesCore;
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }
    }
}