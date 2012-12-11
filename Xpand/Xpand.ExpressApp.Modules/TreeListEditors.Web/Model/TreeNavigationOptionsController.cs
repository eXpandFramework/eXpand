using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Model;

namespace Xpand.ExpressApp.TreeListEditors.Web.Model {
    public class TreeNavigationOptionsController : TreeNavigationOptionsController<NavigationActionContainer, ASPxTreeList> {
        protected override ModelSynchronizer TreeListOptionsModelSynchronizer(ASPxTreeList list, IModelOptionsTreeList modelOptionsTreeList) {
            return new TreeListViewOptionsSynchronizer(list, modelOptionsTreeList);
        }

        protected override ASPxTreeList GetTreeList(NavigationActionContainer navigationActionContainer) {
            return navigationActionContainer.NavigationControl as ASPxTreeList;
        }
    }
}
