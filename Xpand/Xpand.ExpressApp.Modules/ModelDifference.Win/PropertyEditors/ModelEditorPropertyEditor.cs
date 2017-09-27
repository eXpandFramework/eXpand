using System;
using System.ComponentModel;
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
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.RuntimeMembers;
using PropertyChangingEventArgs = DevExpress.ExpressApp.Win.Core.ModelEditor.PropertyChangingEventArgs;

namespace Xpand.ExpressApp.ModelDifference.Win.PropertyEditors{
    [PropertyEditor(typeof(ModelApplicationBase), true)]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexViewItem{
        private static readonly LightDictionary<ModelApplicationBase, ITypesInfo> ModelApplicationBases;
        private static readonly ITypesInfo TypeInfo;

        static ModelEditorPropertyEditor(){
            TypeInfo = XafTypesInfo.Instance;
            ModelApplicationBases = new LightDictionary<ModelApplicationBase, ITypesInfo>();
        }

        #region Constructor

        public ModelEditorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model){
        }

        #endregion

        #region Eventhandler

        private void Model_Modifying(object sender, CancelEventArgs e){
            View.ObjectSpace.SetModified(CurrentObject);
        }

        #endregion

        #region Members

        private ModelEditorViewController _modelEditorViewController;
        private ModelLoader _modelLoader;
        private ModelApplicationBase _currentObjectModel;
        private IObjectSpace _objectSpace;
        private Form _form;
        private bool _xmlContentChanged;

        #endregion

        #region Properties

        public ModelApplicationBase MasterModel { get; private set; }

        public new ModelDifferenceObject CurrentObject{
            get { return base.CurrentObject as ModelDifferenceObject; }
            set { base.CurrentObject = value; }
        }

        public new ModelEditorControl Control => (ModelEditorControl) base.Control;


        public ModelEditorViewController ModelEditorViewModelEditorViewController{
            get{
                if (_modelEditorViewController == null)
                    CreateModelEditorController();

                return _modelEditorViewController;
            }
        }

        #endregion

        #region Overrides

        protected override void OnCurrentObjectChanged(){
            _modelLoader = new ModelLoader(CurrentObject.PersistentApplication.ExecutableName, XafTypesInfo.Instance);
            MasterModel = GetMasterModel(false);
            base.OnCurrentObjectChanged();
        }

        private ModelApplicationBase GetMasterModel(bool recreate){
            var modelApplicationBase = GetMasterModelCore(recreate);
            ModelApplicationBases.Add(modelApplicationBase, XafTypesInfo.Instance);
            TypeInfo.AssignAsInstance();
            return modelApplicationBase;
        }

        private ModelApplicationBase GetMasterModelCore(bool recreate){
            InterfaceBuilder.SkipAssemblyCleanup = true;
            var masterModel = !recreate ? _modelLoader.GetMasterModel(false) : _modelLoader.ReCreate();
            InterfaceBuilder.SkipAssemblyCleanup = false;
            return masterModel;
        }

        protected override object CreateControlCore(){
            CurrentObject.Changed += CurrentObjectOnChanged;
            _objectSpace.Committing += ObjectSpaceOnCommitting;
            var modelEditorControl =
                new ModelEditorControl(
                    new SettingsStorageOnRegistry(@"Software\Developer Express\eXpressApp Framework\Model Editor"));
            modelEditorControl.OnDisposing += modelEditorControl_OnDisposing;
            modelEditorControl.GotFocus += ModelEditorControlOnGotFocus;
            modelEditorControl.Enter+=ModelEditorLoadXml;
            return modelEditorControl;
        }

        private void ModelEditorLoadXml(object sender, EventArgs eventArgs){
            if (_xmlContentChanged){
                _xmlContentChanged = false;
                MergeXmlWithModel();
            }
        }

        public void MergeXmlWithModel(){
            var aspect = MasterModel.CurrentAspect;
            InterfaceBuilder.SkipAssemblyCleanup = true;
            MasterModel = GetMasterModel(false);
            InterfaceBuilder.SkipAssemblyCleanup = false;
            CreateModelEditorController(aspect);
        }

        private void ModelEditorControlOnGotFocus(object sender, EventArgs eventArgs){
            ((ModelEditorControl) sender).GotFocus -= ModelEditorControlOnGotFocus;
            _form = ((ModelEditorControl) sender).FindForm();
            if (_form != null){
                _form.Deactivate += FormOnDeactivate;
                _form.Activated += FormOnActivated;
            }
            
        }

        private void FormOnActivated(object sender, EventArgs eventArgs){
            var typesInfo = ModelApplicationBases[MasterModel];
            typesInfo.AssignAsInstance();
        }

        private void FormOnDeactivate(object sender, EventArgs eventArgs){
            TypeInfo.AssignAsInstance();
        }

        private void modelEditorControl_OnDisposing(object sender, EventArgs e){
            if (_form != null){
                _form.Deactivate -= FormOnDeactivate;
                _form.Activated -= FormOnActivated;
            }
            ModelApplicationBases.Remove(MasterModel);
            Control.OnDisposing -= modelEditorControl_OnDisposing;

            DisposeController();
        }

        protected override void Dispose(bool disposing){
            try{
                if (CurrentObject != null)
                    CurrentObject.Changed -= CurrentObjectOnChanged;

                if (_objectSpace != null){
                    _objectSpace.Committing -= ObjectSpaceOnCommitting;
                    _objectSpace = null;
                }
            }
            finally{
                base.Dispose(disposing);
                TypeInfo.AssignAsInstance();
            }
        }

        private void DisposeController(){
            if (_modelEditorViewController != null){
                _modelEditorViewController.CurrentAspectChanged -= ModelEditorViewControllerOnCurrentAspectChanged;
                _modelEditorViewController.Modifying -= Model_Modifying;
                _modelEditorViewController.ChangeAspectAction.ExecuteCompleted -= ChangeAspectActionOnExecuteCompleted;
                _modelEditorViewController.ModelAttributesPropertyEditorController.PropertyChanged -= ModelAttributesPropertyEditorControllerOnPropertyChanged;
                if (_modelEditorViewController.ModelEditorControl != null)
                    _modelEditorViewController.SaveSettings();
                _modelEditorViewController.SelectedNodes.Clear();
                _modelEditorViewController = null;
            }
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            new ModelValidator(new FastModelEditorHelper()).ValidateNode(_currentObjectModel);
            if (ModelEditorViewModelEditorViewController.SaveAction.Enabled)
                ModelEditorViewModelEditorViewController.SaveAction.DoExecute();
            CurrentObject.CreateAspectsCore(_currentObjectModel);
        }


        private void CurrentObjectOnChanged(object sender, ObjectChangeEventArgs objectChangeEventArgs){
            if (objectChangeEventArgs.PropertyName == nameof(IXpoModelDifference.XmlContent)){
                _xmlContentChanged = true;
            }
        }

        #endregion

        #region Methods

        public void Setup(IObjectSpace objectSpace, XafApplication application){
            _objectSpace = objectSpace;
        }

        private void CreateModelEditorController(){
            const string defaultLanguage = CaptionHelper.DefaultLanguage;
            CreateModelEditorController(defaultLanguage);
        }

        private void CreateModelEditorController(string aspect){
            var allLayers = CurrentObject.GetAllLayers(MasterModel).ToList();
            _currentObjectModel = allLayers.First(@base => @base.Id == $"{CurrentObject.Name}-{CurrentObject.DeviceCategory}");
            MasterModel = GetMasterModel(true);
            foreach (var layer in allLayers){
                ModelApplicationHelper.AddLayer(MasterModel, layer);
            }
            ModelApplicationBases[MasterModel].AssignAsInstance();
            RuntimeMemberBuilder.CreateRuntimeMembers((IModelApplication) MasterModel);
            TypeInfo.AssignAsInstance();

            DisposeController();

            _modelEditorViewController = new ExpressApp.Win.ModelEditorViewController((IModelApplication) MasterModel,
                null);
            if (Control != null){
                _modelEditorViewController.SetControl(Control);
                _modelEditorViewController.LoadSettings();
            }

            if (aspect != CaptionHelper.DefaultLanguage)
                MasterModel.CurrentAspectProvider.CurrentAspect = aspect;

            _modelEditorViewController.CurrentAspectChanged += ModelEditorViewControllerOnCurrentAspectChanged;
            _modelEditorViewController.Modifying += Model_Modifying;
            _modelEditorViewController.ChangeAspectAction.ExecuteCompleted += ChangeAspectActionOnExecuteCompleted;
            _modelEditorViewController.ModelAttributesPropertyEditorController.PropertyChanged += ModelAttributesPropertyEditorControllerOnPropertyChanged;
        }

        private void ModelAttributesPropertyEditorControllerOnPropertyChanged(object sender, PropertyChangingEventArgs propertyChangingEventArgs) {
            CurrentObject.CreateAspectsCore(_currentObjectModel);
        }

        private void ModelEditorViewControllerOnCurrentAspectChanged(object sender, EventArgs eventArgs){
            var modelDifferenceObject = (ModelDifferenceObject) View.CurrentObject;
            if (
                modelDifferenceObject.AspectObjects.FirstOrDefault(
                    o => o.Name == ModelEditorViewModelEditorViewController.CurrentAspect) == null){
                modelDifferenceObject.Model.AddAspect(ModelEditorViewModelEditorViewController.CurrentAspect);
                var aspectObject = _objectSpace.CreateObject<AspectObject>();
                aspectObject.Name = ModelEditorViewModelEditorViewController.CurrentAspect;
                modelDifferenceObject.AspectObjects.Add(aspectObject);
            }
        }

        private void ChangeAspectActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs){
            View.Refresh();
        }

        #endregion
    }
}