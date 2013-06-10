using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.ExpressApp.Web.Layout;
using Xpand.Persistent.Base.PersistentMetaData;


namespace Xpand.ExpressApp.Web {
    public class XpandWebApplication : WebApplication, IXafApplication {
        ApplicationModulesManager _applicationModulesManager;

        public XpandWebApplication() {
            DetailViewCreating += OnDetailViewCreating;
            ListViewCreating += OnListViewCreating;
        }

        string IXafApplication.ModelAssemblyFilePath {
            get { return GetModelAssemblyFilePath(); }
        }
        public virtual AutoCreateOption AutoCreateOption {
            get {
                return this.AutoCreateOption();
            }
        }

        protected virtual void OnUserDifferencesLoaded(EventArgs e) {
            EventHandler handler = UserDifferencesLoaded;
            if (handler != null) handler(this, e);
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args, null);
        }

        protected override LayoutManager CreateLayoutManagerCore(bool simple) {
            return new XpandLayoutManager();
        }

        protected override bool SupportMasterDetailMode {
            get { return true; }
        }

        protected override IHttpRequestManager CreateHttpRequestManager() {
            if (Model != null) {
                var modelOptionsFriendlyUrl = Model.Options as IModelOptionsFriendlyUrl;
                return modelOptionsFriendlyUrl != null && modelOptionsFriendlyUrl.EnableFriendlyUrl ? new XpandHttpRequestManager() : base.CreateHttpRequestManager();
            }
            return base.CreateHttpRequestManager();
        }

        protected override void LoadUserDifferences() {
            base.LoadUserDifferences();
            OnUserDifferencesLoaded(EventArgs.Empty);
        }

        ApplicationModulesManager IXafApplication.ApplicationModulesManager {
            get { return _applicationModulesManager; }
        }

        public event EventHandler UserDifferencesLoaded;

        protected override ApplicationModulesManager CreateApplicationModulesManager(ControllersManager controllersManager) {
            _applicationModulesManager = base.CreateApplicationModulesManager(controllersManager);
            return _applicationModulesManager;
        }
      
        public event EventHandler<ViewShownEventArgs> AfterViewShown;

        public virtual void OnAfterViewShown(Frame frame, Frame sourceFrame) {
            if (AfterViewShown != null) {
                AfterViewShown(this, new ViewShownEventArgs(frame, sourceFrame));
            }
        }

        public event CancelEventHandler ConfirmationRequired;

        protected void OnConfirmationRequired(CancelEventArgs e) {
            CancelEventHandler handler = ConfirmationRequired;
            if (handler != null) handler(this, e);
        }
        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args) {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor(this).Proccess(args);

        }
        public override ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
            var cancelEventArgs = new CancelEventArgs();
            OnConfirmationRequired(cancelEventArgs);
            return !cancelEventArgs.Cancel ? ConfirmationResult.No : base.AskConfirmation(confirmationType);
        }

        void OnListViewCreating(object sender, ListViewCreatingEventArgs args) {
            args.View = ViewFactory.CreateListView(this, args.ViewID, args.CollectionSource, args.IsRoot);
        }
        protected override Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
            Tracing.Tracer.LogVerboseValue("WinApplication.CreateWindowCore.activateControllersImmediatelly", activateControllersImmediatelly);
            return new XpandWebWindow(this, context, controllers, isMain, activateControllersImmediatelly);
        }
        protected override Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
            return new XpandPopupWindow(this, context, controllers);
        }
        void OnDetailViewCreating(object sender, DetailViewCreatingEventArgs args) {
            if (args.Obj != null) {
                args.View = ViewFactory.CreateDetailView(this, args.ViewID, args.Obj, args.IsRoot);
            }
        }

        protected XpandWebApplication(IContainer container) {
            container.Add(this);
        }

        public new SettingsStorage CreateLogonParameterStoreCore() {
            return base.CreateLogonParameterStoreCore();
        }

        public new void WriteLastLogonParameters(DetailView view, object logonObject) {
            base.WriteLastLogonParameters(view, logonObject);
        }

        IDataStore IXafApplicationDataStore.GetDataStore(IDataStore dataStore) {
            if ((ConfigurationManager.AppSettings["DataCache"] + "").Contains("Client")) {
                var cacheNode = HttpContext.Current.Application["DataStore"] as DataCacheNode;
                if (cacheNode == null) {
                    var _cacheRoot = new DataCacheRoot(dataStore);
                    cacheNode = new DataCacheNode(_cacheRoot);
                }
                return cacheNode;
            }
            return null;
        }

    }
}