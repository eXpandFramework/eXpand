using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelDetailViewViewEditMode  {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Control detail view default edit mode")]
        ViewEditMode? ViewEditMode { get; set; }
    }

    public class ViewEditModeController : WindowController, IModelExtender {
        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Application.DetailViewCreating -= ApplicationOnDetailViewCreating;
            Frame.Disposing -= FrameOnDisposing;
            foreach (var action in Frame.Actions()) {
                action.Executed -= ActionOnExecuted;
            }
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Application.DetailViewCreating+=ApplicationOnDetailViewCreating;
            Frame.Disposing+=FrameOnDisposing;
            foreach (var action in Frame.Actions()){
                action.Executed+=ActionOnExecuted;
            }
        }

        private void ApplicationOnDetailViewCreating(object sender, DetailViewCreatingEventArgs e){
            e.View.ControlsCreated+=ViewOnControlsCreated;
        }

        private void ViewOnControlsCreated(object sender, EventArgs eventArgs){
            var view = ((View) sender);
            view.ControlsCreated-=ViewOnControlsCreated;
            UpdateView((DetailView) view);
        }

        private void ActionOnExecuted(object sender, ActionBaseEventArgs e){
            var detailView = e.ShowViewParameters.CreatedView as DetailView;
            if (detailView!=null){
                UpdateView(detailView);
            }
        }

        private void UpdateView(DetailView detailView){
            var viewEditMode = ((IModelDetailViewViewEditMode) detailView.Model).ViewEditMode;
            if (viewEditMode.HasValue){
                if (!detailView.ObjectSpace.IsNewObject(detailView.CurrentObject)){
                    UpdateViewEditModeState(detailView, viewEditMode.Value);
                    UpdateViewAllowEditState(detailView);
                }
            }
        }

        protected virtual void UpdateViewEditModeState(DetailView view,ViewEditMode viewEditMode) {
            view.ViewEditMode = viewEditMode;
        }

        protected virtual void UpdateViewAllowEditState(DetailView view) {
            if (!XpandModuleBase.IsHosted)
                view.AllowEdit["ViewEditMode"] = view.ViewEditMode == ViewEditMode.Edit;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDetailView, IModelDetailViewViewEditMode>();
        }
    }
}