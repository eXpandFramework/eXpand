using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.ModelPropertyEditor
{
    [TestFixture]
    public class When_Controller_IsModified:XpandBaseFixture
    {
        

        [Test][Isolated]
        public void CurrentObjectModel_Should_Be_Equal_CurrentObject_PersistentApp_Not_Modified_Cloned_Combined_With_Controller_Diffs(){

            string s = "<Application><BOModel><Class Name=\"MyClass2\" Caption=\"el\"></Class></BOModel></Application>";
            var modelEditorPropertyEditor = new ModelEditorPropertyEditor(null, null);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.Control.Controller.IsModified).WillReturn(true);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.CurrentObject).WillReturn(modelDifferenceObject);
            Isolate.WhenCalled(() => modelDifferenceObject.PersistentApplication.Model.Clone()).WillReturn(DefaultDictionary2);
            Isolate.WhenCalled(() => modelEditorPropertyEditor.Control.Controller.Dictionary.GetDiffs()).WillReturn(new Dictionary(new DictionaryXmlReader().ReadFromString(s),Schema.GetCommonSchema()));

            modelEditorPropertyEditor.ModifyCurrentObjectModel();
            
            Assert.AreEqual("el", new ApplicationNodeWrapper(modelDifferenceObject.Model).BOModel.FindClassByName("MyClass2").Caption);
        }
    }
}
