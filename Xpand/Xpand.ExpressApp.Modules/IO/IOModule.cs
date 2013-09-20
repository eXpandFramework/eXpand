using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.TreeListEditors;
using DevExpress.Utils;
using Xpand.ExpressApp.IO.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.IO {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class IOModule : XpandModuleBase {
        public IOModule() {
            var types = new[]{typeof (TreeListEditorsModuleBase), typeof (ModelArtifactStateModule)};
            foreach (var type in types) {
                RequiredModuleTypes.Add(type);    
            }
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ImportExport");
                Core.TypesInfo.Instance.RegisterTypes(GetAdditionalClasses(moduleManager));
            }
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }
    }
}