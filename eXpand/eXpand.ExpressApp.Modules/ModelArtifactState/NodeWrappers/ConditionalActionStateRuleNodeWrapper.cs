using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class ConditionalActionStateRuleNodeWrapper:ConditionalArtifactStateNodeWrapper
    {
        public const string NodeNameAttribute = "ConditionalActionState";

        public ConditionalActionStateRuleNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) { }
        public ConditionalActionStateRuleNodeWrapper(DictionaryNode conditionalartifactStateNode) : base(conditionalartifactStateNode) { }


        protected override string NodeName
        {
            get { return ActionStateRuleNodeWrapper.NodeNameAttribute; }
        }
        public override List<ArtifactStateRuleNodeWrapper> Rules
        {
            get
            {
                return GetRules<ActionStateRuleNodeWrapper>();
            }
        }



        public override ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
        {
            var wrapper = base.AddRule<TArtifactStateRuleNodeWrapper>(artifactStateRuleAttribute, typeInfo);
            var nodeWrapper = ((ActionStateRuleNodeWrapper) wrapper);
            nodeWrapper.ActionId= ((ActionStateRuleAttribute) artifactStateRuleAttribute).ActionId;
            return wrapper;
        }


    }
}