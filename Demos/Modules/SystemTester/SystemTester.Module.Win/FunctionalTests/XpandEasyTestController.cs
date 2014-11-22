using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace SystemTester.Module.Win.FunctionalTests{
    public class XpandEasyTestController : Module.FunctionalTests.XpandEasyTestController {
        public override string ChangeColumnCaption(string caption) {
            base.ChangeColumnCaption(caption);
            var gridView = ((WinColumnsListEditor)((ListView)View).Editor).GridView();
            var oldCaption = gridView.Columns[0].Caption;
            gridView.Columns[0].Caption = caption;
            return oldCaption;
        }
    }
}