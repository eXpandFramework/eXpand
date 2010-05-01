using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelMinimizeOnCloseOptions : IModelNode
    {
        bool MinimizeOnClose { get; set; }
    }

    public class MinimizeOnCloseController : WindowController
    {
        private static bool editing;
        
        public MinimizeOnCloseController() { }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args)
        {
            if (Frame.Context == TemplateContext.ApplicationWindow &&
                ((IModelMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose)
            {
                var form = Frame.Template as XtraForm;
                if (form != null){
                    form.FormClosing += FormOnFormClosing;
                    SimpleAction action =
                    Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().Action;
                    action.Executing+=(o,eventArgs) =>  editing = true;
                    action.ExecuteCompleted += (o, eventArgs) => editing = false;
                }
            }
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e){
            if (!editing && e.CloseReason == CloseReason.UserClosing)
            {
                if (Application != null)
                    e.Cancel = ((IModelMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose;
                
                if (e.Cancel)
                    ((XtraForm)sender).Hide();
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelMinimizeOnCloseOptions>();
        }
    }
}
