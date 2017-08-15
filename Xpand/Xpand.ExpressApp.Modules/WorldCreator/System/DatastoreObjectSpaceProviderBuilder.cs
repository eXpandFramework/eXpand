using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.WorldCreator.System{
    class DatastoreObjectSpaceProviderBuilder{
        private readonly ModuleBase[] _dynamicModules;

        public DatastoreObjectSpaceProviderBuilder(ModuleBase[] dynamicModules){
            _dynamicModules = dynamicModules;
        }

        public IEnumerable<IObjectSpaceProvider> CreateProviders() {
            var moduleInfos =_dynamicModules.Select(m => new{Module = m, Attribute = m.GetType().Assembly.Attribute<DataStoreAttribute>()})
                .Where(info => info.Attribute != null);
                
            foreach (var moduleInfo in moduleInfos) {
                var connectionStringDataStoreProvider = new ConnectionStringDataStoreProvider(moduleInfo.Attribute.ConnectionString);
                var reflectionDictionary = new ReflectionDictionary();
                var assembly = moduleInfo.Module.GetType().Assembly;
                reflectionDictionary.CollectClassInfos(assembly);
                AssemblyXpoTypeInfoSource.CreateSource(assembly);
                yield return new XPObjectSpaceProvider(connectionStringDataStoreProvider,
                    moduleInfo.Module.Application.TypesInfo, AssemblyXpoTypeInfoSource.AssemblyInstance[assembly]);
                    
            }
        }

    }
}