using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using AssemblyHelper = DevExpress.ExpressApp.Utils.Reflection.AssemblyHelper;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.ModelDifference.Win {
    [ToolboxBitmap(typeof(ModelDifferenceWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class ModelDifferenceWindowsFormsModule : ModelDifferenceBaseModule {
        public ModelDifferenceWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Security.Win.XpandSecurityWinModule));
        }
        public static ModelApplicationCreator ApplicationCreator { get; set; }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) {
                var winApplication = Application as WinApplication;
                winApplication?.HandleException();
                Application.LoggedOff += Application_LoggedOff;
                Application.Disposed += Application_Disposed;
            }
            moduleManager.Extend(PredefinedMap.RichEditControl);
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory) {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.RichEditRftPropertyEditor, typeof(string), false)));
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.RichEditRftPropertyEditor, typeof(string), typeof(RichEditWinPropertyEditor), false)));
        }

        void Application_Disposed(object sender, EventArgs e) {
            ((XafApplication)sender).Disposed -= Application_Disposed;
            ((XafApplication)sender).LoggedOff -= Application_LoggedOff;
        }

        protected override IEnumerable<Type> GetRegularTypes() {
            var richEditTypes = AssemblyHelper.GetTypesFromAssembly(typeof(XpandSystemWindowsFormsModule).Assembly)
                    .Where(type => type.Namespace != null && type.Namespace.Contains("RichEdit"));
            return base.GetRegularTypes().Concat(richEditTypes);
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
        
    }
}