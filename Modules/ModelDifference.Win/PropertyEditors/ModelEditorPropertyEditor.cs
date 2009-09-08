using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Win.PropertyEditors
{
    [PropertyEditor(typeof(Dictionary))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        private XafApplication _application;
        private ModelEditorControl editorControl;
        private bool modifiedChanged;


        public ModelEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
            
        }

        protected override void ReadValueCore(){
            base.ReadValueCore();
            if (!modifiedChanged)
                Control.Controller = GetModelEditorController(_application);
            Control.Controller.ModifiedChanged += ControllerOnModifiedChanged;            
            
        }



        private void ControllerOnModifiedChanged(object sender, EventArgs args){
            modifiedChanged = true;
            CurrentObject.Model = Control.Controller.Dictionary.GetDiffs();
            modifiedChanged = false;
        }


        public new ModelDifferenceObject CurrentObject{
            get { return base.CurrentObject as ModelDifferenceObject; }
            set { base.CurrentObject = value; }
        }

        public new ModelEditorControl Control
        {
            get { return editorControl; }
        }

        protected override object CreateControlCore()
        {
            Dictionary applicationModel = GetModel();
            return GetModelEditorControl(applicationModel);
        }

        internal ModelEditorControl GetModelEditorControl(Dictionary applicationModel){
            editorControl = new ModelEditorControl(null,new SettingsStorageOnDictionary(applicationModel.RootNode.GetChildNode("ModelEditor")));
            return editorControl;
        }


        internal ModelEditorController GetModelEditorController(XafApplication application){
            var controller = new ModelEditorController(GetControllerModel(), null, application.Modules);
            controller.SetCurrentAspectByName(CurrentObject.CurrentLanguage);
            return controller;
        }

        internal Dictionary GetControllerModel(){
            var dictionary = GetModel();
            if (Control.Controller != null){
                var combiner = new DictionaryCombiner(dictionary);
                combiner.CombineWith(Control.Controller.Dictionary);
            }
            return dictionary;
        }

        public Dictionary GetModel(){
            var combiner = new DictionaryCombiner(CurrentObject.PersistentApplication.Model);
            combiner.CombineWith(CurrentObject);
            return CurrentObject.PersistentApplication.Model;
//            var persistentAppDictionary = dictionaryDifferenceStore.PersistentApplication.Model;
//            var dictionary = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), dictionaryDifferenceStore.PersistentApplication.Model.Schema);
//            var combiner = new DictionaryCombiner(dictionary);
//            combiner.CombineWith(persistentAppDictionary);
//            combiner.CombineWith(dictionaryDifferenceStore.Model);

//            combiner= new DictionaryCombiner(dictionaryDifferenceStore.Model);
//            combiner.CombineWith(dictionaryDifferenceStore);
//            combiner = new DictionaryCombiner(dictionary);
//            combiner.CombineWith(dictionaryDifferenceStore.Model);
//            return dictionary;
        }

        public void Setup(ObjectSpace space, XafApplication app)
        {
            _application = app;
            space.ObjectSaving+=SpaceOnObjectSaving;
        }

        private void SpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args){
            if (ReferenceEquals(args.Object, CurrentObject)){
                Control.Controller.Dictionary.Validate();
                var dictionary = Control.Controller.Dictionary.GetDiffs();
                CurrentObject.Model = dictionary;
            }

        }

    }
}