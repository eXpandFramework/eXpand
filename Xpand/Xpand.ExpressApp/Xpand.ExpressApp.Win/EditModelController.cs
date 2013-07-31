using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win {
    public class EditModelController : DevExpress.ExpressApp.Win.SystemModule.EditModelController {
        public EditModelController() {
            TargetWindowType = WindowType.Main;
        }

        protected override void EditModel() {
            SecuritySystem.ReloadPermissions();
            typeof(DevExpress.ExpressApp.Win.SystemModule.EditModelController).Invoke(this, "UpdateActivity");
            if (SecuritySystem.Instance is IRequestSecurity) {
                SecuritySystem.Demand(new ModelOperationPermissionRequest());
            } else {
                SecuritySystem.Demand(new EditModelPermission(ModelAccessModifier.Allow));
            }
            EditModelCore();
        }


        void EditModelCore() {
            var winApplication = (WinApplication)Application;
            if (!winApplication.ShowViewStrategy.CloseAllWindows()) {
                return;
            }
            var differenceStore = (ModelDifferenceStore)typeof(XafApplication).Invoke(winApplication, "CreateUserModelDifferenceStore");
            if (differenceStore != null) {
                differenceStore.SaveDifference(((ModelApplicationBase)winApplication.Model).LastLayer);
            }
            ICurrentAspectProvider oldAspectProvider = ((ModelApplicationBase)winApplication.Model).CurrentAspectProvider;
            try {
                ((ModelApplicationBase)winApplication.Model).CurrentAspectProvider = new CurrentAspectProvider(oldAspectProvider.CurrentAspect);
                using (Form modelEditorForm = ModelEditorViewController.CreateModelEditorForm(winApplication)) {
                    modelEditorForm.ShowDialog();
                    if (modelEditorForm is IModelEditorSettings) {
                        if (differenceStore != null) {
                            differenceStore.SaveDifference(((ModelApplicationBase)winApplication.Model).LastLayer);
                        }
                    }
                }
            } finally {
                ((ModelApplicationBase)winApplication.Model).CurrentAspectProvider = oldAspectProvider;
            }
            try {
                typeof(WinApplication).Invoke(winApplication, "RefreshShowViewStrategy");
                typeof(WinApplication).Invoke(winApplication, "ShowStartupWindow");
            } catch (Exception e) {
                winApplication.HandleException(e);
                winApplication.Exit();
            }

        }
    }
}