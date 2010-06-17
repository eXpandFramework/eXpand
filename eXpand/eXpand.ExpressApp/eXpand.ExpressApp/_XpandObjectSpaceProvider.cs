using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp
{
    public class XpandObjectSpaceProvider : ObjectSpaceProvider, IObjectSpaceProvider {
        
        public IXpoDataStoreProxy DataStoreProvider { get; set; }

        public XpandObjectSpaceProvider(IXpoDataStoreProxy provider)
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