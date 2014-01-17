using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.TreeListEditors.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListModelAdapterController : TreeListModelAdapterController<TreeListEditor> {
        protected override void ExtendModelInterfaces(InterfaceBuilder builder, Assembly assembly){
            base.ExtendModelInterfaces(builder, assembly);
            ExtendWithFont(builder.Extenders,builder, assembly );
        }

        protected override ModelSynchronizer ModelSynchronizer() {
            return new TreeListEditorDynamicModelSynchronizer(TreeListEditor);
        }

        protected override Type TreeListColumnType() {
            return typeof(TreeListColumn);
        }

        protected override Type TreeListType() {
            return typeof(TreeList);
        }

    }
}
