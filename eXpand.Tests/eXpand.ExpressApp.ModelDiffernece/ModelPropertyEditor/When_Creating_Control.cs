using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.XtraEditors;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    [TestFixture]
    public class When_Creating_Control:eXpandBaseFixture
    {
        [Test]
        [Isolated]
        public void ModelEditorControl_Settings_Will_Be_Save()
        {
            Isolate.Fake.InstanceAndSwapAll<XtraScrollableControl>();
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.CurrentObject).ReturnRecursiveFake();
            var application = Isolate.Fake.Instance<XafApplication>();
            Isolate.WhenCalled(() => application.Model).WillReturn(DefaultDictionary);
            
            var control = editor.GetModelEditorControl();

            Assert.IsInstanceOfType(typeof(SettingsStorageOnDictionary),control.SettingsStorage);
        }

        [Test]
        [Isolated]
        public void It_Will_Be_Of_Type_Model_Editor_Control(){
            var editor = new ModelEditorPropertyEditor(null, null);
            var modelEditorControl = Isolate.Fake.Instance<ModelEditorControl>();
            Isolate.WhenCalled(() => editor.GetModelEditorControl()).WillReturn(modelEditorControl);

            editor.CreateControl();

            Assert.AreEqual(modelEditorControl, editor.Control);
        }
    }
}
