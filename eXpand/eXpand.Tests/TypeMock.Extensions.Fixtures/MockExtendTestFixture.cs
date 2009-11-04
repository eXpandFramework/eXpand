using System.Collections.Generic;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace TypeMock.Extensions.Fixtures
{
    [TestFixture]
    public class MockExtendTestFixture
    {
        [Test]
        [VerifyMocks]
        [ClearMocks]
        public void Test_Track_Instanses()
        {
            Mock<DummyClass> mock = MockManager.MockAll<DummyClass>();

            TrackInstances<DummyClass> instances = mock.TrackInstances();

            new DummyClass();
            new DummyClass();

            Assert.AreEqual(2, instances.Count);
        }

        [Test]
        [VerifyMocks]
        [ClearMocks]
        public void Test_DoAlways()
        {
            Mock<DummyClass> mock = MockManager.MockAll<DummyClass>();
            bool pass = false;
            mock.DoAlways("DummyMethod",dummyClass => pass= true);

            new DummyClass().DummyMethod();

            Assert.AreEqual(true, pass);
        }
        [Isolated]
        [Test]
        public void PropertiesReadAfterSettingThem()
        {
            var myClass = Isolate.Fake.Instance<ClassWithProperties>();
            MockExtend.ActAsFields(myClass);

            myClass.Number = 1;
            myClass.Products = new List<Product> { new Product() };

            Assert.AreEqual(1, myClass.Number);
            Assert.AreEqual(1, myClass.Products.Count);
        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void ClearNonPublicIsolationsOnAllMethods()
        {
            var dummyClass = Isolate.Fake.Instance<DummyClass>();
            Isolate.WhenCalled(() => dummyClass.PublicMethod()).CallOriginal();
            MockManager.GetMockOf(dummyClass).ClearNonPublicIsolations();

            Assert.AreEqual(2, dummyClass.PublicMethod());

        }
        [Test]
        [Isolated]
        [VerifyMocks]
        [ClearMocks]
        public void ClearNonPublicIsolationsOnAllProperties()
        {
            var dummyClass = Isolate.Fake.Instance<DummyClass>();
            Isolate.WhenCalled(() => dummyClass.PublicProperty).CallOriginal();
            MockManager.GetMockOf(dummyClass).ClearNonPublicIsolations();

            Assert.AreEqual(2, dummyClass.PublicProperty);

        }
    }
}
