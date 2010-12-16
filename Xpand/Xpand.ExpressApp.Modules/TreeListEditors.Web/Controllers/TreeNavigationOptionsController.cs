using System;
using System.Linq;
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
        protected override void OnActivated() {
            base.OnActivated();
            Window.TemplateChanged += FrameOnTemplateChanged;
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            if (Window.Template != null) {
                var navigationActionContainer = Window.Template.GetContainers().OfType<NavigationActionContainer>().FirstOrDefault();
                if (navigationActionContainer != null && navigationActionContainer.NavigationControl is ASPxTreeList) {
                    var treeListOptionsModelSynchronizer = new TreeListOptionsModelSynchronizer<IModelRootNavigationItems>(navigationActionContainer.NavigationControl,
                                                                                                                           ((IModelApplicationNavigationItems)Application.Model).NavigationItems);
                    treeListOptionsModelSynchronizer.ApplyModel();
                }
            }
        }
    }
}
