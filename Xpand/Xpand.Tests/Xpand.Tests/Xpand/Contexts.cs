using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.Tests.Xpand.WorldCreator;

namespace Xpand.Tests.Xpand {
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