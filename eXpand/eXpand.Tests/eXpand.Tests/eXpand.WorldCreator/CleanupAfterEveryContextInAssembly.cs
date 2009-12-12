using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.WorldCreator.Core;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.WorldCreator {
    public class CleanupAfterEveryContextInAssembly : ICleanupAfterEveryContextInAssembly
    {
        public void AfterContextCleanup() {
            Isolate.CleanUp();
            ReflectionHelper.Reset();
            XafTypesInfo.Reset(true);
            
        }
    }

    public class AssemblyContext:IAssemblyContext {
        public void OnAssemblyStart() {
//            Isolate.Fake.WCTypesInfo();
            
        }

        public void OnAssemblyComplete() {
            
        }
    }
}