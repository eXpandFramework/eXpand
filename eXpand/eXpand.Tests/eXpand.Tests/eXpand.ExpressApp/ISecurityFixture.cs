//using System;
//using System.Configuration;
//using DevExpress.ExpressApp.Security;
//using DevExpress.Persistent.BaseImpl;
//using eXpand.Persistent.Base;
//using MbUnit.Framework;
//using TypeMock.ArrangeActAssert;
//
//namespace Fixtures.eXpand.ExpressApp
//{
//    [TestFixture]
//    public class ISecurityFixture
//    {
//        [Test]
//        public void Get_User_Type()
//        {
//            Assert.AreEqual("", typeof(User).AssemblyQualifiedName);
//            var security = Isolate.Fake.Instance<ISecurity>();
//            Isolate.Fake.StaticMethods(typeof(ConfigurationManager));
//            Isolate.WhenCalled(() => ConfigurationManager.AppSettings["UserType"]).WillReturn(typeof(MyCustomUser).AssemblyQualifiedName);
//
////            Type type=security.GetUserType();
//
////            Assert.AreEqual(typeof(MyCustomUser),type);
//        }
//    }
//
//    public class MyCustomUser
//    {
//    }
//}
