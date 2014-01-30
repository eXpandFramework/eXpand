using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.TreeListEditors.Win.ListEditors;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListModelAdapterController : TreeListEditors.Model.TreeListModelAdapterController {
        protected override bool GetValidEditor(){
            var validEditor = ((ListView) View).Editor;
            return validEditor is TreeListEditor || validEditor is CategorizedListEditor;
        }

        protected override void ExtendModelInterfaces(InterfaceBuilder builder, Assembly assembly){
            base.ExtendModelInterfaces(builder, assembly);
            ExtendWithFont(builder.Extenders,builder, assembly );
        }

        protected override ModelSynchronizer ModelSynchronizer(){
            var listEditor = (((ListView)View).Editor);
            var treeListEditor = listEditor as TreeListEditor;
            if (treeListEditor!=null)
                return new TreeListEditorDynamicModelSynchronizer(treeListEditor.TreeList, treeListEditor.Model);
            var categorizedListEditor = listEditor as CategorizedListEditor;
            if (categorizedListEditor != null){
                var objectTreeList = (ObjectTreeList) categorizedListEditor.CategoriesListView.Control;
                return new TreeListEditorDynamicModelSynchronizer(objectTreeList, listEditor.Model);
            }
            return null;
        }

        protected override Type TreeListColumnType() {
            return typeof(TreeListColumn);
        }

        protected override Type TreeListType() {
            return typeof(TreeList);
        }

    }
}
