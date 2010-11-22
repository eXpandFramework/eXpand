using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using System.Linq;

namespace Xpand.ExpressApp.ModelDifference.Win.PropertyEditors
{
    [PropertyEditor(typeof(ModelApplicationBase))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor, ISupportMasterModel {
        #region Members
        public event EventHandler ModelCreated;

        protected void OnModelCreated(EventArgs e) {
            EventHandler handler = ModelCreated;
            if (handler != null) handler(this, e);
        }


        private ModelEditorViewController _controller;
        ModelApplicationBuilder _modelApplicationBuilder;
        ModelApplicationBase _masterModel;
        ModelApplicationBase _currentObjectModel;
        IObjectSpace _objectSpace;
        #endregion

        #region Constructor

        public ModelEditorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        #endregion


        #region Properties
        public ModelApplicationBase MasterModel {
            get { return _masterModel; }
        }

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
            get { return _controller ?? (_controller = GetModelEditorController(CaptionHelper.DefaultLanguage)); }
        }


        #endregion

        #region Overrides

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            if (_controller == null) {
                _controller = GetModelEditorController(CaptionHelper.DefaultLanguage);
            }
        }


        protected override void OnCurrentObjectChanged(){
            _modelApplicationBuilder = new ModelApplicationBuilder(CurrentObject.PersistentApplication.ExecutableName);
            _masterModel = _modelApplicationBuilder.GetMasterModel();
            base.OnCurrentObjectChanged();
        }



        protected override object CreateControlCore() {
            View.Closing+=ViewOnClosing;
            CurrentObject.Changed+=CurrentObjectOnChanged;
            _objectSpace.ObjectSaving+=ObjectSpaceOnObjectSaving;
            var modelEditorControl = new ModelEditorControl(new SettingsStorageOnDictionary());
            return modelEditorControl;
        }


        void CurrentObjectOnChanged(object sender, ObjectChangeEventArgs objectChangeEventArgs) {
            if (objectChangeEventArgs.PropertyName=="XmlContent") {
                var aspect = _masterModel.CurrentAspect;
                _masterModel = _modelApplicationBuilder.GetMasterModel();
                _controller = GetModelEditorController(aspect);
            }
        }

        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args) {
            if (ReferenceEquals(args.Object, CurrentObject)){
                //var clone = _currentObjectModel.Clone();
                new ModelValidator().ValidateNode(_currentObjectModel);
                ModelEditorViewController.SaveAction.Active["Not needed"] = true;
                ModelEditorViewController.Save();
                CurrentObject.CreateAspectsCore(_currentObjectModel);
                ModelEditorViewController.SaveAction.Active["Not needed"] = false;
            }
        }

        void ViewOnClosing(object sender, EventArgs eventArgs) {
            _objectSpace.ObjectSaving-=ObjectSpaceOnObjectSaving;
        }

        #endregion

        #region Eventhandler

        private void Model_Modifying(object sender, CancelEventArgs e)
        {
            View.ObjectSpace.SetModified(CurrentObject);
        }

#endregion

        #region Methods

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            _objectSpace = objectSpace;
        }


        private ModelEditorViewController GetModelEditorController(string aspect) {
            var allLayers = CurrentObject.GetAllLayers(_masterModel);
            _currentObjectModel = allLayers.Where(@base => @base.Id == CurrentObject.Name).Single();
            _masterModel.AddLayers(allLayers.ToArray());
            var controller = new ModelEditorViewController((IModelApplication)_masterModel, null, null);
            _masterModel.CurrentAspectProvider.CurrentAspect = aspect;
            controller.SetControl(Control);
            controller.Modifying += Model_Modifying;
            controller.SaveAction.Active["Not needed"] = false;
            controller.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
            OnModelCreated(EventArgs.Empty);
            return controller;
        }

        void ChangeAspectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            View.Refresh();
        }
        #endregion
    }
}