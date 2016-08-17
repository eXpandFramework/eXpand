using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
    public sealed class WorldCreatorWebModule : XpandModuleBase {
        public WorldCreatorWebModule() {
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            return base.GetDeclaredControllerTypes().Concat(new[] { typeof(RegisterScriptsController) });
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), typeof(CSCodePropertyEditor), false)));
        }
    }
}