using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Win.PropertyEditors
{
    [PropertyEditor(typeof(DictionaryNode))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        private XafApplication application;
        private ModelEditorController controller;

        public ModelEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
        }

        protected override object CreateControlCore()
        {
            Dictionary dictionary = application.Model;
            var dictionaryDifferenceStore = ((BaseObjects.XpoModelDictionaryDifferenceStore) CurrentObject);
            Dictionary applicationModel =
                ((XpoUserModelDictionaryDifferenceStore) dictionary.LastDiffStore).ApplicationModel.Clone();

            applicationModel.AddAspect(dictionaryDifferenceStore.Aspect, dictionaryDifferenceStore.Model);
            controller = new ModelEditorController(applicationModel,
                                                   new ModelDictionaryDifferenceStore(applicationModel,
                                                                                      new CustomDictionaryDifferenceStore
                                                                                          ((
                                                                                           BaseObjects.
                                                                                               XpoModelDictionaryDifferenceStore
                                                                                           ) CurrentObject)),
                                                   application.Modules);
            new SettingsStorageOnDictionary(applicationModel.RootNode.GetChildNode("ModelEditor"));
            controller.CurrentNodeChanged += ControllerOnCurrentNodeChanged;
            return
                new ModelEditorControl(
                    controller,
                    new SettingsStorageOnDictionary(applicationModel.RootNode.GetChildNode("ModelEditor")));
        }



        private void ControllerOnCurrentNodeChanged(object sender, EventArgs args)
        {
            Dictionary dictionary = controller.Dictionary.GetDiffs();
            ((PersistentBase) CurrentObject).ClassInfo.FindMember(PropertyName).SetValue(CurrentObject,dictionary.RootNode);
        }

        public void Setup(ObjectSpace space, XafApplication app)
        {
            application = app;
        }
    }
}