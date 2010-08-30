using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;

namespace eXpand.Tests.eXpand {
    public abstract class With_In_Memory_DataStore : With_WC_types
    {
        protected static ObjectSpace ObjectSpace;
        protected static UnitOfWork UnitOfWork;
        Establish context = () =>
        {
            ObjectSpace = ObjectSpaceInMemory.CreateNew();
            UnitOfWork = (UnitOfWork)ObjectSpace.Session;
        };

    }
}