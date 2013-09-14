using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelWarnForUnsavedChanges {
        [Category("eXpand")]
        bool WarnForUnsavedChanges { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelWarnForUnsavedChanges), "Options")]
    public interface IModelClassWarnForUnsavedChanges:IModelWarnForUnsavedChanges {
        [Browsable(false)]
        [ModelValueCalculator("Application.Options")]
        IModelOptions Options { get; }
    }
    [ModelInterfaceImplementor(typeof(IModelWarnForUnsavedChanges), "ModelClass")]
    public interface IModelDetailViewWarnForUnsavedChanges : IModelWarnForUnsavedChanges {
    }
    [DesignerCategory("Code")]
    public class UnsavedObjectController : ViewController<DetailView>,IModelExtender {
        private Boolean _canExitEditMode;
        private Boolean _cancelActionTriggered;
        private Boolean _exitEditModeByCancel;
        private Boolean _isObjectSpaceModified;
        private Boolean _isWarningShown;
        const String ActionActiveID = "ActionActiveReason";

        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelDetailViewWarnForUnsavedChanges) View.Model).WarnForUnsavedChanges) {
                _canExitEditMode = false;
                _cancelActionTriggered = false;
                _isObjectSpaceModified = false;
                _exitEditModeByCancel = false;
                _isWarningShown = false;

                AdjustUIForMode(View.ViewEditMode);

                View.ViewEditModeChanged += View_ViewEditModeChanged;
                View.QueryCanClose += View_QueryCanClose;
                View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            }
        }

        protected override void OnDeactivated() {
            if (((IModelDetailViewWarnForUnsavedChanges)View.Model).WarnForUnsavedChanges) {
                View.ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
                View.QueryCanClose -= View_QueryCanClose;
                View.ViewEditModeChanged -= View_ViewEditModeChanged;

                AdjustUIForMode(ViewEditMode.View);
            }
            base.OnDeactivated();
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (!_isObjectSpaceModified) _isObjectSpaceModified = e.OldValue != e.NewValue;
        }

        void View_ViewEditModeChanged(object sender, EventArgs e) {
            AdjustUIForMode(View.ViewEditMode);
        }

        void View_QueryCanClose(object sender, CancelEventArgs e) {
            e.Cancel = !IsExitEditModeAllowed();
        }

        void HandleDetailActions(object sender, CancelEventArgs e) {
            var anAction = ((ActionBase)sender);
            if (anAction.Id == "Cancel") {
                _cancelActionTriggered = !_cancelActionTriggered;
                _canExitEditMode = !_cancelActionTriggered;
                _exitEditModeByCancel = true;
                e.Cancel = !IsExitEditModeAllowed();
            } else {
                _cancelActionTriggered = false;
                _canExitEditMode = true;
                _exitEditModeByCancel = false;
            }
        }

        protected Boolean IsExitEditModeAllowed() {
            if ((View.ViewEditMode == ViewEditMode.Edit) && !_canExitEditMode && _isObjectSpaceModified && !(_isWarningShown && _exitEditModeByCancel)) {
                var unsavedObjectWarning = CaptionHelper.GetLocalizedText(XpandSystemAspNetModule.XpandWeb, _exitEditModeByCancel ? "CancelAgain" : "UnSavedChanges");
                DevExpress.ExpressApp.Web.ErrorHandling.Instance.SetPageError(new UserFriendlyException(unsavedObjectWarning));
                _isWarningShown = true;
                return false;
            }
            return true;
        }

        protected void AdjustUIForMode(ViewEditMode EditMode) {
            if (EditMode == ViewEditMode.Edit) {
                Frame.GetController<WebModificationsController>().CancelAction.Executing += HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAction.Executing += HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAndCloseAction.Executing += HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAndNewAction.Executing += HandleDetailActions;
            } else {
                Frame.GetController<WebModificationsController>().CancelAction.Executing -= HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAction.Executing -= HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAndCloseAction.Executing -= HandleDetailActions;
                Frame.GetController<WebModificationsController>().SaveAndNewAction.Executing -= HandleDetailActions;
            }

            Frame.GetController<WebRecordsNavigationController>().Active[ActionActiveID] = EditMode == ViewEditMode.View;
            Frame.GetController<RefreshController>().Active[ActionActiveID] = EditMode == ViewEditMode.View;
            Frame.GetController<WebNewObjectViewController>().Active[ActionActiveID] = EditMode == ViewEditMode.View;
            Frame.GetController<WebDeleteObjectsViewController>().Active[ActionActiveID] = EditMode == ViewEditMode.View;
            Frame.GetController<CloneObjectViewController>().Active[ActionActiveID] = EditMode == ViewEditMode.View;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions,IModelWarnForUnsavedChanges>();
            extenders.Add<IModelClass,IModelClassWarnForUnsavedChanges>();
            extenders.Add<IModelDetailView,IModelDetailViewWarnForUnsavedChanges>();
        }
    }
}
