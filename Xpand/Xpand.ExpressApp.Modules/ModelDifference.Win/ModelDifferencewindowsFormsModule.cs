using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;

namespace Xpand.ExpressApp.ModelDifference.Win {
    [ToolboxBitmap(typeof(ModelDifferenceWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule {
        public ModelDifferenceWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
        }
        public static ModelApplicationCreator ApplicationCreator { get; set; }
        private bool? _modelsLoaded = false;
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) {
                var winApplication = Application as WinApplication;
                if (winApplication != null) winApplication.HandleException();
                Application.LoggedOff += Application_LoggedOff;
                Application.Disposed += Application_Disposed;
            }
        }

        void Application_Disposed(object sender, EventArgs e) {
            ((XafApplication)sender).Disposed -= Application_Disposed;
            ((XafApplication)sender).LoggedOff -= Application_LoggedOff;
        }

        void Application_LoggedOff(object sender, EventArgs e) {
            var modelApplicationBase = ((ModelApplicationBase)((XafApplication)sender).Model);
            var lastLayer = modelApplicationBase.LastLayer;
            while (lastLayer.Id != "Unchanged Master Part") {
                ModelApplicationHelper.RemoveLayer(modelApplicationBase);
                lastLayer = modelApplicationBase.LastLayer;
            }
            var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetupLayer.Id = "After Setup";
            ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetupLayer);
        }

        public override string GetPath() {
            return Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }

        protected override bool? ModelsLoaded {
            get { return _modelsLoaded; }
            set { _modelsLoaded = value; }
        }
    }
}