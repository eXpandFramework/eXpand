using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
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
        private Boolean _isWarningShown;
        private bool _isObjectSpaceModified;

        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelDetailViewWarnForUnsavedChanges) View.Model).WarnForUnsavedChanges) {
                View.ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
                _canExitEditMode = false;
                _cancelActionTriggered = false;
                _exitEditModeByCancel = false;
                _isWarningShown = false;
                _isObjectSpaceModified = false;

                AdjustUIForMode(View.ViewEditMode);

                View.ViewEditModeChanged += View_ViewEditModeChanged;
                View.QueryCanClose += View_QueryCanClose;
                View.ObjectSpace.Refreshing+=ObjectSpaceOnRefreshing;
            }
        }

        protected override void OnDeactivated() {
            if (((IModelDetailViewWarnForUnsavedChanges)View.Model).WarnForUnsavedChanges) {
                View.QueryCanClose -= View_QueryCanClose;
                View.ViewEditModeChanged -= View_ViewEditModeChanged;
                View.ObjectSpace.Refreshing-=ObjectSpaceOnRefreshing;
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                AdjustUIForMode(ViewEditMode.View);
            }
            base.OnDeactivated();
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
            if (!_isObjectSpaceModified) _isObjectSpaceModified = e.OldValue != e.NewValue;
        }

        private void ObjectSpaceOnRefreshing(object sender, CancelEventArgs cancelEventArgs){
            cancelEventArgs.Cancel = !IsExitEditModeAllowed();
        }

        void View_ViewEditModeChanged(object sender, EventArgs e) {
            AdjustUIForMode(View.ViewEditMode);
        }

        void View_QueryCanClose(object sender, CancelEventArgs e) {
            e.Cancel = !IsExitEditModeAllowed();
        }

        void HandleWebModificationActions(object sender, CancelEventArgs e) {
            var anAction = ((ActionBase)sender);
            if (anAction.Id == Frame.GetController<WebModificationsController>().CancelAction.Id) {
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
                ErrorHandling.Instance.SetPageError(new UserFriendlyException(unsavedObjectWarning));
                _isWarningShown = true;
                return false;
            }
            return true;
        }

        protected void AdjustUIForMode(ViewEditMode editMode){
            var webRecordsNavigationController = Frame.GetController<WebRecordsNavigationController>();
            var webModificationsController = Frame.GetController<WebModificationsController>();
            if (editMode == ViewEditMode.Edit) {
                webModificationsController.CancelAction.Executing += HandleWebModificationActions;
                webModificationsController.SaveAction.Executing += HandleWebModificationActions;
                webModificationsController.SaveAndCloseAction.Executing += HandleWebModificationActions;
                webModificationsController.SaveAndNewAction.Executing += HandleWebModificationActions;
                webRecordsNavigationController.PreviousObjectAction.Executing+=HandleAllOtherActions;
                webRecordsNavigationController.NextObjectAction.Executing+=HandleAllOtherActions;
            } else {
                webModificationsController.CancelAction.Executing -= HandleWebModificationActions;
                webModificationsController.SaveAction.Executing -= HandleWebModificationActions;
                webModificationsController.SaveAndCloseAction.Executing -= HandleWebModificationActions;
                webModificationsController.SaveAndNewAction.Executing -= HandleWebModificationActions;
                webRecordsNavigationController.PreviousObjectAction.Executing -= HandleAllOtherActions;
                webRecordsNavigationController.NextObjectAction.Executing -= HandleAllOtherActions;
            }
        }

        private void HandleAllOtherActions(object sender, CancelEventArgs e){
            e.Cancel = !IsExitEditModeAllowed();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions,IModelWarnForUnsavedChanges>();
            extenders.Add<IModelClass,IModelClassWarnForUnsavedChanges>();
            extenders.Add<IModelDetailView,IModelDetailViewWarnForUnsavedChanges>();
        }
    }
}
