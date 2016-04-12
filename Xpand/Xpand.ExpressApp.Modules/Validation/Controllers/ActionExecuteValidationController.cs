using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Validation.Controllers {
    public interface IModelValidationContextsActions{
        [Category("eXpand")]
        bool ActionContexts { get; set; }
    }
    public class ActionExecuteValidationController : ObjectViewController,IModelExtender {
        bool _otherValidationContextFailed;
        private ActionBase[] _actionBases;
        public event EventHandler<CustomGetAggregatedObjectsToValidateEventArgs> CustomGetAggregatedObjectsToValidate;
        public event EventHandler<NeedToValidateObjectEventArgs> NeedToValidateObject;
        public event EventHandler<ContextValidatingEventArgs> ContextValidating;

        protected virtual void OnContextValidating(ContextValidatingEventArgs args) {
            if (ContextValidating != null) {
                ContextValidating(this, args);
            }
        }

        private void CustomizeDeleteValidationException(ValidationCompletedEventArgs args) {
            args.Exception.MessageHeader = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageHeader);
            args.Exception.ObjectHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageObjectFormat);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (Enabled){
                if (Validator.RuleSet != null) Validator.RuleSet.ValidationCompleted -= RuleSetOnValidationCompleted;
                foreach (var actionBase in _actionBases){
                    actionBase.Executed -= ActionOnExecuted;
                }
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (Enabled){
                Validator.RuleSet.ValidationCompleted+=RuleSetOnValidationCompleted;
                _actionBases = Frame.Actions().Where(@base => !new[] { "Save", "Delete","Validate" }.Contains(@base.Id)).ToArray();
                foreach (var actionBase in _actionBases){
                    actionBase.Executed+=ActionOnExecuted;
                }
            }
        }

        private bool Enabled{
            get{
                return ((IModelValidationContextsActions) ((IModelApplicationValidation) Application.Model).Validation.Contexts).ActionContexts;
            }
        }

        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs validationCompletedEventArgs) {
            _otherValidationContextFailed = !validationCompletedEventArgs.Successful;
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            if (View != null && View.ObjectTypeInfo.Type != typeof(ValidationResults)) {
                ValidationTargetObjectSelector deleteSelector = new ActionExecuteContextTargetObjectSelector();
                SubscribeSelectorEvents(deleteSelector);
                var selectedObjects = ((SimpleActionExecuteEventArgs)actionBaseEventArgs).SelectedObjects;
                var context = actionBaseEventArgs.Action.Id;
                var contextValidatingEventArgs = new ContextValidatingEventArgs(context, new ArrayList(selectedObjects));
                OnContextValidating(contextValidatingEventArgs);
                if (View.ObjectTypeInfo.IsPersistent && CanAccessDeletedObjects(context)&&!_otherValidationContextFailed)
                    Validator.RuleSet.ValidateAll(ObjectSpace, contextValidatingEventArgs.TargetObjects, context, CustomizeDeleteValidationException);
                _otherValidationContextFailed = false;
            }
        }

        bool CanAccessDeletedObjects(string context) {
            return !(context == "Delete" && !ObjectSpace.IsDeletionDeferredType(View.ObjectTypeInfo.Type));
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

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelValidationContexts,IModelValidationContextsActions>();
        }
    }
    public class ActionExecuteContextTargetObjectSelector : ValidationTargetObjectSelector {

        protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
            return true;
        }
    }

}
