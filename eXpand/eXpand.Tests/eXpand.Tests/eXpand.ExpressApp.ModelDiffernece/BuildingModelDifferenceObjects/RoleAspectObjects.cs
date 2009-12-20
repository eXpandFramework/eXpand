using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using eXpand.ExpressApp.Core;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.BuildingModelDifferenceObjects{
    [TestFixture]
    public class RoleAspectObjects:XpandBaseFixture
    {
    
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
                                                                                                                                                    return
                                                                                                                                                        null;
                                                                                                                                                });

            RoleDifferenceObjectBuilder.CreateDynamicMembers(Isolate.Fake.Instance<ISecurityComplex>());

            Assert.IsTrue(passed);

        }

    }
}