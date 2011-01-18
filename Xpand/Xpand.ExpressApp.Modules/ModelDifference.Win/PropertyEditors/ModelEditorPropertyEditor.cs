using System;
using System.ComponentModel;
using System.Linq;
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

namespace Xpand.ExpressApp.ModelDifference.Win.PropertyEditors {
    [PropertyEditor(typeof(ModelApplicationBase))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor {
        #region Members




        private ModelEditorViewController _controller;
        ModelLoader _modelLoader;
        ModelApplicationBase _masterModel;
        ModelApplicationBase _currentObjectModel;
        IObjectSpace _objectSpace;
        #endregion

        #region Constructor

        public ModelEditorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        #endregion


        #region Properties
        public ModelApplicationBase MasterModel {
            get { return _masterModel; }
        }

        public new ModelDifferenceObject CurrentObject {
            get { return base.CurrentObject as ModelDifferenceObject; }
            set { base.CurrentObject = value; }
        }

        public new ModelEditorControl Control {
            get { return (ModelEditorControl)base.Control; }
        }


        public ModelEditorViewController ModelEditorViewController {
            get { return _controller ?? (_controller = GetModelEditorController(CaptionHelper.DefaultLanguage)); }
        }


        #endregion

        #region Overrides

        protected override void ReadValueCore() {
            base.ReadValueCore();
            if (_controller == null) {
                _controller = GetModelEditorController(CaptionHelper.DefaultLanguage);
            }
        }


        protected override void OnCurrentObjectChanged() {
            _modelLoader = new ModelLoader(CurrentObject.PersistentApplication.ExecutableName);
            _masterModel = _modelLoader.GetMasterModel();
            base.OnCurrentObjectChanged();
        }



        protected override object CreateControlCore() {
            View.Closing += ViewOnClosing;
            CurrentObject.Changed += CurrentObjectOnChanged;
            _objectSpace.Committing +=ObjectSpaceOnCommitting;
            var modelEditorControl = new ModelEditorControl(new SettingsStorageOnDictionary());
            return modelEditorControl;
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            new ModelValidator().ValidateNode(_currentObjectModel);
            if (ModelEditorViewController.SaveAction.Enabled)
                ModelEditorViewController.SaveAction.DoExecute();
            CurrentObject.CreateAspectsCore(_currentObjectModel);
        }


        void CurrentObjectOnChanged(object sender, ObjectChangeEventArgs objectChangeEventArgs) {
            if (objectChangeEventArgs.PropertyName == "XmlContent") {
                var aspect = _masterModel.CurrentAspect;
                _masterModel = _modelLoader.GetMasterModel();
                _controller = GetModelEditorController(aspect);
            }
        }


        void ViewOnClosing(object sender, EventArgs eventArgs) {
            _objectSpace.Committing-=ObjectSpaceOnCommitting;
        }

        #endregion

        #region Eventhandler

        private void Model_Modifying(object sender, CancelEventArgs e) {
            View.ObjectSpace.SetModified(CurrentObject);
        }

        #endregion

        #region Methods

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
        }


        private ModelEditorViewController GetModelEditorController(string aspect) {
            var allLayers = CurrentObject.GetAllLayers(_masterModel);
            _currentObjectModel = allLayers.Where(@base => @base.Id == CurrentObject.Name).Single();
            _masterModel.AddLayers(allLayers.ToArray());
            var controller = new ModelEditorViewController((IModelApplication)_masterModel, null);
            controller.CurrentAspectChanged+=ControllerOnCurrentAspectChanged;
            controller.SaveAction.ExecuteCompleted+=SaveActionOnExecuteCompleted;
            _masterModel.CurrentAspectProvider.CurrentAspect = aspect;
            controller.SetControl(Control);
            controller.Modifying += Model_Modifying;
            controller.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
            return controller;
        }

        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _objectSpace.CommitChanges();
        }

        void ControllerOnCurrentAspectChanged(object sender, EventArgs eventArgs) {
            var modelDifferenceObject = ((ModelDifferenceObject) View.CurrentObject);
            if (modelDifferenceObject.AspectObjects.Where(o => o.Name==_controller.CurrentAspect).FirstOrDefault()==null) {
                modelDifferenceObject.Model.AddAspect(_controller.CurrentAspect);
                var aspectObject = _objectSpace.CreateObject<AspectObject>();
                aspectObject.Name = _controller.CurrentAspect;
                modelDifferenceObject.AspectObjects.Add(aspectObject);
            }
        }

        void ChangeAspectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            View.Refresh();
        }
        #endregion
    }
}