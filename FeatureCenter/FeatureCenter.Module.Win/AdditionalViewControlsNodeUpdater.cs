using System.Collections.Generic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace FeatureCenter.Module.Win {
    public class AdditionalViewControlsNodeUpdater :
        ModelLogicRulesNodesGenerator<IModelApplicationAdditionalViewControls, IModelAdditionalViewControlsRule> {
        public override void UpdateNode(IEnumerable<IModelAdditionalViewControlsRule> rules) {
            foreach (IModelAdditionalViewControlsRule modelAdditionalViewControlsRule in rules) {
                if (modelAdditionalViewControlsRule.Id.StartsWith(Module.Captions.Header))
                    modelAdditionalViewControlsRule.ControlType = typeof (ApplicationCaption);
                else if (modelAdditionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage)) 
                    modelAdditionalViewControlsRule.ControlType = typeof (FooterMessagePanel);
            }
        }
    }
}