using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.WorldCreator.System.DatabaseUpdate;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.WorldCreator.System{
    public class WorldCreatorObjectSpaceProvider : XPObjectSpaceProvider{
        private readonly AssemblyRevisionUpdater _assemblyRevisionUpdater=new AssemblyRevisionUpdater();

        public WorldCreatorObjectSpaceProvider(bool threadSafe, IXpoDataStoreProvider xpoDataStoreProvider)
            : base(
                GetConnectionStringDataStoreProvider(xpoDataStoreProvider), XafTypesInfo.Instance, WorldCreatorTypeInfoSource.Instance,
                threadSafe){
        }

        public WorldCreatorObjectSpaceProvider(bool threadSafe):base(GetConnectionStringDataStoreProvider(), XafTypesInfo.Instance,WorldCreatorTypeInfoSource.Instance, threadSafe) {
        }

        public WorldCreatorObjectSpaceProvider()
            : this(ApplicationHelper.Instance.Application.IsHosted()) {
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
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["WorldCreatorConnectionString"];
            if (connectionStringSettings == null) {
                throw new ConfigurationErrorsException("WorldCreatorConnectionString not found");
            }
            return new ConnectionStringDataStoreProvider(connectionStringSettings.ConnectionString);
        }
    }
}