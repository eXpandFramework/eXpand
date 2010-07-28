using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Win.PropertyEditors
{
    [PropertyEditor(typeof(ModelApplicationBase))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        #region Members

        
        private ModelEditorViewController _controller;
        

        #endregion

        #region Constructor

        public ModelEditorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        #endregion

        #region Properties

        public new ModelDifferenceObject CurrentObject
        {
            get { return base.CurrentObject as ModelDifferenceObject; }
            set { base.CurrentObject = value; }
        }

        public new ModelEditorControl Control
        {
            get { return (ModelEditorControl)base.Control; }
        }


        public ModelEditorViewController ModelEditorViewController
        {
            get { return _controller ?? (_controller = GetModelEditorController()); }
        }


        #endregion

        #region Overrides

        protected override void ReadValueCore()
        {
            base.ReadValueCore();

            if (_controller == null)
                _controller = GetModelEditorController();
        }

        protected override void OnCurrentObjectChanged(){
            if (Control != null){
                Control.CurrentModelNode = null;
                _controller.Modifying -= Model_Modifying;
                _controller = null;
            }

            ModelDifferenceModule.MasterModel = new ModelApplicationBuilder(CurrentObject.PersistentApplication.ExecutableName).GetMasterModel();
            base.OnCurrentObjectChanged();
        }


        protected override object CreateControlCore() {
            var modelEditorControl = new ModelEditorControl(new SettingsStorageOnDictionary());
            return modelEditorControl;
        }

        #endregion

        #region Eventhandler

        private void Model_Modifying(object sender, System.ComponentModel.CancelEventArgs e)
        {
            View.ObjectSpace.SetModified(CurrentObject);
        }

        private void SpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args)
        {
            if (Control == null) return;
            if (ReferenceEquals(args.Object, CurrentObject))
            {
                new ModelValidator().ValidateModel(CurrentObject.Model);
                ModelEditorViewController.SaveAction.Active["Not needed"] = true;
                ModelEditorViewController.Save();
                ModelEditorViewController.SaveAction.Active["Not needed"] = false;
            }
        }

        #endregion

        #region Methods

        public void Setup(ObjectSpace objectSpace, XafApplication application)
        {
            objectSpace.ObjectSaving += SpaceOnObjectSaving;
        }


        private ModelEditorViewController GetModelEditorController()
        {
            ModelDifferenceModule.MasterModel.AddLayers(CurrentObject.GetAllLayers());

            _controller = new ModelEditorViewController((IModelApplication)ModelDifferenceModule.MasterModel, null, null);
            _controller.SetControl(Control);
            ModelDifferenceModule.MasterModel = null;
            _controller.Modifying += Model_Modifying;
            _controller.SaveAction.Active["Not needed"] = false;
            
            return _controller;
        }

        #endregion
    }
}