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
        
        private bool isModifying;


        public ModelEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
            
        }

        protected override void ReadValueCore(){
            base.ReadValueCore();
            if (!isModifying)
                Control.Controller = GetModelEditorController(_application);
            
        }


        internal void ModifyCurrentObjectModel(){
            if (Control.Controller.IsModified){
                Dictionary diffs = Control.Controller.Dictionary.GetDiffs();
                var model = CurrentObject.GetCombinedModel();
                model.CombineWith(diffs);
                isModifying = true;
                CurrentObject.Model = model.GetDiffs();
                isModifying = false;
            }
        }

        private void ControllerOnCurrentNodeChanged(object sender, EventArgs args){
            ModifyCurrentObjectModel();
        }

        private void ControllerOnCurrentAttributeChanged(object sender, EventArgs args){
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
            return new ModelEditorControl(null, new SettingsStorageOnDictionary(CurrentObject.GetCombinedModel().RootNode.GetChildNode("ModelEditor")));
        }


        internal ModelEditorController GetModelEditorController(XafApplication application){
            var controller = new ModelEditorController(CurrentObject.GetCombinedModel(), null, application.Modules);
            controller.CurrentAttributeValueChanged += ControllerOnCurrentAttributeChanged;
            controller.CurrentNodeChanged += ControllerOnCurrentNodeChanged;
            controller.SetCurrentAspectByName(CurrentObject.CurrentLanguage);
            return controller;
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
            }

        }

    }
}