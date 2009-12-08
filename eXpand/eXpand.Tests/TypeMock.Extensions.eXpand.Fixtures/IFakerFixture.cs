using DevExpress.ExpressApp;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace TypeMock.Extensions.eXpand.Fixtures{
    [TestFixture]
    public class IFakerFixture
    {
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void InstanceWithNonPublicNonIsolated()
        {
            var objectSpace = Isolate.Fake.Instance<ObjectSpace>();

            var controller = Isolate.Fake.InstanceWithNonPublicNonIsolated<ViewController>(objectSpace);

            Assert.AreEqual(objectSpace, controller.View.ObjectSpace);
        }
    }
}