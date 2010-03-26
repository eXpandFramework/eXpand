using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests {
    public class CleanupAfterEveryContextInAssembly : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup() {
            Isolate.CleanUp();
            ReflectionHelper.Reset();
            XafTypesInfo.Reset(true);
            
        }
    }

}