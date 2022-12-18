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

        protected virtual void OnContextValidating(ContextValidatingEventArgs args){
            ContextValidating?.Invoke(this, args);
        }

        private void CustomizeDeleteValidationException(ValidationCompletedEventArgs args) {
            args.Exception.MessageHeader = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageHeader);
            args.Exception.ObjectHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageObjectFormat);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (Enabled){
                if (Validator.GetService(Site) != null) Validator.GetService(Site).ValidationCompleted -= RuleSetOnValidationCompleted;
                foreach (var actionBase in _actionBases){
                    actionBase.Executing-=ActionBaseOnExecuting;
                }
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (Enabled){
                Validator.GetService(Site).ValidationCompleted+=RuleSetOnValidationCompleted;
                _actionBases = Frame.Actions().Where(@base => !new[] { "Save", "Delete","Validate" }.Contains(@base.Id)).ToArray();
                foreach (var actionBase in _actionBases){
                    actionBase.Executing+=ActionBaseOnExecuting;
                }
            }
        }

        private ValidationTargetObjectSelector CreateSaveContextTargetObjectSelector() {
            if((View != null) && (View is ListView) 
                              && (View.Model != null) && (View.Model is IModelListView) && (((IModelListView)View.Model).MasterDetailMode == MasterDetailMode.ListViewOnly)) {
                return new SaveContextTargetObjectSelectorModifiedOnly();
            }
            return new SaveContextTargetObjectSelector();
        }

        private void ActionBaseOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            if (View != null && View.ObjectTypeInfo.Type != typeof(ValidationResults)) {
                var selector = CreateSaveContextTargetObjectSelector();
                SubscribeSelectorEvents(selector);
                var selectedObjects = selector.GetObjectsToValidate(ObjectSpace, View.CurrentObject);
                var context = ((ActionBase) sender).Id;
                var contextValidatingEventArgs = new ContextValidatingEventArgs(context, new ArrayList(selectedObjects));
                OnContextValidating(contextValidatingEventArgs);
                if (View.ObjectTypeInfo.IsPersistent && CanAccessDeletedObjects(context)&&!_otherValidationContextFailed)
                    Validator.GetService(Site).ValidateAll(ObjectSpace, contextValidatingEventArgs.TargetObjects, context, CustomizeDeleteValidationException);
            }
            _otherValidationContextFailed = false;
        }

        private bool Enabled => ((IModelValidationContextsActions) ((IModelApplicationValidation) Application.Model).Validation.Contexts).ActionContexts;

        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs validationCompletedEventArgs) {
//            _otherValidationContextFailed = !validationCompletedEventArgs.Successful;
        }


        bool CanAccessDeletedObjects(string context) {
            return !(context == "Delete" && !ObjectSpace.IsDeletionDeferredType(View.ObjectTypeInfo.Type));
        }

        private void OnSelectorCustomGetAggregatedObjectsToValidate(object sender, CustomGetAggregatedObjectsToValidateEventArgs args){
            CustomGetAggregatedObjectsToValidate?.Invoke(this, args);
        }

        private void OnSelectorNeedToValidateObject(object sender, NeedToValidateObjectEventArgs args){
            NeedToValidateObject?.Invoke(this, args);
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
