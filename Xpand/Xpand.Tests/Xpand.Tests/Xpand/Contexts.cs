using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Machine.Specifications;
using Xpand.Tests.Xpand.WorldCreator;

namespace Xpand.Tests.Xpand {
    public abstract class With_In_Memory_DataStore : With_WC_types {
        protected static XPObjectSpace XPObjectSpace;
        protected static UnitOfWork UnitOfWork;
        Establish context = () => {
            XPObjectSpace = (XPObjectSpace)ObjectSpaceInMemory.CreateNew();
            UnitOfWork = (UnitOfWork)XPObjectSpace.Session;
        };

    }
}