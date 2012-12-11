using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;

namespace Xpand.ExpressApp.TreeListEditors.Model {
    public abstract class TreeNavigationOptionsController<NavigationActionContainer, TreeList> : WindowController
        where NavigationActionContainer : class, IActionContainer
        where TreeList : class {
        protected TreeNavigationOptionsController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ProcessActionContainer += FrameOnProcessActionContainer;
        }

        void FrameOnProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
            var container = e.ActionContainer as NavigationActionContainer;
            if (container != null) {
                var navigationActionContainer = container;
                var treeList = GetTreeList(navigationActionContainer);
                if (treeList != null) {
                    var modelOptionsTreeList = ((IModelListViewOptionsTreeListNavigation)((IModelApplicationNavigationItems)Application.Model).NavigationItems).TreeListOptions;
                    var treeListOptionsModelSynchronizer = TreeListOptionsModelSynchronizer(treeList, modelOptionsTreeList);
                    treeListOptionsModelSynchronizer.ApplyModel();
                }
            }
        }

        protected abstract ModelSynchronizer TreeListOptionsModelSynchronizer(TreeList treeList, IModelOptionsTreeList modelOptionsTreeList);

        protected abstract TreeList GetTreeList(NavigationActionContainer navigationActionContainer);
    }
}
