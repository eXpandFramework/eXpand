using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Core.ReadOnlyParameters;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.SystemModule {

    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XafApplication), "Resources.SystemModule.ico")]
    public sealed class XpandSystemModule : XpandModuleBase, IModelXmlConverter {
        static XpandSystemModule() {
            ParametersFactory.RegisterParameter(new MonthAgoParameter());
        }

        public XpandSystemModule() {
            AdditionalBusinessClasses.Add(typeof(XpoSequencer));
            AdditionalBusinessClasses.Add(typeof(XpoServerId));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (ModelApplicationCreator==null) {
                foreach (var persistentType in typesInfo.PersistentTypes) {
                    IEnumerable<Attribute> attributes = GetAttributes(persistentType);
                    foreach (var attribute in attributes) {
                        persistentType.AddAttribute(attribute);
                    }
                }
            }
        }
        IEnumerable<Attribute> GetAttributes(ITypeInfo type) {
            return XafTypesInfo.Instance.FindTypeInfo(typeof(AttributeRegistrator)).Descendants.Select(typeInfo => (AttributeRegistrator)ReflectionHelper.CreateObject(typeInfo.Type)).SelectMany(registrator => registrator.GetAttributes(type));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.CreateCustomCollectionSource += LinqCollectionSourceHelper.CreateCustomCollectionSource;
            application.SetupComplete +=
                (sender, args) =>
                RuntimeMemberBuilder.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
            application.LoggedOn +=
                (sender, args) =>
                RuntimeMemberBuilder.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
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
        }

        public void ConvertXml(ConvertXmlParameters parameters) {
            if (typeof(IModelMember).IsAssignableFrom(parameters.NodeType) ) {
                if (parameters.Values.ContainsKey("IsRuntimeMember") && parameters.XmlNodeName == "Member"&&parameters.Values["IsRuntimeMember"].ToLower()=="true")
                    parameters.NodeType = typeof (IModelRuntimeMember);
            }
            if (parameters.XmlNodeName=="CalculatedRuntimeMember") {
                parameters.NodeType = typeof (IModelRuntimeCalculatedMember);
            }
        }
    }

}