using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelOptionsNotifyIconOptions : IModelNode
    {
        [Category("eXpand")]
        [Description("Add a tray icon with a popup menu")]
        bool NotifyIcon { get; set; }
    }

    public class NotifyIconController : WindowController, IModelExtender
    {
        static bool iconVisible;
        public NotifyIconController() {
            TargetWindowType=WindowType.Main;
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            if (!iconVisible)
                Frame.TemplateChanged += FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args)
        {
            if (Frame.Context == TemplateContext.ApplicationWindow && ((IModelOptionsNotifyIconOptions)Application.Model.Options).NotifyIcon)
            {
                var form = Frame.Template as XtraForm;
                if (form != null)
                {
                    IContainer container = new Container();

                    var strip = new ContextMenuStrip(container);
                    strip.Items.Add(GetMenuItem("Maximize", (o, eventArgs) => changeFormVisibility(form)));
                    strip.Items.Add(GetMenuItem("Minimize", (o, eventArgs) => changeFormVisibility(form)));
                    if (Application is ILogOut)
                        strip.Items.Add(GetMenuItem("LogOut", (o, eventArgs) => ((ILogOut)Application).Logout()));
                    strip.Items.Add(GetMenuItem("Exit", (o, eventArgs) => ModuleBase.Application.Exit()));

                    var notifyIcon1 = new NotifyIcon(container) { Visible = true, ContextMenuStrip = strip };
                    setIcon(notifyIcon1);
                    notifyIcon1.DoubleClick += (o, eventArgs) => changeFormVisibility(form);
                    iconVisible = true;
                }
            }
        }

        private ToolStripMenuItem GetMenuItem(string text, EventHandler clickHandler)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += clickHandler;
            return item;
        }


        private void changeFormVisibility(XtraForm form)
        {
            if (form.Visible)
                form.Hide();
            else
                form.Show();
        }

        private void setIcon(NotifyIcon notifyIcon1)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "ExpressApp.ico");
            if (File.Exists(path))
                notifyIcon1.Icon = new Icon(path);
            else
            {
                Stream resourceStream = typeof(eXpandSystemModule).Assembly.GetManifestResourceStream("eXpand.ExpressApp.Resources.ExpressApp.ico");
                if (resourceStream != null) notifyIcon1.Icon = new Icon(resourceStream);
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsNotifyIconOptions>();
        }
    }
}
