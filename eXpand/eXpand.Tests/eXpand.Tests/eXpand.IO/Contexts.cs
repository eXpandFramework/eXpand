using System;
using eXpand.ExpressApp.IO;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.IO {
    public abstract class With_Isolations
    {
        protected static Func<Type[]> IOArtifacts;

        Establish context = () =>
        {
            IOArtifacts = () => new[] { typeof(IOModule) };
            Isolate.Fake.IOTypesInfo();
        };
    }

    internal class With_Customer {
        
    }
}