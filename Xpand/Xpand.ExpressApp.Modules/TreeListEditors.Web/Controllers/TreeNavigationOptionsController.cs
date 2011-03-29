using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.TreeListEditors.Web.Core;

namespace Xpand.ExpressApp.TreeListEditors.Web.Controllers {
    public class TreeNavigationOptionsController : WindowController {
        public TreeNavigationOptionsController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ProcessActionContainer+=FrameOnProcessActionContainer;
        }

        void FrameOnProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
            if (e.ActionContainer is NavigationActionContainer) {
                var navigationActionContainer =(NavigationActionContainer)e.ActionContainer;
                if (navigationActionContainer.NavigationControl is ASPxTreeList) {
                    var treeListOptionsModelSynchronizer = new TreeListOptionsModelSynchronizer<IModelRootNavigationItems>(navigationActionContainer.NavigationControl,
                                                                                                                           ((IModelApplicationNavigationItems)Application.Model).NavigationItems);
                    treeListOptionsModelSynchronizer.ApplyModel();
                }
            }
        }

    }
}
