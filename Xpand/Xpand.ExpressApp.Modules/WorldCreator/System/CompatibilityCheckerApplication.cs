using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Updating;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.System{
    class CompatibilityCheckerApplication:XafApplication,ITestXafApplication{
        private static readonly object _locker=new object();

        public CompatibilityCheckerApplication(IObjectSpaceProvider objectSpaceProvider, IEnumerable<ModuleBase> moduleList){
            objectSpaceProviders.Add(objectSpaceProvider);
            var moduleBases = moduleList.Select(m => m.GetType().CreateInstance()).Cast<ModuleBase>().OrderBy(m => m.Name).Distinct().ToArray();
            foreach (var moduleBase in moduleBases){
                if (Modules.FindModule(moduleBase.GetType())==null)
                    Modules.Add(moduleBase);
            }
            
            DatabaseVersionMismatch+=OnDatabaseVersionMismatch;
        }


        private void OnDatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e){
            e.Updater.Update();
            e.Handled = true;
        }

        public static void CheckCompatibility(XafApplication application, IObjectSpaceProvider objectSpaceProvider ) {
            lock (_locker){
                using (var compatibilityApplication = new CompatibilityCheckerApplication(objectSpaceProvider, application.Modules)){
                    compatibilityApplication.ApplicationName = application.ApplicationName;
                    try{
                        compatibilityApplication.CheckCompatibility();
                    }
                    catch (CompatibilityException e){
                        if (e.Message.Contains("FK_TemplateInfo_ObjectType")){
                            var message = "Please use " + typeof(WorldCreatorTypeInfoSource).Name + "." +
                                          nameof(WorldCreatorTypeInfoSource.UseDefaultObjectTypePersistance) +
                                          " before " + application.GetType().Name;
                            throw new CompatibilityException(new CompatibilityError(message, e));
                        }
                        throw;
                    }
                    using (objectSpaceProvider.CreateUpdatingObjectSpace(true)){
                    
                    }
                }
                
            }
        }

        public override DatabaseUpdaterBase CreateDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider){
            var databaseUpdaterBase = base.CreateDatabaseUpdater(objectSpaceProvider);
            var databaseSchemaUpdater = databaseUpdaterBase as DatabaseSchemaUpdater;
            if (databaseSchemaUpdater != null)
                return new WorldCreatorSchemaDatabaseUpdater(objectSpaceProvider,Modules);
            return new WorldCreatorDatabaseUpdater(objectSpaceProvider,ApplicationName, Modules);
        }

        private class WorldCreatorDatabaseUpdater : DatabaseUpdater{
            public WorldCreatorDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider, string applicationName,ModuleList moduleBases)
                : base(objectSpaceProvider, moduleBases, applicationName, objectSpaceProvider.ModuleInfoType){
            }

            protected override IList<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, IList<IModuleInfo> versionInfoList){
                List<ModuleUpdater> dbUpdaters = new List<ModuleUpdater>();
                var moduleUpdaterTypes = XafTypesInfo.Instance.FindTypeInfo(typeof(WorldCreatorModuleUpdater))
                    .Descendants.Where(info => !info.IsAbstract).ToArray();
                foreach (ModuleBase module in modules) {
                    Version moduleVersionFromDB = GetModuleVersion(versionInfoList, module.Name);
                    if (ForceUpdateDatabase || module.Version > moduleVersionFromDB || UseAllModuleUpdaters) {
                        dbUpdaters.AddRange(GetModuleUpdaters(module,moduleUpdaterTypes,objectSpace, moduleVersionFromDB));
                    }
                }
                return dbUpdaters;

                    
            }

            private IEnumerable<ModuleUpdater> GetModuleUpdaters(ModuleBase module, ITypeInfo[] moduleUpdaterTypes, IObjectSpace objectSpace, Version moduleVersionFromDB){
                var typeInfos = moduleUpdaterTypes.Where(info => info.Type.Assembly==module.GetType().Assembly);
                return typeInfos.Select(info => info.Type.CreateInstance(objectSpace, moduleVersionFromDB)).Cast<ModuleUpdater>();
            }
        }

        private class WorldCreatorSchemaDatabaseUpdater : DatabaseSchemaUpdater{
            public WorldCreatorSchemaDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider, ModuleList moduleList) : base(objectSpaceProvider,moduleList ){
                   
            }
        }

        protected override LayoutManager CreateLayoutManagerCore(bool simple){
            return null;
        }
    }
}