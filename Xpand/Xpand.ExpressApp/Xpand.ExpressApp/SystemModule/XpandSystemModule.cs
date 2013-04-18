using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Core.ReadOnlyParameters;
using Xpand.ExpressApp.MessageBox;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.ExpressApp.TranslatorProviders;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.SystemModule {

    [ToolboxItem(false)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XafApplication), "Resources.SystemModule.ico")]
    public sealed class XpandSystemModule : XpandModuleBase, IModelXmlConverter, IModelNodeUpdater<IModelMemberEx> {
        public XpandSystemModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
        }
        public void UpdateNode(IModelMemberEx node, IModelApplication application) {
            node.IsCustom = false;
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator) {
            base.AddModelNodeUpdaters(updaterRegistrator);
            updaterRegistrator.AddUpdater(this);
        }

        static XpandSystemModule() {
            ParametersFactory.RegisterParameter(new MonthAgoParameter());
            TranslatorProvider.RegisterProvider(new GoogleTranslatorProvider());
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>(base.GetDeclaredExportedTypes()) { typeof(MessageBoxTextMessage) };
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                AddToAdditionalExportedTypes(new[] { "Xpand.Persistent.BaseImpl.SequenceObject" });
                SequenceObjectType = AdditionalExportedTypes.Single(type => type.FullName == "Xpand.Persistent.BaseImpl.SequenceObject");
                InitializeSequenceGenerator();
            }
        }

        public override void Setup(XafApplication application) {
            if (RuntimeMode && (XafTypesInfo.PersistentEntityStore is XpandXpoTypeInfoSource) && !((ITestSupport)application).IsTesting)
                XafTypesInfo.SetPersistentEntityStore(new XpandXpoTypeInfoSource((TypesInfo)TypesInfo));
            base.Setup(application);
            application.CreateCustomCollectionSource += LinqCollectionSourceHelper.CreateCustomCollectionSource;
            application.SetupComplete +=
                (sender, args) =>
                RuntimeMemberBuilder.AddFields(application.Model);
            application.LoggedOn +=
                (sender, args) =>
                RuntimeMemberBuilder.AddFields(application.Model);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type SequenceObjectType { get; set; }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (ModelApplicationCreator == null) {
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

        void CreateAttributeRegistratorAttributes(ITypeInfo persistentType) {
            IEnumerable<Attribute> attributes = GetAttributes(persistentType);
            foreach (var attribute in attributes) {
                persistentType.AddAttribute(attribute);
            }
        }

        public void InitializeSequenceGenerator() {
            if (SequenceObjectType == null)
                return;
            try {
                if (!typeof(ISequenceObject).IsAssignableFrom(SequenceObjectType))
                    throw new TypeLoadException("Please make sure XPand.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
                if (Application != null && Application.ObjectSpaceProvider != null && !(Application.ObjectSpaceProvider is DataServerObjectSpaceProvider)) {
                    var connectionString = ((IXafApplication)Application).RaiseEstablishingConnection();
                    SequenceGenerator.Initialize(connectionString, SequenceObjectType);
                }
            } catch (Exception e) {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }



        IEnumerable<Attribute> GetAttributes(ITypeInfo type) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(AttributeRegistrator)).Descendants.Select(typeInfo => (AttributeRegistrator)ReflectionHelper.CreateObject(typeInfo.Type)).SelectMany(registrator => registrator.GetAttributes(type));
        }



        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelListViewLinqNodesGeneratorUpdater());
            updaters.Add(new ModelListViewLinqColumnsNodesGeneratorUpdater());
            updaters.Add(new ModelMemberGeneratorUpdater());
            updaters.Add(new ModelViewClonerUpdater());
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
            extenders.Add<IModelMember, IModelMemberEx>();
            extenders.Add<IModelOptions, IModelOptionsClientSideSecurity>();
            extenders.Add<IModelStaticText, IModelStaticTextEx>();
        }

        public void ConvertXml(ConvertXmlParameters parameters) {
            if (typeof(IModelMember).IsAssignableFrom(parameters.NodeType)) {
                if (parameters.Values.ContainsKey("IsRuntimeMember") && parameters.XmlNodeName == "Member" && parameters.Values["IsRuntimeMember"].ToLower() == "true")
                    parameters.NodeType = typeof(IModelRuntimeMember);
            }
            if (parameters.XmlNodeName == "CalculatedRuntimeMember") {
                parameters.NodeType = typeof(IModelRuntimeCalculatedMember);
            }
        }

    }

    public interface ITestSupport {
        bool IsTesting { get; set; }
    }
}