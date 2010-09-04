using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace Xpand.Tests {
    public class CleanupAfterEveryContextInAssembly : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup() {
            Isolate.CleanUp();
            ReflectionHelper.Reset();
            XafTypesInfo.Reset();
            XafTypesInfo.HardReset();
            XafTypesInfo.XpoTypeInfoSource.ResetDictionary();
        }
    }

}