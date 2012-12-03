using System;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;

namespace Xpand.ExpressApp.Win.ListEditors {
    [Obsolete("", true)]
    public class XpandXafGridColumn : XafGridColumn {
        public XpandXafGridColumn(ITypeInfo typeInfo, GridListEditor gridListEditor)
            : base(typeInfo, gridListEditor) {
        }

        public new void Assign(GridColumn column) {
            base.Assign(column);
        }
    }
}