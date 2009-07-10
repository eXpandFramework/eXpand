using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.DictionaryDifferenceStore.DictionaryStores;
using eXpand.Persistent.Base;
using XpoModelDictionaryDifferenceStore=eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects.XpoModelDictionaryDifferenceStore;

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
            var dictionaryDifferenceStore = ((XpoModelDictionaryDifferenceStore) CurrentObject);
            Dictionary applicationModel =
                ((XpoUserModelDictionaryDifferenceStore) dictionary.LastDiffStore).ApplicationModel.Clone();

            applicationModel.AddAspect(dictionaryDifferenceStore.Aspect, dictionaryDifferenceStore.Model);
            controller = new ModelEditorController(applicationModel,
                                                   new ModelDictionaryDifferenceStore(applicationModel,
                                                                                      new CustomDictionaryDifferenceStore
                                                                                          ((
                                                                                           
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

            var modelDictionaryDifferenceStore = ((XpoModelDictionaryDifferenceStore) CurrentObject);
            var dictionary1 = new Dictionary(modelDictionaryDifferenceStore.Model);
            dictionary1.AddAspect(modelDictionaryDifferenceStore.Aspect, dictionary.RootNode);
            modelDictionaryDifferenceStore.Model = dictionary1.RootNode;
        }

        public void Setup(ObjectSpace space, XafApplication app)
        {
            application = app;
        }
    }
}