using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.Web.PropertyEditors.CSCodePropertyEditor;
using Xpand.ExpressApp.Web.SystemModule;
using Xpand.Persistent.Base.General;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator.Web {
    [ToolboxBitmap(typeof(WorldCreatorWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class WorldCreatorWebModule : WorldCreatorModuleBase {
        public WorldCreatorWebModule() {
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            RequiredModuleTypes.Add(typeof(Security.Web.XpandSecurityWebModule));
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            return base.GetDeclaredControllerTypes().Concat(new[] { typeof(RegisterScriptsController) });
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CSCodePropertyEditor,typeof(string),typeof(CSCodePropertyEditor),false)));
        }

        public override string GetPath() {
            if (HttpContext.Current != null){
                return Application.GetStorageFolder(WCAssembliesPath);
            }
            if (Application.IsHosted()){
                return Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            }
            return null;
        }
    }
}