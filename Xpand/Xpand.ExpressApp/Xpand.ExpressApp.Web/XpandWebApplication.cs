using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.SystemModule;


namespace Xpand.ExpressApp.Web {
    public partial class XpandWebApplication : WebApplication, ISupportModelsManager, ISupportConfirmationRequired, ISupportAfterViewShown, ISupportLogonParameterStore, ISupportFullConnectionString, IXafApplication {
        protected XpandWebApplication() {
            InitializeComponent();
            DetailViewCreating += OnDetailViewCreating;
            ListViewCreating += OnListViewCreating;
        }
        protected override void OnLoggedOn(LogonEventArgs args) {
            base.OnLoggedOn(args);
            ((ShowViewStrategy)ShowViewStrategy).CollectionsEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
        }
        protected override ModuleTypeList GetDefaultModuleTypes() {
            var result = new List<Type>(base.GetDefaultModuleTypes()) { typeof(XpandSystemModule), typeof(XpandSystemAspNetModule) };
            return new ModuleTypeList(result.ToArray());
        }
        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            base.OnCreateCustomObjectSpaceProvider(args);
            if (args.ObjectSpaceProvider == null)
                this.CreateCustomObjectSpaceprovider(args);
        }

        string ISupportFullConnectionString.ConnectionString { get; set; }
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
            args.View = ViewFactory.CreateDetailView(this, args.ViewID, args.Obj, args.ObjectSpace, args.IsRoot);
        }
        public ApplicationModelsManager ModelsManager {
            get { return modelsManager; }
        }


        protected XpandWebApplication(IContainer container) {
            container.Add(this);
            InitializeComponent();
        }


        public new SettingsStorage CreateLogonParameterStoreCore() {
            return base.CreateLogonParameterStoreCore();
        }

        public new void WriteLastLogonParameters(DetailView view, object logonObject) {
            base.WriteLastLogonParameters(view, logonObject);
        }

        IDataStore IXafApplication.GetDataStore(IDataStore dataStore) {
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

        string IXafApplication.RaiseEstablishingConnection() {
            return this.GetConnectionString();
        }
    }
}