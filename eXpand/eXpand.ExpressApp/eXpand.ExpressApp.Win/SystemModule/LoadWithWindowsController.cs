using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelLoadWithWindowsOptions : IModelNode
    {
        bool LoadWithWindows { get; set; }
    }

    public partial class LoadWithWindowsController : WindowController, IModelExtender
    {
        public LoadWithWindowsController() { }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;   
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args){
            if (Frame.Context == TemplateContext.ApplicationWindow)
                ((XtraForm) Frame.Template).Closing += (o, eventArgs) => writeRegistry();
        }

        private void writeRegistry(){
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null){
                if (((IModelLoadWithWindowsOptions)Application.Model.Options).LoadWithWindows)
                {
                    key.SetValue(Application.Title, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
                }
                else if (key.GetValue(Application.Title) != null){
                    key.DeleteValue(Application.Title);
                }
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelLoadWithWindowsOptions>();
        }
    }
}
