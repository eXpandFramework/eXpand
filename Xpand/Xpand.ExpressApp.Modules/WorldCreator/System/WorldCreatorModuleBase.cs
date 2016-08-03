using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security.ClientServer;
using Fasterflect;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.ExpressApp.WorldCreator.Services;
using Xpand.ExpressApp.WorldCreator.System.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.System {
    public abstract class WorldCreatorModuleBase : XpandModuleBase{
        private readonly object _locker=new object();
        public const string WCAssembliesPath = "WCAssembliesPath";
        public abstract string GetPath();
        public ModuleBase[] DynamicModules => ModuleManager.Modules.Where(
            module => module.GetType().Assembly.ManifestModule.ScopeName.EndsWith(Compiler.XpandExtension))
            .ToArray();

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ShowOwnerForExtendedMembersUpdater());
            updaters.Add(new ImageSourcesUpdater(DynamicModules));
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (!InterfaceBuilder.SkipAssemblyCleanup&&Application != null && (RuntimeMode || !string.IsNullOrEmpty(ConnectionString))){
                CompatibilityCheckerApplication.CheckCompatibility(Application);
                AddPersistentModules();
                Application.LoggedOn+=ApplicationOnLoggedOn;
                XpoObjectMerger.MergeTypes(this);
            }
        }

        private void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs){
            AddDynamicModulesObjectSpaceProviders();
        }

        private void AddDynamicModulesObjectSpaceProviders(){
            var providerBuilder = new DatastoreObjectSpaceProviderBuilder(DynamicModules);
            providerBuilder.CreateProviders().Each(provider => Application.AddObjectSpaceProvider(provider));
        }

        void AddPersistentModules() {
            if (!string.IsNullOrEmpty(ConnectionString) || Application.ObjectSpaceProvider is DataServerObjectSpaceProvider) {
                lock (_locker){
                    var worldCreatorObjectSpaceProvider = WorldCreatorObjectSpaceProvider.Create(Application,false);
                    using (var objectSpace = worldCreatorObjectSpaceProvider.CreateObjectSpace()) {
                        var codeValidator = new CodeValidator(new Compiler(GetPath()), new AssemblyValidator());
                        var assemblyManager = new AssemblyManager(objectSpace, codeValidator);
                        foreach (var assembly in assemblyManager.LoadAssemblies()) {
                            var moduleType = assembly.GetTypes().First(type => typeof(ModuleBase).IsAssignableFrom(type));
                            ModuleManager.AddModule((ModuleBase)moduleType.CreateInstance());
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
            AddToAdditionalExportedTypes(WorldCreatorModule.BaseImplNameSpace);
            ExistentTypesMemberCreator.CreateMembers(this);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<WorldCreatorTypeInfoSourceAttribute>()!=null);
            foreach (var typeInfo in typeInfos){
                WorldCreatorTypeInfoSource.Instance.ForceRegisterEntity(typeInfo.Type);
            }
        }

    }
}