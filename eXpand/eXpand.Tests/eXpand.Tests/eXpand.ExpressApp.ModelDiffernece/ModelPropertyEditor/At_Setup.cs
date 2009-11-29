using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    public class At_Setup:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Application_Instance_Should_Be_Known(){
            var editor = new ModelEditorPropertyEditor(null, null);
            var application = Isolate.Fake.Instance<XafApplication>();

            editor.Setup(Isolate.Fake.Instance<ObjectSpace>(),application);

            Assert.AreEqual(editor.Application, application);
        }
        [Test]
        [Isolated]
        public void Before_Application_Saving_Current_Object_Should_Validate_ModelEditorControl_Dictionary()
        {
            
            bool validated = false;
            var editor = new ModelEditorPropertyEditor(null, null);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);
            Isolate.WhenCalled(() => editor.CurrentObject).WillReturn(modelDifferenceObject);
            Isolate.WhenCalled(() => editor.Control.Controller.Dictionary.Validate()).DoInstead(context => validated=true);
            var objectSpace = Isolate.Fake.Instance<ObjectSpace>();
            using (RecorderManager.StartRecording()){
                objectSpace.ObjectSaving += null;
            }
            editor.Setup(objectSpace, null);
            var handler = (EventHandler<ObjectManipulatingEventArgs>) RecorderManager.LastMockedEvent.GetEventHandle();

            handler.Invoke(this,new ObjectManipulatingEventArgs(modelDifferenceObject));

            Assert.IsTrue(validated);
        }
    }
}
