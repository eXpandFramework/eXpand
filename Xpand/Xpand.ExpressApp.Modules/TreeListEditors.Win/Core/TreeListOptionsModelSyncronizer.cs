using DevExpress.ExpressApp.Model;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.TreeListEditors.Win.Controllers;

namespace Xpand.ExpressApp.TreeListEditors.Win.Core
{
    public class TreeListOptionsModelSynchronizer : OptionsModelSynchronizer<object, IModelListView, IModelTreeViewOptionsBase>
    {
        public TreeListOptionsModelSynchronizer(object control, IModelListView model)
            : base(control, model)
        {
        }

        protected override object GetControl()
        {
            return Control;
        }

        protected override void ApplyModelCore()
        {
            base.ApplyModelCore();

            var control = (TreeList)Control;
            var treeListModel = (Model as IModelTreeViewMainOptions).TreeListOptions;
            if (!string.IsNullOrEmpty(treeListModel.PreviewFieldName))
                control.PreviewFieldName = treeListModel.PreviewFieldName;

            if (treeListModel.PreviewLineCount.HasValue)
                control.PreviewLineCount = treeListModel.PreviewLineCount.Value;

            if (treeListModel.RowHeight.HasValue)
                control.RowHeight = treeListModel.RowHeight.Value;
        }
    }
}
