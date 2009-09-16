using DevExpress.ExpressApp.Win.Core.ModelEditor;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    [TestFixture]
    public class At_Reading_Value
    {
        [Test]
        [Isolated]
        public void ModelEditorControl_Controller_Should_Be_Set_Every_Time_Editor_ReadsValue()
        {
            var mock = MockManager.Mock(typeof(ModelEditorPropertyEditor));
            mock.CallBase.ExpectCall("ReadValueCore");
            var editor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => editor.Control).WillReturn(new ModelEditorControl());
            Isolate.NonPublic.WhenCalled(editor, "CanReadValue").WillReturn(true);
            var controller = Isolate.Fake.Instance<ModelEditorController>();
            Isolate.WhenCalled(() => editor.GetModelEditorController(null)).WillReturn(controller);

            editor.ReadValue();

            Assert.AreEqual(controller, editor.Control.Controller);


        }
    }
}
