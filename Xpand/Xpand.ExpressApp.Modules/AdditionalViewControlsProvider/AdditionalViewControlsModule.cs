using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider
{
    public sealed class AdditionalViewControlsModule : LogicModuleBase<IAdditionalViewControlsRule,AdditionalViewControlsRule>,IModelExtender
    {
        #region IModelExtender Members
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelApplication, IModelApplicationAdditionalViewControls>();
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new DetailViewItemDescriptor(new DetailViewItemRegistration(typeof(IModelAdditionalViewControlsItem), typeof(AdditionalViewControlsItem), true)));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters)
        {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new AdditionalViewControlsDefaultGroupContextNodeUpdater());
            updaters.Add(new AdditionalViewControlsRulesNodeUpdater());
            updaters.Add(new AdditionalViewControlsDefaultContextNodeUpdater());
        }
        #endregion
        protected override IModelLogic GetModelLogic(IModelApplication applicationModel) {
            return ((IModelApplicationAdditionalViewControls)applicationModel).AdditionalViewControls;
        }
    }
}
