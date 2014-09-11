using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsLoadWithWindowsOptions : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Modify windows registry in order your application to start along with windows")]
        bool LoadWithWindows { get; set; }
    }

    public class LoadWithWindowsController : WindowController, IModelExtender {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            if (Frame.Context == TemplateContext.ApplicationWindow)
                ((XtraForm)Frame.Template).Closing += (o, eventArgs) => writeRegistry();
        }

        private void writeRegistry() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null) {
                if (((IModelOptionsLoadWithWindowsOptions)Application.Model.Options).LoadWithWindows) {
                    key.SetValue(Application.Title, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
                } else if (key.GetValue(Application.Title) != null) {
                    key.DeleteValue(Application.Title);
                }
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsLoadWithWindowsOptions>();
        }
    }
}
