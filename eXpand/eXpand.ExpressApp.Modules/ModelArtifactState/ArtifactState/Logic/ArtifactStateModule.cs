using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic {
    public abstract class ArtifactStateModule<TArtifactStateRule> :
        ConditionalLogicRuleProviderModuleBase<TArtifactStateRule> where TArtifactStateRule : IConditionalLogicRule {
        protected override void ModifySchemaAttributes(AttibuteCreatedEventArgs args) {
            base.ModifySchemaAttributes(args);
            if (args.Attribute.IndexOf("Module") > -1)
                args.AddTag(@"RefNodeName=""/Application/Modules/*""");
        }
    }
}