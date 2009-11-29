using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    public abstract class OnDemand_At_All_Views<T>:XpandBaseFixture where T:ViewController, new(){
        [Test(Order = 1)]
        [Isolated]
        public void All_Types_Of_Difference_Objects_Can_Be_Combined()
        {
            var controller = new T();
            Assert.AreEqual(typeof(ModelDifferenceObject), controller.TargetObjectType);
        }

    }
}