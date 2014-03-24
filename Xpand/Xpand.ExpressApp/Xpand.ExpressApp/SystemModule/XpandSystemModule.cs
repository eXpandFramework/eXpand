using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Core.ReadOnlyParameters;
using Xpand.ExpressApp.MessageBox;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.ExpressApp.TranslatorProviders;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;
using Fasterflect;

namespace Xpand.ExpressApp.SystemModule {

    [ToolboxItem(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSystemModule : XpandModuleBase,ISequenceGeneratorUser,IModelXmlConverter,IDashboardUser {
        public XpandSystemModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        }

        static XpandSystemModule() {
            ParametersFactory.RegisterParameter(new MonthAgoParameter());
            TranslatorProvider.RegisterProvider(new GoogleTranslatorProvider());
            if (!InterfaceBuilder.RuntimeMode)
                new XpandXpoTypeInfoSource((TypesInfo)XafTypesInfo.Instance).AssignAsPersistentEntityStore();
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>(base.GetDeclaredExportedTypes()) { typeof(MessageBoxTextMessage) };
        }

        public override void Setup(XafApplication application) {
            if (RuntimeMode)
                XafTypesInfo.SetPersistentEntityStore(new XpandXpoTypeInfoSource((TypesInfo) application.TypesInfo));
            base.Setup(application);
            if (RuntimeMode) {
                application.SetupComplete +=
                    (sender, args) => RuntimeMemberBuilder.CreateRuntimeMembers(application.Model);
                application.CustomProcessShortcut+=ApplicationOnCustomProcessShortcut;
                application.ListViewCreating+=ApplicationOnListViewCreating;
                application.DetailViewCreating+=ApplicationOnDetailViewCreating;
                application.CreateCustomCollectionSource += LinqCollectionSourceHelper.CreateCustomCollectionSource;
                application.LoggedOn += (sender, args) => RuntimeMemberBuilder.CreateRuntimeMembers(application.Model);
            }
        }

        void ApplicationOnCustomProcessShortcut(object sender, CustomProcessShortcutEventArgs args) {
            new ViewShortCutProccesor(Application).Proccess(args);
        }

        void ApplicationOnDetailViewCreating(object sender, DetailViewCreatingEventArgs args) {
            if (!(args.View is XpandDetailView))
                args.View = ViewFactory.CreateDetailView((XafApplication) sender, args.ViewID, args.ObjectSpace, args.Obj, args.IsRoot);
        }

        void ApplicationOnListViewCreating(object sender, ListViewCreatingEventArgs args) {
            if (!(args.View is XpandListView))
                args.View = ViewFactory.CreateListView((XafApplication) sender, args.ViewID, args.CollectionSource, args.IsRoot);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (RuntimeMode) {
                foreach (var persistentType in typesInfo.PersistentTypes) {
                    CreateAttributeRegistratorAttributes(persistentType);
                }
            }
            if (Application != null && Application.Security != null) {
                CreatePessimisticLockingField(typesInfo);
            }
        }
        void CreatePessimisticLockingField(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                var optimisticLockingAttribute = typeInfo.FindAttribute<OptimisticLockingAttribute>();
                if (optimisticLockingAttribute == null || optimisticLockingAttribute.Enabled)
                    typeInfo.AddAttribute(new OptimisticLockingAttribute(false));
                var memberInfo = typeInfo.FindMember(PessimisticLockingViewController.LockedUser);
                if (memberInfo == null) {
                    memberInfo = typeInfo.CreateMember(PessimisticLockingViewController.LockedUser, Application.Security.UserType);
                    memberInfo.AddAttribute(new BrowsableAttribute(false));
                }
            }
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.TimePropertyEditor, typeof(DateTime), false)));
        }

        void CreateAttributeRegistratorAttributes(ITypeInfo persistentType) {
            IEnumerable<Attribute> attributes = GetAttributes(persistentType);
            foreach (var attribute in attributes) {
                persistentType.AddAttribute(attribute);
            }
        }

        IEnumerable<Attribute> GetAttributes(ITypeInfo type) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(AttributeRegistrator)).Descendants.Select(typeInfo => (AttributeRegistrator)typeInfo.Type.CreateInstance()).SelectMany(registrator => registrator.GetAttributes(type));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelListViewLinqNodesGeneratorUpdater());
            updaters.Add(new ModelListViewLinqColumnsNodesGeneratorUpdater());
            updaters.Add(new ModelMemberGeneratorUpdater());
            updaters.Add(new XpandNavigationItemNodeUpdater());
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewPropertyPathFilters>();
            extenders.Add<IModelClass, IModelClassLoadWhenFiltered>();
            extenders.Add<IModelListView, IModelListViewLoadWhenFiltered>();
            extenders.Add<IModelListView, IModelListViewLinq>();
            extenders.Add<IModelClass, IModelClassProccessViewShortcuts>();
            extenders.Add<IModelDetailView, IModelDetailViewProccessViewShortcuts>();
            extenders.Add<IModelOptions, IModelOptionsClientSideSecurity>();
            extenders.Add<IModelOptions, IModelOptionMemberPersistent>();
            extenders.Add<IModelStaticText, IModelStaticTextEx>();
            extenders.Add<IModelClass, IModelClassPersistModelModifications>();
            extenders.Add<IModelObjectView, IModelObjectViewPersistModelModifications>();
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (string.CompareOrdinal("RuntimeCalculatedColumn", parameters.XmlNodeName)==0){
                parameters.NodeType = typeof (IModelColumn);
                string name = parameters.Values["Id"];
                if (parameters.Values.ContainsKey("CalcPropertyName")) {
                    name = parameters.Values["CalcPropertyName"];
                    parameters.Values.Remove("CalcPropertyName");
                }
                parameters.Values.Add("PropertyName",name);
            }
            if (typeof(IModelListViewLoadWhenFiltered).IsAssignableFrom(parameters.NodeType) && parameters.Values.ContainsKey("LoadWhenFiltered")) {
                if (parameters.Values["LoadWhenFiltered"] == "True"){
                    parameters.Values["LoadWhenFiltered"] = LoadWhenFiltered.FilterAndCriteria.ToString();
                }
                else if (parameters.Values["LoadWhenFiltered"] == "False"){
                    parameters.Values["LoadWhenFiltered"] = LoadWhenFiltered.No.ToString();
                }
            }
        }
    }


    public interface ITestSupport {
        bool IsTesting { get; set; }
    }
}