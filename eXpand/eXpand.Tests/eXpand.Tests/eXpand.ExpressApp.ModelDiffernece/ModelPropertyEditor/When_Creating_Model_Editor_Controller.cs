using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    [TestFixture]
    public class When_Creating_Model_Editor_Controller:XpandBaseFixture
    {
        [Test]
        public void Dictionry_Should_Be_Current_Objet_Dictionary(){
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.CurrentObject).ReturnRecursiveFake();
            Isolate.WhenCalled(() => editor.CurrentObject.GetCombinedModel()).WillReturn(DefaultDictionary);

            ModelEditorController controller = editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>());

            Assert.AreEqual(DefaultDictionary, controller.Dictionary);
        }
        [Test]
        [Isolated]
        public void CurrentAspect_Should_Be_CurrentObjectLanguage()
        {
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.CurrentObject.GetCombinedModel()).WillReturn(DefaultDictionary);
            Isolate.WhenCalled(() => editor.CurrentObject.CurrentLanguage).WillReturn("el");

            ModelEditorController controller = editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>());

            Assert.AreEqual("el", controller.CurrentAspect);
        }
        [Test]
        [Isolated]
        public void When_Current_Node_Changed_It_Should_Modify_Object()
        {
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.CurrentObject).ReturnRecursiveFake();
            Isolate.WhenCalled(() => editor.CurrentObject.GetCombinedModel()).WillReturn(DefaultDictionary);
            bool modified = false;
            Isolate.WhenCalled(() => editor.ModifyCurrentObjectModel()).DoInstead(context => modified = true);
            var editorController = Isolate.Fake.InstanceAndSwapAll<ModelEditorController>();
            using (RecorderManager.StartRecording()){
                editorController.CurrentNodeChanged += null;
            }
            editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>());
            var handler = (EventHandler) RecorderManager.LastMockedEvent.GetEventHandle();

            handler.Invoke(this,EventArgs.Empty);

            Assert.IsTrue(modified);

        }
        [Test]
        [Isolated]
        public void When_Current_Node_Attribute_Changed_It_Should_Modify_Object()
        {
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.CurrentObject).ReturnRecursiveFake();
            Isolate.WhenCalled(() => editor.CurrentObject.GetCombinedModel()).WillReturn(DefaultDictionary);
            bool modified = false;
            Isolate.WhenCalled(() => editor.ModifyCurrentObjectModel()).DoInstead(context => modified = true);
            var editorController = Isolate.Fake.InstanceAndSwapAll<ModelEditorController>();
            using (RecorderManager.StartRecording()){
                editorController.CurrentAttributeValueChanged += null;
            }
            editor.GetModelEditorController(Isolate.Fake.Instance<XafApplication>());
            var handler = (EventHandler) RecorderManager.LastMockedEvent.GetEventHandle();

            handler.Invoke(this,EventArgs.Empty);

            Assert.IsTrue(modified);

        }
    }
}
