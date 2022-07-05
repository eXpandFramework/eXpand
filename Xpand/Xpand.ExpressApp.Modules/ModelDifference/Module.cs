using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.Controllers;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DatabaseUpdate;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Validation;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.ExpressApp.ModelDifference.NodeUpdaters;
using Xpand.ExpressApp.ModelDifference.Security.Improved;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.Security;
using Xpand.XAF.Modules.ModelViewInheritance;
using Updater = Xpand.ExpressApp.ModelDifference.DatabaseUpdate.Updater;


namespace Xpand.ExpressApp.ModelDifference {
    public interface IModelOptionsModelDifference:IModelNode{
	    [Category(ModelDifferenceModule.ModelDifferenceCategory)]
//		[ModelBrowsable(typeof(DesignerOnlyCalculator))]
		ModelUpdateMode ModelUpdateMode{ get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [DefaultValue("Autocreated at {0} For {1}")]
        string UserModelDifferenceObjectSubjectTemplate { get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [CriteriaOptions("MDOTypeInfo")]
        [Description("The MDO object that fits in this crierion it will be updated with the model.xafml contents on each application restart")]
        string ModelToUpdateFromFileCriteria { get; set; }
        [Browsable(false)]
        ITypeInfo  MDOTypeInfo { get; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [CriteriaOptions("MDOTypeInfo")]
        [Description("The MDO object that fits in this criterion it will be updated with the Model.Desktop.xafml contents on each application restart")]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        string ModelToUpdateFromDesktopFileCriteria { get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [CriteriaOptions("MDOTypeInfo")]
        [Description("The MDO object that fits in this criterion it will be updated with the Model.Tablet.xafml contents on each application restart")]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        string ModelToUpdateFromTabletFileCriteria { get; set; }
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [CriteriaOptions("MDOTypeInfo")]
        [Description("The MDO object that fits in this criterion it will be updated with the Model.Mobile.xafml contents on each application restart")]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        string ModelToUpdateFromMobileFileCriteria { get; set; }
        [DefaultValue(true)]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        bool UserMobileModels { get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [DefaultValue(true)]
        [ModelBrowsable(typeof(WebOnlyVisibilityCalculator))]
        bool ApplicationMobileModels { get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        string ApplicationTitle { get; set; }
    }

	public enum ModelUpdateMode{
		Always,
		Never
	}

	[DomainLogic(typeof(IModelOptionsModelDifference))]
    public class ModelOptionsModelDifferenceLogic {

        public static ITypeInfo Get_MDOTypeInfo(IModelOptionsModelDifference modelDifference){
            return modelDifference.Application.BOModel.GetClass(typeof(ModelDifferenceObject)).TypeInfo;
        }

        public static string Get_ModelToUpdateFromFileCriteria(IModelOptionsModelDifference modelDifference) {
            return GetCriteria();
        }
        public static string Get_ModelToUpdateFromDesktopCriteria(IModelOptionsModelDifference modelDifference) {
            return GetCriteria();
        }
        public static string Get_ModelToUpdateFromTabletCriteria(IModelOptionsModelDifference modelDifference) {
            return GetCriteria();
        }

        public static string Get_ModelToUpdateFromMobileFileCriteria(IModelOptionsModelDifference modelDifference) {
            return GetCriteria();
        }

        private static string GetCriteria() {
            return new XPQuery<ModelDifferenceObject>(XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary)
                .TransformExpression(o => o.DifferenceType == DifferenceType.Model && o.Name.Contains("Create a new MDO object") && !o.Disabled).ToString();
        }
    }
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ModelDifferenceModule : XpandModuleBase, ISecurityModuleUser{
        public const string ModelDifferenceCategory = "eXpand.ModelDifference";
        public ModelDifferenceModule() {
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            RequiredModuleTypes.Add(typeof(ExpressApp.Security.XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ViewVariants.XpandViewVariantsModule));
            RequiredModuleTypes.Add(typeof(XAF.Modules.CloneModelView.CloneModelViewModule));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsModelDifference>();
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            var types = new[]{
                typeof(CloneObjectViewController), typeof(CombineActiveUserDiffsWithLastLayerController),
                typeof(MergeDifferencesController),typeof(ModelDifferenceObjectsRuntimeMembersController),typeof(ReloadApplicationModelController),
                typeof(PopulateAspectsController)
            };
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(types));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!RuntimeMode) {
                CreateWeaklyTypedCollection(typesInfo, typeof(UserModelDifferenceObject), "Users");
                CreateWeaklyTypedCollection(typesInfo, typeof(RoleModelDifferenceObject), "Roles");
            } else if (Application.CanBuildSecurityObjects()) {
                BuildSecuritySystemObjects();
            }
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            this.AddSecurityObjectsToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ModelDifference");
            ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(XmlContentCodeRule), typeof(IRuleBaseProperties));
        }

        XpoUserModelDictionaryDifferenceStore _userModelDictionaryDifferenceStore;

        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        internal void OnCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e) {
            var handler = CreateCustomModelDifferenceStore;
            handler?.Invoke(this, e);
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SettingUp += ApplicationOnSettingUp;
                application.SetupComplete+=ApplicationOnSetupComplete;
            }
            Application.UserDifferencesLoaded += OnUserDifferencesLoaded;
            Application.CreateCustomUserModelDifferenceStore += ApplicationOnCreateCustomUserModelDifferenceStore;
            AddToAdditionalExportedTypes(typeof(ModelDifferenceObject).Namespace, GetType().Assembly);
            if (application is ServerApplication serverApplication) {
                if ((serverApplication is IModelDifferenceServerModels serverModels)) {
                    serverApplication.SetupComplete += (sender, args) => {
                        serverApplication.TypesInfo.RegisterEntity(typeof(XPObjectType));
                        using (var objectSpace = application.CreateObjectSpace(typeof(ModelDifferenceObject))){
                            var modelDifferenceObjects = objectSpace.GetObjectsQuery<ModelDifferenceObject>();
                            foreach (var modelDifferenceObject in serverModels.Where(modelDifferenceObjects)){
                                modelDifferenceObject.GetModel((ModelApplicationBase)application.Model);
                            }
                        }
                        LoadModels();

                    };
                    return;
                }
                throw new NotSupportedException($"Your {serverApplication.GetType().FullName} must implement the {nameof(IModelDifferenceServerModels)}");
            }
        }

        

        void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, DevExpress.ExpressApp.CreateCustomModelDifferenceStoreEventArgs createCustomModelDifferenceStoreEventArgs) {
            createCustomModelDifferenceStoreEventArgs.Handled = true;
            _userModelDictionaryDifferenceStore =_userModelDictionaryDifferenceStore?? new XpoUserModelDictionaryDifferenceStore(Application);
            createCustomModelDifferenceStoreEventArgs.Store = _userModelDictionaryDifferenceStore;
        }

        void OnUserDifferencesLoaded(object sender, EventArgs eventArgs) {
            LoadModels();
        }

        public void LoadModels() {
            var model = (ModelApplicationBase)Application.Model;
            LoadApplicationModels(model);
            if (Application.Security is ISecurityComplex)
                _userModelDictionaryDifferenceStore?.Load();
            RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
        }

        void LoadApplicationModels(ModelApplicationBase model) {
            var userDiffLayers = new List<ModelApplicationBase>();
            while (model.LastLayer != null && model.LastLayer.Id != "After Setup"){
                userDiffLayers.Add(model.LastLayer);
                ModelApplicationHelper.RemoveLayer(model);
            }
            if (model.LastLayer == null)
                throw new ArgumentException("Model.LastLayer null");
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled){
                new XpoModelDictionaryDifferenceStore(Application, customModelDifferenceStoreEventArgs.ExtraDiffStores).Load(model);
            }
            userDiffLayers.Reverse();
            foreach (var layer in userDiffLayers){
                ModelApplicationHelper.AddLayer((ModelApplicationBase)Application.Model, layer);
            }
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            if (((XafApplication)sender).Security is SecurityStrategy securityStrategy) {
                (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
            }
        }

        void ApplicationOnSettingUp(object sender, EventArgs eventArgs) {
            BuildSecuritySystemObjects();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB){
            yield return new Updater(objectSpace, versionFromDB);
            yield return new DeviceCategoryUpdater(objectSpace, versionFromDB);
        }

        void BuildSecuritySystemObjects() {
            var dynamicSecuritySystemObjects = new DynamicSecuritySystemObjects(Application);
            var xpMemberInfos = dynamicSecuritySystemObjects.BuildUser(typeof(UserModelDifferenceObject), "UserUsers_UserModelDifferenceObjectUserModelDifferenceObjects", "UserModelDifferenceObjects", "Users");
            dynamicSecuritySystemObjects.HideInDetailView(xpMemberInfos, "UserModelDifferenceObjects");
            xpMemberInfos = dynamicSecuritySystemObjects.BuildRole(typeof(RoleModelDifferenceObject), "RoleRoles_RoleModelDifferenceObjectRoleModelDifferenceObjects", "RoleModelDifferenceObjects", "Roles");
            dynamicSecuritySystemObjects.HideInDetailView(xpMemberInfos, "RoleModelDifferenceObjects");
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs e) {
            var keyValuePair = new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(ModelCombinePermissionRequest), e.Permissions.GetProcessor<ModelCombineRequestProcessor>());
            e.Processors.Add(keyValuePair);
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new BOModelNodesUpdater());
            updaters.Add(new BOModelMemberNodesUpdater());
        }
    }
}