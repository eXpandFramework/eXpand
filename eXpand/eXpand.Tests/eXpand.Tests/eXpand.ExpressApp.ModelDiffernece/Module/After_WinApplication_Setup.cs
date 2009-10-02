using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using eXpand.ExpressApp.ModelDifference.Win;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.Module
{
    [TestFixture]
    public class After_WinApplication_Setup:eXpandBaseFixture
    {
        [Test]
        public void Combine_Application_Model_With_ModelDiffs()
        {
            Isolate.Fake.InstanceAndSwapAll<ObjectSpaceProvider>();
            var dictionaryDifferenceStoreFactory = Isolate.Fake.InstanceAndSwapAll<XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>>();
            Isolate.Fake.InstanceAndSwapAll<ConnectionStringDataStoreProvider>();
            Isolate.WhenCalled(() => dictionaryDifferenceStoreFactory.Create(null, null, false).LoadDifference(null)).WillReturn(elDictionary);
            var module = new ModelDifferenceWindowsFormsModule();
            var application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => application.Model).WillReturn(DefaultDictionary);
            using (RecorderManager.StartRecording()){
                application.SetupComplete += null;
            }
            module.Setup(application);
            var handler = (EventHandler<EventArgs>) RecorderManager.LastMockedEvent.GetEventHandle();

            handler.Invoke(this,EventArgs.Empty);

            Assert.AreEqual("el", new ApplicationNodeWrapper(application.Model).BOModel.FindClassByName("MyClass").Caption);
            
        }
    }
}
