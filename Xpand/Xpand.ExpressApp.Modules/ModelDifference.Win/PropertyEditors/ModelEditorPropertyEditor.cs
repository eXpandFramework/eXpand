using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Win.PropertyEditors {
    [PropertyEditor(typeof(ModelApplicationBase), true)]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexViewItem {
        static readonly LightDictionary<ModelApplicationBase, ITypesInfo> _modelApplicationBases;
        static readonly ITypesInfo _typeInfo  ;
        static ModelEditorPropertyEditor () {
            _typeInfo = XafTypesInfo.Instance;
            _modelApplicationBases=new LightDictionary<ModelApplicationBase, ITypesInfo>();
        }
        #region Members
        private ModelEditorViewController _modelEditorViewController;
        ModelLoader _modelLoader;
        ModelApplicationBase _masterModel;
        ModelApplicationBase _currentObjectModel;
        IObjectSpace _objectSpace;
        Form _form;
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
            _modelLoader = new ModelLoader(CurrentObject.PersistentApplication.ExecutableName, XafTypesInfo.Instance);
            InterfaceBuilder.SkipAssemblyCleanup = true;
            _masterModel = GetMasterModel();
            InterfaceBuilder.SkipAssemblyCleanup = false;
            base.OnCurrentObjectChanged();
        }

        ModelApplicationBase GetMasterModel() {
            var modelApplicationBase = _modelLoader.GetMasterModel(false);
            _modelApplicationBases.Add(modelApplicationBase, XafTypesInfo.Instance);
            _typeInfo.AssignAsInstance();
            return modelApplicationBase;
        }
        
        protected override object CreateControlCore() {
            CurrentObject.Changed += CurrentObjectOnChanged;
            _objectSpace.Committing += ObjectSpaceOnCommitting;
            var modelEditorControl = new ModelEditorControl(new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
            modelEditorControl.OnDisposing += modelEditorControl_OnDisposing;
            modelEditorControl.GotFocus+=ModelEditorControlOnGotFocus;
            return modelEditorControl;
        }

        void ModelEditorControlOnGotFocus(object sender, EventArgs eventArgs) {
            ((ModelEditorControl)sender).GotFocus-=ModelEditorControlOnGotFocus;
            _form = ((ModelEditorControl)sender).FindForm();
            Debug.Assert(_form != null, "form != null");
            _form.Deactivate += FormOnDeactivate;
            _form.Activated += FormOnActivated;
        }

        void FormOnActivated(object sender, EventArgs eventArgs) {
            _modelApplicationBases[_masterModel].AssignAsInstance();
        }

        void FormOnDeactivate(object sender, EventArgs eventArgs) {
            _typeInfo.AssignAsInstance();
        }

        private void modelEditorControl_OnDisposing(object sender, EventArgs e) {
            _form.Deactivate-=FormOnDeactivate;
            _form.Activated-=FormOnActivated;
            _modelApplicationBases.Remove(_masterModel);
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
                InterfaceBuilder.SkipAssemblyCleanup = true;
                _masterModel = GetMasterModel();
                InterfaceBuilder.SkipAssemblyCleanup = false;
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
            InterfaceBuilder.SkipAssemblyCleanup = true;
//            _masterModel = _modelLoader.ReCreate(XafTypesInfo.Instance);
            InterfaceBuilder.SkipAssemblyCleanup = false;
            foreach (var layer in allLayers) {
                ModelApplicationHelper.AddLayer(_masterModel, layer);
            }

            RuntimeMemberBuilder.CreateRuntimeMembers((IModelApplication)_masterModel);

            DisposeController();

            _modelEditorViewController = new ExpressApp.Win.ModelEditorViewController((IModelApplication)_masterModel, null);
            _modelEditorViewController.SetControl(Control);
            _modelEditorViewController.LoadSettings();

            if (aspect != CaptionHelper.DefaultLanguage)
                _masterModel.CurrentAspectProvider.CurrentAspect = aspect;

            _modelEditorViewController.CurrentAspectChanged += ModelEditorViewControllerOnCurrentAspectChanged;
            _modelEditorViewController.Modifying += Model_Modifying;
            _modelEditorViewController.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
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