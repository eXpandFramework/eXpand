using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.IO.NodeUpdaters;

namespace Xpand.ExpressApp.IO {
    public interface IModelOptionIO {
        [Category("IO.eXpand")]
        [Description("Create IOError objects")]
        [DefaultValue(true)]
        bool LogErrors { get; set; }
    }
    [ToolboxItem(false)]
    public sealed partial class IOModule : XpandModuleBase {

        public IOModule() {
            InitializeComponent();
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionIO>();
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ImportExport");
            Core.TypesInfo.Instance.AddTypes(GetAdditionalClasses(moduleManager));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AllowEditForClassInfoNodeListViewsUpdater());
        }
    }
}