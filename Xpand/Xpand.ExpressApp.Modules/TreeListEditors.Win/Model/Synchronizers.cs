using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public TreeListEditorDynamicModelSynchronizer(TreeListEditor editor)
            : base(editor.Control, editor.Model) {
            ModelSynchronizerList.Add(new TreeListViewOptionsSynchronizer(editor.TreeList, ((IModelListViewOptionsTreeList)Model).TreeListOptions));
            ModelSynchronizerList.Add(new TreeListColumnOptionsSynchronizer(editor.TreeList, (IModelListViewOptionsTreeList)Model));
        }

    }
    public class TreeListViewOptionsSynchronizer : TreeListViewOptionsSynchronizer<TreeList> {
        public TreeListViewOptionsSynchronizer(TreeList component, IModelOptionsTreeList modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore(){
            base.ApplyModelCore();
            var modelListViewShowAutoFilterRow = (Model.Parent as IModelListViewShowAutoFilterRow);
            if (modelListViewShowAutoFilterRow != null && modelListViewShowAutoFilterRow.ShowAutoFilterRow) {
                Control.OptionsBehavior.EnableFiltering = true;
                Control.OptionsFilter.FilterMode=FilterMode.Smart;
                var optionsMenu = Control.OptionsMenu;
                optionsMenu.EnableColumnMenu = true;
                optionsMenu.ShowAutoFilterRowItem = true;
                Control.OptionsView.ShowAutoFilterRow = true;
            }
            var modelListViewShowFindPanel = (Model.Parent as IModelListViewShowFindPanel);
            if (modelListViewShowFindPanel != null && modelListViewShowFindPanel.ShowFindPanel){
                Control.OptionsFind.AllowFindPanel = true;
            }
        }
    }

    public class TreeListColumnOptionsSynchronizer : TreeListColumnOptionsSynchronizer<TreeList> {
        public TreeListColumnOptionsSynchronizer(TreeList component, IModelListViewOptionsTreeList modelNode)
            : base(component, modelNode) {
        }


        protected override object Component(IModelColumnOptionsTreeListView column) {
            return Control.Columns[column.PropertyName];
        }
    }
}
