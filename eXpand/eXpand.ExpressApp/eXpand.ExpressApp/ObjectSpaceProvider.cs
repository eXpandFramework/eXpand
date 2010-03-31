using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp
{
    public class ObjectSpaceProvider : DevExpress.ExpressApp.ObjectSpaceProvider, IObjectSpaceProvider {
        
        public IXpoDataStoreProxy DataStoreProvider { get; set; }

        public ObjectSpaceProvider(IXpoDataStoreProxy provider)
            : base(provider)
        {
            DataStoreProvider = provider;
        }

        protected override ObjectSpace CreateObjectSpaceCore(UnitOfWork unitOfWork, ITypesInfo typesInfo)
        {
            return new XpandObjectSpace(new XpandUnitOfWork(unitOfWork.DataLayer), typesInfo);
        }
    }
}