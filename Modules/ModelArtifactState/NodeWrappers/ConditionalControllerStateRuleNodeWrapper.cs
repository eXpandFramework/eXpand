using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ModelArtifactState.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers
{
    public class ConditionalControllerStateRuleNodeWrapper : ConditionalArtifactStateNodeWrapper
    {
        public const string NodeNameAttribute = "ConditionalControllerState";

        public ConditionalControllerStateRuleNodeWrapper() : this(new DictionaryNode(NodeNameAttribute)) { }
        public ConditionalControllerStateRuleNodeWrapper(DictionaryNode conditionalartifactStateNode) : base(conditionalartifactStateNode) { }


        protected override string NodeName
        {
            get { return ControllerStateRuleNodeWrapper.NodeNameAttribute; }
        }

        public override ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
        {
            var stateRuleNodeWrapper = base.AddRule < TArtifactStateRuleNodeWrapper>(artifactStateRuleAttribute, typeInfo);
            ((ControllerStateRuleNodeWrapper) stateRuleNodeWrapper).ControllerType =
                ((ControllerStateRuleAttribute) artifactStateRuleAttribute).ControllerType.FullName;
            return stateRuleNodeWrapper;
        }

        public override List<ArtifactStateRuleNodeWrapper> Rules{
            get {
                return GetRules<ControllerStateRuleNodeWrapper>();
            }
        }
    }
}