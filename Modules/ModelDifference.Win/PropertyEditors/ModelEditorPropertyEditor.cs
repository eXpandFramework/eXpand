using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
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
        private bool isModifying;


        public ModelEditorPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
            
        }

        protected override void ReadValueCore(){
            base.ReadValueCore();
            if (!isModifying)
                Control.Controller = GetModelEditorController(_application);
            
        }



        private void modifyModel(){
            if (Control.Controller.IsModified){
                Dictionary diffs = Control.Controller.Dictionary.GetDiffs();
                CurrentObject.Model.AddAspect(Control.Controller.CurrentAspect,diffs.RootNode);
                
                isModifying = true;
                CurrentObject.SetModelDirty();
                isModifying = false;
            }
        }

        private void ControllerOnCurrentNodeChanged(object sender, EventArgs args){
            modifyModel();
        }

        private void ControllerOnCurrentAttributeChanged(object sender, EventArgs args){
            modifyModel();
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
            Dictionary applicationModel = CurrentObject.GetModel();
            return GetModelEditorControl(applicationModel);
        }

        internal ModelEditorControl GetModelEditorControl(Dictionary applicationModel){
            editorControl = new ModelEditorControl(null,new SettingsStorageOnDictionary(applicationModel.RootNode.GetChildNode("ModelEditor")));
            return editorControl;
        }


        internal ModelEditorController GetModelEditorController(XafApplication application){
            var controller = new ModelEditorController(CurrentObject.GetModel(), null, application.Modules);
            controller.CurrentAttributeChanged += ControllerOnCurrentAttributeChanged;
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
            }

        }

    }
}