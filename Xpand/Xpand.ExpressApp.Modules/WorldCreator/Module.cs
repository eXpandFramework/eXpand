using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using Xpand.ExpressApp.Validation;
using Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation;
using Xpand.Persistent.Base.General;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator {

    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class WorldCreatorModule : XpandModuleBase {
        public const string BaseImplNameSpace = "Xpand.Persistent.BaseImpl.PersistentMetaData";

        public WorldCreatorModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleClassInfoMerge), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleValidCodeIdentifier), typeof(IRuleBaseProperties));
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), false)));
        }
        
    }
}

