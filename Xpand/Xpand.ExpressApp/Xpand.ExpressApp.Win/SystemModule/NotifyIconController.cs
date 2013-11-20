using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsNotifyIconOptions : IModelNode {
        [Category("eXpand")]
        [Description("Add a tray icon with a popup menu")]
        bool NotifyIcon { get; set; }
    }

    public class NotifyIconController : WindowController, IModelExtender {
        Container _container;
        NotifyIcon _notifyIcon1;

        public NotifyIconController() {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Frame.TemplateChanged += FrameOnTemplateChanged;
                Frame.Disposing+=FrameOnDisposing;
            }
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            Frame.TemplateChanged-=FrameOnTemplateChanged;
            if (_notifyIcon1 != null) {
                _notifyIcon1.Icon = null;
                _notifyIcon1.Dispose();
                _container.Dispose();
            }
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            if (NotifyEnabled() && Frame.Context == TemplateContext.ApplicationWindow) {
                var form = Frame.Template as XtraForm;
                if (form != null) {
                    _container = new Container();
                    var strip = new ContextMenuStrip(_container);
                    strip.Items.Add(GetMenuItem(CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "Maximize"), (o, eventArgs) => ChangeFormVisibility(form)));
                    strip.Items.Add(GetMenuItem(CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "Minimize"), (o, eventArgs) => ChangeFormVisibility(form)));
                    var logoffAction = Frame.GetController<LogoffController>().LogoffAction;
                    if (logoffAction.Active)
                        strip.Items.Add(GetMenuItem(CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "LogOut"), (o, eventArgs) => Application.LogOff()));
                    strip.Items.Add(GetMenuItem(CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "Exit"), (o, eventArgs) => Application.Exit()));

                    _notifyIcon1 = new NotifyIcon(_container) { Visible = true, ContextMenuStrip = strip };
                    SetIcon(_notifyIcon1);
                    _notifyIcon1.DoubleClick += (o, eventArgs) => ChangeFormVisibility(form);
                }
            }
        }

        new WinApplication Application {
            get { return (WinApplication) ApplicationHelper.Instance.Application; }
        }

        bool NotifyEnabled() {
            return ((IModelOptionsNotifyIconOptions)Application.Model.Options).NotifyIcon;
        }

        private ToolStripMenuItem GetMenuItem(string text, EventHandler clickHandler) {
            var item = new ToolStripMenuItem(text);
            item.Click += clickHandler;
            return item;
        }


        private void ChangeFormVisibility(XtraForm form) {
            if (form.IsDisposed)
                return;
            if (form.Visible)
                form.Hide();
            else
                form.Show();
        }

        private void SetIcon(NotifyIcon notifyIcon1) {
            string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "", "ExpressApp.ico");
            if (File.Exists(path))
                notifyIcon1.Icon = new Icon(path);
            ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(Application.Model.Logo);
            Image image = imageInfo.Image;
            if (image != null) {
                var bitmap = new Bitmap(image);
                bitmap.SetResolution(16, 16);
                notifyIcon1.Icon = Icon.FromHandle(bitmap.GetHicon());
            } else {
                Stream resourceStream = typeof(XpandSystemModule).Assembly.GetManifestResourceStream("Xpand.ExpressApp.Resources.ExpressApp.ico");
                if (resourceStream != null) notifyIcon1.Icon = new Icon(resourceStream);
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsNotifyIconOptions>();
        }
    }
}
