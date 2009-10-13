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


        public ModelEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
            
        }

        protected override void ReadValueCore(){
            base.ReadValueCore();

            if (Control.Controller== null)
                Control.Controller = GetModelEditorController(_application);

            Control.Controller.Dictionary.AddAspect(CurrentObject.CurrentLanguage,new DictionaryNode("Application"));
            Control.Controller.SetCurrentAspectByName(CurrentObject.CurrentLanguage);
        }


        internal void ModifyCurrentObjectModel(){
            if (Control.Controller.IsModified){
                Dictionary diffs = Control.Controller.Dictionary.GetDiffs();
                var model = CurrentObject.PersistentApplication.Model.Clone();
                model.ResetIsModified();
                model.CombineWith(diffs);
                CurrentObject.Model = model.GetDiffs();
            }
        }



        private void ControllerModifiedChanged(object sender, EventArgs args)
        {
            ModifyCurrentObjectModel();
        }

        public new ModelDifferenceObject CurrentObject{
            get { return base.CurrentObject as ModelDifferenceObject; }
            set { base.CurrentObject = value; }
        }

        public new ModelEditorControl Control
        {
            get { return (ModelEditorControl) base.Control; }
        }

        public XafApplication Application{
            get {
                return _application;
            }
        }

        protected override object CreateControlCore()
        {
            return GetModelEditorControl();
        }

        internal ModelEditorControl GetModelEditorControl(){
            var settingsStorageOnDictionary = new SettingsStorageOnDictionary(CurrentObject.GetCombinedModel().RootNode.GetChildNode("ModelEditor"));
            return new ModelEditorControl(null, settingsStorageOnDictionary);
        }


        internal ModelEditorController GetModelEditorController(XafApplication application){
            var controller = new ModelEditorController(CurrentObject.GetCombinedModel(), new DummyStore(), application.Modules);
            controller.ModifiedChanged += ControllerModifiedChanged;
            controller.SetCurrentAspectByName(CurrentObject.CurrentLanguage);
            return controller;
        }

        private class DummyStore:DictionaryDifferenceStore {
            protected override Dictionary LoadDifferenceCore(Schema schema) {
                throw new NotImplementedException();
            }

            public override string Name {
                get { throw new NotImplementedException(); }
            }

            public override void SaveDifference(Dictionary diffDictionary) {
                
            }
        }
        

        public void Setup(ObjectSpace space, XafApplication app)
        {
            _application = app;
            space.ObjectSaving+=SpaceOnObjectSaving;
        }

        private void SpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args){
            if (ReferenceEquals(args.Object, CurrentObject)){
                Control.Controller.Dictionary.Validate();
                ModifyCurrentObjectModel();
                Control.Controller.Save();
            }

        }

    }
}