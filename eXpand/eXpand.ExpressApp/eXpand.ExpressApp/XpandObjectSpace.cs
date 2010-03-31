using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp
{
    public class XpandObjectSpace : ObjectSpace
    {
        public XpandObjectSpace(UnitOfWork unitOfWork, ITypesInfo typesInfo)
            : base(unitOfWork, typesInfo)
        {
        }

        protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer)
        {
            return new XpandUnitOfWork(dataLayer);
        }
    }
}
