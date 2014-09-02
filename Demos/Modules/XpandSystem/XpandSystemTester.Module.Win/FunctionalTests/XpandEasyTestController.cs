﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace XpandSystemTester.Module.Win.FunctionalTests {
    public class XpandEasyTestController:Module.FunctionalTests.XpandEasyTestController {
        public override string ChangeColumnCaption(string caption){
            base.ChangeColumnCaption(caption);
            var gridView = ((ColumnsListEditor)((ListView)View).Editor).GridView();
            var oldCaption = gridView.Columns[0].Caption;
            gridView.Columns[0].Caption = caption;
            return oldCaption;
        }
    }
}
