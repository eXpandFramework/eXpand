using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;

namespace Xpand.ExpressApp.TreeListEditors.Win.Model {
    public class TreeNavigationOptionsController : TreeNavigationOptionsController<NavigationActionContainer, TreeList> {
        protected override ModelSynchronizer TreeListOptionsModelSynchronizer(TreeList list, IModelOptionsTreeList modelOptionsTreeList) {
            return new TreeListViewOptionsSynchronizer(list, modelOptionsTreeList);
        }

        protected override TreeList GetTreeList(NavigationActionContainer navigationActionContainer) {
            return navigationActionContainer.NavigationControl as TreeList;
        }
    }
}
