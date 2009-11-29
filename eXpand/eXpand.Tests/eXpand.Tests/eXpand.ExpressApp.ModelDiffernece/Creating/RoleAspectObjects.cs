using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Creating
{
    [TestFixture]
    public class RoleAspectObjects:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void DifferenceType_Is_Model()
        {
            var o = new RoleModelDifferenceObject(Session.DefaultSession);

            Assert.AreEqual(DifferenceType.Role, o.DifferenceType);
        }
    }
}
