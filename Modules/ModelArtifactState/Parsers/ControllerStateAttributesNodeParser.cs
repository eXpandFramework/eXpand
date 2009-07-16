using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.ExpressApp.ModelArtifactState.NodeWrappers;
using eXpand.ExpressApp.Security.Attributes;
using eXpand.ExpressApp.Security.Parsers;

namespace eXpand.ExpressApp.ModelArtifactState.Parsers
{
    internal class ControllerStateAttributesNodeParser : StateAttributesNodeParser
    {
        protected override StateRuleAttribute GetActivationRuleAttribute(DictionaryNode childNode, ViewType viewType,XafApplication xafApplication)
        {
            var dictionaryActivationNodeWrapper = new DictionaryControllerStateNodeWrapper(childNode, xafApplication.Modules[0].ModuleManager.ControllersManager);

            return new ControllerStateRuleAttribute(dictionaryActivationNodeWrapper.ControllerType, dictionaryActivationNodeWrapper.Nesting,
                                                         dictionaryActivationNodeWrapper.NormalCriteria,
                                                         dictionaryActivationNodeWrapper.EmptyCriteria, viewType, dictionaryActivationNodeWrapper.Module);
        }
    }
}