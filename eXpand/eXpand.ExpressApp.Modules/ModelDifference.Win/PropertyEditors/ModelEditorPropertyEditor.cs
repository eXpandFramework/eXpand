using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Win.PropertyEditors
{
    [PropertyEditor(typeof(ModelApplicationBase))]
    public class ModelEditorPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        #region Members

        private XafApplication _application;
        private ModelEditorViewController controller;
        private ModelApplicationBase masterModel;

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

        public XafApplication Application
        {
            get
            {
                return _application;
            }
        }

        public ModelEditorViewController ModelEditorViewController
        {
            get { return controller ?? (controller = GetModelEditorController()); }
        }

        public ModelApplicationBase MasterModel
        {
            get
            {
                return masterModel;
            }
        }

        #endregion

        #region Overrides

        protected override void ReadValueCore()
        {
            base.ReadValueCore();

            if (controller == null)
                controller = GetModelEditorController();
        }

        protected override void OnCurrentObjectChanged()
        {
            if (Control != null)
            {
                Control.CurrentModelNode = null;
                controller.Modifying -= Model_Modifying;
                controller = null;
            }

            masterModel = GetMasterModel();
            ModuleBase.ModelApplicationCreator = masterModel.CreatorInstance;

            base.OnCurrentObjectChanged();
        }

        protected override object CreateControlCore()
        {
            return new ModelEditorControl(new SettingsStorageOnDictionary());
        }

        #endregion

        #region Eventhandler

        private void Model_Modifying(object sender, System.ComponentModel.CancelEventArgs e)
        {
            View.ObjectSpace.SetModified(CurrentObject);
        }

        private void SpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args)
        {
            if (ReferenceEquals(args.Object, CurrentObject))
            {
                new ModelValidator().ValidateModel(CurrentObject.Model);
                controller.SaveAction.Active["Not needed"] = true;
                controller.Save();
                controller.SaveAction.Active["Not needed"] = false;
            }
        }

        #endregion

        #region Methods

        public void Setup(ObjectSpace objectSpace, XafApplication application)
        {
            _application = application;
            objectSpace.ObjectSaving += SpaceOnObjectSaving;
        }

        private ModelEditorViewController GetModelEditorController()
        {
            masterModel.AddLayers(CurrentObject.GetAllLayers());

            controller = new ModelEditorViewController((IModelApplication)masterModel, null, null);
            controller.SetControl(Control);
            controller.Modifying += Model_Modifying;
            controller.SaveAction.Active["Not needed"] = false;
            
            return controller;
        }

        private ModelApplicationBase GetMasterModel()
        {
            var application = GetApplication(CurrentObject.PersistentApplication.ExecutableName);

            var modulesManager = new DesignerModelFactory().CreateApplicationModelManager(
                application,
                string.Empty,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            ReadModulesFromConfig(modulesManager, application);

            modulesManager.Load();

            var modelsManager = new ApplicationModelsManager(
                modulesManager.Modules,
                modulesManager.ControllersManager,
                modulesManager.DomainComponents);

            return modelsManager.CreateModelApplication() as ModelApplicationBase;
        }

        private XafApplication GetApplication(string executableName)
        {
            string assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                ReflectionHelper.AddResolvePath(assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(executableName), assemblyPath);
                var assemblyInfo = XafTypesInfo.Instance.FindAssemblyInfo(assembly);
                XafTypesInfo.Instance.LoadTypes(assembly);
                return Enumerator.GetFirst(ReflectionHelper.FindTypeDescendants(assemblyInfo, XafTypesInfo.Instance.FindTypeInfo(typeof(XafApplication)), false)).CreateInstance(new object[0]) as XafApplication;
            }
            finally
            {
                ReflectionHelper.RemoveResolvePath(assemblyPath);
            }
        }

        private void ReadModulesFromConfig(ApplicationModulesManager manager, XafApplication application)
        {
            Configuration config;
            if (application is WinApplication)
            {
                config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + CurrentObject.PersistentApplication.ExecutableName);
            }
            else
            {
                var mapping = new WebConfigurationFileMap();
                mapping.VirtualDirectories.Add("/Dummy", new VirtualDirectoryMapping(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, true));
                config = WebConfigurationManager.OpenMappedWebConfiguration(mapping, "/Dummy");
            }

            if (config.AppSettings.Settings["Modules"] != null)
            {
                manager.AddModuleFromAssemblies(config.AppSettings.Settings["Modules"].Value.Split(';'));
            }
        }

        #endregion
    }
}