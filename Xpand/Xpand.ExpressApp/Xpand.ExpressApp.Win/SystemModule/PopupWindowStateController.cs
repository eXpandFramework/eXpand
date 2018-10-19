using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelViewPopupWindowState {
        [Category(AttributeCategoryNameProvider.Xpand+".PopupWindowState")]
        FormWindowState? PopupWindowState { get; set; }
    }
    public class PopupWindowStateController:Controller,IModelExtender {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateViewChanged+=FrameOnTemplateViewChanged;
            Frame.Disposed+=FrameOnDisposed;
        }

        private void FrameOnDisposed(object sender, EventArgs e) {
            var frame = ((Frame) sender);
            frame.Disposed-=FrameOnDisposed;
            frame.TemplateViewChanged-=FrameOnTemplateViewChanged;
        }

        private void FrameOnTemplateViewChanged(object sender, EventArgs e) {
            var modelViewPopupWindowState = ((IModelViewPopupWindowState) ((Frame) sender).View?.Model);
            if (modelViewPopupWindowState?.PopupWindowState != null) {
                ((Form) Frame.Template).WindowState=modelViewPopupWindowState.PopupWindowState.Value;
                modelViewPopupWindowState.PopupWindowState = null;
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelView,IModelViewPopupWindowState>();
        }
    }
}