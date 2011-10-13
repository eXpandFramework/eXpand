using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Validation {
    public class ActionExecuteValidationController : ObjectViewController {



        public event EventHandler<CustomGetAggregatedObjectsToValidateEventArgs> CustomGetAggregatedObjectsToValidate;
        public event EventHandler<NeedToValidateObjectEventArgs> NeedToValidateObject;

        protected virtual void OnContextValidating(ContextValidatingEventArgs args) {
            if (ContextValidating != null) {
                ContextValidating(this, args);
            }
        }
        public event EventHandler<ContextValidatingEventArgs> ContextValidating;
        private void CustomizeDeleteValidationException(ValidationCompletedEventArgs args) {
            args.Exception.MessageHeader = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageHeader);
            args.Exception.ObjectHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageObjectFormat);
        }


        protected override void OnDeactivated() {
            base.OnDeactivated();
            foreach (var controller in Frame.Controllers) {
                foreach (var action in controller.Actions) {
                    action.Executed -= ActionOnExecuted;
                }
            }
        }


        protected override void OnActivated() {
            base.OnActivated();
            foreach (var controller in Frame.Controllers) {
                foreach (var action in controller.Actions) {
                    action.Executed += ActionOnExecuted;
                }
            }
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            if (View.ObjectTypeInfo.Type != typeof(ValidationResults)) {
                ValidationTargetObjectSelector deleteSelector = new ActionExecuteContextTargetObjectSelector();
                SubscribeSelectorEvents(deleteSelector);
                var selectedObjects = ((SimpleActionExecuteEventArgs)actionBaseEventArgs).SelectedObjects;
                var context = actionBaseEventArgs.Action.Id;

                var deleteContextArgs = new ContextValidatingEventArgs(context, new ArrayList(selectedObjects));
                OnContextValidating(deleteContextArgs);
                Validator.RuleSet.ValidateAll(deleteContextArgs.TargetObjects, context, CustomizeDeleteValidationException);
            }
        }

        private void OnSelectorCustomGetAggregatedObjectsToValidate(object sender, CustomGetAggregatedObjectsToValidateEventArgs args) {
            if (CustomGetAggregatedObjectsToValidate != null) {
                CustomGetAggregatedObjectsToValidate(this, args);
            }
        }

        private void OnSelectorNeedToValidateObject(object sender, NeedToValidateObjectEventArgs args) {
            if (NeedToValidateObject != null) {
                NeedToValidateObject(this, args);
            }
        }

        private void SubscribeSelectorEvents(ValidationTargetObjectSelector selector) {
            selector.CustomNeedToValidateObject += OnSelectorNeedToValidateObject;
            selector.CustomGetAggregatedObjectsToValidate += OnSelectorCustomGetAggregatedObjectsToValidate;
        }

    }
    public class ActionExecuteContextTargetObjectSelector : ValidationTargetObjectSelector {
        protected override bool NeedToValidateObject(Session session, object targetObject) {
            return true;
        }
    }

}
