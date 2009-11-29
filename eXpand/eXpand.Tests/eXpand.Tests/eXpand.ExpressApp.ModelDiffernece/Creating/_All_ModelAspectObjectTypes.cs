using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Creating{
    [TestFixture]
    public class _All_ModelAspectObjectTypes:eXpandBaseFixture
    {
        
        public IEnumerable<ModelDifferenceObject> AllAspectObjectTypes
        {
            get{
                yield return new ModelDifferenceObject(Session.DefaultSession);
                yield return new UserModelDifferenceObject(Session.DefaultSession);
                yield return new RoleModelDifferenceObject(Session.DefaultSession);
            }
        }
        [Test][Factory("AllAspectObjectTypes")]
        [Isolated]
        public void Default_Language_Will_Be_The_Defaul_Aspect(ModelDifferenceObject modelDifferenceObject)
        {

            string aspect = modelDifferenceObject.Aspect;


            Assert.AreEqual(DictionaryAttribute.DefaultLanguage, aspect);
        }
    }
}