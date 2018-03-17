using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.ExcelImporter.Web {
    public sealed partial class WebModule : ModuleBase {
        public WebModule() {
            InitializeComponent();
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
