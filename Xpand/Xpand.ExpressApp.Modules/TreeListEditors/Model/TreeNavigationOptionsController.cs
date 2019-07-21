//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//
//namespace Xpand.ExpressApp.TreeListEditors.Model {
//    public abstract class TreeNavigationOptionsController<TNavigationActionContainer, TTReeList> : WindowController
//        where TNavigationActionContainer : class, IActionContainer
//        where TTReeList : class {
//        protected TreeNavigationOptionsController() {
//            TargetWindowType = WindowType.Main;
//        }
//        protected override void OnFrameAssigned() {
//            base.OnFrameAssigned();
//            if (Frame.Context==TemplateContext.ApplicationWindow)
//                Frame.ProcessActionContainer += FrameOnProcessActionContainer;
//        }
//
//        protected void FrameOnProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
//            var container = e.ActionContainer as TNavigationActionContainer;
//            if (container != null) {
//                var navigationActionContainer = container;
//                var treeList = GetTreeList(navigationActionContainer);
//                if (treeList != null) {
//                    var modelOptionsTreeList = ((IModelListViewOptionsTreeListNavigation)((IModelApplicationNavigationItems)Application.Model).NavigationItems).TreeListOptions;
//                    var treeListOptionsModelSynchronizer = TreeListOptionsModelSynchronizer(treeList, modelOptionsTreeList);
//                    treeListOptionsModelSynchronizer.ApplyModel();
//                }
//            }
//        }
//
//        protected abstract ModelSynchronizer TreeListOptionsModelSynchronizer(TTReeList treeList, IModelOptionsTreeList modelOptionsTreeList);
//
//        protected abstract TTReeList GetTreeList(TNavigationActionContainer navigationActionContainer);
//    }
//}
