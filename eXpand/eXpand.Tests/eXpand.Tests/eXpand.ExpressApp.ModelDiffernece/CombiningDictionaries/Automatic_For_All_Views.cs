using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.Web.Controllers;
using eXpand.ExpressApp.ModelDifference.Win.Controllers;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.CombiningDictionaries{
    [TestFixture]
    public class Automatic_For_All_Views:XpandBaseFixture
    {
        public class With_ActiveDifference:XpandBaseFixture{
            [Test]
            [Isolated]
            public void When_Saving_Combine_With_Application_InstanceModel()
            {
                var controller = Isolate.Fake.Instance<CombineActiveModelDictionaryController<ModelDifferenceObject>>(Members.CallOriginal);
                controller.Application = Isolate.Fake.Instance<XafApplication>();

                var dictionary = DefaultDictionary;
                var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession)
                                            {
                                                Model = dictionary,
                                                PersistentApplication =new PersistentApplication(Session.DefaultSession) { Name = "appNAme" }
                                            };
                Isolate.WhenCalled(() => controller.GetActiveDifference(null, null)).WithExactArguments().WillReturn(modelDifferenceObject);


                controller.ObjectSpaceOnObjectSaved(null, new ObjectManipulatingEventArgs(modelDifferenceObject));


                Assert.IsNotNull(new ApplicationNodeWrapper(controller.Application.Model).BOModel.FindClassByType(typeof(User)));

            }
            public class When_At_Web
            {
                [Test]
                [Isolated]
                public void ActiveDifference_Will_Be_OfType_ModelDifferenceObject()
                {
                    var controller = new ViewControllerFactory().CreateAndActivateController<CombineActiveModelDictionaryWithActiveModelDifferenceController>(typeof(ModelDifferenceObject));
                    var queryUserModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryModelDifferenceObject>();

                    var modelDifferenceObject = controller.GetActiveDifference(new PersistentApplication(Session.DefaultSession),null);

                    Assert.AreEqual(queryUserModelDifferenceObject.GetActiveModelDifference(""), modelDifferenceObject);
                }
                [Test]
                [Isolated]
                public void Combination_Is_Active_Only_For_ModelDifferenceObjects()
                {
                    var controller = new CombineActiveModelDictionaryWithActiveModelDifferenceController();

                    var type = controller.TargetObjectType;

                    Assert.AreEqual(typeof(ModelDifferenceObject), type);
                }

            }
            public class When_At_Win:XpandBaseFixture
            {
                [Test]
                [Isolated]
                public void Combination_Is_Active_Only_For_UserDifferenceObjects()
                {
                    var controller = new CombineActiveModelDictionaryWithActiveUserDifferenceController();

                    var type = controller.TargetObjectType;
                        
                    Assert.AreEqual(typeof(UserModelDifferenceObject), type);
                }
                [Test]
                [Isolated]
                public void ActiveDifference_Will_Be_OfType_UserDifferenceObject()
                {

                    var controller = new ViewControllerFactory().CreateAndActivateController<CombineActiveModelDictionaryWithActiveUserDifferenceController>(typeof(ModelDifferenceObject));
                    var queryUserModelDifferenceObject = Isolate.Fake.InstanceAndSwapAll<QueryUserModelDifferenceObject>();

                    
                    var modelDifferenceObject = controller.GetActiveDifference(new PersistentApplication(Session.DefaultSession),null);

                    Assert.AreEqual(queryUserModelDifferenceObject.GetActiveModelDifference(""), modelDifferenceObject);
                }

            }
        }
        
    }
}