using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.WizardUI.Win {
    public static class Extensions {
        internal static void CreateWizardViewInternal(this ActionBaseEventArgs e,IObjectSpace objectSpace, object newObject, View sourceView) {
            if (CanCreateView(e)) {
                e.CreateWizardView(objectSpace, newObject, sourceView);
            }
        }

        public static void CreateWizardView(this ActionBaseEventArgs e, IObjectSpace objectSpace, object newObject, View sourceView){
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Context = "WizardDetailViewForm";
            if (e.ShowViewParameters.CreatedView == null){
                e.ShowViewParameters.CreatedView = e.Action.Application.CreateDetailView(objectSpace, newObject, sourceView);
            }
        }

        private static IModelDetailViewWizard GetModelDetailViewWizard(ActionBaseEventArgs e) {
            if (e.ShowViewParameters.CreatedView != null) {
                return e.ShowViewParameters.CreatedView.Model as IModelDetailViewWizard;
            }
            if (e.ShowViewParameters.CreatedView == null && e.Action.Controller is NewObjectViewController) {
                var viewID = e.Action.Application.GetDetailViewId(((SingleChoiceActionExecuteEventArgs)e).SelectedChoiceActionItem.Data as Type);
                return e.Action.Application.Model.Views[viewID] as IModelDetailViewWizard;
            }
            return null;
        }

        static bool CanCreateView(ActionBaseEventArgs e) {
            var modelWizard = GetModelDetailViewWizard(e);
            var canCreate = modelWizard != null && modelWizard.Wizard.Count > 0 && modelWizard.Wizard.ShowInWizard;
            return canCreate && (!modelWizard.Wizard.NewObjectsOnly || ((e.Action.Controller is NewObjectViewController) && modelWizard.Wizard.NewObjectsOnly));
        }

    }

}
