using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;

namespace eXpand.ExpressApp.Win.ListEditors {
    public class XpandXafGridColumn:XafGridColumn {
        public XpandXafGridColumn(ITypeInfo typeInfo, DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor) : base(typeInfo, gridListEditor) {
        }

        public new void Assign(GridColumn column) {
            base.Assign(column);
        }
    }
}