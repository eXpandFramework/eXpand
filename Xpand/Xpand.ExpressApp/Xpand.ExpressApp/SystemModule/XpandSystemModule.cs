using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.ExpressApp;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Core.ReadOnlyParameters;

namespace Xpand.ExpressApp.SystemModule
{
    public interface IModelBOModelRuntimeMember : IModelNode
    {
        [Category("eXpand")]
        bool IsRuntimeMember { get; set; }
    }

    [ToolboxItem(true)]
    [Description("Includes Controllers that represent basic features for XAF applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XafApplication), "Resources.SystemModule.ico")]
    public sealed class XpandSystemModule : XpandModuleBase
    {
        static XpandSystemModule()
        {
            DevExpress.Persistent.Base.ParametersFactory.RegisterParameter(new MonthAgoParameter());
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomCollectionSource += LinqCollectionSourceHelper.CreateCustomCollectionSource;
            application.SetupComplete +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
            application.LoggedOn +=
                (sender, args) =>
                DictionaryHelper.AddFields(application.Model, application.ObjectSpaceProvider.XPDictionary);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelListViewLinqNodesGeneratorUpdater());
            updaters.Add(new ModelListViewLinqColumnsNodesGeneratorUpdater());
            updaters.Add(new ModelViewClonerUpdater());
            updaters.Add(new XpandNavigationItemNodeUpdater());
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewPropertyPathFilters>();
            extenders.Add<IModelClass, IModelClassLoadWhenFiltered>();
            extenders.Add<IModelListView, IModelListViewLoadWhenFiltered>();
            extenders.Add<IModelListView, IModelListViewLinq>();
            extenders.Add<IModelMember, IModelBOModelRuntimeMember>();
        }
    }

}