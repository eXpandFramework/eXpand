using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Updating;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.WorldCreator.System {
    public class WorldCreatorApplication : XafApplication, ITestXafApplication {
        private static readonly object Locker = new();

        public WorldCreatorApplication(IObjectSpaceProvider objectSpaceProvider, IEnumerable<ModuleBase> moduleList) {
            this.AddObjectSpaceProvider(objectSpaceProvider);
            var moduleBases = moduleList.Select(m => m.GetType().CreateInstance()).Cast<ModuleBase>().OrderBy(m => m.Name).Distinct().ToArray();
            foreach (var moduleBase in moduleBases) {
                if (Modules.FindModule(moduleBase.GetType()) == null)
                    Modules.Add(moduleBase);
            }
            ObjectSpaceCreated+=Application_ObjectSpaceCreated;
        }

        private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
            if (e.ObjectSpace is CompositeObjectSpace compositeObjectSpace) {
                if (!(compositeObjectSpace.Owner is CompositeObjectSpace)) {
                    compositeObjectSpace.PopulateAdditionalObjectSpaces((XafApplication)sender);
                }
            }
        }
        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs e){
            e.Updater.Update();
            e.Handled = true;
        }

        internal static void CheckCompatibility(XafApplication application,Func<IObjectSpaceProvider, ModuleList, WorldCreatorApplication> func) {
            lock (Locker) {
                if (application.Security.IsRemoteClient())
                    return;
                var objectSpaceProvider = WorldCreatorObjectSpaceProvider.Create(application, false);
                using (var worldCreatorApplication = func(objectSpaceProvider, application.Modules)) {
                    worldCreatorApplication.ApplicationName = application.ApplicationName;
                    try {
                        worldCreatorApplication.CheckCompatibility();
                    }
                    catch (CompatibilityException e) {
                        if (e.Message.Contains("FK_TemplateInfo_ObjectType")) {
                            var message = "Please use " + nameof(WorldCreatorTypeInfoSource) + "." +
                                          nameof(WorldCreatorTypeInfoSource.UseDefaultObjectTypePersistance) +
                                          " before " + application.GetType().Name;
                            throw new CompatibilityException(new CompatibilityError(message, e));
                        }
                        throw;
                    }
                    using (objectSpaceProvider.CreateUpdatingObjectSpace(Debugger.IsAttached || InterfaceBuilder.IsDBUpdater)) {

                    }
                }
                objectSpaceProvider.MakeThreadSafe();
            }
        }

        public override DatabaseUpdaterBase CreateDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider) {
            var databaseUpdaterBase = base.CreateDatabaseUpdater(objectSpaceProvider);
            if (databaseUpdaterBase is DatabaseSchemaUpdater)
                return new WorldCreatorSchemaDatabaseUpdater(objectSpaceProvider, Modules);
            return new WorldCreatorDatabaseUpdater(objectSpaceProvider, ApplicationName, Modules);
        }

        private class WorldCreatorDatabaseUpdater : DatabaseUpdater {
            public WorldCreatorDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider, string applicationName, ModuleList moduleBases)
                : base(objectSpaceProvider, moduleBases, applicationName, objectSpaceProvider.ModuleInfoType) {
            }

            protected override IList<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, IList<IModuleInfo> versionInfoList) {
                List<ModuleUpdater> dbUpdaters = new List<ModuleUpdater>();
                var moduleUpdaterTypes = XafTypesInfo.Instance.FindTypeInfo(typeof(WorldCreatorModuleUpdater))
                    .Descendants.Where(info => !info.IsAbstract).ToArray();
                foreach (ModuleBase module in modules) {
                    Version moduleVersionFromDB = GetModuleVersion(versionInfoList, module.Name);
                    if (ForceUpdateDatabase || module.Version > moduleVersionFromDB || UseAllModuleUpdaters) {
                        dbUpdaters.AddRange(GetModuleUpdaters(module, moduleUpdaterTypes, objectSpace, moduleVersionFromDB));
                    }
                }
                return dbUpdaters;


            }

            private IEnumerable<ModuleUpdater> GetModuleUpdaters(ModuleBase module, ITypeInfo[] moduleUpdaterTypes, IObjectSpace objectSpace, Version moduleVersionFromDB) {
                var typeInfos = moduleUpdaterTypes.Where(info => info.Type.Assembly == module.GetType().Assembly);
                return typeInfos.Select(info => info.Type.CreateInstance(objectSpace, moduleVersionFromDB)).Cast<ModuleUpdater>();
            }
        }

        private class WorldCreatorSchemaDatabaseUpdater : DatabaseSchemaUpdater {
            public WorldCreatorSchemaDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider, ModuleList moduleList) : base(objectSpaceProvider, moduleList) {

            }
        }

        protected override LayoutManager CreateLayoutManagerCore(bool simple) {
            return null;
        }
    }
}