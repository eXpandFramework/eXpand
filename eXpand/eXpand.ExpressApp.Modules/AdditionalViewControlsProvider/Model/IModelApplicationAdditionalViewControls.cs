using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Model {
    public interface IModelApplicationAdditionalViewControls : IModelNode {
        [Description("Provides access to the AdditionalViewControls node.")]
        IModelLogicAdditionalViewControls AdditionalViewControls { get; }
    }
}