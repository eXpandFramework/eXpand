using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace FeatureCenter.Module.Miscellaneous.ExceptionHandling {
    public class ThrowExceptionController:ViewController<DetailView> {
        public ThrowExceptionController() {
            TargetViewId = "ExceptionHandling_DetailView";
            var simpleAction = new SimpleAction(this, "Throw Exception", PredefinedCategory.ObjectsCreation);
            simpleAction.Execute+=SimpleActionOnExecute;
            simpleAction = new SimpleAction(this, "Throw Validation Exception", PredefinedCategory.ObjectsCreation);
            simpleAction.Execute+=SimpleActionOnExecute;
        }


        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            if (((ActionBase)sender).Id == "Throw Exception")
                throw new Exception("Exception was thrown");
            
            var result = new RuleSetValidationResult();
            const string messageTemplate = "This exception will not be logged see FeatureCenterWindowsFormsModule ExceptionHandlingWinModuleOnCustomHandleException method ";
            result.AddResult(new RuleSetValidationResultItem(View.CurrentObject, ContextIdentifier.Delete, null,
                                                             new RuleValidationResult(null, this, ValidationState.Invalid,
                                                                                      messageTemplate)));

            throw new ValidationException(messageTemplate, result);
        }
    }
}