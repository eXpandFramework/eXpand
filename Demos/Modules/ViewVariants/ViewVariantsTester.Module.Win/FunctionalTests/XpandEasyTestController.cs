using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;

namespace ViewVariantsTester.Module.Win.FunctionalTests {
    public class XpandEasyTestController : Module.FunctionalTests.XpandEasyTestController {
        public override string ChangeColumnCaption(string caption) {
            base.ChangeColumnCaption(caption);
            var gridView = ((GridListEditor)((ListView)View).Editor).GridView;
            var gridColumn = gridView.Columns.First(column => column.Visible);
            var oldCaption = gridColumn.Caption;
            gridColumn.Caption = caption;
            return oldCaption;
        }
    }
}
