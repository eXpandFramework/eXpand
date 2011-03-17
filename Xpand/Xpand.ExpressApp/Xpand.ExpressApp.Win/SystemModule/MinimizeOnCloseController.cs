using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsMinimizeOnCloseOptions : IModelNode {
        [Category("eXpand")]
        [Description("Minimize application on closing instead of closing")]
        bool MinimizeOnClose { get; set; }
    }

    public class MinimizeOnCloseController : WindowController, IModelExtender {
        private static bool editing;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            if (Frame.Context == TemplateContext.ApplicationWindow &&
                ((IModelOptionsMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose) {
                var form = Frame.Template as XtraForm;
                if (form != null) {
                    form.FormClosing += FormOnFormClosing;
                    SimpleAction action =
                    Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().EditModelAction;
                    action.Executing += (o, eventArgs) => editing = true;
                    action.ExecuteCompleted += (o, eventArgs) => editing = false;
                }
            }
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (!editing && e.CloseReason == CloseReason.UserClosing) {
                if (Application != null)
                    e.Cancel = ((IModelOptionsMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose;

                if (e.Cancel)
                    ((XtraForm)sender).Hide();
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsMinimizeOnCloseOptions>();
        }
    }
}
