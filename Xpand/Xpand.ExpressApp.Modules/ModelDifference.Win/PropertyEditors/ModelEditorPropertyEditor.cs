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
        private ModelEditorViewController _modelEditorViewController;
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


        public ModelEditorViewController ModelEditorViewModelEditorViewController {
            get {
                if (_modelEditorViewController == null)
                    CreateModelEditorController();

                return _modelEditorViewController;
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
            if (_modelEditorViewController != null) {
                _modelEditorViewController.CurrentAspectChanged -= ModelEditorViewControllerOnCurrentAspectChanged;
                _modelEditorViewController.SaveAction.ExecuteCompleted -= SaveActionOnExecuteCompleted;
                _modelEditorViewController.Modifying -= Model_Modifying;
                _modelEditorViewController.ChangeAspectAction.ExecuteCompleted -= ChangeAspectActionOnExecuteCompleted;
                _modelEditorViewController.SaveSettings();
                _modelEditorViewController = null;
            }
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            new ModelValidator(new FastModelEditorHelper()).ValidateNode(_currentObjectModel);
            if (ModelEditorViewModelEditorViewController.SaveAction.Enabled)
                ModelEditorViewModelEditorViewController.SaveAction.DoExecute();
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
            _currentObjectModel = allLayers.Single(@base => @base.Id == CurrentObject.Name);
            _masterModel=_modelLoader.ReCreate();
//            foreach (var layer in allLayers)
//                ModelApplicationHelper.RemoveLayer(layer);
            foreach (var layer in allLayers) {
                ModelApplicationHelper.AddLayer(_masterModel, layer);
            }
            
            RuntimeMemberBuilder.AddFields((IModelApplication)_masterModel, XpandModuleBase.Dictiorary);

            DisposeController();

            _modelEditorViewController = new ModelEditorViewController((IModelApplication)_masterModel, null);
            _modelEditorViewController.SetControl(Control);
            _modelEditorViewController.LoadSettings();

            if (aspect != CaptionHelper.DefaultLanguage)
                _masterModel.CurrentAspectProvider.CurrentAspect = aspect;

            _modelEditorViewController.CurrentAspectChanged += ModelEditorViewControllerOnCurrentAspectChanged;
            _modelEditorViewController.SaveAction.ExecuteCompleted += SaveActionOnExecuteCompleted;
            _modelEditorViewController.Modifying += Model_Modifying;
            _modelEditorViewController.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
        }

        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _objectSpace.CommitChanges();
        }

        void ModelEditorViewControllerOnCurrentAspectChanged(object sender, EventArgs eventArgs) {
            var modelDifferenceObject = ((ModelDifferenceObject)View.CurrentObject);
            if (modelDifferenceObject.AspectObjects.FirstOrDefault(o => o.Name == ModelEditorViewModelEditorViewController.CurrentAspect) == null) {
                modelDifferenceObject.Model.AddAspect(ModelEditorViewModelEditorViewController.CurrentAspect);
                var aspectObject = _objectSpace.CreateObject<AspectObject>();
                aspectObject.Name = ModelEditorViewModelEditorViewController.CurrentAspect;
                modelDifferenceObject.AspectObjects.Add(aspectObject);
            }
        }

        void ChangeAspectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            View.Refresh();
        }
        #endregion
    }
}