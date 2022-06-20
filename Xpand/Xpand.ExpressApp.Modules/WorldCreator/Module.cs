using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;

using Mono.Cecil;
using Xpand.ExpressApp.Validation;
using Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.ExpressApp.WorldCreator.Services;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.ExpressApp.WorldCreator.System.NodeUpdaters;
using Xpand.Extensions.Mono.Cecil;
using Xpand.Extensions.TypeExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Utils.Helpers;
using Xpand.XAF.Modules.ModelViewInheritance;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.WorldCreator{
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    
    public sealed class WorldCreatorModule : XpandModuleBase, IAdditionalModuleProvider{
        
        public event EventHandler<CustomWorldCreatorApplicationArgs> CustomWorldCreatorApplication;
        public const string BaseImplNameSpace = "Xpand.Persistent.BaseImpl.PersistentMetaData";

        public WorldCreatorModule(){
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
        }

        private readonly object _locker = new();
        public const string WCAssembliesPath = "WCAssembliesPath";

        public ModuleBase[] DynamicModules => ModuleManager.Modules.Where(
                module => module.GetType().Assembly.ManifestModule.ScopeName.EndsWith(Compiler.XpandExtension))
            .ToArray();

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ImageSourcesUpdater(DynamicModules));
        }


        private void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs){
            AddDynamicModulesObjectSpaceProviders();
        }

        private void AddDynamicModulesObjectSpaceProviders(){
            var providerBuilder = new DatastoreObjectSpaceProviderBuilder(DynamicModules);
            providerBuilder.CreateProviders().Each(provider => Application.AddObjectSpaceProvider(provider));
        }

        void AddPersistentModules(ApplicationModulesManager applicationModulesManager){
            WorldCreatorApplication.CheckCompatibility(Application, GetWorldCreatorApplication);

            if (!string.IsNullOrEmpty(ConnectionString)||Application.ObjectSpaceProviders.Any(provider => provider.GetType().InheritsFrom("DevExpress.ExpressApp.Security.ClientServer.DataServerObjectSpaceProvider"))){
                lock (_locker){
                    var worldCreatorObjectSpaceProvider = WorldCreatorObjectSpaceProvider.Create(Application, false);
                    using (var objectSpace = worldCreatorObjectSpaceProvider.CreateObjectSpace()){
                        var codeValidator =
                            new CodeValidator(new Compiler(AssemblyPathProvider.Instance.GetPath(Application)),
                                new AssemblyValidator());
                        var assemblyManager = new AssemblyManager(objectSpace, codeValidator);
                        foreach (var assembly in assemblyManager.LoadAssemblies()){
                            var moduleType = assembly.GetTypes()
                                .First(type => typeof(ModuleBase).IsAssignableFrom(type));
                            applicationModulesManager.AddModule(Application, (ModuleBase) moduleType.CreateInstance());
                        }

                        worldCreatorObjectSpaceProvider.MakeThreadSafe();
                    }
                    worldCreatorObjectSpaceProvider.SchemaUpdateMode=SchemaUpdateMode.None;
                }
            }
            else{
                var assemblies =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => assembly.ManifestModule.ScopeName.EndsWith(Compiler.XpandExtension));
                foreach (var assembly1 in assemblies){
                    applicationModulesManager.AddModule(assembly1.GetTypes()
                        .First(type => typeof(ModuleBase).IsAssignableFrom(type)));
                }
            }

            XpoObjectMerger.MergeTypes(this);
        }

        private WorldCreatorApplication GetWorldCreatorApplication(IObjectSpaceProvider provider, ModuleList list){
            var customWorldCreatorApplicationArgs =
                new CustomWorldCreatorApplicationArgs{
                    ObjectSpaceProvider = provider,
                    Modules = list
                };
            OnCustomWorldCreatorApplication(customWorldCreatorApplicationArgs);
            return customWorldCreatorApplicationArgs.WorldCreatorApplication ??
                   new WorldCreatorApplication(provider, list);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            if (RuntimeMode){
                var classInfos = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.Classes.OfType<XPClassInfo>()
                    .Where(info => info.IsPersistent &&
                                   WorldCreatorTypeInfoSource.Instance.RegisteredEntities.Contains(info.ClassType));
                foreach (var xpClassInfo in classInfos){
                    xpClassInfo.AddAttribute(new NonPersistentAttribute());
                }

                ExistentTypesMemberCreator.CreateMembers(this);
            }
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleClassInfoMerge),
                typeof(IRuleBaseProperties));
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(RuleValidCodeIdentifier),
                typeof(IRuleBaseProperties));
            if (Application != null && (RuntimeMode || !string.IsNullOrEmpty(ConnectionString))){
                AddPersistentModules(moduleManager);
                RegisterDerivedTypes();
                Application.LoggedOn += ApplicationOnLoggedOn;
                Application.SetupComplete += ApplicationOnSetupComplete;
            }
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs e){
            var reportsModule = Application.Modules.FirstOrDefault(module => module.Name == "ReportsModuleV2");
            if (reportsModule != null){
                var type =
                    reportsModule.GetType().Assembly
                        .GetType("DevExpress.ExpressApp.ReportsV2.ApplicationReportObjectSpaceProvider");
                type.GetProperties().First(info => info.Name == "ContextApplication").SetValue(null, Application, null);
            }
        }

        void IAdditionalModuleProvider.AddAdditionalModules(ApplicationModulesManager applicationModulesManager){
            AddToAdditionalExportedTypes(BaseImplNameSpace);
            AddPersistentModules(applicationModulesManager);
        }


        private void RegisterDerivedTypes(){
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToAssemblyDefinition().Where(assembly =>
                new[]{"System", "DevExpress"}.All(s => !assembly.Name.Name.StartsWith(s)));
            var types = assemblies.SelectMany(assembly => assembly.MainModule.Types);
            var additionExportedTypeDefinitions = AdditionalExportedTypes.GroupBy(type => AssemblyDefinition.ReadAssembly(type.Assembly.Location))
                .SelectMany(grouping => grouping.Key.MainModule.Types.Where(definition =>
                    AdditionalControllerTypes.Select(type => type).Any(type => type.FullName == definition.FullName)))
                .Where(definition => definition.Namespace!=null&&definition.Namespace.StartsWith(BaseImplNameSpace)).ToArray();

            types = types.Where(type =>
                type.Module.Assembly.FullName != BaseImplAssembly.FullName && additionExportedTypeDefinitions.Any(type.IsSubclassOf));
            foreach (var type in types){
                WorldCreatorTypeInfoSource.Instance.ForceRegisterEntity(AppDomain.CurrentDomain.FindType(type));
            }
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory){
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(
                new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CSCodePropertyEditor, typeof(string),
                    false)));
        }

        private void OnCustomWorldCreatorApplication(CustomWorldCreatorApplicationArgs e){
            CustomWorldCreatorApplication?.Invoke(this, e);
        }
    }

    public class CustomWorldCreatorApplicationArgs : EventArgs{
        public WorldCreatorApplication WorldCreatorApplication{ internal get; set; }
        public IObjectSpaceProvider ObjectSpaceProvider{ get; internal set; }
        public ModuleList Modules{ get; internal set; }
    }
}