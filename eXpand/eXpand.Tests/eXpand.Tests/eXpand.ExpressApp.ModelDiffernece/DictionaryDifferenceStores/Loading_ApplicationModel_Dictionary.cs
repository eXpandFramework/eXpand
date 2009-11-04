using System.Collections.Generic;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using eXpand.ExpressApp.ModelDifference.Win;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DictionaryDifferenceStores{
    [TestFixture]
    public class Loading_ApplicationModel_Dictionary:XpandBaseFixture
    {
        public class From_Path{
            [Test]
            [Isolated]
            public void Load_From_Directory_Is_Enable_If_Debugger_Is_Attached()
            {


                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                Isolate.WhenCalled(() => store.IsDebuggerAttached).WillReturn(true);

                var path = store.UseModelFromPath();

                Assert.IsTrue(path);
            }


            [Row("true")]
            [Test]
            [Isolated]
            public void The_Load_From_Directory_Can_BeDisabled_Even_If_Debugger_Is_Attached(string disableDebuggerAttachedCheck)
            {
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                Isolate.WhenCalled(() => store.IsDebuggerAttached).WillReturn(true);
                Isolate.Fake.StaticMethods(typeof(ConfigurationManager));
                Isolate.WhenCalled(() => ConfigurationManager.AppSettings[XpoModelDictionaryDifferenceStore.EnableDebuggerAttachedCheck]).WillReturn(disableDebuggerAttachedCheck);

                var path = store.UseModelFromPath();

                Assert.IsFalse(path);
            }

            [Test]
            [Isolated]
            [MultipleAsserts]
            public void Load_From_Directory()
            {

                Isolate.Fake.StaticMethods(typeof(Validator));
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                #region isolate store
                Isolate.WhenCalled(() => store.GetModelPaths()).WillReturn(new List<string> { "model.xafml", "model_el.xafml", "LogonParameters.xafml" });
                Isolate.WhenCalled(() => store.UseModelFromPath()).WillReturn(true);
                Isolate.WhenCalled(() => store.SaveDifference(null)).IgnoreCall();
                #endregion
                var dictionaryNode = new DictionaryNode("Application");
                #region isolate dictionaryXmlReader
                var dictionaryXmlReader = Isolate.Fake.Instance<DictionaryXmlReader>();
                Isolate.Swap.AllInstances<DictionaryXmlReader>().With(dictionaryXmlReader);
                Isolate.WhenCalled(() => dictionaryXmlReader.ReadFromFile(null)).WillReturn(dictionaryNode);
                #endregion

                Dictionary dictionary = store.LoadDifference(Schema.GetCommonSchema());


                Assert.AreEqual(dictionaryNode.ToXml(), dictionary.RootNode.ToXml());
            }
            [Test]
            [Isolated]
            public void If_Can_Load_From_Path_ApplicationModel_Will_Saved()
            {
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                Isolate.WhenCalled(() => store.UseModelFromPath()).WillReturn(true);
                Isolate.WhenCalled(() => store.GetPath()).WillReturn(@"c:\1");
                bool called = false;
                Isolate.WhenCalled(() => store.SaveDifference(null)).DoInstead(context => called = true);

                store.LoadDifference(Schema.GetCommonSchema());

                Assert.IsTrue(called);

            }    
        }

        public class From_DataStore : Loading_ApplicationModel_Dictionary
        {
            [Test]
            [Isolated]
            public void Will_Return_An_Empty_Dictionary_If_Loading_is_Disabled()
            {
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);

                Dictionary dictionary = store.LoadDifference(Schema.GetCommonSchema());

                Assert.AreEqual("<Application />\r\n", dictionary.RootNode.ToXml());
            }
            [Test]
            [Isolated]
            public void If_ActiveDifference_Not_Found_It_Will_Save_A_New_Default_Dictionary(){
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                Isolate.WhenCalled(() => store.UseModelFromPath()).WillReturn(false);
                Isolate.WhenCalled(() => store.GetActiveDifferenceObject()).WillReturn(null);

                var dictionary = store.LoadDifference(Schema.GetCommonSchema());

                Assert.AreEqual(new DictionaryNode("Application").ToXml(), dictionary.RootNode.ToXml());
            }
            [Test]
            [Isolated]
            public void If_Active_Difference_Foind_It_Will_Return_Its_Model(){
                var store = new XpoModelDictionaryDifferenceStoreFactory<XpoWinModelDictionaryDifferenceStore>().Create(Session.DefaultSession,
                                                                         Isolate.Fake.Instance<XafApplication>(), true);
                Isolate.WhenCalled(() => store.UseModelFromPath()).WillReturn(false);
                Isolate.WhenCalled(() => store.GetActiveDifferenceObject()).WillReturn(new ModelDifferenceObject(Session.DefaultSession) { Model = DefaultDictionary, PersistentApplication = new PersistentApplication(Session.DefaultSession) });
                Isolate.WhenCalled(() => store.SaveDifference(null)).IgnoreCall();

                var dictionary = store.LoadDifference(Schema.GetCommonSchema());

                Assert.AreEqual(DefaultDictionary.RootNode.ToXml(), dictionary.RootNode.ToXml());
            }
        }
    }
}