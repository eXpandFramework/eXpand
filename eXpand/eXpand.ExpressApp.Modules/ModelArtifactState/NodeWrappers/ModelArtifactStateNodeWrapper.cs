using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using System.Linq;

namespace eXpand.ExpressApp.ModelArtifactState.NodeWrappers{
    public class ModelArtifactStateNodeWrapper : NodeWrapper{
        public const string NodeNameAttribute = "ModelArtifact";

        private ConditionalActionStateRuleNodeWrapper conditionalActionStateRuleNodeWrapper;

        private ConditionalControllerStateRuleNodeWrapper conditionalControllerStateRuleNodeWrapper;

        public ModelArtifactStateNodeWrapper(DictionaryNode node) : base(node){
        }

        public ConditionalActionStateRuleNodeWrapper ConditionalActionStateRuleNodeWrapper{
            get {
                return conditionalActionStateRuleNodeWrapper ??new ConditionalActionStateRuleNodeWrapper(Node.GetChildNode(ConditionalActionStateRuleNodeWrapper.NodeNameAttribute));
            }
            set { conditionalActionStateRuleNodeWrapper = value; }
        }

        public ConditionalControllerStateRuleNodeWrapper ConditionalControllerStateRuleNodeWrapper{
            get {
                return conditionalControllerStateRuleNodeWrapper ?? new ConditionalControllerStateRuleNodeWrapper(
                                                                        Node.GetChildNode(ConditionalControllerStateRuleNodeWrapper.NodeNameAttribute));
            }
            set { conditionalControllerStateRuleNodeWrapper = value; }
        }

        public List<ArtifactStateRuleNodeWrapper> Rules{
            get{
                var rules = new List<ArtifactStateRuleNodeWrapper>();
                addRules(ConditionalControllerStateRuleNodeWrapper.NodeNameAttribute, rules);
                addRules(ConditionalActionStateRuleNodeWrapper.NodeNameAttribute, rules);
                return rules;
            }
        }

        public virtual ArtifactStateRuleNodeWrapper AddRule<TArtifactStateRuleNodeWrapper>(
            ArtifactStateRuleAttribute artifactStateRuleAttribute, ITypeInfo typeInfo)
            where TArtifactStateRuleNodeWrapper : ArtifactStateRuleNodeWrapper{
            ArtifactStateRuleNodeWrapper wrapper;
            wrapper = artifactStateRuleAttribute is ActionStateRuleAttribute
                          ? ConditionalActionStateRuleNodeWrapper.AddRule<ActionStateRuleNodeWrapper>(
                                artifactStateRuleAttribute, typeInfo)
                          : ConditionalControllerStateRuleNodeWrapper.AddRule<ControllerStateRuleNodeWrapper>(
                                artifactStateRuleAttribute, typeInfo);


            return wrapper;
        }

        private void addRules(string controllerstaterule, List<ArtifactStateRuleNodeWrapper> rules){
            DictionaryNode rulesNode = Node.FindChildNode(controllerstaterule);
            if (rulesNode != null){
                rules.AddRange(rulesNode.ChildNodes.GetOrderedByIndex().Select(
                        node =>(ArtifactStateRuleNodeWrapper)Activator.CreateInstance(typeof (ArtifactStateRuleNodeWrapper), new[] {node})));
            }
            return;
        }

        public IEnumerable<ArtifactStateRuleNodeWrapper> FindRules(ITypeInfo typeInfo) {
            return typeInfo != null ? ConditionalControllerStateRuleNodeWrapper.FindRules(typeInfo).Concat(ConditionalActionStateRuleNodeWrapper.FindRules(typeInfo)) : null;
        }
    }
}