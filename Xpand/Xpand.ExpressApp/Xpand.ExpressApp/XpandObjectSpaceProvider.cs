using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Xpand.Xpo;

namespace Xpand.ExpressApp
{
    public class XpandObjectSpaceProvider : DevExpress.ExpressApp.ObjectSpaceProvider, IXpandObjectSpaceProvider {
        
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