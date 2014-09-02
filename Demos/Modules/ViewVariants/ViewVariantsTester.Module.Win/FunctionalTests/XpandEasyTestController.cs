using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;

namespace ViewVariantsTester.Module.Win.FunctionalTests {
    public class XpandEasyTestController : Module.FunctionalTests.XpandEasyTestController {
        public override string ChangeColumnCaption(string caption) {
            base.ChangeColumnCaption(caption);
            var gridView = ((GridListEditor)((ListView)View).Editor).GridView;
            var oldCaption = gridView.Columns[0].Caption;
            gridView.Columns[0].Caption = caption;
            return oldCaption;
        }
    }
}
