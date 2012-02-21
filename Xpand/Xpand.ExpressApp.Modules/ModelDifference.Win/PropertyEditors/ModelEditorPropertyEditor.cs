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
using Xpand.ExpressApp.Core;
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
            get {
                if (_controller == null)
                    CreateModelEditorController();

                return _controller;
            }
        }


        #endregion

        #region Overrides

        protected override void OnCurrentObjectChanged() {
            _modelLoader = new ModelLoader(CurrentObject.PersistentApplication.ExecutableName);
            _masterModel = _modelLoader.GetMasterModel(false);
            base.OnCurrentObjectChanged();
        }



        protected override object CreateControlCore() {
            CurrentObject.Changed += CurrentObjectOnChanged;
            _objectSpace.Committing += ObjectSpaceOnCommitting;
            var modelEditorControl = new ModelEditorControl(new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
            modelEditorControl.OnDisposing += modelEditorControl_OnDisposing;
            return modelEditorControl;
        }

        private void modelEditorControl_OnDisposing(object sender, EventArgs e) {
            Control.OnDisposing -= modelEditorControl_OnDisposing;
            DisposeController();
        }

        protected override void Dispose(bool disposing) {
            try {
                if (CurrentObject != null)
                    CurrentObject.Changed -= CurrentObjectOnChanged;

                if (_objectSpace != null) {
                    _objectSpace.Committing -= ObjectSpaceOnCommitting;
                    _objectSpace = null;
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        private void DisposeController() {
            if (_controller != null) {
                _controller.CurrentAspectChanged -= ControllerOnCurrentAspectChanged;
                _controller.SaveAction.ExecuteCompleted -= SaveActionOnExecuteCompleted;
                _controller.Modifying -= Model_Modifying;
                _controller.ChangeAspectAction.ExecuteCompleted -= ChangeAspectActionOnExecuteCompleted;
                _controller.SaveSettings();
                _controller = null;
            }
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
                _masterModel = _modelLoader.GetMasterModel(false);
                CreateModelEditorController(aspect);
            }
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

        private void CreateModelEditorController() {
            const string defaultLanguage = CaptionHelper.DefaultLanguage;
            CreateModelEditorController(defaultLanguage);
        }

        private void CreateModelEditorController(string aspect) {
            var allLayers = CurrentObject.GetAllLayers(_masterModel).ToList();
            _currentObjectModel = allLayers.Where(@base => @base.Id == CurrentObject.Name).Single();
            foreach (var layer in allLayers)
                _masterModel.RemoveLayer(layer);

            _masterModel.AddLayers(allLayers.ToArray());
            RuntimeMemberBuilder.AddFields((IModelApplication)_masterModel, XpandModuleBase.Dictiorary);

            DisposeController();

            _controller = new ModelEditorViewController((IModelApplication)_masterModel, null);
            _controller.SetControl(Control);
            _controller.LoadSettings();

            if (aspect != CaptionHelper.DefaultLanguage)
                _masterModel.CurrentAspectProvider.CurrentAspect = aspect;

            _controller.CurrentAspectChanged += ControllerOnCurrentAspectChanged;
            _controller.SaveAction.ExecuteCompleted += SaveActionOnExecuteCompleted;
            _controller.Modifying += Model_Modifying;
            _controller.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
        }

        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _objectSpace.CommitChanges();
        }

        void ControllerOnCurrentAspectChanged(object sender, EventArgs eventArgs) {
            var modelDifferenceObject = ((ModelDifferenceObject)View.CurrentObject);
            if (modelDifferenceObject.AspectObjects.Where(o => o.Name == ModelEditorViewController.CurrentAspect).FirstOrDefault() == null) {
                modelDifferenceObject.Model.AddAspect(ModelEditorViewController.CurrentAspect);
                var aspectObject = _objectSpace.CreateObject<AspectObject>();
                aspectObject.Name = ModelEditorViewController.CurrentAspect;
                modelDifferenceObject.AspectObjects.Add(aspectObject);
            }
        }

        void ChangeAspectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            View.Refresh();
        }
        #endregion
    }
}