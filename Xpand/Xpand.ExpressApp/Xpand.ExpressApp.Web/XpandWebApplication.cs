using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;


namespace Xpand.ExpressApp.Web {

    public class XpandWebApplication : WebApplication, IWebApplication {

        public XpandWebApplication() {
        }

        string IXafApplication.ModelAssemblyFilePath {
            get { return GetModelAssemblyFilePath(); }
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args, null);
        }

        protected override bool SupportMasterDetailMode {
            get { return true; }
        }

        protected override IHttpRequestManager CreateHttpRequestManager() {
            return this.NewHttpRequestManager();
        }

      
        public event CancelEventHandler ConfirmationRequired;

        protected void OnConfirmationRequired(CancelEventArgs e) {
            CancelEventHandler handler = ConfirmationRequired;
            if (handler != null) handler(this, e);
        }

        public override ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
            var cancelEventArgs = new CancelEventArgs();
            OnConfirmationRequired(cancelEventArgs);
            return !cancelEventArgs.Cancel ? ConfirmationResult.No : base.AskConfirmation(confirmationType);
        }

        protected override Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
            Tracing.Tracer.LogVerboseValue("WinApplication.CreateWindowCore.activateControllersImmediatelly", activateControllersImmediatelly);
            var windowCreatingEventArgs = new WindowCreatingEventArgs();
            OnWindowCreating(windowCreatingEventArgs);
            return windowCreatingEventArgs.Handled? windowCreatingEventArgs.Window
                       : new XpandWebWindow(this, context, controllers, isMain, activateControllersImmediatelly);
        }
        protected override Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
            return new XpandPopupWindow(this, context, controllers);
        }

        protected XpandWebApplication(IContainer container) {
            container.Add(this);
        }

        public new SettingsStorage CreateLogonParameterStoreCore() {
            return base.CreateLogonParameterStoreCore();
        }

        protected override void WriteSecuredLogonParameters() {
            var handledEventArgs = new HandledEventArgs();
            OnCustomWriteSecuredLogonParameters(handledEventArgs);
            if (!handledEventArgs.Handled)
                base.WriteSecuredLogonParameters();
        }

        public new void WriteLastLogonParameters(DetailView view, object logonObject) {
            base.WriteLastLogonParameters(view, logonObject);
        }

        public event HandledEventHandler CustomWriteSecuredLogonParameters;

        protected virtual void OnCustomWriteSecuredLogonParameters(HandledEventArgs e) {
            var handler = CustomWriteSecuredLogonParameters;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<WindowCreatingEventArgs> WindowCreating;

        protected virtual void OnWindowCreating(WindowCreatingEventArgs e) {
            var handler = WindowCreating;
            if (handler != null) handler(this, e);
        }
    }
}