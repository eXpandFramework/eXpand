using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDifference.Win{
    
    [TestFixture]
    public class ModelEditorPropertyEditorFixture:eXpandBaseFixture
    {

        [Test]
        [Isolated]
        public void ModelEditorPropertyEditor_Control_Will_Be_A_ModelEditorControl(){
            var editor = new ModelEditorPropertyEditor(null, null);
//            Isolate.WhenCalled(() => editor.GetModel()).ReturnRecursiveFake();
            Isolate.WhenCalled(() => editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>())).ReturnRecursiveFake();
            editor.Setup(Isolate.Fake.Instance<ObjectSpace>(), Isolate.Fake.Instance<XafApplication>());
            Isolate.Swap.NextInstance<ModelEditorControl>().With(Isolate.Fake.Instance<ModelEditorControl>());

            editor.CreateControl();

            Assert.IsInstanceOfType(typeof(ModelEditorControl), editor.Control);
        }

        [Test]
        [Isolated]
        public void ModelEditorControl_Will_Save_Save_Settings_At_A_Node(){
            var editor = new ModelEditorPropertyEditor(null, null);
            editor.Setup(Isolate.Fake.Instance<ObjectSpace>(),Isolate.Fake.Instance<XafApplication>());

            throw new NotImplementedException();
//            ModelEditorControl control = editor.GetModelEditorControl(new Dictionary());

//            Assert.IsNotNull(control.SettingsStorage);
//            Assert.IsInstanceOfType(typeof(SettingsStorageOnDictionary),control.SettingsStorage);

        }

        [Test]
        [Isolated]
        public void When_Model_Dictionary_Is_Modified_Current_Object_Model_Should_Be_Set_To_Dictionary_Diffs()
        {
            var mock = MockManager.Mock(typeof(ModelEditorPropertyEditor),Constructor.NotMocked);
            mock.CallBase.ExpectCall("ReadValueCore");
            var editor = new ModelEditorPropertyEditor(null, null){CurrentObject = Isolate.Fake.Instance<ModelDifferenceObject>()};
            Isolate.WhenCalled(() => editor.Control).WillReturn(new ModelEditorControl());
            Isolate.NonPublic.WhenCalled(editor, "CanReadValue").WillReturn(true);
            var controller = Isolate.Fake.Instance<ModelEditorController>();
            Isolate.WhenCalled(() => controller.Dictionary.GetDiffs()).WillReturn(DefaultDictionary);
            Isolate.WhenCalled(() => editor.GetModelEditorController(null)).WillReturn(controller);
            using (RecorderManager.StartRecording()){
                controller.ModifiedChanged += null;
            }
            editor.ReadValue();
            var eventHandle = (EventHandler) RecorderManager.LastMockedEvent.GetEventHandle();

            eventHandle.Invoke(this,EventArgs.Empty);

            Assert.AreEqual(controller.Dictionary.GetDiffs().RootNode.ToXml(), editor.CurrentObject.Model.RootNode.ToXml());
        }
        [Test]
        [Isolated]
        public void CurrentObject_Model_Should_Be_Validated_Every_Time_CurrentObject_Is_Saving(){
            var editor = new ModelEditorPropertyEditor(null, null);
            bool validated = false;
            Isolate.WhenCalled(() => editor.Control.Controller.Dictionary.GetDiffs()).ReturnRecursiveFake();
            Isolate.WhenCalled(() => editor.Control.Controller.Dictionary.Validate()).DoInstead(context => validated=true);
            var objectSpace = new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance);
            var modelDifferenceObject = new ModelDifferenceObject(objectSpace.Session) { PersistentApplication = new PersistentApplication(objectSpace.Session) };
            editor.CurrentObject = modelDifferenceObject;
            editor.Setup(objectSpace, Isolate.Fake.Instance<XafApplication>());

            objectSpace.CommitChanges();

            Assert.IsTrue(validated);

        }

        [Test]
        [Isolated]
        public void CurrentObject_Model_Should_Be_Set_To_Control_Controller_Dictionary_Every_Time_CurrentObject_Is_Saving(){
            var editor = new ModelEditorPropertyEditor(null, null);
            var dictionary = new Dictionary(new DictionaryNode("test"));
            Isolate.WhenCalled(() => editor.Control.Controller.Dictionary).WillReturn(dictionary);
            var objectSpace = new ObjectSpace(new UnitOfWork(XpoDefault.DataLayer),XafTypesInfo.Instance);
            var modelDifferenceObject = new ModelDifferenceObject(objectSpace.Session) { PersistentApplication = new PersistentApplication(objectSpace.Session) };
            editor.CurrentObject=modelDifferenceObject;
            editor.Setup(objectSpace, Isolate.Fake.Instance<XafApplication>());
            
            objectSpace.CommitChanges();

            Assert.AreEqual(dictionary.RootNode.ToXml(), modelDifferenceObject.Model.RootNode.ToXml());
        }

        [Test]
        [Isolated]
        public void Model_Should_be_Created_By_Combining_Persistent_ApplicationModel_DefaultLanguage_And_PrefferedLanguage_With_CurrentObject_DefaultLang_And_PrefferedLanguage(){
            var persistentAppDictionary = DefaultDictionary.Clone();
            persistentAppDictionary.AddAspect("el",elDictionary.Clone().RootNode);
            const string s = "<Application><BOModel><Class Name=\"MyClass\" Caption=\"el2\"></Class></BOModel></Application>";
            var diffObjectDictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(s), Schema.GetCommonSchema());
            diffObjectDictionary.AddAspect("el", new DictionaryXmlReader().ReadFromString("<Application><BOModel><Class Name=\"MyClass\" Caption=\"el3\"></Class></BOModel></Application>"));
            var editor = new ModelEditorPropertyEditor(null, null)
                         {
                             CurrentObject =new ModelDifferenceObject(
                                 Session.DefaultSession)
                                            {
                                                Model = diffObjectDictionary,
                                                PersistentApplication = new PersistentApplication(Session.DefaultSession) { Model = persistentAppDictionary },
                                                PreferredAspect = "el"
                                            }
                         };

//            var dictionary = editor.GetModel();

//            dictionary.CurrentAspectProvider.CurrentAspect = "el";
//            Assert.AreEqual("el3", new ApplicationNodeWrapper(dictionary).BOModel.FindClassByName("MyClass").Caption);
            throw new NotImplementedException();
            
        }
        [Test]
        [Isolated]
        public void ModelEditorControlController_Should_Have_Editor_Model_And_CurrentObject_CurrentLanguage(){
            var editor = new ModelEditorPropertyEditor(null, null){
                                                                      CurrentObject =new ModelDifferenceObject(Session.DefaultSession)
                                                                          {PersistentApplication = new PersistentApplication(Session.DefaultSession),PreferredAspect = "el"}
                                                                  };
            Isolate.WhenCalled(() => editor.Control).WillReturn(new ModelEditorControl());
            var dictionary = new Dictionary();
            throw new NotImplementedException();
//            Isolate.WhenCalled(() => editor.GetModel()).WillReturn(dictionary);

//            var controller = editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>());
//
//            Assert.AreEqual("el", controller.CurrentAspect);
//            Assert.AreEqual(dictionary, controller.Dictionary);
        }


    }
}