using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.Win.PropertyEditors;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using AssemblyHelper = DevExpress.ExpressApp.Utils.Reflection.AssemblyHelper;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator.Win {
    [ToolboxBitmap(typeof(WorldCreatorWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class WorldCreatorWinModule : XpandModuleBase {
        
        public WorldCreatorWinModule() {
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }

        protected override IEnumerable<Type> GetRegularTypes(){
            var richEditTypes = AssemblyHelper.GetTypesFromAssembly(typeof(XpandSystemWindowsFormsModule).Assembly)
                    .Where(type => type.Namespace != null && type.Namespace.Contains("RichEdit"));
            return base.GetRegularTypes().Concat(richEditTypes);
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredefinedMap.RichEditControl);
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), typeof(CSCodePropertyEditor), false)));
        }


    }
}