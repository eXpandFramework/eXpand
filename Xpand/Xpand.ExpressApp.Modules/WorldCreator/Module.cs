using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using Fasterflect;
using Xpand.ExpressApp.Validation;
using Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.ExpressApp.WorldCreator.Services;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.ExpressApp.WorldCreator.System.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;
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
        private readonly object _locker = new object();
        public const string WCAssembliesPath = "WCAssembliesPath";
        
        public ModuleBase[] DynamicModules => ModuleManager.Modules.Where(
            module => module.GetType().Assembly.ManifestModule.ScopeName.EndsWith(Compiler.XpandExtension))
            .ToArray();

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DynamicModules));
        }


        private void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            AddDynamicModulesObjectSpaceProviders();
        }

        private void AddDynamicModulesObjectSpaceProviders() {
            var providerBuilder = new DatastoreObjectSpaceProviderBuilder(DynamicModules);
            providerBuilder.CreateProviders().Each(provider => Application.AddObjectSpaceProvider(provider));
        }

        void AddPersistentModules() {
            if (!string.IsNullOrEmpty(ConnectionString) || Application.ObjectSpaceProvider is DataServerObjectSpaceProvider) {
                lock (_locker) {
                    var worldCreatorObjectSpaceProvider = WorldCreatorObjectSpaceProvider.Create(Application, false);
                    using (var objectSpace = worldCreatorObjectSpaceProvider.CreateObjectSpace()) {
                        var codeValidator = new CodeValidator(new Compiler(AssemblyPathProvider.Instance.GetPath(Application)), new AssemblyValidator());
                        var assemblyManager = new AssemblyManager(objectSpace, codeValidator);
                        foreach (var assembly in assemblyManager.LoadAssemblies()) {
                            var moduleType = assembly.GetTypes().First(type => typeof(ModuleBase).IsAssignableFrom(type));
                            ModuleManager.AddModule(Application,(ModuleBase)moduleType.CreateInstance());
                        }
                        worldCreatorObjectSpaceProvider.ResetThreadSafe();
                    }
                }
            }
            else {
                var assemblies =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(Compiler.XpandExtension));
                foreach (var assembly1 in assemblies) {
                    ModuleManager.AddModule(assembly1.GetTypes().First(type => typeof(ModuleBase).IsAssignableFrom(type)));
                }
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            ExistentTypesMemberCreator.CreateMembers(this);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<WorldCreatorTypeInfoSourceAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                WorldCreatorTypeInfoSource.Instance.ForceRegisterEntity(typeInfo.Type);
            }
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleClassInfoMerge), typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleValidCodeIdentifier), typeof(IRuleBaseProperties));
            if (!InterfaceBuilder.SkipAssemblyCleanup && Application != null && (RuntimeMode || !string.IsNullOrEmpty(ConnectionString))) {
                CompatibilityCheckerApplication.CheckCompatibility(Application);
                AddPersistentModules();
                Application.LoggedOn += ApplicationOnLoggedOn;
                XpoObjectMerger.MergeTypes(this);
            }
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CSCodePropertyEditor, typeof(string), false)));
        }
        
    }
}

