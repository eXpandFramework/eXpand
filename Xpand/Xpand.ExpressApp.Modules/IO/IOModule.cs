using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.IO.NodeUpdaters;

namespace Xpand.ExpressApp.IO {
    [ToolboxItem(false)]
    public sealed partial class IOModule : XpandModuleBase {

        public IOModule() {
            InitializeComponent();
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ImportExport");
            Core.TypesInfo.Instance.RegisterTypes(GetAdditionalClasses(moduleManager));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }
    }
}