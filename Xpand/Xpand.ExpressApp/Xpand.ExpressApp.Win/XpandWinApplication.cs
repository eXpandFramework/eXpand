using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ViewStrategies;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win {

    public class XpandWinApplication : WinApplication, IWinApplication, ITestSupport {
        public XpandWinApplication() {
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            this.CreateCustomObjectSpaceprovider(args, null);
        }

        protected override void OnSetupComplete() {
            this.SetClientSideSecurity();
            base.OnSetupComplete();
        }

        protected override Form CreateModelEditorForm() {
            return ModelEditorViewController.CreateModelEditorForm(this);
        }

        public new SettingsStorage CreateLogonParameterStoreCore() {
            return base.CreateLogonParameterStoreCore();
        }

        public new void WriteLastLogonParameters(DetailView view, object logonObject) {
            base.WriteLastLogonParameters(view, logonObject);
        }

        public new void Start() {
            if (SecuritySystem.LogonParameters is IXpandLogonParameters) ReadLastLogonParameters(SecuritySystem.LogonParameters);
            base.Start();
        }

        public event EventHandler<CreatingListEditorEventArgs> CustomCreateListEditor;

        public event CancelEventHandler ConfirmationRequired;


        protected void OnConfirmationRequired(CancelEventArgs e) {
            CancelEventHandler handler = ConfirmationRequired;
            if (handler != null) handler(this, e);

        }

        string IXafApplication.ModelAssemblyFilePath {
            get { return GetModelAssemblyFilePath(); }
        }

        public void OnCustomCreateListEditor(CreatingListEditorEventArgs e) {
            EventHandler<CreatingListEditorEventArgs> handler = CustomCreateListEditor;
            if (handler != null) handler(this, e);
        }

        public override ConfirmationResult AskConfirmation(ConfirmationType confirmationType) {
            var cancelEventArgs = new CancelEventArgs();
            OnConfirmationRequired(cancelEventArgs);
            return cancelEventArgs.Cancel ? ConfirmationResult.No : base.AskConfirmation(confirmationType);
        }

        protected override ShowViewStrategyBase CreateShowViewStrategy() {
            var showViewStrategyBase = base.CreateShowViewStrategy();
            return showViewStrategyBase is ShowInMultipleWindowsStrategy
                       ? new XpandShowInMultipleWindowsStrategy(this)
                       : showViewStrategyBase;
        }

        protected override Window CreateWindowCore(TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly) {
            var windowCreatingEventArgs = new WindowCreatingEventArgs();
            OnWindowCreating(windowCreatingEventArgs);
            Tracing.Tracer.LogVerboseValue("WinApplication.CreateWindowCore.activateControllersImmediatelly", activateControllersImmediatelly);
            return windowCreatingEventArgs.Handled? windowCreatingEventArgs.Window
                       : new XpandWinWindow(this, context, controllers, isMain, activateControllersImmediatelly);
        }

        protected override Window CreatePopupWindowCore(TemplateContext context, ICollection<Controller> controllers) {
            return new XpandWinWindow(this, context, controllers, false, true);
        }

        public override IModelTemplate GetTemplateCustomizationModel(IFrameTemplate template) {
            var applicationBase = ((ModelApplicationBase)Model);
            if (applicationBase.Id == "Application") {
                var list = new List<ModelApplicationBase>();
                while (applicationBase.LastLayer.Id != "UserDiff" && applicationBase.LastLayer.Id != AfterSetupLayerId && applicationBase.LastLayer.Id != "Unchanged Master Part") {
                    list.Add(applicationBase.LastLayer);
                    ModelApplicationHelper.RemoveLayer(applicationBase);
                }
                var modelTemplate = base.GetTemplateCustomizationModel(template);
                foreach (var modelApplicationBase in list) {
                    ModelApplicationHelper.AddLayer((ModelApplicationBase)Model, modelApplicationBase);
                }
                return modelTemplate;
            }
            return base.GetTemplateCustomizationModel(template);
        }

        protected override ListEditor CreateListEditorCore(IModelListView modelListView, CollectionSourceBase collectionSource) {
            var creatingListEditorEventArgs = new CreatingListEditorEventArgs(modelListView, collectionSource);
            OnCustomCreateListEditor(creatingListEditorEventArgs);
            return creatingListEditorEventArgs.Handled ? creatingListEditorEventArgs.ListEditor : base.CreateListEditorCore(modelListView, collectionSource);
        }

        public XpandWinApplication(IContainer container) {
            container.Add(this);
        }

        bool ITestSupport.IsTesting { get; set; }

        public event EventHandler<WindowCreatingEventArgs> WindowCreating;

        protected virtual void OnWindowCreating(WindowCreatingEventArgs e) {
            var handler = WindowCreating;
            if (handler != null) handler(this, e);
        }
    }

}