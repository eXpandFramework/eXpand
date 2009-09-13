using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions.eXpand;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.BuildingModelDifferenceObjects{
    [TestFixture]
    public class RoleAspectObjects:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Test()
        {
            string xml1 = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"Default\"></Class></BOModel></Application>";
            string xml2 = "<Application><BOModel><Class Name=\"MyClass2\" Caption=\"Default\"></Class></BOModel></Application>";
            var dictionary1 = new Dictionary(new DictionaryXmlReader().ReadFromString(xml1),Schema.GetCommonSchema());
            var dictionary2 = new Dictionary(new DictionaryXmlReader().ReadFromString(xml2),Schema.GetCommonSchema());

            dictionary1.CombineWith(dictionary2);

            Assert.IsNotNull(new ApplicationNodeWrapper(dictionary1).BOModel.FindClassByName("MyClass2"));
        }
        [Test]
        [Isolated]
        public void When_Security_Is_Complex_Create_A_Many_To_Many_Association_With_System_Role()
        {
            Isolate.Fake.ISecurityComplex();
            ITypesInfo instance = XafTypesInfo.Instance;
            instance.RegisterEntity(typeof(Role));
            bool passed = false;
            var dictionary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
            Isolate.WhenCalled(() => instance.CreateBothPartMembers(typeof(RoleModelDifferenceObject), typeof(Role), "", dictionary)).DoInstead(context =>
                                                                                                                                                {
                                                                                                                                                    passed = true;
                                                                                                                                                    Assert.IsInstanceOfType(typeof(bool), context.Parameters[4]);
                                                                                                                                                    Assert.IsTrue((bool)context.Parameters[4]);
                                                                                                                                                    return passed;
                                                                                                                                                });

            RoleDifferenceObjectBuilder.CreateDynamicMembers();

            Assert.IsTrue(passed);

        }

    }
}