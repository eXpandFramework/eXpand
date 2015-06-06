using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelDetailViewViewEditMode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Control detail view default edit mode")]
        ViewEditMode? ViewEditMode { get; set; }
    }

    public class ViewEditModeController : WindowController, IModelExtender {
        public const string ViewActiveKey = "ViewEditMode";
        private void FrameOnDisposing(object sender, EventArgs eventArgs) {
//            Application.DetailViewCreated -= ApplicationOnDetailViewCreated;
            Frame.ViewChanging-=FrameOnViewChanging;
            Frame.Disposing -= FrameOnDisposing;
            foreach (var action in Frame.Actions()) {
                action.Executed -= ActionOnExecuted;
            }
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanging+=FrameOnViewChanging;
//            Application.DetailViewCreated += ApplicationOnDetailViewCreated;
            Frame.Disposing += FrameOnDisposing;
            foreach (var action in Frame.Actions()) {
                action.Executed += ActionOnExecuted;
            }
        }

        private void FrameOnViewChanging(object sender, ViewChangingEventArgs e){
            var detailView = e.View as DetailView;
            if (detailView != null){
                detailView.ControlsCreated += ViewOnControlsCreated;
                detailView.ObjectSpace.Reloaded += (o, args) => UpdateEditableActions(detailView);
            }
        }

        private void ViewOnControlsCreated(object sender, EventArgs eventArgs) {
            var view = ((View)sender);
            view.ControlsCreated -= ViewOnControlsCreated;
            UpdateView((DetailView)view);
            if (!Application.IsHosted())
                UpdateEditableActions(view);
            view.ObjectSpace.Reloaded += (o, args) => UpdateEditableActions(view);
        }

        private void ActionOnExecuted(object sender, ActionBaseEventArgs e) {
            var detailView = e.ShowViewParameters.CreatedView as DetailView;
            if (detailView != null) {
                UpdateView(detailView);
            }
        }

        private void UpdateView(DetailView detailView) {
            var viewEditMode = ((IModelDetailViewViewEditMode)detailView.Model).ViewEditMode;
            if (viewEditMode.HasValue) {
                if (!detailView.ObjectSpace.IsNewObject(detailView.CurrentObject)) {
                    UpdateViewEditModeState(detailView, viewEditMode.Value);
                    UpdateViewAllowEditState(detailView);
                }
            }
        }

        protected virtual void UpdateViewEditModeState(DetailView view, ViewEditMode viewEditMode) {
            view.ViewEditMode = viewEditMode;
        }

        protected virtual void UpdateViewAllowEditState(DetailView view) {
            if (!ApplicationHelper.Instance.Application.IsHosted()) {
                view.AllowEdit[ViewActiveKey] = view.ViewEditMode == ViewEditMode.Edit;
            }
        }

        private void UpdateEditableActions(View view) {
            var modificationsController = Frame.GetController<ModificationsController>();
            modificationsController.SaveAction.Active[typeof(ViewEditModeController).Name] = view.AllowEdit;
            modificationsController.SaveAndCloseAction.Active[typeof(ViewEditModeController).Name] = view.AllowEdit;
            modificationsController.CancelAction.Active[typeof(ViewEditModeController).Name] = view.AllowEdit;
            Frame.GetController<RefreshController>().RefreshAction.Active[typeof(ViewEditModeController).Name] = view.AllowEdit;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDetailView, IModelDetailViewViewEditMode>();
        }
    }
}