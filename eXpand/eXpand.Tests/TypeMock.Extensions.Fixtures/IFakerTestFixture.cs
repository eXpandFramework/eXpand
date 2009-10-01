using System;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace TypeMock.Extensions.Fixtures
{
    /// <summary>
    /// FakeInstanceWithNonPublicNonIsolated
    /// Fake_All_Calls_To_base_members
    /// </summary>
    [TestFixture]
    public class IFakerTestFixture
    {
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void FakeInstanceWithNonPublicNonIsolated()
        {
            var dummyClass = Isolate.Fake.InstanceWithNonPublicNonIsolated<DummyClass>();
            Isolate.WhenCalled(() => dummyClass.PublicMethod()).CallOriginal();
            

            Assert.AreEqual(2, dummyClass.PublicMethod());

        }
        [Test]
        [Isolated]
        public void Fake_All_Calls_To_base_members(){
            var dummyClass = Isolate.Fake.BaseCallsIsolated<DummyClass>();

            int call = dummyClass.PublicMethodWithBaseCall();

            Assert.AreEqual(1, call);
        }
    }
}
