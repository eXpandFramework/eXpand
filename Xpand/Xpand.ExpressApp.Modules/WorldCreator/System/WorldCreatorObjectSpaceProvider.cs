using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using Fasterflect;
using Xpand.ExpressApp.WorldCreator.System.DatabaseUpdate;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.WorldCreator.System{
    public class WorldCreatorObjectSpaceProvider : XPObjectSpaceProvider{
        private const string WorldCreatorConnectionString = "WorldCreatorConnectionString";
        private readonly AssemblyRevisionUpdater _assemblyRevisionUpdater=new AssemblyRevisionUpdater();
        private bool _threadSafe;

        public WorldCreatorObjectSpaceProvider(bool threadSafe, IXpoDataStoreProvider xpoDataStoreProvider,ITypesInfo typesInfo)
            : base(GetConnectionStringDataStoreProvider(xpoDataStoreProvider), typesInfo, WorldCreatorTypeInfoSource.Instance,threadSafe){
        }

        public WorldCreatorObjectSpaceProvider(bool threadSafe):base(GetConnectionStringDataStoreProvider(), XafTypesInfo.Instance,WorldCreatorTypeInfoSource.Instance, threadSafe) {
        }

        public WorldCreatorObjectSpaceProvider()
            : this(IsHosted()) {
        }

        private static bool IsHosted(){
            return !InterfaceBuilder.IsDBUpdater && ApplicationHelper.Instance.Application.IsHosted();
        }

        protected override XPObjectSpace CreateUpdatingObjectSpaceCore(bool allowUpdateSchema){
            var objectSpace = base.CreateUpdatingObjectSpaceCore(allowUpdateSchema);
            _assemblyRevisionUpdater.Attach(objectSpace);
            return objectSpace;
        }

        protected override IObjectSpace CreateObjectSpaceCore(){
            var objectSpaceCore = base.CreateObjectSpaceCore();
            _assemblyRevisionUpdater.Attach(objectSpaceCore);
            return objectSpaceCore;
        }

        private static IXpoDataStoreProvider GetConnectionStringDataStoreProvider(IXpoDataStoreProvider xpoDataStoreProvider=null){
            if (InterfaceBuilder.IsDBUpdater)
                return new MemoryDataStoreProvider();
            if (xpoDataStoreProvider!=null){
                return xpoDataStoreProvider;
            }
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[WorldCreatorConnectionString];
            if (connectionStringSettings == null) {
                throw new ConfigurationErrorsException("WorldCreatorConnectionString not found");
            }
            return new ConnectionStringDataStoreProvider(connectionStringSettings.ConnectionString);
        }

        public static WorldCreatorObjectSpaceProvider Create(XafApplication application,bool threadSafe){
            var worldCreatorObjectSpaceProvider = application.ObjectSpaceProviders.OfType<WorldCreatorObjectSpaceProvider>().SingleOrDefault();
            if (worldCreatorObjectSpaceProvider != null){
                worldCreatorObjectSpaceProvider._threadSafe=threadSafe;
                worldCreatorObjectSpaceProvider.threadSafe=threadSafe;
                return worldCreatorObjectSpaceProvider;
            }
            ((IList<IObjectSpaceProvider>)application.GetFieldValue("objectSpaceProviders")).Add(new WorldCreatorObjectSpaceProvider());
            return Create(application,threadSafe);
        }

        public void ResetThreadSafe(){
            threadSafe=_threadSafe;    
        }
    }
}