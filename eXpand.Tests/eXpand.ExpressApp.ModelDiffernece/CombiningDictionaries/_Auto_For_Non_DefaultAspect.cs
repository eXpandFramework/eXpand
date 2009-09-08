using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.Controllers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Combining
{
    [TestFixture]
    public class _Auto_For_Non_DefaultAspect
    {
//        [Test]
//        [Isolated]
//        public void Test(){
//            var dictionary = DictionaryFactory.Create(typeof (User), CultureInfo.GetCultureInfo("el"));
//            var s = new DictionaryXmlWriter().GetAspectXml("el", dictionary.RootNode);
//            var xml = dictionary.RootNode.ToXml();
//            Assert.IsNull(s);
//        }

        [Test]
        [Isolated]
        public void When_Saving_Localized_Aspect_Then_Non_Localized_Nodes_Should_Be_Saved_At_DefaultAspect()
        {
            throw new NotImplementedException();
//            var objectSpace = Isolate.Fake.InstanceAndSwapAll<ObjectSpace>();
//            using (RecorderManager.StartRecording())
//            {
//                objectSpace.Committing += null;
//            }
//
//            var localizedModelAspectObject = new ModelAspectObject(Session.DefaultSession);
//            var controller = new ViewControllerFactory().CreateController<CombineLocalizedWithDefaultAspectController>(ViewType.Any, localizedModelAspectObject);
//            var localizedDictionary = DictionaryFactory.Create(typeof(User),CultureInfo.GetCultureInfo("el"));
//            new ApplicationNodeWrapper(localizedDictionary).BOModel.FindClassByType(typeof(User)).DefaultListView = "User_LookupListView";
//            localizedModelAspectObject.AspectModel = localizedDictionary.RootNode;
//            localizedModelAspectObject.Aspect = "el";
//            var handler = ((EventHandler<CancelEventArgs>)RecorderManager.LastMockedEvent.GetEventHandle());
//            var activeAspectObject = new ModelAspectObject(Session.DefaultSession);
//            var activeDictionary = DictionaryFactory.Create(typeof(User));
//            activeAspectObject.AspectModel = activeDictionary.RootNode;
//            Isolate.WhenCalled(() => localizedModelAspectObject.QueryModelAspectObject.GetActiveAspect("", "")).WillReturn(activeAspectObject);
//
//            handler.Invoke(this, new CancelEventArgs());
//
//            Assert.AreEqual(typeof(ModelAspectObject), controller.TargetObjectType);
//            var defaultListView = new ApplicationNodeWrapper(activeAspectObject.AspectModel).BOModel.FindClassByType(typeof(User)).DefaultListView;
//            Assert.AreEqual("User_Lookup_ListView", defaultListView);
        }
    }
}
