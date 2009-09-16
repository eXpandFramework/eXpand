using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    [TestFixture]
    public class When_Controller_IsModified:eXpandBaseFixture
    {
        

        [Test][Isolated]
        public void CurrentObjectModel_Should_Be_Equal_CurrentObject_CombinedModel_Combined_With_Controller_Diffs(){

            var modelEditorPropertyEditor = new ModelEditorPropertyEditor(null, null);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.CurrentObject).WillReturn(modelDifferenceObject);
            Isolate.WhenCalled(() => modelDifferenceObject.GetCombinedModel()).WillReturn(DefaultDictionary);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.Control.Controller.IsModified).WillReturn(true);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.Control.Controller.Dictionary.GetDiffs()).WillReturn(elDictionary);

            modelEditorPropertyEditor.ModifyCurrentObjectModel();
            
            Assert.AreEqual("el", new ApplicationNodeWrapper(modelDifferenceObject.Model).BOModel.FindClassByName("MyClass").Caption);
        }
    }
}
